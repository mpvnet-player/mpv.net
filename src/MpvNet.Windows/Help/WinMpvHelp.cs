
using System.Windows;

using MpvNet.Windows.WPF;

namespace MpvNet.Windows.Help;

public class WinMpvHelp
{
    public static void AddToPath()
    {
        string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)!;

        if (path.ToLower().Contains(Folder.Startup.TrimEnd(Path.DirectorySeparatorChar).ToLower()))
            return;

        string dir = RegistryHelp.GetString("PathEnvVarCheck");

        if (dir == Folder.Startup)
            return;

        var result = Msg.ShowQuestion("Would you like to add mpv.net to the Path environment variable?",
            MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            Environment.SetEnvironmentVariable("Path",
                Folder.Startup.TrimEnd(Path.DirectorySeparatorChar) + ";" + path,
                EnvironmentVariableTarget.User);

            Msg.ShowInfo("mpv.net was added successfully to Path.");
        }

        RegistryHelp.SetString("PathEnvVarCheck", Folder.Startup);
    }
}
