
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinForms = System.Windows.Forms;

using static mpvnet.libmpv;
using static Native;

namespace mpvnet
{
    public delegate void MpvBoolPropChangeHandler(string propName, bool value);

    public class mp
    {
        public static event Action VideoSizeChanged;
                                                              // Lua/JS event       libmpv event

                                                              //                    MPV_EVENT_NONE
        public static event Action Shutdown;                  // shutdown           MPV_EVENT_SHUTDOWN
        public static event Action LogMessage;                // log-message        MPV_EVENT_LOG_MESSAGE
        public static event Action GetPropertyReply;          // get-property-reply MPV_EVENT_GET_PROPERTY_REPLY
        public static event Action SetPropertyReply;          // set-property-reply MPV_EVENT_SET_PROPERTY_REPLY
        public static event Action CommandReply;              // command-reply      MPV_EVENT_COMMAND_REPLY
        public static event Action StartFile;                 // start-file         MPV_EVENT_START_FILE
        public static event Action<EndFileEventMode> EndFile; // end-file           MPV_EVENT_END_FILE
        public static event Action FileLoaded;                // file-loaded        MPV_EVENT_FILE_LOADED
        public static event Action TracksChanged;             //                    MPV_EVENT_TRACKS_CHANGED
        public static event Action TrackSwitched;             //                    MPV_EVENT_TRACK_SWITCHED
        public static event Action Idle;                      // idle               MPV_EVENT_IDLE
        public static event Action Pause;                     //                    MPV_EVENT_PAUSE
        public static event Action Unpause;                   //                    MPV_EVENT_UNPAUSE
        public static event Action Tick;                      // tick               MPV_EVENT_TICK
        public static event Action ScriptInputDispatch;       //                    MPV_EVENT_SCRIPT_INPUT_DISPATCH
        public static event Action<string[]> ClientMessage;   // client-message     MPV_EVENT_CLIENT_MESSAGE
        public static event Action VideoReconfig;             // video-reconfig     MPV_EVENT_VIDEO_RECONFIG
        public static event Action AudioReconfig;             // audio-reconfig     MPV_EVENT_AUDIO_RECONFIG
        public static event Action MetadataUpdate;            //                    MPV_EVENT_METADATA_UPDATE
        public static event Action Seek;                      // seek               MPV_EVENT_SEEK
        public static event Action PlaybackRestart;           // playback-restart   MPV_EVENT_PLAYBACK_RESTART
                                                              //                    MPV_EVENT_PROPERTY_CHANGE
        public static event Action ChapterChange;             //                    MPV_EVENT_CHAPTER_CHANGE
        public static event Action QueueOverflow;             //                    MPV_EVENT_QUEUE_OVERFLOW
        public static event Action Hook;                      //                    MPV_EVENT_HOOK

        public static event Action Initialized;

        public static List<KeyValuePair<string, Action<bool>>> BoolPropChangeActions { get; set; } = new List<KeyValuePair<string, Action<bool>>>();
        public static List<KeyValuePair<string, Action<int>>> IntPropChangeActions { get; set; } = new List<KeyValuePair<string, Action<int>>>();
        public static List<KeyValuePair<string, Action<double>>> DoublePropChangeActions { get; set; } = new List<KeyValuePair<string, Action<double>>>();
        public static List<KeyValuePair<string, Action<string>>> StringPropChangeActions { get; set; } = new List<KeyValuePair<string, Action<string>>>();
        public static List<MediaTrack> MediaTracks { get; set; } = new List<MediaTrack>();
        public static List<KeyValuePair<string, double>> Chapters { get; set; } = new List<KeyValuePair<string, double>>();
        public static IntPtr Handle { get; set; }
        public static IntPtr WindowHandle { get; set; }
        public static List<PythonScript> PythonScripts { get; set; } = new List<PythonScript>();
        public static Size VideoSize { get; set; }
        public static TimeSpan Duration;
        public static AutoResetEvent ShutdownAutoResetEvent { get; set; } = new AutoResetEvent(false);
        public static AutoResetEvent VideoSizeAutoResetEvent { get; set; } = new AutoResetEvent(false);

        public static string InputConfPath { get => ConfigFolder + "input.conf"; }
        public static string ConfPath      { get => ConfigFolder + "mpv.conf"; }
        public static string Sid { get; set; } = "";
        public static string Aid { get; set; } = "";
        public static string Vid { get; set; } = "";
        public static string GPUAPI { get; set; } = "auto";

        public static bool IsLogoVisible { set; get; }
        public static bool IsQuitNeeded { set; get; } = true;
        public static bool Fullscreen { get; set; }
        public static bool Border { get; set; } = true;
        public static bool TaskbarProgress { get; set; } = true;

        public static int Screen { get; set; } = -1;
        public static int Edition { get; set; }

        public static float Autofit { get; set; } = 0.5f;
        public static float AutofitSmaller { get; set; } = 0.4f;
        public static float AutofitLarger { get; set; } = 0.75f;

        public static void Init()
        {
            Handle = mpv_create();
            Task.Run(() => EventLoop());

            if (App.IsStartedFromTerminal)
            {
                set_property_string("terminal", "yes");
                set_property_string("input-terminal", "yes");
                set_property_string("msg-level", "osd/libass=fatal");
            }

            set_property_string("wid", MainForm.Hwnd.ToString());
            set_property_string("osc", "yes");
            set_property_string("input-media-keys", "yes");
            set_property_string("force-window", "yes");
            set_property_string("config-dir", ConfigFolder);
            set_property_string("config", "yes");

            ProcessCommandLine(true);
            mpv_initialize(Handle);
            Initialized?.Invoke();
            LoadMpvScripts();
        }

        public static void ProcessProperty(string name, string value)
        {
            if (name.Any(char.IsUpper))
                Msg.ShowError("Uppercase char detected: " + name,
                    "mpv properties using the command line and the mpv.conf config file are required to be lowercase.");

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
                case "border": Border = value == "yes"; break;
                case "taskbar-progress": TaskbarProgress = value == "yes"; break;
                case "screen": Screen = Convert.ToInt32(value); break;
                case "gpu-api": GPUAPI = value; break;
            }
        }

        static string _ConfigFolder;

        public static string ConfigFolder {
            get {
                if (_ConfigFolder == null)
                {
                    string portableFolder = Folder.Startup + @"portable_config\";
                    _ConfigFolder = portableFolder;

                    if (!Directory.Exists(_ConfigFolder))
                        _ConfigFolder = RegistryHelp.GetString(App.RegPath, "ConfigFolder");

                    if (!Directory.Exists(_ConfigFolder))
                    {
                        string appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\mpv.net\";

                        using (TaskDialog<string> td = new TaskDialog<string>())
                        {
                            td.MainInstruction = "Choose a settings folder.";
                            td.AddCommandLink(@"AppData\Roaming\mpv.net", appdataFolder, appdataFolder);
                            td.AddCommandLink(@"<startup>\portable_config", portableFolder, portableFolder);
                            td.AddCommandLink("Choose custom folder", "custom");
                            _ConfigFolder = td.Show();
                        }

                        if (_ConfigFolder == null)
                        {
                            _ConfigFolder = "";
                            return "";
                        }

                        if (_ConfigFolder == "custom")
                        {
                            using (var d = new WinForms.FolderBrowserDialog())
                            {
                                d.Description = "Choose a folder.";

                                if (d.ShowDialog() == WinForms.DialogResult.OK)
                                    _ConfigFolder = d.SelectedPath + @"\";
                                else
                                    _ConfigFolder = appdataFolder;
                            }
                        }
                    }

                    if (Folder.Startup == _ConfigFolder)
                    {
                        Msg.ShowError("Startup folder and config folder cannot be identical, using portable_config instead.");
                        _ConfigFolder = portableFolder;
                    }

                    if (!Directory.Exists(_ConfigFolder))
                        Directory.CreateDirectory(_ConfigFolder);

                    if (!_ConfigFolder.Contains("portable_config"))
                        RegistryHelp.SetValue(App.RegPath, "ConfigFolder", _ConfigFolder);

                    if (!File.Exists(_ConfigFolder + "input.conf"))
                        File.WriteAllText(_ConfigFolder + "input.conf", Properties.Resources.inputConf);

                    if (!File.Exists(_ConfigFolder + "mpv.conf"))
                        File.WriteAllText(_ConfigFolder + "mpv.conf", Properties.Resources.mpvConf);
                }

                return _ConfigFolder;
            }
        }

        static Dictionary<string, string> _Conf;

        public static Dictionary<string, string> Conf {
            get {
                if (_Conf == null)
                {
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

        public static void LoadMpvScripts()
        {
            if (Directory.Exists(Folder.Startup + "Scripts"))
            {
                string[] startupScripts = Directory.GetFiles(Folder.Startup + "Scripts");

                foreach (string path in startupScripts)
                    if ((path.EndsWith(".lua") || path.EndsWith(".js")) && KnownScripts.Contains(Path.GetFileName(path)))
                        commandv("load-script", $"{path}");
            }
        }

        public static string[] KnownScripts { get; } = { "osc-visibility.js", "show-playlist.js", "seek-show-position.py", "repl.lua" };

        public static void LoadScripts()
        {
            if (Directory.Exists(Folder.Startup + "Scripts"))
            {
                foreach (string scriptPath in Directory.GetFiles(Folder.Startup + "Scripts"))
                {
                    if (KnownScripts.Contains(Path.GetFileName(scriptPath)))
                    {
                        if (scriptPath.EndsWith(".py"))
                            Task.Run(() => PythonScripts.Add(new PythonScript(scriptPath)));
                        else if (scriptPath.EndsWith(".ps1"))
                            Task.Run(() => PowerShellScript.Init(scriptPath));
                    }
                    else
                        Msg.ShowError("Failed to load script", scriptPath + "\n\nOnly scripts that ship with mpv.net are allowed in <startup>\\scripts\n\nUser scripts have to use <config folder>\\scripts\n\nNever copy or install a new mpv.net version over a old mpv.net version.");
                }
            }

            if (Directory.Exists(ConfigFolder + "scripts"))
                foreach (string scriptPath in Directory.GetFiles(ConfigFolder + "scripts"))
                    if (scriptPath.EndsWith(".py"))
                        Task.Run(() => PythonScripts.Add(new PythonScript(scriptPath)));
                    else if (scriptPath.EndsWith(".ps1"))
                        Task.Run(() => PowerShellScript.Init(scriptPath));
        }

        public static void EventLoop()
        {
            while (true)
            {
                IntPtr ptr = mpv_wait_event(Handle, -1);
                mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event));

                if (WindowHandle == IntPtr.Zero)
                    WindowHandle = FindWindowEx(MainForm.Hwnd, IntPtr.Zero, "mpv", null);

                // Debug.WriteLine(evt.event_id.ToString());

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
                            LogMessage?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_GET_PROPERTY_REPLY:
                            GetPropertyReply?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_SET_PROPERTY_REPLY:
                            SetPropertyReply?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_COMMAND_REPLY:
                            CommandReply?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_START_FILE:
                            StartFile?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_END_FILE:
                            var end_fileData = (mpv_event_end_file)Marshal.PtrToStructure(evt.data, typeof(mpv_event_end_file));
                            EndFileEventMode reason = (EndFileEventMode)end_fileData.reason;
                            EndFile?.Invoke(reason);
                            break;
                        case mpv_event_id.MPV_EVENT_FILE_LOADED:
                            HideLogo();
                            Duration = TimeSpan.FromSeconds(get_property_number("duration"));
                            Size vidSize = new Size(get_property_int("width"), get_property_int("height"));
                            if (vidSize.Width == 0 || vidSize.Height == 0)
                                vidSize = new Size(1, 1);
                            if (VideoSize != vidSize)
                            {
                                VideoSize = vidSize;
                                VideoSizeChanged?.Invoke();
                            }
                            VideoSizeAutoResetEvent.Set();
                            Task.Run(new Action(() => ReadMetaData()));
                            string path = mp.get_property_string("path");
                            if (path.Contains("://"))
                                path = mp.get_property_string("media-title");
                            WriteHistory(path);
                            FileLoaded?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_TRACKS_CHANGED:
                            TracksChanged?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_TRACK_SWITCHED:
                            TrackSwitched?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_IDLE:
                            Idle?.Invoke();
                            ShowLogo();
                            break;
                        case mpv_event_id.MPV_EVENT_PAUSE:
                            Pause?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_UNPAUSE:
                            Unpause?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_TICK:
                            Tick?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_SCRIPT_INPUT_DISPATCH:
                            ScriptInputDispatch?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_CLIENT_MESSAGE:
                            var client_messageData = (mpv_event_client_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_client_message));
                            string[] args = NativeUtf8StrArray2ManagedStrArray(client_messageData.args, client_messageData.num_args);
                            if (args.Length > 1 && args[0] == "mpv.net")
                                Command.Execute(args[1], args.Skip(2).ToArray());
                            else if (args.Length > 0)
                                ClientMessage?.Invoke(args);
                            break;
                        case mpv_event_id.MPV_EVENT_VIDEO_RECONFIG:
                            VideoReconfig?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_AUDIO_RECONFIG:
                            AudioReconfig?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_METADATA_UPDATE:
                            MetadataUpdate?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_SEEK:
                            Seek?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_PROPERTY_CHANGE:
                            var propData = (mpv_event_property)Marshal.PtrToStructure(evt.data, typeof(mpv_event_property));

                            if (propData.format == mpv_format.MPV_FORMAT_FLAG)
                            {
                                lock (BoolPropChangeActions)
                                    foreach (var i in BoolPropChangeActions)
                                        if (i.Key== propData.name)
                                            i.Value.Invoke(Marshal.PtrToStructure<int>(propData.data) == 1);
                            }
                            else if (propData.format == mpv_format.MPV_FORMAT_STRING)
                            {
                                lock (StringPropChangeActions)
                                    foreach (var i in StringPropChangeActions)
                                        if (i.Key == propData.name)
                                            i.Value.Invoke(StringFromNativeUtf8(Marshal.PtrToStructure<IntPtr>(propData.data)));
                            }
                            else if(propData.format == mpv_format.MPV_FORMAT_INT64)
                            {
                                lock (IntPropChangeActions)
                                    foreach (var i in IntPropChangeActions)
                                        if (i.Key == propData.name)
                                            i.Value.Invoke(Marshal.PtrToStructure<int>(propData.data));
                            }
                            else if (propData.format == mpv_format.MPV_FORMAT_DOUBLE)
                            {
                                lock (DoublePropChangeActions)
                                    foreach (var i in DoublePropChangeActions)
                                        if (i.Key == propData.name)
                                            i.Value.Invoke(Marshal.PtrToStructure<double>(propData.data));
                            }
                            break;
                        case mpv_event_id.MPV_EVENT_PLAYBACK_RESTART:
                            PlaybackRestart?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_CHAPTER_CHANGE:
                            ChapterChange?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_QUEUE_OVERFLOW:
                            QueueOverflow?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_HOOK:
                            Hook?.Invoke();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Msg.ShowException(ex);
                }
            }
        }

        static void HideLogo()
        {
            command("overlay-remove 0");
            IsLogoVisible = false;
        }

        static List<PythonEventObject> PythonEventObjects = new List<PythonEventObject>();

        public static void register_event(string name, IronPython.Runtime.PythonFunction pyFunc)
        {
            foreach (var eventInfo in typeof(mp).GetEvents())
            {
                if (eventInfo.Name.ToLower() == name.Replace("-", ""))
                {
                    PythonEventObject eventObject = new PythonEventObject();
                    PythonEventObjects.Add(eventObject);
                    eventObject.PythonFunction = pyFunc;
                    MethodInfo mi;

                    if (eventInfo.EventHandlerType == typeof(Action))
                        mi = eventObject.GetType().GetMethod(nameof(PythonEventObject.Invoke));
                    else if (eventInfo.EventHandlerType == typeof(Action<EndFileEventMode>))
                        mi = eventObject.GetType().GetMethod(nameof(PythonEventObject.InvokeEndFileEventMode));
                    else if (eventInfo.EventHandlerType == typeof(Action<string[]>))
                        mi = eventObject.GetType().GetMethod(nameof(PythonEventObject.InvokeStrings));
                    else
                        throw new Exception();

                    eventObject.EventInfo = eventInfo;
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, eventObject, mi);
                    eventObject.Delegate = handler;
                    eventInfo.AddEventHandler(eventObject, handler);
                    break;
                }
            }
        }

        public static void unregister_event(IronPython.Runtime.PythonFunction pyFunc)
        {
            foreach (var eventObjects in PythonEventObjects)
                if (eventObjects.PythonFunction == pyFunc)
                    eventObjects.EventInfo.RemoveEventHandler(eventObjects, eventObjects.Delegate);
        }

        public static void commandv(params string[] args)
        {
            if (Handle == IntPtr.Zero) return;
            IntPtr mainPtr = AllocateUtf8IntPtrArrayWithSentinel(args, out IntPtr[] byteArrayPointers);
            int err = mpv_command(Handle, mainPtr);
            if (err < 0) throw new Exception($"{(mpv_error)err}");
            foreach (var ptr in byteArrayPointers)
                Marshal.FreeHGlobal(ptr);
            Marshal.FreeHGlobal(mainPtr);
        }

        public static void command(string command, bool throwException = false)
        {
            if (Handle == IntPtr.Zero) return;
            int err = mpv_command_string(Handle, command);
            if (err < 0 && throwException) throw new Exception($"{(mpv_error)err}\n\n" + command);
        }

        public static void set_property_string(string name, string value, bool throwOnException = false)
        {
            byte[] bytes = GetUtf8Bytes(value);
            int err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref bytes);
            if (err < 0 && throwOnException) throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static string get_property_string(string name, bool throwOnException = false)
        {
            try
            {
                int err = mpv_get_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, out IntPtr lpBuffer);

                if (err < 0)
                {
                    if (throwOnException) throw new Exception($"{name}: {(mpv_error)err}");
                    return "";
                }

                string ret = StringFromNativeUtf8(lpBuffer);
                mpv_free(lpBuffer);
                return ret;
            }
            catch (Exception e)
            {
                if (throwOnException) throw e;
                return "";
            }
        }

        public static int get_property_int(string name, bool throwOnException = false)
        {
            int err = mpv_get_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);

            if (err < 0)
            {
                if (throwOnException) throw new Exception($"{name}: {(mpv_error)err}");
                return 0;
            }

            return lpBuffer.ToInt32();
        }

        public static double get_property_number(string name, bool throwOnException = false)
        {
            double val = 0;
            int err = mpv_get_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_DOUBLE, ref val);

            if (err < 0)
            {
                if (throwOnException) throw new Exception($"{name}: {(mpv_error)err}");
                return 0;
            }

            return val;
        }

        public static void set_property_int(string name, int value, bool throwOnException = false)
        {
            Int64 val = value;
            int err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);
            if (err < 0 && throwOnException) throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void observe_property_int(string name, Action<int> action)
        {
            int err = mpv_observe_property(Handle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_INT64);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                lock (IntPropChangeActions)
                    IntPropChangeActions.Add(new KeyValuePair<string, Action<int>>(name, action));
        }

        public static void observe_property_double(string name, Action<double> action)
        {
            int err = mpv_observe_property(Handle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_DOUBLE);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                lock (DoublePropChangeActions)
                    DoublePropChangeActions.Add(new KeyValuePair<string, Action<double>>(name, action));
        }

        public static void observe_property_bool(string name, Action<bool> action)
        {
            int err = mpv_observe_property(Handle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_FLAG);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                lock (BoolPropChangeActions)
                    BoolPropChangeActions.Add(new KeyValuePair<string, Action<bool>>(name, action));
        }

        public static void observe_property_string(string name, Action<string> action)
        {
            int err = mpv_observe_property(Handle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_STRING);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                lock (StringPropChangeActions)
                    StringPropChangeActions.Add(new KeyValuePair<string, Action<string>>(name, action));
        }

        public static void ProcessCommandLine(bool preInit)
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            //Msg.Show(string.Join("\n", args));

            string[] preInitProperties = { "input-terminal", "terminal", "input-file", "config", "config-dir", "input-conf", "load-scripts", "scripts", "player-operation-mode" };

            foreach (string i in args)
            {
                string arg = i;

                if (arg.StartsWith("--"))
                {
                    try
                    {
                        if (!arg.Contains("=")) arg += "=yes";

                        string left = arg.Substring(2, arg.IndexOf("=") - 2);
                        string right = arg.Substring(left.Length + 3);

                        if (left == "script") left = "scripts";

                        if (preInit && preInitProperties.Contains(left))
                        {
                            mp.ProcessProperty(left, right);
                            if (!App.ProcessProperty(left, right))
                                set_property_string(left, right, true);
                        }
                        else if (!preInit && !preInitProperties.Contains(left))
                        {
                            if (!PrintCommandLineArgument(arg))
                            {
                                mp.ProcessProperty(left, right);
                                if (!App.ProcessProperty(left, right))
                                    set_property_string(left, right, true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Msg.ShowException(e);
                    }
                }
            }

            if (!preInit)
            {
                List<string> files = new List<string>();

                foreach (string i in args)
                {
                    if (!i.StartsWith("--") && (i == "-" || i.Contains("://") ||
                        i.Contains(":\\") || i.StartsWith("\\\\") || File.Exists(i)))
                    {
                        files.Add(i);
                    }
                }

                Load(files.ToArray(), !App.Queue, Control.ModifierKeys.HasFlag(Keys.Control) || App.Queue);

                if (files.Count == 0 || files[0].Contains("://"))
                {
                    VideoSizeChanged?.Invoke();
                    VideoSizeAutoResetEvent.Set();
                }
            }
        }

        static bool PrintCommandLineArgument(string argument)
        {
            switch (argument)
            {
                case "--list-properties=yes":
                    var list = get_property_string("property-list").Split(',').ToList();
                    list.Sort();
                    Console.WriteLine(string.Join("\r\n", list.ToArray()));
                    return true;
            }

            return false;
        }

        public static DateTime LastLoad;

        public static void Load(string[] files, bool loadFolder, bool append)
        {
            if (files is null || files.Length == 0)
                return;

            HideLogo();

            if ((DateTime.Now - LastLoad).TotalMilliseconds < 1000)
                append = true;

            LastLoad = DateTime.Now;

            for (int i = 0; i < files.Length; i++)
                if (App.SubtitleTypes.Contains(files[i].ShortExt()))
                    commandv("sub-add", files[i]);
                else
                    if (i == 0 && !append)
                        commandv("loadfile", files[i]);
                    else
                        commandv("loadfile", files[i], "append");

            if (string.IsNullOrEmpty(get_property_string("path")))
                set_property_int("playlist-pos", 0);

            if (loadFolder && !append)
                Task.Run(() => LoadFolder());
        }

        public static void LoadFolder()
        {
            if (!App.AutoLoadFolder || Control.ModifierKeys.HasFlag(Keys.Shift))
                return;

            Thread.Sleep(1000);
            string path = get_property_string("path");

            if (!File.Exists(path) || get_property_int("playlist-count") != 1)
                return;

            List<string> files = Directory.GetFiles(Path.GetDirectoryName(path)).ToList();

            files = files.Where(file =>
                App.VideoTypes.Contains(file.ShortExt()) ||
                App.AudioTypes.Contains(file.ShortExt()) ||
                App.ImageTypes.Contains(file.ShortExt())).ToList();

            files.Sort(new StringLogicalComparer());
            int index = files.IndexOf(path);
            files.Remove(path);

            foreach (string i in files)
                commandv("loadfile", i, "append");

            if (index > 0)
                commandv("playlist-move", "0", (index + 1).ToString());
        }

        public static IntPtr AllocateUtf8IntPtrArrayWithSentinel(string[] arr, out IntPtr[] byteArrayPointers)
        {
            int numberOfStrings = arr.Length + 1; // add extra element for extra null pointer last (sentinel)
            byteArrayPointers = new IntPtr[numberOfStrings];
            IntPtr rootPointer = Marshal.AllocCoTaskMem(IntPtr.Size * numberOfStrings);

            for (int index = 0; index < arr.Length; index++)
            {
                var bytes = GetUtf8Bytes(arr[index]);
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);
                byteArrayPointers[index] = unmanagedPointer;
            }

            Marshal.Copy(byteArrayPointers, 0, rootPointer, numberOfStrings);
            return rootPointer;
        }

        public static string[] NativeUtf8StrArray2ManagedStrArray(IntPtr unmanagedStringArray, int StringCount)
        {
            IntPtr[] intPtrArray = new IntPtr[StringCount];
            string[] stringArray = new string[StringCount];
            Marshal.Copy(unmanagedStringArray, intPtrArray, 0, StringCount);

            for (int i = 0; i < StringCount; i++)
                stringArray[i] = StringFromNativeUtf8(intPtrArray[i]);

            return stringArray;
        }

        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            int len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        public static byte[] GetUtf8Bytes(string s) => Encoding.UTF8.GetBytes(s + "\0");

        static string LastHistoryPath;
        static DateTime LastHistoryStartDateTime;

        static void WriteHistory(string path)
        {
            if (!File.Exists(ConfigFolder + "history.txt"))
                return;

            int totalMinutes = Convert.ToInt32((DateTime.Now - LastHistoryStartDateTime).TotalMinutes);

            if (LastHistoryPath != null && totalMinutes > 1)
                File.AppendAllText(ConfigFolder + "history.txt", DateTime.Now.ToString().Substring(0, 16) +
                    " " + totalMinutes.ToString().PadLeft(3) + " " + LastHistoryPath + "\r\n");

            LastHistoryPath = path;
            LastHistoryStartDateTime = DateTime.Now;
        }

        public static void ShowLogo()
        {
            if (MainForm.Instance is null) return;
            Rectangle cr = MainForm.Instance.ClientRectangle;
            int len = cr.Height / 5;
            if (len == 0) return;

            using (Bitmap b = new Bitmap(len, len))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.Clear(Color.Black);
                    Rectangle rect = new Rectangle(0, 0, len, len);
                    g.DrawImage(Properties.Resources.mpvnet, rect);
                    BitmapData bd = b.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
                    int x = Convert.ToInt32((cr.Width - len) / 2.0);
                    int y = Convert.ToInt32(((cr.Height - len) / 2.0) * 0.9);
                    commandv("overlay-add", "0", $"{x}", $"{y}", "&" + bd.Scan0.ToInt64().ToString(), "0", "bgra", bd.Width.ToString(), bd.Height.ToString(), bd.Stride.ToString());
                    b.UnlockBits(bd);
                    IsLogoVisible = true;
                }
            }
        }

        static void ReadMetaData()
        {
            lock (MediaTracks)
            {
                MediaTracks.Clear();
                string path = get_property_string("path");

                if (File.Exists(path))
                {
                    using (MediaInfo mi = new MediaInfo(path))
                    {
                        int count = mi.GetCount(MediaInfoStreamKind.Video);

                        for (int i = 0; i < count; i++)
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

                        count = mi.GetCount(MediaInfoStreamKind.Audio);

                        for (int i = 0; i < count; i++)
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

                        count = mi.GetCount(MediaInfoStreamKind.Text);

                        for (int i = 0; i < count; i++)
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

                        count = get_property_int("edition-list/count");

                        for (int i = 0; i < count; i++)
                        {
                            MediaTrack track = new MediaTrack();
                            track.Text = "E: " + get_property_string($"edition-list/{i}/title");
                            track.Type = "e";
                            track.ID = i;
                            MediaTracks.Add(track);
                        }

                        void Add(MediaTrack track, string val)
                        {
                            if (!string.IsNullOrEmpty(val) && !(track.Text != null && track.Text.Contains(val)))
                                track.Text += " " + val + ",";
                        }
                    }
                }
            }

            lock (Chapters)
            {
                Chapters.Clear();
                int count = get_property_int("chapter-list/count");

                for (int x = 0; x < count; x++)
                {
                    string text = get_property_string($"chapter-list/{x}/title");
                    double time = get_property_number($"chapter-list/{x}/time");
                    Chapters.Add(new KeyValuePair<string, double>(text, time));
                }
            }
        }
    }

    public enum EndFileEventMode
    {
        Eof,
        Stop,
        Quit,
        Error,
        Redirect,
        Unknown
    }
}