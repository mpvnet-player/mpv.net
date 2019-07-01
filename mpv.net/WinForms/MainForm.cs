using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd { get; set; }

        public new ContextMenuStripEx ContextMenu { get; set; }

        Point  LastCursorPosChanged;
        int    LastCursorChangedTickCount;
        bool   IgnoreDpiChanged = true;
        List<string> RecentFiles;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ThreadException += Application_ThreadException;
                Msg.SupportURL = "https://github.com/stax76/mpv.net#support";
                Instance = this;
                WPF.WPF.Init();
                System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                Hwnd = Handle;
                MinimumSize = new Size(FontHeight * 16, FontHeight * 9);
                Text += " " + Application.ProductVersion;

                object recent = RegHelp.GetObject(App.RegPath, "Recent");

                if (recent is string[] r)
                    RecentFiles = new List<string>(r);
                else
                    RecentFiles = new List<string>();

                var dummy = mp.Conf;
                App.ProcessCommandLineEarly();
                if (mp.Screen == -1) mp.Screen = Array.IndexOf(Screen.AllScreens, Screen.PrimaryScreen);
                SetScreen(mp.Screen);
                CycleFullscreen(mp.Fullscreen);
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }

        public MenuItem FindMenuItem(string text) => FindMenuItem(text, ContextMenu.Items);

        void Idle() => BeginInvoke(new Action(() => { Text = "mpv.net " + Application.ProductVersion; }));

        void CM_Popup(object sender, EventArgs e) => CursorHelp.Show();

        void VideoSizeChanged() => BeginInvoke(new Action(() => SetFormPosAndSize()));

        void Shutdown() => BeginInvoke(new Action(() => Close()));

        void PropChangeFullscreen(bool value) => BeginInvoke(new Action(() => CycleFullscreen(value)));

        void ContextMenu_Opened(object sender, EventArgs e) => CursorHelp.Show();

        bool IsFullscreen => WindowState == FormWindowState.Maximized;

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
                        mi.Action = () => { mp.commandv("set", "vid", track.ID.ToString()); };
                        mi.Checked = mp.Vid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (vidTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in audTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => { mp.commandv("set", "aid", track.ID.ToString()); };
                        mi.Checked = mp.Aid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in subTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => { mp.commandv("set", "sid", track.ID.ToString()); };
                        mi.Checked = mp.Sid == track.ID.ToString();
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (subTracks.Length > 0)
                    {
                        MenuItem mi = new MenuItem("S: No subtitles");
                        mi.Action = () => { mp.commandv("set", "sid", "no"); };
                        mi.Checked = mp.Sid == "no";
                        trackMenuItem.DropDownItems.Add(mi);
                    }

                    if (ediTracks.Length > 0)
                        trackMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    foreach (MediaTrack track in ediTracks)
                    {
                        MenuItem mi = new MenuItem(track.Text);
                        mi.Action = () => { mp.commandv("set", "edition", track.ID.ToString()); };
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
                        mi.Action = () => { mp.commandv("seek", i.Value.ToString(CultureInfo.InvariantCulture), "absolute"); };
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

        protected void SetScreen(int targetIndex)
        {
            Screen[] screens = Screen.AllScreens;
            if (targetIndex < 0) targetIndex = 0;
            if (targetIndex > screens.Length - 1) targetIndex = screens.Length - 1;
            SetScreen(screens[Array.IndexOf(screens, screens[targetIndex])]);
        }

        protected void SetScreen(Screen screen)
        {
            Rectangle target = screen.Bounds;
            Left = target.X + Convert.ToInt32((target.Width - Width) / 2.0);
            Top = target.Y + Convert.ToInt32((target.Height - Height) / 2.0);
            SetStartFormPositionAndSize();
        }

        void SetStartFormPositionAndSize()
        {
            if (IsFullscreen || mp.VideoSize.Width == 0) return;
            Screen screen = Screen.FromControl(this);
            int height = Convert.ToInt32(screen.Bounds.Height * mp.Autofit);
            int width = Convert.ToInt32(height * mp.VideoSize.Width / (double)mp.VideoSize.Height);
            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new Native.RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
            NativeHelp.AddWindowBorders(Handle, ref rect);
            int left = middlePos.X - rect.Width / 2;
            int top = middlePos.Y - rect.Height / 2;
            Native.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */, left, top, rect.Width, rect.Height, 4 /* SWP_NOZORDER */);
        }

        void SetFormPosAndSize()
        {
            if (IsFullscreen || mp.VideoSize.Width == 0) return;
            Screen screen = Screen.FromControl(this);
            int height = mp.VideoSize.Height;
            if (mp.RememberHeight) height = ClientSize.Height;
            if (height > screen.Bounds.Height * 0.9) height = Convert.ToInt32(screen.Bounds.Height * mp.Autofit);
            int width = Convert.ToInt32(height * mp.VideoSize.Width / (double)mp.VideoSize.Height);
            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new Native.RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
            NativeHelp.AddWindowBorders(Handle, ref rect);
            int left = middlePos.X - rect.Width / 2;
            int top = middlePos.Y - rect.Height / 2;
            Screen[] screens = Screen.AllScreens;
            Native.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */, left, top, rect.Width, rect.Height, 4 /* SWP_NOZORDER */);
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
                if (string.IsNullOrEmpty(item.Path)) continue;
                string path = item.Path.Replace("&", "&&");
                MenuItem menuItem = ContextMenu.Add(path, () => {
                    try {
                        mp.command_string(item.Command);
                    } catch (Exception ex) {
                        Msg.ShowException(ex);
                    }
                });
                if (menuItem != null) menuItem.ShortcutKeyDisplayString = item.Input + "    ";
            }
        }

        private void FileLoaded()
        {
            string path = mp.get_property_string("path");
            BeginInvoke(new Action(() => {
                if (File.Exists(path) || path.Contains("://"))
                    Text = Path.GetFileName(path) + " - mpv.net " + Application.ProductVersion;
                else
                    Text = "mpv.net " + Application.ProductVersion;
            }));
            if (RecentFiles.Contains(path)) RecentFiles.Remove(path);
            RecentFiles.Insert(0, path);
            if (RecentFiles.Count > 15) RecentFiles.RemoveAt(15);
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Msg.ShowException(e.Exception);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           Msg.ShowError(e.ExceptionObject.ToString());
        }
        
        public void CycleFullscreen(bool enabled)
        {
            if (enabled)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
            }
            else
            {
                WindowState = FormWindowState.Normal;

                if (mp.Border)
                    FormBorderStyle = FormBorderStyle.Sizable;
                else
                    FormBorderStyle = FormBorderStyle.None;

                SetFormPosAndSize();
            }
        }

        protected override void WndProc(ref Message m)
        {
            //System.Diagnostics.Debug.WriteLine(m);

            switch (m.Msg)
            {
                case 0x0201: // WM_LBUTTONDOWN
                case 0x0202: // WM_LBUTTONUP
                case 0x0100: // WM_KEYDOWN
                case 0x0101: // WM_KEYUP
                case 0x0104: // WM_SYSKEYDOWN
                case 0x0105: // WM_SYSKEYUP
                case 0x020A: // WM_MOUSEWHEEL
                case 0x0207: // WM_MBUTTONDOWN
                case 0x0208: // WM_MBUTTONUP
                    if (mp.WindowHandle != IntPtr.Zero)
                        Native.SendMessage(mp.WindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x0200: // WM_MOUSEMOVE
                    Point pos = PointToClient(Cursor.Position);
                    mp.command_string($"mouse {pos.X} {pos.Y}");
                    if (CursorHelp.IsPosDifferent(LastCursorPosChanged)) CursorHelp.Show();
                    break;
                case 0x2a3: // WM_MOUSELEAVE
                    mp.command_string("mouse 1 1"); // osc won't always auto hide
                    break;
                case 0x319: // WM_APPCOMMAND
                    if (mp.WindowHandle != IntPtr.Zero)
                        Native.PostMessage(mp.WindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x203: // Native.WM.LBUTTONDBLCLK
                    if (!IsMouseInOSC()) mp.command_string("cycle fullscreen");
                    break;
                case 0x02E0: // WM_DPICHANGED
                    if (IgnoreDpiChanged) break;
                    var r2 = Marshal.PtrToStructure<Native.RECT>(m.LParam);
                    Native.SetWindowPos(Handle, IntPtr.Zero, r2.Left, r2.Top, r2.Width, r2.Height, 0);
                    break;
                case 0x0214: // WM_SIZING
                    var rc = Marshal.PtrToStructure<Native.RECT>(m.LParam);
                    var r = rc;
                    NativeHelp.SubtractWindowBorders(Handle, ref r);
                    int c_w = r.Right - r.Left, c_h = r.Bottom - r.Top;
                    float aspect = mp.VideoSize.Width / (float)mp.VideoSize.Height;
                    int d_w = Convert.ToInt32(c_h * aspect - c_w);
                    int d_h = Convert.ToInt32(c_w / aspect - c_h);
                    int[] d_corners = { d_w, d_h, -d_w, -d_h };
                    int[] corners = { rc.Left, rc.Top, rc.Right, rc.Bottom };
                    int corner = NativeHelp.GetResizeBorder(m.WParam.ToInt32());

                    if (corner >= 0)
                        corners[corner] -= d_corners[corner];

                    Marshal.StructureToPtr<Native.RECT>(new Native.RECT(corners[0], corners[1], corners[2], corners[3]), m.LParam, false);
                    m.Result = new IntPtr(1);
                    return;
            }

            if (m.Msg == SingleProcess.Message)
            {
                object filesObject = RegHelp.GetObject(App.RegPath, "ShellFiles");

                if (filesObject is string[] files)
                {
                    switch (RegHelp.GetString(App.RegPath, "ProcessInstanceMode"))
                    {
                        case "single":
                            mp.Load(files, true, Control.ModifierKeys.HasFlag(Keys.Control));
                            break;
                        case "queue":
                            foreach (string file in files)
                                mp.commandv("loadfile", file, "append");
                            break;
                    }
                }

                RegHelp.RemoveValue(App.RegPath, "ShellFiles");
                Activate();
                return;
            }

            base.WndProc(ref m);
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

        void Timer_Tick(object sender, EventArgs e)
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

        void PropChangeOnTop(bool value) => BeginInvoke(new Action(() => TopMost = value));

        void PropChangeAid(string value) => mp.Aid = value;

        void PropChangeSid(string value) => mp.Sid = value;

        void PropChangeVid(string value) => mp.Vid = value;

        void PropChangeEdition(int value) => mp.Edition = value;

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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mp.Init();
            mp.observe_property_bool("fullscreen", PropChangeFullscreen);
            mp.observe_property_bool("ontop", PropChangeOnTop);
            mp.observe_property_bool("border", PropChangeBorder);
            mp.observe_property_string("sid", PropChangeSid);
            mp.observe_property_string("aid", PropChangeAid);
            mp.observe_property_string("vid", PropChangeVid);
            mp.observe_property_int("edition", PropChangeEdition);
            mp.Shutdown += Shutdown;
            mp.VideoSizeChanged += VideoSizeChanged;
            mp.FileLoaded += FileLoaded;
            mp.Idle += Idle;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (App.IsDarkMode) ToolStripRendererEx.ColorTheme = Color.Black;
            ContextMenu = new ContextMenuStripEx(components);
            ContextMenu.Opened += ContextMenu_Opened;
            ContextMenu.Opening += ContextMenu_Opening;
            BuildMenu();
            ContextMenuStrip = ContextMenu;
            IgnoreDpiChanged = false;
            CheckUrlInClipboard();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            CheckUrlInClipboard();
            Message m = new Message() { Msg = 0x0202 }; // WM_LBUTTONUP
            Native.SendMessage(Handle, m.Msg, m.WParam, m.LParam);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (mp.IsLogoVisible) mp.ShowLogo();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            RegHelp.SetObject(App.RegPath, "Recent", RecentFiles.ToArray());
            App.Exit();
            mp.commandv("quit");
            mp.AutoResetEvent.WaitOne(3000);
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

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            CursorHelp.Show();
        }

        void CheckUrlInClipboard()
        {
            string clipboard = Clipboard.GetText();

            foreach (string url in App.UrlWhitelist)
            {
                if (clipboard.Contains("://") && ! clipboard.Contains("\n") &&
                    ! clipboard.Contains(" ") && clipboard.Contains(url.ToLower()) &&
                    RegHelp.GetString(App.RegPath, "LastURL") != clipboard && Visible)
                {
                    RegHelp.SetObject(App.RegPath, "LastURL", clipboard);

                    if (Msg.ShowQuestion("Play URL?", clipboard) == MsgResult.OK)
                        mp.Load(new[] { clipboard }, true, Control.ModifierKeys.HasFlag(Keys.Control));
                }
            }
        }
    }
}