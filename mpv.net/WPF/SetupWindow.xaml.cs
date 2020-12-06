
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

using WinForms = System.Windows.Forms;

using static StockIcon;

namespace mpvnet
{
    public partial class SetupWindow : Window
    {
        public SetupWindow() => InitializeComponent();

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

        void RegisterVideo_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("video");
        void RegisterAudio_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("audio");
        void RegisterImage_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("image");

        void UnregisterFileAssociations_Click(object sender, RoutedEventArgs e) => RegisterFileAssociations("unreg");

        void AddToPathEnvVar_Click(object sender, RoutedEventArgs e)
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

        void RemoveFromPathEnvVar_Click(object sender, RoutedEventArgs e)
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

        void aaa()
        {
            BitmapSource shieldSource = null;
            IntPtr icon = GetIcon(SHSTOCKICONID.Shield, SHSTOCKICONFLAGS.SHGSI_LARGEICON);
            shieldSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DestroyIcon(icon);
            //shieldSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            //    System.Drawing.SystemIcons.Shield.Handle,
            //    Int32Rect.Empty,
            //    BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
