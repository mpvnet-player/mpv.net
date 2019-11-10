
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

using VB = Microsoft.VisualBasic;

namespace mpvnet
{
    public class Command
    {
        public static void Execute(string id, string[] args)
        {
            switch (id)
            {
                case "open-files": OpenFiles(args); break;
                case "update-check": UpdateCheck.CheckOnline(true); break;
                case "open-url": OpenURL(); break;
                case "open-optical-media": Open_DVD_Or_BD_Folder(); break;
                case "manage-file-associations": // deprecated 2019
                case "show-setup-dialog": ShowDialog(typeof(SetupWindow)); break;
                case "cycle-audio": CycleAudio(); break;
                case "load-audio": LoadAudio(); break;
                case "load-sub": LoadSubtitle(); break;
                case "execute-mpv-command": ExecuteMpvCommand(); break;
                case "show-history": ShowHistory(); break;
                case "show-media-search": ShowDialog(typeof(EverythingWindow)); break;
                case "show-command-palette": ShowDialog(typeof(CommandPaletteWindow)); break;
                case "show-about": ShowDialog(typeof(AboutWindow)); break;
                case "show-conf-editor": ShowDialog(typeof(ConfWindow)); break;
                case "show-input-editor": ShowDialog(typeof(InputWindow)); break;
                case "open-conf-folder": Process.Start(mp.ConfigFolder); break;
                case "shell-execute": Process.Start(args[0]); break;
                case "show-info": ShowInfo(); break;
                case "playlist-last": PlaylistLast(); break;
                case "add-files-to-playlist": OpenFiles("append"); break; // deprecated 2019
                default: Msg.ShowError($"No command '{id}' found."); break;
            }

            MainForm.Instance.BeginInvoke(new Action(() => {
                Message m = new Message() { Msg = 0x0202 }; // WM_LBUTTONUP
                Native.SendMessage(MainForm.Instance.Handle, m.Msg, m.WParam, m.LParam);           
            }));
        }

        public static void InvokeOnMainThread(Action action) => MainForm.Instance.Invoke(action);

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
                        mp.Load(d.FileNames, loadFolder, append);
            }));
        }

        public static void Open_DVD_Or_BD_Folder()
        {
            InvokeOnMainThread(new Action(() => {
                using (var d = new FolderBrowserDialog())
                {
                    d.Description = "Select a DVD or Blu-ray folder.";
                    d.ShowNewFolderButton = false;

                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        if (Directory.Exists(d.SelectedPath + "\\BDMV"))
                        {
                            mp.set_property_string("bluray-device", d.SelectedPath);
                            mp.Load(new[] { @"bd://" }, false, false);
                        }
                        else
                        {
                            mp.set_property_string("dvd-device", d.SelectedPath);
                            mp.Load(new[] { @"dvd://" }, false, false);
                        }
                    }
                }
            }));
        }

        public static void PlaylistLast() => mp.set_property_int("playlist-pos", mp.get_property_int("playlist-count") - 1);

        public static void ShowHistory()
        {
            if (File.Exists(mp.ConfigFolder + "history.txt"))
                Process.Start(mp.ConfigFolder + "history.txt");
            else
            {
                if (Msg.ShowQuestion("Create history.txt file in config folder?",
                    "mpv.net will write the date, time and filename of opened files to it.") == MsgResult.OK)

                    File.WriteAllText(mp.ConfigFolder + "history.txt", "");
            }
        }

        public static void ShowInfo()
        {
            try
            {
                string performer, title, album, genre, date, duration, text = "";
                long fileSize = 0;
                string path = mp.get_property_string("path");

                if (path.Contains("://"))
                    path = mp.get_property_string("media-title");

                int width = mp.get_property_int("video-params/w");
                int height = mp.get_property_int("video-params/h");

                if (File.Exists(path))
                {
                    fileSize = new FileInfo(path).Length;

                    if (App.AudioTypes.Contains(path.ShortExt()))
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
                            text += "Type: " + path.ShortExt().ToUpper();

                            mp.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                    else if (App.ImageTypes.Contains(path.ShortExt()))
                    {
                        using (MediaInfo mediaInfo = new MediaInfo(path))
                        {
                            text =
                                "Width: " + mediaInfo.GetInfo(MediaInfoStreamKind.Image, "Width") + "\n" +
                                "Height: " + mediaInfo.GetInfo(MediaInfoStreamKind.Image, "Height") + "\n" +
                                "Size: " + mediaInfo.GetInfo(MediaInfoStreamKind.General, "FileSize/String") + "\n" +
                                "Type: " + path.ShortExt().ToUpper();

                            mp.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                }

                TimeSpan position = TimeSpan.FromSeconds(mp.get_property_number("time-pos"));
                TimeSpan duration2 = TimeSpan.FromSeconds(mp.get_property_number("duration"));
                string videoFormat = mp.get_property_string("video-format").ToUpper();
                string audioCodec = mp.get_property_string("audio-codec-name").ToUpper();

                text = path.FileName() + "\n" +
                    FormatTime(position.TotalMinutes) + ":" +
                    FormatTime(position.Seconds) + " / " +
                    FormatTime(duration2.TotalMinutes) + ":" +
                    FormatTime(duration2.Seconds) + "\n" +
                    $"{width} x {height}\n";

                if (fileSize > 0)
                    text += Convert.ToInt32(fileSize / 1024.0 / 1024.0) + " MB\n";

                text += $"{videoFormat}\n{audioCodec}";

                mp.commandv("show-text", text, "5000");
                string FormatTime(double value) => ((int)value).ToString("00");
            }
            catch (Exception e)
            {
                Msg.ShowException(e);
            }
        }

        public static void ExecuteMpvCommand() // deprecated 2019
        {
            InvokeOnMainThread(new Action(() => {
                string command = VB.Interaction.InputBox("Enter a mpv command to be executed.", "Execute Command", RegistryHelp.GetString(App.RegPath, "RecentExecutedCommand"));
                if (string.IsNullOrEmpty(command)) return;
                RegistryHelp.SetValue(App.RegPath, "RecentExecutedCommand", command);
                mp.command(command, false);
            }));
        }
        
        public static void OpenURL()
        {
            InvokeOnMainThread(new Action(() => {
                string clipboard = System.Windows.Forms.Clipboard.GetText();
                if (string.IsNullOrEmpty(clipboard) || (!clipboard.Contains("://") && !File.Exists(clipboard)) || clipboard.Contains("\n"))
                {
                    Msg.ShowError("The clipboard does not contain a valid URL or file, URLs have to contain :// and are not allowed to contain a newline character.");
                    return;
                }
                mp.Load(new [] { clipboard }, false, Control.ModifierKeys.HasFlag(Keys.Control));
            }));
        }

        public static void LoadSubtitle()
        {
            InvokeOnMainThread(new Action(() => {
                using (var d = new OpenFileDialog())
                {
                    string path = mp.get_property_string("path");
                    if (File.Exists(path)) d.InitialDirectory = Path.GetDirectoryName(path);
                    d.Multiselect = true;
                    if (d.ShowDialog() == DialogResult.OK)
                        foreach (string i in d.FileNames)
                            mp.commandv("sub-add", i);
                }
            }));
        }

        public static void LoadAudio()
        {
            InvokeOnMainThread(new Action(() => {
                using (var d = new OpenFileDialog())
                {
                    string path = mp.get_property_string("path");
                    if (File.Exists(path))
                        d.InitialDirectory = Path.GetDirectoryName(path);
                    d.Multiselect = true;

                    if (d.ShowDialog() == DialogResult.OK)
                        foreach (string i in d.FileNames)
                            mp.commandv("audio-add", i);
                }
            }));
        }

        public static void CycleAudio()
        {
            string path = mp.get_property_string("path");
            if (!File.Exists(path)) return;

            using (MediaInfo mi = new MediaInfo(path))
            {
                MediaTrack[] audTracks = mp.MediaTracks.Where(track => track.Type == "a").ToArray();
                if (audTracks.Length < 2) return;
                int aid = mp.get_property_int("aid");
                aid += 1;
                if (aid > audTracks.Length) aid = 1;
                mp.commandv("set", "aid", aid.ToString());
                mp.commandv("show-text", audTracks[aid - 1].Text.Substring(3), "5000");
            }
        }
    }
}