
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using static libmpv;
using static mpvnet.Global;

namespace mpvnet
{
    public class CorePlayer
    {
        public static string[] VideoTypes { get; set; } = "264 265 asf avc avi avs dav flv h264 h265 hevc m2t m2ts m2v m4v mkv mov mp4 mpeg mpg mpv mts ts vob vpy webm wmv y4m".Split(' ');
        public static string[] AudioTypes { get; set; } = "aac ac3 dts dtshd dtshr dtsma eac3 flac m4a mka mp2 mp3 mpa mpc ogg opus thd thd+ac3 w64 wav".Split(' ');
        public static string[] ImageTypes { get; set; } = { "jpg", "bmp", "png", "gif" };
        public static string[] SubtitleTypes { get; } = { "srt", "ass", "idx", "sub", "sup", "ttxt", "txt", "ssa", "smi", "mks" };

        public event Action<mpv_log_level, string> LogMessageAsync; // log-message        MPV_EVENT_LOG_MESSAGE
        public event Action<mpv_end_file_reason> EndFileAsync;      // end-file           MPV_EVENT_END_FILE
        public event Action<string[]> ClientMessageAsync;           // client-message     MPV_EVENT_CLIENT_MESSAGE
        public event Action GetPropertyReplyAsync;                  // get-property-reply MPV_EVENT_GET_PROPERTY_REPLY
        public event Action SetPropertyReplyAsync;                  // set-property-reply MPV_EVENT_SET_PROPERTY_REPLY
        public event Action CommandReplyAsync;                      // command-reply      MPV_EVENT_COMMAND_REPLY
        public event Action StartFileAsync;                         // start-file         MPV_EVENT_START_FILE
        public event Action FileLoadedAsync;                        // file-loaded        MPV_EVENT_FILE_LOADED
        public event Action TracksChangedAsync;                     //                    MPV_EVENT_TRACKS_CHANGED
        public event Action TrackSwitchedAsync;                     //                    MPV_EVENT_TRACK_SWITCHED
        public event Action IdleAsync;                              // idle               MPV_EVENT_IDLE
        public event Action PauseAsync;                             //                    MPV_EVENT_PAUSE
        public event Action UnpauseAsync;                           //                    MPV_EVENT_UNPAUSE
        public event Action ScriptInputDispatchAsync;               //                    MPV_EVENT_SCRIPT_INPUT_DISPATCH
        public event Action VideoReconfigAsync;                     // video-reconfig     MPV_EVENT_VIDEO_RECONFIG
        public event Action AudioReconfigAsync;                     // audio-reconfig     MPV_EVENT_AUDIO_RECONFIG
        public event Action MetadataUpdateAsync;                    //                    MPV_EVENT_METADATA_UPDATE
        public event Action SeekAsync;                              // seek               MPV_EVENT_SEEK
        public event Action PlaybackRestartAsync;                   // playback-restart   MPV_EVENT_PLAYBACK_RESTART
        public event Action ChapterChangeAsync;                     //                    MPV_EVENT_CHAPTER_CHANGE

        public event Action<mpv_log_level, string>LogMessage; // log-message        MPV_EVENT_LOG_MESSAGE
        public event Action<mpv_end_file_reason> EndFile;     // end-file           MPV_EVENT_END_FILE
        public event Action<string[]> ClientMessage;          // client-message     MPV_EVENT_CLIENT_MESSAGE
        public event Action Shutdown;                         // shutdown           MPV_EVENT_SHUTDOWN
        public event Action GetPropertyReply;                 // get-property-reply MPV_EVENT_GET_PROPERTY_REPLY
        public event Action SetPropertyReply;                 // set-property-reply MPV_EVENT_SET_PROPERTY_REPLY
        public event Action CommandReply;                     // command-reply      MPV_EVENT_COMMAND_REPLY
        public event Action StartFile;                        // start-file         MPV_EVENT_START_FILE
        public event Action FileLoaded;                       // file-loaded        MPV_EVENT_FILE_LOADED
        public event Action TracksChanged;                    //                    MPV_EVENT_TRACKS_CHANGED
        public event Action TrackSwitched;                    //                    MPV_EVENT_TRACK_SWITCHED
        public event Action Idle;                             // idle               MPV_EVENT_IDLE
        public event Action Pause;                            //                    MPV_EVENT_PAUSE
        public event Action Unpause;                          //                    MPV_EVENT_UNPAUSE
        public event Action ScriptInputDispatch;              //                    MPV_EVENT_SCRIPT_INPUT_DISPATCH
        public event Action VideoReconfig;                    // video-reconfig     MPV_EVENT_VIDEO_RECONFIG
        public event Action AudioReconfig;                    // audio-reconfig     MPV_EVENT_AUDIO_RECONFIG
        public event Action MetadataUpdate;                   //                    MPV_EVENT_METADATA_UPDATE
        public event Action Seek;                             // seek               MPV_EVENT_SEEK
        public event Action PlaybackRestart;                  // playback-restart   MPV_EVENT_PLAYBACK_RESTART
        public event Action ChapterChange;                    //                    MPV_EVENT_CHAPTER_CHANGE

        public event Action Initialized;
        public event Action InitializedAsync;
        public event Action VideoSizeChanged;
        public event Action VideoSizeChangedAsync;
        public event Action<float> ScaleWindow;
        public event Action<float> WindowScale;

        public Dictionary<string, List<Action>>               PropChangeActions { get; set; } = new Dictionary<string, List<Action>>();
        public Dictionary<string, List<Action<int>>>       IntPropChangeActions { get; set; } = new Dictionary<string, List<Action<int>>>();
        public Dictionary<string, List<Action<bool>>>     BoolPropChangeActions { get; set; } = new Dictionary<string, List<Action<bool>>>();
        public Dictionary<string, List<Action<double>>> DoublePropChangeActions { get; set; } = new Dictionary<string, List<Action<double>>>();
        public Dictionary<string, List<Action<string>>> StringPropChangeActions { get; set; } = new Dictionary<string, List<Action<string>>>();
    
        public List<MediaTrack> MediaTracks { get; set; } = new List<MediaTrack>();
        public List<KeyValuePair<string, double>> Chapters { get; set; } = new List<KeyValuePair<string, double>>();
        public List<TimeSpan> BluRayTitles { get; } = new List<TimeSpan>();
        public IntPtr Handle { get; set; }
        public IntPtr WindowHandle { get; set; }
        
        public Size VideoSize { get; set; }
        public TimeSpan Duration;
        
        public AutoResetEvent ShutdownAutoResetEvent  { get; } = new AutoResetEvent(false);
        public AutoResetEvent VideoSizeAutoResetEvent { get; } = new AutoResetEvent(false);

        public string ConfPath { get => ConfigFolder + "mpv.conf"; }
        public string GPUAPI { get; set; } = "auto";
        public string VO { get; set; } = "gpu";
        public string InputConfPath { get => ConfigFolder + "input.conf"; }

        public string VID { get; set; } = "";
        public string AID { get; set; } = "";
        public string SID { get; set; } = "";

        public bool Border { get; set; } = true;
        public bool FileEnded { get; set; }
        public bool Fullscreen { get; set; }
        public bool IsLogoVisible { set; get; } = true;
        public bool IsQuitNeeded { set; get; } = true;
        public bool KeepaspectWindow { get; set; }
        public bool Paused { get; set; }
        public bool TaskbarProgress { get; set; } = true;
        public bool WasInitialSizeSet;
        public bool WindowMaximized { get; set; }
        public bool WindowMinimized { get; set; }

        public int Screen { get; set; } = -1;
        public int Edition { get; set; }
        public int VideoRotate { get; set; }

        public float Autofit { get; set; } = 0.6f;
        public float AutofitSmaller { get; set; } = 0.3f;
        public float AutofitLarger { get; set; } = 0.8f;

        public void Init()
        {
            Handle = mpv_create();

            if (Handle == IntPtr.Zero)
                throw new Exception("error mpv_create");

            mpv_request_log_messages(Handle, "terminal-default");

            App.RunTask(() => EventLoop());

            if (App.IsTerminalAttached)
            {
                SetPropertyString("terminal", "yes");
                SetPropertyString("input-terminal", "yes");
                SetPropertyString("msg-level", "osd/libass=fatal");
            }

            SetPropertyString("watch-later-options", "mute");
            SetPropertyString("screenshot-directory", "~~desktop/");
            SetPropertyString("osd-playing-msg", "${filename}");
            SetPropertyString("wid", MainForm.Hwnd.ToString());
            SetPropertyString("osc", "yes");
            SetPropertyString("force-window", "yes");
            SetPropertyString("config-dir", ConfigFolder);
            SetPropertyString("config", "yes");

            SetPropertyInt("osd-duration", 2000);

            SetPropertyBool("keep-open", true);
            SetPropertyBool("keep-open-pause", false);

            SetPropertyBool("input-default-bindings", true);

            try {
                SetPropertyBool("input-builtin-bindings", false, true);
            } catch {
                SetPropertyBool("input-default-bindings", false);
            }

            ProcessCommandLine(true);
            mpv_error err = mpv_initialize(Handle);

            if (err < 0)
                throw new Exception("mpv_initialize error" + BR2 + GetError(err) + BR);

            ObservePropertyInt("video-rotate", value => {
                VideoRotate = value;
                UpdateVideoSize("dwidth", "dheight");
            });

            Initialized?.Invoke();
            InvokeAsync(InitializedAsync);
        }

        void ApplyCompatibilityFixes()
        {
            if (!App.Settings.InputDefaultBindingsFixApplied)
            {
                if (File.Exists(ConfPath))
                {
                    string content = File.ReadAllText(ConfPath);

                    if (content.Contains("input-default-bindings = no"))
                        File.WriteAllText(ConfPath, content.Replace("input-default-bindings = no", ""));

                    if (content.Contains("input-default-bindings=no"))
                        File.WriteAllText(ConfPath, content.Replace("input-default-bindings=no", ""));
                }

                App.Settings.InputDefaultBindingsFixApplied = true;
            }
        }

        public void ProcessProperty(string name, string value)
        {
            switch (name)
            {
                case "autofit":
                    if (int.TryParse(value.Trim('%'), out int result))
                        Autofit = result / 100f;
                    break;
                case "autofit-smaller":
                    if (int.TryParse(value.Trim('%'), out int result2))
                        AutofitSmaller = result2 / 100f;
                    break;
                case "autofit-larger":
                    if (int.TryParse(value.Trim('%'), out int result3))
                        AutofitLarger = result3 / 100f;
                    break;
                case "fs":
                case "fullscreen": Fullscreen = value == "yes"; break;
                case "border":     Border = value == "yes"; break;
                case "keepaspect-window": KeepaspectWindow = value == "yes"; break;
                case "window-maximized": WindowMaximized = value == "yes"; break;
                case "window-minimized": WindowMinimized = value == "yes"; break;
                case "taskbar-progress": TaskbarProgress = value == "yes"; break;
                case "screen": Screen = Convert.ToInt32(value); break;
                case "gpu-api": GPUAPI = value; break;
                case "vo": VO = value; break;
            }

            if (AutofitLarger > 1)
                AutofitLarger = 1;
        }

        string _ConfigFolder;

        public string ConfigFolder {
            get {
                if (_ConfigFolder == null)
                {
                    _ConfigFolder = Folder.Startup + "portable_config";

                    if (!Directory.Exists(_ConfigFolder))
                        _ConfigFolder = Folder.AppData + "mpv.net";

                    if (!Directory.Exists(_ConfigFolder))
                        Directory.CreateDirectory(_ConfigFolder);

                    _ConfigFolder = _ConfigFolder.AddSep();

                    if (!File.Exists(_ConfigFolder + "input.conf"))
                        File.WriteAllText(_ConfigFolder + "input.conf", Properties.Resources.input_conf);
                }

                return _ConfigFolder;
            }
        }

        Dictionary<string, string> _Conf;

        public Dictionary<string, string> Conf {
            get {
                if (_Conf == null)
                {
                    ApplyCompatibilityFixes();

                    _Conf = new Dictionary<string, string>();

                    if (File.Exists(ConfPath))
                        foreach (var i in File.ReadAllLines(ConfPath))
                            if (i.Contains("=") && !i.TrimStart().StartsWith("#"))
                                _Conf[i.Substring(0, i.IndexOf("=")).Trim()] = i.Substring(i.IndexOf("=") + 1).Trim();

                    foreach (var i in _Conf)
                        ProcessProperty(i.Key, i.Value);
                }

                return _Conf;
            }
        }

        public void LoadScripts()
        {
            if (Directory.Exists(ConfigFolder + "scripts-ps"))
                foreach (string file in Directory.GetFiles(ConfigFolder + "scripts-ps", "*.ps1"))
                    App.RunTask(() => InvokePowerShellScript(file));
        }

        public void InvokePowerShellScript(string file)
        {
            PowerShell ps = new PowerShell();
            ps.Variables.Add(new KeyValuePair<string, object>("core", Core));
            ps.Variables.Add(new KeyValuePair<string, object>("window", MainForm.Instance));
            ps.Scripts.Add("Using namespace mpvnet; [Reflection.Assembly]::LoadWithPartialName('mpvnet')" + BR);

            string eventCode = @"
                $eventJob = Register-ObjectEvent -InputObject $mp -EventName Event -Action {
                    foreach ($pair in $mp.EventHandlers)
                    {
                        if ($pair.Key -eq $args[0])
                        {
                            if ($args.Length -gt 1)
                            {
                                $args2 = $args[1]
                            }

                            Invoke-Command -ScriptBlock $pair.Value -ArgumentList $args2
                        }
                    }
                }

                $mp.RedirectStreams($eventJob)
            ";

            string propertyChangedCode = @"
                $propertyChangedJob = Register-ObjectEvent -InputObject $mp -EventName PropertyChanged -Action {
                    foreach ($pair in $mp.PropChangedHandlers)
                    {
                        if ($pair.Key -eq $args[0])
                        {
                            if ($args.Length -gt 1)
                            {
                                $args2 = $args[1]
                            }

                            Invoke-Command -ScriptBlock $pair.Value -ArgumentList $args2
                        }
                    }
                }

                $mp.RedirectStreams($propertyChangedJob)
            ";

            ps.Scripts.Add(eventCode);
            ps.Scripts.Add(propertyChangedCode);
            ps.Scripts.Add(File.ReadAllText(file));
            ps.Module = Path.GetFileName(file);
            ps.Print = true;

            lock (PowerShell.References)
                PowerShell.References.Add(ps);

            ps.Invoke();
        }

        void UpdateVideoSize(string w, string h)
        {
            Size size = new Size(GetPropertyInt(w), GetPropertyInt(h));

            if (size.Width == 0 || size.Height == 0)
                return;

            if (VideoRotate == 90 || VideoRotate == 270)
                size = new Size(size.Height, size.Width);

            if (VideoSize != size)
            {
                VideoSize = size;
                InvokeEvent(VideoSizeChanged, VideoSizeChangedAsync);
                VideoSizeAutoResetEvent.Set();
            }
        }

        public void EventLoop()
        {
            while (true)
            {
                IntPtr ptr = mpv_wait_event(Handle, -1);
                mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event));

                if (WindowHandle == IntPtr.Zero)
                {
                    WindowHandle = Native.FindWindowEx(MainForm.Hwnd, IntPtr.Zero, "mpv", null);

                    if (WindowHandle != IntPtr.Zero)
                    {
                        int GWL_STYLE = -16;

                        uint WS_CHILD        = 0x40000000;
                        uint WS_CLIPSIBLINGS = 0x04000000;
                        uint WS_DISABLED     = 0x08000000;
                        uint WS_VISIBLE      = 0x10000000;

                        Native.SetWindowLong(WindowHandle, GWL_STYLE,
                            WS_CHILD | WS_VISIBLE | WS_DISABLED | WS_CLIPSIBLINGS);
                    }
                }

                try
                {
                    switch (evt.event_id)
                    {
                        case mpv_event_id.MPV_EVENT_SHUTDOWN:
                            IsQuitNeeded = false;
                            Shutdown?.Invoke();
                            WriteHistory(null);
                            ShutdownAutoResetEvent.Set();
                            return;
                        case mpv_event_id.MPV_EVENT_LOG_MESSAGE:
                            {
                                var data = (mpv_event_log_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_log_message));

                                if (data.log_level == mpv_log_level.MPV_LOG_LEVEL_INFO)
                                {
                                    string prefix = ConvertFromUtf8(data.prefix);

                                    if (prefix == "bd")
                                        ProcessBluRayLogMessage(ConvertFromUtf8(data.text));
                                }

                                if (LogMessage != null || LogMessageAsync != null)
                                {
                                    string msg = $"[{ConvertFromUtf8(data.prefix)}] {ConvertFromUtf8(data.text)}";
                                    InvokeAsync(LogMessageAsync, data.log_level, msg);
                                    LogMessage?.Invoke(data.log_level, msg);
                                }
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_CLIENT_MESSAGE:
                            {
                                var data = (mpv_event_client_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_client_message));
                                string[] args = ConvertFromUtf8Strings(data.args, data.num_args);

                                if (args.Length > 1 && args[0] == "mpv.net")
                                    App.RunTask(() => Commands.Execute(args[1], args.Skip(2).ToArray()));

                                InvokeAsync(ClientMessageAsync, args);
                                ClientMessage?.Invoke(args);
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_VIDEO_RECONFIG:
                            UpdateVideoSize("dwidth", "dheight");
                            InvokeEvent(VideoReconfig, VideoReconfigAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_END_FILE:
                            {
                                var data = (mpv_event_end_file)Marshal.PtrToStructure(evt.data, typeof(mpv_event_end_file));
                                var reason = (mpv_end_file_reason)data.reason;
                                InvokeAsync(EndFileAsync, reason);
                                EndFile?.Invoke(reason);
                                FileEnded = true;
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_FILE_LOADED:
                            {
                                HideLogo();
                                Duration = TimeSpan.FromSeconds(GetPropertyDouble("duration"));

                                if (App.StartSize == "video")
                                    WasInitialSizeSet = false;

                                string path = GetPropertyString("path");

                                if (!VideoTypes.Contains(path.Ext()) || AudioTypes.Contains(path.Ext()))
                                {
                                    UpdateVideoSize("width", "height");
                                    VideoSizeAutoResetEvent.Set();
                                }

                                App.RunTask(new Action(() => ReadMetaData()));

                                App.RunTask(new Action(() => {
                                    if (path.Contains("://"))
                                        path = GetPropertyString("media-title");

                                    WriteHistory(path);
                                }));

                                InvokeEvent(FileLoaded, FileLoadedAsync);
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_PROPERTY_CHANGE:
                            {
                                var data = (mpv_event_property)Marshal.PtrToStructure(evt.data, typeof(mpv_event_property));

                                if (data.format == mpv_format.MPV_FORMAT_FLAG)
                                {
                                    lock (BoolPropChangeActions)
                                        foreach (var pair in BoolPropChangeActions)
                                            if (pair.Key == data.name)
                                            {
                                                bool value = Marshal.PtrToStructure<int>(data.data) == 1;

                                                foreach (var action in pair.Value)
                                                    action.Invoke(value);
                                            }
                                }
                                else if (data.format == mpv_format.MPV_FORMAT_STRING)
                                {
                                    lock (StringPropChangeActions)
                                        foreach (var pair in StringPropChangeActions)
                                            if (pair.Key == data.name)
                                            {
                                                string value = ConvertFromUtf8(Marshal.PtrToStructure<IntPtr>(data.data));

                                                foreach (var action in pair.Value)
                                                    action.Invoke(value);
                                            }
                                }
                                else if (data.format == mpv_format.MPV_FORMAT_INT64)
                                {
                                    lock (IntPropChangeActions)
                                        foreach (var pair in IntPropChangeActions)
                                            if (pair.Key == data.name)
                                            {
                                                int value = Marshal.PtrToStructure<int>(data.data);

                                                foreach (var action in pair.Value)
                                                    action.Invoke(value);
                                            }
                                }
                                else if (data.format == mpv_format.MPV_FORMAT_NONE)
                                {
                                    lock (PropChangeActions)
                                        foreach (var pair in PropChangeActions)
                                            if (pair.Key == data.name)
                                                foreach (var action in pair.Value)
                                                    action.Invoke();
                                }
                                else if (data.format == mpv_format.MPV_FORMAT_DOUBLE)
                                {
                                    lock (DoublePropChangeActions)
                                        foreach (var pair in DoublePropChangeActions)
                                            if (pair.Key == data.name)
                                            {
                                                double value = Marshal.PtrToStructure<double>(data.data);

                                                foreach (var action in pair.Value)
                                                    action.Invoke(value);
                                            }
                                }
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_GET_PROPERTY_REPLY:
                            InvokeEvent(GetPropertyReply, GetPropertyReplyAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_SET_PROPERTY_REPLY:
                            InvokeEvent(SetPropertyReply, SetPropertyReplyAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_COMMAND_REPLY:
                            InvokeEvent(CommandReply, CommandReplyAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_START_FILE:
                            InvokeEvent(StartFile, StartFileAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_TRACKS_CHANGED:
                            InvokeEvent(TracksChanged, TracksChangedAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_TRACK_SWITCHED:
                            InvokeEvent(TrackSwitched, TrackSwitchedAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_IDLE:
                            InvokeEvent(Idle, IdleAsync);

                            if (FileEnded)
                            {
                                ShowLogo();

                                if (GetPropertyString("keep-open") == "no")
                                    Core.CommandV("quit");
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_PAUSE:
                            Paused = true;
                            InvokeEvent(Pause, PauseAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_UNPAUSE:
                            Paused = false;
                            InvokeEvent(Unpause, UnpauseAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_SCRIPT_INPUT_DISPATCH:
                            InvokeEvent(ScriptInputDispatch, ScriptInputDispatchAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_AUDIO_RECONFIG:
                            InvokeEvent(AudioReconfig, AudioReconfigAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_METADATA_UPDATE:
                            InvokeEvent(MetadataUpdate, MetadataUpdateAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_SEEK:
                            InvokeEvent(Seek, SeekAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_PLAYBACK_RESTART:
                            InvokeEvent(PlaybackRestart, PlaybackRestartAsync);
                            break;
                        case mpv_event_id.MPV_EVENT_CHAPTER_CHANGE:
                            InvokeEvent(ChapterChange, ChapterChangeAsync);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowException(ex);
                }
            }
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

        public void SetBluRayTitle(int id)
        {
            LoadFiles(new[] { @"bd://" + id }, false, false);
        }

        void InvokeEvent(Action action, Action asyncAction)
        {
            InvokeAsync(asyncAction);
            action?.Invoke();
        }

        void InvokeAsync(Action action)
        {
            if (action != null)
            {
                foreach (Action a in action.GetInvocationList())
                {
                    var a2 = a;
                    App.RunTask(a2);
                }
            }
        }

        void InvokeAsync<T>(Action<T> action, T t)
        {
            if (action != null)
            {
                foreach (Action<T> a in action.GetInvocationList())
                {
                    var a2 = a;
                    App.RunTask(() => a2.Invoke(t));
                }
            }
        }

        void InvokeAsync<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            if (action != null)
            {
                foreach (Action<T1, T2> a in action.GetInvocationList())
                {
                    var a2 = a;
                    App.RunTask(() => a2.Invoke(t1, t2));
                }
            }
        }

        void HideLogo()
        {
            Command("overlay-remove 0");
            IsLogoVisible = false;
        }

        public void Command(string command, bool throwException = false)
        {
            mpv_error err = mpv_command_string(Handle, command);

            if (err < 0)
                HandleError(err, throwException, "error executing command:", command);
        }

        public void CommandV(params string[] args)
        {
            int count = args.Length + 1;
            IntPtr[] pointers = new IntPtr[count];
            IntPtr rootPtr = Marshal.AllocHGlobal(IntPtr.Size * count);

            for (int index = 0; index < args.Length; index++)
            {
                var bytes = GetUtf8Bytes(args[index]);
                IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                pointers[index] = ptr;
            }

            Marshal.Copy(pointers, 0, rootPtr, count);
            mpv_error err = mpv_command(Handle, rootPtr);

            foreach (IntPtr ptr in pointers)
                Marshal.FreeHGlobal(ptr);

            Marshal.FreeHGlobal(rootPtr);

            if (err < 0)
                HandleError(err, true, "error executing command:", string.Join("\n", args));
        }

        public string Expand(string value)
        {
            if (value == null)
                return "";

            if (!value.Contains("${"))
                return value;

            string[] args = { "expand-text", value };
            int count = args.Length + 1;
            IntPtr[] pointers = new IntPtr[count];
            IntPtr rootPtr = Marshal.AllocHGlobal(IntPtr.Size * count);

            for (int index = 0; index < args.Length; index++)
            {
                var bytes = GetUtf8Bytes(args[index]);
                IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                pointers[index] = ptr;
            }

            Marshal.Copy(pointers, 0, rootPtr, count);
            IntPtr resultNodePtr = Marshal.AllocHGlobal(16);
            mpv_error err = mpv_command_ret(Handle, rootPtr, resultNodePtr);

            foreach (IntPtr ptr in pointers)
                Marshal.FreeHGlobal(ptr);

            Marshal.FreeHGlobal(rootPtr);

            if (err < 0)
            {
                HandleError(err, true, "error executing command:", string.Join("\n", args));
                Marshal.FreeHGlobal(resultNodePtr);
                return "property expansion error";
            }

            mpv_node resultNode = Marshal.PtrToStructure<mpv_node>(resultNodePtr);
            string ret = ConvertFromUtf8(resultNode.str);
            mpv_free_node_contents(resultNodePtr);
            Marshal.FreeHGlobal(resultNodePtr);
            return ret;
        }

        public bool GetPropertyBool(string name, bool throwException = false)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_FLAG, out IntPtr lpBuffer);

            if (err < 0)
                HandleError(err, throwException, $"error getting property: {name}");

            return lpBuffer.ToInt32() != 0;
        }

        public void SetPropertyBool(string name, bool value, bool throwException = false)
        {
            long val = value ? 1 : 0;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_FLAG, ref val);

            if (err < 0)
                HandleError(err, throwException, $"error setting property: {name} = {value}");
        }

        public int GetPropertyInt(string name, bool throwException = false)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);

            if (err < 0)
                HandleError(err, throwException, $"error getting property: {name}");

            return lpBuffer.ToInt32();
        }

        public void SetPropertyInt(string name, int value, bool throwException = false)
        {
            long val = value;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);

            if (err < 0)
                HandleError(err, throwException, $"error setting property: {name} = {value}");
        }

        public long GetPropertyLong(string name, bool throwException = false)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);

            if (err < 0)
                HandleError(err, throwException, $"error getting property: {name}");

            return lpBuffer.ToInt64();
        }

        public void SetPropertyLong(string name, long value, bool throwException = false)
        {
            long val = value;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);

            if (err < 0)
                HandleError(err, throwException, $"error setting property: {name} = {value}");
        }

        public double GetPropertyDouble(string name, bool throwException = false)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_DOUBLE, out double value);

            if (err < 0)
                HandleError(err, throwException, $"error getting property: {name}");

            return value;
        }

        public void SetPropertyDouble(string name, double value, bool throwException = false)
        {
            double val = value;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_DOUBLE, ref val);

            if (err < 0)
                HandleError(err, throwException, $"error setting property: {name} = {value}");
        }

        public string GetPropertyString(string name, bool throwException = false)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_STRING, out IntPtr lpBuffer);

            if (err == 0)
            {
                string ret = ConvertFromUtf8(lpBuffer);
                mpv_free(lpBuffer);
                return ret;
            }

            HandleError(err, throwException, $"error getting property: {name}");
            return "";
        }

        public void SetPropertyString(string name, string value, bool throwException = false)
        {
            byte[] bytes = GetUtf8Bytes(value);
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref bytes);

            if (err < 0)
                HandleError(err, throwException, $"error setting property: {name} = " + value);
        }

        public string GetPropertyOsdString(string name, bool throwException = false)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_OSD_STRING, out IntPtr lpBuffer);

            if (err == 0)
            {
                string ret = ConvertFromUtf8(lpBuffer);
                mpv_free(lpBuffer);
                return ret;
            }

            HandleError(err, throwException, $"error getting property: {name}");
            return "";
        }

        public string GetScriptOption(string name, string defaultValue = "")
        {
            string value = GetPropertyString("script-opts");

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            string[] values = value.Split(',');

            foreach (string item in values)
            {
                if (item.Contains("="))
                {
                    string optionName = item.Substring(0, item.IndexOf("="));

                    if (optionName == name)
                        return item.Substring(item.IndexOf("=") + 1);
                }
            }

            return defaultValue;
        }

        public void ObservePropertyInt(string name, Action<int> action)
        {
            lock (IntPropChangeActions)
            {
                if (!IntPropChangeActions.ContainsKey(name))
                {
                    mpv_error err = mpv_observe_property(Handle, 0, name, mpv_format.MPV_FORMAT_INT64);

                    if (err < 0)
                        HandleError(err, true, $"error observing property: {name}");
                    else
                        IntPropChangeActions[name] = new List<Action<int>>();
                }

                if (IntPropChangeActions.ContainsKey(name))
                    IntPropChangeActions[name].Add(action);
            }
        }

        public void ObservePropertyDouble(string name, Action<double> action)
        {
            lock (DoublePropChangeActions)
            {
                if (!DoublePropChangeActions.ContainsKey(name))
                {
                    mpv_error err = mpv_observe_property(Handle, 0, name, mpv_format.MPV_FORMAT_DOUBLE);

                    if (err < 0)
                        HandleError(err, true, $"error observing property: {name}");
                    else
                        DoublePropChangeActions[name] = new List<Action<double>>();
                }

                if (DoublePropChangeActions.ContainsKey(name))
                    DoublePropChangeActions[name].Add(action);
            }
        }

        public void ObservePropertyBool(string name, Action<bool> action)
        {
            lock (BoolPropChangeActions)
            {
                if (!BoolPropChangeActions.ContainsKey(name))
                {
                    mpv_error err = mpv_observe_property(Handle, 0, name, mpv_format.MPV_FORMAT_FLAG);

                    if (err < 0)
                        HandleError(err, true, $"error observing property: {name}");
                    else
                        BoolPropChangeActions[name] = new List<Action<bool>>();
                }

                if (BoolPropChangeActions.ContainsKey(name))
                    BoolPropChangeActions[name].Add(action);
            }
        }

        public void ObservePropertyString(string name, Action<string> action)
        {
            lock (StringPropChangeActions)
            {
                if (!StringPropChangeActions.ContainsKey(name))
                {
                    mpv_error err = mpv_observe_property(Handle, 0, name, mpv_format.MPV_FORMAT_STRING);

                    if (err < 0)
                        HandleError(err, true, $"error observing property: {name}");
                    else
                        StringPropChangeActions[name] = new List<Action<string>>();
                }

                if (StringPropChangeActions.ContainsKey(name))
                    StringPropChangeActions[name].Add(action);
            }
        }

        public void ObserveProperty(string name, Action action)
        {
            lock (PropChangeActions)
            {
                if (!PropChangeActions.ContainsKey(name))
                {
                    mpv_error err = mpv_observe_property(Handle, 0, name, mpv_format.MPV_FORMAT_NONE);

                    if (err < 0)
                        HandleError(err, true, $"error observing property: {name}");
                    else
                        PropChangeActions[name] = new List<Action>();
                }

                if (PropChangeActions.ContainsKey(name))
                    PropChangeActions[name].Add(action);
            }
        }

        public void HandleError(mpv_error err, bool throwException, params string[] messages)
        {
            if (throwException)
            {
                foreach (string msg in messages)
                    Terminal.WriteError(msg);

                Terminal.WriteError(GetError(err));
                throw new Exception(string.Join(BR2, messages) + BR2 + GetError(err) + BR);
            }
        }

        public void ProcessCommandLine(bool preInit)
        {
            bool shuffle = false;
            var args = Environment.GetCommandLineArgs().Skip(1);

            string[] preInitProperties = { "input-terminal", "terminal", "input-file", "config",
                "config-dir", "input-conf", "load-scripts", "scripts", "player-operation-mode" };

            foreach (string i in args)
            {
                string arg = i;

                if (arg.StartsWith("-") && arg.Length > 1)
                {
                    try
                    {
                        if (!preInit)
                        {
                            if (arg == "--profile=help")
                            {
                                Console.WriteLine(mpvHelp.GetProfiles());
                                continue;
                            }
                            else if (arg == "--vd=help" || arg == "--ad=help")
                            {
                                Console.WriteLine(mpvHelp.GetDecoders());
                                continue;
                            }
                            else if (arg == "--audio-device=help")
                            {
                                Console.WriteLine(GetPropertyOsdString("audio-device-list"));
                                continue;
                            }
                            else if (arg == "--version")
                            {
                                Console.WriteLine(App.Version);
                                continue;
                            }
                            else if (arg == "--input-keylist")
                            {
                                Console.WriteLine(GetPropertyString("input-key-list").Replace(",", BR));
                                continue;
                            }
                            else if (arg.StartsWith("--command="))
                            {
                                Command(arg.Substring(10));
                                continue;
                            }
                        }

                        if (!arg.StartsWith("--"))
                            arg = "-" + arg;

                        if (!arg.Contains("="))
                        {
                            if (arg.Contains("--no-"))
                            {
                                arg = arg.Replace("--no-", "--");
                                arg += "=no";
                            }
                            else
                                arg += "=yes";
                        }

                        string left = arg.Substring(2, arg.IndexOf("=") - 2);
                        string right = arg.Substring(left.Length + 3);

                        if (left == "script")
                            left = "scripts";

                        if (left == "external-file")
                            left = "external-files";

                        if (preInit && preInitProperties.Contains(left))
                        {
                            ProcessProperty(left, right);

                            if (!App.ProcessProperty(left, right))
                                SetPropertyString(left, right, true);
                        }
                        else if (!preInit && !preInitProperties.Contains(left))
                        {
                            ProcessProperty(left, right);

                            if (!App.ProcessProperty(left, right))
                            {
                                SetPropertyString(left, right, true);

                                if (left == "shuffle" && right == "yes")
                                    shuffle = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        App.ShowException(e);
                    }
                }
            }

            if (!preInit)
            {
                List<string> files = new List<string>();

                foreach (string i in args)
                    if (!i.StartsWith("--") && (i == "-" || i.Contains("://") ||
                        i.Contains(":\\") || i.StartsWith("\\\\") || File.Exists(i)))

                        files.Add(i);

                LoadFiles(files.ToArray(), !App.Queue, Control.ModifierKeys.HasFlag(Keys.Control) || App.Queue);

                if (shuffle)
                {
                    Command("playlist-shuffle");
                    SetPropertyInt("playlist-pos", 0);
                }

                if (files.Count == 0 || files[0].Contains("://"))
                {
                    VideoSizeChanged?.Invoke();
                    VideoSizeAutoResetEvent.Set();
                }
            }
        }

        public DateTime LastLoad;

        public void LoadFiles(string[] files, bool loadFolder, bool append)
        {
            if (files is null || files.Length == 0)
                return;

            HideLogo();

            if ((DateTime.Now - LastLoad).TotalMilliseconds < 1000)
                append = true;

            LastLoad = DateTime.Now;

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];

                if (file.Ext() == "avs")
                    LoadAviSynth();

                if (file.Ext() == "iso")
                    LoadISO(file);
                else if(SubtitleTypes.Contains(file.Ext()))
                    CommandV("sub-add", file);
                else if (file.Ext().Length != 3 && File.Exists(Path.Combine(file, "BDMV\\index.bdmv")))
                {
                    Command("stop");
                    Thread.Sleep(500);
                    SetPropertyString("bluray-device", file);
                    CommandV("loadfile", @"bd://");
                }
                else
                {
                    if (i == 0 && !append)
                    {
                        CommandV("loadfile", file);

                        if (App.AutoPlay && Paused)
                            SetPropertyBool("pause", false);
                    }
                    else
                        CommandV("loadfile", file, "append");
                }
            }

            if (string.IsNullOrEmpty(GetPropertyString("path")))
                SetPropertyInt("playlist-pos", 0);

            if (loadFolder && !append)
                App.RunTask(() => LoadFolder());
        }

        public void LoadISO(string path)
        {
            long gb = new FileInfo(path).Length / 1024 / 1024 / 1024;

            if (gb < 10)
            {
                System.Windows.MessageBoxResult result =
                    Msg.ShowQuestion("Click Yes for Blu-ray and No for DVD.",
                    System.Windows.MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case System.Windows.MessageBoxResult.Yes:
                        Command("stop");
                        Thread.Sleep(500);
                        SetPropertyString("bluray-device", path);
                        LoadFiles(new[] { @"bd://" }, false, false);
                        break;
                    case System.Windows.MessageBoxResult.No:
                        Command("stop");
                        Thread.Sleep(500);
                        SetPropertyString("dvd-device", path);
                        LoadFiles(new[] { @"dvd://" }, false, false);
                        break;
                }
            }
            else
            {
                Command("stop");
                Thread.Sleep(500);
                SetPropertyString("bluray-device", path);
                LoadFiles(new[] { @"bd://" }, false, false);
            }
        }

        public void LoadDiskFolder(string path)
        {
            Core.Command("stop");
            Thread.Sleep(500);

            if (Directory.Exists(path + "\\BDMV"))
            {
                Core.SetPropertyString("bluray-device", path);
                Core.LoadFiles(new[] { @"bd://" }, false, false);
            }
            else
            {
                Core.SetPropertyString("dvd-device", path);
                Core.LoadFiles(new[] { @"dvd://" }, false, false);
            }
        }

        public void LoadFolder()
        {
            if (!App.AutoLoadFolder || Control.ModifierKeys.HasFlag(Keys.Shift))
                return;

            Thread.Sleep(1000);
            string path = GetPropertyString("path");

            if (!File.Exists(path) || GetPropertyInt("playlist-count") != 1)
                return;

            string dir = Environment.CurrentDirectory;

            if (path.Contains(Path.DirectorySeparatorChar))
                dir = Path.GetDirectoryName(path);

            List<string> files = Directory.GetFiles(dir).ToList();

            files = files.Where(file =>
                VideoTypes.Contains(file.Ext()) ||
                AudioTypes.Contains(file.Ext()) ||
                ImageTypes.Contains(file.Ext())).ToList();

            files.Sort(new StringLogicalComparer());
            int index = files.IndexOf(path);
            files.Remove(path);

            foreach (string i in files)
                CommandV("loadfile", i, "append");

            if (index > 0)
                CommandV("playlist-move", "0", (index + 1).ToString());
        }

        bool WasAviSynthLoaded;

        void LoadAviSynth()
        {
            if (!WasAviSynthLoaded)
            {
                string dll = Environment.GetEnvironmentVariable("AviSynthDLL");

                if (File.Exists(dll))
                    Native.LoadLibrary(dll);
                else
                    Native.LoadLibrary("AviSynth.dll");

                WasAviSynthLoaded = true;
            }
        }

        string LastHistoryPath;
        DateTime LastHistoryStartDateTime;

        void WriteHistory(string path)
        {
            if (!File.Exists(ConfigFolder + "history.txt"))
                return;

            double totalMinutes = (DateTime.Now - LastHistoryStartDateTime).TotalMinutes;

            if (LastHistoryPath != null && totalMinutes > 1 && !HistoryDiscard())
                File.AppendAllText(ConfigFolder + "history.txt", DateTime.Now.ToString().Substring(0, 16) +
                    " " + Convert.ToInt32(totalMinutes).ToString().PadLeft(3) + " " + LastHistoryPath + "\r\n");

            LastHistoryPath = path;
            LastHistoryStartDateTime = DateTime.Now;
        }

        string HistoryDiscardOption;

        bool HistoryDiscard()
        {
            if (HistoryDiscardOption == null)
                HistoryDiscardOption = GetScriptOption("history-discard");

            if (string.IsNullOrEmpty(HistoryDiscardOption))
                return false;

            foreach (string i in HistoryDiscardOption.Split(';'))
                if (LastHistoryPath.Contains(i))
                    return true;

            return false;
        }

        public void ShowLogo()
        {
            if (MainForm.Instance is null)
                return;

            bool december = DateTime.Now.Month == 12;

            Rectangle cr = MainForm.Instance.ClientRectangle;
            int len = Convert.ToInt32(cr.Height / (december ? 4.5 : 5));

            if (len == 0)
                return;

            using (Bitmap bmp = new Bitmap(len, len))
            {
                using (Graphics gx = Graphics.FromImage(bmp))
                {
                    gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gx.Clear(Color.Black);
                    Rectangle rect = new Rectangle(0, 0, len, len);
                    Bitmap bmp2 = december ? Properties.Resources.mpvnet_santa : Properties.Resources.mpvnet;
                    gx.DrawImage(bmp2, rect);
                    BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
                    int x = Convert.ToInt32((cr.Width - len) / (december ? 1.95 : 2));
                    int y = Convert.ToInt32((cr.Height - len) / 2.0 * (december ? 0.85 : 0.9));
                    CommandV("overlay-add", "0", $"{x}", $"{y}", "&" + bd.Scan0.ToInt64().ToString(), "0", "bgra", bd.Width.ToString(), bd.Height.ToString(), bd.Stride.ToString());
                    bmp.UnlockBits(bd);
                    IsLogoVisible = true;
                }
            }
        }

        string GetLanguage(string id)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                if (ci.ThreeLetterISOLanguageName == id)
                    return ci.EnglishName;

            return id;
        }

        public void RaiseScaleWindow(float value) => ScaleWindow(value);

        public void RaiseWindowScale(float value) => WindowScale(value);

        void ReadMetaData()
        {
            string path = GetPropertyString("path");

            if (!path.ToLowerEx().StartsWithEx("bd://"))
                lock (BluRayTitles)
                    BluRayTitles.Clear();

            lock (MediaTracks)
            {
                MediaTracks.Clear();
                int trackListCount = GetPropertyInt("track-list/count");

                if (path.ToLowerEx().Contains("://"))
                {
                    for (int i = 0; i < trackListCount; i++)
                    {
                        string type = GetPropertyString($"track-list/{i}/type");

                        if (type == "audio")
                        {
                            MediaTrack track = new MediaTrack();
                            Add(track, GetLanguage(GetPropertyString($"track-list/{i}/lang")));
                            Add(track, GetPropertyString($"track-list/{i}/codec").ToUpperEx());
                            Add(track, GetPropertyInt($"track-list/{i}/audio-channels") + " channels");
                            track.Text = "A: " + track.Text.Trim(' ', ',');
                            track.Type = "a";
                            track.ID = GetPropertyInt($"track-list/{i}/id");
                            MediaTracks.Add(track);
                        }
                        else if (type == "sub")
                        {
                            MediaTrack track = new MediaTrack();
                            Add(track, GetLanguage(GetPropertyString($"track-list/{i}/lang")));
                            track.Text = "S: " + track.Text.Trim(' ', ',');
                            track.Type = "s";
                            track.ID = GetPropertyInt($"track-list/{i}/id");
                            MediaTracks.Add(track);
                        }
                    }
                }
                else if (File.Exists(path) && !path.Contains(@"\\.\pipe\"))
                {
                    using (MediaInfo mi = new MediaInfo(path))
                    {
                        int videoCount = mi.GetCount(MediaInfoStreamKind.Video);

                        for (int i = 0; i < videoCount; i++)
                        {
                            MediaTrack track = new MediaTrack();
                            Add(track, mi.GetVideo(i, "Format"));
                            Add(track, mi.GetVideo(i, "Format_Profile"));
                            Add(track, mi.GetVideo(i, "Width") + "x" + mi.GetVideo(i, "Height"));
                            Add(track, mi.GetVideo(i, "FrameRate") + " FPS");
                            Add(track, mi.GetVideo(i, "Language/String"));
                            Add(track, mi.GetVideo(i, "Forced") == "Yes" ? "Forced" : "");
                            Add(track, mi.GetVideo(i, "Default") == "Yes" ? "Default" : "");
                            Add(track, mi.GetVideo(i, "Title"));
                            track.Text = "V: " + track.Text.Trim(' ', ',');
                            track.Type = "v";
                            track.ID = i + 1;
                            MediaTracks.Add(track);
                        }

                        int audioCount = mi.GetCount(MediaInfoStreamKind.Audio);

                        for (int i = 0; i < audioCount; i++)
                        {
                            MediaTrack track = new MediaTrack();
                            Add(track, mi.GetAudio(i, "Language/String"));
                            Add(track, mi.GetAudio(i, "Format"));
                            Add(track, mi.GetAudio(i, "Format_Profile"));
                            Add(track, mi.GetAudio(i, "BitRate/String"));
                            Add(track, mi.GetAudio(i, "Channel(s)/String"));
                            Add(track, mi.GetAudio(i, "SamplingRate/String"));
                            Add(track, mi.GetAudio(i, "Forced") == "Yes" ? "Forced" : "");
                            Add(track, mi.GetAudio(i, "Default") == "Yes" ? "Default" : "");
                            Add(track, mi.GetAudio(i, "Title"));
                            track.Text = "A: " + track.Text.Trim(' ', ',');
                            track.Type = "a";
                            track.ID = i + 1;
                            MediaTracks.Add(track);
                        }

                        for (int i = 0; i < trackListCount; i++)
                        {
                            string type     = GetPropertyString($"track-list/{i}/type");
                            string external = GetPropertyString($"track-list/{i}/external");

                            if (type == "audio" && external == "yes")
                            {
                                MediaTrack track = new MediaTrack();
                                Add(track, GetLanguage(GetPropertyString($"track-list/{i}/lang")));
                                Add(track, GetPropertyString($"track-list/{i}/codec").ToUpperEx());
                                Add(track, GetPropertyInt($"track-list/{i}/audio-channels") + " channels");
                                track.Text = "A: " + (track.Text.Trim(' ', ',') + ", External").Trim(' ', ',');
                                track.Type = "a";
                                track.ID = GetPropertyInt($"track-list/{i}/id");
                                MediaTracks.Add(track);
                            }
                        }

                        int subCount = mi.GetCount(MediaInfoStreamKind.Text);

                        for (int i = 0; i < subCount; i++)
                        {
                            MediaTrack track = new MediaTrack();
                            Add(track, mi.GetText(i, "Language/String"));
                            Add(track, mi.GetText(i, "Format"));
                            Add(track, mi.GetText(i, "Format_Profile"));
                            Add(track, mi.GetText(i, "Forced") == "Yes" ? "Forced" : "");
                            Add(track, mi.GetText(i, "Default") == "Yes" ? "Default" : "");
                            Add(track, mi.GetText(i, "Title"));
                            track.Text = "S: " + track.Text.Trim(' ', ',');
                            track.Type = "s";
                            track.ID = i + 1;
                            MediaTracks.Add(track);
                        }

                        for (int i = 0; i < trackListCount; i++)
                        {
                            string type     = GetPropertyString($"track-list/{i}/type");
                            string external = GetPropertyString($"track-list/{i}/external");

                            if (type == "sub" && external == "yes")
                            {
                                MediaTrack track = new MediaTrack();
                                Add(track, GetLanguage(GetPropertyString($"track-list/{i}/lang")));
                                track.Text = "S: " + (track.Text.Trim(' ', ',') + ", External").Trim(' ', ',');
                                track.Type = "s";
                                track.ID = GetPropertyInt($"track-list/{i}/id");
                                MediaTracks.Add(track);
                            }
                        }

                        int editionCount = GetPropertyInt("edition-list/count");

                        for (int i = 0; i < editionCount; i++)
                        {
                            MediaTrack track = new MediaTrack();
                            track.Text = "E: " + GetPropertyString($"edition-list/{i}/title");
                            track.Type = "e";
                            track.ID = i;
                            MediaTracks.Add(track);
                        }
                    }
                }
            }

            lock (Chapters)
            {
                Chapters.Clear();
                int count = GetPropertyInt("chapter-list/count");

                for (int x = 0; x < count; x++)
                {
                    string text = GetPropertyString($"chapter-list/{x}/title");
                    double time = GetPropertyDouble($"chapter-list/{x}/time");
                    Chapters.Add(new KeyValuePair<string, double>(text, time));
                }
            }

            void Add(MediaTrack track, object value)
            {
                if (value != null && !(track.Text != null && track.Text.Contains(value.ToString())))
                    track.Text += " " + value + ",";
            }
        }
    }
}
