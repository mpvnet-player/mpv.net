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

        private static void CreateMpvConf()
        {
            if (!File.Exists(mp.mpvConfPath))
            {
                if (!Directory.Exists(mp.mpvConfFolderPath))
                    Directory.CreateDirectory(mp.mpvConfFolderPath);

                File.WriteAllText(mp.mpvConfPath, "# https://mpv.io/manual/master/#configuration-files");
            }
        }

        public static void show_prefs(string[] args)
        {
            CreateMpvConf();
            Process.Start(mp.mpvConfPath);
        }

        public static void history(string[] args)
        {
            var fp = mp.mpvConfFolderPath + "history.txt";

            if (File.Exists(fp))
                Process.Start(fp);
            else
                if (MsgQuestion("Create history.txt file in config folder?\r\n\r\nmpv.net will write the date, time and filename of opened files to it.") == DialogResult.OK)
                    File.WriteAllText(fp, "");
        }

        public static void shell_execute(string[] args)
        {
            Process.Start(args[0]);
        }

        public static void set_setting(string[] args)
        {
            CreateMpvConf();

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
            {
                File.WriteAllText(fp, String.Join(Environment.NewLine, confLines));
            }
            else
            {
                File.WriteAllText(fp,
                    File.ReadAllText(fp) + Environment.NewLine + args[0] + "=" + args[1]);
            }

            MsgInfo("Please restart mpv.net");
        }

        public static void show_info(string[] args)
        {
            var fi = new FileInfo(mp.GetStringProp("path"));

            using (var mi = new MediaInfo(fi.FullName))
            {
                var w = mi.GetInfo(MediaInfoStreamKind.Video, "Width");
                var h = mi.GetInfo(MediaInfoStreamKind.Video, "Height");
                var pos = TimeSpan.FromSeconds(mp.GetIntProp("time-pos"));
                var dur = TimeSpan.FromSeconds(mp.GetIntProp("duration"));
                string mibr = mi.GetInfo(MediaInfoStreamKind.Video, "BitRate");

                if (mibr == "")
                    mibr = "0";

                var br = Convert.ToInt32(mibr) / 1000.0 / 1000.0;
                var vf = mp.GetStringProp("video-format").ToUpper();
                var fn = fi.Name;

                if (fn.Length > 60)
                    fn = fn.Insert(59, "\r\n");

                var info =
                    FormatTime(pos.TotalMinutes) + ":" +
                    FormatTime(pos.Seconds) + " / " +
                    FormatTime(dur.TotalMinutes) + ":" +
                    FormatTime(dur.Seconds) + "\n" +
                    ((int)(fi.Length / 1024 / 1024)).ToString() +
                    $" MB - {w} x {h}\n{vf} - {br.ToString("f1")} Mb/s" + "\n" + fn;

                mp.Command("show-text", info, "5000");

                string FormatTime(double value) => ((int)(Math.Floor(value))).ToString("00");
            }
        }
    }
}