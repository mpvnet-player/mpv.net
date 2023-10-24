
using MpvNet.ExtensionMethod;

namespace MpvNet;

public class Folder
{
    public static string Startup { get; } = Path.GetDirectoryName(Environment.ProcessPath)!.AddSep();
    public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).AddSep();
}
