
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using static mpvnet.Global;

namespace mpvnet
{
    public partial class InputWindow : Window
    {
        ICollectionView CollectionView;
        string InitialInputConfContent;

        public InputWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitialInputConfContent = GetInputConfContent();
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            DataGrid.SelectionMode = DataGridSelectionMode.Single;
            CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = CommandItem.Items };
            CollectionView = collectionViewSource.View;
            CollectionView.Filter = new Predicate<object>(item => Filter((CommandItem)item));
            DataGrid.ItemsSource = CollectionView;
        }

        public Theme Theme => Theme.Current;

        void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView.Refresh();

            if (SearchControl.SearchTextBox.Text == "?")
            {
                SearchControl.SearchTextBox.Text = "";

                Msg.ShowInfo("Filtering" + BR2 +
                    "Reduce the filter scope with:" + BR2 +
                    "i input" + BR2 +
                    "m menu" + BR2 +
                    "c command" + BR2 +
                    "If only one character is entered input search is performed.");
            }
        }

        bool Filter(CommandItem item)
        {
            if (item.Command == "")
                return false;

            string searchText = SearchControl.SearchTextBox.Text.ToLower();

            if (searchText == "" || searchText == "?")
                return true;

            if (searchText.Length == 1)
                return item.Input.ToLower().Replace("ctrl+", "").Replace("shift+", "").Replace("alt+", "") == searchText.ToLower();
            else if (searchText.StartsWith("i ") || searchText.StartsWith("i:") || searchText.Length == 1)
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

        void ButtonClick(object sender, RoutedEventArgs e)
        {
            CommandItem item = ((Button)e.Source).DataContext as CommandItem;

            if (item is null)
                return;

            LearnWindow w = new LearnWindow();
            w.Owner = this;
            w.InputItem = item;
            w.ShowDialog();

            var items = new Dictionary<string, CommandItem>();

            foreach (CommandItem i in CommandItem.Items)
                items[i.Input] = i;
        }

        void Window_Loaded(object sender, RoutedEventArgs e) => Keyboard.Focus(SearchControl.SearchTextBox);

        string GetInputConfContent()
        {
            string text = null;

            foreach (string line in Properties.Resources.input_conf.Split(new[] { "\r\n" }, StringSplitOptions.None))
            {
                string test = line.Trim();

                if (test == "" || test.StartsWith("#"))
                    text += test + BR;
            }

            text = BR + text.Trim() + BR2;

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

                text += line + BR;
            }

            return text;
        }

        void Window_Closed(object sender, EventArgs e)
        {
            if (InitialInputConfContent == GetInputConfContent())
                return;

            File.WriteAllText(Core.InputConfPath, GetInputConfContent());
            Msg.ShowInfo("Changes will be available on next startup.");
        }

        void DataGrid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            if (e.Command == DataGrid.DeleteCommand)
                if (Msg.ShowQuestion($"Confirm to delete: {(grid.SelectedItem as CommandItem).Input} ({(grid.SelectedItem as CommandItem).Path})") != MessageBoxResult.OK)
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
