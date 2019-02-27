using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using vbnet;
using vbnet.UI;
using static vbnet.UI.MainModule;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd;

        private Point LastCursorPosChanged;
        private int LastCursorChangedTickCount;
        private bool IsCloseRequired = true;

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
                ChangeFullscreen((mpv.mpvConv.ContainsKey("fullscreen") && mpv.mpvConv["fullscreen"] == "yes") || (mpv.mpvConv.ContainsKey("fs") && mpv.mpvConv["fs"] == "yes"));
                ToolStripManager.Renderer = new ToolStripRendererEx(ToolStripRenderModeEx.SystemDefault);
                CMS = new ContextMenuStripEx(components);
                CMS.Opened += CMS_Opened;
                ContextMenuStrip = CMS;
                BuildMenu();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void BuildMenu()
        {
            if (!File.Exists(mpv.InputConfPath))
            {
                var dirPath = Folder.AppDataRoaming + "mpv\\";

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                File.WriteAllText(mpv.InputConfPath, Properties.Resources.input_conf);
            }

            foreach (var i in File.ReadAllText(mpv.InputConfPath).SplitLinesNoEmpty())
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
                        mpv.CommandString(cmd, false);
                    }
                    catch (Exception e)
                    {
                        MsgException(e);
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

        private void mpv_PlaybackRestart()
        {
            var fn = mpv.GetStringProp("filename");
            BeginInvoke(new Action(() => { Text = fn + " - mpv.net " + Application.ProductVersion; }));
            var fp = Folder.AppDataRoaming + "mpv\\history.txt";

            if (LastHistory != fn && File.Exists(fp))
            {
                File.AppendAllText(fp, DateTime.Now.ToString() + " " + Path.GetFileNameWithoutExtension(fn) + BR);
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
            MsgException(e);
        }

        private void Mpv_VideoSizeChanged()
        {
            BeginInvoke(new Action(() => SetFormPosSize()));
        }

        private void Mpv_AfterShutdown()
        {
            if (IsCloseRequired)
                Invoke(new Action(() => Close()));
        }

        public bool IsFullscreen
        {
            get { return WindowState == FormWindowState.Maximized; }
        }

        void MpvChangeFullscreen(bool value)
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
                    if (mpv.MpvWindowHandle != IntPtr.Zero)
                        Native.SendMessage(mpv.MpvWindowHandle, m.Msg, m.WParam, m.LParam);
                    break;
                case 0x203: // Native.WM.LBUTTONDBLCLK
                    if (!IsMouseInOSC())
                        mpv.CommandString("cycle fullscreen");
                    break;
                case 0x0214: // WM_SIZING
                    var rc = Marshal.PtrToStructure<Native.RECT>(m.LParam);
                    var r = rc;
                    NativeHelp.SubtractWindowBorders(Handle, ref r);
                    int c_w = r.Right - r.Left, c_h = r.Bottom - r.Top;
                    float aspect = mpv.VideoSize.Width / (float)mpv.VideoSize.Height;
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
            if (IsFullscreen || mpv.VideoSize.Width == 0) return;
            var wa = Screen.GetWorkingArea(this);
            int h = (int)(wa.Height * 0.6);
            int w = (int)(h * mpv.VideoSize.Width / (float)mpv.VideoSize.Height);
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
                mpv.LoadFiles(e.Data.GetData(DataFormats.FileDrop) as String[]);
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
                mpv.Command("quit");
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // send mouse command to make OSC show
            mpv.CommandString($"mouse {e.X} {e.Y}");

            if (CursorHelp.IsPosDifferent(LastCursorPosChanged))
                CursorHelp.Show();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            IsCloseRequired = false;
            mpv.Command("quit");
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

        private void MainForm_Load(object sender, EventArgs ea)
        {
            mpv.Init();
            mpv.ObserveBoolProp("fullscreen", MpvChangeFullscreen);
            mpv.AfterShutdown += Mpv_AfterShutdown;
            mpv.VideoSizeChanged += Mpv_VideoSizeChanged;
            mpv.PlaybackRestart += mpv_PlaybackRestart;
        }

        private void MainForm_Activated(object sender, EventArgs ea)
        {
            
        }
    }
}