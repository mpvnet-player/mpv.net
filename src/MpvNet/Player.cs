
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

using MpvNet.ExtensionMethod;
using MpvNet.Help;
using MpvNet.Native;

using static MpvNet.Native.LibMpv;

namespace MpvNet;

public class MainPlayer : MpvClient
{
    public string ConfPath { get => ConfigFolder + "mpv.conf"; }
    public string GPUAPI { get; set; } = "auto";
    public string Path { get; set; } = "";
    public string VO { get; set; } = "gpu";
    public string UsedInputConfContent { get; set; } = "";

    public string VID { get; set; } = "";
    public string AID { get; set; } = "";
    public string SID { get; set; } = "";

    public bool Border { get; set; } = true;
    public bool FileEnded { get; set; }
    public bool Fullscreen { get; set; }
    public bool IsQuitNeeded { set; get; } = true;
    public bool KeepaspectWindow { get; set; }
    public bool Paused { get; set; }
    public bool SnapWindow { get; set; }
    public bool TaskbarProgress { get; set; } = true;
    public bool TitleBar { get; set; } = true;
    public bool WasInitialSizeSet;
    public bool WindowMaximized { get; set; }
    public bool WindowMinimized { get; set; }

    public int Edition { get; set; }
    public int PlaylistPos { get; set; } = -1;
    public int Screen { get; set; } = -1;
    public int VideoRotate { get; set; }

    public float Autofit { get; set; } = 0.6f;
    public float AutofitSmaller { get; set; } = 0.3f;
    public float AutofitLarger { get; set; } = 0.8f;

    public AutoResetEvent ShutdownAutoResetEvent { get; } = new AutoResetEvent(false);
    public nint MainHandle { get; set; }
    public List<MediaTrack> MediaTracks { get; set; } = new List<MediaTrack>();
    public List<TimeSpan> BluRayTitles { get; } = new List<TimeSpan>();
    public object MediaTracksLock { get; } = new object();
    public Size VideoSize { get; set; }
    public TimeSpan Duration;
    public List<MpvClient> Clients { get; } = new List<MpvClient>();

    List<StringPair>? _audioDevices;

    public event Action? Initialized;
    public event Action? Pause;
    public event Action<int>? PlaylistPosChanged;
    public event Action<Size>? VideoSizeChanged;

    public void Init(IntPtr formHandle, bool processCommandLine)
    {
        App.ApplyShowMenuFix();

        MainHandle = mpv_create();
        Handle = MainHandle;

        var events = Enum.GetValues(typeof(mpv_event_id)).Cast<mpv_event_id>();

        foreach (mpv_event_id i in events)
            mpv_request_event(MainHandle, i, 0);

        mpv_request_log_messages(MainHandle, "no");

        if (formHandle != IntPtr.Zero)
            TaskHelp.Run(MainEventLoop);

        if (MainHandle == IntPtr.Zero)
            throw new Exception("error mpv_create");

        if (App.IsTerminalAttached)
        {
            SetPropertyString("terminal", "yes");
            SetPropertyString("input-terminal", "yes");
        }

        if (formHandle != IntPtr.Zero)
        {
            SetPropertyString("force-window", "yes");
            SetPropertyLong("wid", formHandle.ToInt64());
        }

        SetPropertyInt("osd-duration", 2000);

        SetPropertyBool("input-default-bindings", true);
        SetPropertyBool("input-builtin-bindings", false);

        SetPropertyString("idle", "yes");
        SetPropertyString("screenshot-directory", "~~desktop/");
        SetPropertyString("osd-playing-msg", "${media-title}");
        SetPropertyString("osc", "yes");
        SetPropertyString("config-dir", ConfigFolder);
        SetPropertyString("config", "yes");
        
        UsedInputConfContent = App.InputConf.GetContent();

        if (!string.IsNullOrEmpty(UsedInputConfContent))
            SetPropertyString("input-conf", @"memory://" + UsedInputConfContent);

        if (processCommandLine)
            CommandLine.ProcessCommandLineArgsPreInit();

        if (CommandLine.Contains("config-dir"))
        {
            string configDir = CommandLine.GetValue("config-dir");
            string fullPath = System.IO.Path.GetFullPath(configDir);
            App.InputConf.Path = fullPath.AddSep() + "input.conf";
            string content = App.InputConf.GetContent();

            if (!string.IsNullOrEmpty(content))
                SetPropertyString("input-conf", @"memory://" + content);
        }

        Environment.SetEnvironmentVariable("MPVNET_VERSION", AppInfo.Version.ToString());  // deprecated

        mpv_error err = mpv_initialize(MainHandle);

        if (err < 0)
            throw new Exception("mpv_initialize error" + BR2 + GetError(err) + BR);

        string idle = GetPropertyString("idle");
        App.Exit = idle == "no" || idle == "once";

        Handle = mpv_create_client(MainHandle, "mpvnet");

        if (Handle == IntPtr.Zero)
            throw new Exception("mpv_create_client error");

        mpv_request_log_messages(Handle, "info");

        if (formHandle != IntPtr.Zero)
            TaskHelp.Run(EventLoop);

        // otherwise shutdown is raised before media files are loaded,
        // this means Lua scripts that use idle might not work correctly
        SetPropertyString("idle", "yes");

        SetPropertyString("user-data/frontend/name", "mpv.net");
        SetPropertyString("user-data/frontend/version", AppInfo.Version.ToString());
        SetPropertyString("user-data/frontend/process-path", Environment.ProcessPath!);

        ObservePropertyBool("pause", value => {
            Paused = value;
            Pause?.Invoke();
        });

        VideoRotate = GetPropertyInt("video-rotate");

        ObservePropertyInt("video-rotate", value =>
        {
            if (VideoRotate != value)
            {
                VideoRotate = value;
                UpdateVideoSize("dwidth", "dheight");
            }
        });

        ObservePropertyInt("playlist-pos", value => {
            PlaylistPos = value;
            PlaylistPosChanged?.Invoke(value);

            if (FileEnded && value == -1)
                if (GetPropertyString("keep-open") == "no" && App.Exit)
                    CommandV("quit");
        });

        Initialized?.Invoke();
    }

    public void Destroy()
    {
        mpv_destroy(MainHandle);
        mpv_destroy(Handle);

        foreach (var client in Clients)
            mpv_destroy(client.Handle);
    }

    public void ProcessProperty(string? name, string? value)
    {
        switch (name)
        {
            case "autofit":
                {
                    if (int.TryParse(value?.Trim('%'), out int result))
                        Autofit = result / 100f;
                }
                break;
            case "autofit-smaller":
                {
                    if (int.TryParse(value?.Trim('%'), out int result))
                        AutofitSmaller = result / 100f;
                }
                break;
            case "autofit-larger":
                {
                    if (int.TryParse(value?.Trim('%'), out int result))
                        AutofitLarger = result / 100f;
                }
                break;
            case "border": Border = value == "yes"; break;
            case "fs":
            case "fullscreen": Fullscreen = value == "yes"; break;
            case "gpu-api": GPUAPI = value!; break;
            case "keepaspect-window": KeepaspectWindow = value == "yes"; break;
            case "screen": Screen = Convert.ToInt32(value); break;
            case "snap-window": SnapWindow = value == "yes"; break;
            case "taskbar-progress": TaskbarProgress = value == "yes"; break;
            case "vo": VO = value!; break;
            case "window-maximized": WindowMaximized = value == "yes"; break;
            case "window-minimized": WindowMinimized = value == "yes"; break;
            case "title-bar": TitleBar = value == "yes"; break;
        }

        if (AutofitLarger > 1)
            AutofitLarger = 1;
    }

    string? _configFolder;

    public string ConfigFolder {
        get {
            if (_configFolder == null)
            {
                string? mpvnet_home = Environment.GetEnvironmentVariable("MPVNET_HOME");

                if (Directory.Exists(mpvnet_home))
                    return _configFolder = mpvnet_home.AddSep();

                _configFolder = Folder.Startup + "portable_config";

                if (!Directory.Exists(_configFolder))
                    _configFolder = Folder.AppData + "mpv.net";

                if (!Directory.Exists(_configFolder))
                {
                    try {
                        using Process proc = new Process();
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.FileName = "powershell.exe";
                        proc.StartInfo.Arguments = $@"-Command New-Item -Path '{_configFolder}' -ItemType Directory";
                        proc.Start();
                        proc.WaitForExit();
                    } catch (Exception) {}

                    if (!Directory.Exists(_configFolder))
                        Directory.CreateDirectory(_configFolder);
                }

                _configFolder = _configFolder.AddSep();
            }

            return _configFolder;
        }
    }

    Dictionary<string, string>? _Conf;

    public Dictionary<string, string> Conf {
        get
        {
            if (_Conf != null)
                return _Conf;

            App.ApplyInputDefaultBindingsFix();

            _Conf = new Dictionary<string, string>();

            if (File.Exists(ConfPath))
            {
                foreach (string? it in File.ReadAllLines(ConfPath))
                {
                    string line = it.TrimStart(' ', '-').TrimEnd();

                    if (line.StartsWith("#"))
                        continue;

                    if (!line.Contains('='))
                    {
                        if (Regex.Match(line, "^[\\w-]+$").Success)
                            line += "=yes";
                        else
                            continue;
                    }

                    string key = line[..line.IndexOf("=")].Trim();
                    string value = line[(line.IndexOf("=") + 1)..].Trim();

                    if (value.Contains('#') && !value.StartsWith("#") &&
                        !value.StartsWith("'#") && !value.StartsWith("\"#"))

                        value = value[..value.IndexOf("#")].Trim();

                    _Conf[key] = value;
                }
            }

            foreach (var i in _Conf)
                ProcessProperty(i.Key, i.Value);

            return _Conf;
        }
    }

    void UpdateVideoSize(string w, string h)
    {
        if (string.IsNullOrEmpty(Path))
            return;

        Size size = new Size(GetPropertyInt(w), GetPropertyInt(h));

        if (VideoRotate == 90 || VideoRotate == 270)
            size = new Size(size.Height, size.Width);

        if (size != VideoSize && size != Size.Empty)
        {
            VideoSize = size;
            VideoSizeChanged?.Invoke(size);
        }
    }

    public void MainEventLoop()
    {
        while (true)
            mpv_wait_event(MainHandle, -1);
    }

    protected override void OnShutdown()
    {
        IsQuitNeeded = false;
        base.OnShutdown();
        ShutdownAutoResetEvent.Set();
    }

    protected override void OnLogMessage(mpv_event_log_message data)
    {
        if (data.log_level == mpv_log_level.MPV_LOG_LEVEL_INFO)
        {
            string prefix = ConvertFromUtf8(data.prefix);

            if (prefix == "bd")
                ProcessBluRayLogMessage(ConvertFromUtf8(data.text));
        }

        base.OnLogMessage(data);
    }

    protected override void OnEndFile(mpv_event_end_file data)
    {
        base.OnEndFile(data);
        FileEnded = true;
    }

    protected override void OnVideoReconfig()
    {
        UpdateVideoSize("dwidth", "dheight");
        base.OnVideoReconfig();
    }

    // executed before OnFileLoaded
    protected override void OnStartFile()
    {
        Path = GetPropertyString("path");
        base.OnStartFile();
        TaskHelp.Run(LoadFolder);
    }

    // executed after OnStartFile
    protected override void OnFileLoaded()
    {
        Duration = TimeSpan.FromSeconds(GetPropertyDouble("duration"));

        if (App.StartSize == "video")
            WasInitialSizeSet = false;

        TaskHelp.Run(UpdateTracks);

        base.OnFileLoaded();
    }

    void ProcessBluRayLogMessage(string msg)
    {
        lock (BluRayTitles)
        {
            if (msg.Contains(" 0 duration: "))
                BluRayTitles.Clear();

            if (msg.Contains(" duration: "))
            {
                int start = msg.IndexOf(" duration: ") + 11;
                BluRayTitles.Add(new TimeSpan(
                    msg.Substring(start, 2).ToInt(),
                    msg.Substring(start + 3, 2).ToInt(),
                    msg.Substring(start + 6, 2).ToInt()));
            }
        }
    }

    public void SetBluRayTitle(int id) => LoadFiles(new[] { @"bd://" + id }, false, false);

    public DateTime LastLoad;

    public void LoadFiles(string[]? files, bool loadFolder, bool append)
    {
        if (files == null || files.Length == 0)
            return;

        if ((DateTime.Now - LastLoad).TotalMilliseconds < 1000)
            append = true;

        LastLoad = DateTime.Now;

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];

            if (string.IsNullOrEmpty(file))
                continue;

            if (file.Contains('|'))
                file = file[..file.IndexOf("|")];

            file = ConvertFilePath(file);

            string ext = file.Ext();

            if (OperatingSystem.IsWindows())
            {
                switch (ext)
                {
                    case "avs": LoadAviSynth(); break;
                    case "lnk": file = GetShortcutTarget(file); break;
                }
            }

            if (ext == "iso")
                LoadBluRayISO(file);
            else if(FileTypes.Subtitle.Contains(ext))
                CommandV("sub-add", file);
            else if (!FileTypes.IsMedia(ext) && !file.Contains("://") && Directory.Exists(file) &&
                File.Exists(System.IO.Path.Combine(file, "BDMV\\index.bdmv")))
            {
                Command("stop");
                Thread.Sleep(500);
                SetPropertyString("bluray-device", file);
                CommandV("loadfile", @"bd://");
            }
            else
            {
                if (i == 0 && !append)
                    CommandV("loadfile", file);
                else
                    CommandV("loadfile", file, "append");
            }
        }

        if (string.IsNullOrEmpty(GetPropertyString("path")))
            SetPropertyInt("playlist-pos", 0);
    }

    public static string ConvertFilePath(string path)
    {
        if ((path.Contains(":/") && !path.Contains("://")) || (path.Contains(":\\") && path.Contains('/')))
            path = path.Replace("/", "\\");

        if (!path.Contains(':') && !path.StartsWith("\\\\") && File.Exists(path))
            path = System.IO.Path.GetFullPath(path);

        return path;
    }

    public void LoadBluRayISO(string path)
    {
        Command("stop");
        Thread.Sleep(500);
        SetPropertyString("bluray-device", path);
        LoadFiles(new[] { @"bd://" }, false, false);
    }

    public void LoadDiskFolder(string path)
    {
        Command("stop");
        Thread.Sleep(500);

        if (Directory.Exists(path + "\\BDMV"))
        {
            SetPropertyString("bluray-device", path);
            LoadFiles(new[] { @"bd://" }, false, false);
        }
        else
        {
            SetPropertyString("dvd-device", path);
            LoadFiles(new[] { @"dvd://" }, false, false);
        }
    }

    static readonly object LoadFolderLockObject = new object();

    public void LoadFolder()
    {
        if (!App.AutoLoadFolder)
            return;

        Thread.Sleep(1000);

        lock (LoadFolderLockObject)
        {
            string path = GetPropertyString("path");

            if (!File.Exists(path) || GetPropertyInt("playlist-count") != 1)
                return;

            string dir = Environment.CurrentDirectory;

            if (path.Contains(":/") && !path.Contains("://"))
                path = path.Replace("/", "\\");

            if (path.Contains('\\'))
                dir = System.IO.Path.GetDirectoryName(path)!;

            List<string> files = FileTypes.GetMediaFiles(Directory.GetFiles(dir)).ToList();

            if (OperatingSystem.IsWindows())
                files.Sort(new StringLogicalComparer());

            int index = files.IndexOf(path);
            files.Remove(path);

            foreach (string file in files)
                CommandV("loadfile", file, "append");

            if (index > 0)
                CommandV("playlist-move", "0", (index + 1).ToString());
        }
    }

    bool _wasAviSynthLoaded;

    [SupportedOSPlatform("windows")]
    void LoadAviSynth()
    {
        if (!_wasAviSynthLoaded)
        {
            string? dll = Environment.GetEnvironmentVariable("AviSynthDLL");  // StaxRip sets it in portable mode
            LoadLibrary(File.Exists(dll) ? dll : "AviSynth.dll");
            _wasAviSynthLoaded = true;
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr LoadLibrary(string path);

    [SupportedOSPlatform("windows")]
    public static string GetShortcutTarget(string path)
    {
        Type? t = Type.GetTypeFromProgID("WScript.Shell");
        dynamic? sh = Activator.CreateInstance(t!);
        return sh?.CreateShortcut(path).TargetPath!;
    }

    static string GetLanguage(string id)
    {
        foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            if (ci.ThreeLetterISOLanguageName == id || Convert(ci.ThreeLetterISOLanguageName) == id)
                return ci.EnglishName;

        return id;

        static string Convert(string id2) => id2 switch
        {
            "bng" => "ben",
            "ces" => "cze",
            "deu" => "ger",
            "ell" => "gre",
            "eus" => "baq",
            "fra" => "fre",
            "hye" => "arm",
            "isl" => "ice",
            "kat" => "geo",
            "mya" => "bur",
            "nld" => "dut",
            "sqi" => "alb",
            "zho" => "chi",
            _ => id2,
        };
    }

    static string GetNativeLanguage(string name)
    {
        foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            if (ci.EnglishName == name)
                return ci.NativeName;

        return name;
    }

    public void UpdateTracks()
    {
        string path = GetPropertyString("path");

        if (!path.ToLowerEx().StartsWithEx("bd://"))
            lock (BluRayTitles)
                BluRayTitles.Clear();

        lock (MediaTracksLock)
        {
            if (App.MediaInfo && !path.Contains("://") && !path.Contains(@"\\.\pipe\") && File.Exists(path))
                MediaTracks = GetMediaInfoTracks(path);
            else
                MediaTracks = GetTracks();
        }
    }

    public List<StringPair> AudioDevices {
        get {
            if (_audioDevices != null)
                return _audioDevices;

            _audioDevices = new();
            string json = GetPropertyString("audio-device-list");
            var enumerator = JsonDocument.Parse(json).RootElement.EnumerateArray();

            foreach (var element in enumerator)
            {
                string name = element.GetProperty("name").GetString()!;
                string description = element.GetProperty("description").GetString()!;
                _audioDevices.Add(new StringPair(name, description));
            }

            return _audioDevices;
        }
    }

    public List<Chapter> GetChapters() {
        List<Chapter> chapters = new List<Chapter>();
        int count = GetPropertyInt("chapter-list/count");

        for (int x = 0; x < count; x++)
        {
            string title = GetPropertyString($"chapter-list/{x}/title");
            double time = GetPropertyDouble($"chapter-list/{x}/time");

            if (string.IsNullOrEmpty(title) ||
                (title.Length == 12 && title.Contains(':') && title.Contains('.')))

                title = "Chapter " + (x + 1);

            chapters.Add(new Chapter() { Title = title, Time = time });
        }

        return chapters;
    }

    public void UpdateExternalTracks()
    { 
        int trackListTrackCount = GetPropertyInt("track-list/count");
        int editionCount = GetPropertyInt("edition-list/count");
        int count = MediaTracks.Where(i => i.Type != "g").Count();

        lock (MediaTracksLock)
        {
            if (count != (trackListTrackCount + editionCount))
            {
                MediaTracks = MediaTracks.Where(i => !i.External).ToList();
                MediaTracks.AddRange(GetTracks(false));
            }
        }
    }

    public List<MediaTrack> GetTracks(bool includeInternal = true, bool includeExternal = true)
    {
        List<MediaTrack> tracks = new List<MediaTrack>();

        int trackCount = GetPropertyInt("track-list/count");

        for (int i = 0; i < trackCount; i++)
        {
            bool external = GetPropertyBool($"track-list/{i}/external");

            if ((external && !includeExternal) || (!external && !includeInternal))
                continue;

            string type = GetPropertyString($"track-list/{i}/type");
            string filename = GetPropertyString($"filename/no-ext");
            string title = GetPropertyString($"track-list/{i}/title").Replace(filename, "");

            title = Regex.Replace(title, @"^[\._\-]", "");

            if (type == "video")
            {
                string codec = GetPropertyString($"track-list/{i}/codec").ToUpperEx();
                if (codec == "MPEG2VIDEO")
                    codec = "MPEG2";
                else if (codec == "DVVIDEO")
                    codec = "DV";
                MediaTrack track = new MediaTrack();
                Add(track, codec);
                Add(track, GetPropertyString($"track-list/{i}/demux-w") + "x" + GetPropertyString($"track-list/{i}/demux-h"));
                Add(track, GetPropertyString($"track-list/{i}/demux-fps").Replace(".000000", "") + " FPS");
                Add(track, GetPropertyBool($"track-list/{i}/default") ? "Default" : null);
                track.Text = "V: " + track.Text.Trim(' ', ',');
                track.Type = "v";
                track.ID = GetPropertyInt($"track-list/{i}/id");
                tracks.Add(track);
            }
            else if (type == "audio")
            {
                string codec = GetPropertyString($"track-list/{i}/codec").ToUpperEx();
                if (codec.Contains("PCM"))
                    codec = "PCM";
                MediaTrack track = new MediaTrack();
                Add(track, GetLanguage(GetPropertyString($"track-list/{i}/lang")));
                Add(track, codec);
                Add(track, GetPropertyInt($"track-list/{i}/audio-channels") + " ch");
                Add(track, GetPropertyInt($"track-list/{i}/demux-samplerate") / 1000 + " kHz");
                Add(track, GetPropertyBool($"track-list/{i}/forced") ? "Forced" : null);
                Add(track, GetPropertyBool($"track-list/{i}/default") ? "Default" : null);
                Add(track, GetPropertyBool($"track-list/{i}/external") ? "External" : null);
                Add(track, title);
                track.Text = "A: " + track.Text.Trim(' ', ',');
                track.Type = "a";
                track.ID = GetPropertyInt($"track-list/{i}/id");
                track.External = external;
                tracks.Add(track);
            }
            else if (type == "sub")
            {
                string codec = GetPropertyString($"track-list/{i}/codec").ToUpperEx();
                if (codec.Contains("PGS"))
                    codec = "PGS";
                else if (codec == "SUBRIP")
                    codec = "SRT";
                else if (codec == "WEBVTT")
                    codec = "VTT";
                else if (codec == "DVB_SUBTITLE")
                    codec = "DVB";
                else if (codec == "DVD_SUBTITLE")
                    codec = "VOB";
                MediaTrack track = new MediaTrack();
                Add(track, GetLanguage(GetPropertyString($"track-list/{i}/lang")));
                Add(track, codec);
                Add(track, GetPropertyBool($"track-list/{i}/forced") ? "Forced" : null);
                Add(track, GetPropertyBool($"track-list/{i}/default") ? "Default" : null);
                Add(track, GetPropertyBool($"track-list/{i}/external") ? "External" : null);
                Add(track, title);
                track.Text = "S: " + track.Text.Trim(' ', ',');
                track.Type = "s";
                track.ID = GetPropertyInt($"track-list/{i}/id");
                track.External = external;
                tracks.Add(track);
            }
        }

        if (includeInternal)
        {
            int editionCount = GetPropertyInt("edition-list/count");

            for (int i = 0; i < editionCount; i++)
            {
                string title = GetPropertyString($"edition-list/{i}/title");

                if (string.IsNullOrEmpty(title))
                    title = "Edition " + i;

                MediaTrack track = new MediaTrack
                {
                    Text = "E: " + title,
                    Type = "e",
                    ID = i
                };

                tracks.Add(track);
            }
        }

        return tracks;

        static void Add(MediaTrack track, object? value)
        {
            string str = (value + "").Trim();

            if (str != "" && !track.Text.Contains(str))
                track.Text += " " + str + ",";
        }
    }

    public List<MediaTrack> GetMediaInfoTracks(string path)
    {
        List<MediaTrack> tracks = new List<MediaTrack>();

        using (MediaInfo mi = new MediaInfo(path))
        {
            MediaTrack track = new MediaTrack();
            Add(track, mi.GetGeneral("Format"));
            Add(track, mi.GetGeneral("FileSize/String"));
            Add(track, mi.GetGeneral("Duration/String"));
            Add(track, mi.GetGeneral("OverallBitRate/String"));
            track.Text = "G: " + track.Text.Trim(' ', ',');
            track.Type = "g";
            tracks.Add(track);

            int videoCount = mi.GetCount(MediaInfoStreamKind.Video);

            for (int i = 0; i < videoCount; i++)
            {
                string fps = mi.GetVideo(i, "FrameRate");

                if (float.TryParse(fps, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                    fps = result.ToString(CultureInfo.InvariantCulture);

                track = new MediaTrack();
                Add(track, mi.GetVideo(i, "Format"));
                Add(track, mi.GetVideo(i, "Format_Profile"));
                Add(track, mi.GetVideo(i, "Width") + "x" + mi.GetVideo(i, "Height"));
                Add(track, mi.GetVideo(i, "BitRate/String"));
                Add(track, fps + " FPS");
                Add(track, (videoCount > 1 && mi.GetVideo(i, "Default") == "Yes") ? "Default" : "");
                track.Text = "V: " + track.Text.Trim(' ', ',');
                track.Type = "v";
                track.ID = i + 1;
                tracks.Add(track);
            }

            int audioCount = mi.GetCount(MediaInfoStreamKind.Audio);

            for (int i = 0; i < audioCount; i++)
            {
                string lang = mi.GetAudio(i, "Language/String");
                string nativeLang = GetNativeLanguage(lang);
                string? title = mi.GetAudio(i, "Title");
                string format = mi.GetAudio(i, "Format");

                if (!string.IsNullOrEmpty(title))
                {
                    if (title.ContainsEx("DTS-HD MA"))
                        format = "DTS-MA";

                    if (title.ContainsEx("DTS-HD MA"))
                        title = title.Replace("DTS-HD MA", "");

                    if (title.ContainsEx("Blu-ray"))
                        title = title.Replace("Blu-ray", "");

                    if (title.ContainsEx("UHD "))
                        title = title.Replace("UHD ", "");

                    if (title.ContainsEx("EAC"))
                        title = title.Replace("EAC", "E-AC");

                    if (title.ContainsEx("AC3"))
                        title = title.Replace("AC3", "AC-3");

                    if (title.ContainsEx(lang))
                        title = title.Replace(lang, "").Trim();

                    if (title.ContainsEx(nativeLang))
                        title = title.Replace(nativeLang, "").Trim();

                    if (title.ContainsEx("Surround"))
                        title = title.Replace("Surround", "");

                    if (title.ContainsEx("Dolby Digital"))
                        title = title.Replace("Dolby Digital", "");

                    if (title.ContainsEx("Stereo"))
                        title = title.Replace("Stereo", "");

                    if (title.StartsWithEx(format + " "))
                        title = title.Replace(format + " ", "");

                    foreach (string i2 in new[] { "2.0", "5.1", "6.1", "7.1" })
                        if (title.ContainsEx(i2))
                            title = title.Replace(i2, "").Trim();

                    if (title.ContainsEx("@ "))
                        title = title.Replace("@ ", "");

                    if (title.ContainsEx(" @"))
                        title = title.Replace(" @", "");

                    if (title.ContainsEx("()"))
                        title = title.Replace("()", "");

                    if (title.ContainsEx("[]"))
                        title = title.Replace("[]", "");

                    if (title.TrimEx() == format)
                        title = null;

                    if (!string.IsNullOrEmpty(title))
                        title = title.Trim(" _-".ToCharArray());
                }

                track = new MediaTrack();
                Add(track, lang);
                Add(track, format);
                Add(track, mi.GetAudio(i, "Format_Profile"));
                Add(track, mi.GetAudio(i, "BitRate/String"));
                Add(track, mi.GetAudio(i, "Channel(s)") + " ch");
                Add(track, mi.GetAudio(i, "SamplingRate/String"));
                Add(track, mi.GetAudio(i, "Forced") == "Yes" ? "Forced" : "");
                Add(track, (audioCount > 1 && mi.GetAudio(i, "Default") == "Yes") ? "Default" : "");
                Add(track, title);

                if (track.Text.Contains("MPEG Audio, Layer 2"))
                    track.Text = track.Text.Replace("MPEG Audio, Layer 2", "MP2");

                if (track.Text.Contains("MPEG Audio, Layer 3"))
                    track.Text = track.Text.Replace("MPEG Audio, Layer 2", "MP3");

                track.Text = "A: " + track.Text.Trim(' ', ',');
                track.Type = "a";
                track.ID = i + 1;
                tracks.Add(track);
            }

            int subCount = mi.GetCount(MediaInfoStreamKind.Text);

            for (int i = 0; i < subCount; i++)
            {
                string codec = mi.GetText(i, "Format").ToUpperEx();

                if (codec == "UTF-8")
                    codec = "SRT";
                else if (codec == "WEBVTT")
                    codec = "VTT";
                else if (codec == "VOBSUB")
                    codec = "VOB";

                string lang = mi.GetText(i, "Language/String");
                string nativeLang = GetNativeLanguage(lang);
                string title = mi.GetText(i, "Title");
                bool forced = mi.GetText(i, "Forced") == "Yes";

                if (!string.IsNullOrEmpty(title))
                {
                    if (title.ContainsEx("VobSub"))
                        title = title.Replace("VobSub", "VOB");

                    if (title.ContainsEx(codec))
                        title = title.Replace(codec, "");

                    if (title.ContainsEx(lang.ToLowerEx()))
                        title = title.Replace(lang.ToLowerEx(), lang);

                    if (title.ContainsEx(nativeLang.ToLowerEx()))
                        title = title.Replace(nativeLang.ToLowerEx(), nativeLang).Trim();

                    if (title.ContainsEx(lang))
                        title = title.Replace(lang, "");

                    if (title.ContainsEx(nativeLang))
                        title = title.Replace(nativeLang, "").Trim();

                    if (title.ContainsEx("full"))
                        title = title.Replace("full", "").Trim();

                    if (title.ContainsEx("Full"))
                        title = title.Replace("Full", "").Trim();

                    if (title.ContainsEx("Subtitles"))
                        title = title.Replace("Subtitles", "").Trim();

                    if (title.ContainsEx("forced"))
                        title = title.Replace("forced", "Forced").Trim();

                    if (forced && title.ContainsEx("Forced"))
                        title = title.Replace("Forced", "").Trim();

                    if (title.ContainsEx("()"))
                        title = title.Replace("()", "");

                    if (title.ContainsEx("[]"))
                        title = title.Replace("[]", "");

                    if (!string.IsNullOrEmpty(title))
                        title = title.Trim(" _-".ToCharArray());
                }

                track = new MediaTrack();
                Add(track, lang);
                Add(track, codec);
                Add(track, mi.GetText(i, "Format_Profile"));
                Add(track, forced ? "Forced" : "");
                Add(track, (subCount > 1 && mi.GetText(i, "Default") == "Yes") ? "Default" : "");
                Add(track, title);
                track.Text = "S: " + track.Text.Trim(' ', ',');
                track.Type = "s";
                track.ID = i + 1;
                tracks.Add(track);
            }
        }

        int editionCount = GetPropertyInt("edition-list/count");

        for (int i = 0; i < editionCount; i++)
        {
            string title = GetPropertyString($"edition-list/{i}/title");

            if (string.IsNullOrEmpty(title))
                title = "Edition " + i;

            MediaTrack track = new MediaTrack
            {
                Text = "E: " + title,
                Type = "e",
                ID = i
            };

            tracks.Add(track);
        }

        return tracks;

        static void Add(MediaTrack track, object? value)
        {
            string str = value?.ToStringEx().Trim() ?? "";

            if (str != "" && !(track.Text != null && track.Text.Contains(str)))
                track.Text += " " + str + ",";
        }
    }

    string[]? _profileNames;

    public string[] ProfileNames
    {
        get
        {
            if (_profileNames != null)
                return _profileNames;

            string[] ignore = { "builtin-pseudo-gui", "encoding", "libmpv", "pseudo-gui", "default" };
            string json = GetPropertyString("profile-list");
            return _profileNames = JsonDocument.Parse(json).RootElement.EnumerateArray()
                .Select(it => it.GetProperty("name").GetString())
                .Where(it => !ignore.Contains(it)).ToArray()!;
        }
    }

    public string GetProfiles()
    {
        string json = GetPropertyString("profile-list");
        StringBuilder sb = new StringBuilder();

        foreach (var profile in JsonDocument.Parse(json).RootElement.EnumerateArray())
        {
            sb.Append(profile.GetProperty("name").GetString() + BR2);

            foreach (var it in profile.GetProperty("options").EnumerateArray())
                sb.AppendLine($"    {it.GetProperty("key").GetString()} = {it.GetProperty("value").GetString()}");

            sb.Append(BR);
        }

        return sb.ToString();
    }

    public string GetDecoders()
    {
        var list = JsonDocument.Parse(GetPropertyString("decoder-list")).RootElement.EnumerateArray()
            .Select(it => $"{it.GetProperty("codec").GetString()} - {it.GetProperty("description").GetString()}")
            .OrderBy(it => it);

        return string.Join(BR, list);
    }

    public string GetProtocols() => string.Join(BR, GetPropertyString("protocol-list").Split(',').OrderBy(i => i));

    public string GetDemuxers() => string.Join(BR, GetPropertyString("demuxer-lavf-list").Split(',').OrderBy(i => i));

    public MpvClient CreateNewPlayer(string name)
    {
        var client = new MpvClient { Handle = mpv_create_client(MainHandle, name) };

        if (client.Handle == IntPtr.Zero)
            throw new Exception("Error CreateNewPlayer");

        TaskHelp.Run(client.EventLoop);
        Clients.Add(client);
        return client;
    }
}
