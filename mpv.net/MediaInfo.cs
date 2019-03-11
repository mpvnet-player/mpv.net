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

    ~MediaInfo()
    {
        Dispose();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string path);

    [DllImport("MediaInfo.dll")]
    private static extern IntPtr MediaInfo_New();

    [DllImport("MediaInfo.dll")]
    private static extern void MediaInfo_Delete(IntPtr Handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    private static extern int MediaInfo_Open(IntPtr Handle, string FileName);

    [DllImport("MediaInfo.dll")]
    private static extern int MediaInfo_Close(IntPtr Handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr MediaInfo_Get(IntPtr Handle, MediaInfoStreamKind StreamKind, int StreamNumber, string Parameter, MediaInfoInfoKind KindOfInfo, MediaInfoInfoKind KindOfSearch);
}

public enum MediaInfoStreamKind
{
    General,
    Video,
    Audio,
    Text,
    Chapters,
    Image
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