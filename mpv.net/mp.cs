using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sys;

using static mpvnet.libmpv;
using static mpvnet.Native;

using PyRT = IronPython.Runtime;

namespace mpvnet
{
    public delegate void MpvBoolPropChangeHandler(string propName, bool value);

    public class mp
    {
        public static event Action VideoSizeChanged;
                                                              // Lua/JS evens       libmpv events

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

        public static IntPtr MpvHandle { get; set; }
        public static IntPtr MpvWindowHandle { get; set; }
        public static Addon Addon { get; set; }
        public static List<KeyValuePair<string, Action<bool>>> BoolPropChangeActions { get; set; } = new List<KeyValuePair<string, Action<bool>>>();
        public static List<KeyValuePair<string, Action<int>>> IntPropChangeActions { get; set; } = new List<KeyValuePair<string, Action<int>>>();
        public static List<KeyValuePair<string, Action<string>>> StringPropChangeActions { get; set; } = new List<KeyValuePair<string, Action<string>>>();
        public static Size VideoSize { get; set; } = new Size(1920, 1080);
        public static List<PythonScript> PythonScripts { get; set; } = new List<PythonScript>();
        public static AutoResetEvent AutoResetEvent { get; set; } = new AutoResetEvent(false);
        public static List<MediaTrack> MediaTracks { get; set; } = new List<MediaTrack>();
        public static List<KeyValuePair<string, double>> Chapters { get; set; } = new List<KeyValuePair<string, double>>();

        public static string InputConfPath  { get; } = MpvConfFolder + "\\input.conf";
        public static string MpvConfPath    { get; } = MpvConfFolder + "\\mpv.conf";
        public static string MpvNetConfPath { get; } = MpvConfFolder + "\\mpvnet.conf";

        static string _MpvConfFolder;

        public static string MpvConfFolder {
            get {
                if (_MpvConfFolder == null)
                {
                    string portableFolder = Application.StartupPath + "\\portable_config\\";
                    string appdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\";

                    if (!Directory.Exists(appdataFolder) && !Directory.Exists(portableFolder) &&
                        Sys.IsDirectoryWritable(Application.StartupPath))
                    {
                        using (TaskDialog<string> td = new TaskDialog<string>())
                        {
                            td.MainInstruction = "Choose a settings folder.";
                            td.Content = "[MPV documentation about files on Windows.](https://mpv.io/manual/master/#files-on-windows)";
                            td.AddCommandLink("appdata", appdataFolder, appdataFolder);
                            td.AddCommandLink("portable", portableFolder, portableFolder);
                            td.AllowCancel = false;
                            _MpvConfFolder = td.Show();
                        }
                    }
                    else
                        if (Directory.Exists(portableFolder))
                            _MpvConfFolder = portableFolder;
                        else
                            _MpvConfFolder = appdataFolder;

                    if (string.IsNullOrEmpty(_MpvConfFolder)) _MpvConfFolder = appdataFolder;
                    if (!Directory.Exists(_MpvConfFolder)) Directory.CreateDirectory(_MpvConfFolder);

                    if (!File.Exists(_MpvConfFolder + "\\input.conf"))
                        File.WriteAllText(_MpvConfFolder + "\\input.conf", Properties.Resources.inputConf);

                    if (!File.Exists(_MpvConfFolder + "\\mpv.conf"))
                        File.WriteAllText(_MpvConfFolder + "\\mpv.conf", Properties.Resources.mpvConf);
                }
                return _MpvConfFolder;
            }
        }

        static Dictionary<string, string> _mpvConf;

        public static Dictionary<string, string> mpvConf {
            get {
                if (_mpvConf == null)
                {
                    _mpvConf = new Dictionary<string, string>();

                    if (File.Exists(MpvConfPath))
                        foreach (var i in File.ReadAllLines(MpvConfPath))
                            if (i.Contains("=") && ! i.StartsWith("#"))
                                _mpvConf[i.Substring(0, i.IndexOf("=")).Trim()] = i.Substring(i.IndexOf("=") + 1).Trim();
                }
                return _mpvConf;
            }
        }

        static Dictionary<string, string> _mpvNetConf;

        public static Dictionary<string, string> mpvNetConf {
            get {
                if (_mpvNetConf == null)
                {
                    _mpvNetConf = new Dictionary<string, string>();

                    if (File.Exists(MpvNetConfPath))
                        foreach (string i in File.ReadAllLines(MpvNetConfPath))
                            if (i.Contains("=") && !i.StartsWith("#"))
                                _mpvNetConf[i.Substring(0, i.IndexOf("=")).Trim()] = i.Substring(i.IndexOf("=") + 1).Trim();
                }
                return _mpvNetConf;
            }
        }

        public static void Init()
        {
            string dummy = MpvConfFolder;
            LoadLibrary("mpv-1.dll");
            MpvHandle = mpv_create();
            set_property_string("input-default-bindings", "yes");
            set_property_string("osc", "yes");
            set_property_string("config", "yes");
            set_property_string("wid", MainForm.Hwnd.ToString());
            set_property_string("force-window", "yes");
            set_property_string("input-media-keys", "yes");
            mpv_initialize(MpvHandle);
            ProcessCommandLine();
            Task.Run(() => { LoadScripts(); });
            Task.Run(() => { Addon = new Addon(); });
            Task.Run(() => { EventLoop(); });
        }

        public static void LoadScripts()
        {
            string[] jsLua = { ".lua", ".js" };
            string[] startupScripts = Directory.GetFiles(Application.StartupPath + "\\Scripts");

            foreach (var scriptPath in startupScripts)
                if (jsLua.Contains(Path.GetExtension(scriptPath).ToLower()))
                    mp.commandv("load-script", $"{scriptPath}");

            foreach (var scriptPath in startupScripts)
                if (Path.GetExtension(scriptPath) == ".py")
                    PythonScripts.Add(new PythonScript(File.ReadAllText(scriptPath)));

            foreach (var scriptPath in startupScripts)
                if (Path.GetExtension(scriptPath) == ".ps1")
                    PowerShellScript.Init(scriptPath);

            if (Directory.Exists(mp.MpvConfFolder + "Scripts"))
                foreach (var scriptPath in Directory.GetFiles(mp.MpvConfFolder + "Scripts"))
                    if (Path.GetExtension(scriptPath) == ".py")
                        PythonScripts.Add(new PythonScript(File.ReadAllText(scriptPath)));
                    else if (Path.GetExtension(scriptPath) == ".ps1")
                        PowerShellScript.Init(scriptPath);
        }

        public static void EventLoop()
        {
            while (true)
            {
                IntPtr ptr = mpv_wait_event(MpvHandle, -1);
                mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event));

                if (MpvWindowHandle == IntPtr.Zero)
                    MpvWindowHandle = FindWindowEx(MainForm.Hwnd, IntPtr.Zero, "mpv", null);

                //Debug.WriteLine(evt.event_id.ToString());

                try
                {
                    switch (evt.event_id)
                    {
                        case mpv_event_id.MPV_EVENT_SHUTDOWN:
                            Shutdown?.Invoke();
                            WriteHistory(null);
                            AutoResetEvent.Set();
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
                            EndFile?.Invoke((EndFileEventMode)end_fileData.reason);
                            break;
                        case mpv_event_id.MPV_EVENT_FILE_LOADED:
                            FileLoaded?.Invoke();
                            LoadFolder();
                            WriteHistory(mp.get_property_string("path"));
                            break;
                        case mpv_event_id.MPV_EVENT_TRACKS_CHANGED:
                            TracksChanged?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_TRACK_SWITCHED:
                            TrackSwitched?.Invoke();
                            break;
                        case mpv_event_id.MPV_EVENT_IDLE:
                            Idle?.Invoke();
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

                            if (args != null && args.Length > 1 && args[0] == "mpv.net")
                            {
                                bool found = false;

                                foreach (var i in mpvnet.Command.Commands)
                                {
                                    if (args[1] == i.Name)
                                    {
                                        found = true;
                                        i.Action.Invoke(args.Skip(2).ToArray());
                                        MainForm.Instance.BeginInvoke(new Action(() => {
                                            Message m = new Message() { Msg = 0x0202 }; // WM_LBUTTONUP
                                            Native.SendMessage(MainForm.Instance.Handle, m.Msg, m.WParam, m.LParam);
                                        }));
                                    }
                                }
                                if (!found)
                                {
                                    List<string> names = mpvnet.Command.Commands.Select((item) => item.Name).ToList();
                                    names.Sort();
                                    Msg.ShowError($"No command '{args[1]}' found.", $"Available commands are:\n\n{string.Join("\n", names)}\n\nHow to bind these commands can be seen in the [default input bindings and menu definition](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt).");
                                }
                            }
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
                                foreach (var i in BoolPropChangeActions)
                                    if (i.Key== propData.name)
                                        i.Value.Invoke(Marshal.PtrToStructure<int>(propData.data) == 1);

                            if (propData.format == mpv_format.MPV_FORMAT_STRING)
                                foreach (var i in StringPropChangeActions)
                                    if (i.Key == propData.name)
                                        i.Value.Invoke(StringFromNativeUtf8(Marshal.PtrToStructure<IntPtr>(propData.data)));

                            if (propData.format == mpv_format.MPV_FORMAT_INT64)
                                foreach (var i in IntPropChangeActions)
                                    if (i.Key == propData.name)
                                        i.Value.Invoke(Marshal.PtrToStructure<int>(propData.data));
                            break;
                        case mpv_event_id.MPV_EVENT_PLAYBACK_RESTART:
                            PlaybackRestart?.Invoke();
                            Size vidSize = new Size(get_property_int("dwidth"), get_property_int("dheight"));

                            if (VideoSize != vidSize && vidSize != Size.Empty)
                            {
                                VideoSize = vidSize;
                                VideoSizeChanged?.Invoke();
                            }                    

                            Task.Run(new Action(() => ReadMetaData()));
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

        static List<PythonEventObject> PythonEventObjects = new List<PythonEventObject>();

        public static void register_event(string name, PyRT.PythonFunction pyFunc)
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
                    {
                        mi = eventObject.GetType().GetMethod(nameof(PythonEventObject.Invoke));
                    }
                    else if (eventInfo.EventHandlerType == typeof(Action<EndFileEventMode>))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(PythonEventObject.InvokeEndFileEventMode));
                    }
                    else if (eventInfo.EventHandlerType == typeof(Action<string[]>))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(PythonEventObject.InvokeStrings));
                    }
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

        public static void unregister_event(PyRT.PythonFunction pyFunc)
        {
            foreach (var eventObjects in PythonEventObjects)
                if (eventObjects.PythonFunction == pyFunc)
                    eventObjects.EventInfo.RemoveEventHandler(eventObjects, eventObjects.Delegate);
        }

        public static void commandv(params string[] args)
        {
            if (MpvHandle == IntPtr.Zero)
                return;

            IntPtr mainPtr = AllocateUtf8IntPtrArrayWithSentinel(args, out IntPtr[] byteArrayPointers);
            int err = mpv_command(MpvHandle, mainPtr);

            if (err < 0)
                throw new Exception($"{(mpv_error)err}");

            foreach (var ptr in byteArrayPointers)
                Marshal.FreeHGlobal(ptr);

            Marshal.FreeHGlobal(mainPtr);
        }

        public static void command_string(string command, bool throwException = false)
        {
            if (MpvHandle == IntPtr.Zero)
                return;

            int err = mpv_command_string(MpvHandle, command);

            if (err < 0 && throwException)
                throw new Exception($"{(mpv_error)err}\r\n\r\n" + command);
        }

        public static void set_property_string(string name, string value, bool throwOnException = false)
        {
            byte[] bytes = GetUtf8Bytes(value);
            int err = mpv_set_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref bytes);

            if (err < 0 && throwOnException)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static string get_property_string(string name, bool throwOnException = false)
        {
            try
            {
                int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, out IntPtr lpBuffer);

                if (err < 0 && throwOnException)
                    throw new Exception($"{name}: {(mpv_error)err}");

                string ret = StringFromNativeUtf8(lpBuffer);
                mpv_free(lpBuffer);

                return ret;
            }
            catch (Exception ex)
            {
                if (throwOnException) throw ex;
                return "";
            }
        }

        public static int get_property_int(string name, bool throwOnException = false)
        {
            int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);

            if (err < 0 && throwOnException)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                return lpBuffer.ToInt32();
        }

        public static double get_property_number(string name, bool throwOnException = false)
        {
            double val = 0;
            int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_DOUBLE, ref val);

            if (err < 0 && throwOnException)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                return val;
        }

        public static bool get_property_bool(string name, bool throwOnException = false)
        {
            int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_FLAG, out IntPtr lpBuffer);

            if (err < 0 && throwOnException)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                return lpBuffer.ToInt32() == 1;
        }

        public static void set_property_int(string name, int value, bool throwOnException = false)
        {
            Int64 val = value;
            int err = mpv_set_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);

            if (err < 0 && throwOnException)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void observe_property_int(string name, Action<int> action)
        {
            int err = mpv_observe_property(MpvHandle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_INT64);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                IntPropChangeActions.Add(new KeyValuePair<string, Action<int>>(name, action));
        }

        public static void observe_property_bool(string name, Action<bool> action)
        {
            int err = mpv_observe_property(MpvHandle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_FLAG);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                BoolPropChangeActions.Add(new KeyValuePair<string, Action<bool>>(name, action));
        }

        public static void observe_property_string(string name, Action<string> action)
        {
            int err = mpv_observe_property(MpvHandle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_STRING);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                StringPropChangeActions.Add(new KeyValuePair<string, Action<string>>(name, action));
        }

        protected static void ProcessCommandLine()
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            foreach (string i in args)
                if (!i.StartsWith("--") && File.Exists(i))
                    mp.commandv("loadfile", i, "append");

            mp.set_property_string("playlist-pos", "0");

            foreach (string i in args)
            {
                if (i.StartsWith("--"))
                {
                    if (i.Contains("="))
                    {
                        string left = i.Substring(2, i.IndexOf("=") - 2);
                        string right = i.Substring(left.Length + 3);
                        mp.set_property_string(left, right);
                    }
                    else
                        mp.set_property_string(i.Substring(2), "yes");
                }
            }
        }

        public static void LoadFiles(params string[] files)
        {
            int count = mp.get_property_int("playlist-count");

            foreach (string file in files)
                mp.commandv("loadfile", file, "append");

            mp.set_property_int("playlist-pos", count);

            for (int i = 0; i < count; i++)
                mp.commandv("playlist-remove", "0");

            mp.LoadFolder();
        }

        static bool WasFolderLoaded;

        static void LoadFolder()
        {
            if (WasFolderLoaded) return;

            if (get_property_int("playlist-count") == 1)
            {
                string path = get_property_string("path");
                if (!Directory.Exists(Path.GetDirectoryName(path))) return;
                string[] types = "264 265 3gp aac ac3 avc avi avs bmp divx dts dtshd dtshr dtsma eac3 evo flac flv h264 h265 hevc hvc jpg jpeg m2t m2ts m2v m4a m4v mka mkv mlp mov mp2 mp3 mp4 mpa mpeg mpg mpv mts ogg ogm opus pcm png pva raw rmvb thd thd+ac3 true-hd truehd ts vdr vob vpy w64 wav webm wmv y4m".Split(' ');
                List<string> files = Directory.GetFiles(Path.GetDirectoryName(path)).ToList();
                files = files.Where((file) => types.Contains(Path.GetExtension(file).TrimStart(".".ToCharArray()).ToLower())).ToList();
                files.Sort(new StringLogicalComparer());
                int index = files.IndexOf(path);
                files.Remove(path);

                foreach (string i in files)
                    commandv("loadfile", i, "append");

                if (index > 0)
                    commandv("playlist-move", "0", (index + 1).ToString());
            }

            WasFolderLoaded = true;
        }

        static IntPtr AllocateUtf8IntPtrArrayWithSentinel(string[] arr, out IntPtr[] byteArrayPointers)
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

        static string[] NativeUtf8StrArray2ManagedStrArray(IntPtr pUnmanagedStringArray, int StringCount)
        {
            IntPtr[] pIntPtrArray = new IntPtr[StringCount];
            string[] ManagedStringArray = new string[StringCount];
            Marshal.Copy(pUnmanagedStringArray, pIntPtrArray, 0, StringCount);

            for (int i = 0; i < StringCount; i++)
                ManagedStringArray[i] = StringFromNativeUtf8(pIntPtrArray[i]);

            return ManagedStringArray;
        }

        static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            int len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        static byte[] GetUtf8Bytes(string s) => Encoding.UTF8.GetBytes(s + "\0");

        static string LastHistoryPath;
        static DateTime LastHistoryStartDateTime;

        static void WriteHistory(string filePath)
        {
            int totalMinutes = Convert.ToInt32((DateTime.Now - LastHistoryStartDateTime).TotalMinutes);

            if (File.Exists(LastHistoryPath) && totalMinutes > 1)
            {
                string historyFilepath = mp.MpvConfFolder + "history.txt";

                File.AppendAllText(historyFilepath, DateTime.Now.ToString().Substring(0, 16) +
                    " " + totalMinutes.ToString().PadLeft(3) + " " +
                    Path.GetFileNameWithoutExtension(LastHistoryPath) + "\r\n");
            }

            LastHistoryPath = filePath;
            LastHistoryStartDateTime = DateTime.Now;
        }

        static void ReadMetaData()
        {
            lock (MediaTracks)
            {
                MediaTracks.Clear();

                using (MediaInfo mi = new MediaInfo(mp.get_property_string("path")))
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
                        track.Text = "V: " + track.Text.Trim(" ,".ToCharArray());
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
                        track.Text = "A: " + track.Text.Trim(" ,".ToCharArray());
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
                        track.Text = "S: " + track.Text.Trim(" ,".ToCharArray());
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