
using MpvNet.Extensions;

namespace MpvNet;

public static class FileTypes
{
    public static string[] Subtitle { get; } = ["srt", "ass", "idx", "sub", "sup", "ttxt", "txt", "ssa", "smi", "mks"];

    public static bool IsVideo(string[] exts, string ext) => exts?.Contains(ext) ?? false;
    public static bool IsAudio(string[] exts, string ext) => exts?.Contains(ext) ?? false;
    public static bool IsImage(string[] exts, string ext) => exts?.Contains(ext) ?? false;

    public static bool IsVideo(string ext) => GetVideoExts().Contains(ext);
    public static bool IsAudio(string ext) => GetAudioExts().Contains(ext);
    public static bool IsImage(string ext) => GetImgExts().Contains(ext);

    public static string[] GetVideoExts()
    {
        string exts = Player.GetPropertyString("video-exts");

        if (string.IsNullOrEmpty(exts))
            return ["mkv", "mp4", "avi", "mov", "flv", "mpg", "webm", "wmv", "ts", "vob", "264", "265", "asf", "avc", "avs", "dav", "h264", "h265", "hevc", "m2t", "m2ts", "m2v", "m4v", "mpeg", "mpv", "mts", "vpy", "y4m"];

        return exts.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] GetAudioExts()
    {
        string exts = Player.GetPropertyString("audio-exts");

        if (string.IsNullOrEmpty(exts))
            return ["mp3", "flac", "m4a", "mka", "mp2", "ogg", "opus", "aac", "ac3", "dts", "dtshd", "dtshr", "dtsma", "eac3", "mpa", "mpc", "thd", "w64", "wav"];

        return exts.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] GetImgExts()
    {
        string exts = Player.GetPropertyString("image-exts");

        if (string.IsNullOrEmpty(exts))
            return ["jpg", "bmp", "png", "gif", "webp"];

        return exts.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    public static bool IsMedia(string[] exts, string ext) =>
        IsVideo(exts, ext) || IsAudio(exts, ext) || IsImage(exts, ext);

    public static IEnumerable<string> GetMediaFiles(string[] files) =>
        files.Where(i => IsMedia(files, i.Ext));
}
