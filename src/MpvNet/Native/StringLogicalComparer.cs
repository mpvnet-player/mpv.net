
using System.Runtime.InteropServices;
using System.Collections;

namespace MpvNet.Native;

public class StringLogicalComparer : IComparer, IComparer<string>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    static extern int StrCmpLogical(string? x, string? y);

    static int IComparer_Compare(object? x, object? y) => StrCmpLogical(x!.ToString(), y!.ToString());
    static int IComparerOfString_Compare(string? x, string? y) => StrCmpLogical(x, y);

    int IComparer.Compare(object? x, object? y) => IComparer_Compare(x, y);
    int IComparer<string>.Compare(string? x, string? y) => IComparerOfString_Compare(x, y);
}
