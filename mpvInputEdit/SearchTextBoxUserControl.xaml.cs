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
            SearchHintTextBlock.Text = SearchTextBox.Text == "" ? "Type ? to get help." : "";

            if (SearchTextBox.Text == "")
                SearchClearButton.Visibility = Visibility.Hidden;
            else
                SearchClearButton.Visibility = Visibility.Visible;

            if (SearchTextBox.Text == "?")
                MessageBox.Show("Filtering works by searching in the Input, Menu and Command but it's possible to reduce the filter scope to either of Input, Menu or Command by prefixing as follows:\n\ni <input search>\ni: <input search>\n\nm <menu search>\nm: <menu search>\n\nc <command search>\nc: <command search>", "Filtering", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}