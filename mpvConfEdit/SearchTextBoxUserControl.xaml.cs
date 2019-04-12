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

        private void SearchClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Keyboard.Focus(SearchTextBox);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HintTextBlock.Text = SearchTextBox.Text == "" ? "Find a setting" : "";

            if (SearchTextBox.Text == "")
                SearchClearButton.Visibility = Visibility.Hidden;
            else
                SearchClearButton.Visibility = Visibility.Visible;
        }
    }
}