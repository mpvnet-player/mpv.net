
#pragma warning disable IDE1006 // type name starts with underscore
#pragma warning disable CA1401  // P/Invokes should not be visible
#pragma warning disable CA2101  // Specify marshaling for P/Invoke string arguments

using System.Runtime.InteropServices;
using System.Text;

namespace MpvNet.Native;

public static class LibMpv
{
    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint mpv_create();

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint mpv_create_client(nint mpvHandle, [MarshalAs(UnmanagedType.LPUTF8Str)] string command);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_initialize(nint mpvHandle);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void mpv_destroy(nint mpvHandle);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_command(nint mpvHandle, nint strings);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_command_string(nint mpvHandle, [MarshalAs(UnmanagedType.LPUTF8Str)] string command);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_command_ret(nint mpvHandle, nint strings, nint node);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void mpv_free_node_contents(nint node);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint mpv_error_string(mpv_error error);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_request_log_messages(nint mpvHandle, [MarshalAs(UnmanagedType.LPUTF8Str)] string min_level);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_set_option(nint mpvHandle, byte[] name, mpv_format format, ref long data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_set_option_string(nint mpvHandle, byte[] name, byte[] value);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_get_property(nint mpvHandle, byte[] name, mpv_format format, out nint data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_get_property(nint mpvHandle, byte[] name, mpv_format format, out double data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_set_property(nint mpvHandle, byte[] name, mpv_format format, ref byte[] data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_set_property(nint mpvHandle, byte[] name, mpv_format format, ref long data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_set_property(nint mpvHandle, byte[] name, mpv_format format, ref double data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_observe_property(nint mpvHandle, ulong reply_userdata, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, mpv_format format);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int mpv_unobserve_property(nint mpvHandle, ulong registered_reply_userdata);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void mpv_free(nint data);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern nint mpv_wait_event(nint mpvHandle, double timeout);

    [DllImport("libmpv-2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern mpv_error mpv_request_event(nint mpvHandle, mpv_event_id id, int enable);

    public enum mpv_error
    {
        MPV_ERROR_SUCCESS = 0,
        MPV_ERROR_EVENT_QUEUE_FULL = -1,
        MPV_ERROR_NOMEM = -2,
        MPV_ERROR_UNINITIALIZED = -3,
        MPV_ERROR_INVALID_PARAMETER = -4,
        MPV_ERROR_OPTION_NOT_FOUND = -5,
        MPV_ERROR_OPTION_FORMAT = -6,
        MPV_ERROR_OPTION_ERROR = -7,
        MPV_ERROR_PROPERTY_NOT_FOUND = -8,
        MPV_ERROR_PROPERTY_FORMAT = -9,
        MPV_ERROR_PROPERTY_UNAVAILABLE = -10,
        MPV_ERROR_PROPERTY_ERROR = -11,
        MPV_ERROR_COMMAND = -12,
        MPV_ERROR_LOADING_FAILED = -13,
        MPV_ERROR_AO_INIT_FAILED = -14,
        MPV_ERROR_VO_INIT_FAILED = -15,
        MPV_ERROR_NOTHING_TO_PLAY = -16,
        MPV_ERROR_UNKNOWN_FORMAT = -17,
        MPV_ERROR_UNSUPPORTED = -18,
        MPV_ERROR_NOT_IMPLEMENTED = -19,
        MPV_ERROR_GENERIC = -20
    }

    public enum mpv_event_id
    {
        MPV_EVENT_NONE = 0,
        MPV_EVENT_SHUTDOWN = 1,
        MPV_EVENT_LOG_MESSAGE = 2,
        MPV_EVENT_GET_PROPERTY_REPLY = 3,
        MPV_EVENT_SET_PROPERTY_REPLY = 4,
        MPV_EVENT_COMMAND_REPLY = 5,
        MPV_EVENT_START_FILE = 6,
        MPV_EVENT_END_FILE = 7,
        MPV_EVENT_FILE_LOADED = 8,
        MPV_EVENT_IDLE = 11, //deprecated
        MPV_EVENT_TICK = 14, //deprecated
        MPV_EVENT_SCRIPT_INPUT_DISPATCH = 15,
        MPV_EVENT_CLIENT_MESSAGE = 16,
        MPV_EVENT_VIDEO_RECONFIG = 17,
        MPV_EVENT_AUDIO_RECONFIG = 18,
        MPV_EVENT_SEEK = 20,
        MPV_EVENT_PLAYBACK_RESTART = 21,
        MPV_EVENT_PROPERTY_CHANGE = 22,
        MPV_EVENT_QUEUE_OVERFLOW = 24,
        MPV_EVENT_HOOK = 25
    }

    public enum mpv_format
    {
        MPV_FORMAT_NONE = 0,
        MPV_FORMAT_STRING = 1,
        MPV_FORMAT_OSD_STRING = 2,
        MPV_FORMAT_FLAG = 3,
        MPV_FORMAT_INT64 = 4,
        MPV_FORMAT_DOUBLE = 5,
        MPV_FORMAT_NODE = 6,
        MPV_FORMAT_NODE_ARRAY = 7,
        MPV_FORMAT_NODE_MAP = 8,
        MPV_FORMAT_BYTE_ARRAY = 9
    }

    public enum mpv_log_level
    {
        MPV_LOG_LEVEL_NONE = 0,
        MPV_LOG_LEVEL_FATAL = 10,
        MPV_LOG_LEVEL_ERROR = 20,
        MPV_LOG_LEVEL_WARN = 30,
        MPV_LOG_LEVEL_INFO = 40,
        MPV_LOG_LEVEL_V = 50,
        MPV_LOG_LEVEL_DEBUG = 60,
        MPV_LOG_LEVEL_TRACE = 70,
    }

    public enum mpv_end_file_reason
    {
        MPV_END_FILE_REASON_EOF = 0,
        MPV_END_FILE_REASON_STOP = 2,
        MPV_END_FILE_REASON_QUIT = 3,
        MPV_END_FILE_REASON_ERROR = 4,
        MPV_END_FILE_REASON_REDIRECT = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mpv_event_log_message
    {
        public nint prefix;
        public nint level;
        public nint text;
        public mpv_log_level log_level;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mpv_event
    {
        public mpv_event_id event_id;
        public int error;
        public ulong reply_userdata;
        public nint data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mpv_event_client_message
    {
        public int num_args;
        public nint args;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mpv_event_property
    {
        public string name;
        public mpv_format format;
        public nint data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct mpv_event_end_file
    {
        public int reason;
        public int error;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct mpv_node
    {
        [FieldOffset(0)] public nint str;
        [FieldOffset(0)] public int flag;
        [FieldOffset(0)] public long int64;
        [FieldOffset(0)] public double dbl;
        [FieldOffset(0)] public nint list;
        [FieldOffset(0)] public nint ba;
        [FieldOffset(8)] public mpv_format format;
    }

    public static string[] ConvertFromUtf8Strings(nint utf8StringArray, int stringCount)
    {
        nint[] intPtrArray = new nint[stringCount];
        string[] stringArray = new string[stringCount];
        Marshal.Copy(utf8StringArray, intPtrArray, 0, stringCount);

        for (int i = 0; i < stringCount; i++)
            stringArray[i] = ConvertFromUtf8(intPtrArray[i]);

        return stringArray;
    }

    public static string ConvertFromUtf8(nint nativeUtf8)
    {
        int len = 0;

        while (Marshal.ReadByte(nativeUtf8, len) != 0)
            ++len;

        byte[] buffer = new byte[len];
        Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer);
    }

    public static string GetError(mpv_error err) => ConvertFromUtf8(mpv_error_string(err));

    public static byte[] GetUtf8Bytes(string s) => Encoding.UTF8.GetBytes(s + "\0");
}
