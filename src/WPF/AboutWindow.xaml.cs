
using System.Windows;
using System.Windows.Input;

namespace mpvnet
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            DataContext = this;
            ContentBlock.Text = App.Version;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) => Close();
        protected override void OnMouseDown(MouseButtonEventArgs e) => Close();

        public Theme Theme {
            get => Theme.Current;
        }
    }
}
