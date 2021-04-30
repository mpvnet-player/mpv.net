
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows;

using VB = Microsoft.VisualBasic;

using static mpvnet.NewLine;
using static mpvnet.Core;
using System.Threading.Tasks;

namespace mpvnet
{
    public class Commands
    {
        public static void Execute(string id, string[] args = null)
        {
            switch (id)
            {
                case "add-files-to-playlist": OpenFiles("append"); break; // deprecated 2019
                case "cycle-audio": CycleAudio(); break;
                case "execute-mpv-command": ExecuteMpvCommand(); break;
                case "load-audio": LoadAudio(); break;
                case "load-sub": LoadSubtitle(); break;
                case "manage-file-associations": // deprecated 2019
                case "open-conf-folder": ProcessHelp.ShellExecute(core.ConfigFolder); break;
                case "open-files": OpenFiles(args); break;
                case "open-optical-media": Open_DVD_Or_BD_Folder(); break;
                case "open-url": OpenURL(); break;
                case "playlist-first": PlaylistFirst(); break;
                case "playlist-last": PlaylistLast(); break;
                case "scale-window": ScaleWindow(float.Parse(args[0], CultureInfo.InvariantCulture)); break;
                case "shell-execute": ProcessHelp.ShellExecute(args[0]); break;
                case "show-about": ShowDialog(typeof(AboutWindow)); break;
                case "show-audio-devices": ShowTextWithEditor("audio-device-list", core.get_property_osd_string("audio-device-list")); break;
                case "show-command-palette": ShowDialog(typeof(CommandPaletteWindow)); break;
                case "show-commands": ShowCommands(); break;
                case "show-conf-editor": ShowDialog(typeof(ConfWindow)); break;
                case "show-decoders": ShowTextWithEditor("decoder-list", mpvHelp.GetDecoders()); break;
                case "show-demuxers": ShowTextWithEditor("demuxer-lavf-list", mpvHelp.GetDemuxers()); break;
                case "show-history": ShowHistory(); break;
                case "show-info": ShowInfo(); break;
                case "show-input-editor": ShowDialog(typeof(InputWindow)); break;
                case "show-keys": ShowTextWithEditor("input-key-list", core.get_property_string("input-key-list").Replace(",", BR)); break;
                case "show-media-search": ShowDialog(typeof(EverythingWindow)); break;
                case "show-profiles": ShowTextWithEditor("profile-list", mpvHelp.GetProfiles()); break;
                case "show-playlist": ShowPlaylist(); break;
                case "show-properties": ShowProperties(); break;
                case "show-protocols": ShowTextWithEditor("protocol-list", mpvHelp.GetProtocols()); break;
                case "show-setup-dialog": ShowDialog(typeof(SetupWindow)); break;
                case "show-text": ShowText(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2])); break;
                case "update-check": UpdateCheck.CheckOnline(true); break;

                default: Msg.ShowError($"No command '{id}' found."); break;
            }
        }

        public static void InvokeOnMainThread(Action action) => MainForm.Instance.BeginInvoke(action);

        public static void ShowDialog(Type winType)
        {
            InvokeOnMainThread(new Action(() => {
                Window win = Activator.CreateInstance(winType) as Window;
                new WindowInteropHelper(win).Owner = MainForm.Instance.Handle;
                win.ShowDialog();
            }));
        }

        public static void OpenFiles(params string[] args)
        {
            bool append = Control.ModifierKeys.HasFlag(Keys.Control);
            bool loadFolder = true;

            foreach (string arg in args)
            {
                if (arg == "append") append = true;
                if (arg == "no-folder") loadFolder = false;
            }

            InvokeOnMainThread(new Action(() => {
                using (var d = new OpenFileDialog() { Multiselect = true })
                    if (d.ShowDialog() == DialogResult.OK)
                        core.LoadFiles(d.FileNames, loadFolder, append);
            }));
        }

        public static void Open_DVD_Or_BD_Folder()
        {
            InvokeOnMainThread(new Action(() => {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Select a DVD or Blu-ray folder.";
                    dialog.ShowNewFolderButton = false;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        core.command("stop");
                        Thread.Sleep(500);

                        if (Directory.Exists(dialog.SelectedPath + "\\BDMV"))
                        {
                            core.set_property_string("bluray-device", dialog.SelectedPath);
                            core.LoadFiles(new[] { @"bd://" }, false, false);
                        }
                        else
                        {
                            core.set_property_string("dvd-device", dialog.SelectedPath);
                            core.LoadFiles(new[] { @"dvd://" }, false, false);
                        }
                    }
                }
            }));
        }

        public static void PlaylistFirst()
        {
            int pos = core.get_property_int("playlist-pos");

            if (pos != 0)
                core.set_property_int("playlist-pos", 0);
        }

        public static void PlaylistLast()
        {
            int pos = core.get_property_int("playlist-pos");
            int count = core.get_property_int("playlist-count");

            if (pos < count - 1)
                core.set_property_int("playlist-pos", count - 1);
        }

        public static void ShowHistory()
        {
            if (File.Exists(core.ConfigFolder + "history.txt"))
                ProcessHelp.ShellExecute(core.ConfigFolder + "history.txt");
            else
            {
                if (Msg.ShowQuestion("Create history.txt file in config folder?",
                    "mpv.net will write the date, time and filename of opened files to it.") == MsgResult.OK)

                    File.WriteAllText(core.ConfigFolder + "history.txt", "");
            }
        }

        public static void ShowInfo()
        {
            try
            {
                string performer, title, album, genre, date, duration, text = "";
                long fileSize = 0;
                string path = core.get_property_string("path");

                if (path.Contains("://"))
                    path = core.get_property_string("media-title");

                int width = core.get_property_int("video-params/w");
                int height = core.get_property_int("video-params/h");

                if (File.Exists(path))
                {
                    fileSize = new FileInfo(path).Length;

                    if (AudioTypes.Contains(path.Ext()))
                    {
                        using (MediaInfo mediaInfo = new MediaInfo(path))
                        {
                            performer = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Performer");
                            title = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Title");
                            album = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Album");
                            genre = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Genre");
                            date = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Recorded_Date");
                            duration = mediaInfo.GetInfo(MediaInfoStreamKind.Audio, "Duration/String");

                            if (performer != "") text += "Artist: " + performer + "\n";
                            if (title != "") text += "Title: " + title + "\n";
                            if (album != "") text += "Album: " + album + "\n";
                            if (genre != "") text += "Genre: " + genre + "\n";
                            if (date != "") text += "Year: " + date + "\n";
                            if (duration != "") text += "Length: " + duration + "\n";
                            text += "Size: " + mediaInfo.GetInfo(MediaInfoStreamKind.General, "FileSize/String") + "\n";
                            text += "Type: " + path.Ext().ToUpper();

                            core.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                    else if (ImageTypes.Contains(path.Ext()))
                    {
                        using (MediaInfo mediaInfo = new MediaInfo(path))
                        {
                            text =
                                "Width: " + mediaInfo.GetInfo(MediaInfoStreamKind.Image, "Width") + "\n" +
                                "Height: " + mediaInfo.GetInfo(MediaInfoStreamKind.Image, "Height") + "\n" +
                                "Size: " + mediaInfo.GetInfo(MediaInfoStreamKind.General, "FileSize/String") + "\n" +
                                "Type: " + path.Ext().ToUpper();

                            core.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                }

                TimeSpan position = TimeSpan.FromSeconds(core.get_property_number("time-pos"));
                TimeSpan duration2 = TimeSpan.FromSeconds(core.get_property_number("duration"));
                string videoFormat = core.get_property_string("video-format").ToUpper();
                string audioCodec = core.get_property_string("audio-codec-name").ToUpper();

                text = path.FileName() + "\n" +
                    FormatTime(position.TotalMinutes) + ":" +
                    FormatTime(position.Seconds) + " / " +
                    FormatTime(duration2.TotalMinutes) + ":" +
                    FormatTime(duration2.Seconds) + "\n" +
                    $"{width} x {height}\n";

                if (fileSize > 0)
                    text += Convert.ToInt32(fileSize / 1024.0 / 1024.0) + " MB\n";

                text += $"{videoFormat}\n{audioCodec}";

                core.commandv("show-text", text, "5000");
                string FormatTime(double value) => ((int)value).ToString("00");
            }
            catch (Exception e)
            {
                App.ShowException(e);
            }
        }

        public static void ExecuteMpvCommand() // deprecated 2019
        {
            InvokeOnMainThread(new Action(() => {
                string command = VB.Interaction.InputBox("Enter a mpv command to be executed.", "Execute Command", RegistryHelp.GetString(App.RegPath, "RecentExecutedCommand"));
             
                if (string.IsNullOrEmpty(command))
                    return;

                RegistryHelp.SetValue(App.RegPath, "RecentExecutedCommand", command);
                core.command(command, false);
            }));
        }
        
        public static void OpenURL()
        {
            InvokeOnMainThread(new Action(() => {
                string clipboard = System.Windows.Forms.Clipboard.GetText();

                if (string.IsNullOrEmpty(clipboard) || (!clipboard.Contains("://") && !File.Exists(clipboard)) ||
                    clipboard.Contains("\n"))
                {
                    App.ShowError("No URL found", "The clipboard does not contain a valid URL or file.");
                    return;
                }

                core.LoadFiles(new [] { clipboard }, false, Control.ModifierKeys.HasFlag(Keys.Control));
            }));
        }

        public static void LoadSubtitle()
        {
            InvokeOnMainThread(new Action(() => {
                using (var d = new OpenFileDialog())
                {
                    string path = core.get_property_string("path");

                    if (File.Exists(path))
                        d.InitialDirectory = Path.GetDirectoryName(path);

                    d.Multiselect = true;

                    if (d.ShowDialog() == DialogResult.OK)
                        foreach (string filename in d.FileNames)
                            core.commandv("sub-add", filename);
                }
            }));
        }

        public static void LoadAudio()
        {
            InvokeOnMainThread(new Action(() => {
                using (var d = new OpenFileDialog())
                {
                    string path = core.get_property_string("path");
                    if (File.Exists(path))
                        d.InitialDirectory = Path.GetDirectoryName(path);
                    d.Multiselect = true;

                    if (d.ShowDialog() == DialogResult.OK)
                        foreach (string i in d.FileNames)
                            core.commandv("audio-add", i);
                }
            }));
        }

        public static void CycleAudio()
        {
            MediaTrack[] tracks = core.MediaTracks.Where(track => track.Type == "a").ToArray();

            if (tracks.Length < 2)
                return;

            int aid = core.get_property_int("aid");

            if (++aid > tracks.Length)
                aid = 1;

            core.commandv("set", "aid", aid.ToString());
            core.commandv("show-text", aid + ": " + tracks[aid - 1].Text.Substring(3), "5000");
        }

        public static void ShowCommands()
        {
            string code = @"
                foreach ($item in ($json | ConvertFrom-Json | foreach { $_ } | sort name))
                {
                    ''
                    $item.name

                    foreach ($arg in $item.args)
                    {
                        $value = $arg.name + ' <' + $arg.type.ToLower() + '>'

                        if ($arg.optional -eq $true)
                        {
                            $value = '[' + $value + ']'
                        }

                        '    ' + $value
                    }
                }";

            string json = core.get_property_string("command-list");
            ShowTextWithEditor("command-list", PowerShell.InvokeAndReturnString(code, "json", json));
        }

        public static void ShowProperties()
        {
            var props = core.get_property_string("property-list").Split(',').OrderBy(prop => prop);
            ShowTextWithEditor("property-list", string.Join(BR, props));
        }

        public static void ShowTextWithEditor(string name, string text)
        {
            string file = Path.GetTempPath() + $"\\{name}.txt";
            File.WriteAllText(file, BR + text.Trim() + BR);
            ProcessHelp.ShellExecute(file);
        }

        public static void ScaleWindow(float factor)
        {
            core.RaiseScaleWindow(factor);
        }

        public static void ShowText(string text, int duration = 0, int fontSize = 0)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (duration == 0)
                duration = core.get_property_int("osd-duration");

            if (fontSize == 0)
                fontSize = core.get_property_int("osd-font-size");

            core.command("show-text \"${osd-ass-cc/0}{\\\\fs" + fontSize +
                "}${osd-ass-cc/1}" + text + "\" " + duration);
        }

        public static void ShowPlaylist(string[] args = null)
        {
            int duration = 5000;

            if (args?.Length == 1)
                duration = Convert.ToInt32(args[0]);

            var size = core.get_property_number("osd-font-size");
            core.set_property_number("osd-font-size", 40);
            core.command("show-text ${playlist} " + duration);

            App.RunTask(() => {
                Thread.Sleep(6000);
                core.set_property_number("osd-font-size", size);
            });
        }
    }
}
