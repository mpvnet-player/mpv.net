
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace mpvnet
{
    class UpdateCheck
    {
        public static void DailyCheck()
        {
            if (App.UpdateCheck && RegistryHelp.GetInt(RegistryHelp.ApplicationKey, "UpdateCheckLast")
                != DateTime.Now.DayOfYear)

                CheckOnline();
        }

        public static async void CheckOnline(bool showUpToDateMessage = false)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    RegistryHelp.SetValue(RegistryHelp.ApplicationKey, "UpdateCheckLast", DateTime.Now.DayOfYear);
                    client.DefaultRequestHeaders.Add("User-Agent", "mpv.net");
                    var response = await client.GetAsync("https://api.github.com/repos/stax76/mpv.net/releases/latest");
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    Match match = Regex.Match(content, @"""mpv\.net-portable-x64-([\d\.]+)\.7z""");
                    Version onlineVersion = Version.Parse(match.Groups[1].Value);
                    Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                    if (onlineVersion == currentVersion)
                    {
                        if (showUpToDateMessage)
                            Msg.Show($"{Application.ProductName} is up to date.");
                        return;
                    }

                    if (RegistryHelp.GetString(RegistryHelp.ApplicationKey, "UpdateCheckVersion")
                        != onlineVersion.ToString() && Msg.ShowQuestion(
                            $"New version {onlineVersion} is available, update now?") == MsgResult.OK)
                    {
                        string arch = IntPtr.Size == 8 ? "64" : "86";
                        string url = $"https://github.com/stax76/mpv.net/releases/download/{onlineVersion}/mpv.net-portable-x{arch}-{onlineVersion}.7z";

                        using (Process proc = new Process())
                        {
                            proc.StartInfo.UseShellExecute = true;
                            proc.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            proc.StartInfo.FileName = "PowerShell";
                            proc.StartInfo.Arguments = $"-NoLogo -NoExit -File \"{Folder.Startup + "Update.ps1"}\" \"{url}\" \"{Application.StartupPath.TrimEnd('\\')}\"";

                            if (Folder.Startup.Contains("Program Files"))
                                proc.StartInfo.Verb = "runas";

                            proc.Start();
                        }

                        mp.command("quit");
                    }

                    RegistryHelp.SetValue(RegistryHelp.ApplicationKey, "UpdateCheckVersion", onlineVersion.ToString());
                }
            }
            catch (Exception ex)
            {
                if (showUpToDateMessage)
                    Msg.ShowException(ex);
            }
        }
    }
}