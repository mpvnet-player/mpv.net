
using System.Runtime.InteropServices;

namespace MpvNet;

public class MediaInfo : IDisposable
{
    readonly IntPtr Handle;

    public MediaInfo(string file)
    {
        if ((Handle = MediaInfo_New()) == IntPtr.Zero)
            throw new Exception("Failed to call MediaInfo_New");

        if (MediaInfo_Open(Handle, file) == 0)
            throw new Exception("Error MediaInfo_Open");
    }

    public string GetInfo(MediaInfoStreamKind kind, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, kind, 0,
            parameter, MediaInfoKind.Text, MediaInfoKind.Name)) ?? "";
    }

    public int GetCount(MediaInfoStreamKind kind) => MediaInfo_Count_Get(Handle, kind, -1);

    public string GetGeneral(string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.General,
            0, parameter, MediaInfoKind.Text, MediaInfoKind.Name)) ?? "";
    }

    public string GetVideo(int stream, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.Video,
            stream, parameter, MediaInfoKind.Text, MediaInfoKind.Name)) ?? "";
    }

    public string GetAudio(int stream, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.Audio,
            stream, parameter, MediaInfoKind.Text, MediaInfoKind.Name)) ?? "";
    }

    public string GetText(int stream, string parameter)
    {
        return Marshal.PtrToStringUni(MediaInfo_Get(Handle, MediaInfoStreamKind.Text,
            stream, parameter, MediaInfoKind.Text, MediaInfoKind.Name)) ?? "";
    }

    public string GetSummary(bool complete, bool rawView)
    {
        MediaInfo_Option(Handle, "Language", rawView ? "raw" : "");
        MediaInfo_Option(Handle, "Complete", complete ? "1" : "0");
        return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, 0)) ?? "";
    }

    bool Disposed;

    public void Dispose()
    {
        if (!Disposed)
        {
            if (Handle != IntPtr.Zero)
            {
                MediaInfo_Close(Handle);
                MediaInfo_Delete(Handle);
            }

            Disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    ~MediaInfo() { Dispose(); }

    [DllImport("MediaInfo.dll")]
    static extern IntPtr MediaInfo_New();

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern int MediaInfo_Open(IntPtr handle, string path);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr MediaInfo_Option(IntPtr handle, string option, string value);

    [DllImport("MediaInfo.dll")]
    static extern IntPtr MediaInfo_Inform(IntPtr handle, int reserved);

    [DllImport("MediaInfo.dll")]
    static extern int MediaInfo_Close(IntPtr handle);

    [DllImport("MediaInfo.dll")]
    static extern void MediaInfo_Delete(IntPtr handle);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern IntPtr MediaInfo_Get(IntPtr handle, MediaInfoStreamKind kind,
        int stream, string parameter, MediaInfoKind infoKind, MediaInfoKind searchKind);

    [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
    static extern int MediaInfo_Count_Get(IntPtr handle, MediaInfoStreamKind streamKind, int stream);
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

public enum MediaInfoKind
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
