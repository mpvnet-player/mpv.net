using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace mpvnet
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            Version.Text = $"mpv.net Version {System.Windows.Forms.Application.ProductVersion}";
            mpvVersion.Text = $"{mp.get_property_string("mpv-version")} ({File.GetLastWriteTime(PathHelp.StartupPath + "mpv-1.dll").ToShortDateString()})";
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) => Close();
        protected override void OnMouseDown(MouseButtonEventArgs e) => Close();
    }
}