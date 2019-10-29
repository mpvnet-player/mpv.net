using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls
{
    public partial class SearchTextBoxUserControl : UserControl
    {
        public SearchTextBoxUserControl()
        {
            InitializeComponent();
        }

        public string Text { get => SearchTextBox.Text; set => SearchTextBox.Text = value; }

        private string _HintText;

        public string HintText {
            get => _HintText;
            set {
                _HintText = value;
                UpdateControls();
            }
        }

        private void SearchClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Keyboard.Focus(SearchTextBox);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateControls();
        }

        void UpdateControls()
        {
            HintTextBlock.Text = SearchTextBox.Text == "" ? HintText : "";

            if (SearchTextBox.Text == "")
                SearchClearButton.Visibility = Visibility.Hidden;
            else
                SearchClearButton.Visibility = Visibility.Visible;
        }
    }
}