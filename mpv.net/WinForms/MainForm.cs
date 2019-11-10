
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

using UI;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd { get; set; }
        public new ContextMenuStripEx ContextMenu { get; set; }
        Point  LastCursorPosChanged;
        int    LastCursorChangedTickCount;
        int    TaskbarButtonCreatedMessage;
        bool   WasShown;
        DateTime LastCycleFullscreen;
        Taskbar  Taskbar;
        List<string> RecentFiles;

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
                mp.Init();

                mp.Shutdown += Shutdown;
                mp.VideoSizeChanged += VideoSizeChanged;
                mp.FileLoaded += FileLoaded;
                mp.Idle += Idle;
                mp.Seek += () => UpdateProgressBar();

                mp.observe_property_bool("pause", PropChangePause);
                mp.observe_property_bool("fullscreen", PropChangeFullscreen);
                mp.observe_property_bool("ontop", PropChangeOnTop);
                mp.observe_property_bool("border", PropChangeBorder);
                mp.observe_property_string("sid", PropChangeSid);
                mp.observe_property_string("aid", PropChangeAid);
                mp.observe_property_string("vid", PropChangeVid);
                mp.observe_property_int("edition", PropChangeEdition);
                mp.observe_property_double("window-scale", PropChangeWindowScale);
                
                if (mp.GPUAPI != "vulkan")
                    mp.ProcessCommandLine(false);

                AppDomain.CurrentDomain.UnhandledException += (sender, e) => Msg.ShowError(e.ExceptionObject.ToString());
                Application.ThreadException += (sender, e) => Msg.ShowException(e.Exception);
                Msg.SupportURL = "https://github.com/stax76/mpv.net#support";
                Text = "mpv.net " + Application.ProductVersion;
                TaskbarButtonCreatedMessage = Native.RegisterWindowMessage("TaskbarButtonCreated");
                
                ContextMenu = new ContextMenuStripEx(components);
                ContextMenu.Opened += ContextMenu_Opened;
                ContextMenu.Opening += ContextMenu_Opening;

                if (mp.Screen == -1)
                    mp.Screen = Array.IndexOf(Screen.AllScreens, Screen.PrimaryScreen);

                int targetIndex = mp.Screen;
                Screen[] screens = Screen.AllScreens;

                if (targetIndex < 0)
                    targetIndex = 0;

                if (targetIndex > screens.Length - 1)
                    targetIndex = screens.Length - 1;

                Screen screen = screens[Array.IndexOf(screens, screens[targetIndex])];
                Rectangle target = screen.Bounds;
                Left = target.X + (target.Width - Width) / 2;
                Top = target.Y + (target.Height - Height) / 2;

                int posX = RegistryHelp.GetInt(App.RegPath, "PosX");
                int posY = RegistryHelp.GetInt(App.RegPath, "PosY");

                if (posX != 0 && posY != 0 && App.RememberPosition)
                {
                    Left = posX - Width / 2;
                    Top = posY - Height / 2;
                }

                if (App.Maximized)
                {
                    SetFormPosAndSize(1, true);
                    WindowState = FormWindowState.Maximized;
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

        void CM_Popup(object sender, EventArgs e) => CursorHelp.Show();

        void VideoSizeChanged() => BeginInvoke(new Action(() => SetFormPosAndSize()));

        void PropChangeFullscreen(bool value) => BeginInvoke(new Action(() => CycleFullscreen(value)));

        void ContextMenu_Opened(object sender, EventArgs e) => CursorHelp.Show();

        bool IsFullscreen => WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None;

        bool IsMouseInOSC() => PointToClient(Control.MousePosition).Y > ClientSize.Height * 0.9;

        void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            lock (mp.MediaTracks)
            {
                MenuItem trackMenuItem = FindMenuItem("Track");

                if (trackMenuItem != null)
                {
                    trackMenuItem.DropDownItems.Clear();

                    MediaTrack[] audTracks = mp.MediaTracks.Where(track => track.Type == "a").ToArray();
                    MediaTrack[] subTracks = mp.MediaTracks.Where(track => track.Type == "s").ToArray();
                    MediaTrack[] vidTracks = mp.MediaTracks.Where(track => track.Type == "v").ToArray();
                    MediaTrack[] ediTracks = mp.MediaTracks.Where(track => track.Type == "e").ToArray();

                    foreach (MediaTrack track in vidTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => mp.commandv("set", "vid", track.ID.ToString());
                        mi.Checked = mp.Vid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (vidTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in audTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => mp.commandv("set", "aid", track.ID.ToString());
                        mi.Checked = mp.Aid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in subTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => mp.commandv("set", "sid", track.ID.ToString());
                        mi.Checked = mp.Sid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                    {
                        MenuItem mi = new MenuItem("S: No subtitles");
                        mi.Action = () => mp.commandv("set", "sid", "no");
                        mi.Checked = mp.Sid == "no";
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (ediTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in ediTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => mp.commandv("set", "edition", track.ID.ToString());
                        mi.Checked = mp.Edition == track.ID;
                        trackMenuItem.DropDownItems.Add(mi);
                    }
                }
            }

            lock (mp.Chapters)
            {
                MenuItem chaptersMenuItem = FindMenuItem("Chapters");

                if (chaptersMenuItem != null)
                {
                    chaptersMenuItem.DropDownItems.Clear();

                    foreach (var i in mp.Chapters)
                    {
                        MenuItem mi = new MenuItem(i.Key);
                        mi.ShortcutKeyDisplayString = TimeSpan.FromSeconds(i.Value).ToString().Substring(0, 8) + "     ";
                        mi.Action = () => mp.commandv("seek", i.Value.ToString(CultureInfo.InvariantCulture), "absolute");
                        chaptersMenuItem.DropDownItems.Add(mi);
                    }
                }
            }

            MenuItem recent = FindMenuItem("Recent");

            if (recent != null)
            {
                recent.DropDownItems.Clear();

                foreach (string path in RecentFiles)
                    MenuItem.Add(recent.DropDownItems, path, () => mp.Load(new[] { path }, true, Control.ModifierKeys.HasFlag(Keys.Control)));
               
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

        bool WasInitialSizeSet;

        void SetFormPosAndSize(double scale = 1, bool force = false)
        {
            if (!force)
            {
                if (WindowState == FormWindowState.Maximized)
                    return;

                if (mp.Fullscreen)
                {
                    CycleFullscreen(true);
                    return;
                }
            }
            
            Screen screen = Screen.FromControl(this);
            int autoFitHeight = Convert.ToInt32(screen.WorkingArea.Height * mp.Autofit);

            if (mp.VideoSize.Height == 0 || mp.VideoSize.Width == 0 ||
                mp.VideoSize.Width / (float)mp.VideoSize.Height < App.MinimumAspectRatio)

                mp.VideoSize = new Size((int)(autoFitHeight * (16 / 9.0)), autoFitHeight);

            Size size = mp.VideoSize;
            int height = size.Height;

            if (App.RememberHeight)
            {
                if (WasInitialSizeSet)
                    height = ClientSize.Height;
                else
                {
                    height = autoFitHeight;
                    WasInitialSizeSet = true;
                }
            }

            height = Convert.ToInt32(height * scale);
            int width = Convert.ToInt32(height * size.Width / (double)size.Height);

            if (height > screen.WorkingArea.Height * 0.9)
            {
                height = Convert.ToInt32(screen.WorkingArea.Height * 0.9);
                width = Convert.ToInt32(height * size.Width / (double)size.Height);
            }

            if (width > screen.WorkingArea.Width * 0.9)
            {
                width = Convert.ToInt32(screen.WorkingArea.Width * 0.9);
                height = Convert.ToInt32(width * size.Height / (double)size.Width);
            }

            if (height < screen.WorkingArea.Height * mp.AutofitSmaller)
            {
                height = Convert.ToInt32(screen.WorkingArea.Height * mp.AutofitSmaller);
                width = Convert.ToInt32(height * size.Width / (double)size.Height);
            }

            if (height > screen.WorkingArea.Height * mp.AutofitLarger)
            {
                height = Convert.ToInt32(screen.WorkingArea.Height * mp.AutofitLarger);
                width = Convert.ToInt32(height * size.Width / (double)size.Height);
            }

            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new Native.RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
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

            Native.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */, left, top, rect.Width, rect.Height, 4 /* SWP_NOZORDER */);
        }

        public void CycleFullscreen(bool enabled)
        {
            LastCycleFullscreen = DateTime.Now;
            mp.Fullscreen = enabled;

            if (enabled)
            {
                if (WindowState != FormWindowState.Maximized || FormBorderStyle != FormBorderStyle.None)
                {
                    if (WindowState == FormWindowState.Maximized)
                        WindowState = FormWindowState.Minimized;

                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
            }
            else
            {
                if (WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None)
                {
                    WindowState = FormWindowState.Normal;

                    if (mp.Border)
                        FormBorderStyle = FormBorderStyle.Sizable;
                    else
                        FormBorderStyle = FormBorderStyle.None;                      

                    SetFormPosAndSize();
                }
            }
        }

        public void BuildMenu()
        {
            string content = File.ReadAllText(mp.InputConfPath);
            var items = CommandItem.GetItems(content);

            if (!content.Contains("#menu:"))
            {
                var defaultItems = CommandItem.GetItems(Properties.Resources.inputConf);

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
                        mp.command(item.Command);
                    } catch (Exception ex) {
                        Msg.ShowException(ex);
                    }
                });

                if (menuItem != null)
                    menuItem.ShortcutKeyDisplayString = item.Input + "    ";
            }
        }

        private void FileLoaded()
        {
            string path = mp.get_property_string("path");

            BeginInvoke(new Action(() => {
                if (path.Contains("://"))
                    Text = mp.get_property_string("media-title") + " - mpv.net " + Application.ProductVersion;
                else if (path.Contains(":\\") || path.StartsWith("\\\\"))
                    Text = path.FileName() + " - mpv.net " + Application.ProductVersion;
                else
                    Text = "mpv.net " + Application.ProductVersion;

                int interval = (int)(mp.Duration.TotalMilliseconds / 100);
                if (interval < 100) interval = 100;
                if (interval > 1000) interval = 1000;
                ProgressTimer.Interval = interval;
                UpdateProgressBar();
            }));

            if (RecentFiles.Contains(path))
                RecentFiles.Remove(path);

            RecentFiles.Insert(0, path);

            while (RecentFiles.Count > App.RecentCount)
                RecentFiles.RemoveAt(App.RecentCount);
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
                case 0x0201: // WM_LBUTTONDOWN
                case 0x0202: // WM_LBUTTONUP
                case 0x0207: // WM_MBUTTONDOWN
                case 0x0208: // WM_MBUTTONUP
                case 0x020b: // WM_XBUTTONDOWN
                case 0x020c: // WM_XBUTTONUP
                case 0x020A: // WM_MOUSEWHEEL
                case 0x0100: // WM_KEYDOWN
                case 0x0101: // WM_KEYUP
                case 0x0104: // WM_SYSKEYDOWN
                case 0x0105: // WM_SYSKEYUP
                case 0x319:  // WM_APPCOMMAND
                    if (mp.WindowHandle != IntPtr.Zero)
                        Native.SendMessage(mp.WindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x0200: // WM_MOUSEMOVE
                    {
                        if ((DateTime.Now - LastCycleFullscreen).TotalMilliseconds > 500)
                        {
                            Point pos = PointToClient(Cursor.Position);
                            mp.command($"mouse {pos.X} {pos.Y}");
                        }

                        if (CursorHelp.IsPosDifferent(LastCursorPosChanged))
                            CursorHelp.Show();
                    }
                    break;
                case 0x2a3: // WM_MOUSELEAVE
                    // osc won't always auto hide
                    mp.command("mouse 1 1");
                    break;
                case 0x203: // WM_LBUTTONDBLCLK
                    {
                        Point pos = PointToClient(Cursor.Position);
                        mp.command($"mouse {pos.X} {pos.Y} 0 double");
                    }
                    break;
                case 0x02E0: // WM_DPICHANGED
                    {
                        if (!WasShown)
                            break;

                        Native.RECT rect = Marshal.PtrToStructure<Native.RECT>(m.LParam);
                        Native.SetWindowPos(Handle, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, 0);
                    }
                    break;
                case 0x0214: // WM_SIZING
                    {
                        var rc = Marshal.PtrToStructure<Native.RECT>(m.LParam);
                        var r = rc;
                        NativeHelp.SubtractWindowBorders(Handle, ref r);
                        int c_w = r.Right - r.Left, c_h = r.Bottom - r.Top;
                        Size s = mp.VideoSize;

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

                        Marshal.StructureToPtr<Native.RECT>(new Native.RECT(corners[0], corners[1], corners[2], corners[3]), m.LParam, false);
                        m.Result = new IntPtr(1);
                    }
                    return;
                case 0x004A: // WM_COPYDATA
                    {
                        var copyData = (Native.COPYDATASTRUCT)m.GetLParam(typeof(Native.COPYDATASTRUCT));
                        string[] files = copyData.lpData.Split('\n');
                        string mode = files[0];
                        files = files.Skip(1).ToArray();

                        switch (mode)
                        {
                            case "single":
                                mp.Load(files, true, Control.ModifierKeys.HasFlag(Keys.Control));
                                break;
                            case "queue":
                                foreach (string file in files)
                                    mp.commandv("loadfile", file, "append");
                                break;
                        }

                        Activate();
                    }
                    return;
            }

            if (m.Msg == TaskbarButtonCreatedMessage && mp.TaskbarProgress)
            {
                Taskbar = new Taskbar(Handle);
                ProgressTimer.Start();
            }

            base.WndProc(ref m);
        }

        void CursorTimer_Tick(object sender, EventArgs e)
        {
            if (CursorHelp.IsPosDifferent(LastCursorPosChanged))
            {
                LastCursorPosChanged = Control.MousePosition;
                LastCursorChangedTickCount = Environment.TickCount;
            }
            else if (Environment.TickCount - LastCursorChangedTickCount > 1500 &&
                !IsMouseInOSC() && ClientRectangle.Contains(PointToClient(MousePosition)) &&
                Form.ActiveForm == this && !ContextMenu.Visible)

                CursorHelp.Hide();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e) => UpdateProgressBar();

        void UpdateProgressBar()
        {
            if (mp.TaskbarProgress && Taskbar != null)
                Taskbar.SetValue(mp.get_property_number("time-pos"), mp.Duration.TotalSeconds);
        }

        void PropChangeOnTop(bool value) => BeginInvoke(new Action(() => TopMost = value));

        void PropChangeAid(string value) => mp.Aid = value;

        void PropChangeSid(string value) => mp.Sid = value;

        void PropChangeVid(string value) => mp.Vid = value;

        void PropChangeEdition(int value) => mp.Edition = value;
        
        void PropChangeWindowScale(double value)
        {
            if (value != 1)
            {
                BeginInvoke(new Action(() => SetFormPosAndSize(value)));
                mp.command("no-osd set window-scale 1");
            }
        }

        void PropChangeBorder(bool enabled) {
            mp.Border = enabled;

            BeginInvoke(new Action(() => {
                if (!IsFullscreen)
                {
                    if (mp.Border && FormBorderStyle == FormBorderStyle.None)
                        FormBorderStyle = FormBorderStyle.Sizable;

                    if (!mp.Border && FormBorderStyle == FormBorderStyle.Sizable)
                        FormBorderStyle = FormBorderStyle.None;
                }
            }));
        }

        void PropChangePause(bool enabled)
        {
            if (Taskbar != null && mp.TaskbarProgress)
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

            if (mp.GPUAPI != "vulkan")
                mp.VideoSizeAutoResetEvent.WaitOne(App.StartThreshold);

            LastCycleFullscreen = DateTime.Now;
            SetFormPosAndSize();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (mp.GPUAPI == "vulkan")
                mp.ProcessCommandLine(false);

            ToolStripRendererEx.ForegroundColor = Theme.Current.GetWinFormsColor("menu-foreground");
            ToolStripRendererEx.BackgroundColor = Theme.Current.GetWinFormsColor("menu-background");
            ToolStripRendererEx.SelectionColor = Theme.Current.GetWinFormsColor("menu-highlight");
            ToolStripRendererEx.BorderColor = Theme.Current.GetWinFormsColor("menu-border");
            ToolStripRendererEx.CheckedColor = Theme.Current.GetWinFormsColor("menu-checked");

            BuildMenu();
            ContextMenuStrip = ContextMenu;
            WPF.WPF.Init();
            System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y);
            UpdateCheck.DailyCheck();
            mp.LoadScripts();
            Task.Run(() => App.Extension = new Extension());
            WasShown = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (mp.IsLogoVisible)
                mp.ShowLogo();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (WindowState == FormWindowState.Normal)
            {
                RegistryHelp.SetValue(App.RegPath, "PosX", Left + Width / 2);
                RegistryHelp.SetValue(App.RegPath, "PosY", Top + Height / 2);
            }

            RegistryHelp.SetValue(App.RegPath, "Recent", RecentFiles.ToArray());

            if (mp.IsQuitNeeded)
                mp.commandv("quit");

            if (!mp.ShutdownAutoResetEvent.WaitOne(10000))
                Msg.ShowError("Shutdown thread failed to complete within 10 seconds.");
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (WindowState == FormWindowState.Normal &&
                e.Button == MouseButtons.Left &&
                e.Y < ClientSize.Height * 0.9)
            {
                var HTCAPTION = new IntPtr(2);
                Native.ReleaseCapture();
                Native.PostMessage(Handle, 0xA1 /* WM_NCLBUTTONDOWN */, HTCAPTION, IntPtr.Zero);
            }

            if (Width - e.Location.X < 10 && e.Location.Y < 10)
                mp.commandv("quit");
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
                mp.Load(e.Data.GetData(DataFormats.FileDrop) as String[], true, Control.ModifierKeys.HasFlag(Keys.Control));
          
            if (e.Data.GetDataPresent(DataFormats.Text))
                mp.Load(new[] { e.Data.GetData(DataFormats.Text).ToString() }, true, Control.ModifierKeys.HasFlag(Keys.Control));
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            CursorHelp.Show();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.SuppressKeyPress = true; // prevent beep using alt key
            base.OnKeyDown(e);
        }
    }
}