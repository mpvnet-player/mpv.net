
using System.ComponentModel;
using System.Runtime.InteropServices;

using MpvNet.ExtensionMethod;

namespace MpvNet.Windows.UI;

class GlobalHotkey
{
    public static Dictionary<int, string>? Commands { get; set; }
    static int ID;
    static IntPtr HWND;

    public static void RegisterGlobalHotkeys(IntPtr hwnd)
    {
        HWND = hwnd;
        string path = Player.ConfigFolder + "global-input.conf";

        if (!File.Exists(path))
            return;

        foreach (string i in File.ReadAllLines(path))
        {
            string line = i.Trim();

            if (line.StartsWith("#") || !line.Contains(' '))
                continue;

            ProcessGlobalHotkeyLine(line);
        }
    }

    static void ProcessGlobalHotkeyLine(string line)
    {
        string key = line[..line.IndexOf(" ")];
        string command = line[(line.IndexOf(" ") + 1)..];
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

        key = parts[^1];

        if (key.Length == 1)
        {
            short result = VkKeyScanEx(key[0], GetKeyboardLayout(0));

            int hi = result >> 8;
            int lo = result & 0xFF;

            vk = lo;

            if ((hi & 1) == 1) mod |= KeyModifiers.Shift;
            if ((hi & 2) == 2) mod |= KeyModifiers.Ctrl;
            if ((hi & 4) == 4) mod |= KeyModifiers.Alt;
        }
        else
            vk = Mpv_to_VK(key);

        Commands ??= new Dictionary<int, string>();

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
        if (Commands!.ContainsKey(id))
            Player.Command(Commands[id]);
    }

    static int Mpv_to_VK(string value)
    {
        return value.ToUpperEx() switch
        {
            "NEXT" => 0xB0,// VK_MEDIA_NEXT_TRACK
            "PREV" => 0xB1,// VK_MEDIA_PREV_TRACK
            "STOP" => 0xB2,// VK_MEDIA_STOP
            "PLAYPAUSE" => 0xB3,// VK_MEDIA_PLAY_PAUSE
            "SLEEP" => 0x5F,// VK_SLEEP
            "RIGHT" => 0x27,// VK_RIGHT
            "UP" => 0x26,// VK_UP
            "LEFT" => 0x25,// VK_LEFT
            "DOWN" => 0x28,// VK_DOWN
            "PGUP" => 0x21,// VK_PRIOR
            "PGDWN" => 0x22,// VK_NEXT
            "PAUSE" => 0x13,// VK_PAUSE
            "PRINT" => 0x2A,// VK_PRINT
            "HOME" => 0x24,// VK_HOME
            "INS" => 0x2D,// VK_INSERT
            "KP_INS" => 0x2D,// VK_INSERT
            "DEL" => 0x2E,// VK_DELETE
            "KP_DEL" => 0x2E,// VK_DELETE
            "END" => 0x23,// VK_END
            "F1" => 0x70,// VK_F1
            "F2" => 0x71,// VK_F2
            "F3" => 0x72,// VK_F3
            "F4" => 0x73,// VK_F4
            "F5" => 0x74,// VK_F5
            "F6" => 0x75,// VK_F6
            "F7" => 0x76,// VK_F7
            "F8" => 0x77,// VK_F8
            "F9" => 0x78,// VK_F9
            "F10" => 0x79,// VK_F10
            "F11" => 0x7A,// VK_F11
            "F12" => 0x7B,// VK_F12
            "F13" => 0x7C,// VK_F13
            "F14" => 0x7D,// VK_F14
            "F15" => 0x7E,// VK_F15
            "F16" => 0x7F,// VK_F16
            "F17" => 0x80,// VK_F17
            "F18" => 0x81,// VK_F18
            "F19" => 0x82,// VK_F19
            "F20" => 0x83,// VK_F20
            "F21" => 0x84,// VK_F21
            "F22" => 0x85,// VK_F22
            "F23" => 0x86,// VK_F23
            "F24" => 0x87,// VK_F24
            "ENTER" => 0x0D,// VK_RETURN
            "KP_ENTER" => 0x0D,// VK_RETURN
            "TAB" => 0x09,// VK_TAB
            "MENU" => 0x5D,// VK_APPS
            "CANCEL" => 0x03,// VK_CANCEL
            "BS" => 0x08,// VK_BACK
            "KP_DEC" => 0x6E,// VK_DECIMAL
            "ESC" => 0x1B,// VK_ESCAPE
            "KP0" => 0x60,// VK_NUMPAD0
            "KP1" => 0x61,// VK_NUMPAD1
            "KP2" => 0x62,// VK_NUMPAD2
            "KP3" => 0x63,// VK_NUMPAD3
            "KP4" => 0x64,// VK_NUMPAD4
            "KP5" => 0x65,// VK_NUMPAD5
            "KP6" => 0x66,// VK_NUMPAD6
            "KP7" => 0x67,// VK_NUMPAD7
            "KP8" => 0x68,// VK_NUMPAD8
            "KP9" => 0x69,// VK_NUMPAD9
            "FAVORITES" => 0xAB,// VK_BROWSER_FAVORITES
            "SEARCH" => 0xAA,// VK_BROWSER_SEARCH
            "MAIL" => 0xB4,// VK_LAUNCH_MAIL
            "VOLUME_UP" => 0xAF,// VK_VOLUME_UP
            "VOLUME_DOWN" => 0xAE,// VK_VOLUME_DOWN
            "MUTE" => 0xAD,// VK_VOLUME_MUTE
            "SPACE" => 0x20,// VK_SPACE
            "IDEOGRAPHIC_SPACE" => 0x20,// VK_SPACE
            _ => 0,
        };
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
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        Win = 8
    }
}
