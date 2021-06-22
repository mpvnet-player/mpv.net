
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Forms;

namespace mpvnet
{
    public partial class LearnWindow : Window
    {
        public CommandItem InputItem { get; set; }
        string NewKey = "";

        uint MAPVK_VK_TO_VSC = 0;

        int VK_MENU  = 0x12;
        int VK_LMENU = 0xA4;
        int VK_RMENU = 0xA5;

        int VK_CONTROL  = 0x11;
        int VK_LCONTROL = 0xA2;
        int VK_RCONTROL = 0xA3;

        public LearnWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Theme Theme => Theme.Current;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
            StringBuilder pwszBuff, int cchBuff, uint wFlags);

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        string ToUnicode(uint vk)
        {
            byte[] keys = new byte[256];

            if (!GetKeyboardState(keys))
                return "";

            if ((keys[VK_CONTROL] & 0x80) != 0 && (keys[VK_MENU] & 0x80) == 0)
                keys[VK_LCONTROL] = keys[VK_RCONTROL] = keys[VK_CONTROL] = 0;

            uint scanCode = MapVirtualKey(vk, MAPVK_VK_TO_VSC);

            string ret = ToUnicode(vk, scanCode, keys);

            if (ret.Length == 1 && ret[0] < 32)
                return "";

            if (ret == "" && (keys[VK_MENU] & 0x80) != 0)
            {
                keys[VK_LMENU] = keys[VK_RMENU] = keys[VK_MENU] = 0;
                ret = ToUnicode(vk, scanCode, keys);
            }

            return ret;
        }

        public string ToUnicode(uint vk, uint scanCode, byte[] keys)
        {
            StringBuilder sb = new StringBuilder(10);
            ToUnicode(vk, scanCode, keys, sb, sb.Capacity, 0);
            return sb.ToString();
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Message m = new Message();
            m.HWnd = hwnd;
            m.Msg = msg;
            m.WParam = wParam;
            m.LParam = lParam;
            ProcessKeyEventArgs(ref m);
            return m.Result;
        }

        void OnKeyDown(uint vk)
        {
            Keys key = (Keys)vk;

            if (key == Keys.ControlKey || key == Keys.ShiftKey ||
                key == Keys.Menu || key == Keys.None)

                return;

            string text = ToUnicode(vk);

            if ((int)key > 111 && (int)key < 136)
                text = "F" + ((int)key - 111);

            if ((int)key > 95 && (int)key < 106)
                text = "KP" + ((int)key - 96);

            switch (text)
            {
                case "#":  text = "SHARP"; break;
                case "´´": text = "´"; break;
                case "``": text = "`"; break;
                case "^^": text = "^"; break;
            }

            switch (key)
            {
                case Keys.Left:               text = "LEFT"; break;
                case Keys.Up:                 text = "UP"; break;
                case Keys.Right:              text = "RIGHT"; break;
                case Keys.Down:               text = "DOWN"; break;
                case Keys.Space:              text = "SPACE"; break;
                case Keys.Enter:              text = "ENTER"; break;
                case Keys.Tab:                text = "TAB"; break;
                case Keys.Back:               text = "BS"; break;
                case Keys.Delete:             text = "DEL"; break;
                case Keys.Insert:             text = "INS"; break;
                case Keys.Home:               text = "HOME"; break;
                case Keys.End:                text = "END"; break;
                case Keys.PageUp:             text = "PGUP"; break;
                case Keys.PageDown:           text = "PGDWN"; break;
                case Keys.Escape:             text = "ESC"; break;
                case Keys.Sleep:              text = "SLEEP"; break;
                case Keys.Cancel:             text = "CANCEL"; break;
                case Keys.PrintScreen:        text = "PRINT"; break;
                case Keys.BrowserFavorites:   text = "FAVORITES"; break;
                case Keys.BrowserSearch:      text = "SEARCH"; break;
                case Keys.BrowserHome:        text = "HOMEPAGE"; break;
                case Keys.LaunchMail:         text = "MAIL"; break;
                case Keys.Play:               text = "PLAY"; break;
                case Keys.Pause:              text = "PAUSE"; break;
                case Keys.MediaPlayPause:     text = "PLAYPAUSE"; break;
                case Keys.MediaStop:          text = "STOP"; break;
                case Keys.MediaNextTrack:     text = "NEXT"; break;
                case Keys.MediaPreviousTrack: text = "PREV"; break;

                case Keys.VolumeUp:
                case Keys.VolumeDown:
                case Keys.VolumeMute:
                    text = ""; break;
            }

            bool isAlt   = GetKeyState(18) < 0;
            bool isShift = GetKeyState(16) < 0;
            bool isCtrl  = GetKeyState(17) < 0;

            bool isLetter = (int)key > 64 && (int)key < 91;

            if (isLetter && isShift)
                text = text.ToUpper();

            string keyString = ToUnicode(vk);

            if (isAlt && !isCtrl)
                text = "ALT+" + text;

            if (isShift && keyString == "")
                text = "SHIFT+" + text;

            if (isCtrl && !(keyString != "" && isCtrl && isAlt))
                text = "CTRL+" + text;

            if (!string.IsNullOrEmpty(text))
                SetKey(text);
        }

        void SetKey(string key)
        {
            NewKey = key;
            MenuTextBlock.Text = InputItem.Path;
            KeyTextBlock.Text = key;
        }

        void ProcessKeyEventArgs(ref Message m)
        {
            int WM_KEYDOWN    = 0x100;
            int WM_SYSKEYDOWN = 0x104;
            int WM_APPCOMMAND = 0x319;

            if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
                OnKeyDown((uint)m.WParam.ToInt64());
            else if (m.Msg == WM_APPCOMMAND)
            {
                string value = Input.WM_APPCOMMAND_to_mpv_key((int)(m.LParam.ToInt64() >> 16 & ~0xf000));

                if (value != null)
                    SetKey(value);
            }
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            SetKey(InputItem.Input);
        }

        void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            InputItem.Input = NewKey;
            Close();
        }

        void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputItem.Input = "_";
            Close();
        }

        void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                SetKey("WHEEL_UP");
            else
                SetKey("WHEEL_DOWN");
        }

        void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    if (BlockMBTN_LEFT)
                        BlockMBTN_LEFT = false;
                    else
                        SetKey("MBTN_LEFT");
                    break;
                case MouseButton.Middle:
                    SetKey("MBTN_MID");
                    break;
                case MouseButton.XButton1:
                    SetKey("MBTN_BACK");
                    break;
                case MouseButton.XButton2:
                    SetKey("MBTN_FORWARD");
                    break;
            }
        }

        bool BlockMBTN_LEFT;

        void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SetKey("MBTN_LEFT_DBL");
                BlockMBTN_LEFT = true;
            }
        }

        void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                OnKeyDown((uint)Keys.Tab);
                e.Handled = true;
            }
        }
    }
}
