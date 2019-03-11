using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static mpvnet.libmpv;
using static mpvnet.Native;
using static mpvnet.StaticUsing;

namespace mpvnet
{
    public delegate void MpvBoolPropChangeHandler(string propName, bool value);

    public class mpv
    {
        public static event Action<string[]> ClientMessage;
        public static event Action Shutdown;
        public static event Action AfterShutdown;
        public static event Action PlaybackRestart;
        public static event Action VideoSizeChanged;

        public static IntPtr MpvHandle;
        public static IntPtr MpvWindowHandle;
        public static Addon Addon;
        public static List<Action<bool>> BoolPropChangeActions = new List<Action<bool>>();
        public static Size VideoSize = new Size(1920, 1080);
        public static string mpvConfFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\";
        public static string InputConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\input.conf";
        public static string mpvConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpv.conf";

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
            LoadLibrary("mpv-1.dll");
            MpvHandle = mpv_create();
            SetIntProp("input-ar-delay", 500);
            SetIntProp("input-ar-rate", 20);
            SetIntProp("volume", 50);
            SetStringProp("hwdec", "yes");
            SetStringProp("vo", "direct3d");
            SetStringProp("input-default-bindings", "yes");
            SetStringProp("osd-playing-msg", "'${filename}'");
            SetStringProp("screenshot-directory", "~~desktop/");
            SetStringProp("keep-open", "yes");
            SetStringProp("keep-open-pause", "no");
            SetStringProp("osc", "yes");
            SetStringProp("config", "yes");
            SetStringProp("wid", MainForm.Hwnd.ToString());
            SetStringProp("force-window", "yes");
            mpv_initialize(MpvHandle);
            ProcessCommandLine();
            Task.Run(() => { Addon = new Addon(); });
            Task.Run(() => { EventLoop(); });
        }

        public static void EventLoop()
        {
            while (true)
            {
                IntPtr ptr = mpv_wait_event(MpvHandle, -1);
                mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event));
                Debug.WriteLine(evt.event_id);

                if (MpvWindowHandle == IntPtr.Zero)
                    MpvWindowHandle = FindWindowEx(MainForm.Hwnd, IntPtr.Zero, "mpv", null);

                switch (evt.event_id)
                {
                    case mpv_event_id.MPV_EVENT_SHUTDOWN:
                        Shutdown?.Invoke();
                        AfterShutdown?.Invoke();
                        return;
                    case mpv_event_id.MPV_EVENT_FILE_LOADED:
                        LoadFolder();
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
                    case mpv_event_id.MPV_EVENT_PROPERTY_CHANGE:
                        var eventData = (mpv_event_property)Marshal.PtrToStructure(evt.data, typeof(mpv_event_property));

                        if (eventData.format == mpv_format.MPV_FORMAT_FLAG)
                            foreach (var action in BoolPropChangeActions)
                                action.Invoke(Marshal.PtrToStructure<int>(eventData.data) == 1);
                        break;
                }
            }
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

        public static void SetIntProp(string name, int value)
        {
            Int64 val = value;
            int err = mpv_set_property(MpvHandle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void ObserveBoolProp(string name, Action<bool> action)
        {
            BoolPropChangeActions.Add(action);
            int err = mpv_observe_property(MpvHandle, (ulong)action.GetHashCode(), name, mpv_format.MPV_FORMAT_FLAG);

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void UnobserveBoolProp(string name, Action<bool> action)
        {
            BoolPropChangeActions.Remove(action);
            int err = mpv_unobserve_property(MpvHandle, (ulong)action.GetHashCode());

            if (err < 0)
                throw new Exception($"{name}: {(mpv_error)err}");
        }

        public static void ProcessCommandLine()
        {
            var args = Environment.GetCommandLineArgs().Skip(1);

            foreach (string i in args)
                if (!i.StartsWith("--") && File.Exists(i))
                    mpv.Command("loadfile", i, "append");

            mpv.SetStringProp("playlist-pos", "0", false);

            foreach (string i in args)
            {
                if (i.StartsWith("--"))
                {
                    if (i.Contains("="))
                    {
                        string left = i.Substring(2, i.IndexOf("=") - 2);
                        string right = i.Substring(left.Length + 3);
                        mpv.SetStringProp(left, right);
                    }
                    else
                        mpv.SetStringProp(i.Substring(2), "yes");
                }
            }
        }

        public static void LoadFiles(string[] files)
        {
            int count = mpv.GetIntProp("playlist-count");

            foreach (string file in files)
                mpv.Command("loadfile", file, "append");

            mpv.SetIntProp("playlist-pos", count);

            for (int i = 0; i < count; i++)
                mpv.Command("playlist-remove", "0");

            mpv.LoadFolder();
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
}