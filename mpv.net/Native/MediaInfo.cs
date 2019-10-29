using System;
using System.Runtime.InteropServices;

public class MediaInfo : IDisposable
{
    IntPtr Handle;
    static bool Loaded;

    public MediaInfo(string sourcepath)
    {
        if (!Loaded)
        {
            if (Native.LoadLibrary("MediaInfo.dll") == IntPtr.Zero)
                throw new Exception("Failed to load MediaInfo.dll.");

            Loaded = true;
        }

        Handle = MediaInfo_New();
        MediaInfo_Open(Handle, sourcepath);
    }

    public string GetInfo(MediaInfoStreamKind streamKind, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, streamKind, 0, parameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name));
    }

    public int GetCount(MediaInfoStreamKind streamKind) => MediaInfo_Count_Get(Handle, streamKind, -1);

    public string GetVideo(int streamNumber, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.Video, streamNumber, parameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name));
    }

    public string GetAudio(int streamNumber, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.Audio, streamNumber, parameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name));
    }

    public string GetText(int streamNumber, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.Text, streamNumber, parameter, MediaInfoInfoKind.Text, MediaInfoInfoKind.Name));
    }

    bool Disposed;

    public void Dispose()
    {
        if (!Disposed)
        {
            Disposed = true;
            MediaInfo_Close(Handle);
            MediaInfo_Delete(Handle);
        }
    }

    ~MediaInfo() { Dispose(); }

    [DllImport("MediaInfo.dll")]
    static extern IntPtr MediaInfo_New();

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern int MediaInfo_Open(IntPtr handle, string path);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr MediaInfo_Option(IntPtr handle, string optionString, string value);

    [DllImport("MediaInfo.dll")]
    static extern IntPtr MediaInfo_Inform(IntPtr handle, int reserved);

    [DllImport("MediaInfo.dll")]
    static extern int MediaInfo_Close(IntPtr handle);

    [DllImport("MediaInfo.dll")]
    static extern void MediaInfo_Delete(IntPtr handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr MediaInfo_Get(IntPtr handle,
                                       MediaInfoStreamKind streamKind,
                                       int streamNumber,
                                       string parameter,
                                       MediaInfoInfoKind kindOfInfo,
                                       MediaInfoInfoKind kindOfSearch);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern int MediaInfo_Count_Get(IntPtr handle,
                                          MediaInfoStreamKind streamKind,
                                          int streamNumber);
}

public enum MediaInfoStreamKind
{
    General,
    Video,
    Audio,
    Text,
    Other,
    Image,
    Menu,
    Max,
}

public enum MediaInfoInfoKind
{
    Name,
    Text,
    Measure,
    Options,
    NameText,
    MeasureText,
    Info,
    HowTo
}