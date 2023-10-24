
using MpvNet.ExtensionMethod;

namespace MpvNet;

public static class FileTypes
{
    public static string[] Video { get; set; } = "mkv mp4 avi mov flv mpg webm wmv ts vob 264 265 asf avc avs dav h264 h265 hevc m2t m2ts m2v m4v mpeg mpv mts vpy y4m".Split(' ');
    public static string[] Audio { get; set; } = "mp3 flac m4a mka mp2 ogg opus aac ac3 dts dtshd dtshr dtsma eac3 mpa mpc thd w64 wav".Split(' ');
    public static string[] Image { get; set; } = { "jpg", "bmp", "png", "gif", "webp" };
    public static string[] Subtitle { get; } = { "srt", "ass", "idx", "sub", "sup", "ttxt", "txt", "ssa", "smi", "mks" };

    public static bool IsImage(string extension) => Image.Contains(extension);
    public static bool IsAudio(string extension) => Audio.Contains(extension);

    public static bool IsMedia(string extension) =>
        Video.Contains(extension) || Audio.Contains(extension) || Image.Contains(extension);

    public static IEnumerable<string> GetMediaFiles(IEnumerable<string> files) => files.Where(i => IsMedia(i.Ext()));
}
