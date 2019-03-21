using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using static mpvnet.StaticUsing;

namespace mpvnet
{
    public class Command
    {
        public string Name { get; set; }
        public Action<string[]> Action { get; set; }

        private static List<Command> commands;

        public static List<Command> Commands
        {
            get
            {
                if (commands == null)
                {
                    commands = new List<Command>();
                    var type = typeof(Command);
                    var methods = type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                    foreach (var i in methods)
                    {
                        var parameters = i.GetParameters();

                        if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(string[]))
                            continue;

                        var cmd = new Command() { Name = i.Name.Replace("_","-"), Action = (Action<string[]>)i.CreateDelegate(typeof(Action<string[]>)) };
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

        public static void open_config_folder(string[] args)
        {
            Process.Start(mp.mpvConfFolderPath);
        }

        public static void show_keys(string[] args)
        {
            Process.Start(mp.InputConfPath);
        }

        public static void show_prefs(string[] args)
        {
            Process.Start(mp.mpvConfPath);
        }

        public static void history(string[] args)
        {
            var fp = mp.mpvConfFolderPath + "history.txt";

            if (File.Exists(fp))
                Process.Start(fp);
            else
                if (MsgQuestion("Create history.txt file in config folder?\n\nmpv.net will write the date, time and filename of opened files to it.") == DialogResult.OK)
                    File.WriteAllText(fp, "");
        }

        public static void shell_execute(string[] args)
        {
            Process.Start(args[0]);
        }

        public static void set_setting(string[] args)
        {
            bool changed = false;
            string fp = mp.mpvConfPath;
            var confLines = File.ReadAllLines(fp);

            for (int i = 0; i < confLines.Length; i++)
            {
                if (confLines[i].Left("=").Trim() == args[0])
                {
                    confLines[i] = args[0] + "=" + args[1];
                    changed = true;
                }
            }

            if (changed)
                File.WriteAllText(fp, String.Join(Environment.NewLine, confLines));
            else
                File.WriteAllText(fp, File.ReadAllText(fp) + Environment.NewLine + args[0] + "=" + args[1]);

            MsgInfo("Please restart mpv.net");
        }

        public static void show_info(string[] args)
        {
            var fileInfo = new FileInfo(mp.get_property_string("path"));

            using (var mediaInfo = new MediaInfo(fileInfo.FullName))
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
                    if (title != "") text += "Title: " + title + "\n";
                    if (album != "") text += "Album: " + album + "\n";
                    if (genre != "") text += "Genre: " + genre + "\n";
                    if (date != "") text += "Year: " + date + "\n";
                    if (duration != "") text += "Length: " + duration + "\n";

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

                    var bitrate2 = Convert.ToDouble(bitrate) / 1000.0 / 1000.0;
                    var format = mp.get_property_string("video-format").ToUpper();
                    var filename = fileInfo.Name;

                    var text =
                        FormatTime(position.TotalMinutes) + ":" +
                        FormatTime(position.Seconds) + " / " +
                        FormatTime(duration.TotalMinutes) + ":" +
                        FormatTime(duration.Seconds) + "\n" +
                        Convert.ToInt32(fileInfo.Length / 1024 / 1024).ToString() +
                        $" MB - {width} x {height}\n{format} - {bitrate2.ToString("f1")} Mb/s" + "\n" + filename;

                    mp.commandv("show-text", text, "5000");
                }

                string FormatTime(double value) => ((int)value).ToString("00");
            }
        }
    }
}