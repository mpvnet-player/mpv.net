using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using static mpvnet.StaticUsing;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd;

        private Point LastCursorPosChanged;
        private int LastCursorChangedTickCount;
        private bool IsClosed;

        public ContextMenuStripEx CMS;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                Application.ThreadException += Application_ThreadException;
                SetFormPosSize();
                Instance = this;
                Hwnd = Handle;
                ChangeFullscreen((mp.mpvConv.ContainsKey("fullscreen") && mp.mpvConv["fullscreen"] == "yes") || (mp.mpvConv.ContainsKey("fs") && mp.mpvConv["fs"] == "yes"));
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void BuildMenu()
        {
            foreach (var i in File.ReadAllText(mp.InputConfPath).SplitLinesNoEmpty())
            {
                if (!i.Contains("#menu:"))
                    continue;

                var left = i.Left("#menu:").Trim();

                if (left.StartsWith("#"))
                    continue;

                var cmd = left.Right(" ").Trim();
                var menu = i.Right("#menu:").Trim();
                var key = menu.Left(";").Trim();
                var path = menu.Right(";").Trim();

                if (path == "" || cmd == "")
                    continue;

                var menuItem = CMS.Add(path, () => {
                    try
                    {
                        mp.CommandString(cmd, false);
                    }
                    catch (Exception e)
                    {
                        MsgError(e.ToString());
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
            var fn = mp.GetStringProp("filename");
            BeginInvoke(new Action(() => { Text = fn + " - mpv.net " + Application.ProductVersion; }));
            var fp = mp.mpvConfFolderPath + "history.txt";

            if (LastHistory != fn && File.Exists(fp))
            {
                File.AppendAllText(fp, DateTime.Now.ToString() + " " + Path.GetFileNameWithoutExtension(fn) + "\r\n");
                LastHistory = fn;
            }
        }

        private void CM_Popup(object sender, EventArgs e)
        {
            CursorHelp.Show();
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        void HandleException(Exception e)
        {
            MsgError(e.ToString());
        }

        private void mp_VideoSizeChanged()
        {
            BeginInvoke(new Action(() => SetFormPosSize()));
        }

        private void mp_Shutdown()
        {
            if (!IsClosed)
                BeginInvoke(new Action(() => Close()));
        }

        public bool IsFullscreen
        {
            get => WindowState == FormWindowState.Maximized;
        }

        void mp_ChangeFullscreen(bool value)
        {
            BeginInvoke(new Action(() => ChangeFullscreen(value)));
        }

        void ChangeFullscreen(bool value)
        {
            if (value)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;
                SetFormPosSize();
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
                case 0x203: // Native.WM.LBUTTONDBLCLK
                    if (!IsMouseInOSC())
                        mp.CommandString("cycle fullscreen");
                    break;
                case 0x0214: // WM_SIZING
                    var rc = Marshal.PtrToStructure<Native.RECT>(m.LParam);
                    var r = rc;
                    NativeHelp.SubtractWindowBorders(Handle, ref r);
                    int c_w = r.Right - r.Left, c_h = r.Bottom - r.Top;
                    float aspect = mp.VideoSize.Width / (float)mp.VideoSize.Height;
                    int d_w = (int)(c_h * aspect - c_w);
                    int d_h = (int)(c_w / aspect - c_h);
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

        void SetFormPosSize()
        {
            if (IsFullscreen || mp.VideoSize.Width == 0) return;
            var wa = Screen.GetWorkingArea(this);
            int h = (int)(wa.Height * 0.6);
            int w = (int)(h * mp.VideoSize.Width / (float)mp.VideoSize.Height);
            Point middlePos = new Point(Left + Width / 2, Top + Height / 2);
            var r = new Native.RECT(new Rectangle(0, 0, w, h));
            NativeHelp.AddWindowBorders(Handle, ref r);
            int l = middlePos.X - r.Width / 2;
            int t = middlePos.Y - r.Height / 2;
            if (l < 0) l = 0;
            if (t < 0) t = 0;
            if (l + r.Width > wa.Width ) l = wa.Width - r.Width;
            if (t + r.Height > wa.Height ) t = wa.Height - r.Height;
            Native.SetWindowPos(Handle, IntPtr.Zero /* HWND_TOP */, l, t, r.Width, r.Height, 4 /* SWP_NOZORDER */);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                mp.LoadFiles(e.Data.GetData(DataFormats.FileDrop) as String[]);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // window-dragging
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
                mp.Command("quit");
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // send mouse command to make OSC show
            mp.CommandString($"mouse {e.X} {e.Y}");

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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mp.Init();
            mp.ObserveBoolProp("fullscreen", mp_ChangeFullscreen);
            mp.Shutdown += mp_Shutdown;
            mp.VideoSizeChanged += mp_VideoSizeChanged;
            mp.PlaybackRestart += mp_PlaybackRestart;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CMS = new ContextMenuStripEx(components);
            CMS.Opened += CMS_Opened;
            ContextMenuStrip = CMS;
            BuildMenu();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            IsClosed = true;
            mp.Command("quit");

            for (int i = 0; i < 99; i++)
            {
                if (mp.IsShutdownComplete) break;
                Thread.Sleep(100);
            }
        }
    }
}