
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
    public class Sys
    {
        public static bool IsDarkTheme {
            get {
                object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
            
                if (value is null)
                    value = 1;

                return (int)value == 0;
            }
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

            RegistryHelp.SetValue(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename, null, ExePath);
            RegistryHelp.SetValue(@"HKCR\Applications\" + ExeFilename, "FriendlyAppName", "mpv.net media player");
            RegistryHelp.SetValue($@"HKCR\Applications\{ExeFilename}\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
            RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationDescription", "mpv.net media player");
            RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationName", "mpv.net");
            RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\video\OpenWithList\" + ExeFilename, null, "");
            RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + ExeFilename, null, "");
            RegistryHelp.SetValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net", @"SOFTWARE\Clients\Media\mpv.net\Capabilities");

            foreach (string ext in Types)
            {
                RegistryHelp.SetValue($@"HKCR\Applications\{ExeFilename}\SupportedTypes", "." + ext, "");
                RegistryHelp.SetValue($@"HKCR\" + "." + ext, null, ExeFilenameNoExt + "." + ext);
                RegistryHelp.SetValue($@"HKCR\" + "." + ext + @"\OpenWithProgIDs", ExeFilenameNoExt + "." + ext, "");

                if (App.VideoTypes.Contains(ext))
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, "PerceivedType", "video");

                if (App.AudioTypes.Contains(ext))
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, "PerceivedType", "audio");

                if (App.ImageTypes.Contains(ext))
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, "PerceivedType", "image");

                RegistryHelp.SetValue($@"HKCR\" + ExeFilenameNoExt + "." + ext + @"\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities\FileAssociations", "." + ext, ExeFilenameNoExt + "." + ext);
            }
        }

        public static void Unregister()
        {
            RegistryHelp.RemoveKey($@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename);
            RegistryHelp.RemoveKey($@"HKCR\Applications\" + ExeFilename);
            RegistryHelp.RemoveKey($@"HKLM\SOFTWARE\Clients\Media\mpv.net");
            RegistryHelp.RemoveKey($@"HKCR\SystemFileAssociations\video\OpenWithList\" + ExeFilename);
            RegistryHelp.RemoveKey($@"HKCR\SystemFileAssociations\audio\OpenWithList\" + ExeFilename);
            RegistryHelp.RemoveValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net");

            foreach (string id in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (id.StartsWith(ExeFilenameNoExt + "."))
                    Registry.ClassesRoot.DeleteSubKeyTree(id);

                RegistryHelp.RemoveValue($@"HKCR\Software\Classes\{id}\OpenWithProgIDs", ExeFilenameNoExt + id);
                RegistryHelp.RemoveValue($@"HKLM\Software\Classes\{id}\OpenWithProgIDs", ExeFilenameNoExt + id);
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

                    if (val.StartsWith("#"))
                        continue;

                    if (!val.Contains(" "))
                        continue;

                    CommandItem item = new CommandItem();
                    item.Input = val.Substring(0, val.IndexOf(" "));

                    if (item.Input == "_")
                        item.Input = "";

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

        // last change 2019
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

    public class Folder
    {
        public static string Startup { get; } = Application.StartupPath + @"\";
    }
}