
using CommunityToolkit.Mvvm.Messaging;

using MpvNet.ExtensionMethod;
using MpvNet.Help;
using MpvNet.MVVM;

namespace MpvNet;

public class AppClass
{
    public List<string> TempFiles { get; } = new ();

    public string ConfPath { get => Player.ConfigFolder + "mpvnet.conf"; }
    public string ProcessInstance { get; set; } = "single";
    public string DarkMode { get; set; } = "always";
    public string DarkTheme { get; set; } = "dark";
    public string LightTheme { get; set; } = "light";
    public string StartSize { get; set; } = "height-session";
    public string Language { get; set; } = "system";
    public string CommandLine { get; set; } = Environment.CommandLine;
    public string MenuSyntax { get; set; } = "#menu:";

    public bool AutoLoadFolder { get; set; } = true;
    public bool DebugMode { get; set; }
    public bool Exit { get; set; }
    public bool IsTerminalAttached { get; } = Environment.GetEnvironmentVariable("_started_from_console") == "yes";
    public bool MediaInfo { get; set; } = true;
    public bool Queue { get; set; }
    public bool RememberAudioDevice { get; set; } = true;
    public bool RememberVolume { get; set; } = true;
    public bool RememberWindowPosition { get; set; }

    public int RecentCount { get; set; } = 15;

    public float AutofitAudio { get; set; } = 0.7f;
    public float AutofitImage { get; set; } = 0.8f;
    public float MinimumAspectRatio { get; set; }
    public float MinimumAspectRatioAudio { get; set; }

    readonly ExtensionLoader _extensionManager = new ExtensionLoader();

    AppSettings? _settings;

    public AppClass()
    {
        _extensionManager.UnhandledException +=  ex => Terminal.WriteError(ex);

        StrongReferenceMessenger.Default.Register<MainWindowIsLoadedMessage>(this, (r, msg) =>
        {
            TaskHelp.Run(() => _extensionManager.LoadFolder(Player.ConfigFolder + "extensions"));
        });
    }

    public AppSettings Settings => _settings ??= SettingsManager.Load();

    public void Init()
    {
        var useless1 = Player.ConfigFolder;
        var useless2 = Player.Conf;

        foreach (var i in Conf)
            ProcessProperty(i.Key, i.Value, true);

        if (DebugMode)
        {
            string filePath = Player.ConfigFolder + "MpvNet-debug.log";

            if (File.Exists(filePath))
                File.Delete(filePath);

            Trace.Listeners.Add(new TextWriterTraceListener(filePath));
            Trace.AutoFlush = true;
        }

        Player.Shutdown += Player_Shutdown;
        Player.Initialized += Player_Initialized;
    }

    public static string About => "Copyright (C) 2000-2024 mpv.net/mpv/mplayer\n" +
        $"{AppInfo.Product} {AppInfo.Version}" + GetLastWriteTime(Environment.ProcessPath!) + "\n" +
        $"{Player.GetPropertyString("mpv-version")}" + GetLastWriteTime(Folder.Startup + "libmpv-2.dll") + "\n" +
        $"ffmpeg {Player.GetPropertyString("ffmpeg-version")}\n" + "GPL v2 License";

    static string GetLastWriteTime(string path)
    {
        if (IsStoreVersion)
            return "";

        return $" ({File.GetLastWriteTime(path).ToShortDateString()})";
    }

    static bool IsStoreVersion => Folder.Startup.Contains("FrankSkare.mpvnet");

    void Player_Initialized()
    {
        if (RememberVolume)
        {
            Player.SetPropertyInt("volume", Settings.Volume);
            Player.SetPropertyString("mute", Settings.Mute);
        }

        if (RememberAudioDevice && Settings.AudioDevice != "")
            Player.SetPropertyString("audio-device", Settings.AudioDevice);
    }

    void Player_Shutdown()
    {
        Settings.Volume = Player.GetPropertyInt("volume");
        Settings.Mute = Player.GetPropertyString("mute");

        SettingsManager.Save(Settings);

        foreach (string file in TempFiles)
            FileHelp.Delete(file);
    }

    Dictionary<string, string>? _Conf;

    public Dictionary<string, string> Conf {
        get {
            if (_Conf == null)
            {
                _Conf = new Dictionary<string, string>();

                if (File.Exists(ConfPath))
                    foreach (string i in File.ReadAllLines(ConfPath))
                        if (i.Contains('=') && !i.StartsWith("#"))
                            _Conf[i[..i.IndexOf("=")].Trim()] = i[(i.IndexOf("=") + 1)..].Trim();
            }

            return _Conf;
        }
    }

    public bool ProcessProperty(string name, string value, bool writeError = false)
    {
        switch (name)
        {
            case "audio-file-extensions": FileTypes.Audio = value.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); return true;
            case "auto-load-folder": AutoLoadFolder = value == "yes"; return true;
            case "autofit-audio": AutofitAudio = value.Trim('%').ToInt(70) / 100f; return true;
            case "autofit-image": AutofitImage = value.Trim('%').ToInt(80) / 100f; return true;
            case "dark-mode": DarkMode = value; return true;
            case "dark-theme": DarkTheme = value.Trim('\'', '"'); return true;
            case "debug-mode": DebugMode = value == "yes"; return true;
            case "image-file-extensions": FileTypes.Image = value.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); return true;
            case "language": Language = value; return true;
            case "light-theme": LightTheme = value.Trim('\'', '"'); return true;
            case "media-info": MediaInfo = value == "yes"; return true;
            case "menu-syntax": MenuSyntax = value; return true;
            case "minimum-aspect-ratio-audio": MinimumAspectRatioAudio = value.ToFloat(); return true;
            case "minimum-aspect-ratio": MinimumAspectRatio = value.ToFloat(); return true;
            case "process-instance": ProcessInstance = value; return true;
            case "queue": Queue = value == "yes"; return true;
            case "recent-count": RecentCount = value.ToInt(15); return true;
            case "remember-audio-device": RememberAudioDevice = value == "yes"; return true;
            case "remember-volume": RememberVolume = value == "yes"; return true;
            case "remember-window-position": RememberWindowPosition = value == "yes"; return true;
            case "start-size": StartSize = value; return true;
            case "video-file-extensions": FileTypes.Video = value.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); return true;

            default:
                if (writeError)
                    Terminal.WriteError($"unknown MpvNet.conf property: {name}");

                return false;
        }
    }

    public static (string Title, string Path) GetTitleAndPath(string input)
    {
        if (input.Contains('|'))
        {
            var a = input.Split('|');
            return (a[1], a[0]);
        }

        return (input, input);
    }

    InputConf? _inputConf;

    public InputConf InputConf => _inputConf ??= new InputConf(Player.ConfigFolder + "input.conf");

    public void ApplyShowMenuFix()
    {
        if (Settings.ShowMenuFixApplied)
            return;

        if (File.Exists(InputConf.Path))
        {
            string content = File.ReadAllText(InputConf.Path);

            if (!content.Contains("script-message mpvnet show-menu") &&
                !content.Contains("script-message-to mpvnet show-menu"))

                File.WriteAllText(InputConf.Path, BR + content.Trim() + BR +
                    "MBTN_Right script-message-to mpvnet show-menu" + BR);
        }

        Settings.ShowMenuFixApplied = true;
    }

    public void ApplyInputDefaultBindingsFix()
    {
        if (Settings.InputDefaultBindingsFixApplied)
            return;

        if (File.Exists(Player.ConfPath))
        {
            string content = File.ReadAllText(Player.ConfPath);

            if (content.Contains("input-default-bindings = no"))
                File.WriteAllText(ConfPath, content.Replace("input-default-bindings = no", ""));

            if (content.Contains("input-default-bindings=no"))
                File.WriteAllText(ConfPath, content.Replace("input-default-bindings=no", ""));
        }

        Settings.InputDefaultBindingsFixApplied = true;
    }
}
