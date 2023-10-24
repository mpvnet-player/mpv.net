
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MpvNet.Windows.UI;

namespace MpvNet.Windows.WPF;

public partial class InputWindow : Window
{
    ICollectionView CollectionView;
    string StartupContent;
    public List<Binding> Bindings { get; }
    public Theme? Theme => Theme.Current;

    public InputWindow()
    {
        InitializeComponent();
        DataContext = this;

        if (App.InputConf.HasMenu)
            Bindings = InputHelp.Parse(App.InputConf.Content);
        else
            Bindings = InputHelp.GetEditorBindings(App.InputConf.Content);

        StartupContent = InputHelp.ConvertToString(Bindings);
        SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        DataGrid.SelectionMode = DataGridSelectionMode.Single;
        CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = Bindings };
        CollectionView = collectionViewSource.View;
        CollectionView.Filter = new Predicate<object>(item => Filter((Binding)item));
        DataGrid.ItemsSource = CollectionView;
    }

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

    bool Filter(Binding item)
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
        Binding? item = ((Button)e.Source).DataContext as Binding;

        if (item == null)
            return;

        LearnWindow window = new LearnWindow();
        window.Owner = this;
        window.InputItem = item;
        window.ShowDialog();
        Keyboard.Focus(SearchControl.SearchTextBox);
    }

    void Window_Loaded(object sender, RoutedEventArgs e) => Keyboard.Focus(SearchControl.SearchTextBox);

    void Window_Closed(object sender, EventArgs e)
    {
        string newContent =  InputHelp.ConvertToString(Bindings);

        if (StartupContent == newContent)
            return;

        if (App.InputConf.HasMenu)
            File.WriteAllText(App.InputConf.Path, App.InputConf.Content = newContent);
        else
        {
            newContent = InputHelp.ConvertToString(InputHelp.GetReducedBindings(Bindings));
            File.WriteAllText(App.InputConf.Path, App.InputConf.Content = newContent);
        }
    
        Msg.ShowInfo("Changes will be available on next startup.");
    }

    void DataGrid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        DataGrid grid = (DataGrid)sender;

        if (e.Command == DataGrid.DeleteCommand)
            if (Msg.ShowQuestion($"Confirm to delete: {(grid.SelectedItem as Binding)!.Input} ({(grid.SelectedItem as Binding)!.Path})") != MessageBoxResult.OK)
                e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
            Close();

        if (e.Key == Key.F3 || e.Key == Key.F6 || (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl)))
        {
            Keyboard.Focus(SearchControl.SearchTextBox);
            SearchControl.SearchTextBox.SelectAll();
        }
    }
}
