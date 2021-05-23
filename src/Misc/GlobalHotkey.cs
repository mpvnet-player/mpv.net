
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

using static mpvnet.Global;

namespace mpvnet
{
    class GlobalHotkey
    {
        public static Dictionary<int, string> Commands { get; set; }
        static int ID;
        static IntPtr HWND;

        public static void RegisterGlobalHotkeys(IntPtr hwnd)
        {
            HWND = hwnd;
            string path = Core.ConfigFolder + "global-input.conf";

            if (!File.Exists(path))
                return;

            foreach (string i in File.ReadAllLines(path))
            {
                string line = i.Trim();

                if (line.StartsWith("#") || !line.Contains(" "))
                    continue;

                ProcessGlobalHotkeyLine(line);
            }
        }

        static void ProcessGlobalHotkeyLine(string line)
        {
            string key = line.Substring(0, line.IndexOf(" "));
            string command = line.Substring(line.IndexOf(" ") + 1);
            string[] parts = key.Split('+');
            KeyModifiers mod = KeyModifiers.None;
            int vk;
            
            for (int i = 0; i < parts.Length - 1; i++)
            {
                string umod = parts[i].ToUpper();

                if (umod == "ALT") mod |= KeyModifiers.Alt;
                if (umod == "CTRL") mod |= KeyModifiers.Ctrl;
                if (umod == "SHIFT") mod |= KeyModifiers.Shift;
                if (umod == "WIN") mod |= KeyModifiers.Win;
            }

            key = parts[parts.Length - 1];

            if (key.Length == 1)
            {
                short result = VkKeyScanEx(key[0], GetKeyboardLayout(0));

                int hi = result >> 8;
                int lo = result & 0xFF;

                if (lo == -1)
                    return;

                vk = lo;

                if ((hi & 1) == 1) mod |= KeyModifiers.Shift;
                if ((hi & 2) == 2) mod |= KeyModifiers.Ctrl;
                if ((hi & 4) == 4) mod |= KeyModifiers.Alt;
            }
            else
                vk = mpv_to_VK(key);

            if (Commands == null)
                Commands = new Dictionary<int, string>();

            if (vk > 0)
            {
                Commands[ID] = command.Trim();
                bool success = RegisterHotKey(HWND, ID++, mod, vk);

                if (!success)
                    Terminal.WriteError(line + ": " + new Win32Exception().Message + "\n", "global-input.conf");
            }
        }

        public static void Execute(int id)
        {
            if (Commands.ContainsKey(id))
                Core.command(Commands[id]);
        }

        static int mpv_to_VK(string value)
        {
            switch (value.ToUpperEx())
            {
                case "NEXT"       : return 0xB0; // VK_MEDIA_NEXT_TRACK
                case "PREV"       : return 0xB1; // VK_MEDIA_PREV_TRACK
                case "STOP"       : return 0xB2; // VK_MEDIA_STOP
                case "PLAYPAUSE"  : return 0xB3; // VK_MEDIA_PLAY_PAUSE
                case "SLEEP"      : return 0x5F; // VK_SLEEP
                case "RIGHT"      : return 0x27; // VK_RIGHT
                case "UP"         : return 0x26; // VK_UP
                case "LEFT"       : return 0x25; // VK_LEFT
                case "DOWN"       : return 0x28; // VK_DOWN
                case "PGUP"       : return 0x21; // VK_PRIOR
                case "PGDWN"      : return 0x22; // VK_NEXT
                case "PAUSE"      : return 0x13; // VK_PAUSE
                case "PRINT"      : return 0x2A; // VK_PRINT
                case "HOME"       : return 0x24; // VK_HOME
                case "INS"        : return 0x2D; // VK_INSERT
                case "KP_INS"     : return 0x2D; // VK_INSERT
                case "DEL"        : return 0x2E; // VK_DELETE
                case "KP_DEL"     : return 0x2E; // VK_DELETE
                case "END"        : return 0x23; // VK_END
                case "F1"         : return 0x70; // VK_F1
                case "F2"         : return 0x71; // VK_F2
                case "F3"         : return 0x72; // VK_F3
                case "F4"         : return 0x73; // VK_F4
                case "F5"         : return 0x74; // VK_F5
                case "F6"         : return 0x75; // VK_F6
                case "F7"         : return 0x76; // VK_F7
                case "F8"         : return 0x77; // VK_F8
                case "F9"         : return 0x78; // VK_F9
                case "F10"        : return 0x79; // VK_F10
                case "F11"        : return 0x7A; // VK_F11
                case "F12"        : return 0x7B; // VK_F12
                case "F13"        : return 0x7C; // VK_F13
                case "F14"        : return 0x7D; // VK_F14
                case "F15"        : return 0x7E; // VK_F15
                case "F16"        : return 0x7F; // VK_F16
                case "F17"        : return 0x80; // VK_F17
                case "F18"        : return 0x81; // VK_F18
                case "F19"        : return 0x82; // VK_F19
                case "F20"        : return 0x83; // VK_F20
                case "F21"        : return 0x84; // VK_F21
                case "F22"        : return 0x85; // VK_F22
                case "F23"        : return 0x86; // VK_F23
                case "F24"        : return 0x87; // VK_F24
                case "ENTER"      : return 0x0D; // VK_RETURN
                case "KP_ENTER"   : return 0x0D; // VK_RETURN
                case "TAB"        : return 0x09; // VK_TAB
                case "MENU"       : return 0x5D; // VK_APPS
                case "CANCEL"     : return 0x03; // VK_CANCEL
                case "BS"         : return 0x08; // VK_BACK
                case "KP_DEC"     : return 0x6E; // VK_DECIMAL
                case "ESC"        : return 0x1B; // VK_ESCAPE
                case "KP0"        : return 0x60; // VK_NUMPAD0
                case "KP1"        : return 0x61; // VK_NUMPAD1
                case "KP2"        : return 0x62; // VK_NUMPAD2
                case "KP3"        : return 0x63; // VK_NUMPAD3
                case "KP4"        : return 0x64; // VK_NUMPAD4
                case "KP5"        : return 0x65; // VK_NUMPAD5
                case "KP6"        : return 0x66; // VK_NUMPAD6
                case "KP7"        : return 0x67; // VK_NUMPAD7
                case "KP8"        : return 0x68; // VK_NUMPAD8
                case "KP9"        : return 0x69; // VK_NUMPAD9
                case "FAVORITES"  : return 0xAB; // VK_BROWSER_FAVORITES
                case "SEARCH"     : return 0xAA; // VK_BROWSER_SEARCH
                case "MAIL"       : return 0xB4; // VK_LAUNCH_MAIL
                case "VOLUME_UP"  : return 0xAF; // VK_VOLUME_UP
                case "VOLUME_DOWN": return 0xAE; // VK_VOLUME_DOWN
                case "MUTE"       : return 0xAD; // VK_VOLUME_MUTE
                case "SPACE"      : return 0x20; // VK_SPACE
                case "IDEOGRAPHIC_SPACE": return 0x20; // VK_SPACE
                default: return 0;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, int vk);

        [Flags]
        enum KeyModifiers
        {
            None  = 0,
            Alt   = 1,
            Ctrl  = 2,
            Shift = 4,
            Win   = 8
        }
    }
}
