using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using vbnet;
using static vbnet.UI.MainModule;

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
                        mpv.LoadFiles(d.FileNames);
                }
            }));
        }

        public static void open_config_folder(string[] args)
        {
            ProcessHelp.Start(Folder.AppDataRoaming + "mpv");
        }

        public static void show_keys(string[] args)
        {
            ProcessHelp.Start(OS.GetTextEditor(), '"' + mpv.InputConfPath + '"');
        }

        private static void CreateMpvConf()
        {
            if (!File.Exists(mpv.mpvConfPath))
            {
                var dirPath = Folder.AppDataRoaming + "mpv\\";

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                File.WriteAllText(mpv.mpvConfPath, "# https://mpv.io/manual/master/#configuration-files");
            }
        }

        public static void show_prefs(string[] args)
        {
            CreateMpvConf();
            ProcessHelp.Start(OS.GetTextEditor(), '"' + mpv.mpvConfPath + '"');
        }

        public static void history(string[] args)
        {
            var fp = Folder.AppDataRoaming + "mpv\\history.txt";

            if (File.Exists(fp))
                Process.Start(fp);
            else
                if (MsgQuestion($"Create history.txt file in config folder?{BR2}mpv.net will write the date, time and filename of opened files to it.") == DialogResult.OK)
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
            string fp = mpv.mpvConfPath;
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
            var fi = new FileInfo(mpv.GetStringProp("path"));

            using (var mi = new MediaInfo(fi.FullName))
            {
                var w = mi.GetInfo(StreamKind.Video, "Width");
                var h = mi.GetInfo(StreamKind.Video, "Height");
                var pos = TimeSpan.FromSeconds(mpv.GetIntProp("time-pos"));
                var dur = TimeSpan.FromSeconds(mpv.GetIntProp("duration"));
                string mibr = mi.GetInfo(StreamKind.Video, "BitRate");

                if (mibr == "")
                    mibr = "0";

                var br = Convert.ToInt32(mibr) / 1000.0 / 1000.0;
                var vf = mpv.GetStringProp("video-format").ToUpper();
                var fn = fi.Name;

                if (fn.Length > 60)
                    fn = fn.Insert(59, BR);

                var info =
                    FormatTime(pos.TotalMinutes) + ":" +
                    FormatTime(pos.Seconds) + " / " +
                    FormatTime(dur.TotalMinutes) + ":" +
                    FormatTime(dur.Seconds) + "\n" +
                    ((int)(fi.Length / 1024 / 1024)).ToString() +
                    $" MB - {w} x {h}\n{vf} - {br.ToString("f1")} Mb/s" + "\n" + fn;

                mpv.Command("show-text", info, "5000");

                string FormatTime(double value) => ((int)(Math.Floor(value))).ToString("00");
            }
        }
    }
}