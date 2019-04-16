using System;
using System.Runtime.InteropServices;

public class MediaInfo : IDisposable
{
    private IntPtr Handle;
    private static bool Loaded;

    public MediaInfo(string sourcepath)
    {
        if (!Loaded)
        {
            if (LoadLibrary("MediaInfo.dll") == IntPtr.Zero)
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

    private bool Disposed;

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

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string path);

    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_New();

    [DllImport("MediaInfo.dll")]
    private static extern void MediaInfo_Delete(IntPtr handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    private static extern int MediaInfo_Open(IntPtr handle, string fileName);

    [DllImport("MediaInfo.dll")]
    private static extern int MediaInfo_Close(IntPtr handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr MediaInfo_Get(IntPtr handle,
                                               MediaInfoStreamKind streamKind,
                                               int streamNumber,
                                               string parameter,
                                               MediaInfoInfoKind kindOfInfo,
                                               MediaInfoInfoKind kindOfSearch);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    private static extern int MediaInfo_Count_Get(IntPtr handle,
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