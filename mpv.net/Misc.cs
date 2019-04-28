using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Microsoft.Win32;

namespace mpvnet
{
    public class App
    {
        public static bool IsDarkMode { 
            get {
                string darkMode = MainForm.Instance.MpvNetDarkMode;
                object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                if (value is null) value = 1;
                bool isDarkTheme = (int)value == 0;
                return (darkMode == "system" && isDarkTheme) || darkMode == "always";
            }
        }
    }

    public class Sys
    {
        public static bool IsDarkTheme {
            get {
                object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
                if (value is null) value = 1;
                return (int)value == 0;
            }
        }

        public static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath,
                    Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch
            { }
            return false;
        }

        public static string GetFilter()
        {
            string[] fileTypes = "264 265 3gp aac ac3 avc avi avs bmp divx dts dtshd dtshr dtsma eac3 evo flac flv h264 h265 hevc hvc jpg jpeg m2t m2ts m2v m4a m4v mka mkv mlp mov mp2 mp3 mp4 mpa mpeg mpg mpv mts ogg ogm opus pcm png pva raw rmvb thd thd+ac3 true-hd truehd ts vdr vob vpy w64 wav webm wmv y4m".Split(' ');
            return "*." + String.Join(";*.", fileTypes) + "|*." + String.Join(";*.", fileTypes) + "|All Files|*.*";
        }
    }

    public class StringLogicalComparer : IComparer, IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogical(string x, string y);

        int IComparer_Compare(object x, object y) => StrCmpLogical(x.ToString(), y.ToString());
        int IComparer.Compare(object x, object y) => IComparer_Compare(x, y);
        int IComparerOfString_Compare(string x, string y) => StrCmpLogical(x, y);
        int IComparer<string>.Compare(string x, string y) => IComparerOfString_Compare(x, y);
    }

    public class FileAssociation
    {
        static string ExePath = Application.ExecutablePath;
        static string ExeFilename = Path.GetFileName(Application.ExecutablePath);
        static string ExeFilenameNoExt = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        static string[] Types;
        public static string[] VideoTypes = "mpg avi vob mp4 mkv avs 264 mov wmv flv h264 asf webm mpeg mpv y4m avc hevc 265 h265 m2v m2ts vpy mts webm m4v".Split(" ".ToCharArray());
        public static string[] AudioTypes = "mp2 mp3 ac3 wav w64 m4a dts dtsma dtshr dtshd eac3 thd thd+ac3 ogg mka aac opus flac mpa".Split(" ".ToCharArray());

        public static void Register(string[] types)
        {
            Types = types;

            RegistryHelp.SetObject(@"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename, null, ExePath);
            RegistryHelp.SetObject($"HKCR\\Applications\\{ExeFilename}", "FriendlyAppName", "mpv.net media player");
            RegistryHelp.SetObject($"HKCR\\Applications\\{ExeFilename}\\shell\\open\\command", null, $"\"{ExePath}\" \"%1\"");
            RegistryHelp.SetObject(@"HKLM\SOFTWARE\Clients\Media\mpv\Capabilities", "ApplicationDescription", "mpv.net media player");
            RegistryHelp.SetObject(@"HKLM\SOFTWARE\Clients\Media\mpv\Capabilities", "ApplicationName", "mpv.net");
            RegistryHelp.SetObject($"HKCR\\SystemFileAssociations\\video\\OpenWithList\\{ExeFilename}", null, "");
            RegistryHelp.SetObject($"HKCR\\SystemFileAssociations\\audio\\OpenWithList\\{ExeFilename}", null, "");

            foreach (string ext in Types)
            {
                RegistryHelp.SetObject($"HKCR\\Applications\\{ExeFilename}\\SupportedTypes", "." + ext, "");
                RegistryHelp.SetObject($"HKCR\\" + "." + ext, null, ExeFilenameNoExt + "." + ext);
                RegistryHelp.SetObject($"HKCR\\" + "." + ext + "\\OpenWithProgIDs", ExeFilenameNoExt + "." + ext, "");
                if (VideoTypes.Contains(ext))
                    RegistryHelp.SetObject($"HKCR\\" + "." + ext, "PerceivedType", "video");
                if (AudioTypes.Contains(ext))
                    RegistryHelp.SetObject($"HKCR\\" + "." + ext, "PerceivedType", "audio");
                RegistryHelp.SetObject($"HKCR\\" + ExeFilenameNoExt + "." + ext + "\\shell\\open", null, "Play with " +  Application.ProductName);
                RegistryHelp.SetObject($"HKCR\\" + ExeFilenameNoExt + "." + ext + "\\shell\\open\\command", null, $"\"{ExePath}\" \"%1\"");
                RegistryHelp.SetObject(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities\FileAssociations", "." + ext, ExeFilenameNoExt + "." + ext);
            }
        }

        public static void Unregister()
        {
            RegistryHelp.RemoveKey(@"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename);
            RegistryHelp.RemoveKey($"HKCR\\Applications\\{ExeFilename}");
            RegistryHelp.RemoveKey(@"HKLM\SOFTWARE\Clients\Media\mpv.net");
            RegistryHelp.RemoveKey($"HKCR\\SystemFileAssociations\\video\\OpenWithList\\{ExeFilename}");
            RegistryHelp.RemoveKey($"HKCR\\SystemFileAssociations\\audio\\OpenWithList\\{ExeFilename}");

            foreach (string id in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (id.StartsWith(ExeFilenameNoExt + "."))
                    Registry.ClassesRoot.DeleteSubKeyTree(id);

                RegistryHelp.RemoveValue($"HKCR\\Software\\Classes\\" + id + "\\OpenWithProgIDs", ExeFilenameNoExt + id);
                RegistryHelp.RemoveValue($"HKLM\\Software\\Classes\\" + id + "\\OpenWithProgIDs", ExeFilenameNoExt + id);
            }
        }
    }

    public class RegistryHelp
    {
        public static void SetObject(string path, string name, object value)
        {
            using (RegistryKey rk = GetRootKey(path).CreateSubKey(path.Substring(5), RegistryKeyPermissionCheck.ReadWriteSubTree))
                rk.SetValue(name, value);
        }

        public static string GetString(string path, string name)
        {
            object val = GetObject(path, name);
            if (val == null || !(val is string)) return "";
            return val.ToString();
        }

        public static object GetObject(string path, string name)
        {
            using (RegistryKey rk = GetRootKey(path).OpenSubKey(path.Substring(5)))
                if (rk != null)
                    return rk.GetValue(name, "");
                else
                    return null;
        }

        public static void RemoveKey(string path)
        {
            GetRootKey(path).DeleteSubKeyTree(path.Substring(5), false);
        }

        public static void RemoveValue(string path, string name)
        {
            using (RegistryKey rk = GetRootKey(path).OpenSubKey(path.Substring(5), true))
                if (!(rk is null))
                    rk.DeleteValue(name, false);
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

    public class MediaTrack
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public int    ID   { get; set; }
    }

    [Serializable]
    public class CommandItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Path { get; set; } = "";
        public string Command { get; set; } = "";

        public CommandItem() { }

        public CommandItem(SerializationInfo info, StreamingContext context) { }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Input = "";

        public string Input {
            get => _Input;
            set {
                _Input = value;
                NotifyPropertyChanged();
            }
        }

        private static ObservableCollection<CommandItem> _Items;

        public static ObservableCollection<CommandItem> Items {
            get {
                if (_Items is null)
                {
                    _Items = new ObservableCollection<CommandItem>();

                    if (File.Exists(mp.InputConfPath))
                    {
                        foreach (string line in File.ReadAllLines(mp.InputConfPath))
                        {
                            string val = line.Trim();
                            if (val.StartsWith("#")) continue;
                            if (!val.Contains(" ")) continue;
                            CommandItem item = new CommandItem();
                            item.Input = val.Substring(0, val.IndexOf(" ")).Replace("_", "");
                            val = val.Substring(val.IndexOf(" ") + 1);

                            if (val.Contains("#menu:"))
                            {
                                item.Path = val.Substring(val.IndexOf("#menu:") + 6).Trim();
                                val = val.Substring(0, val.IndexOf("#menu:"));

                                if (item.Path.Contains(";"))
                                    item.Path = item.Path.Substring(item.Path.IndexOf(";") + 1).Trim();
                            }

                            item.Command = val.Trim();
                            if (item.Command == "")
                                continue;
                            if (item.Command.ToLower() == "ignore")
                                item.Command = "";
                            _Items.Add(item);
                        }
                    }
                }
                return _Items;
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
            return
                Math.Abs(screenPos.X - Control.MousePosition.X) > 10 ||
                Math.Abs(screenPos.Y - Control.MousePosition.Y) > 10;
        }
    }
}