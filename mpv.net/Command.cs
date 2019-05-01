using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Interop;

using Sys;

namespace mpvnet
{
    public class Command
    {
        public string Name { get; set; }
        public Action<string[]> Action { get; set; }

        static List<Command> commands;

        public static List<Command> Commands
        {
            get
            {
                if (commands == null)
                {
                    commands = new List<Command>();
                    Type type = typeof(Command);
                    MethodInfo[] methods = type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                    foreach (var i in methods)
                    {
                        ParameterInfo[] parameters = i.GetParameters();

                        if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(string[]))
                            continue;

                        Command cmd = new Command() { Name = i.Name.Replace("_","-"), Action = (Action<string[]>)i.CreateDelegate(typeof(Action<string[]>)) };
                        commands.Add(cmd);
                    }
                }
                return commands;
            }
        }

        public static void open_files(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                using (var d = new OpenFileDialog())
                {
                    d.Multiselect = true;
                    d.Filter = Sys.GetFilter();

                    if (d.ShowDialog() == DialogResult.OK)
                        mp.LoadFiles(d.FileNames);
                }
            }));
        }

        public static void open_conf_folder(string[] args)
        {
            Process.Start(mp.MpvConfFolder);
        }

        public static void show_input_editor(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                InputWindow w = new InputWindow();
                new WindowInteropHelper(w).Owner = MainForm.Instance.Handle;
                w.ShowDialog();
            }));
        }

        public static void show_conf_editor(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                ConfWindow w = new ConfWindow();
                new WindowInteropHelper(w).Owner = MainForm.Instance.Handle;
                w.ShowDialog();
            }));
        }

        public static void show_about(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                AboutWindow w = new AboutWindow();
                new WindowInteropHelper(w).Owner = MainForm.Instance.Handle;
                w.ShowDialog();
            }));
        }

        public static void show_command_palette(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                var w = new CommandPaletteWindow();
                new WindowInteropHelper(w).Owner = MainForm.Instance.Handle;
                w.ShowDialog();
            }));
        }

        public static void show_history(string[] args)
        {
            var fp = mp.MpvConfFolder + "history.txt";

            if (File.Exists(fp))
                Process.Start(fp);
            else
                if (Msg.ShowQuestion("Create history.txt file in config folder?",
                    "mpv.net will write the date, time and filename of opened files to it.") == MsgResult.OK)
                    File.WriteAllText(fp, "");
        }

        public static void shell_execute(string[] args) => Process.Start(args[0]);

        public static void show_info(string[] args)
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

                    if (FileAssociation.AudioTypes.Contains(Path.GetExtension(path).ToLower().TrimStart('.')))
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

                            mp.commandv("show-text", text, "5000");
                            return;
                        }
                    }
                }

                TimeSpan position = TimeSpan.FromSeconds(mp.get_property_number("time-pos"));
                TimeSpan duration2 = TimeSpan.FromSeconds(mp.get_property_number("duration"));
                string videoCodec = mp.get_property_string("video-format").ToUpper();

                text = Path.GetFileName(path) + "\n" +
                    FormatTime(position.TotalMinutes) + ":" +
                    FormatTime(position.Seconds) + " / " +
                    FormatTime(duration2.TotalMinutes) + ":" +
                    FormatTime(duration2.Seconds) + "\n" +
                    $"{width} x {height}\n";

                if (fileSize > 0)
                    text += Convert.ToInt32(fileSize / 1024.0 / 1024.0).ToString() + " MB\n";

                text += $"{videoCodec}\n";

                mp.commandv("show-text", text, "5000");
                string FormatTime(double value) => ((int)value).ToString("00");
            }
            catch (Exception)
            {
            }
        }

        public static void execute_mpv_command(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                string command = Microsoft.VisualBasic.Interaction.InputBox("Enter a mpv command to be executed.", "Execute Command", RegistryHelp.GetString("HKCU\\Software\\" + Application.ProductName, "RecentExecutedCommand"));
                if (string.IsNullOrEmpty(command)) return;
                RegistryHelp.SetObject("HKCU\\Software\\" + Application.ProductName, "RecentExecutedCommand", command);
                mp.command_string(command, false);
            }));
        }
        
        public static void open_url(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                string command = Microsoft.VisualBasic.Interaction.InputBox("Enter URL to be opened.");
                if (string.IsNullOrEmpty(command)) return;
                mp.LoadFiles(command);
            }));
        }

        public static void load_sub(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
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

        public static void load_audio(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
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

        public static void cycle_audio(string[] args)
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
    }
}