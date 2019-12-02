
using System;
using System.Runtime.InteropServices;

namespace mpvnet
{
    public class libmpv
    {
        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mpv_create();

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_initialize(IntPtr mpvHandle);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_command(IntPtr mpvHandle, IntPtr strings);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_command_string(IntPtr mpvHandle, [MarshalAs(UnmanagedType.LPUTF8Str)] string command);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_terminate_destroy(IntPtr mpvHandle);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_option(IntPtr mpvHandle, byte[] name, mpv_format format, ref long data);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_option_string(IntPtr mpvHandle, byte[] name, byte[] value);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_get_property(IntPtr mpvHandle, byte[] name, mpv_format format, out IntPtr data);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_get_property(IntPtr mpvHandle, byte[] name, mpv_format format, ref double data);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_property(IntPtr mpvHandle, byte[] name, mpv_format format, ref byte[] data);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_set_property(IntPtr mpvHandle, byte[] name, mpv_format format, ref Int64 data);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_observe_property(
            IntPtr mpvHandle,
            UInt64 reply_userdata,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
            mpv_format format);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int mpv_unobserve_property(IntPtr mpvHandle, UInt64 registered_reply_userdata);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mpv_free(IntPtr data);

        [DllImport("mpv-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mpv_wait_event(IntPtr mpvHandle, double timeout);

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
            MPV_EVENT_TRACKS_CHANGED = 9,
            MPV_EVENT_TRACK_SWITCHED = 10,
            MPV_EVENT_IDLE = 11,
            MPV_EVENT_PAUSE = 12,
            MPV_EVENT_UNPAUSE = 13,
            MPV_EVENT_TICK = 14,
            MPV_EVENT_SCRIPT_INPUT_DISPATCH = 15,
            MPV_EVENT_CLIENT_MESSAGE = 16,
            MPV_EVENT_VIDEO_RECONFIG = 17,
            MPV_EVENT_AUDIO_RECONFIG = 18,
            MPV_EVENT_METADATA_UPDATE = 19,
            MPV_EVENT_SEEK = 20,
            MPV_EVENT_PLAYBACK_RESTART = 21,
            MPV_EVENT_PROPERTY_CHANGE = 22,
            MPV_EVENT_CHAPTER_CHANGE = 23,
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
            MPV_LOG_LEVEL_NONE  = 0,
            MPV_LOG_LEVEL_FATAL = 10,
            MPV_LOG_LEVEL_ERROR = 20,
            MPV_LOG_LEVEL_WARN  = 30,
            MPV_LOG_LEVEL_INFO  = 40,
            MPV_LOG_LEVEL_V     = 50,
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
            public string prefix;
            public string level;
            public string text;
            public mpv_log_level log_level;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct mpv_event
        {
            public mpv_event_id event_id;
            public int error;
            public UInt64 reply_userdata;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct mpv_event_client_message
        {
            public int num_args;
            public IntPtr args;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct mpv_event_property
        {
            public string name;
            public mpv_format format;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct mpv_event_end_file
        {
            public int reason;
            public int error;
        }
    }
}