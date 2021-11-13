
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

using MsgBoxEx;
using WpfControls = System.Windows.Controls;

using static mpvnet.Native;
using static mpvnet.Global;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public ElementHost CommandPaletteHost { get; set; }
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd { get; set; }
   
        new WpfControls.ContextMenu ContextMenu { get; set; }
        AutoResetEvent MenuAutoResetEvent { get; } = new AutoResetEvent(false);
        Point LastCursorPosition;
        Taskbar Taskbar;

        int LastCursorChanged;
        int LastCycleFullscreen;
        int TaskbarButtonCreatedMessage;

        bool WasMaximized;
        bool WasShown;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                Instance = this;
                Hwnd = Handle;
                Core.Init();

                Core.Shutdown += Core_Shutdown;
                Core.VideoSizeChanged += Core_VideoSizeChanged;
                Core.ScaleWindow += Core_ScaleWindow;
                Core.WindowScale += Core_WindowScale;
                Core.FileLoaded += Core_FileLoaded;
                Core.Idle += Core_Idle;
                Core.Seek += () => UpdateProgressBar();

                Core.ObserveProperty("window-maximized", PropChangeWindowMaximized);
                Core.ObserveProperty("window-minimized", PropChangeWindowMinimized);

                Core.ObservePropertyBool("border", PropChangeBorder);
                Core.ObservePropertyBool("fullscreen", PropChangeFullscreen);
                Core.ObservePropertyBool("keepaspect-window", value => Core.KeepaspectWindow = value);
                Core.ObservePropertyBool("ontop", PropChangeOnTop);
                Core.ObservePropertyBool("pause", PropChangePause);

                Core.ObservePropertyString("sid", PropChangeSid);
                Core.ObservePropertyString("aid", PropChangeAid);
                Core.ObservePropertyString("vid", PropChangeVid);

                Core.ObservePropertyString("title", PropChangeTitle);

                Core.ObservePropertyInt("edition", PropChangeEdition);

                Core.ObservePropertyDouble("window-scale", WindowScale);

                if (!IsVulkanOrGpuNext)
                    Core.ProcessCommandLine(false);

                AppDomain.CurrentDomain.UnhandledException += (sender, e) => App.ShowException(e.ExceptionObject);
                Application.ThreadException += (sender, e) => App.ShowException(e.Exception);

                TaskbarButtonCreatedMessage = RegisterWindowMessage("TaskbarButtonCreated");

                if (Core.Screen > -1)
                {
                    int targetIndex = Core.Screen;
                    Screen[] screens = Screen.AllScreens;

                    if (targetIndex < 0)
                        targetIndex = 0;

                    if (targetIndex > screens.Length - 1)
                        targetIndex = screens.Length - 1;

                    Screen screen = screens[Array.IndexOf(screens, screens[targetIndex])];
                    Rectangle target = screen.Bounds;
                    Left = target.X + (target.Width - Width) / 2;
                    Top = target.Y + (target.Height - Height) / 2;
                }

                if (!Core.Border)
                    FormBorderStyle = FormBorderStyle.None;

                Point pos = App.Settings.WindowPosition;

                if ((pos.X != 0 || pos.Y != 0) && App.RememberWindowPosition)
                {
                    Left = pos.X - Width / 2;
                    Top = pos.Y - Height / 2;

                    Point location = App.Settings.WindowLocation;

                    if (location.X == -1) Left = pos.X;
                    if (location.X ==  1) Left = pos.X - Width;
                    if (location.Y == -1) Top = pos.Y;
                    if (location.Y ==  1) Top = pos.Y - Height;
                }

                if (Core.WindowMaximized)
                {
                    SetFormPosAndSize(true);
                    WindowState = FormWindowState.Maximized;
                }

                if (Core.WindowMinimized)
                {
                    SetFormPosAndSize(true);
                    WindowState = FormWindowState.Minimized;
                }
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }

        void Core_ScaleWindow(float scale) {
            BeginInvoke(new Action(() => {
                int w, h;

                if (KeepSize())
                {
                    w = (int)(ClientSize.Width * scale);
                    h = (int)(ClientSize.Height * scale);
                }
                else
                {
                    w = (int)(ClientSize.Width * scale);
                    h = (int)Math.Ceiling(w * Core.VideoSize.Height / (double)Core.VideoSize.Width);
                }

                SetSize(w, h, Screen.FromControl(this), false);
            }));
        }

        void Core_WindowScale(float scale)
        {
            BeginInvoke(new Action(() => {
                SetSize(
                    (int)(Core.VideoSize.Width * scale),
                    (int)Math.Ceiling(Core.VideoSize.Height * scale),
                    Screen.FromControl(this), false);
                Core.Command($"show-text \"window-scale {scale.ToString(CultureInfo.InvariantCulture)}\"");
            }));
        }

        void WindowScale(double scale)
        {
            if (!WasShown)
                return;

            BeginInvoke(new Action(() => {
                SetSize(
                    (int)(Core.VideoSize.Width * scale),
                    (int)Math.Ceiling(Core.VideoSize.Height * scale),
                    Screen.FromControl(this), false);
            }));
        }

        void Core_Shutdown() => BeginInvoke(new Action(() => Close()));

        void Core_Idle() => SetTitle();

        void CM_Popup(object sender, EventArgs e) => CursorHelp.Show();

        void Core_VideoSizeChanged() => BeginInvoke(new Action(() =>
        {
            if (!KeepSize())
                SetFormPosAndSize();
        }));

        void PropChangeFullscreen(bool value) => BeginInvoke(new Action(() => CycleFullscreen(value)));

        bool IsFullscreen => WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None;

        bool IsCommandPaletteVissible() => CommandPaletteHost != null && CommandPaletteHost.Visible;

        bool IsVulkanOrGpuNext => Core.GPUAPI == "vulkan" || Core.VO == "gpu-next";

        bool KeepSize() => App.StartSize == "session" || App.StartSize == "always";

        bool IsMouseInOSC()
        {
            Point pos = PointToClient(MousePosition);
            float top = 0;

            if (FormBorderStyle == FormBorderStyle.None)
                top = ClientSize.Height * 0.1f;

            return pos.Y > ClientSize.Height * 0.85 || pos.Y < top;
        }

        void UpdateMenu()
        {
            lock (Core.MediaTracks)
            {
                var trackMenuItem = FindMenuItem("Track");

                if (trackMenuItem != null)
                {
                    trackMenuItem.Items.Clear();

                    MediaTrack[] audTracks = Core.MediaTracks.Where(track => track.Type == "a").ToArray();
                    MediaTrack[] subTracks = Core.MediaTracks.Where(track => track.Type == "s").ToArray();
                    MediaTrack[] vidTracks = Core.MediaTracks.Where(track => track.Type == "v").ToArray();
                    MediaTrack[] ediTracks = Core.MediaTracks.Where(track => track.Type == "e").ToArray();

                    foreach (MediaTrack track in vidTracks)
                    {
                        var mi = new WpfControls.MenuItem() { Header = track.Text };
                        mi.Click += (sender, args) => Core.CommandV("set", "vid", track.ID.ToString());
                        mi.IsChecked = Core.VID == track.ID.ToString();
                        trackMenuItem.Items.Add(mi);
                    }

                    if (vidTracks.Length > 0)
                        trackMenuItem.Items.Add(new WpfControls.Separator());

                    foreach (MediaTrack track in audTracks)
                    {
                        var mi = new WpfControls.MenuItem() { Header = track.Text };
                        mi.Click += (sender, args) => Core.CommandV("set", "aid", track.ID.ToString());
                        mi.IsChecked = Core.AID == track.ID.ToString();
                        trackMenuItem.Items.Add(mi);
                    }

                    if (subTracks.Length > 0)
                        trackMenuItem.Items.Add(new WpfControls.Separator());

                    foreach (MediaTrack track in subTracks)
                    {
                        var mi = new WpfControls.MenuItem() { Header = track.Text };
                        mi.Click += (sender, args) => Core.CommandV("set", "sid", track.ID.ToString());
                        mi.IsChecked = Core.SID == track.ID.ToString();
                        trackMenuItem.Items.Add(mi);
                    }

                    if (subTracks.Length > 0)
                    {
                        var mi = new WpfControls.MenuItem() { Header = "S: No subtitles" };
                        mi.Click += (sender, args) => Core.CommandV("set", "sid", "no");
                        mi.IsChecked = Core.SID == "no";
                        trackMenuItem.Items.Add(mi);
                    }

                    if (ediTracks.Length > 0)
                        trackMenuItem.Items.Add(new WpfControls.Separator());

                    foreach (MediaTrack track in ediTracks)
                    {
                        var mi = new WpfControls.MenuItem() { Header = track.Text };
                        mi.Click += (sender, args) => Core.CommandV("set", "edition", track.ID.ToString());
                        mi.IsChecked = Core.Edition == track.ID;
                        trackMenuItem.Items.Add(mi);
                    }
                }
            }

            lock (Core.Chapters)
            {
                var chaptersMenuItem = FindMenuItem("Chapters");

                if (chaptersMenuItem != null)
                {
                    chaptersMenuItem.Items.Clear();

                    foreach (var pair in Core.Chapters)
                    {
                        var chapterMenuItem = new WpfControls.MenuItem() { Header = pair.Key };
                        chapterMenuItem.InputGestureText = TimeSpan.FromSeconds(pair.Value).ToString().Substring(0, 8);
                        chapterMenuItem.Click += (sender, args) => Core.CommandV("seek", pair.Value.ToString(CultureInfo.InvariantCulture), "absolute");
                        chaptersMenuItem.Items.Add(chapterMenuItem);
                    }
                }
            }

            var recentMenuItem = FindMenuItem("Recent");

            if (recentMenuItem != null)
            {
                recentMenuItem.Items.Clear();

                foreach (string path in App.Settings.RecentFiles)
                {
                    var mi = MenuHelp.Add(recentMenuItem.Items, path.ShortPath(100));

                    if (mi != null)
                        mi.Click += (sender, args) =>
                            Core.LoadFiles(new[] { path }, true, ModifierKeys.HasFlag(Keys.Control));
                }

                recentMenuItem.Items.Add(new WpfControls.Separator());
                var clearMenuItem = new WpfControls.MenuItem() { Header = "Clear List" };
                clearMenuItem.Click += (sender, args) => App.Settings.RecentFiles.Clear();
                recentMenuItem.Items.Add(clearMenuItem);
            }

            var titlesMenuItem = FindMenuItem("Titles");

            if (titlesMenuItem != null)
            {
                titlesMenuItem.Items.Clear();

                lock (Core.BluRayTitles)
                {
                    List<(int Index, TimeSpan Len)> items = new List<(int Index, TimeSpan Len)>(); 

                    for (int i = 0; i < Core.BluRayTitles.Count; i++)
                        items.Add((i, Core.BluRayTitles[i]));

                    var titleItems = items.OrderByDescending(item => item.Len)
                        .Take(20).OrderBy(item => item.Index);

                    foreach (var item in titleItems)
                    {
                        if (item.Len != TimeSpan.Zero)
                        {
                            var mi = MenuHelp.Add(titlesMenuItem.Items, $"{item.Len} ({item.Index})");

                            if (mi != null)
                                mi.Click += (sender, args) => Core.SetBluRayTitle(item.Index);
                        }
                    }
                }
            }

            var profilesMenuItem = FindMenuItem("Profile");

            if (profilesMenuItem != null)
            {
                profilesMenuItem.Items.Clear();

                foreach (string profile in ProfileNames)
                {
                    if (!profile.StartsWith("extension."))
                    {
                        var mi = MenuHelp.Add(profilesMenuItem.Items, profile);

                        if (mi != null)
                        {
                            mi.Click += (sender, args) => {
                                Core.CommandV("show-text", profile);
                                Core.CommandV("apply-profile", profile);
                            };
                        }
                    }
                }
            }
        }

        private string[] _ProfileNames;

        public string[] ProfileNames {
            get {
                if (_ProfileNames == null)
                {
                    string[] ignore = { "builtin-pseudo-gui", "encoding", "libmpv", "pseudo-gui", "default" };
                    string profileList = Core.GetPropertyString("profile-list");
                    var json = profileList.FromJson<List<Dictionary<string, object>>>();
                    _ProfileNames = json.Select(i => i["name"].ToString())
                                        .Where(i => !ignore.Contains(i)).ToArray();
                }

                return _ProfileNames;
            }
        }

        public WpfControls.MenuItem FindMenuItem(string text) => FindMenuItem(text, ContextMenu.Items);

        WpfControls.MenuItem FindMenuItem(string text, WpfControls.ItemCollection items)
        {
            foreach (object item in items)
            {
                if (item is WpfControls.MenuItem mi)
                {
                    if (mi.Header.ToString().StartsWithEx(text) && mi.Header.ToString().TrimEx() == text)
                        return mi;

                    if (mi.Items.Count > 0)
                    {
                        WpfControls.MenuItem val = FindMenuItem(text, mi.Items);

                        if (val != null)
                            return val;
                    }
                }
            }
            return null;
        }

        void SetFormPosAndSize(bool force = false, bool checkAutofit = true)
        {
            if (!force)
            {
                if (WindowState != FormWindowState.Normal)
                    return;

                if (Core.Fullscreen)
                {
                    CycleFullscreen(true);
                    return;
                }
            }

            Screen screen = Screen.FromControl(this);
            int autoFitHeight = Convert.ToInt32(screen.WorkingArea.Height * Core.Autofit);

            if (Core.VideoSize.Height == 0 || Core.VideoSize.Width == 0 ||
                Core.VideoSize.Width / (float)Core.VideoSize.Height < App.MinimumAspectRatio)

                Core.VideoSize = new Size((int)(autoFitHeight * (16 / 9f)), autoFitHeight);

            Size videoSize = Core.VideoSize;

            int height = videoSize.Height;
            int width  = videoSize.Width;

            if (App.StartSize == "previous")
                App.StartSize = "height-session";

            if (Core.WasInitialSizeSet)
            {
                if (KeepSize())
                {
                    width = ClientSize.Width;
                    height = ClientSize.Height;
                }
                else if (App.StartSize == "height-always" || App.StartSize == "height-session")
                {
                    height = ClientSize.Height;
                    width = height * videoSize.Width / videoSize.Height;
                }
                else if (App.StartSize == "width-always" || App.StartSize == "width-session")
                {
                    width = ClientSize.Width;
                    height = (int)Math.Ceiling(width * videoSize.Height / (double)videoSize.Width);
                }
            }
            else
            {
                Size windowSize = App.Settings.WindowSize;

                if (App.StartSize == "height-always" && windowSize.Height != 0)
                {
                    height = windowSize.Height;
                    width = height * videoSize.Width / videoSize.Height;
                }
                else if (App.StartSize == "height-session")
                {
                    height = autoFitHeight;
                    width = height * videoSize.Width / videoSize.Height;
                }
                else if(App.StartSize == "width-always" && windowSize.Height != 0)
                {
                    width = windowSize.Width;
                    height = (int)Math.Ceiling(width * videoSize.Height / (double)videoSize.Width);
                }
                else if (App.StartSize == "width-session")
                {
                    width = autoFitHeight / 9 * 16;
                    height = (int)Math.Ceiling(width * videoSize.Height / (double)videoSize.Width);
                }
                else if (KeepSize() && windowSize.Height != 0)
                {
                    height = windowSize.Height;
                    width = windowSize.Width;
                }

                Core.WasInitialSizeSet = true;
            }

            SetSize(width, height, screen, checkAutofit);
        }

        void SetSize(int width, int height, Screen screen, bool checkAutofit = true)
        {
            int maxHeight = screen.WorkingArea.Height - (Height - ClientSize.Height) - 2;
            int maxWidth = screen.WorkingArea.Width - (Width - ClientSize.Width);

            int startWidth = width;
            int startHeight = height;

            if (checkAutofit)
            {
                if (height < maxHeight * Core.AutofitSmaller)
                {
                    height = Convert.ToInt32(maxHeight * Core.AutofitSmaller);
                    width = Convert.ToInt32(height * startWidth / (double)startHeight);
                }

                if (height > maxHeight * Core.AutofitLarger)
                {
                    height = Convert.ToInt32(maxHeight * Core.AutofitLarger);
                    width = Convert.ToInt32(height * startWidth / (double)startHeight);
                }
            }

            if (width > maxWidth)
            {
                width = maxWidth;
                height = (int)Math.Ceiling(width * startHeight / (double)startWidth);
            }

            if (height > maxHeight)
            {
                height = maxHeight;
                width = Convert.ToInt32(height * startWidth / (double)startHeight);
            }

            if (height < maxHeight * 0.1)
            {
                height = Convert.ToInt32(maxHeight * 0.1);
                width = Convert.ToInt32(height * startWidth / (double)startHeight);
            }

            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
            AddWindowBorders(Handle, ref rect, GetDPI(Handle));

            int left = middlePos.X - rect.Width / 2;
            int top = middlePos.Y - rect.Height / 2;

            Rectangle currentRect = new Rectangle(Left, Top, Width, Height);

            if (GetHorizontalLocation(screen) == -1) left = Left;
            if (GetHorizontalLocation(screen) ==  1) left = currentRect.Right - rect.Width;

            if (GetVerticalLocation(screen) == -1) top = Top;
            if (GetVerticalLocation(screen) ==  1) top = currentRect.Bottom - rect.Height;

            Screen[] screens = Screen.AllScreens;

            int minLeft   = screens.Select(val => val.WorkingArea.X).Min();
            int maxRight  = screens.Select(val => val.WorkingArea.Right).Max();
            int minTop    = screens.Select(val => val.WorkingArea.Y).Min();
            int maxBottom = screens.Select(val => val.WorkingArea.Bottom).Max();

            if (left < minLeft)
                left = minLeft;

            if (left + rect.Width > maxRight)
                left = maxRight - rect.Width;

            if (top < minTop)
                top = minTop;

            if (top + rect.Height > maxBottom)
                top = maxBottom - rect.Height;

            uint SWP_NOACTIVATE = 0x0010;
            SetWindowPos(Handle, IntPtr.Zero, left, top, rect.Width, rect.Height, SWP_NOACTIVATE);
        }

        public void CycleFullscreen(bool enabled)
        {
            LastCycleFullscreen = Environment.TickCount;
            Core.Fullscreen = enabled;

            if (enabled)
            {
                if (WindowState != FormWindowState.Maximized || FormBorderStyle != FormBorderStyle.None)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;

                    if (WasMaximized)
                    {
                        Rectangle bounds = Screen.FromControl(this).Bounds;
                        uint SWP_SHOWWINDOW = 0x0040;
                        IntPtr HWND_TOP= IntPtr.Zero;
                        SetWindowPos(Handle, HWND_TOP, bounds.X, bounds.Y, bounds.Width, bounds.Height, SWP_SHOWWINDOW);
                    }
                }
            }
            else
            {
                if (WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None)
                {
                    if (WasMaximized)
                        WindowState = FormWindowState.Maximized;
                    else
                        WindowState = FormWindowState.Normal;

                    if (Core.Border)
                        FormBorderStyle = FormBorderStyle.Sizable;
                    else
                        FormBorderStyle = FormBorderStyle.None;

                    if (!KeepSize())
                        SetFormPosAndSize();
                }
            }
        }

        public int GetHorizontalLocation(Screen screen)
        {
            Rectangle workingArea = screen.WorkingArea;
            Rectangle rect = new Rectangle(Left - workingArea.X, Top - workingArea.Y, Width, Height);

            if (workingArea.Width / (float)Width < 1.1)
                return 0;

            if (rect.X * 3 < workingArea.Width - rect.Right)
                return -1;

            if (rect.X > (workingArea.Width - rect.Right) * 3)
                return 1;

            return 0;
        }

        public int GetVerticalLocation(Screen screen)
        {
            Rectangle workingArea = screen.WorkingArea;
            Rectangle rect = new Rectangle(Left - workingArea.X, Top - workingArea.Y, Width, Height);

            if (workingArea.Height / (float)Height < 1.1)
                return 0;

            if (rect.Y * 3 < workingArea.Height - rect.Bottom)
                return -1;

            if (rect.Y > (workingArea.Height - rect.Bottom) * 3)
                return 1;

            return 0;
        }

        public void BuildMenu()
        {
            string content = File.ReadAllText(Core.InputConfPath);
            var items = CommandItem.GetItems(content);

            if (!content.Contains("#menu:"))
            {
                var defaultItems = CommandItem.GetItems(Properties.Resources.input_conf);

                foreach (CommandItem item in items)
                    foreach (CommandItem defaultItem in defaultItems)
                        if (item.Command == defaultItem.Command)
                            defaultItem.Input = item.Input;

                items = defaultItems;
            }

            foreach (CommandItem item in items)
            {
                if (string.IsNullOrEmpty(item.Path))
                    continue;

                var menuItem = MenuHelp.Add(ContextMenu.Items, item.Path);

                if (menuItem != null)
                {
                    menuItem.Click += (sender, args) => {
                        try {
                            App.RunTask(() => {
                                MenuAutoResetEvent.WaitOne();
                                System.Windows.Application.Current.Dispatcher.Invoke(
                                    DispatcherPriority.Background, new Action(delegate { }));
                                Core.Command(item.Command);                
                            });
                        }
                        catch (Exception ex) {
                            Msg.ShowException(ex);
                        }
                    };

                    menuItem.InputGestureText = item.Input;
                }
            }
        }

        void Core_FileLoaded()
        {
            string path = Core.GetPropertyString("path");

            BeginInvoke(new Action(() => {
                Text = Core.Expand(Title);

                int interval = (int)(Core.Duration.TotalMilliseconds / 100);

                if (interval < 100)
                    interval = 100;

                if (interval > 1000)
                    interval = 1000;

                ProgressTimer.Interval = interval;
                UpdateProgressBar();
            }));

            if (App.Settings.RecentFiles.Contains(path))
                App.Settings.RecentFiles.Remove(path);

            App.Settings.RecentFiles.Insert(0, path);

            while (App.Settings.RecentFiles.Count > App.RecentCount)
                App.Settings.RecentFiles.RemoveAt(App.RecentCount);
        }

        void SetTitle() => BeginInvoke(new Action(() => Text = Core.Expand(Title)));

        public void Voodoo()
        {
            Message m = new Message() { Msg = 0x0202 }; // WM_LBUTTONUP
            SendMessage(Handle, m.Msg, m.WParam, m.LParam);
        }

        void SaveWindowProperties()
        {
            if (WindowState == FormWindowState.Normal && WasShown)
            {
                SavePosition();
                App.Settings.WindowSize = ClientSize;
            }
        }

        void SavePosition()
        {
            Point pos = new Point(Left + Width / 2, Top + Height / 2);
            Screen screen = Screen.FromControl(this);

            int x = GetHorizontalLocation(screen);
            int y = GetVerticalLocation(screen);

            if (x == -1) pos.X = Left;
            if (x ==  1) pos.X = Left + Width;
            if (y == -1) pos.Y = Top;
            if (y ==  1) pos.Y = Top + Height;

            App.Settings.WindowPosition = pos;
            App.Settings.WindowLocation = new Point(x, y);
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x00020000 /* WS_MINIMIZEBOX */;
                return cp;
            }
        }

        string _Title;

        public string Title {
            get => _Title;
            set {
                if (string.IsNullOrEmpty(value))
                    return;

                if (value.EndsWith("} - mpv"))
                    value = value.Replace("} - mpv", "} - mpv.net");

                _Title = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x100: // WM_KEYDOWN
                case 0x101: // WM_KEYUP
                case 0x104: // WM_SYSKEYDOWN
                case 0x105: // WM_SYSKEYUP
                case 0x201: // WM_LBUTTONDOWN
                case 0x202: // WM_LBUTTONUP
                case 0x207: // WM_MBUTTONDOWN
                case 0x208: // WM_MBUTTONUP
                case 0x20a: // WM_MOUSEWHEEL
                case 0x20e: // WM_MOUSEHWHEEL
                case 0x20b: // WM_XBUTTONDOWN
                case 0x20c: // WM_XBUTTONUP         
                    if (Core.WindowHandle != IntPtr.Zero)
                        m.Result = SendMessage(Core.WindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x0204: // WM_RBUTTONDOWN
                    if (IsMouseInOSC() && Core.WindowHandle != IntPtr.Zero)
                        m.Result = SendMessage(Core.WindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x0205: // WM_RBUTTONUP
                    if (!IsMouseInOSC())
                    {
                        CursorHelp.Show();
                        UpdateMenu();
                        ContextMenu.IsOpen = true;
                    }
                    else
                        if (Core.WindowHandle != IntPtr.Zero)
                            m.Result = SendMessage(Core.WindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x319: // WM_APPCOMMAND
                    {
                        string value = Input.WM_APPCOMMAND_to_mpv_key((int)(m.LParam.ToInt64() >> 16 & ~0xf000));

                        if (value != null)
                        {
                            Core.Command("keypress " + value);
                            m.Result = new IntPtr(1);
                            return;
                        }
                    }
                    break;
                case 0x0312: // WM_HOTKEY
                    GlobalHotkey.Execute(m.WParam.ToInt32());
                    break;
                case 0x0200: // WM_MOUSEMOVE
                    if (Environment.TickCount - LastCycleFullscreen > 500)
                    {
                        Point pos = PointToClient(Cursor.Position);
                        Core.Command($"mouse {pos.X} {pos.Y}");
                    }

                    if (CursorHelp.IsPosDifferent(LastCursorPosition))
                        CursorHelp.Show();
                    break;
                case 0x2a3: // WM_MOUSELEAVE
                    //osc won't auto hide after mouse left window in borderless mode
                    Core.Command($"mouse {ClientSize.Width / 2} {ClientSize.Height / 3}");
                    break;
                case 0x203: // WM_LBUTTONDBLCLK
                    {
                        Point pos = PointToClient(Cursor.Position);
                        Core.Command($"mouse {pos.X} {pos.Y} 0 double");
                    }
                    break;
                case 0x02E0: // WM_DPICHANGED
                    {
                        if (!WasShown)
                            break;

                        RECT rect = Marshal.PtrToStructure<RECT>(m.LParam);
                        SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, 0);
                    }
                    break;
                case 0x0214: // WM_SIZING
                    if (Core.KeepaspectWindow)
                    {
                        RECT rc = Marshal.PtrToStructure<RECT>(m.LParam);
                        RECT r = rc;
                        SubtractWindowBorders(Handle, ref r, GetDPI(Handle));

                        int c_w = r.Right - r.Left, c_h = r.Bottom - r.Top;
                        Size videoSize = Core.VideoSize;

                        if (videoSize == Size.Empty)
                            videoSize = new Size(16, 9);

                        float aspect = videoSize.Width / (float)videoSize.Height;
                        int d_w = (int)(c_h * aspect - c_w);
                        int d_h = (int)(c_w / aspect - c_h);

                        int[] d_corners = { d_w, d_h, -d_w, -d_h };
                        int[] corners = { rc.Left, rc.Top, rc.Right, rc.Bottom };
                        int corner = GetResizeBorder(m.WParam.ToInt32());

                        if (corner >= 0)
                            corners[corner] -= d_corners[corner];

                        Marshal.StructureToPtr(new RECT(corners[0], corners[1], corners[2], corners[3]), m.LParam, false);
                        m.Result = new IntPtr(1);
                    }
                    return;
                case 0x004A: // WM_COPYDATA
                    {
                        var copyData = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                        string[] args = copyData.lpData.Split('\n');
                        string mode = args[0];
                        args = args.Skip(1).ToArray();

                        switch (mode)
                        {
                            case "single":
                                Core.LoadFiles(args, true, ModifierKeys.HasFlag(Keys.Control));
                                break;
                            case "queue":
                                foreach (string file in args)
                                    Core.CommandV("loadfile", file, "append");
                                break;
                            case "command":
                                Core.Command(args[0]);
                                break;
                        }

                        Activate();
                    }
                    return;
            }

            if (m.Msg == TaskbarButtonCreatedMessage && Core.TaskbarProgress)
            {
                Taskbar = new Taskbar(Handle);
                ProgressTimer.Start();
            }

            // beep sound when closed using taskbar due to exception
            if (!IsDisposed)
                base.WndProc(ref m);
        }

        void CursorTimer_Tick(object sender, EventArgs e)
        {
            if (CursorHelp.IsPosDifferent(LastCursorPosition))
            {
                LastCursorPosition = MousePosition;
                LastCursorChanged = Environment.TickCount;
            }
            else if (Environment.TickCount - LastCursorChanged > 1500 &&
                !IsMouseInOSC() && ClientRectangle.Contains(PointToClient(MousePosition)) &&
                ActiveForm == this && !ContextMenu.IsVisible && !IsCommandPaletteVissible())

                CursorHelp.Hide();
        }

        void ProgressTimer_Tick(object sender, EventArgs e) => UpdateProgressBar();

        void UpdateProgressBar()
        {
            if (Core.TaskbarProgress && Taskbar != null)
                Taskbar.SetValue(Core.GetPropertyDouble("time-pos"), Core.Duration.TotalSeconds);
        }

        void PropChangeOnTop(bool value) => BeginInvoke(new Action(() => TopMost = value));

        void PropChangeAid(string value) => Core.AID = value;

        void PropChangeSid(string value) => Core.SID = value;

        void PropChangeVid(string value) => Core.VID = value;

        void PropChangeTitle(string value) { Title = value; SetTitle(); }
        
        void PropChangeEdition(int value) => Core.Edition = value;

        void PropChangeWindowMaximized()
        {
            if (!WasShown)
                return;

            BeginInvoke(new Action(() =>
            {
                Core.WindowMaximized = Core.GetPropertyBool("window-maximized");

                if (Core.WindowMaximized && WindowState != FormWindowState.Maximized)
                    WindowState = FormWindowState.Maximized;
                else if (!Core.WindowMaximized && WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;
            }));
        }

        void PropChangeWindowMinimized()
        {
            if (!WasShown)
                return;

            BeginInvoke(new Action(() =>
            {
                Core.WindowMinimized = Core.GetPropertyBool("window-minimized");

                if (Core.WindowMinimized && WindowState != FormWindowState.Minimized)
                    WindowState = FormWindowState.Minimized;
                else if (!Core.WindowMinimized && WindowState == FormWindowState.Minimized)
                    WindowState = FormWindowState.Normal;
            }));
        }

        void PropChangeBorder(bool enabled) {
            Core.Border = enabled;

            BeginInvoke(new Action(() => {
                if (!IsFullscreen)
                {
                    if (Core.Border && FormBorderStyle == FormBorderStyle.None)
                        FormBorderStyle = FormBorderStyle.Sizable;

                    if (!Core.Border && FormBorderStyle == FormBorderStyle.Sizable)
                        FormBorderStyle = FormBorderStyle.None;
                }
            }));
        }

        void PropChangePause(bool paused)
        {
            Core.Paused = paused;

            if (Taskbar != null && Core.TaskbarProgress)
            {
                if (paused)
                    Taskbar.SetState(TaskbarStates.Paused);
                else
                    Taskbar.SetState(TaskbarStates.Normal);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Core.VideoSizeAutoResetEvent.WaitOne(App.StartThreshold);
            LastCycleFullscreen = Environment.TickCount;
            SetFormPosAndSize();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Voodoo();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (WindowState == FormWindowState.Maximized)
                Core.SetPropertyBool("window-maximized", true);

            if (IsVulkanOrGpuNext)
                Core.ProcessCommandLine(false);

            WPF.Init();
            App.UpdateWpfColors();
            MessageBoxEx.MessageForeground = Theme.Current.GetBrush("heading");
            MessageBoxEx.MessageBackground = Theme.Current.GetBrush("background");
            MessageBoxEx.ButtonBackground  = Theme.Current.GetBrush("highlight");
            ContextMenu = new WpfControls.ContextMenu();
            ContextMenu.Closed += ContextMenu_Closed;
            ContextMenu.UseLayoutRounding = true;
            BuildMenu();
            System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y);
            Core.LoadScripts();
            GlobalHotkey.RegisterGlobalHotkeys(Handle);
            App.RunTask(() => App.Extension = new Extension());
            App.RunTask(() => App.CopyMpvnetCom());
            CSharpScriptHost.ExecuteScriptsInFolder(Core.ConfigFolder + "scripts-cs");
            WasShown = true;
        }

        void ContextMenu_Closed(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuAutoResetEvent.Set();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SaveWindowProperties();

            if (Core.IsLogoVisible)
                Core.ShowLogo();

            if (FormBorderStyle != FormBorderStyle.None)
            {
                if (WindowState == FormWindowState.Maximized)
                    WasMaximized = true;
                else if (WindowState == FormWindowState.Normal)
                    WasMaximized = false;
            }

            if (WasShown)
            {
                if (WindowState == FormWindowState.Minimized)
                    Core.SetPropertyBool("window-minimized", true);
                else if (WindowState == FormWindowState.Normal)
                {
                    Core.SetPropertyBool("window-maximized", false);
                    Core.SetPropertyBool("window-minimized", false);
                }
                else if (WindowState == FormWindowState.Maximized)
                    Core.SetPropertyBool("window-maximized", true);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (Core.IsQuitNeeded)
                Core.CommandV("quit");

            if (!Core.ShutdownAutoResetEvent.WaitOne(10000))
                Msg.ShowError("Shutdown thread failed to complete within 10 seconds.");
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (WindowState == FormWindowState.Normal &&
                e.Button == MouseButtons.Left && !IsMouseInOSC())
            {
                var HTCAPTION = new IntPtr(2);
                ReleaseCapture();
                PostMessage(Handle, 0xA1 /* WM_NCLBUTTONDOWN */, HTCAPTION, IntPtr.Zero);
            }

            if (Width - e.Location.X < 10 && e.Location.Y < 10)
                Core.CommandV("quit");
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            SaveWindowProperties();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                Core.LoadFiles(e.Data.GetData(DataFormats.FileDrop) as String[], true, Control.ModifierKeys.HasFlag(Keys.Control));
          
            if (e.Data.GetDataPresent(DataFormats.Text))
                Core.LoadFiles(new[] { e.Data.GetData(DataFormats.Text).ToString() }, true, Control.ModifierKeys.HasFlag(Keys.Control));
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            CursorHelp.Show();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // prevent annoying beep using alt key
            if (ModifierKeys == Keys.Alt)
                e.SuppressKeyPress = true;

            base.OnKeyDown(e);
        }

        protected override void OnLayout(LayoutEventArgs args)
        {
            base.OnLayout(args);
            AdjustCommandPaletteLeftAndWidth();
        }

        class ElementHostEx : ElementHost
        {
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                const int LWA_ColorKey = 1;
                
                if (Environment.OSVersion.Version > new Version(10, 0))
                    SetLayeredWindowAttributes(Handle, 0x111111, 255, LWA_ColorKey);
            }

            protected override CreateParams CreateParams {
                get {
                    const int WS_EX_LAYERED = 0x00080000;
                    CreateParams cp = base.CreateParams;

                    if (Environment.OSVersion.Version > new Version(10, 0))
                        cp.ExStyle = cp.ExStyle | WS_EX_LAYERED;

                    return cp;
                }
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                try {
                    return base.ProcessCmdKey(ref msg, keyData);
                } catch (Exception) {
                    return true;
                }
            }

            [DllImport("user32.dll")]
            public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, int dwFlags);
        }

        public void ShowCommandPalette()
        {
            if (CommandPaletteHost == null)
            {
                CommandPaletteHost = new ElementHostEx();
                CommandPaletteHost.BackColor = Color.FromArgb(0x111111);

                AdjustCommandPaletteLeftAndWidth();
                CommandPaletteHost.Child = CommandPalette.Instance;
                CommandPalette.Instance.AdjustHeight();
                Controls.Add(CommandPaletteHost);
            }
        }

        public void HideCommandPalette()
        {
            if (CommandPaletteHost != null)
            {
                CommandPaletteHost.Visible = false;

                CommandPalette.Instance.Items.Clear();
                CommandPalette.Instance.SearchControl.SearchTextBox.Text = "";
                CommandPalette.Instance.UpdateLayout();

                ActiveControl = null;
                Controls.Remove(CommandPaletteHost);

                CommandPaletteHost.Child = null;
                CommandPaletteHost.Dispose();
                CommandPaletteHost = null;
            }
        }

        void AdjustCommandPaletteLeftAndWidth()
        {
            if (CommandPaletteHost == null)
                return;

            CommandPaletteHost.Width = FontHeight * 25;

            if (CommandPaletteHost.Width > ClientSize.Width)
                CommandPaletteHost.Width = ClientSize.Width;

            CommandPaletteHost.Left = (ClientSize.Width - CommandPaletteHost.Size.Width) / 2;
        }
    }
}
