
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace mpvnet
{
    public partial class SearchTextBoxUserControl : UserControl
    {
        public bool HideClearButton { get; set; }

        public SearchTextBoxUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Theme Theme => Theme.Current;

        public string Text {
            get => SearchTextBox.Text;
            set => SearchTextBox.Text = value;
        }

        string _HintText;

        public string HintText {
            get => _HintText;
            set {
                _HintText = value;
                UpdateControls();
            }
        }

        void SearchClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Keyboard.Focus(SearchTextBox);
        }

        void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateControls();
        }

        void UpdateControls()
        {
            HintTextBlock.Text = SearchTextBox.Text == "" ? HintText : "";

            if (SearchTextBox.Text == "" || HideClearButton)
                SearchClearButton.Visibility = Visibility.Hidden;
            else
                SearchClearButton.Visibility = Visibility.Visible;
        }
    }
}
