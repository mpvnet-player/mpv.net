
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

using static mpvnet.Core;

namespace mpvnet
{
    public static class App
    {
        public static string RegPath { get; } = @"HKCU\Software\" + Application.ProductName;
        public static string ConfPath { get => core.ConfigFolder + "mpvnet.conf"; }
        public static string ProcessInstance { get; set; } = "single";
        public static string DarkMode { get; set; } = "always";
        public static string DarkTheme { get; set; } = "dark";
        public static string LightTheme { get; set; } = "light";
        public static string StartSize { get; set; } = "previous-height";

        public static bool RememberPosition { get; set; }
        public static bool DebugMode { get; set; }
        public static bool IsStartedFromTerminal { get; } = Environment.GetEnvironmentVariable("_started_from_console") == "yes";
        public static bool RememberVolume { get; set; } = true;
        public static bool AutoLoadFolder { get; set; } = true;
        public static bool Queue { get; set; }
        public static bool UpdateCheck { get; set; }
        public static bool GlobalMediaKeys { get; set; }

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

            if (DebugMode)
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
        }

        public static void RunTask(Action action)
        {
            Task.Run(() => {
                try {
                    action.Invoke();
                } catch (Exception e) {
                    ShowException(e);
                }
            });
        }

        public static string Version {
            get {
                return "Copyright (C) 2017-2021 mpv.net/mpv/mplayer\n" +
                    $"mpv.net {Application.ProductVersion} ({File.GetLastWriteTime(Application.ExecutablePath).ToShortDateString()})\n" +
                    $"{core.get_property_string("mpv-version")} ({File.GetLastWriteTime(Folder.Startup + "mpv-1.dll").ToShortDateString()})\nffmpeg {core.get_property_string("ffmpeg-version")}\nMIT License";
            }
        }

        public static void ShowException(object obj)
        {
            if (obj is Exception e)
            {
                if (IsStartedFromTerminal)
                    ConsoleHelp.WriteError(e.ToString());
                else
                    Msg.ShowException(e);
            }
            else
            {
                if (IsStartedFromTerminal)
                    ConsoleHelp.WriteError(obj.ToString());
                else
                    Msg.ShowError(obj.ToString());
            }
        }

        public static void ShowError(string title, string msg)
        {
            if (IsStartedFromTerminal)
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
                core.set_property_int("volume", RegistryHelp.GetInt("Volume", 70));
                core.set_property_string("mute", RegistryHelp.GetString("Mute", "no"));
            }
        }

        static void Shutdown()
        {
            if (RememberVolume)
            {
                RegistryHelp.SetValue(RegPath, "Volume", core.get_property_int("volume"));
                RegistryHelp.SetValue(RegPath, "Mute", core.get_property_string("mute"));
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
                case "start-threshold": StartThreshold = value.ToInt(); return true;
                case "recent-count": RecentCount = value.ToInt(); return true;
                case "minimum-aspect-ratio": MinimumAspectRatio = value.ToFloat(); return true;
                case "dark-theme": DarkTheme = value.Trim('\'', '"'); return true;
                case "light-theme": LightTheme = value.Trim('\'', '"'); return true;
                case "video-file-extensions": VideoTypes = value.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); return true;
                case "audio-file-extensions": AudioTypes = value.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); return true;
                case "image-file-extensions": ImageTypes = value.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); return true;
                default:
                    if (writeError)
                        ConsoleHelp.WriteError($"unknown mpvnet.conf property: {name}");
                    return false;
            }
        }

        public static void ShowSetup()
        {
            int value = RegistryHelp.GetInt(Folder.Startup);

            if (value != 1)
            {
                if (Msg.ShowQuestion("Would you like to setup mpv.net?",
                    "The setup allows to create a start menu shortcut, file associations and " +
                    "adding mpv.net to the Path environment variable.") == MsgResult.OK)

                    Commands.Execute("show-setup-dialog");
                else
                    Msg.Show("The setup dialog can be found in the context menu at:\n\nTools > Setup");

                RegistryHelp.SetValue(RegistryHelp.ApplicationKey, Folder.Startup, 1);
            }
        }
    }
}
