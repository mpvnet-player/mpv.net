
using System.Windows;
using MpvNet.Windows.WPF;

namespace MpvNet.Windows.Help;

public class WinMpvHelp
{
    public static void Setup()
    {
        if (RegistryHelp.GetString("PathEnvVarCheck") == Folder.Startup ||
            RegistryHelp.GetString("Setup") == Folder.Startup ||
            App.Settings.StartupFolder == Folder.Startup)

            return;

        string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)!;

        if (!path.ToLower().Contains(Folder.Startup.TrimEnd(Path.DirectorySeparatorChar).ToLower()))
        {
            var result = Msg.ShowQuestion("Would you like to add mpv.net to the Path environment variable?" + BR2 +
                "This will allow using mpv.net in a console/terminal.", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                Environment.SetEnvironmentVariable("Path",
                    Folder.Startup.TrimEnd(Path.DirectorySeparatorChar) + ";" + path,
                    EnvironmentVariableTarget.User);

                Msg.ShowInfo("mpv.net was added successfully to Path.");
            }
            else
                Msg.ShowInfo("If you want to add mpv.net to the Path environment variable later," + BR +
                    "you can do so with the context menu (Settings/Setup).");
        }

        var result2 = Msg.ShowQuestion("Would you like to register video file associations?", MessageBoxButton.YesNo);

        if (result2 == MessageBoxResult.Yes)
        {
            Player.Command("script-message-to mpvnet reg-file-assoc video");

            result2 = Msg.ShowQuestion("Would you like to register audio file associations?", MessageBoxButton.YesNo);

            if (result2 == MessageBoxResult.Yes)
                Player.Command("script-message-to mpvnet reg-file-assoc audio");
        }
        else
            Msg.ShowInfo("If you want to register file associations later," + BR +
                "you can do so with the context menu (Settings/Setup).");

        App.Settings.StartupFolder = Folder.Startup;
    }
}
