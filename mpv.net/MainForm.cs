using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd;

        private Point LastCursorPosChanged;
        private int LastCursorChangedTickCount;
        private bool IgnoreDpiChanged = true;

        public ContextMenuStripEx CMS;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                Application.ThreadException += Application_ThreadException;
                Instance = this;
                Hwnd = Handle;
                Text += " " + Application.ProductVersion;

                if (mp.mpvConf.ContainsKey("screen"))
                    SetScreen(Convert.ToInt32(mp.mpvConf["screen"]));
                else
                    SetScreen(Screen.PrimaryScreen);

                ChangeFullscreen((mp.mpvConf.ContainsKey("fullscreen") && mp.mpvConf["fullscreen"] == "yes") ||
                                 (mp.mpvConf.ContainsKey("fs") && mp.mpvConf["fs"] == "yes"));

                ProcessCommandLineEarly();
            }
            catch (Exception e)
            {
                MainForm.Instance.ShowMsgBox(e.ToString(), MessageBoxIcon.Error);
            }
        }

        protected void SetScreen(int targetIndex)
        {
            Screen[] screens = Screen.AllScreens;
            if (targetIndex < 0 || targetIndex > screens.Length - 1) return;
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
            int height = Convert.ToInt32(screen.Bounds.Height * 0.6);
            int width = Convert.ToInt32(height * mp.VideoSize.Width / (double)mp.VideoSize.Height);
            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new Native.RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
            NativeHelp.AddWindowBorders(Handle, ref rect);
            int left = middlePos.X - rect.Width / 2;
            int top = middlePos.Y - rect.Height / 2;
            Native.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */, left, top, rect.Width, rect.Height, 4 /* SWP_NOZORDER */);
        }

        void SetFormPositionAndSizeKeepHeight()
        {
            if (IsFullscreen || mp.VideoSize.Width == 0) return;
            Screen screen = Screen.FromControl(this);
            int height = ClientSize.Height;
            int width = Convert.ToInt32(height * mp.VideoSize.Width / (double)mp.VideoSize.Height);
            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var rect = new Native.RECT(new Rectangle(screen.Bounds.X, screen.Bounds.Y, width, height));
            NativeHelp.AddWindowBorders(Handle, ref rect);
            int left = middlePos.X - rect.Width / 2;
            int top = middlePos.Y - rect.Height / 2;
            Screen[] screens = Screen.AllScreens;

            if (left < screens[0].Bounds.Left)
                left = screens[0].Bounds.Left;

            int maxLeft = screens[0].Bounds.Left + screens.Select((sc) => sc.Bounds.Width).Sum() - rect.Width - SystemInformation.CaptionHeight;

            if (left > maxLeft)
                left = maxLeft;

            Native.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */, left, top, rect.Width, rect.Height, 4 /* SWP_NOZORDER */);
        }

        protected void ProcessCommandLineEarly()
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            foreach (string i in args)
            {
                if (i.StartsWith("--"))
                {
                    if (i.Contains("="))
                    {
                        string left = i.Substring(2, i.IndexOf("=") - 2);
                        string right = i.Substring(left.Length + 3);
                        
                        if (left == "screen")
                            SetScreen(Convert.ToInt32(right));

                        ChangeFullscreen((left == "fs" || left == "fullscreen") && right == "yes");
                    }
                    else
                    {
                        string switchName = i.Substring(2);

                        switch (switchName)
                        {
                            case "fs":
                            case "fullscreen":
                                ChangeFullscreen(true);
                                break;
                        }
                    }
                }
            }
        }

        public void BuildMenu()
        {
            foreach (var i in File.ReadAllText(mp.InputConfPath).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (!i.Contains("#menu:")) continue;
                var left = i.Substring(0, i.IndexOf("#menu:")).Trim();
                if (left.StartsWith("#")) continue;
                var cmd = left.Substring(left.IndexOf(" ") + 1).Trim();
                var menu = i.Substring(i.IndexOf("#menu:") + "#menu:".Length).Trim();
                var key = left.Substring(0, left.IndexOf(" "));
                if (key == "_") key = "";
                if (menu.Contains(";")) key = menu.Substring(0, menu.IndexOf(";")).Trim();
                var path = menu.Substring(menu.IndexOf(";") + 1).Trim().Replace("&", "&&");
                if (path == "" || cmd == "") continue;

                var menuItem = CMS.Add(path, () => {
                    try
                    {
                        mp.command_string(cmd);
                    }
                    catch (Exception e)
                    {
                        MainForm.Instance.ShowMsgBox(e.ToString(), MessageBoxIcon.Error);
                    }
                });
                
                if (menuItem != null)
                    menuItem.ShortcutKeyDisplayString = key.Replace("_","") + "   ";
            }
        }

        private void CMS_Opened(object sender, EventArgs e)
        {
            CursorHelp.Show();
        }

        private string LastHistory;

        private void mp_PlaybackRestart()
        {
            var filename = mp.get_property_string("filename");
            BeginInvoke(new Action(() => { Text = filename + " - mpv.net " + Application.ProductVersion; }));
            var historyFilepath = mp.mpvConfFolderPath + "history.txt";

            if (LastHistory != filename && File.Exists(historyFilepath))
            {
                File.AppendAllText(historyFilepath, DateTime.Now.ToString() + " " +
                    Path.GetFileNameWithoutExtension(filename) + "\r\n");
                LastHistory = filename;
            }
        }

        private void Mp_Idle()
        {
            BeginInvoke(new Action(() => { Text = "mpv.net " + Application.ProductVersion; }));
        }

        private void CM_Popup(object sender, EventArgs e)
        {
            CursorHelp.Show();
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowMsgBox(e.Exception.ToString(), MessageBoxIcon.Error);
        }

        private void mp_VideoSizeChanged()
        {
            BeginInvoke(new Action(() => SetFormPositionAndSizeKeepHeight()));
        }

        private void mp_Shutdown()
        {
            BeginInvoke(new Action(() => Close()));  
        }

        public bool IsFullscreen => WindowState == FormWindowState.Maximized;

        void mp_ChangeFullscreen(bool value)
        {
            BeginInvoke(new Action(() => ChangeFullscreen(value)));
        }

        void ChangeFullscreen(bool value)
        {
            if (value)
            {
                if (FormBorderStyle != FormBorderStyle.None)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
            }
            else
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;
                SetFormPositionAndSizeKeepHeight();
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0201: // WM_LBUTTONDOWN
                case 0x0202: // WM_LBUTTONUP
                case 0x0100: // WM_KEYDOWN
                case 0x0101: // WM_KEYUP
                case 0x020A: // WM_MOUSEWHEEL
                    if (mp.MpvWindowHandle != IntPtr.Zero)
                        Native.SendMessage(mp.MpvWindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x319: // WM_APPCOMMAND
                    if (mp.MpvWindowHandle != IntPtr.Zero)
                        Native.PostMessage(mp.MpvWindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x0104: // WM_SYSKEYDOWN:
                    if (mp.MpvWindowHandle != IntPtr.Zero)
                        Native.SendMessage(mp.MpvWindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x0105: // WM_SYSKEYUP:
                    if (mp.MpvWindowHandle != IntPtr.Zero)
                        Native.SendMessage(mp.MpvWindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x203: // Native.WM.LBUTTONDBLCLK
                    if (!IsMouseInOSC())
                        mp.command_string("cycle fullscreen");
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
                mp.LoadFiles(e.Data.GetData(DataFormats.FileDrop) as String[]);
            if (e.Data.GetDataPresent(DataFormats.Text))
                mp.LoadURL(e.Data.GetData(DataFormats.Text).ToString());
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

            var sb = Screen.FromControl(this).Bounds;
            var p1 = new Point(sb.Width, 0);
            var p2 = PointToScreen(e.Location);

            if (Math.Abs(p1.X - p2.X) < 10 && Math.Abs(p1.Y - p2.Y) < 10)
                mp.commandv("quit");
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mp.command_string($"mouse {e.X} {e.Y}");

            if (CursorHelp.IsPosDifferent(LastCursorPosChanged))
                CursorHelp.Show();
        }

        bool IsMouseInOSC()
        {
            return PointToClient(Control.MousePosition).Y > ClientSize.Height * 0.9;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (CursorHelp.IsPosDifferent(LastCursorPosChanged))
            {
                LastCursorPosChanged = Control.MousePosition;
                LastCursorChangedTickCount = Environment.TickCount;
            }
            else if (Environment.TickCount - LastCursorChangedTickCount > 1500 &&
                !IsMouseInOSC() && ClientRectangle.Contains(PointToClient(MousePosition)) &&
                Form.ActiveForm == this && !CMS.Visible)
            {
                CursorHelp.Hide();
            }
        }

        public DialogResult ShowMsgBox(string message, MessageBoxIcon icon)
        {
            var buttons = MessageBoxButtons.OK;
            if (icon == MessageBoxIcon.Question) buttons = MessageBoxButtons.OKCancel;

            var fn = new Func<DialogResult>(() => MessageBox.Show(
                message, Application.ProductName, buttons, MessageBoxIcon.Information));

            return (DialogResult)Invoke(fn);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mp.Init();
            mp.observe_property_bool("fullscreen", mp_ChangeFullscreen);
            mp.Shutdown += mp_Shutdown;
            mp.VideoSizeChanged += mp_VideoSizeChanged;
            mp.PlaybackRestart += mp_PlaybackRestart;
            mp.Idle += Mp_Idle;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CMS = new ContextMenuStripEx(components);
            CMS.Opened += CMS_Opened;
            ContextMenuStrip = CMS;
            BuildMenu();
            IgnoreDpiChanged = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            mp.commandv("quit");
            mp.AutoResetEvent.WaitOne(3000); 
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            CursorHelp.Show();
        }
    }
}