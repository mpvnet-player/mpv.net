/**
 *mpv.net
 *Copyright(C) 2017 stax76
 *
 *This program is free software: you can redistribute it and/or modify
 *it under the terms of the GNU General Public License as published by
 *the Free Software Foundation, either version 3 of the License, or
 *(at your option) any later version.
 *
 *This program is distributed in the hope that it will be useful,
 *but WITHOUT ANY WARRANTY; without even the implied warranty of
 *MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *GNU General Public License for more details.
 *
 *You should have received a copy of the GNU General Public License
 *along with this program. If not, see http://www.gnu.org/licenses/.
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mpvnet
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; set; }
        public static IntPtr Hwnd;

        private Point LastCursorPosChanged;
        private int LastCursorChangedTickCount;

        public ContextMenuEx CM;

        public MainForm()
        {
            try
            {
                Application.ThreadException += Application_ThreadException;
                InitializeComponent();
                Instance = this;
                Hwnd = Handle;
                mpv.Init();
                mpv.ObserveBoolProp("fullscreen", MpvChangeFullscreen);
                mpv.AfterShutdown += Mpv_AfterShutdown;
                mpv.VideoSizeChanged += Mpv_VideoSizeChanged;
                mpv.PlaybackRestart += Mpv_PlaybackRestart;

                CM = new ContextMenuEx();
                ContextMenu = CM;
                CM.Popup += CM_Popup;
                ContextMenu.MenuItems.Add("About mpv.net", About);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        private void Mpv_PlaybackRestart()
        {
            BeginInvoke(new Action(() => Text = mpv.GetStringProp("filename") + " - mpv.net"));
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
            MessageBox.Show(e.ToString(), "mpv.net Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void About(object sender, EventArgs e)
        {
            mpv.Command("show-text", Application.ProductName + " v" + Application.ProductVersion.ToString() +
                "\nCopyright (c) 2017 stax76\nGPL License", "5000");
        }

        private void Mpv_VideoSizeChanged()
        {
            BeginInvoke(new Action(() => SetFormPosSize()));
        }

        private void Mpv_AfterShutdown() => Invoke(new Action(() => Close()));

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
            {
                int count = mpv.GetIntProp("playlist-count");
                string[] files = e.Data.GetData(DataFormats.FileDrop) as String[];

                foreach (string file in files)
                    mpv.Command("loadfile", file, "append");

                mpv.SetIntProp("playlist-pos", count);

                for (int i = 0; i < count; i++)
                    mpv.Command("playlist-remove", "0");         

                mpv.LoadFolder();
            }
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
                Form.ActiveForm == this && !CM.Visible)
            {
                CursorHelp.Hide();
            }
        }
    }
}