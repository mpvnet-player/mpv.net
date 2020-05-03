
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using UI;

using static libmpv;
using static mpvnet.Core;

using System.Threading.Tasks;

namespace mpvnet
{
    public static class App
    {
        public static string[] VideoTypes { get; } = "264 265 asf avc avi avs flv h264 h265 hevc m2ts m2v m4v mkv mov mp4 mpeg mpg mpv mts ts vob vpy webm wmv y4m".Split(' ');
        public static string[] AudioTypes { get; } = "mp3 mp2 ac3 ogg opus flac wav w64 m4a dts dtsma dtshr dtshd eac3 thd thd+ac3 mka aac mpa".Split(' ');
        public static string[] ImageTypes { get; } = {"jpg", "bmp", "gif", "png"};
        public static string[] SubtitleTypes { get; } = { "srt", "ass", "idx", "sup", "ttxt", "ssa", "smi" };

        public static string RegPath { get; } = @"HKCU\Software\" + Application.ProductName;
        public static string ConfPath { get => core.ConfigFolder + "mpvnet.conf"; }
        public static string ProcessInstance { get; set; } = "single";
        public static string DarkMode { get; set; } = "always";
        public static string DarkTheme { get; set; } = "dark";
        public static string LightTheme { get; set; } = "light";
        public static string StartSize { get; set; } = "previous";

        public static bool RememberPosition { get; set; }
        public static bool DebugMode { get; set; }
        public static bool IsStartedFromTerminal { get; } = Environment.GetEnvironmentVariable("_started_from_console") == "yes";
        public static bool RememberVolume { get; set; } = true;
        public static bool AutoLoadFolder { get; set; } = true;
        public static bool Queue { get; set; }
        public static bool UpdateCheck { get; set; }

        public static int StartThreshold { get; set; } = 1500;
        public static int RecentCount { get; set; } = 15;

        public static float MinimumAspectRatio { get; set; } = 1.2f;

        public static Extension Extension { get; set; }

        public static bool IsDarkMode {
            get => (DarkMode == "system" && Sys.IsDarkTheme) || DarkMode == "always";
        }

        public static void Init()
        {
            string dummy = core.ConfigFolder;
            var dummy2 = core.Conf;

            foreach (var i in Conf)
                ProcessProperty(i.Key, i.Value, true);

            if (App.DebugMode)
            {
                try
                {
                    string filePath = core.ConfigFolder + "mpvnet-debug.log";

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    Trace.Listeners.Add(new TextWriterTraceListener(filePath));
                    Trace.AutoFlush = true;

                    //if (App.DebugMode)
                    //    Trace.WriteLine("");
                }
                catch (Exception e)
                {
                    Msg.ShowException(e);
                }
            }

            string themeContent = null;

            if (File.Exists(core.ConfigFolder + "theme.conf"))
                themeContent = File.ReadAllText(core.ConfigFolder + "theme.conf");

            Theme.Init(
                themeContent,
                Properties.Resources.theme,
                IsDarkMode ? DarkTheme : LightTheme);

            core.Shutdown += Shutdown;
            core.Initialized += Initialized;
            core.LogMessage += ShowFatalError;
        }

        static void ShowFatalError(mpv_log_level level, string msg)
        {
            if (!App.IsStartedFromTerminal && level == mpv_log_level.MPV_LOG_LEVEL_FATAL)
                Msg.ShowError(msg);
        }

        public static void RunAction(Action action)
        {
            Task.Run(() => {
                try
                {
                    action.Invoke();
                    Debug.WriteLine(Environment.TickCount);
                }
                catch (Exception e)
                {
                    ShowException(e);
                }
            });
        }

        public static void ShowException(object obj)
        {
            if (obj is Exception e)
            {
                if (App.IsStartedFromTerminal)
                    ConsoleHelp.WriteError(e.ToString());
                else
                    Msg.ShowException(e);
            }
            else
            {
                if (App.IsStartedFromTerminal)
                    ConsoleHelp.WriteError(obj.ToString());
                else
                    Msg.ShowError(obj.ToString());
            }
        }

        public static void ShowError(string title, string msg)
        {
            if (App.IsStartedFromTerminal)
            {
                ConsoleHelp.WriteError(title);
                ConsoleHelp.WriteError(msg);
            }
            else
                Msg.ShowError(title, msg);
        }

        static void Initialized()
        {
            if (RememberVolume)
            {
                core.set_property_int("volume", RegistryHelp.GetInt(App.RegPath, "Volume", 70));
                core.set_property_string("mute", RegistryHelp.GetString(App.RegPath, "Mute", "no"));
            }
        }

        static void Shutdown()
        {
            if (RememberVolume)
            {
                RegistryHelp.SetValue(App.RegPath, "Volume", core.get_property_int("volume"));
                RegistryHelp.SetValue(App.RegPath, "Mute", core.get_property_string("mute"));
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

        public static bool ProcessProperty(string name, string value, bool writeError = false)
        {
            switch (name)
            {
                case "remember-position": RememberPosition = value == "yes"; return true;
                case "debug-mode": DebugMode = value == "yes"; return true;
                case "remember-volume": RememberVolume = value == "yes"; return true;
                case "queue": Queue = value == "yes"; return true;
                case "auto-load-folder": AutoLoadFolder = value == "yes"; return true;
                case "update-check": UpdateCheck = value == "yes"; return true;
                case "start-size": StartSize = value; return true;
                case "process-instance": ProcessInstance = value; return true;
                case "dark-mode": DarkMode = value; return true;
                case "start-threshold": StartThreshold = value.Int(); return true;
                case "recent-count": RecentCount = value.Int(); return true;
                case "minimum-aspect-ratio": MinimumAspectRatio = value.Float(); return true;
                case "dark-theme": DarkTheme = value.Trim('\'', '"'); return true;
                case "light-theme": LightTheme = value.Trim('\'', '"'); return true;
                default:
                    if (writeError)
                        ConsoleHelp.WriteError($"unknown mpvnet.conf property: {name}");
                    return false;
            }
        }
    }
}
