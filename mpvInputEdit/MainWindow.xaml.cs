using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace mpvInputEdit
{
    public partial class MainWindow : Window
    {
        ICollectionView CollectionView;

        public MainWindow()
        {
            InitializeComponent();
            Title = (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            DataGrid.SelectionMode = DataGridSelectionMode.Single;
            CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = App.InputItems };
            CollectionView = collectionViewSource.View;
            var yourCostumFilter = new Predicate<object>(item => Filter((InputItem)item));
            CollectionView.Filter = yourCostumFilter;
            DataGrid.ItemsSource = CollectionView;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView.Refresh();
        }

        bool Filter(InputItem item)
        {
            string searchText = SearchControl.SearchTextBox.Text.ToLowerInvariant();

            if (searchText == "")
                return true;

            if (item.Command.ToLower().Contains(searchText) ||
                item.Menu.ToLower().Contains(searchText) ||
                item.Key.ToLower().Contains(searchText))
            {
                return true;
            }
            return false;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            InputItem item = ((Button)e.Source).DataContext as InputItem;
            if (item is null) return;
            InputWindow w = new InputWindow();
            w.Owner = this;
            w.InputItem = item;
            w.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SearchControl.SearchTextBox);
        }

        private void Grid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            if (e.Command == DataGrid.DeleteCommand)
            {
                if (MessageBox.Show($"Would you like to delete the selected item?\n\n{(grid.SelectedItem as InputItem).Menu}",
                    "Confirm Delete", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    e.Handled = true;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you like to save changes?", "Confirm Save", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;

            var backupDir = Path.GetDirectoryName(App.InputConfPath) + "\\backup\\";

            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            File.Copy(App.InputConfPath, backupDir + "input conf " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".conf");

            string text = "";

            foreach (InputItem item in App.InputItems)
            {
                string line = " " + item.Key.PadRight(14);

                if (item.Command.Trim() == "")
                    line += " ignore";
                else
                    line += " " + item.Command.Trim();

                if (item.Menu.Trim() != "")
                    line = line.PadRight(40) + " #menu: " + item.Menu;

                text += line + "\r\n";
            }

            File.WriteAllText(App.InputConfPath, text);

            foreach (Process process in Process.GetProcesses())
                if (process.ProcessName == "mpvnet")
                    MessageBox.Show("Restart mpv.net in order to apply changed input bindings.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                else if (process.ProcessName == "mpv")
                    MessageBox.Show("Restart mpv in order to apply changed input bindings.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}