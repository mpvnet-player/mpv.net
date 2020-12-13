
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
            ContentBlock.Text= "Copyright (C) 2017-2020 Frank Skare (stax76)\n" +
                $"mpv.net {System.Windows.Forms.Application.ProductVersion} ({File.GetLastWriteTime(System.Windows.Forms.Application.ExecutablePath).ToShortDateString()})\n" +
                $"{core.get_property_string("mpv-version")} ({File.GetLastWriteTime(Folder.Startup + "mpv-1.dll").ToShortDateString()})\nffmpeg {core.get_property_string("ffmpeg-version")}\nMIT License";
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) => Close();
        protected override void OnMouseDown(MouseButtonEventArgs e) => Close();
    }
}
