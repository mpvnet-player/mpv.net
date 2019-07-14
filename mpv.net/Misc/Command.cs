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
                case "manage-file-associations": ManageFileAssociations(); break;
                case "cycle-audio": CycleAudio(); break;
                case "load-audio": LoadAudio(); break;
                case "load-sub": LoadSubtitle(); break;
                case "open-url": OpenURL(); break;
                case "execute-mpv-command": ExecuteMpvCommand(); break;
                case "show-history": ShowHistory(); break;
                case "show-media-search": ShowDialog(typeof(EverythingWindow)); break;
                case "show-command-palette": ShowDialog(typeof(CommandPaletteWindow)); break;
                case "show-about": ShowDialog(typeof(AboutWindow)); break;
                case "show-conf-editor": ShowDialog(typeof(ConfWindow)); break;
                case "show-input-editor": ShowDialog(typeof(InputWindow)); break;
                case "open-conf-folder": Process.Start(mp.ConfFolder); break;
                case "open-files": OpenFiles(args); break;
                case "shell-execute": Process.Start(args[0]); break;
                case "show-info": ShowInfo(); break;
                case "add-files-to-playlist": OpenFiles("append"); break; // deprecated 2019
                default: Msg.ShowError($"No command '{id}' found."); break;
            }
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

        public static void ShowHistory()
        {
            var fp = mp.ConfFolder + "history.txt";

            if (File.Exists(fp))
                Process.Start(fp);
            else
                if (Msg.ShowQuestion("Create history.txt file in config folder?",
                    "mpv.net will write the date, time and filename of opened files to it.") == MsgResult.OK)
                    File.WriteAllText(fp, "");
        }

        public static void ShowInfo()
        {
            try
            {
                string performer, title, album, genre, date, duration, text = "";
                long fileSize = 0;
                string path = mp.get_property_string("path");
                int width = mp.get_property_int("video-params/w");
                int height = mp.get_property_int("video-params/h");

                if (File.Exists(path))
                {
                    fileSize = new FileInfo(path).Length;

                    if (App.AudioTypes.Contains(Path.GetExtension(path).ToLower().TrimStart('.')))
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
                            text += "Type: " + Path.GetExtension(path).ToUpper().TrimStart('.');

                            mp.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                    else if (App.ImageTypes.Contains(Path.GetExtension(path).ToLower().TrimStart('.')))
                    {
                        using (MediaInfo mediaInfo = new MediaInfo(path))
                        {
                            text =
                                "Width: " + mediaInfo.GetInfo(MediaInfoStreamKind.Image, "Width") + "\n" +
                                "Height: " + mediaInfo.GetInfo(MediaInfoStreamKind.Image, "Height") + "\n" +
                                "Size: " + mediaInfo.GetInfo(MediaInfoStreamKind.General, "FileSize/String") + "\n" +
                                "Type: " + Path.GetExtension(path).ToUpper().TrimStart('.');

                            mp.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                }

                TimeSpan position = TimeSpan.FromSeconds(mp.get_property_number("time-pos"));
                TimeSpan duration2 = TimeSpan.FromSeconds(mp.get_property_number("duration"));
                string videoFormat = mp.get_property_string("video-format").ToUpper();
                string audioCodec = mp.get_property_string("audio-codec-name").ToUpper();

                text = Path.GetFileName(path) + "\n" +
                    FormatTime(position.TotalMinutes) + ":" +
                    FormatTime(position.Seconds) + " / " +
                    FormatTime(duration2.TotalMinutes) + ":" +
                    FormatTime(duration2.Seconds) + "\n" +
                    $"{width} x {height}\n";

                if (fileSize > 0)
                    text += Convert.ToInt32(fileSize / 1024.0 / 1024.0).ToString() + " MB\n";

                text += $"{videoFormat}\n{audioCodec}";

                mp.commandv("show-text", text, "5000");
                string FormatTime(double value) => ((int)value).ToString("00");
            }
            catch (Exception e)
            {
                Msg.ShowException(e);
            }
        }

        public static void ExecuteMpvCommand()
        {
            InvokeOnMainThread(new Action(() => {
                string command = VB.Interaction.InputBox("Enter a mpv command to be executed.", "Execute Command", RegHelp.GetString(App.RegPath, "RecentExecutedCommand"));
                if (string.IsNullOrEmpty(command)) return;
                RegHelp.SetObject(App.RegPath, "RecentExecutedCommand", command);
                mp.command_string(command, false);
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
                    d.InitialDirectory = Path.GetDirectoryName(mp.get_property_string("path", false));
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
                    d.InitialDirectory = Path.GetDirectoryName(mp.get_property_string("path", false));
                    d.Multiselect = true;

                    if (d.ShowDialog() == DialogResult.OK)
                        foreach (string i in d.FileNames)
                            mp.commandv("audio-add", i);
                }
            }));
        }

        public static void CycleAudio()
        {
            string filePath = mp.get_property_string("path", false);
            if (!File.Exists(filePath)) return;

            using (MediaInfo mi = new MediaInfo(filePath))
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

        public static void ManageFileAssociations()
        {
            using (var td = new TaskDialog<string>())
            {
                td.MainInstruction = "Choose an option.";
                td.MainIcon = MsgIcon.Shield;

                td.AddCommandLink("Register video file extensions", "video");
                td.AddCommandLink("Register audio file extensions", "audio");
                td.AddCommandLink("Register audio file extensions", "image");
                td.AddCommandLink("Unregister file extensions", "unreg");

                string result = td.Show();

                if (!string.IsNullOrEmpty(result))
                {
                    using (var proc = new Process())
                    {
                        proc.StartInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                        proc.StartInfo.Arguments = "--reg-file-assoc " + result;
                        proc.StartInfo.Verb = "runas";
                        try { proc.Start(); } catch { }
                    }
                }
            }
        }
    }
}