
using MpvNet.ExtensionMethod;

namespace MpvNet.Windows.Help;

public class WinMpvHelp
{
    public static void CopyMpvNetCom()
    {
        string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).AddSep() +
            "Microsoft\\WindowsApps\\";

        if (File.Exists(dir + "mpvnet.exe") && !File.Exists(dir + "mpvnet.com"))
            File.Copy(Folder.Startup + "mpvnet.com", dir + "mpvnet.com");
    }
}
