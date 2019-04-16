using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WF = System.Windows.Forms;

namespace mpvInputEdit
{
    public partial class InputWindow : Window
    {
        public InputItem InputItem { get; set; }
        public string NewKey { get; set; } = "";

        public InputWindow()
        {
            InitializeComponent();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            WF.Message m = new WF.Message();
            m.HWnd = hwnd;
            m.Msg = msg;
            m.WParam = wParam;
            m.LParam = lParam;
            ProcessKeyEventArgs(ref m);
            return m.Result;
        }

        void OnKeyUp(WF.KeyEventArgs e)
        {
            if (e.KeyCode == WF.Keys.ControlKey || e.KeyCode == WF.Keys.ShiftKey ||
                e.KeyCode == WF.Keys.Menu || e.KeyCode == WF.Keys.None)

                return;

            string text = "";
            uint charValue = MapVirtualKey((uint)e.KeyCode, 2);

            if (charValue == 0 || (charValue & 1 << 31) == 1 << 31)
                text = e.KeyCode.ToString().Trim();
            else
                try {
                    text = Convert.ToChar(charValue).ToString().ToLower().Trim();
                }
                catch {}

            for (int i = 0; i < 13; i++)
                if ("D" + i.ToString() == text)
                    text = text.Substring(1);

            switch (e.KeyCode)
            {
                case WF.Keys.NumPad0:
                case WF.Keys.NumPad1:
                case WF.Keys.NumPad2:
                case WF.Keys.NumPad3:
                case WF.Keys.NumPad4:
                case WF.Keys.NumPad5:
                case WF.Keys.NumPad6:
                case WF.Keys.NumPad7:
                case WF.Keys.NumPad8:
                case WF.Keys.NumPad9:
                    text = "KP" + e.KeyCode.ToString()[6].ToString(); break;
                case WF.Keys.Space:
                    text = "Space"; break;
                case WF.Keys.Enter:
                    text = "Enter"; break;
                case WF.Keys.Tab:
                    text = "Tab"; break;
                case WF.Keys.Back:
                    text = "BS"; break;
                case WF.Keys.Delete:
                    text = "Del"; break;
                case WF.Keys.Insert:
                    text = "Ins"; break;
                case WF.Keys.Home:
                    text = "Home"; break;
                case WF.Keys.End:
                    text = "End"; break;
                case WF.Keys.PageUp:
                    text = "PGUP"; break;
                case WF.Keys.PageDown:
                    text = "PGDWN"; break;
                case WF.Keys.Escape:
                    text = "Esc"; break;
                case WF.Keys.PrintScreen:
                    text = "Print"; break;
                case WF.Keys.Play:
                    text = "Play"; break;
                case WF.Keys.Pause:
                    text = "Pause"; break;
                case WF.Keys.MediaPlayPause:
                    text = "PlayPause"; break;
                case WF.Keys.MediaStop:
                    text = "Stop"; break;
                case WF.Keys.MediaNextTrack:
                    text = "Next"; break;
                case WF.Keys.MediaPreviousTrack:
                    text = "Prev"; break;
                case WF.Keys.VolumeUp:
                    text = "Volume_Up"; break;
                case WF.Keys.VolumeDown:
                    text = "Volume_Down"; break;
                case WF.Keys.VolumeMute:
                    text = "Mute"; break;
                case WF.Keys.BrowserHome:
                    text = "Homepage"; break;
                case WF.Keys.LaunchMail:
                    text = "Mail"; break;
                case WF.Keys.BrowserFavorites:
                    text = "Favorites"; break;
                case WF.Keys.BrowserSearch:
                    text = "Search"; break;
                case WF.Keys.Sleep:
                    text = "Sleep"; break;
                case WF.Keys.Cancel:
                    text = "Cancel"; break;
            }

            bool shiftWasHandled = false;

            bool isAlt   = GetKeyState(18) < (short)0;
            bool isShift = GetKeyState(16) < (short)0;
            bool isCtrl  = GetKeyState(17) < (short)0;

            if (text.Length == 1 && isShift && text[0] != GetModifiedKey(text[0]))
            {
                text = GetModifiedKey(text[0]).ToString();
                shiftWasHandled = true;
            }

            if (text == "#") text = "Sharp";

            if (isAlt) text = "Alt+" + text;
            if (isShift && !shiftWasHandled) text = "Shift+" + text;
            if (isCtrl) text = "Ctrl+" + text;

            if (!string.IsNullOrEmpty(text))
                SetKey(text);
        }

        void SetKey(string key)
        {
            NewKey = key;
            MenuLabel.Content = InputItem.Menu;
            KeyLabel.Content = key;
        }

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static WF.Keys ModifierKeys {
            get {
                WF.Keys keys = WF.Keys.None;
                if (GetKeyState(17) < (short)0)
                    keys |= WF.Keys.Control;
                if (GetKeyState(16) < (short)0)
                    keys |= WF.Keys.Shift;
                if (GetKeyState(18) < (short)0)
                    keys |= WF.Keys.Alt;
                return keys;
            }
        }

        public static char GetModifiedKey(char c)
        {
            short vkKeyScanResult = VkKeyScan(c);

            if (vkKeyScanResult == -1)
                return c;

            uint code = (uint)vkKeyScanResult & 0xff;
            byte[] b = new byte[256];
            b[0x10] = 0x80;
            uint r;

            if (1 != ToAscii(code, code, b, out r, 0))
                return c;

            return (char)r;
        }

        void ProcessKeyEventArgs(ref WF.Message m)
        {
            int WM_KEYUP = 0x0101, WM_SYSKEYUP = 0x0105, WM_APPCOMMAND = 0x0319;

            if (m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYUP)
                OnKeyUp(new WF.KeyEventArgs((WF.Keys)(unchecked((int)(long)m.WParam)) | ModifierKeys));
            else if (m.Msg == WM_APPCOMMAND)
            {
                switch ((AppCommand)(m.LParam.ToInt32() >> 16))
                {
                    case AppCommand.MEDIA_CHANNEL_DOWN:
                        SetKey("Channel_Down");
                        break;
                    case AppCommand.MEDIA_CHANNEL_UP:
                        SetKey("Channel_Up");
                        break;
                    case AppCommand.MEDIA_FAST_FORWARD:
                        SetKey("Forward");
                        break;
                    case AppCommand.MEDIA_REWIND:
                        SetKey("Rewind");
                        break;
                    case AppCommand.MEDIA_PAUSE:
                        SetKey("Pause");
                        break;
                    case AppCommand.MEDIA_PLAY:
                        SetKey("Play");
                        break;
                    case AppCommand.MEDIA_PLAY_PAUSE:
                        SetKey("PlayPause");
                        break;
                    case AppCommand.MEDIA_NEXTTRACK:
                        SetKey("Next");
                        break;
                    case AppCommand.MEDIA_PREVIOUSTRACK:
                        SetKey("Prev");
                        break;
                    case AppCommand.MEDIA_RECORD:
                        SetKey("Record");
                        break;
                    case AppCommand.MEDIA_STOP:
                        SetKey("Stop");
                        break;
                    case AppCommand.VolumeUp:
                        SetKey("Volume_Up");
                        break;
                    case AppCommand.VolumeDown:
                        SetKey("Volume_Down");
                        break;
                    case AppCommand.VolumeMute:
                        SetKey("Mute");
                        break;
                }
            }
        }

        internal enum AppCommand
        {
            MEDIA_CHANNEL_DOWN = 52,
            MEDIA_CHANNEL_UP = 51,
            MEDIA_FAST_FORWARD = 49,
            MEDIA_NEXTTRACK = 11,
            MEDIA_PAUSE = 47,
            MEDIA_PLAY = 46,
            MEDIA_PLAY_PAUSE = 14,
            MEDIA_PREVIOUSTRACK = 12,
            MEDIA_RECORD = 48,
            MEDIA_REWIND = 50,
            MEDIA_STOP = 13,
            VolumeMute = 8,
            VolumeDown = 9,
            VolumeUp = 10
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char c);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int ToAscii(uint     uVirtKey,
                                  uint     uScanCode,
                                  byte[]   lpKeyState,
                                  out uint lpChar,
                                  uint     flags);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            SetKey(InputItem.Input);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            InputItem.Input = NewKey;
            Close();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputItem.Input = "_";
            Close();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                SetKey("WHEEL_UP");
            else
                SetKey("WHEEL_DOWN");
        }
    }
}