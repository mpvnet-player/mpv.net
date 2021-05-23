
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using static mpvnet.Global;

namespace mpvnet
{
    class UpdateCheck
    {
        public static void DailyCheck()
        {
            if (App.UpdateCheck && RegistryHelp.GetInt("last-update-check")
                != DateTime.Now.DayOfYear)

                CheckOnline();
        }

        public static async void CheckOnline(bool showUpToDateMessage = false)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    RegistryHelp.SetValue("last-update-check", DateTime.Now.DayOfYear);
                    client.DefaultRequestHeaders.Add("User-Agent", "mpv.net");
                    var response = await client.GetAsync("https://api.github.com/repos/stax76/mpv.net/releases/latest");
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync();
                    Match match = Regex.Match(content, @"""mpv\.net-([\d\.]+)-portable\.zip""");

                    if (!match.Success)
                    {
                        App.ShowError("Update check is currently not available.");
                        return;
                    }
                    
                    Version onlineVersion = Version.Parse(match.Groups[1].Value);
                    Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                    if (onlineVersion <= currentVersion)
                    {
                        if (showUpToDateMessage)
                            Msg.ShowInfo($"{Application.ProductName} is up to date.");

                        return;
                    }

                    if ((RegistryHelp.GetString("update-check-version")
                        != onlineVersion.ToString() || showUpToDateMessage) && Msg.ShowQuestion(
                            $"New version {onlineVersion} is available, update now?") == DialogResult.OK)
                    {
                        string url = $"https://github.com/stax76/mpv.net/releases/download/{onlineVersion}/mpv.net-{onlineVersion}-portable.zip";

                        using (Process proc = new Process())
                        {
                            proc.StartInfo.UseShellExecute = true;
                            proc.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            proc.StartInfo.FileName = "powershell.exe";
                            proc.StartInfo.Arguments = $"-NoExit -ExecutionPolicy Bypass -File \"{Folder.Startup + "Setup\\update.ps1"}\" \"{url}\" \"{Folder.Startup.TrimEnd(Path.DirectorySeparatorChar)}\"";

                            if (Folder.Startup.Contains("Program Files"))
                                proc.StartInfo.Verb = "runas";

                            proc.Start();
                        }

                        Core.command("quit");
                    }

                    RegistryHelp.SetValue("update-check-version", onlineVersion.ToString());
                }
            }
            catch (Exception ex)
            {
                if (showUpToDateMessage)
                    App.ShowException(ex);
            }
        }
    }
}
