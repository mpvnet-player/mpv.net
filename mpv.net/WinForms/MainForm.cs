
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;

using static mpvnet.Core;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd { get; set; }
        public new ContextMenuStripEx ContextMenu { get; set; }
        Point  LastCursorPosition;
        int    LastCursorChanged;
        int    LastCycleFullscreen;
        int    LastAppCommand;
        int    TaskbarButtonCreatedMessage;
        int    ShownTickCount;

        Taskbar  Taskbar;
        List<string> RecentFiles;
        bool WasMaximized;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                object recent = RegistryHelp.GetValue(App.RegPath, "Recent");

                if (recent is string[] r)
                    RecentFiles = new List<string>(r);
                else
                    RecentFiles = new List<string>();

                Instance = this;
                Hwnd = Handle;
                ConsoleHelp.Padding = 60;
                core.Init();

                core.Shutdown += Shutdown;
                core.VideoSizeChanged += VideoSizeChanged;
                core.FileLoaded += FileLoaded;
                core.Idle += Idle;
                core.Seek += () => UpdateProgressBar();

                core.observe_property("window-maximized", PropChangeWindowMaximized);
                core.observe_property("window-minimized", PropChangeWindowMinimized);
                core.observe_property_bool("pause", PropChangePause);
                core.observe_property_bool("fullscreen", PropChangeFullscreen);
                core.observe_property_bool("ontop", PropChangeOnTop);
                core.observe_property_bool("border", PropChangeBorder);

                core.observe_property_string("sid", PropChangeSid);
                core.observe_property_string("aid", PropChangeAid);
                core.observe_property_string("vid", PropChangeVid);

                core.observe_property_int("edition", PropChangeEdition);
                core.observe_property_double("window-scale", PropChangeWindowScale);
                
                if (core.GPUAPI != "vulkan")
                    core.ProcessCommandLine(false);

                AppDomain.CurrentDomain.UnhandledException += (sender, e) => App.ShowException(e.ExceptionObject);
                Application.ThreadException += (sender, e) => App.ShowException(e.Exception);
                Msg.SupportURL = "https://github.com/stax76/mpv.net#support";
                Text = "mpv.net " + Application.ProductVersion;
                TaskbarButtonCreatedMessage = WinAPI.RegisterWindowMessage("TaskbarButtonCreated");
                
                ContextMenu = new ContextMenuStripEx(components);
                ContextMenu.Opened += ContextMenu_Opened;
                ContextMenu.Opening += ContextMenu_Opening;

                if (core.Screen == -1)
                    core.Screen = Array.IndexOf(Screen.AllScreens, Screen.PrimaryScreen);

                int targetIndex = core.Screen;
                Screen[] screens = Screen.AllScreens;

                if (targetIndex < 0)
                    targetIndex = 0;

                if (targetIndex > screens.Length - 1)
                    targetIndex = screens.Length - 1;

                Screen screen = screens[Array.IndexOf(screens, screens[targetIndex])];
                Rectangle target = screen.Bounds;
                Left = target.X + (target.Width - Width) / 2;
                Top = target.Y + (target.Height - Height) / 2;

                if (!core.Border)
                    FormBorderStyle = FormBorderStyle.None;

                int posX = RegistryHelp.GetInt(App.RegPath, "PosX");
                int posY = RegistryHelp.GetInt(App.RegPath, "PosY");

                if (posX != 0 && posY != 0 && App.RememberPosition)
                {
                    Left = posX - Width / 2;
                    Top = posY - Height / 2;
                }

                if (core.WindowMaximized)
                {
                    SetFormPosAndSize(1, true);
                    WindowState = FormWindowState.Maximized;
                }

                if (core.WindowMinimized)
                {
                    SetFormPosAndSize(1, true);
                    WindowState = FormWindowState.Minimized;
                }
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }

        public MenuItem FindMenuItem(string text) => FindMenuItem(text, ContextMenu.Items);

        void Shutdown() => BeginInvoke(new Action(() => Close()));

        void Idle()
        {
            BeginInvoke(new Action(() => Text = "mpv.net " + Application.ProductVersion));
        }

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
            lock (core.MediaTracks)
            {
                MenuItem trackMenuItem = FindMenuItem("Track");

                if (trackMenuItem != null)
                {
                    trackMenuItem.DropDownItems.Clear();

                    MediaTrack[] audTracks = core.MediaTracks.Where(track => track.Type == "a").ToArray();
                    MediaTrack[] subTracks = core.MediaTracks.Where(track => track.Type == "s").ToArray();
                    MediaTrack[] vidTracks = core.MediaTracks.Where(track => track.Type == "v").ToArray();
                    MediaTrack[] ediTracks = core.MediaTracks.Where(track => track.Type == "e").ToArray();

                    foreach (MediaTrack track in vidTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => core.commandv("set", "vid", track.ID.ToString());
                        mi.Checked = core.Vid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (vidTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in audTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => core.commandv("set", "aid", track.ID.ToString());
                        mi.Checked = core.Aid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in subTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => core.commandv("set", "sid", track.ID.ToString());
                        mi.Checked = core.Sid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                    {
                        MenuItem mi = new MenuItem("S: No subtitles");
                        mi.Action = () => core.commandv("set", "sid", "no");
                        mi.Checked = core.Sid == "no";
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (ediTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in ediTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => core.commandv("set", "edition", track.ID.ToString());
                        mi.Checked = core.Edition == track.ID;
                        trackMenuItem.DropDownItems.Add(mi);
                    }
                }
            }

            lock (core.Chapters)
            {
                MenuItem chaptersMenuItem = FindMenuItem("Chapters");

                if (chaptersMenuItem != null)
                {
                    chaptersMenuItem.DropDownItems.Clear();

                    foreach (var i in core.Chapters)
                    {
                        MenuItem mi = new MenuItem(i.Key);
                        mi.ShortcutKeyDisplayString = TimeSpan.FromSeconds(i.Value).ToString().Substring(0, 8) + "     ";
                        mi.Action = () => core.commandv("seek", i.Value.ToString(CultureInfo.InvariantCulture), "absolute");
                        chaptersMenuItem.DropDownItems.Add(mi);
                    }
                }
            }

            MenuItem recent = FindMenuItem("Recent");

            if (recent != null)
            {
                recent.DropDownItems.Clear();

                foreach (string path in RecentFiles)
                    MenuItem.Add(recent.DropDownItems, path, () => core.LoadFiles(new[] { path }, true, Control.ModifierKeys.HasFlag(Keys.Control)));
               
                recent.DropDownItems.Add(new ToolStripSeparator());
                MenuItem mi = new MenuItem("Clear List");
                mi.Action = () => RecentFiles.Clear();
                recent.DropDownItems.Add(mi);
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
                        if (val != null) return val;
                    }
                }
            }
            return null;
        }

        void SetFormPosAndSize(double scale = 1, bool force = false)
        {
            if (!force)
            {
                if (WindowState != FormWindowState.Normal)
                    return;

                if (core.Fullscreen)
                {
                    CycleFullscreen(true);
                    return;
                }
            }
            
            Screen screen = Screen.FromControl(this);
            int autoFitHeight = Convert.ToInt32(screen.WorkingArea.Height * core.Autofit);

            if (core.VideoSize.Height == 0 || core.VideoSize.Width == 0 ||
                core.VideoSize.Width / (float)core.VideoSize.Height < App.MinimumAspectRatio)

                core.VideoSize = new Size((int)(autoFitHeight * (16 / 9.0)), autoFitHeight);

            Size videoSize = core.VideoSize;
            int height = videoSize.Height;

            if (core.WasInitialSizeSet || scale != 1)
                height = ClientSize.Height;
            else
            {
                int savedHeight = RegistryHelp.GetInt(App.RegPath, "Height");

                if (App.StartSize == "always" && savedHeight != 0)
                    height = savedHeight;
                else
                    if (App.StartSize != "video")
                        height = autoFitHeight;

                core.WasInitialSizeSet = true;
            }

            height = Convert.ToInt32(height * scale);
            int width = Convert.ToInt32(height * videoSize.Width / (double)videoSize.Height);
            int maxHeight = screen.WorkingArea.Height - (Height - ClientSize.Height);
            int maxWidth = screen.WorkingArea.Width - (Width - ClientSize.Width);

            if (height < maxHeight * core.AutofitSmaller)
            {
                height = Convert.ToInt32(maxHeight * core.AutofitSmaller);
                width = Convert.ToInt32(height * videoSize.Width / (double)videoSize.Height);
            }

            if (height > maxHeight * core.AutofitLarger)
            {
                height = Convert.ToInt32(maxHeight * core.AutofitLarger);
                width = Convert.ToInt32(height * videoSize.Width / (double)videoSize.Height);
            }

            if (width > maxWidth)
            {
                width = maxWidth;
                height = Convert.ToInt32(width * videoSize.Height / (double)videoSize.Width);
            }

            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new WinAPI.RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
            NativeHelp.AddWindowBorders(Handle, ref rect);
            int left = middlePos.X - rect.Width / 2;
            int top = middlePos.Y - rect.Height / 2;

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

            WinAPI.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */,
                left, top, rect.Width, rect.Height, 4 /* SWP_NOZORDER */);
        }

        public void CycleFullscreen(bool enabled)
        {
            LastCycleFullscreen = Environment.TickCount;
            core.Fullscreen = enabled;

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
                        WinAPI.SetWindowPos(Handle, HWND_TOP, b.X, b.Y, b.Width, b.Height, SWP_SHOWWINDOW);
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

                    if (core.Border)
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
            string content = File.ReadAllText(core.InputConfPath);
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

                string path = item.Path.Replace("&", "&&");

                MenuItem menuItem = ContextMenu.Add(path, () => {
                    try {
                        core.command(item.Command);
                    } catch (Exception ex) {
                        Msg.ShowException(ex);
                    }
                });

                if (menuItem != null)
                    menuItem.ShortcutKeyDisplayString = item.Input + "    ";
            }
        }

        void FileLoaded()
        {
            string path = core.get_property_string("path");

            BeginInvoke(new Action(() => {
                if (path.Contains("://"))
                    Text = core.get_property_string("media-title") + " - mpv.net " + Application.ProductVersion;
                else if (path.Contains(":\\") || path.StartsWith("\\\\"))
                    Text = path.FileName() + " - mpv.net " + Application.ProductVersion;
                else
                    Text = "mpv.net " + Application.ProductVersion;

                int interval = (int)(core.Duration.TotalMilliseconds / 100);

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

        void SaveWindowProperties()
        {
            if (WindowState == FormWindowState.Normal)
            {
                RegistryHelp.SetValue(App.RegPath, "PosX", Left + Width / 2);
                RegistryHelp.SetValue(App.RegPath, "PosY", Top + Height / 2);
                RegistryHelp.SetValue(App.RegPath, "Height", ClientSize.Height);
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x00020000 /* WS_MINIMIZEBOX */;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            //Debug.WriteLine(m);

            switch (m.Msg)
            {
                case 0x201: // WM_LBUTTONDOWN
                case 0x202: // WM_LBUTTONUP
                case 0x207: // WM_MBUTTONDOWN
                case 0x208: // WM_MBUTTONUP
                case 0x20b: // WM_XBUTTONDOWN
                case 0x20c: // WM_XBUTTONUP
                case 0x20A: // WM_MOUSEWHEEL
                case 0x100: // WM_KEYDOWN
                case 0x101: // WM_KEYUP
                case 0x104: // WM_SYSKEYDOWN
                case 0x105: // WM_SYSKEYUP
                    {
                        bool skip = m.Msg == 0x100 && LastAppCommand != 0 &&
                            (Environment.TickCount - LastAppCommand) < 1000;

                        if (core.WindowHandle != IntPtr.Zero && !skip)
                            m.Result = WinAPI.SendMessage(core.WindowHandle, m.Msg, m.WParam, m.LParam);
                    }

                    break;
                case 0x319: // WM_APPCOMMAND
                    {
                        string value = mpvHelp.WM_APPCOMMAND_to_mpv_key((int)(m.LParam.ToInt64() >> 16 & ~0xf000));

                        if (value != null)
                        {
                            core.command("keypress " + value);
                            m.Result = new IntPtr(1);
                            LastAppCommand = Environment.TickCount;
                            return;
                        }
                    }

                    break;
                case 0x0200: // WM_MOUSEMOVE
                    if (Environment.TickCount - LastCycleFullscreen > 500)
                    {
                        Point pos = PointToClient(Cursor.Position);
                        core.command($"mouse {pos.X} {pos.Y}");
                    }

                    if (CursorHelp.IsPosDifferent(LastCursorPosition))
                        CursorHelp.Show();
                    break;
                case 0x2a3: // WM_MOUSELEAVE
                    //osc won't auto hide after mouse left window in borderless mode
                    core.command($"mouse {ClientSize.Width / 2} {ClientSize.Height / 3}");
                    break;
                case 0x203: // WM_LBUTTONDBLCLK
                    {
                        Point pos = PointToClient(Cursor.Position);
                        core.command($"mouse {pos.X} {pos.Y} 0 double");
                    }
                    break;
                case 0x02E0: // WM_DPICHANGED
                    {
                        if (!WasShown())
                            break;

                        WinAPI.RECT rect = Marshal.PtrToStructure<WinAPI.RECT>(m.LParam);
                        WinAPI.SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, 0);
                    }
                    break;
                case 0x0214: // WM_SIZING
                    {
                        var rc = Marshal.PtrToStructure<WinAPI.RECT>(m.LParam);
                        var r = rc;
                        NativeHelp.SubtractWindowBorders(Handle, ref r);
                        int c_w = r.Right - r.Left, c_h = r.Bottom - r.Top;
                        Size s = core.VideoSize;

                        if (s == Size.Empty)
                            s = new Size(16, 9);

                        float aspect = s.Width / (float)s.Height;
                        int d_w = Convert.ToInt32(c_h * aspect - c_w);
                        int d_h = Convert.ToInt32(c_w / aspect - c_h);
                        int[] d_corners = { d_w, d_h, -d_w, -d_h };
                        int[] corners = { rc.Left, rc.Top, rc.Right, rc.Bottom };
                        int corner = NativeHelp.GetResizeBorder(m.WParam.ToInt32());

                        if (corner >= 0)
                            corners[corner] -= d_corners[corner];

                        Marshal.StructureToPtr<WinAPI.RECT>(new WinAPI.RECT(corners[0], corners[1], corners[2], corners[3]), m.LParam, false);
                        m.Result = new IntPtr(1);
                    }
                    return;
                case 0x004A: // WM_COPYDATA
                    {
                        var copyData = (WinAPI.COPYDATASTRUCT)m.GetLParam(typeof(WinAPI.COPYDATASTRUCT));
                        string[] files = copyData.lpData.Split('\n');
                        string mode = files[0];
                        files = files.Skip(1).ToArray();

                        switch (mode)
                        {
                            case "single":
                                core.LoadFiles(files, true, Control.ModifierKeys.HasFlag(Keys.Control));
                                break;
                            case "queue":
                                foreach (string file in files)
                                    core.commandv("loadfile", file, "append");
                                break;
                        }

                        Activate();
                    }
                    return;
            }

            if (m.Msg == TaskbarButtonCreatedMessage && core.TaskbarProgress)
            {
                Taskbar = new Taskbar(Handle);
                ProgressTimer.Start();
            }

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
            if (core.TaskbarProgress && Taskbar != null)
                Taskbar.SetValue(core.get_property_number("time-pos"), core.Duration.TotalSeconds);
        }

        void PropChangeOnTop(bool value) => BeginInvoke(new Action(() => TopMost = value));

        void PropChangeAid(string value) => core.Aid = value;

        void PropChangeSid(string value) => core.Sid = value;

        void PropChangeVid(string value) => core.Vid = value;

        void PropChangeEdition(int value) => core.Edition = value;
        
        void PropChangeWindowScale(double value)
        {
            if (value != 1)
            {
                BeginInvoke(new Action(() => SetFormPosAndSize(value)));
                core.command("no-osd set window-scale 1");
            }
        }

        void PropChangeWindowMaximized()
        {
            if (!WasShown())
                return;

            BeginInvoke(new Action(() =>
            {
                core.WindowMaximized = core.get_property_bool("window-maximized");

                if (core.WindowMaximized && WindowState != FormWindowState.Maximized)
                    WindowState = FormWindowState.Maximized;
                else if (!core.WindowMaximized && WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;
            }));
        }

        void PropChangeWindowMinimized()
        {
            if (!WasShown())
                return;

            BeginInvoke(new Action(() =>
            {
                core.WindowMinimized = core.get_property_bool("window-minimized");

                if (core.WindowMinimized && WindowState != FormWindowState.Minimized)
                    WindowState = FormWindowState.Minimized;
                else if (!core.WindowMinimized && WindowState == FormWindowState.Minimized)
                    WindowState = FormWindowState.Normal;
            }));
        }

        void PropChangeBorder(bool enabled) {
            core.Border = enabled;

            BeginInvoke(new Action(() => {
                if (!IsFullscreen)
                {
                    if (core.Border && FormBorderStyle == FormBorderStyle.None)
                        FormBorderStyle = FormBorderStyle.Sizable;

                    if (!core.Border && FormBorderStyle == FormBorderStyle.Sizable)
                        FormBorderStyle = FormBorderStyle.None;
                }
            }));
        }

        void PropChangePause(bool enabled)
        {
            if (Taskbar != null && core.TaskbarProgress)
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

            if (core.GPUAPI != "vulkan")
                core.VideoSizeAutoResetEvent.WaitOne(App.StartThreshold);

            LastCycleFullscreen = Environment.TickCount;
            SetFormPosAndSize();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (core.GPUAPI == "vulkan")
                core.ProcessCommandLine(false);

            ToolStripRendererEx.ForegroundColor = Theme.Current.GetWinFormsColor("menu-foreground");
            ToolStripRendererEx.BackgroundColor = Theme.Current.GetWinFormsColor("menu-background");
            ToolStripRendererEx.SelectionColor  = Theme.Current.GetWinFormsColor("menu-highlight");
            ToolStripRendererEx.BorderColor     = Theme.Current.GetWinFormsColor("menu-border");
            ToolStripRendererEx.CheckedColor    = Theme.Current.GetWinFormsColor("menu-checked");

            BuildMenu();
            ContextMenuStrip = ContextMenu;
            WPF.WPF.Init();
            System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y);
            UpdateCheck.DailyCheck();
            core.LoadScripts();
            Task.Run(() => App.Extension = new Extension());
            ShownTickCount = Environment.TickCount;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Message m = new Message() { Msg = 0x0202 }; // WM_LBUTTONUP
            WinAPI.SendMessage(MainForm.Instance.Handle, m.Msg, m.WParam, m.LParam);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (core.IsLogoVisible)
                core.ShowLogo();

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
                    core.set_property_string("window-minimized", "yes");
                }
                else if (WindowState == FormWindowState.Normal)
                {
                    core.set_property_string("window-maximized", "no");
                    core.set_property_string("window-minimized", "no");
                }
                else if (WindowState == FormWindowState.Maximized)
                {
                    core.set_property_string("window-maximized", "yes");
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SaveWindowProperties();
            RegistryHelp.SetValue(App.RegPath, "Recent", RecentFiles.ToArray());

            if (core.IsQuitNeeded)
                core.commandv("quit");

            if (!core.ShutdownAutoResetEvent.WaitOne(10000))
                Msg.ShowError("Shutdown thread failed to complete within 10 seconds.");

            PowerShell.Shutdown();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (WindowState == FormWindowState.Normal &&
                e.Button == MouseButtons.Left && !IsMouseInOSC())
            {
                var HTCAPTION = new IntPtr(2);
                WinAPI.ReleaseCapture();
                WinAPI.PostMessage(Handle, 0xA1 /* WM_NCLBUTTONDOWN */, HTCAPTION, IntPtr.Zero);
            }

            if (Width - e.Location.X < 10 && e.Location.Y < 10)
                core.commandv("quit");
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
                core.LoadFiles(e.Data.GetData(DataFormats.FileDrop) as String[], true, Control.ModifierKeys.HasFlag(Keys.Control));
          
            if (e.Data.GetDataPresent(DataFormats.Text))
                core.LoadFiles(new[] { e.Data.GetData(DataFormats.Text).ToString() }, true, Control.ModifierKeys.HasFlag(Keys.Control));
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            CursorHelp.Show();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Alt)
                e.SuppressKeyPress = true; // prevent beep using alt key

            base.OnKeyDown(e);
        }
    }
}
