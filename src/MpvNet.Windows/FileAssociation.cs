
using Microsoft.Win32;

using MpvNet.Windows.Help;

namespace MpvNet.Windows;

public static class FileAssociation
{
    public static void Register(string perceivedType, string[] extensions)
    {
        string exePath = Environment.ProcessPath!;
        string exeFilename = Path.GetFileName(exePath);
        string exeFilenameNoExt = Path.GetFileNameWithoutExtension(exePath);

        string[] protocols = { "ytdl", "rtsp", "srt", "srtp" };

        if (perceivedType != "unreg")
        {
            foreach (string it in protocols)
            {
                RegistryHelp.SetValue($@"HKCR\{it}", $"{it.ToUpper()} Protocol", "");
                RegistryHelp.SetValue($@"HKCR\{it}\shell\open\command", "", $"\"{exePath}\" \"%1\"");
            }

            RegistryHelp.SetValue(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + exeFilename, "", exePath);
            RegistryHelp.SetValue(@"HKCR\Applications\" + exeFilename, "FriendlyAppName", "mpv.net media player");
            RegistryHelp.SetValue(@"HKCR\Applications\" + exeFilename + @"\shell\open\command", "", $"\"{exePath}\" \"%1\"");
            RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\video\OpenWithList\" + exeFilename, "", "");
            RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + exeFilename, "", "");
            RegistryHelp.SetValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net", @"SOFTWARE\Clients\Media\mpv.net\Capabilities");
            RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationDescription", "mpv.net media player");
            RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationName", "mpv.net");

            foreach (string ext in extensions)
            {
                RegistryHelp.SetValue(@"HKCR\Applications\" + exeFilename + @"\SupportedTypes", "." + ext, "");
                RegistryHelp.SetValue(@"HKCR\" + "." + ext, "", exeFilenameNoExt + "." + ext);
                RegistryHelp.SetValue(@"HKCR\" + "." + ext + @"\OpenWithProgIDs", exeFilenameNoExt + "." + ext, "");
                RegistryHelp.SetValue(@"HKCR\" + "." + ext, "PerceivedType", perceivedType);
                RegistryHelp.SetValue(@"HKCR\" + exeFilenameNoExt + "." + ext + @"\shell\open\command", "", $"\"{exePath}\" \"%1\"");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities\FileAssociations", "." + ext, exeFilenameNoExt + "." + ext);
            }
        }
        else
        {
            foreach (string i in protocols)
                RegistryHelp.RemoveKey($@"HKCR\{i}");

            RegistryHelp.RemoveKey(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + exeFilename);
            RegistryHelp.RemoveKey(@"HKCR\Applications\" + exeFilename);
            RegistryHelp.RemoveKey(@"HKLM\SOFTWARE\Clients\Media\mpv.net");
            RegistryHelp.RemoveKey(@"HKCR\SystemFileAssociations\video\OpenWithList\" + exeFilename);
            RegistryHelp.RemoveKey(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + exeFilename);

            RegistryHelp.RemoveValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net");

            foreach (string id in Registry.ClassesRoot.GetSubKeyNames())
            {
                if (id.StartsWith(exeFilenameNoExt + "."))
                    Registry.ClassesRoot.DeleteSubKeyTree(id);

                RegistryHelp.RemoveValue($@"HKCR\Software\Classes\{id}\OpenWithProgIDs", exeFilenameNoExt + id);
                RegistryHelp.RemoveValue($@"HKLM\Software\Classes\{id}\OpenWithProgIDs", exeFilenameNoExt + id);
            }
        }
    }
}
