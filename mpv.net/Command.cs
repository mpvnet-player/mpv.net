using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using VBNET;

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
                    d.Filter = Misc.GetFilter(Misc.FileTypes);

                    if (d.ShowDialog() == DialogResult.OK)
                        mp.LoadFiles(d.FileNames);
                }
            }));
        }

        public static void open_conf_folder(string[] args)
        {
            Process.Start(mp.MpvConfFolderPath);
        }

        public static void show_input_editor(string[] args)
        {
            Process.Start(Application.StartupPath + "\\mpvInputEdit.exe");
        }

        public static void show_conf_editor(string[] args)
        {
            Process.Start(Application.StartupPath + "\\mpvConfEdit.exe");
        }

        public static void show_history(string[] args)
        {
            var fp = mp.MpvConfFolderPath + "history.txt";

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
                FileInfo fileInfo = new FileInfo(mp.get_property_string("path"));

                using (MediaInfo mediaInfo = new MediaInfo(fileInfo.FullName))
                {
                    string width = mediaInfo.GetInfo(MediaInfoStreamKind.Video, "Width");

                    if (width == "")
                    {
                        string performer = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Performer");
                        string title = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Title");
                        string album = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Album");
                        string genre = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Genre");
                        string date = mediaInfo.GetInfo(MediaInfoStreamKind.General, "Recorded_Date");
                        string duration = mediaInfo.GetInfo(MediaInfoStreamKind.Audio, "Duration/String");

                        string text = "";

                        if (performer != "") text += "Artist: " + performer + "\n";
                        if (title != "")     text += "Title: "  + title + "\n";
                        if (album != "")     text += "Album: "  + album + "\n";
                        if (genre != "")     text += "Genre: "  + genre + "\n";
                        if (date != "")      text += "Year: "   + date + "\n";
                        if (duration != "")  text += "Length: " + duration + "\n";

                        mp.commandv("show-text", text, "5000");
                    }
                    else
                    {
                        string height = mediaInfo.GetInfo(MediaInfoStreamKind.Video, "Height");
                        TimeSpan position = TimeSpan.FromSeconds(mp.get_property_number("time-pos"));
                        TimeSpan duration = TimeSpan.FromSeconds(mp.get_property_number("duration"));
                        string bitrate = mediaInfo.GetInfo(MediaInfoStreamKind.Video, "BitRate");

                        if (bitrate == "")
                            bitrate = "0";

                        double bitrate2 = Convert.ToDouble(bitrate) / 1000.0 / 1000.0;
                        string videoCodec = mp.get_property_string("video-format").ToUpper();
                        string filename = fileInfo.Name;

                        string text = filename + "\n" +
                            FormatTime(position.TotalMinutes) + ":" +
                            FormatTime(position.Seconds) + " / " +
                            FormatTime(duration.TotalMinutes) + ":" +
                            FormatTime(duration.Seconds) + "\n" +
                            $"{width} x {height}\n" +
                            $"{bitrate2.ToString("f1")} Mb/s\n" +
                            Convert.ToInt32(fileInfo.Length / 1024 / 1024).ToString() + " MB\n" +
                            $"{videoCodec}\n";

                        mp.commandv("show-text", text, "5000");
                    }

                    string FormatTime(double value) => ((int)value).ToString("00");
                }
            }
            catch (Exception)
            {
            }
        }

        public static void execute_mpv_command(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                string command = Microsoft.VisualBasic.Interaction.InputBox("Enter a mpv command to be executed.");
                if (string.IsNullOrEmpty(command)) return;
                mp.command_string(command, false);
            }));
        }
        
        public static void open_url(string[] args)
        {
            MainForm.Instance.Invoke(new Action(() => {
                string command = Microsoft.VisualBasic.Interaction.InputBox("Enter URL to be opened.");
                if (string.IsNullOrEmpty(command)) return;
                mp.LoadURL(command);
            }));
        }

        public static void load_sub(string[] args)
        {
            MainForm.Instance.BeginInvoke(new Action(() => {
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
            MainForm.Instance.BeginInvoke(new Action(() => {
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