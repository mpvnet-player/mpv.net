using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static mpvnet.libmpv;
using static mpvnet.Native;
using static mpvnet.StaticUsing;

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

        public static IntPtr MpvHandle;
        public static IntPtr MpvWindowHandle;
        public static Addon Addon;
        public static List<KeyValuePair<string, Action<bool>>> BoolPropChangeActions = new List<KeyValuePair<string, Action<bool>>>();
        public static Size VideoSize = new Size(1920, 1080);
        public static string mpvConfFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\";
        public static string InputConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\input.conf";
        public static string mpvConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpv.conf";
        public static List<PyScript> PyScripts { get; } = new List<PyScript>();
        public static bool IsShutdownComplete { get; set; }

        private static Dictionary<string, string> _mpvConv;

        public static Dictionary<string, string> mpvConv {
            get {
                if (_mpvConv == null)
                {
                    _mpvConv = new Dictionary<string, string>();

                    if (File.Exists(mpvConfPath))
                    {
                        foreach (var i in File.ReadAllLines(mpvConfPath))
                        {
                            if (i.Contains("=") && ! i.StartsWith("#"))
                            {
                                _mpvConv[i.Left("=").Trim()] = i.Right("=").Trim();
                            }
                        }
                    }
                }
                return _mpvConv;
            }
        }

        public static void Init()
        {
            if (!Directory.Exists(mp.mpvConfFolderPath))
                Directory.CreateDirectory(mp.mpvConfFolderPath);

            if (!File.Exists(mp.mpvConfPath))
                File.WriteAllText(mp.mpvConfPath, Properties.Resources.mpv_conf);

            if (!File.Exists(mp.InputConfPath))
                File.WriteAllText(mp.InputConfPath, Properties.Resources.input_conf);

            LoadLibrary("mpv-1.dll");
            MpvHandle = mpv_create();
            SetStringProp("input-default-bindings", "yes");
            SetStringProp("osc", "yes");
            SetStringProp("config", "yes");
            SetStringProp("wid", MainForm.Hwnd.ToString());
            SetStringProp("force-window", "yes");
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
                    mp.Command("load-script", $"{scriptPath}");

            foreach (var scriptPath in startupScripts)
                if (Path.GetExtension(scriptPath) == ".py")
                    PyScripts.Add(new PyScript(File.ReadAllText(scriptPath)));

            foreach(var scriptPath in Directory.GetFiles(mp.mpvConfFolderPath + "scripts", "*.py"))
                PyScripts.Add(new PyScript(File.ReadAllText(scriptPath)));
        }

        public static void EventLoop()
        {
            while (true)
            {
                IntPtr ptr = mpv_wait_event(MpvHandle, -1);
                mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event));

                if (MpvWindowHandle == IntPtr.Zero)
                    MpvWindowHandle = FindWindowEx(MainForm.Hwnd, IntPtr.Zero, "mpv", null);

                switch (evt.event_id)
                {
                    case mpv_event_id.MPV_EVENT_SHUTDOWN:
                        Shutdown?.Invoke();
                        IsShutdownComplete = true;
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
                        if (ClientMessage != null)
                        {
                            var client_messageData = (mpv_event_client_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_client_message));
                            var args = NativeUtf8StrArray2ManagedStrArray(client_messageData.args, client_messageData.num_args);

                            if (args != null && args.Length > 1 && args[0] == "mpv.net")
                                foreach (var i in mpvnet.Command.Commands)
                                    if (args[1] == i.Name)
                                        try
                                        {
                                            i.Action(args.Skip(2).ToArray());
                                        }
                                        catch (Exception ex)
                                        {
                                            MsgError(ex.GetType().Name + "\r\n\r\n" + ex.ToString());
                                        }
                            ClientMessage?.Invoke(args);
                        }
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
                        var event_propertyData = (mpv_event_property)Marshal.PtrToStructure(evt.data, typeof(mpv_event_property));

                        if (event_propertyData.format == mpv_format.MPV_FORMAT_FLAG)
                            foreach (var i in BoolPropChangeActions)
                                if (i.Key== event_propertyData.name)
                                    i.Value.Invoke(Marshal.PtrToStructure<int>(event_propertyData.data) == 1);
                        break;
                    case mpv_event_id.MPV_EVENT_PLAYBACK_RESTART:
                        PlaybackRestart?.Invoke();
                        Size s = new Size(GetIntProp("dwidth", false), GetIntProp("dheight", false));

                        if (VideoSize != s && s != Size.Empty)
                        {
                            VideoSize = s;
                            VideoSizeChanged?.Invoke();
                        }
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
        }

        public class EventObject
        {
            public PyRT.PythonFunction PythonFunction { get; set; }
            public EventInfo EventInfo { get; set; }
            public Delegate Delegate { get; set; }

            public void Invoke()
            {
                PyRT.Operations.PythonCalls.Call(PythonFunction);
            }

            public void InvokeEndFileEventMode(EndFileEventMode arg)
            {
                PyRT.Operations.PythonCalls.Call(PythonFunction, new[] { arg });
            }

            public void InvokeStrings(string[] arg)
            {
                PyRT.Operations.PythonCalls.Call(PythonFunction, new[] { arg });
            }
        }

        private static List<EventObject> EventObjects = new List<EventObject>();

        public static void register_event(string name, PyRT.PythonFunction pyFunc)
        {
            foreach (var eventInfo in typeof(mp).GetEvents())
            {
                if (eventInfo.Name.ToLower() == name.Replace("-", ""))
                {
                    EventObject eventObject = new EventObject();
                    EventObjects.Add(eventObject);
                    eventObject.PythonFunction = pyFunc;
                    MethodInfo mi;

                    if (eventInfo.EventHandlerType == typeof(Action))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(EventObject.Invoke));
                    }
                    else if (eventInfo.EventHandlerType == typeof(Action<EndFileEventMode>))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(EventObject.InvokeEndFileEventMode));
                    }
                    else if (eventInfo.EventHandlerType == typeof(Action<string[]>))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(EventObject.InvokeStrings));
                    }
                    else
                        throw new Exception();

                    eventObject.EventInfo = eventInfo;
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, eventObject, mi);
                    eventObject.Delegate = handler;
                    eventInfo.AddEventHandler(eventObject, handler);
                }
            }
        }

        public static void unregister_event(PyRT.PythonFunction pyFunc)
        {
            foreach (var eventObjects in EventObjects)
                if (eventObjects.PythonFunction == pyFunc)
                    eventObjects.EventInfo.RemoveEventHandler(eventObjects, eventObjects.Delegate);
        }

        public static void commandv(params string[] args)
        {
            Command(args);
        }

        public static void Command(params string[] args)
        {
            if (MpvHandle == IntPtr.Zero)
                return;

            IntPtr[] byteArrayPointers;
            var mainPtr = AllocateUtf8IntPtrArrayWithSentinel(args, out byteArrayPointers);
            int err = mpv_command(MpvHandle, mainPtr);

            if (err < 0)
                throw new Exception($"{(mpv_error)err}");

            foreach (var ptr in byteArrayPointers)
                Marshal.FreeHGlobal(ptr);

            Marshal.FreeHGlobal(mainPtr);
        }

        public static void CommandString(string command, bool throwException = true)
        {
            if (MpvHandle == IntPtr.Zero)
                return;

            int err = mpv_command_string(MpvHandle, command);

            if (err < 0 && throwException)
                throw new Exception($"{(mpv_error)err}\r\n\r\n" + command);
        }

        public static void SetStringProp(string name, string value, bool throwException = true)
        {
            var bytes = GetUtf8Bytes(value);
            int err = mpv_set_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref bytes);

            if (err < 0 && throwException)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static string GetStringProp(string name)
        {
            var lpBuffer = IntPtr.Zero;
            int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref lpBuffer);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");

            var ret = StringFromNativeUtf8(lpBuffer);
            mpv_free(lpBuffer);

            return ret;
        }

        public static int GetIntProp(string name, bool throwException = true)
        {
            var lpBuffer = IntPtr.Zero;
            int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref lpBuffer);

            if (err < 0 && throwException)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                return lpBuffer.ToInt32();
        }

        public static double get_property_number(string name)
        {
            return GetDoubleProp(name);
        }

        public static double GetDoubleProp(string name, bool throwException = true)
        {
            double val = 0;
            int err = mpv_get_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_DOUBLE, ref val);

            if (err < 0 && throwException)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                return val;
        }

        public static void SetIntProp(string name, int value)
        {
            Int64 val = value;
            int err = mpv_set_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void ObserveBoolProp(string name, Action<bool> action)
        {
            int err = mpv_observe_property(MpvHandle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_FLAG);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
            else
                BoolPropChangeActions.Add(new KeyValuePair<string, Action<bool>>(name, action));
        }

        public static void UnobserveBoolProp(string name, Action<bool> action)
        {
            foreach (var i in BoolPropChangeActions.ToArray())
                if (i.Value == action)
                    BoolPropChangeActions.Remove(i);

            int err = mpv_unobserve_property(MpvHandle, (ulong)action.GetHashCode());

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void ProcessCommandLine()
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            foreach (string i in args)
                if (!i.StartsWith("--") && File.Exists(i))
                    mp.Command("loadfile", i, "append");

            mp.SetStringProp("playlist-pos", "0", false);

            foreach (string i in args)
            {
                if (i.StartsWith("--"))
                {
                    if (i.Contains("="))
                    {
                        string left = i.Substring(2, i.IndexOf("=") - 2);
                        string right = i.Substring(left.Length + 3);
                        mp.SetStringProp(left, right);
                    }
                    else
                        mp.SetStringProp(i.Substring(2), "yes");
                }
            }
        }

        public static void LoadFiles(string[] files)
        {
            int count = mp.GetIntProp("playlist-count");

            foreach (string file in files)
                mp.Command("loadfile", file, "append");

            mp.SetIntProp("playlist-pos", count);

            for (int i = 0; i < count; i++)
                mp.Command("playlist-remove", "0");

            mp.LoadFolder();
        }

        private static bool WasFolderLoaded;

        public static void LoadFolder()
        {
            if (WasFolderLoaded)
                return;

            if (GetIntProp("playlist-count") == 1)
            {
                string[] types = "264 265 3gp aac ac3 avc avi avs bmp divx dts dtshd dtshr dtsma eac3 evo flac flv h264 h265 hevc hvc jpg jpeg m2t m2ts m2v m4a m4v mka mkv mlp mov mp2 mp3 mp4 mpa mpeg mpg mpv mts ogg ogm opus pcm png pva raw rmvb thd thd+ac3 true-hd truehd ts vdr vob vpy w64 wav webm wmv y4m".Split(' ');
                string path = GetStringProp("path");
                List<string> files = Directory.GetFiles(Path.GetDirectoryName(path)).ToList();
                files = files.Where((file) => types.Contains(file.Ext())).ToList();
                files.Sort(new StringLogicalComparer());
                int index = files.IndexOf(path);
                files.Remove(path);

                foreach (string i in files)
                    Command("loadfile", i, "append");

                if (index > 0)
                    Command("playlist-move", "0", (index + 1).ToString());
            }

            WasFolderLoaded = true;
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

        public static string[] NativeUtf8StrArray2ManagedStrArray(IntPtr pUnmanagedStringArray, int StringCount)
        {
            IntPtr[] pIntPtrArray = new IntPtr[StringCount];
            string[] ManagedStringArray = new string[StringCount];
            Marshal.Copy(pUnmanagedStringArray, pIntPtrArray, 0, StringCount);

            for (int i = 0; i < StringCount; i++)
                ManagedStringArray[i] = StringFromNativeUtf8(pIntPtrArray[i]);

            return ManagedStringArray;
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