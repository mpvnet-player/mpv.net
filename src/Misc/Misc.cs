
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Microsoft.Win32;

using static mpvnet.Global;

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

    public class Input
    {
        public static string WM_APPCOMMAND_to_mpv_key(int value)
        {
            switch (value)
            {
                case 5: return "SEARCH";       // BROWSER_SEARCH
                case 6: return "FAVORITES";    // BROWSER_FAVORITES
                case 7: return "HOMEPAGE";     // BROWSER_HOME
                case 15: return "MAIL";         // LAUNCH_MAIL
                case 33: return "PRINT";        // PRINT
                case 11: return "NEXT";         // MEDIA_NEXTTRACK
                case 12: return "PREV";         // MEDIA_PREVIOUSTRACK
                case 13: return "STOP";         // MEDIA_STOP
                case 14: return "PLAYPAUSE";    // MEDIA_PLAY_PAUSE
                case 46: return "PLAY";         // MEDIA_PLAY
                case 47: return "PAUSE";        // MEDIA_PAUSE
                case 48: return "RECORD";       // MEDIA_RECORD
                case 49: return "FORWARD";      // MEDIA_FAST_FORWARD
                case 50: return "REWIND";       // MEDIA_REWIND
                case 51: return "CHANNEL_UP";   // MEDIA_CHANNEL_UP
                case 52: return "CHANNEL_DOWN"; // MEDIA_CHANNEL_DOWN
            }

            return null;
        }
    }

    public class FileAssociation
    {
        static string ExePath = Application.ExecutablePath;
        static string ExeFilename = Path.GetFileName(Application.ExecutablePath);
        static string ExeFilenameNoExt = Path.GetFileNameWithoutExtension(Application.ExecutablePath);

        public static void Register(string perceivedType, string[] extensions)
        {
            if (perceivedType != "unreg")
            {
                RegistryHelp.SetValue(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename, null, ExePath);
                RegistryHelp.SetValue(@"HKCR\Applications\" + ExeFilename, "FriendlyAppName", "mpv.net media player");
                RegistryHelp.SetValue($@"HKCR\Applications\{ExeFilename}\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationDescription", "mpv.net media player");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationName", "mpv.net");
                RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\video\OpenWithList\" + ExeFilename, null, "");
                RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + ExeFilename, null, "");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net", @"SOFTWARE\Clients\Media\mpv.net\Capabilities");

                foreach (string ext in extensions)
                {
                    RegistryHelp.SetValue($@"HKCR\Applications\{ExeFilename}\SupportedTypes", "." + ext, "");
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, null, ExeFilenameNoExt + "." + ext);
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext + @"\OpenWithProgIDs", ExeFilenameNoExt + "." + ext, "");
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, "PerceivedType", perceivedType);
                    RegistryHelp.SetValue(@"HKCR\" + ExeFilenameNoExt + "." + ext + @"\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
                    RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities\FileAssociations", "." + ext, ExeFilenameNoExt + "." + ext);
                }
            }
            else
            {
                RegistryHelp.RemoveKey(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename);
                RegistryHelp.RemoveKey(@"HKCR\Applications\" + ExeFilename);
                RegistryHelp.RemoveKey(@"HKLM\SOFTWARE\Clients\Media\mpv.net");
                RegistryHelp.RemoveKey(@"HKCR\SystemFileAssociations\video\OpenWithList\" + ExeFilename);
                RegistryHelp.RemoveKey(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + ExeFilename);
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
        public string Display { get { return string.IsNullOrEmpty(Path) ? Command : Path; } }

        public CommandItem() { }

        public CommandItem(SerializationInfo info, StreamingContext context) { }

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        string _Input = "";

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

                    items.Add(item);
                }
            }
            return items;
        }

        static ObservableCollection<CommandItem> _Items;

        public static ObservableCollection<CommandItem> Items {
            get {
                if (_Items is null)
                    _Items = GetItems(File.ReadAllText(Core.InputConfPath));

                return _Items;
            }
        }
    }

    public class Folder
    {
        public static string Startup { get; } = Application.StartupPath.AddSep();
        public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).AddSep();
    }

    public class CommandPaletteItem
    {
        public string Text { get; set; } = "";
        public string SecondaryText { get; set; } = "";
        public Action Action { get; set; }
    }

    public class CommandPalette
    {
        public static CommandPaletteControl Instance { get; } = new CommandPaletteControl();

        public static IEnumerable<CommandPaletteItem> GetItems()
        {
            return CommandItem.Items
                .Where(i => i.Command != "")
                .Select(i => new CommandPaletteItem() {
                    Text = i.Display,
                    SecondaryText = i.Input,
                    Action = () => Core.Command(i.Command)
                });
        }
    }
}
