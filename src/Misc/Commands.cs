
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

using WinForms = System.Windows.Forms;

using static mpvnet.Global;

namespace mpvnet
{
    public class Commands
    {
        public static void Execute(string id, string[] args)
        {
            switch (id)
            {
                case "cycle-audio": CycleAudio(); break;
                case "cycle-subtitles": CycleSubtitles(); break;
                case "load-audio": LoadAudio(); break;
                case "load-sub": LoadSubtitle(); break;
                case "open-clipboard": OpenFromClipboard(); break;
                case "open-conf-folder": ProcessHelp.ShellExecute(Core.ConfigFolder); break;
                case "open-files": OpenFiles(args); break;
                case "open-optical-media": Open_DVD_Or_BD_Folder(); break;
                case "play-pause": PlayPause(); break;
                case "playlist-add": PlaylistAdd(Convert.ToInt32(args[0])); break;
                case "playlist-first": PlaylistFirst(); break;
                case "playlist-last": PlaylistLast(); break;
                case "playlist-random": PlaylistRandom(); break;
                case "quick-bookmark": QuickBookmark(); break;
                case "reg-file-assoc": RegisterFileAssociations(args[0]); break;
                case "scale-window": ScaleWindow(float.Parse(args[0], CultureInfo.InvariantCulture)); break;
                case "select-profile": SelectProfile(); break;
                case "shell-execute": ProcessHelp.ShellExecute(args[0]); break;
                case "show-about": ShowDialog(typeof(AboutWindow)); break;
                case "show-audio-devices": Msg.ShowInfo(Core.GetPropertyOsdString("audio-device-list")); break;
                case "show-audio-tracks": ShowAudioTracks(); break;
                case "show-chapters": ShowChapters(); break;
                case "show-command-palette": ShowCommandPalette(); break;
                case "show-commands": ShowCommands(); break;
                case "show-conf-editor": ShowDialog(typeof(ConfWindow)); break;
                case "show-decoders": ShowStrings(mpvHelp.GetDecoders().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)); break;
                case "show-demuxers": ShowStrings(mpvHelp.GetDemuxers().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)); break;
                case "show-history": ShowHistory(); break;
                case "show-info": ShowInfo(); break;
                case "show-input-editor": ShowDialog(typeof(InputWindow)); break;
                case "show-keys": ShowStrings(Core.GetPropertyString("input-key-list").Split(',')); break;
                case "show-media-info": ShowMediaInfo(args); break;
                case "show-menu": ShowMenu(); break;
                case "show-playlist": ShowPlaylist(); break;
                case "show-profiles": Msg.ShowInfo(mpvHelp.GetProfiles()); break;
                case "show-progress": ShowProgress(); break;
                case "show-properties": ShowProperties(); break;
                case "show-protocols": ShowStrings(mpvHelp.GetProtocols().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)); break;
                case "show-recent": ShowRecent(); break;
                case "show-subtitle-tracks": ShowSubtitleTracks(); break;
                case "show-text": ShowText(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2])); break;
                case "window-scale": WindowScale(float.Parse(args[0], CultureInfo.InvariantCulture)); break;

                // deprecated 2019
                case "add-files-to-playlist": OpenFiles("append"); break;

                // deprecated 2020
                case "execute-mpv-command": Msg.ShowError("command was removed, reset input.conf by deleting it, in the new menu use the on screen console."); break;
                case "key-binding": if (args[0] == "show-playlist") ShowPlaylist(); break;

                // deprecated 2022
                case "show-setup-dialog": ShowSetupDialog(); break;
                case "open-url": OpenFromClipboard(); break;
            }
        }

        public static void ShowTextWithEditor(string name, string text)
        {
            string file = Path.Combine(Path.GetTempPath(), name + ".txt");
            App.TempFiles.Add(file);
            File.WriteAllText(file, BR + text.Trim() + BR);
            ProcessHelp.ShellExecute(file);
        }

        public static void ShowDialog(Type winType) => App.InvokeOnMainThread(() =>
        {
            Window win = Activator.CreateInstance(winType) as Window;
            new WindowInteropHelper(win).Owner = MainForm.Instance.Handle;
            win.ShowDialog();
        });

        public static void OpenFiles(params string[] args)
        {
            bool append = Control.ModifierKeys.HasFlag(Keys.Control);

            foreach (string arg in args)
                if (arg == "append")
                    append = true;

            App.InvokeOnMainThread(new Action(() => {
                using (var d = new OpenFileDialog() { Multiselect = true })
                    if (d.ShowDialog() == DialogResult.OK)
                        Core.LoadFiles(d.FileNames, true, append);
            }));
        }

        public static void Open_DVD_Or_BD_Folder() => App.InvokeOnMainThread(() =>
        {
            var dialog = new FolderBrowser();

            if (dialog.Show())
                Core.LoadDiskFolder(dialog.SelectedPath);
        });

        public static void PlaylistFirst()
        {
            if (Core.PlaylistPos != 0)
                Core.SetPropertyInt("playlist-pos", 0);
        }

        public static void PlaylistLast()
        {
            int count = Core.GetPropertyInt("playlist-count");

            if (Core.PlaylistPos < count - 1)
                Core.SetPropertyInt("playlist-pos", count - 1);
        }

        public static void PlayPause()
        {
            int count = Core.GetPropertyInt("playlist-count");

            if (count > 0)
                Core.Command("cycle pause");
            else if (App.Settings.RecentFiles.Count > 0)
            {
                foreach (string i in App.Settings.RecentFiles)
                {
                    if (i.Contains("://") || File.Exists(i))
                    {
                        Core.LoadFiles(new[] { i }, true, false);
                        break;
                    }
                }
            }
        }

        public static void ShowHistory()
        {
            if (File.Exists(Core.ConfigFolder + "history.txt"))
                ProcessHelp.ShellExecute(Core.ConfigFolder + "history.txt");
            else
            {
                if (Msg.ShowQuestion("Create a 'history.txt' file in the config folder?" + BR2 +
                    "mpv.net will write the date, time, play length and path of watched files to it.") == MessageBoxResult.OK)

                    File.WriteAllText(Core.ConfigFolder + "history.txt", "");
            }
        }

        public static void ShowInfo()
        {
            if (Core.PlaylistPos == -1)
                return;

            string text;
            long fileSize = 0;
            string path = Core.GetPropertyString("path");

            if (File.Exists(path))
            {
                if (CorePlayer.AudioTypes.Contains(path.Ext()))
                {
                    text = Core.GetPropertyOsdString("filtered-metadata");
                    Core.CommandV("show-text", text, "5000");
                    return;
                }
                else if (CorePlayer.ImageTypes.Contains(path.Ext()))
                {
                    fileSize = new FileInfo(path).Length;
                    text = "Width: " + Core.GetPropertyInt("width") + "\n" +
                           "Height: " + Core.GetPropertyInt("height") + "\n" +
                           "Size: " + Convert.ToInt32(fileSize / 1024.0) + " KB\n" +
                           "Type: " + path.Ext().ToUpper();

                    Core.CommandV("show-text", text, "5000");
                    return;
                }
                else
                {
                    Core.Command("script-message-to mpvnet show-media-info osd");
                    return;
                }
            }

            if (path.Contains("://")) path = Core.GetPropertyString("media-title");
            string videoFormat = Core.GetPropertyString("video-format").ToUpper();
            string audioCodec = Core.GetPropertyString("audio-codec-name").ToUpper();
            int width = Core.GetPropertyInt("video-params/w");
            int height = Core.GetPropertyInt("video-params/h");
            TimeSpan len = TimeSpan.FromSeconds(Core.GetPropertyDouble("duration"));
            text = path.FileName() + "\n";
            text += FormatTime(len.TotalMinutes) + ":" + FormatTime(len.Seconds) + "\n";
            if (fileSize > 0) text += Convert.ToInt32(fileSize / 1024.0 / 1024.0) + " MB\n";
            text += $"{width} x {height}\n";
            text += $"{videoFormat}\n{audioCodec}";
            Core.CommandV("show-text", text, "5000");
        }

        static string FormatTime(double value) => ((int)value).ToString("00");

        public static void ShowProgress()
        {
            TimeSpan position = TimeSpan.FromSeconds(Core.GetPropertyDouble("time-pos"));
            TimeSpan duration = TimeSpan.FromSeconds(Core.GetPropertyDouble("duration"));

            string text = FormatTime(position.TotalMinutes) + ":" +
                          FormatTime(position.Seconds) + " / " +
                          FormatTime(duration.TotalMinutes) + ":" +
                          FormatTime(duration.Seconds) + "    " +
                          DateTime.Now.ToString("H:mm dddd d MMMM", CultureInfo.InvariantCulture);

            Core.CommandV("show-text", text, "5000");
        }

        public static void OpenFromClipboard() => App.InvokeOnMainThread(() =>
        {
            if (WinForms.Clipboard.ContainsFileDropList())
            {
                string[] files = WinForms.Clipboard.GetFileDropList().Cast<string>().ToArray();
                Core.LoadFiles(files, false, Control.ModifierKeys.HasFlag(Keys.Control));
            }
            else
            {
                string clipboard = WinForms.Clipboard.GetText();
                List<string> files = new List<string>();

                foreach (string i in clipboard.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    if (i.Contains("://") || File.Exists(i))
                        files.Add(i);

                if (files.Count == 0)
                {
                    App.ShowError("The clipboard does not contain a valid URL or file.");
                    return;
                }

                Core.LoadFiles(files.ToArray(), false, Control.ModifierKeys.HasFlag(Keys.Control));
            }
        });

        public static void LoadSubtitle() => App.InvokeOnMainThread(() =>
        {
            using (var d = new OpenFileDialog())
            {
                string path = Core.GetPropertyString("path");

                if (File.Exists(path))
                    d.InitialDirectory = Path.GetDirectoryName(path);

                d.Multiselect = true;

                if (d.ShowDialog() == DialogResult.OK)
                    foreach (string filename in d.FileNames)
                        Core.CommandV("sub-add", filename);
            }
        });

        public static void LoadAudio() => App.InvokeOnMainThread(() =>
        {
            using (var d = new OpenFileDialog())
            {
                string path = Core.GetPropertyString("path");

                if (File.Exists(path))
                    d.InitialDirectory = Path.GetDirectoryName(path);

                d.Multiselect = true;

                if (d.ShowDialog() == DialogResult.OK)
                    foreach (string i in d.FileNames)
                        Core.CommandV("audio-add", i);
            }
        });

        public static void CycleAudio()
        {
            Core.UpdateExternalTracks();

            lock (Core.MediaTracksLock)
            {
                MediaTrack[] tracks = Core.MediaTracks.Where(track => track.Type == "a").ToArray();

                if (tracks.Length < 1)
                {
                    Core.CommandV("show-text", "No audio tracks");
                    return;
                }

                int aid = Core.GetPropertyInt("aid");

                if (tracks.Length > 1)
                {
                    if (++aid > tracks.Length)
                        aid = 1;

                    Core.SetPropertyInt("aid", aid);
                }

                Core.CommandV("show-text", aid + "/" + tracks.Length + ": " + tracks[aid - 1].Text.Substring(3), "5000");
            }
        }

        public static void CycleSubtitles()
        {
            Core.UpdateExternalTracks();

            lock (Core.MediaTracksLock)
            {
                MediaTrack[] tracks = Core.MediaTracks.Where(track => track.Type == "s").ToArray();

                if (tracks.Length < 1)
                {
                    Core.CommandV("show-text", "No subtitles");
                    return;
                }

                int sid = Core.GetPropertyInt("sid");

                if (tracks.Length > 1)
                {
                    if (++sid > tracks.Length)
                        sid = 0;

                    Core.SetPropertyInt("sid", sid);
                }

                if (sid == 0)
                    Core.CommandV("show-text", "No subtitle");
                else
                    Core.CommandV("show-text", sid + "/" + tracks.Length + ": " + tracks[sid - 1].Text.Substring(3), "5000");
            }
        }

        public static void ShowCommands()
        {
            string jsonString = Core.GetPropertyString("command-list");
            var jsonObject = jsonString.FromJson<List<Dictionary<string, object>>>().OrderBy(i => i["name"]);
            StringBuilder sb = new StringBuilder();

            foreach (Dictionary<string, object> dic in jsonObject)
            {
                sb.AppendLine();
                sb.AppendLine(dic["name"].ToString());

                foreach (Dictionary<string, object> i2 in dic["args"] as List<object>)
                {
                    string value = i2["name"].ToString() + " <" + i2["type"].ToString().ToLower() + ">";

                    if ((bool)i2["optional"] == true)
                        value = "[" + value + "]";

                    sb.AppendLine("    " + value);
                }
            }

            ShowTextWithEditor("command-list", sb.ToString());
        }

        public static void ScaleWindow(float factor) => Core.RaiseScaleWindow(factor);

        public static void WindowScale(float value) => Core.RaiseWindowScaleNET(value);

        public static void ShowText(string text, int duration = 0, int fontSize = 0)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (duration == 0)
                duration = Core.GetPropertyInt("osd-duration");

            if (fontSize == 0)
                fontSize = Core.GetPropertyInt("osd-font-size");

            Core.Command("show-text \"${osd-ass-cc/0}{\\\\fs" + fontSize +
                "}${osd-ass-cc/1}" + text + "\" " + duration);
        }

        public static void ShowMediaInfo(string[] args) => App.InvokeOnMainThread(() =>
        {
            if (args == null || args.Length == 0)
            {
                (string Name, string Value)[] pairs = {
                    ("Show text box",    "script-message-to mpvnet show-media-info default"),
                    ("Show text editor", "script-message-to mpvnet show-media-info editor"),
                    ("Show on screen",   "script-message-to mpvnet show-media-info osd"),
                    ("Show full",        "script-message-to mpvnet show-media-info editor full"),
                    ("Show raw",         "script-message-to mpvnet show-media-info editor full raw") };

                var list = pairs.Select(i => new CommandPaletteItem(i.Name, () => Core.Command(i.Value)));
                CommandPalette.Instance.SetItems(list);
                MainForm.Instance.ShowCommandPalette();
                CommandPalette.Instance.SelectFirst();
                return;
            }

            string path = Core.GetPropertyString("path");
            string text = "";

            bool full = args.Contains("full");
            bool raw = args.Contains("raw");
            bool editor = args.Contains("editor");
            bool osd = args.Contains("osd");

            if (App.MediaInfo && !osd && File.Exists(path) && !path.Contains(@"\\.\pipe\"))
                using (MediaInfo mediaInfo = new MediaInfo(path))
                    text = Regex.Replace(mediaInfo.GetSummary(full, raw), "Unique ID.+", "");
            else
            {
                Core.UpdateExternalTracks();
                text = "N: " + Core.GetPropertyString("filename") + BR;
                lock (Core.MediaTracksLock)
                    foreach (MediaTrack track in Core.MediaTracks)
                        text += track.Text + BR;
            }

            text = text.TrimEx();

            if (editor)
                ShowTextWithEditor("media-info", text);
            else if (osd)
                ShowText(text.Replace("\r", ""), 5000, 17);
            else
            {
                MsgBoxEx.MessageBoxEx.MsgFontFamily = new FontFamily("Consolas");
                Msg.ShowInfo(text);
                MsgBoxEx.MessageBoxEx.MsgFontFamily = new FontFamily("Segoe UI");
            }
        });

        public static void ShowCommandPalette() => App.InvokeOnMainThread(() =>
        {
            CommandPalette.Instance.SetItems(CommandPalette.GetItems());
            MainForm.Instance.ShowCommandPalette();
            CommandPalette.Instance.SelectFirst();
        });

        public static void ShowAudioTracks() => App.InvokeOnMainThread(() =>
        {
            Core.UpdateExternalTracks();

            lock (Core.MediaTracksLock)
            {
                MediaTrack[] tracks = Core.MediaTracks.Where(track => track.Type == "a").ToArray();
       
                if (tracks.Length < 1)
                {
                    Core.CommandV("show-text", "No audio tracks");
                    return;
                }

                List<CommandPaletteItem> items = new List<CommandPaletteItem>();

                foreach (MediaTrack i in tracks)
                {
                    MediaTrack track = i;

                    CommandPaletteItem item = new CommandPaletteItem()
                    {
                        Text = track.Text,
                        Action = () => {
                            Core.CommandV("set", "aid", track.ID.ToString());
                            Core.CommandV("show-text", track.ID + "/" + tracks.Length + ": " +
                                tracks[track.ID - 1].Text.Substring(3), "5000");
                        }
                    };

                    items.Add(item);
                }

                CommandPalette.Instance.SetItems(items);
                MainForm.Instance.ShowCommandPalette();
                CommandPalette.Instance.SelectFirst();
            }

        });

        public static void ShowSubtitleTracks() => App.InvokeOnMainThread(() =>
        {
            Core.UpdateExternalTracks();

            lock (Core.MediaTracksLock)
            {
                MediaTrack[] tracks = Core.MediaTracks.Where(track => track.Type == "s").ToArray();
         
                if (tracks.Length < 1)
                {
                    Core.CommandV("show-text", "No subtitle tracks");
                    return;
                }

                List<CommandPaletteItem> items = new List<CommandPaletteItem>();

                foreach (MediaTrack i in tracks)
                {
                    MediaTrack track = i;

                    CommandPaletteItem item = new CommandPaletteItem()
                    {
                        Text = track.Text,
                        Action = () => {
                            Core.CommandV("set", "sid", track.ID.ToString());
                            Core.CommandV("show-text", track.ID + "/" + tracks.Length + ": " +
                                tracks[track.ID - 1].Text.Substring(3), "5000");
                        }
                    };

                    items.Add(item);
                }

                CommandPalette.Instance.SetItems(items);
                MainForm.Instance.ShowCommandPalette();
                CommandPalette.Instance.SelectFirst();
            }
        });

        public static void ShowPlaylist() => App.InvokeOnMainThread(() =>
        {
            int count = Core.GetPropertyInt("playlist-count");
            string currentPath = Core.GetPropertyString("path");
            CommandPaletteItem currentItem = null;

            if (count < 1)
                return;

            List<CommandPaletteItem> items = new List<CommandPaletteItem>();

            for (int i = 0; i < count; i++)
            {
                int index = i;
                string file = Core.GetPropertyString($"playlist/{i}/filename");

                CommandPaletteItem item = new CommandPaletteItem()
                {
                    Text = file.FileName(),
                    Action = () => Core.SetPropertyInt("playlist-pos", index)
                };

                if (string.IsNullOrEmpty(item.Text))
                    item.Text = file;

                items.Add(item);

                if (currentPath.ToLowerEx() == file.ToLowerEx())
                    currentItem = item;
            }

            CommandPalette.Instance.SetItems(items);

            if (currentItem != null)
            {
                CommandPalette.Instance.MainListView.SelectedItem = currentItem;
                CommandPalette.Instance.MainListView.ScrollIntoView(
                    CommandPalette.Instance.MainListView.SelectedItem);
            }

            MainForm.Instance.ShowCommandPalette();
        });

        public static void ShowProperties() => App.InvokeOnMainThread(() =>
        {
            var props = Core.GetPropertyString("property-list").Split(',').OrderBy(prop => prop);
            List<CommandPaletteItem> items = new List<CommandPaletteItem>();

            foreach (string i in props)
            {
                string prop = i;

                CommandPaletteItem item = new CommandPaletteItem()
                {
                    Text = prop,
                    Action = () =>
                    {
                        string propValue = Core.GetPropertyString(prop);

                        if (propValue.ContainsEx("${"))
                            propValue += BR2 + Core.Expand(propValue);

                        App.ShowInfo(prop + "\n\n" + propValue);
                    }
                };

                items.Add(item);
            }

            CommandPalette.Instance.SetItems(items);
            MainForm.Instance.ShowCommandPalette();
        });

        public static void ShowRecent() => App.InvokeOnMainThread(() =>
        {
            List<CommandPaletteItem> items = new List<CommandPaletteItem>();

            foreach (string path in App.Settings.RecentFiles)
            {
                var file = App.GetTitleAndPath(path);

                CommandPaletteItem item = new CommandPaletteItem()
                {
                    Text = file.Title.ShortPath(60),
                    Action = () => Core.LoadFiles(new[] { file.Path }, true, Control.ModifierKeys.HasFlag(Keys.Control))
                };

                items.Add(item);
            }

            CommandPalette.Instance.SetItems(items);
            MainForm.Instance.ShowCommandPalette();
            CommandPalette.Instance.SelectFirst();
        });

        public static void RegisterFileAssociations(string perceivedType)
        {
            string[] extensions = { };

            switch (perceivedType)
            {
                case "video": extensions = CorePlayer.VideoTypes; break;
                case "audio": extensions = CorePlayer.AudioTypes; break;
                case "image": extensions = CorePlayer.ImageTypes; break;
            }

            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = WinForms.Application.ExecutablePath;
                    proc.StartInfo.Arguments = "--register-file-associations " +
                        perceivedType + " " + string.Join(" ", extensions);
                    proc.StartInfo.Verb = "runas";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                    proc.WaitForExit();

                    if (proc.ExitCode == 0)
                        Msg.ShowInfo("File associations were successfully " + 
                            (perceivedType == "unreg" ? "removed" : "created") +
                            ".\n\nFile Explorer icons will refresh after process restart.");
                    else
                        Msg.ShowError("Error creating file associations.");
                }
            } catch { }
        }

        public static void ShowStrings(string[] strings) => App.InvokeOnMainThread(() =>
        {
            List<CommandPaletteItem> items = new List<CommandPaletteItem>();

            foreach (string i in strings)
            {
                string str = i;

                CommandPaletteItem item = new CommandPaletteItem()
                {
                    Text = str,
                    Action = () => Msg.ShowInfo(str)
                };

                items.Add(item);
            }

            CommandPalette.Instance.SetItems(items);
            MainForm.Instance.ShowCommandPalette();
            CommandPalette.Instance.SelectFirst();
        });

        public static void ShowSetupDialog() => App.InvokeOnMainThread(() =>
        {
            (string Name, string Value)[] pairs = {
                ("Register video file associations", "script-message-to mpvnet reg-file-assoc video"),
                ("Register audio file associations", "script-message-to mpvnet reg-file-assoc audio"),
                ("Register image file associations", "script-message-to mpvnet reg-file-assoc image"),
                ("Unregister file associations",     "script-message-to mpvnet reg-file-assoc unreg") };

            var list = pairs.Select(i => new CommandPaletteItem(i.Name, () => Core.Command(i.Value)));
            CommandPalette.Instance.SetItems(list);
            MainForm.Instance.ShowCommandPalette();
            CommandPalette.Instance.SelectFirst();
        });

        public static void SelectProfile() => App.InvokeOnMainThread(() =>
        {
            var items = Core.ProfileNames.Where(i => !i.StartsWith("extension."))
                                         .Select(i => new CommandPaletteItem(i, () => {
                                             Core.CommandV("show-text", i);
                                             Core.CommandV("apply-profile", i);
                                         }));

            CommandPalette.Instance.SetItems(items);
            MainForm.Instance.ShowCommandPalette();
            CommandPalette.Instance.SelectFirst();
        });

        public static void ShowChapters() => App.InvokeOnMainThread(() =>
        {
            var items = Core.GetChapters().Select(i => new CommandPaletteItem(i.Title, i.TimeDisplay, () =>
                Core.CommandV("seek", i.Time.ToString(CultureInfo.InvariantCulture), "absolute")));

            CommandPalette.Instance.SetItems(items);
            MainForm.Instance.ShowCommandPalette();
            CommandPalette.Instance.SelectFirst();
        });

        public static void ShowMenu() => Core.RaiseShowMenu();

        public static void PlaylistAdd(int value)
        {
            int pos = Core.PlaylistPos;
            int count = Core.GetPropertyInt("playlist-count");

            if (count < 2)
                return;

            pos = pos + value;

            if (pos < 0)
                pos = count - 1;

            if (pos > count - 1)
                pos = 0;

            Core.SetPropertyInt("playlist-pos", pos);
        }

        public static void PlaylistRandom()
        {
            int count = Core.GetPropertyInt("playlist-count");
            Core.SetPropertyInt("playlist-pos", new Random().Next(count));
        }

        public static void QuickBookmark()
        {
            if (App.QuickBookmark == 0)
            {
                App.QuickBookmark = (float)Core.GetPropertyDouble("time-pos");

                if (App.QuickBookmark != 0)
                    Core.Command("show-text 'Bookmark Saved'");
            }
            else
            {
                Core.SetPropertyDouble("time-pos", App.QuickBookmark);
                App.QuickBookmark = 0;
            }
        }
    }
}
