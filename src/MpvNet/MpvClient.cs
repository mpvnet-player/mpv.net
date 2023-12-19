
using System.Runtime.InteropServices;

using static MpvNet.Native.LibMpv;

namespace MpvNet;

public class MpvClient
{
    public event Action<string[]>? ClientMessage;            // client-message      MPV_EVENT_CLIENT_MESSAGE
    public event Action<mpv_log_level, string>? LogMessage;  // log-message         MPV_EVENT_LOG_MESSAGE
    public event Action<mpv_end_file_reason>? EndFile;       // end-file            MPV_EVENT_END_FILE
    public event Action? Shutdown;                           // shutdown            MPV_EVENT_SHUTDOWN
    public event Action? GetPropertyReply;                   // get-property-reply  MPV_EVENT_GET_PROPERTY_REPLY
    public event Action? SetPropertyReply;                   // set-property-reply  MPV_EVENT_SET_PROPERTY_REPLY
    public event Action? CommandReply;                       // command-reply       MPV_EVENT_COMMAND_REPLY
    public event Action? StartFile;                          // start-file          MPV_EVENT_START_FILE
    public event Action? FileLoaded;                         // file-loaded         MPV_EVENT_FILE_LOADED
    public event Action? VideoReconfig;                      // video-reconfig      MPV_EVENT_VIDEO_RECONFIG
    public event Action? AudioReconfig;                      // audio-reconfig      MPV_EVENT_AUDIO_RECONFIG
    public event Action? Seek;                               // seek                MPV_EVENT_SEEK
    public event Action? PlaybackRestart;                    // playback-restart    MPV_EVENT_PLAYBACK_RESTART

    public Dictionary<string, List<Action>> PropChangeActions { get; set; } = new Dictionary<string, List<Action>>();
    public Dictionary<string, List<Action<int>>> IntPropChangeActions { get; set; } = new Dictionary<string, List<Action<int>>>();
    public Dictionary<string, List<Action<bool>>> BoolPropChangeActions { get; set; } = new Dictionary<string, List<Action<bool>>>();
    public Dictionary<string, List<Action<double>>> DoublePropChangeActions { get; set; } = new Dictionary<string, List<Action<double>>>();
    public Dictionary<string, List<Action<string>>> StringPropChangeActions { get; set; } = new Dictionary<string, List<Action<string>>>();

    public nint Handle { get; set; }

    public void EventLoop()
    {
        while (true)
        {
            IntPtr ptr = mpv_wait_event(Handle, -1);
            mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event))!;

            try
            {
                switch (evt.event_id)
                {
                    case mpv_event_id.MPV_EVENT_SHUTDOWN:
                        OnShutdown();
                        return;
                    case mpv_event_id.MPV_EVENT_LOG_MESSAGE:
                        {
                            var data = (mpv_event_log_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_log_message))!; 
                            OnLogMessage(data);
                        }
                        break;
                    case mpv_event_id.MPV_EVENT_CLIENT_MESSAGE:
                        {
                            var data = (mpv_event_client_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_client_message))!;
                            OnClientMessage(data);
                        }
                        break;
                    case mpv_event_id.MPV_EVENT_VIDEO_RECONFIG:
                        OnVideoReconfig();
                        break;
                    case mpv_event_id.MPV_EVENT_END_FILE:
                        {
                            var data = (mpv_event_end_file)Marshal.PtrToStructure(evt.data, typeof(mpv_event_end_file))!;
                            OnEndFile(data);
                        }
                        break;
                    case mpv_event_id.MPV_EVENT_FILE_LOADED:  // triggered after MPV_EVENT_START_FILE
                        OnFileLoaded();
                        break;
                    case mpv_event_id.MPV_EVENT_PROPERTY_CHANGE:
                        {
                            var data = (mpv_event_property)Marshal.PtrToStructure(evt.data, typeof(mpv_event_property))!;
                            OnPropertyChange(data);
                        }
                        break;
                    case mpv_event_id.MPV_EVENT_GET_PROPERTY_REPLY:
                        OnGetPropertyReply();
                        break;
                    case mpv_event_id.MPV_EVENT_SET_PROPERTY_REPLY:
                        OnSetPropertyReply();
                        break;
                    case mpv_event_id.MPV_EVENT_COMMAND_REPLY:
                        OnCommandReply();
                        break;
                    case mpv_event_id.MPV_EVENT_START_FILE:  // triggered before MPV_EVENT_FILE_LOADED
                        OnStartFile();
                        break;
                    case mpv_event_id.MPV_EVENT_AUDIO_RECONFIG:
                        OnAudioReconfig();
                        break;
                    case mpv_event_id.MPV_EVENT_SEEK:
                        OnSeek();
                        break;
                    case mpv_event_id.MPV_EVENT_PLAYBACK_RESTART:
                        OnPlaybackRestart();
                        break;
                }
            }
            catch (Exception ex)
            {
                Terminal.WriteError(ex);
            }
        }
    }

    protected virtual void OnClientMessage(mpv_event_client_message data) =>
        ClientMessage?.Invoke(ConvertFromUtf8Strings(data.args, data.num_args));

    protected virtual void OnLogMessage(mpv_event_log_message data)
    {
        if (LogMessage != null)
        {
            string msg = $"[{ConvertFromUtf8(data.prefix)}] {ConvertFromUtf8(data.text)}";
            LogMessage.Invoke(data.log_level, msg);
        }
    }

    protected virtual void OnPropertyChange(mpv_event_property data)
    {
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

    protected virtual void OnEndFile(mpv_event_end_file data) => EndFile?.Invoke((mpv_end_file_reason)data.reason);
    protected virtual void OnFileLoaded() => FileLoaded?.Invoke();
    protected virtual void OnShutdown() => Shutdown?.Invoke();
    protected virtual void OnGetPropertyReply() => GetPropertyReply?.Invoke();
    protected virtual void OnSetPropertyReply() => SetPropertyReply?.Invoke();
    protected virtual void OnCommandReply() => CommandReply?.Invoke();
    protected virtual void OnStartFile() => StartFile?.Invoke();
    protected virtual void OnVideoReconfig() => VideoReconfig?.Invoke();
    protected virtual void OnAudioReconfig() => AudioReconfig?.Invoke();
    protected virtual void OnSeek() => Seek?.Invoke();
    protected virtual void OnPlaybackRestart() => PlaybackRestart?.Invoke();

    public void Command(string command)
    {
        mpv_error err = mpv_command_string(Handle, command);

        if (err < 0)
            HandleError(err, "error executing command: " + command);
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
            HandleError(err, "error executing command: " + string.Join("\n", args));
    }

    public string Expand(string? value)
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
            HandleError(err, "error executing command: " + string.Join("\n", args));
            Marshal.FreeHGlobal(resultNodePtr);
            return "property expansion error";
        }

        mpv_node resultNode = Marshal.PtrToStructure<mpv_node>(resultNodePtr);
        string ret = ConvertFromUtf8(resultNode.str);
        mpv_free_node_contents(resultNodePtr);
        Marshal.FreeHGlobal(resultNodePtr);
        return ret;
    }

    public bool GetPropertyBool(string name)
    {
        mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
            mpv_format.MPV_FORMAT_FLAG, out IntPtr lpBuffer);

        if (err < 0)
            HandleError(err, "error getting property: " + name);

        return lpBuffer.ToInt32() != 0;
    }

    public void SetPropertyBool(string name, bool value)
    {
        long val = value ? 1 : 0;
        mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_FLAG, ref val);

        if (err < 0)
            HandleError(err, $"error setting property: {name} = {value}");
    }

    public int GetPropertyInt(string name)
    {
        mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
            mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);

        if (err < 0 && App.DebugMode)
            HandleError(err, "error getting property: " + name);

        return lpBuffer.ToInt32();
    }

    public void SetPropertyInt(string name, int value)
    {
        long val = value;
        mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);

        if (err < 0)
            HandleError(err, $"error setting property: {name} = {value}");
    }

    public void SetPropertyLong(string name, long value)
    {
        mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref value);

        if (err < 0)
            HandleError(err, $"error setting property: {name} = {value}");
    }

    public long GetPropertyLong(string name)
    {
        mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
            mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);

        if (err < 0)
            HandleError(err, "error getting property: " + name);

        return lpBuffer.ToInt64();
    }

    public double GetPropertyDouble(string name, bool handleError = true)
    {
        mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
            mpv_format.MPV_FORMAT_DOUBLE, out double value);

        if (err < 0 && handleError && App.DebugMode)
            HandleError(err, "error getting property: " + name);

        return value;
    }

    public void SetPropertyDouble(string name, double value)
    {
        double val = value;
        mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_DOUBLE, ref val);

        if (err < 0)
            HandleError(err, $"error setting property: {name} = {value}");
    }

    public string GetPropertyString(string name)
    {
        mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
            mpv_format.MPV_FORMAT_STRING, out IntPtr lpBuffer);

        if (err == 0)
        {
            string ret = ConvertFromUtf8(lpBuffer);
            mpv_free(lpBuffer);
            return ret;
        }

        if (err < 0 && App.DebugMode)
            HandleError(err, "error getting property: " + name);

        return "";
    }

    public void SetPropertyString(string name, string value)
    {
        byte[] bytes = GetUtf8Bytes(value);
        mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref bytes);

        if (err < 0)
            HandleError(err, $"error setting property: {name} = {value}");
    }

    public string GetPropertyOsdString(string name)
    {
        mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
            mpv_format.MPV_FORMAT_OSD_STRING, out IntPtr lpBuffer);

        if (err == 0)
        {
            string ret = ConvertFromUtf8(lpBuffer);
            mpv_free(lpBuffer);
            return ret;
        }

        if (err < 0)
            HandleError(err, "error getting property: " + name);

        return "";
    }

    public void ObservePropertyInt(string name, Action<int> action)
    {
        lock (IntPropChangeActions)
        {
            if (!IntPropChangeActions.ContainsKey(name))
            {
                mpv_error err = mpv_observe_property(Handle, 0, name, mpv_format.MPV_FORMAT_INT64);

                if (err < 0)
                    HandleError(err, "error observing property: " + name);
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
                    HandleError(err, "error observing property: " + name);
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
                    HandleError(err, "error observing property: " + name);
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
                    HandleError(err, "error observing property: " + name);
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
                    HandleError(err, "error observing property: " + name);
                else
                    PropChangeActions[name] = new List<Action>();
            }

            if (PropChangeActions.ContainsKey(name))
                PropChangeActions[name].Add(action);
        }
    }

    static void HandleError(mpv_error err, string msg)
    {
        Terminal.WriteError(msg);
        Terminal.WriteError(GetError(err));
    }
}
