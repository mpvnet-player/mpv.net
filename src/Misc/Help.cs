
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

using static mpvnet.Global;

namespace mpvnet
{
    public static class StringHelp
    {
        public static string GetMD5Hash(string txt)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBuffer = Encoding.UTF8.GetBytes(txt);
                byte[] hashBuffer = md5.ComputeHash(inputBuffer);
                return BitConverter.ToString(md5.ComputeHash(inputBuffer)).Replace("-", "");
            }
        }
    }

    public static class FileHelp
    {
        public static void Delete(string path)
        {
            try {
                if (File.Exists(path))
                    File.Delete(path);
            } catch (Exception ex) {
                Terminal.WriteError("Failed to delete file:" + BR + path + BR + ex.Message);
            }
        }
    }

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
            return Math.Abs(screenPos.X - Control.MousePosition.X) > 10 ||
                   Math.Abs(screenPos.Y - Control.MousePosition.Y) > 10;
        }
    }

    public class mpvHelp
    {
        public static string GetProfiles()
        {
            string json = Core.GetPropertyString("profile-list");
            var o = json.FromJson<List<Dictionary<string, object>>>().OrderBy(i => i["name"]);
            StringBuilder sb = new StringBuilder();

            foreach (Dictionary<string, object> i in o)
            {
                sb.Append(i["name"].ToString() + BR2);

                foreach (Dictionary<string, object> i2 in i["options"] as List<object>)
                    sb.AppendLine("   " + i2["key"] + " = " + i2["value"]);

                sb.Append(BR);
            }

            return sb.ToString();
        }

        public static string GetDecoders()
        {           
            string json = Core.GetPropertyString("decoder-list");
            var o = json.FromJson<List<Dictionary<string, object>>>().OrderBy(i => i["codec"]);
            StringBuilder sb = new StringBuilder();

            foreach (Dictionary<string, object> i in o)
                sb.AppendLine(i["codec"] + " - " + i["description"]);

            return sb.ToString();
        }

        public static string GetProtocols()
        {
            string list = Core.GetPropertyString("protocol-list");
            return string.Join(BR, list.Split(',').OrderBy(a => a));
        }

        public static string GetDemuxers()
        {
            string list = Core.GetPropertyString("demuxer-lavf-list");
            return string.Join(BR, list.Split(',').OrderBy(a => a));
        }
    }

    public class RegistryHelp
    {
        public static string ApplicationKey { get; } = @"HKCU\Software\" + Application.ProductName;

        public static void SetInt(string name, object value)
        {
            SetValue(ApplicationKey, name, value);
        }

        public static void SetString(string name, string value)
        {
            SetValue(ApplicationKey, name, value);
        }

        public static void SetValue(string name, object value)
        {
            using (RegistryKey regKey = GetRootKey(ApplicationKey).CreateSubKey(ApplicationKey.Substring(5), RegistryKeyPermissionCheck.ReadWriteSubTree))
                regKey.SetValue(name, value);
        }

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

        public static object GetValue(string name) => GetValue(ApplicationKey, name, null);

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
