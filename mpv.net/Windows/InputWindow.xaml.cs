using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace mpvnet
{
    public partial class InputWindow : Window
    {
        ICollectionView CollectionView;

        public InputWindow()
        {
            InitializeComponent();
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            DataGrid.SelectionMode = DataGridSelectionMode.Single;
            CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = InputItem.InputItems };
            CollectionView = collectionViewSource.View;
            var yourCostumFilter = new Predicate<object>(item => Filter((InputItem)item));
            CollectionView.Filter = yourCostumFilter;
            DataGrid.ItemsSource = CollectionView;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView.Refresh();

            if (SearchControl.SearchTextBox.Text == "?")
                MessageBox.Show("Filtering works by searching in the Input, Menu and Command but it's possible to reduce the filter scope to either of Input, Menu or Command by prefixing as follows:\n\ni <input search>\ni: <input search>\n\nm <menu search>\nm: <menu search>\n\nc <command search>\nc: <command search>\n\nIf only one character is entered the search will be performed only in the input.", "Filtering", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        bool Filter(InputItem item)
        {
            string searchText = SearchControl.SearchTextBox.Text.ToLower();
            if (searchText == "") return true;

            if (searchText.StartsWith("i ") || searchText.StartsWith("i:") || searchText.Length == 1)
            {
                if (searchText.Length > 1)
                    searchText = searchText.Substring(2).Trim();

                if (searchText.Length < 3)
                    return item.Input.ToLower().Replace("ctrl+", "").Replace("shift+", "").Replace("alt+", "").Contains(searchText);
                else
                    return item.Input.ToLower().Contains(searchText);
            }
            else if (searchText.StartsWith("m ") || searchText.StartsWith("m:"))
                return item.Menu.ToLower().Contains(searchText.Substring(2).Trim());
            else if (searchText.StartsWith("c ") || searchText.StartsWith("c:"))
                return item.Command.ToLower().Contains(searchText.Substring(2).Trim());
            else if (item.Command.ToLower().Contains(searchText) ||
                item.Menu.ToLower().Contains(searchText) ||
                item.Input.ToLower().Contains(searchText))
            {
                return true;
            }
            return false;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            InputItem item = ((Button)e.Source).DataContext as InputItem;
            if (item is null) return;
            LearnWindow w = new LearnWindow();
            w.Owner = this;
            w.InputItem = item;
            w.ShowDialog();

            var items = new Dictionary<string, InputItem>();

            foreach (InputItem i in InputItem.InputItems)
                if (items.ContainsKey(i.Input) && i.Input != "_")
                    MessageBox.Show($"Duplicate found:\n\n{i.Input}: {i.Menu}\n\n{items[i.Input].Input}: {items[i.Input].Menu}\n\nPlease note that you can chain multiple commands in the same line by using a semicolon as separator.", "Duplicate Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    items[i.Input] = i;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => Keyboard.Focus(SearchControl.SearchTextBox);

        private void Window_Closed(object sender, EventArgs e)
        {
            string backupDir = Path.GetDirectoryName(mp.InputConfPath) + "\\backup\\";

            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            if (File.Exists(mp.InputConfPath))
                File.Copy(mp.InputConfPath, backupDir + "input conf " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".conf", true);

            string text = Properties.Resources.inputConfHeader + "\r\n";

            foreach (InputItem item in InputItem.InputItems)
            {
                string line = " " + item.Input.PadRight(10);

                if (item.Command.Trim() == "")
                    line += " ignore";
                else
                    line += " " + item.Command.Trim();

                if (item.Menu.Trim() != "")
                    line = line.PadRight(40) + " #menu: " + item.Menu;

                text += line + "\r\n";
            }

            File.WriteAllText(mp.InputConfPath, text);

            MessageBox.Show("Changes will be available on next mpv.net startup.",
                Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DataGrid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            if (e.Command == DataGrid.DeleteCommand)
                if (MessageBox.Show($"Confirm to delete: {(grid.SelectedItem as InputItem).Input} ({(grid.SelectedItem as InputItem).Menu})", "Confirm Delete", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                    e.Handled = true;
        }
    }
}