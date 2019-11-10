
using System;
using System.Diagnostics;
using System.Windows;

using WinForms = System.Windows.Forms;

namespace mpvnet
{
    public partial class SetupWindow : Window
    {
        public SetupWindow() => InitializeComponent();

        void RegisterFileAssociations(string value)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = WinForms.Application.ExecutablePath;
                    proc.StartInfo.Arguments = "--reg-file-assoc " + value;
                    proc.StartInfo.Verb = "runas";
                    proc.Start();
                }

                Process.Start("ms-settings:defaultapps");
            } catch {}
        }

        private void RegisterVideo_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("video");
        private void RegisterAudio_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("audio");
        private void RegisterImage_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("image");

        private void UnregisterFileAssociations_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("unreg");

        private void AddToPathEnvVar_Click(object sender, RoutedEventArgs e)
        {
            string var = WinForms.Application.StartupPath + ";";
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            if (path.Contains(var))
                Msg.ShowWarning("Path was already containing mpv.net.");
            else
            {
                Environment.SetEnvironmentVariable("Path", var + path, EnvironmentVariableTarget.User);
                Msg.Show("mpv.net was successfully added to Path.", (var + path).Replace(";","\n"));
            }
        }

        private void RemoveFromPathEnvVar_Click(object sender, RoutedEventArgs e)
        {
            string var = WinForms.Application.StartupPath + ";";
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            if (path.Contains(var))
            {
                Environment.SetEnvironmentVariable("Path", path.Replace(var, ""), EnvironmentVariableTarget.User);
                Msg.Show("mpv.net was successfully removed from Path.");
            }
            else
                Msg.ShowWarning("Path was not containing mpv.net.");
        }
    }
}