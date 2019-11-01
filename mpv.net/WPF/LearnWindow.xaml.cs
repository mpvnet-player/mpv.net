
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using WinForms = System.Windows.Forms;

namespace mpvnet
{
    public partial class LearnWindow : Window
    {
        public CommandItem InputItem { get; set; }
        string NewKey = "";
        string KeyChar = "";

        public LearnWindow() => InitializeComponent();

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            WinForms.Message m = new WinForms.Message();
            m.HWnd = hwnd;
            m.Msg = msg;
            m.WParam = wParam;
            m.LParam = lParam;
            ProcessKeyEventArgs(ref m);
            return m.Result;
        }

        void OnKeyUp(WinForms.KeyEventArgs e)
        {
            if (e.KeyCode == WinForms.Keys.ControlKey || e.KeyCode == WinForms.Keys.ShiftKey ||
                e.KeyCode == WinForms.Keys.Menu || e.KeyCode == WinForms.Keys.None)

                return;

            string text = "";
            uint charValue = MapVirtualKey((uint)e.KeyCode, 2);

            if (charValue == 0 || (charValue & 1 << 31) == 1 << 31)
                text = e.KeyCode.ToString().Trim();
            else
                try {
                    text = Convert.ToChar(charValue).ToString().ToLower().Trim();
                } catch {}

            for (int i = 0; i < 13; i++)
                if ("D" + i == text)
                    text = text.Substring(1);

            switch (e.KeyCode)
            {
                case WinForms.Keys.NumPad0:
                case WinForms.Keys.NumPad1:
                case WinForms.Keys.NumPad2:
                case WinForms.Keys.NumPad3:
                case WinForms.Keys.NumPad4:
                case WinForms.Keys.NumPad5:
                case WinForms.Keys.NumPad6:
                case WinForms.Keys.NumPad7:
                case WinForms.Keys.NumPad8:
                case WinForms.Keys.NumPad9:
                    text = "KP" + e.KeyCode.ToString()[6]; break;
                case WinForms.Keys.Space:
                    text = "Space"; break;
                case WinForms.Keys.Enter:
                    text = "Enter"; break;
                case WinForms.Keys.Tab:
                    text = "TAB"; break;
                case WinForms.Keys.Back:
                    text = "BS"; break;
                case WinForms.Keys.Delete:
                    text = "DEL"; break;
                case WinForms.Keys.Insert:
                    text = "INS"; break;
                case WinForms.Keys.Home:
                    text = "Home"; break;
                case WinForms.Keys.End:
                    text = "END"; break;
                case WinForms.Keys.PageUp:
                    text = "PGUP"; break;
                case WinForms.Keys.PageDown:
                    text = "PGDWN"; break;
                case WinForms.Keys.Escape:
                    text = "ESC"; break;
                case WinForms.Keys.PrintScreen:
                    text = "Print"; break;
                case WinForms.Keys.Play:
                    text = "Play"; break;
                case WinForms.Keys.Pause:
                    text = "Pause"; break;
                case WinForms.Keys.MediaPlayPause:
                    text = "PlayPause"; break;
                case WinForms.Keys.MediaStop:
                    text = "Stop"; break;
                case WinForms.Keys.MediaNextTrack:
                    text = "Next"; break;
                case WinForms.Keys.MediaPreviousTrack:
                    text = "Prev"; break;
                case WinForms.Keys.VolumeUp:
                    text = "Volume_Up"; break;
                case WinForms.Keys.VolumeDown:
                    text = "Volume_Down"; break;
                case WinForms.Keys.VolumeMute:
                    text = "Mute"; break;
                case WinForms.Keys.BrowserHome:
                    text = "Homepage"; break;
                case WinForms.Keys.LaunchMail:
                    text = "Mail"; break;
                case WinForms.Keys.BrowserFavorites:
                    text = "Favorites"; break;
                case WinForms.Keys.BrowserSearch:
                    text = "Search"; break;
                case WinForms.Keys.Sleep:
                    text = "Sleep"; break;
                case WinForms.Keys.Cancel:
                    text = "Cancel"; break;
            }

            bool wasModified = false;

            bool isAlt   = GetKeyState(18) < (short)0;
            bool isShift = GetKeyState(16) < (short)0;
            bool isCtrl  = GetKeyState(17) < (short)0;

            if (text.Length == 1 && KeyChar != text)
            {
                text = KeyChar;
                wasModified = true;
            }

            if (text == "#") text = "SHARP";

            if (isAlt   && !wasModified)
                text = "ALT+" + text;

            if (isShift && !wasModified)
                text = "SHIFT+" + text;

            if (isCtrl  && !wasModified)
                text = "CTRL+" + text;

            if (!string.IsNullOrEmpty(text))
                SetKey(text);
        }

        DateTime LastKeyUp;

        void SetKey(string key)
        {
            NewKey = key;
            MenuTextBlock.Text = InputItem.Path;
            KeyTextBlock.Text = key;
            LastKeyUp = DateTime.Now;
        }

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static WinForms.Keys ModifierKeys {
            get {
                WinForms.Keys keys = WinForms.Keys.None;

                if (GetKeyState(17) < (short)0)
                    keys |= WinForms.Keys.Control;

                if (GetKeyState(16) < (short)0)
                    keys |= WinForms.Keys.Shift;

                if (GetKeyState(18) < (short)0)
                    keys |= WinForms.Keys.Alt;

                return keys;
            }
        }

        void ProcessKeyEventArgs(ref WinForms.Message m)
        {
            int WM_KEYUP = 0x0101, WM_SYSKEYUP = 0x0105, WM_APPCOMMAND = 0x0319;

            if (m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYUP)
                OnKeyUp(new WinForms.KeyEventArgs((WinForms.Keys)(unchecked((int)(long)m.WParam)) | ModifierKeys));
            else if (m.Msg == WM_APPCOMMAND)
            {
                var value = (AppCommand)(m.LParam.ToInt64() >> 16 & ~0xf000);

                switch (value)
                {
                    case AppCommand.APPCOMMAND_MEDIA_CHANNEL_DOWN:
                        SetKey("CHANNEL_DOWN");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_CHANNEL_UP:
                        SetKey("CHANNEL_UP");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_FAST_FORWARD:
                        SetKey("FORWARD");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_REWIND:
                        SetKey("REWIND");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_PAUSE:
                        SetKey("PAUSE");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_PLAY:
                        SetKey("PLAY");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_PLAY_PAUSE:
                        SetKey("PLAYPAUSE");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_NEXTTRACK:
                        SetKey("NEXT");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_PREVIOUSTRACK:
                        SetKey("PREV");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_RECORD:
                        SetKey("RECORD");
                        break;
                    case AppCommand.APPCOMMAND_MEDIA_STOP:
                        SetKey("STOP");
                        break;
                    case AppCommand.APPCOMMAND_VOLUME_UP:
                        SetKey("VOLUME_UP");
                        break;
                    case AppCommand.APPCOMMAND_VOLUME_DOWN:
                        SetKey("VOLUME_DOWN");
                        break;
                    case AppCommand.APPCOMMAND_VOLUME_MUTE:
                        SetKey("MUTE");
                        break;
                    case AppCommand.APPCOMMAND_BROWSER_HOME:
                        SetKey("HOMEPAGE");
                        break;
                    case AppCommand.APPCOMMAND_LAUNCH_MAIL:
                        SetKey("MAIL");
                        break;
                    case AppCommand.APPCOMMAND_BROWSER_FAVORITES:
                        SetKey("FAVORITES");
                        break;
                    case AppCommand.APPCOMMAND_BROWSER_SEARCH:
                        SetKey("SEARCH");
                        break;
                    case AppCommand.APPCOMMAND_PRINT:
                        SetKey("PRINT");
                        break;
                }
            }
        }

        internal enum AppCommand
        {
            APPCOMMAND_BASS_BOOST = 20,
            APPCOMMAND_BASS_DOWN = 19,
            APPCOMMAND_BASS_UP = 21,
            APPCOMMAND_BROWSER_BACKWARD = 1,
            APPCOMMAND_BROWSER_FAVORITES = 6,
            APPCOMMAND_BROWSER_FORWARD = 2,
            APPCOMMAND_BROWSER_HOME = 7,
            APPCOMMAND_BROWSER_REFRESH = 3,
            APPCOMMAND_BROWSER_SEARCH = 5,
            APPCOMMAND_BROWSER_STOP = 4,
            APPCOMMAND_CLOSE = 31,
            APPCOMMAND_COPY = 36,
            APPCOMMAND_CORRECTION_LIST = 45,
            APPCOMMAND_CUT = 37,
            APPCOMMAND_DICTATE_OR_COMMAND_CONTROL_TOGGLE = 43,
            APPCOMMAND_FIND = 28,
            APPCOMMAND_FORWARD_MAIL = 40,
            APPCOMMAND_HELP = 27,
            APPCOMMAND_LAUNCH_APP1 = 17,
            APPCOMMAND_LAUNCH_APP2 = 18,
            APPCOMMAND_LAUNCH_MAIL = 15,
            APPCOMMAND_LAUNCH_MEDIA_SELECT = 16,
            APPCOMMAND_MEDIA_CHANNEL_DOWN = 52,
            APPCOMMAND_MEDIA_CHANNEL_UP = 51,
            APPCOMMAND_MEDIA_FAST_FORWARD = 49,
            APPCOMMAND_MEDIA_NEXTTRACK = 11,
            APPCOMMAND_MEDIA_PAUSE = 47,
            APPCOMMAND_MEDIA_PLAY = 46,
            APPCOMMAND_MEDIA_PLAY_PAUSE = 14,
            APPCOMMAND_MEDIA_PREVIOUSTRACK = 12,
            APPCOMMAND_MEDIA_RECORD = 48,
            APPCOMMAND_MEDIA_REWIND = 50,
            APPCOMMAND_MEDIA_STOP = 13,
            APPCOMMAND_MIC_ON_OFF_TOGGLE = 44,
            APPCOMMAND_MICROPHONE_VOLUME_DOWN = 25,
            APPCOMMAND_MICROPHONE_VOLUME_MUTE = 24,
            APPCOMMAND_MICROPHONE_VOLUME_UP = 26,
            APPCOMMAND_NEW = 29,
            APPCOMMAND_OPEN = 30,
            APPCOMMAND_PASTE = 38,
            APPCOMMAND_PRINT = 33,
            APPCOMMAND_REDO = 35,
            APPCOMMAND_REPLY_TO_MAIL = 39,
            APPCOMMAND_SAVE = 32,
            APPCOMMAND_SEND_MAIL = 41,
            APPCOMMAND_SPELL_CHECK = 42,
            APPCOMMAND_TREBLE_DOWN = 22,
            APPCOMMAND_TREBLE_UP = 23,
            APPCOMMAND_UNDO = 34,
            APPCOMMAND_VOLUME_DOWN = 9,
            APPCOMMAND_VOLUME_MUTE = 8,
            APPCOMMAND_VOLUME_UP = 10
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

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
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

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SetKey("MBTN_LEFT_DBL");
                BlockMBTN_LEFT = true;
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            KeyChar = e.Text;
        }
    }
}