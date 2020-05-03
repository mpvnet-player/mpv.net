
using System.IO;
using System.Windows;
using System.Windows.Input;

using static mpvnet.Core;

namespace mpvnet
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            Version.Text = $"mpv.net Version {System.Windows.Forms.Application.ProductVersion} ({File.GetLastWriteTime(System.Windows.Forms.Application.ExecutablePath).ToShortDateString()})";
            mpvVersion.Text = $"{core.get_property_string("mpv-version")} ({File.GetLastWriteTime(Folder.Startup + "mpv-1.dll").ToShortDateString()})";
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) => Close();
        protected override void OnMouseDown(MouseButtonEventArgs e) => Close();
    }
}