
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Microsoft.Win32;

using static mpvnet.Core;
using static mpvnet.NewLine;

namespace mpvnet
{
    public static class ProcessHelp
    {
        public static void Execute(string file, string arguments = null)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = file;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
            }
        }

        public static void ShellExecute(string file, string arguments = null)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = file;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }
    }

    public static class ConsoleHelp
    {
        public static int Padding { get; set; }

        public static void WriteError(object obj, string module = "mpv.net")
        {
            Write(obj, module, ConsoleColor.DarkRed, false);    
        }

        public static void Write(object obj, string module = "mpv.net")
        {
            Write(obj, module, ConsoleColor.Black, true);
        }

        public static void Write(object obj, string module, ConsoleColor color)
        {
            Write(obj, module, color, false);
        }

        public static void Write(object obj, string module, ConsoleColor color, bool useDefaultColor)
        {
            if (obj == null)
                return;

            string value = obj.ToString();               

            if (!string.IsNullOrEmpty(module))
                module = "[" + module + "] ";

            if (useDefaultColor)
                Console.ResetColor();
            else
                Console.ForegroundColor = color;

            value = module + value;

            if (Padding > 0 && value.Length < Padding)
                value = value.PadRight(Padding);

            if (color == ConsoleColor.Red || color == ConsoleColor.DarkRed)
                Console.Error.WriteLine(value);
            else
                Console.WriteLine(value);

            Console.ResetColor();
            Trace.WriteLine(obj);
        }
    }

    public class CursorHelp
    {
        static bool IsVisible = true;

        public static void Show()
        {
            if (!IsVisible)
            {
                Cursor.Show();
                IsVisible = true;
            }
        }

        public static void Hide()
        {
            if (IsVisible)
            {
                Cursor.Hide();
                IsVisible = false;
            }
        }

        public static bool IsPosDifferent(Point screenPos)
        {
            return
                Math.Abs(screenPos.X - Control.MousePosition.X) > 10 ||
                Math.Abs(screenPos.Y - Control.MousePosition.Y) > 10;
        }
    }

    public class mpvHelp
    {
        public static string GetProfiles()
        {
            string code = @"
                foreach ($item in ($json | ConvertFrom-Json | foreach { $_ } | sort name))
                {
                    $item.name
                    ''

                    foreach ($option in $item.options)
                    {
                        '   ' + $option.key + ' = ' + $option.value
                    }

                    ''
                }";

            string json = core.get_property_string("profile-list");
            return PowerShell.InvokeAndReturnString(code, "json", json).Trim();
        }

        public static string GetDecoders()
        {
            string code = @"
                foreach ($item in ($json | ConvertFrom-Json | foreach { $_ } | sort codec))
                {
                    $item.codec + ' - ' + $item.description
                }";

            string json = core.get_property_string("decoder-list");
            return PowerShell.InvokeAndReturnString(code, "json", json).Trim();
        }

        public static string GetProtocols()
        {
            string list = core.get_property_string("protocol-list");
            return string.Join(BR, list.Split(',').OrderBy(a => a));
        }

        public static string GetDemuxers()
        {
            string list = core.get_property_string("demuxer-lavf-list");
            return string.Join(BR, list.Split(',').OrderBy(a => a));
        }
    }

    public class RegistryHelp
    {
        public static string ApplicationKey { get; } = @"HKCU\Software\" + Application.ProductName;

        public static void SetValue(string path, string name, object value)
        {
            using (RegistryKey regKey = GetRootKey(path).CreateSubKey(path.Substring(5), RegistryKeyPermissionCheck.ReadWriteSubTree))
                regKey.SetValue(name, value);
        }

        public static string GetString(string name, string defaultValue = "")
        {
            object value = GetValue(ApplicationKey, name, defaultValue);
            return !(value is string) ? defaultValue : value.ToString();
        }

        public static int GetInt(string name, int defaultValue = 0)
        {
            object value = GetValue(ApplicationKey, name, defaultValue);
            return !(value is int) ? defaultValue : (int)value;
        }

        public static object GetValue(string path, string name, object defaultValue = null)
        {
            using (RegistryKey regKey = GetRootKey(path).OpenSubKey(path.Substring(5)))
                return regKey == null ? null : regKey.GetValue(name, defaultValue);
        }

        public static void RemoveKey(string path)
        {
            try {
                GetRootKey(path).DeleteSubKeyTree(path.Substring(5), false);
            } catch { }
        }

        public static void RemoveValue(string path, string name)
        {
            try {
                using (RegistryKey regKey = GetRootKey(path).OpenSubKey(path.Substring(5), true))
                    if (regKey != null)
                        regKey.DeleteValue(name, false);
            } catch { }
        }

        static RegistryKey GetRootKey(string path)
        {
            switch (path.Substring(0, 4))
            {
                case "HKLM": return Registry.LocalMachine;
                case "HKCU": return Registry.CurrentUser;
                case "HKCR": return Registry.ClassesRoot;
                default: throw new Exception();
            }
        }
    }
}
