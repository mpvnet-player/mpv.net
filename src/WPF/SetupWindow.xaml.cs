
using System;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;

using WinForms = System.Windows.Forms;

using static StockIcon;

namespace mpvnet
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Theme Theme {
            get => Theme.Current;
        }

        static BitmapSource _ShieldIcon;

        public static BitmapSource ShieldIcon {
            get {
                if (_ShieldIcon == null)
                {
                    IntPtr icon = GetIcon(SHSTOCKICONID.Shield, SHSTOCKICONFLAGS.SHGSI_ICON);
                    _ShieldIcon = Imaging.CreateBitmapSourceFromHIcon(
                        icon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    DestroyIcon(icon);
                }
                return _ShieldIcon;
            }
        }

        void RegFileAssoc(string[] extensions)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = WinForms.Application.ExecutablePath;
                    proc.StartInfo.Arguments = "--reg-file-assoc " + String.Join(" ", extensions);
                    proc.StartInfo.Verb = "runas";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                    proc.WaitForExit();

                    if (proc.ExitCode == 0)
                        Msg.ShowInfo("File associations successfully created.");
                    else
                        Msg.ShowError("Error creating file associations.");
                }
            } catch {}
        }

        void AddVideo_Click(object sender, RoutedEventArgs e) => RegFileAssoc(CorePlayer.VideoTypes);
        void AddAudio_Click(object sender, RoutedEventArgs e) => RegFileAssoc(CorePlayer.AudioTypes);
        void AddImage_Click(object sender, RoutedEventArgs e) => RegFileAssoc(CorePlayer.ImageTypes);

        void RemoveFileAssociations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "powershell.exe";
                    proc.StartInfo.Arguments = "-NoLogo -NoExit -NoProfile -ExecutionPolicy Bypass -File \"" +
                        Folder.Startup + "Setup\\remove file associations.ps1\"";
                    proc.StartInfo.Verb = "runas";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
            } catch { }
        }

        void AddToPathEnvVar_Click(object sender, RoutedEventArgs e)
        {
            ExecutePowerShellScript(Folder.Startup + "Setup\\add environment variable.ps1");
        }

        void RemoveFromPathEnvVar_Click(object sender, RoutedEventArgs e)
        {
            ExecutePowerShellScript(Folder.Startup + "Setup\\remove environment variable.ps1");
        }

        void AddStartMenuShortcut_Click(object sender, RoutedEventArgs e)
        {
            ExecutePowerShellScript(Folder.Startup + "Setup\\create start menu shortcut.ps1");
        }

        void RemoveStartMenuShortcut_Click(object sender, RoutedEventArgs e)
        {
            ExecutePowerShellScript(Folder.Startup + "Setup\\remove start menu shortcut.ps1");
        }

        void ShowEnvVarEditor_Click(object sender, RoutedEventArgs e)
        {
            ProcessHelp.Execute("rundll32.exe", "sysdm.cpl,EditEnvironmentVariables");
        }

        void ExecutePowerShellScript(string file)
        {
            ProcessHelp.Execute("powershell.exe",
                "-NoLogo -NoExit -NoProfile -ExecutionPolicy Bypass -File \"" + file + "\"");
        }

        void EditDefaultApp_Click(object sender, RoutedEventArgs e)
        {
            ProcessHelp.ShellExecute("ms-settings:defaultapps");
        }
    }
}
