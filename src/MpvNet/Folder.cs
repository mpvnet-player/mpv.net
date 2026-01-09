
using MpvNet.Extensions;

namespace MpvNet;

public class Folder
{
    public static string Startup { get; } = Path.GetDirectoryName(Environment.ProcessPath)!.Separator;
    public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Separator;
}
