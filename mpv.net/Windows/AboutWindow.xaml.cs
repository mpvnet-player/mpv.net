using System.Windows;
using System.Windows.Input;

namespace mpvnet
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            Version.Text = $"Version {System.Windows.Forms.Application.ProductVersion}";
            Foreground = WPF.WPF.ThemeBrush;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) => Close();
        protected override void OnMouseDown(MouseButtonEventArgs e) => Close();
    }
}