using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace mpvnet
{
    public class App
    {
        public static string[] VideoTypes { get; } = "264 265 asf avc avi avs flv h264 h265 hevc m2ts m2v m4v mkv mov mp4 mpeg mpg mpv mts ts vob vpy webm wmv y4m".Split(' ');
        public static string[] AudioTypes { get; } = "mp3 mp2 ac3 ogg opus flac wav w64 m4a dts dtsma dtshr dtshd eac3 thd thd+ac3 mka aac mpa".Split(' ');
        public static string[] ImageTypes { get; } = {"jpg", "bmp", "gif", "png"};
        public static string[] SubtitleTypes { get; } = { "srt", "ass", "idx", "sup", "ttxt", "ssa", "smi" };
        public static string[] UrlWhitelist { get; set; } = { "tube", "vimeo", "ard", "zdf" };

        public static string RegPath { get; } = @"HKCU\Software\" + Application.ProductName;
        public static string ConfPath { get => mp.ConfigFolder + "mpvnet.conf"; }
        public static string DarkMode { get; set; } = "always";
        public static string ProcessInstance { get; set; } = "single";
        public static string DarkColor { get; set; }
        public static string LightColor { get; set; }

        public static bool RememberHeight { get; set; } = true;
        public static bool RememberPosition { get; set; }
        public static bool DebugMode { get; set; }
        public static bool IsStartedFromTerminal { get; } = Environment.GetEnvironmentVariable("_started_from_console") == "yes";
        public static bool RememberVolume { get; set; } = true;
        public static bool AutoLoadFolder { get; set; } = true;
        public static bool ThemedMenu { get; set; }

        public static int StartThreshold { get; set; } = 1500;
        public static int RecentCount { get; set; } = 15;

        public static float MinimumAspectRatio { get; set; } = 1.3f;
               
        public static bool IsDarkMode {
            get => (DarkMode == "system" && Sys.IsDarkTheme) || DarkMode == "always";
        }

        public static void Init()
        {
            string dummy = mp.ConfigFolder;
            var dummy2 = mp.Conf;

            foreach (var i in Conf)
                ProcessProperty(i.Key, i.Value);

            if (App.DebugMode)
            {
                try
                {
                    string filePath = mp.ConfigFolder + "mpvnet-debug.log";
                    if (File.Exists(filePath)) File.Delete(filePath);
                    Trace.Listeners.Add(new TextWriterTraceListener(filePath));
                    Trace.AutoFlush = true;
                    //if (App.DebugMode) Trace.WriteLine("");
                }
                catch (Exception e)
                {
                    Msg.ShowException(e);
                }
            }

            mp.Shutdown += Shutdown;
            mp.Initialized += Initialized;
        }

        private static void Initialized()
        {
            if (RememberVolume)
            {
                mp.set_property_int("volume", RegHelp.GetInt(App.RegPath, "Volume"));
                mp.set_property_string("mute", RegHelp.GetString(App.RegPath, "Mute"));
            }
        }

        private static void Shutdown()
        {
            if (RememberVolume)
            {
                RegHelp.SetObject(App.RegPath, "Volume", mp.get_property_int("volume"));
                RegHelp.SetObject(App.RegPath, "Mute", mp.get_property_string("mute"));
            }
        }

        static Dictionary<string, string> _Conf;

        public static Dictionary<string, string> Conf {
            get {
                if (_Conf == null)
                {
                    _Conf = new Dictionary<string, string>();

                    if (File.Exists(ConfPath))
                        foreach (string i in File.ReadAllLines(ConfPath))
                            if (i.Contains("=") && !i.StartsWith("#"))
                                _Conf[i.Substring(0, i.IndexOf("=")).Trim()] = i.Substring(i.IndexOf("=") + 1).Trim();
                }
                return _Conf;
            }
        }

        public static bool ProcessProperty(string name, string value)
        {
            switch (name)
            {
                case "remember-position": RememberPosition = value == "yes"; return true;
                case "start-size": RememberHeight = value == "previous"; return true;
                case "process-instance": ProcessInstance = value; return true;
                case "dark-mode": DarkMode = value; return true;
                case "debug-mode": DebugMode = value == "yes"; return true;
                case "dark-color": DarkColor = value.Trim('\'', '"'); return true;
                case "light-color": LightColor = value.Trim('\'', '"'); return true;
                case "url-whitelist": UrlWhitelist = value.Split(' ', ',', ';'); return true;
                case "remember-volume": RememberVolume = value == "yes"; return true;
                case "start-threshold": StartThreshold = value.Int(); return true;
                case "minimum-aspect-ratio": MinimumAspectRatio = value.Float(); return true;
                case "auto-load-folder": AutoLoadFolder = value == "yes"; return true;
                case "themed-menu": ThemedMenu = value == "yes"; return true;
                case "recent-count": RecentCount = value.Int(); return true;
            }
            return false;
        }
    }
}