
using System.IO;
using System.Windows.Forms;

using Microsoft.Win32;

namespace mpvnet
{
    public class FileAssociation
    {
        static string ExePath = Application.ExecutablePath;
        static string ExeFilename = Path.GetFileName(Application.ExecutablePath);
        static string ExeFilenameNoExt = Path.GetFileNameWithoutExtension(Application.ExecutablePath);

        public static void Register(string perceivedType, string[] extensions)
        {
            string[] protocols = { "ytdl", "rtsp", "srt", "srtp" };

            if (perceivedType != "unreg")
            {
                foreach (string i in protocols)
                {
                    RegistryHelp.SetValue($@"HKCR\{i}", $"{i.ToUpper()} Protocol", "");
                    RegistryHelp.SetValue($@"HKCR\{i}\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
                }

                RegistryHelp.SetValue(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename, null, ExePath);
                RegistryHelp.SetValue(@"HKCR\Applications\" + ExeFilename, "FriendlyAppName", "mpv.net media player");
                RegistryHelp.SetValue(@"HKCR\Applications\" + ExeFilename + @"\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
                RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\video\OpenWithList\" + ExeFilename, null, "");
                RegistryHelp.SetValue(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + ExeFilename, null, "");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net", @"SOFTWARE\Clients\Media\mpv.net\Capabilities");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationDescription", "mpv.net media player");
                RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities", "ApplicationName", "mpv.net");

                foreach (string ext in extensions)
                {
                    RegistryHelp.SetValue(@"HKCR\Applications\" + ExeFilename + @"\SupportedTypes", "." + ext, "");
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, null, ExeFilenameNoExt + "." + ext);
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext + @"\OpenWithProgIDs", ExeFilenameNoExt + "." + ext, "");
                    RegistryHelp.SetValue(@"HKCR\" + "." + ext, "PerceivedType", perceivedType);
                    RegistryHelp.SetValue(@"HKCR\" + ExeFilenameNoExt + "." + ext + @"\shell\open\command", null, $"\"{ExePath}\" \"%1\"");
                    RegistryHelp.SetValue(@"HKLM\SOFTWARE\Clients\Media\mpv.net\Capabilities\FileAssociations", "." + ext, ExeFilenameNoExt + "." + ext);
                }
            }
            else
            {
                foreach (string i in protocols)
                    RegistryHelp.RemoveKey($@"HKCR\{i}");

                RegistryHelp.RemoveKey(@"HKCU\Software\Microsoft\Windows\CurrentVersion\App Paths\" + ExeFilename);
                RegistryHelp.RemoveKey(@"HKCR\Applications\" + ExeFilename);
                RegistryHelp.RemoveKey(@"HKLM\SOFTWARE\Clients\Media\mpv.net");
                RegistryHelp.RemoveKey(@"HKCR\SystemFileAssociations\video\OpenWithList\" + ExeFilename);
                RegistryHelp.RemoveKey(@"HKCR\SystemFileAssociations\audio\OpenWithList\" + ExeFilename);
           
                RegistryHelp.RemoveValue(@"HKLM\SOFTWARE\RegisteredApplications", "mpv.net");

                foreach (string id in Registry.ClassesRoot.GetSubKeyNames())
                {
                    if (id.StartsWith(ExeFilenameNoExt + "."))
                        Registry.ClassesRoot.DeleteSubKeyTree(id);

                    RegistryHelp.RemoveValue($@"HKCR\Software\Classes\{id}\OpenWithProgIDs", ExeFilenameNoExt + id);
                    RegistryHelp.RemoveValue($@"HKLM\Software\Classes\{id}\OpenWithProgIDs", ExeFilenameNoExt + id);
                }
            }
        }
    }
}
