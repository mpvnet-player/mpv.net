using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mpvnet
{
    public class Misc
    {
        public static readonly string[] FileTypes = "264 265 3gp aac ac3 avc avi avs bmp divx dts dtshd dtshr dtsma eac3 evo flac flv h264 h265 hevc hvc jpg jpeg m2t m2ts m2v m4a m4v mka mkv mlp mov mp2 mp3 mp4 mpa mpeg mpg mpv mts ogg ogm opus pcm png pva raw rmvb thd thd+ac3 true-hd truehd ts vdr vob vpy w64 wav webm wmv y4m".Split(' ');

        public static string GetFilter(IEnumerable<string> values) => "*." + 
            String.Join(";*.", values) + "|*." + String.Join(";*.", values) + "|All Files|*.*";
    }

    public class StringLogicalComparer : IComparer, IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogical(string x, string y);

        int IComparer_Compare(object x, object y) => StrCmpLogical(x.ToString(), y.ToString());
        int IComparer.Compare(object x, object y) => IComparer_Compare(x, y);
        int IComparerOfString_Compare(string x, string y) => StrCmpLogical(x, y);
        int IComparer<string>.Compare(string x, string y) => IComparerOfString_Compare(x, y);
    }

    public class OSVersion
    {
        public static float Windows7 { get; } = 6.1f;
        public static float Windows8 { get; } = 6.2f;
        public static float Windows81 { get; } = 6.3f;
        public static float Windows10 { get; } = 10f;

        public static float Current => Environment.OSVersion.Version.Major + Environment.OSVersion.Version.Minor / 10f;
    }
}