using MpvNet.ExtensionMethod;

namespace MpvNet.Windows;

public class Misc
{
    public static void CopyMpvnetCom()
    {
        string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).AddSep() +
            "Microsoft\\WindowsApps\\";

        if (File.Exists(dir + "MpvNet.exe") && !File.Exists(dir + "MpvNet.com"))
            File.Copy(Folder.Startup + "MpvNet.com", dir + "MpvNet.com");
    }
}
