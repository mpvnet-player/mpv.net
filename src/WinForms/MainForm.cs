
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using static mpvnet.Native;
using static mpvnet.Global;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd { get; set; }
        public new ContextMenuStripEx ContextMenu { get; set; }
        Point LastCursorPosition;
     
        int LastCursorChanged;
        int LastCycleFullscreen;
        int LastAppCommand;
        int TaskbarButtonCreatedMessage;
        int ShownTickCount;

        Taskbar  Taskbar;
        List<string> RecentFiles;
        bool WasMaximized;

        public MainForm()
        {
            InitializeComponent();
            SetColors();

            try
            {
                object recent = RegistryHelp.GetValue("recent");
                RecentFiles = recent is string[] r ? new List<string>(r) : new List<string>();

                Instance = this;
                Hwnd = Handle;
                Core.Init();

                Core.Shutdown += Shutdown;
                Core.VideoSizeChanged += VideoSizeChanged;
                Core.ScaleWindow += ScaleWindow;
                Core.WindowScale += WindowScale;
                Core.FileLoaded += FileLoaded;
                Core.Idle += Idle;
                Core.Seek += () => UpdateProgressBar();

                Core.observe_property("window-maximized", PropChangeWindowMaximized);
                Core.observe_property("window-minimized", PropChangeWindowMinimized);
                Core.observe_property_bool("pause", PropChangePause);
                Core.observe_property_bool("fullscreen", PropChangeFullscreen);
                Core.observe_property_bool("ontop", PropChangeOnTop);
                Core.observe_property_bool("border", PropChangeBorder);

                Core.observe_property_string("sid", PropChangeSid);
                Core.observe_property_string("aid", PropChangeAid);
                Core.observe_property_string("vid", PropChangeVid);

                Core.observe_property_string("title", PropChangeTitle);

                Core.observe_property_int("edition", PropChangeEdition);
                
                if (Core.GPUAPI != "vulkan")
                    Core.ProcessCommandLine(false);

                AppDomain.CurrentDomain.UnhandledException += (sender, e) => App.ShowException(e.ExceptionObject);
                Application.ThreadException += (sender, e) => App.ShowException(e.Exception);

                TaskbarButtonCreatedMessage = RegisterWindowMessage("TaskbarButtonCreated");
                
                ContextMenu = new ContextMenuStripEx(components);
                ContextMenu.Opened += ContextMenu_Opened;
                ContextMenu.Opening += ContextMenu_Opening;

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

                int posX = RegistryHelp.GetInt("position-x");
                int posY = RegistryHelp.GetInt("position-y");
                
                if ((posX != 0 || posY != 0) && App.RememberPosition)
                {
                    Left = posX - Width / 2;
                    Top = posY - Height / 2;

                    int horizontal = RegistryHelp.GetInt("location-horizontal");
                    int vertical = RegistryHelp.GetInt("location-vertical");

                    if (horizontal == -1) Left = posX;
                    if (horizontal ==  1) Left = posX - Width;
                    if (vertical   == -1) Top = posY;
                    if (vertical   ==  1) Top = posY - Height;
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

        void ScaleWindow(float scale) {
            BeginInvoke(new Action(() => {
                int w = (int)(ClientSize.Width * scale);
                int h = (int)Math.Ceiling(w * Core.VideoSize.Height / (double)Core.VideoSize.Width);
                SetSize(w, h, Screen.FromControl(this), false);
            }));
        }

        void WindowScale(float scale)
        {
            BeginInvoke(new Action(() => {
                SetSize(
                    (int)(Core.VideoSize.Width * scale),
                    (int)Math.Ceiling(Core.VideoSize.Height * scale),
                    Screen.FromControl(this), false);
                Core.command($"show-text \"window-scale {scale.ToString(CultureInfo.InvariantCulture)}\"");
            }));
        }

        public MenuItem FindMenuItem(string text) => FindMenuItem(text, ContextMenu.Items);

        void Shutdown() => BeginInvoke(new Action(() => Close()));

        void Idle() => SetTitle();

        bool WasShown() => ShownTickCount != 0 && Environment.TickCount > ShownTickCount + 500;

        void CM_Popup(object sender, EventArgs e) => CursorHelp.Show();

        void VideoSizeChanged() => BeginInvoke(new Action(() => SetFormPosAndSize()));

        void PropChangeFullscreen(bool value) => BeginInvoke(new Action(() => CycleFullscreen(value)));

        void ContextMenu_Opened(object sender, EventArgs e) => CursorHelp.Show();

        bool IsFullscreen => WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None;

        bool IsMouseInOSC()
        {
            Point pos = PointToClient(Control.MousePosition);
            float top = 0;

            if (FormBorderStyle == FormBorderStyle.None)
                top = ClientSize.Height * 0.1f;

            return pos.Y > ClientSize.Height * 0.85 || pos.Y < top;
        }

        void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            lock (Core.MediaTracks)
            {
                MenuItem trackMenuItem = FindMenuItem("Track");

                if (trackMenuItem != null)
                {
                    trackMenuItem.DropDownItems.Clear();

                    MediaTrack[] audTracks = Core.MediaTracks.Where(track => track.Type == "a").ToArray();
                    MediaTrack[] subTracks = Core.MediaTracks.Where(track => track.Type == "s").ToArray();
                    MediaTrack[] vidTracks = Core.MediaTracks.Where(track => track.Type == "v").ToArray();
                    MediaTrack[] ediTracks = Core.MediaTracks.Where(track => track.Type == "e").ToArray();

                    foreach (MediaTrack track in vidTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => Core.commandv("set", "vid", track.ID.ToString());
                        mi.Checked = Core.Vid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (vidTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in audTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => Core.commandv("set", "aid", track.ID.ToString());
                        mi.Checked = Core.Aid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in subTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => Core.commandv("set", "sid", track.ID.ToString());
                        mi.Checked = Core.Sid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                    {
                        MenuItem mi = new MenuItem("S: No subtitles");
                        mi.Action = () => Core.commandv("set", "sid", "no");
                        mi.Checked = Core.Sid == "no";
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (ediTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in ediTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => Core.commandv("set", "edition", track.ID.ToString());
                        mi.Checked = Core.Edition == track.ID;
                        trackMenuItem.DropDownItems.Add(mi);
                    }
                }
            }

            lock (Core.Chapters)
            {
                MenuItem chaptersMenuItem = FindMenuItem("Chapters");

                if (chaptersMenuItem != null)
                {
                    chaptersMenuItem.DropDownItems.Clear();

                    foreach (var pair in Core.Chapters)
                    {
                        MenuItem mi = new MenuItem(pair.Key);
                        mi.ShortcutKeyDisplayString = TimeSpan.FromSeconds(pair.Value).ToString().Substring(0, 8) + "     ";
                        mi.Action = () => Core.commandv("seek", pair.Value.ToString(CultureInfo.InvariantCulture), "absolute");
                        chaptersMenuItem.DropDownItems.Add(mi);
                    }
                }
            }

            MenuItem recent = FindMenuItem("Recent");

            if (recent != null)
            {
                recent.DropDownItems.Clear();

                foreach (string path in RecentFiles)
                    MenuItem.Add(recent.DropDownItems, path, () => Core.LoadFiles(new[] { path }, true, Control.ModifierKeys.HasFlag(Keys.Control)));
               
                recent.DropDownItems.Add(new ToolStripSeparator());
                MenuItem mi = new MenuItem("Clear List");
                mi.Action = () => RecentFiles.Clear();
                recent.DropDownItems.Add(mi);
            }

            MenuItem titles = FindMenuItem("Titles");

            if (titles != null)
            {
                titles.DropDownItems.Clear();

                lock (Core.BluRayTitles)
                {
                    List<(int Index, TimeSpan Len)> items = new List<(int Index, TimeSpan Len)>(); 

                    for (int i = 0; i < Core.BluRayTitles.Count; i++)
                        items.Add((i, Core.BluRayTitles[i]));

                    var titleItems = items.OrderByDescending(item => item.Len)
                        .Take(20).OrderBy(item => item.Index);

                    foreach (var item in titleItems)
                        if (item.Len != TimeSpan.Zero)
                            MenuItem.Add(titles.DropDownItems, $"{item.Len} ({item.Index})",
                                () => Core.SetBluRayTitle(item.Index));
                }
            }
        }

        MenuItem FindMenuItem(string text, ToolStripItemCollection items)
        {
            foreach (var item in items)
            {
                if (item is MenuItem mi)
                {
                    if (mi.Text.StartsWith(text) && mi.Text.Trim() == text)
                        return mi;

                    if (mi.DropDownItems.Count > 0)
                    {
                        MenuItem val = FindMenuItem(text, mi.DropDownItems);
                        
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
                if (App.StartSize == "always")
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
                int savedHeight = RegistryHelp.GetInt("window-height");
                int savedWidth  = RegistryHelp.GetInt("window-width");

                if (App.StartSize == "height-always" && savedHeight != 0)
                {
                    height = savedHeight;
                    width = height * videoSize.Width / videoSize.Height;
                }
                else if (App.StartSize == "height-session")
                {
                    height = autoFitHeight;
                    width = height * videoSize.Width / videoSize.Height;
                }
                if (App.StartSize == "width-always" && savedHeight != 0)
                {
                    width = savedWidth;
                    height = (int)Math.Ceiling(width * videoSize.Height / (double)videoSize.Width);
                }
                else if (App.StartSize == "width-session")
                {
                    width = autoFitHeight / 9 * 16;
                    height = (int)Math.Ceiling(width * videoSize.Height / (double)videoSize.Width);
                }
                else if (App.StartSize == "always" && savedHeight != 0)
                {
                    height = savedHeight;
                    width = savedWidth;
                }

                Core.WasInitialSizeSet = true;
            }

            SetSize(width, height, screen, checkAutofit);
        }

        void SetSize(int width, int height, Screen screen, bool checkAutofit = true)
        {
            int maxHeight = screen.WorkingArea.Height - (Height - ClientSize.Height) - FontHeight / 2;
            int maxWidth = screen.WorkingArea.Width - (Width - ClientSize.Width) - FontHeight / 2;

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
            Rectangle workingArea = screen.WorkingArea;
            Rectangle currentRect = new Rectangle(Left, Top, Width, Height);

            if (GetHorizontalLocation(screen) == -1) left = Left;
            if (GetHorizontalLocation(screen) ==  1) left = currentRect.Right - rect.Width;

            if (GetVerticalLocation(screen) == -1) top = Top;
            if (GetVerticalLocation(screen) ==  1) top = currentRect.Bottom - rect.Height;

            Screen[] screens = Screen.AllScreens;
            int minLeft = screens.Select(val => val.WorkingArea.X).Min();
            int maxRight = screens.Select(val => val.WorkingArea.Right).Max();
            int minTop = screens.Select(val => val.WorkingArea.Y).Min();
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

        public int GetHorizontalLocation(Screen screen)
        {
            Rectangle workingArea = screen.WorkingArea;
            Rectangle rect = new Rectangle(Left - workingArea.X, Top - workingArea.Y, Width, Height);

            if (workingArea.Width / (float)Width < 1.2)
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

            if (workingArea.Height / (float)Height < 1.2)
                return 0;

            if (rect.Y * 3 < workingArea.Height - rect.Bottom)
                return -1;

            if (rect.Y > (workingArea.Height - rect.Bottom) * 3)
                return 1;

            return 0;
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
                        Rectangle b = Screen.FromControl(this).Bounds;
                        uint SWP_SHOWWINDOW = 0x0040;
                        IntPtr HWND_TOP= IntPtr.Zero;
                        SetWindowPos(Handle, HWND_TOP, b.X, b.Y, b.Width, b.Height, SWP_SHOWWINDOW);
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

                    SetFormPosAndSize();
                    SaveWindowProperties();
                }
            }
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

                MenuItem menuItem = ContextMenu.Add(item.Path.Replace("&", "&&"), () => {
                    try {
                        Core.command(item.Command);
                    } catch (Exception ex) {
                        Msg.ShowException(ex);
                    }
                });

                if (menuItem != null)
                    menuItem.ShortcutKeyDisplayString = item.Input.Replace("&", "&&") + "    ";
            }
        }

        void FileLoaded()
        {
            string path = Core.get_property_string("path");

            BeginInvoke(new Action(() => {
                Text = Core.expand(Title);

                int interval = (int)(Core.Duration.TotalMilliseconds / 100);

                if (interval < 100)
                    interval = 100;

                if (interval > 1000)
                    interval = 1000;

                ProgressTimer.Interval = interval;
                UpdateProgressBar();
            }));

            if (RecentFiles.Contains(path))
                RecentFiles.Remove(path);

            RecentFiles.Insert(0, path);

            while (RecentFiles.Count > App.RecentCount)
                RecentFiles.RemoveAt(App.RecentCount);
        }

        void SetTitle() => BeginInvoke(new Action(() => Text = Core.expand(Title)));

        void SaveWindowProperties()
        {
            if (WindowState == FormWindowState.Normal)
            {
                SavePosition();

                RegistryHelp.SetInt("window-width", ClientSize.Width);
                RegistryHelp.SetInt("window-height", ClientSize.Height);
            }
        }

        void SavePosition()
        {
            int posX = Left + Width / 2;
            int posY = Top + Height / 2;

            Screen screen = Screen.FromControl(this);

            int x = GetHorizontalLocation(screen);
            int y = GetVerticalLocation(screen);

            if (x == -1) posX = Left;
            if (x ==  1) posX = Left + Width;
            if (y == -1) posY = Top;
            if (y ==  1) posY = Top + Height;

            RegistryHelp.SetInt("position-x", posX);
            RegistryHelp.SetInt("position-y", posY);
            
            RegistryHelp.SetInt("location-horizontal", x);
            RegistryHelp.SetInt("location-vertical", y);
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
            //Debug.WriteLine(m);

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
                    {
                        bool skip = m.Msg == 0x100 && LastAppCommand != 0 &&
                            (Environment.TickCount - LastAppCommand) < 1000;

                        if (Core.WindowHandle != IntPtr.Zero && !skip)
                            m.Result = SendMessage(Core.WindowHandle, m.Msg, m.WParam, m.LParam);
                    }
                    break;
                case 0x319: // WM_APPCOMMAND
                    {
                        string value = Input.WM_APPCOMMAND_to_mpv_key((int)(m.LParam.ToInt64() >> 16 & ~0xf000));

                        if (value != null)
                        {
                            Core.command("keypress " + value);
                            m.Result = new IntPtr(1);
                            LastAppCommand = Environment.TickCount;
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
                        Core.command($"mouse {pos.X} {pos.Y}");
                    }

                    if (CursorHelp.IsPosDifferent(LastCursorPosition))
                        CursorHelp.Show();
                    break;
                case 0x2a3: // WM_MOUSELEAVE
                    //osc won't auto hide after mouse left window in borderless mode
                    Core.command($"mouse {ClientSize.Width / 2} {ClientSize.Height / 3}");
                    break;
                case 0x203: // WM_LBUTTONDBLCLK
                    {
                        Point pos = PointToClient(Cursor.Position);
                        Core.command($"mouse {pos.X} {pos.Y} 0 double");
                    }
                    break;
                case 0x02E0: // WM_DPICHANGED
                    {
                        if (!WasShown())
                            break;

                        RECT rect = Marshal.PtrToStructure<RECT>(m.LParam);
                        SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, 0);
                    }
                    break;
                case 0x0214: // WM_SIZING
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
                                    Core.commandv("loadfile", file, "append");
                                break;
                            case "command":
                                Core.command(args[0]);
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
                LastCursorPosition = Control.MousePosition;
                LastCursorChanged = Environment.TickCount;
            }
            else if (Environment.TickCount - LastCursorChanged > 1500 &&
                !IsMouseInOSC() && ClientRectangle.Contains(PointToClient(MousePosition)) &&
                Form.ActiveForm == this && !ContextMenu.Visible)

                CursorHelp.Hide();
        }

        void ProgressTimer_Tick(object sender, EventArgs e) => UpdateProgressBar();

        void UpdateProgressBar()
        {
            if (Core.TaskbarProgress && Taskbar != null)
                Taskbar.SetValue(Core.get_property_number("time-pos"), Core.Duration.TotalSeconds);
        }

        void PropChangeOnTop(bool value) => BeginInvoke(new Action(() => TopMost = value));

        void PropChangeAid(string value) => Core.Aid = value;

        void PropChangeSid(string value) => Core.Sid = value;

        void PropChangeVid(string value) => Core.Vid = value;

        void PropChangeTitle(string value) { Title = value; SetTitle(); }

        void PropChangeEdition(int value) => Core.Edition = value;

        void PropChangeWindowMaximized()
        {
            if (!WasShown())
                return;

            BeginInvoke(new Action(() =>
            {
                Core.WindowMaximized = Core.get_property_bool("window-maximized");

                if (Core.WindowMaximized && WindowState != FormWindowState.Maximized)
                    WindowState = FormWindowState.Maximized;
                else if (!Core.WindowMaximized && WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;
            }));
        }

        void PropChangeWindowMinimized()
        {
            if (!WasShown())
                return;

            BeginInvoke(new Action(() =>
            {
                Core.WindowMinimized = Core.get_property_bool("window-minimized");

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

        void PropChangePause(bool enabled)
        {
            if (Taskbar != null && Core.TaskbarProgress)
            {
                if (enabled)
                    Taskbar.SetState(TaskbarStates.Paused);
                else
                    Taskbar.SetState(TaskbarStates.Normal);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Core.GPUAPI != "vulkan")
                Core.VideoSizeAutoResetEvent.WaitOne(App.StartThreshold);

            LastCycleFullscreen = Environment.TickCount;
            SetFormPosAndSize();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Message m = new Message() { Msg = 0x0202 }; // WM_LBUTTONUP
            SendMessage(Handle, m.Msg, m.WParam, m.LParam);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (WindowState == FormWindowState.Maximized)
                Core.set_property_bool("window-maximized", true);

            if (Core.GPUAPI == "vulkan")
                Core.ProcessCommandLine(false);

            BuildMenu();
            ContextMenuStrip = ContextMenu;
            WPF.Init();
            System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y);
            UpdateCheck.DailyCheck();
            Core.LoadScripts();
            GlobalHotkey.RegisterGlobalHotkeys(Handle);
            App.RunTask(() => App.Extension = new Extension());
            CSharpScriptHost.ExecuteScriptsInFolder(Core.ConfigFolder + "scripts-cs");
            ShownTickCount = Environment.TickCount;
            App.ShowSetup();

            //if (Debugger.IsAttached)
            //{
            //}
        }

        static void SetColors()
        {
            ToolStripRendererEx.ForegroundColor = Theme.Current.GetWinFormsColor("menu-foreground");
            ToolStripRendererEx.BackgroundColor = Theme.Current.GetWinFormsColor("menu-background");
            ToolStripRendererEx.SelectionColor = Theme.Current.GetWinFormsColor("menu-highlight");
            ToolStripRendererEx.BorderColor = Theme.Current.GetWinFormsColor("menu-border");
            ToolStripRendererEx.CheckedColor = Theme.Current.GetWinFormsColor("menu-checked");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Core.IsLogoVisible)
                Core.ShowLogo();

            if (FormBorderStyle != FormBorderStyle.None)
            {
                if (WindowState == FormWindowState.Maximized)
                    WasMaximized = true;
                else if (WindowState == FormWindowState.Normal)
                    WasMaximized = false;
            }

            if (WasShown())
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Core.set_property_bool("window-minimized", true);
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Core.set_property_bool("window-maximized", false);
                    Core.set_property_bool("window-minimized", false);
                }
                else if (WindowState == FormWindowState.Maximized)
                {
                    Core.set_property_bool("window-maximized", true);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SaveWindowProperties();
            RegistryHelp.SetValue("recent", RecentFiles.ToArray());

            if (Core.IsQuitNeeded)
                Core.commandv("quit");

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
                Core.commandv("quit");
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
    }
}
