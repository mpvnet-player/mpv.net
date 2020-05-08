
using System;
using System.Diagnostics;
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

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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
                    text = "SPACE"; break;
                case WinForms.Keys.Enter:
                    text = "ENTER"; break;
                case WinForms.Keys.Tab:
                    text = "TAB"; break;
                case WinForms.Keys.Back:
                    text = "BS"; break;
                case WinForms.Keys.Delete:
                    text = "DEL"; break;
                case WinForms.Keys.Insert:
                    text = "INS"; break;
                case WinForms.Keys.Home:
                    text = "HOME"; break;
                case WinForms.Keys.End:
                    text = "END"; break;
                case WinForms.Keys.PageUp:
                    text = "PGUP"; break;
                case WinForms.Keys.PageDown:
                    text = "PGDWN"; break;
                case WinForms.Keys.Escape:
                    text = "ESC"; break;
                case WinForms.Keys.PrintScreen:
                    text = "PRINT"; break;
                case WinForms.Keys.Play:
                    text = "PLAY"; break;
                case WinForms.Keys.Pause:
                    text = "PAUSE"; break;
                case WinForms.Keys.MediaPlayPause:
                    text = "PLAYPAUSE"; break;
                case WinForms.Keys.MediaStop:
                    text = "STOP"; break;
                case WinForms.Keys.MediaNextTrack:
                    text = "NEXT"; break;
                case WinForms.Keys.MediaPreviousTrack:
                    text = "PREV"; break;
                case WinForms.Keys.VolumeMute:
                    text = "MUTE"; break;
                case WinForms.Keys.BrowserHome:
                    text = "HOMEPAGE"; break;
                case WinForms.Keys.LaunchMail:
                    text = "MAIL"; break;
                case WinForms.Keys.BrowserFavorites:
                    text = "FAVORITES"; break;
                case WinForms.Keys.BrowserSearch:
                    text = "SEARCH"; break;
                case WinForms.Keys.Sleep:
                    text = "SLEEP"; break;
                case WinForms.Keys.Cancel:
                    text = "CANCEL"; break;
                case WinForms.Keys.VolumeUp:
                    text = ""; break;
                case WinForms.Keys.VolumeDown:
                    text = ""; break;
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

            if (text == "#")
                text = "SHARP";

            if (isAlt   && !wasModified)
                text = "ALT+" + text;

            if (isShift && !wasModified)
                text = "SHIFT+" + text;

            if (isCtrl  && !wasModified)
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
                string value = mpvHelp.WM_APPCOMMAND_to_mpv_key((int)(m.LParam.ToInt64() >> 16 & ~0xf000));

                if (value != null)
                    SetKey(value);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int keyCode);

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

        void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            KeyChar = e.Text;
        }
    }
}
