using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        public static string ConfFilePath { get; } = mp.ConfFolder + "\\mpvnet.conf";
        public static string RegPath { get; } = @"HKCU\Software\" + Application.ProductName;
        public static string DarkMode { get; set; } = "always";
        public static string ProcessInstance { get; set; } = "single";

        public static string[] VideoTypes    { get; } = "mkv mp4 mpg avi mov webm vob wmv flv avs 264 h264 asf webm mpeg mpv y4m avc hevc 265 h265 m2v m2ts vpy mts m4v".Split(' ');
        public static string[] AudioTypes    { get; } = "mp3 mp2 ac3 ogg opus flac wav w64 m4a dts dtsma dtshr dtshd eac3 thd thd+ac3 mka aac mpa".Split(' ');
        public static string[] SubtitleTypes { get; } = "srt ass idx sup ttxt ssa smi".Split(' ');
        public static string[] UrlWhitelist { get; set; } = { "tube", "vimeo", "ard", "zdf" };

        public static bool DebugMode { get; set; } = false;

        public static bool IsDarkMode { 
            get => (DarkMode == "system" && Sys.IsDarkTheme) || DarkMode == "always";
        }

        public static void Init()
        {
            foreach (var i in Conf)
                ProcessProperty(i.Key, i.Value);

            if (App.DebugMode)
            {
                try
                {
                    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\mpvnet-debug.log";
                    if (File.Exists(filePath)) File.Delete(filePath);
                    Trace.Listeners.Clear();
                    Trace.Listeners.Add(new TextWriterTraceListener(filePath));
                    foreach (Screen screen in Screen.AllScreens)
                        Trace.WriteLine(screen);
                }
                catch (Exception e)
                {
                    Msg.ShowException(e);
                }
            }
        }

        public static void Exit()
        {
            if (Trace.Listeners.Count > 0) Trace.Listeners[0].Close();
        }

        static Dictionary<string, string> _Conf;

        public static Dictionary<string, string> Conf {
            get {
                if (_Conf == null)
                {
                    _Conf = new Dictionary<string, string>();

                    if (File.Exists(ConfFilePath))
                        foreach (string i in File.ReadAllLines(ConfFilePath))
                            if (i.Contains("=") && !i.StartsWith("#"))
                                _Conf[i.Substring(0, i.IndexOf("=")).Trim()] = i.Substring(i.IndexOf("=") + 1).Trim();
                }
                return _Conf;
            }
        }

        public static void ProcessProperty(string name, string value)
        {
            switch (name)
            {
                case "process-instance": ProcessInstance = value; break;
                case "dark-mode": DarkMode = value; break;
                case "debug-mode": DebugMode = value == "yes"; break;
                case "url-whitelist":
                    UrlWhitelist = value.Split(' ', ',', ';');
                    break;
            }
        }

        public static void ProcessCommandLineEarly()
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            foreach (string i in args)
            {
                if (i.StartsWith("--"))
                {
                    if (i.Contains("="))
                    {
                        string left = i.Substring(2, i.IndexOf("=") - 2);
                        string right = i.Substring(left.Length + 3);
                        mp.ProcessProperty(left, right);
                        ProcessProperty(left, right);
                    }
                    else
                    {
                        string switchName = i.Substring(2);

                        switch (switchName)
                        {
                            case "fs":
                            case "fullscreen":
                                mp.Fullscreen = true;
                                break;
                        }
                    }
                }
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

        public static void Register(string[] types)
        {
            Types = types;

            RegHelp.SetObject(@"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename, null, ExePath);
            RegHelp.SetObject($"HKCR\\Applications\\{ExeFilename}", "FriendlyAppName", "mpv.net media player");
            RegHelp.SetObject($"HKCR\\Applications\\{ExeFilename}\\shell\\open\\command", null, $"\"{ExePath}\" \"%1\"");
            RegHelp.SetObject(@"HKLM\SOFTWARE\Clients\Media\mpv\Capabilities", "ApplicationDescription", "mpv.net media player");
            RegHelp.SetObject(@"HKLM\SOFTWARE\Clients\Media\mpv\Capabilities", "ApplicationName", "mpv.net");
            RegHelp.SetObject($"HKCR\\SystemFileAssociations\\video\\OpenWithList\\{ExeFilename}", null, "");
            RegHelp.SetObject($"HKCR\\SystemFileAssociations\\audio\\OpenWithList\\{ExeFilename}", null, "");

            foreach (string ext in Types)
            {
                RegHelp.SetObject($"HKCR\\Applications\\{ExeFilename}\\SupportedTypes", "." + ext, "");
                RegHelp.SetObject($"HKCR\\" + "." + ext, null, ExeFilenameNoExt + "." + ext);
                RegHelp.SetObject($"HKCR\\" + "." + ext + "\\OpenWithProgIDs", ExeFilenameNoExt + "." + ext, "");
                if (App.VideoTypes.Contains(ext))
                    RegHelp.SetObject($"HKCR\\" + "." + ext, "PerceivedType", "video");
                if (App.AudioTypes.Contains(ext))
                    RegHelp.SetObject($"HKCR\\" + "." + ext, "PerceivedType", "audio");
                RegHelp.SetObject($"HKCR\\" + ExeFilenameNoExt + "." + ext + "\\shell\\open", null, "Play with " +  Application.ProductName);
                RegHelp.SetObject($"HKCR\\" + ExeFilenameNoExt + "." + ext + "\\shell\\open\\command", null, $"\"{ExePath}\" \"%1\"");
                RegHelp.SetObject(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities\FileAssociations", "." + ext, ExeFilenameNoExt + "." + ext);
            }
        }

        public static void Unregister()
        {
            RegHelp.RemoveKey(@"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename);
            RegHelp.RemoveKey($"HKCR\\Applications\\{ExeFilename}");
            RegHelp.RemoveKey(@"HKLM\SOFTWARE\Clients\Media\mpv.net");
            RegHelp.RemoveKey($"HKCR\\SystemFileAssociations\\video\\OpenWithList\\{ExeFilename}");
            RegHelp.RemoveKey($"HKCR\\SystemFileAssociations\\audio\\OpenWithList\\{ExeFilename}");

            foreach (string id in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (id.StartsWith(ExeFilenameNoExt + "."))
                    Registry.ClassesRoot.DeleteSubKeyTree(id);

                RegHelp.RemoveValue($"HKCR\\Software\\Classes\\" + id + "\\OpenWithProgIDs", ExeFilenameNoExt + id);
                RegHelp.RemoveValue($"HKLM\\Software\\Classes\\" + id + "\\OpenWithProgIDs", ExeFilenameNoExt + id);
            }
        }
    }

    public class RegHelp
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
            try {
                GetRootKey(path).DeleteSubKeyTree(path.Substring(5), false);
            } catch { }
        }

        public static void RemoveValue(string path, string name)
        {
            try {
                using (RegistryKey rk = GetRootKey(path).OpenSubKey(path.Substring(5), true))
                    if (!(rk is null))
                        rk.DeleteValue(name, false);
            } catch {}
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

        public static ObservableCollection<CommandItem> GetItems(string content)
        {
            var items = new ObservableCollection<CommandItem>();

            if (!string.IsNullOrEmpty(content))
            {
                foreach (string line in content.Split('\r', '\n'))
                {
                    string val = line.Trim();
                    if (val.StartsWith("#")) continue;
                    if (!val.Contains(" ")) continue;
                    CommandItem item = new CommandItem();
                    item.Input = val.Substring(0, val.IndexOf(" "));
                    if (item.Input == "_") item.Input = "";
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
                    MigrateCommands(item);
                    items.Add(item);
                }
            }
            return items;
        }

        private static ObservableCollection<CommandItem> _Items;

        public static ObservableCollection<CommandItem> Items {
            get {
                if (_Items is null)
                    _Items = GetItems(File.ReadAllText(mp.InputConfPath));
                return _Items;
            }
        }

        public static void MigrateCommands(CommandItem item)
        {
            switch (item.Command)
            {
                case "script-message mpv.net show-prefs":
                    item.Command = "script-message mpv.net show-conf-editor";
                    break;
                case "script-message mpv.net show-keys":
                    item.Command = "script-message mpv.net show-input-editor";
                    break;
                case "script-message mpv.net history":
                    item.Command = "script-message mpv.net show-history";
                    break;
                case "script-message mpv.net open-config-folder":
                    item.Command = "script-message open-conf-folder";
                    break;
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

    public class SingleProcess
    {
        public static int Message { get; } = RegisterWindowMessage("mpvnet_IPC");

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int RegisterWindowMessage(string id);

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);
    }
}