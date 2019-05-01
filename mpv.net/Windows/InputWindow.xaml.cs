using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Sys;

namespace mpvnet
{
    public partial class InputWindow : Window
    {
        ICollectionView CollectionView;
        string InitialInputConfContent;

        public InputWindow()
        {
            InitializeComponent();
            InitialInputConfContent = GetInputConfContent();
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            DataGrid.SelectionMode = DataGridSelectionMode.Single;
            CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = CommandItem.Items };
            CollectionView = collectionViewSource.View;
            var yourCostumFilter = new Predicate<object>(item => Filter((CommandItem)item));
            CollectionView.Filter = yourCostumFilter;
            DataGrid.ItemsSource = CollectionView;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView.Refresh();

            if (SearchControl.SearchTextBox.Text == "?")
                Msg.Show("Filtering works by searching in the Input, Menu and Command but it's possible to reduce the filter scope to either of Input, Menu or Command by prefixing as follows:\n\ni <input search>\ni: <input search>\n\nm <menu search>\nm: <menu search>\n\nc <command search>\nc: <command search>\n\nIf only one character is entered the search will be performed only in the input.", "Filtering");
        }

        bool Filter(CommandItem item)
        {
            if (item.Command == "") return false;
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
                return item.Path.ToLower().Contains(searchText.Substring(2).Trim());
            else if (searchText.StartsWith("c ") || searchText.StartsWith("c:"))
                return item.Command.ToLower().Contains(searchText.Substring(2).Trim());
            else if (item.Command.ToLower().Contains(searchText) ||
                item.Path.ToLower().Contains(searchText) ||
                item.Input.ToLower().Contains(searchText))
            {
                return true;
            }
            return false;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            CommandItem item = ((Button)e.Source).DataContext as CommandItem;
            if (item is null) return;
            LearnWindow w = new LearnWindow();
            w.Owner = this;
            w.InputItem = item;
            w.ShowDialog();

            var items = new Dictionary<string, CommandItem>();

            foreach (CommandItem i in CommandItem.Items)
                if (items.ContainsKey(i.Input) && i.Input != "")
                    Msg.Show($"Duplicate found:\n\n{i.Input}: {i.Path}\n\n{items[i.Input].Input}: {items[i.Input].Path}\n\nPlease note that you can chain multiple commands in the same line by using a semicolon as separator.", "Duplicate Found");
                else
                    items[i.Input] = i;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => Keyboard.Focus(SearchControl.SearchTextBox);

        string GetInputConfContent()
        {
            string text = Properties.Resources.inputConfHeader + "\r\n";

            foreach (CommandItem item in CommandItem.Items)
            {
                string input = item.Input == "" ? "_" : item.Input;
                string line = " " + input.PadRight(10);

                if (item.Command.Trim() == "")
                    line += " ignore";
                else
                    line += " " + item.Command.Trim();

                if (item.Path.Trim() != "")
                    line = line.PadRight(40) + " #menu: " + item.Path;

                text += line + "\r\n";
            }
            return text;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (InitialInputConfContent == GetInputConfContent()) return;
            File.WriteAllText(mp.InputConfPath, GetInputConfContent());
            Msg.Show("Changes will be available on next mpv.net startup.");
        }

        private void DataGrid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            if (e.Command == DataGrid.DeleteCommand)
                if (Msg.ShowQuestion($"Confirm to delete: {(grid.SelectedItem as CommandItem).Input} ({(grid.SelectedItem as CommandItem).Path})") != MsgResult.OK)
                    e.Handled = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
                Close();
        }
    }
}