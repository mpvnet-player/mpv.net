using System.Windows;

namespace mpvnet
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            TextBlock.Text = $"mpv.net\nVersion {System.Windows.Forms.Application.ProductVersion}\nCopyright (c) 2017-2019 Frank Skare (stax76)\nMIT License";
        }
    }
}