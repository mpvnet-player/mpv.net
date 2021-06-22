
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace mpvnet
{
    public partial class CommandPaletteControl : UserControl
    {
        public ICollectionView CollectionView { get; set; }
        public ICommand EscapeCommand { get; }
        public CollectionViewSource CollectionViewSource { get; }
        public ObservableCollection<CommandPaletteItem> Items { get; } = new ObservableCollection<CommandPaletteItem>();

        public CommandPaletteControl()
        {
            InitializeComponent();
            DataContext = this;
            CollectionViewSource = new CollectionViewSource() { Source = Items };
            CollectionView = CollectionViewSource.View;
            CollectionView.Filter = new Predicate<object>(item => Filter((CommandPaletteItem)item));
            MainListView.ItemsSource = CollectionView;

            EscapeCommand = new RelayCommand(OnEscapeCommand);
            SearchControl.SearchTextBox.PreviewKeyDown += SearchControl_PreviewKeyDown;
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            SearchControl.SearchTextBox.BorderBrush = Theme.Background;
            SearchControl.HideClearButton = true;
        }

        void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView.Refresh();
            SelectFirst();
        }

        void SearchControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        int index = MainListView.SelectedIndex;
                        index -= 1;

                        if (index < 0)
                            index = 0;

                        MainListView.SelectedIndex = index;
                        MainListView.ScrollIntoView(MainListView.SelectedItem);
                    }
                    break;
                case Key.Down:
                    {
                        int index = MainListView.SelectedIndex;

                        if (++index > MainListView.Items.Count - 1)
                            index = MainListView.Items.Count - 1;

                        MainListView.SelectedIndex = index;
                        MainListView.ScrollIntoView(MainListView.SelectedItem);
                    }
                    break;
                case Key.Enter:
                    Execute();
                    break;
            }
        }

        void OnEscapeCommand(object param)
        {
            MainForm.Instance.HideCommandPalette();
        }

        public Theme Theme => Theme.Current;

        bool Filter(CommandPaletteItem item)
        {
            string filter = SearchControl.SearchTextBox.Text.ToLower();

            if (filter == "" || item.Text.ToLower().Contains(filter) ||
                item.SecondaryText.ToLower().Contains(filter))

                return true;

            return false;
        }

        public void SelectFirst()
        {
            if (MainListView.Items.Count > 0)
                MainListView.SelectedIndex = 0;
        }

        void Execute()
        {
            if (MainListView.SelectedItem != null)
            {
                CommandPaletteItem item = MainListView.SelectedItem as CommandPaletteItem;
                MainForm.Instance.HideCommandPalette();
                item.Action.Invoke();
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SearchControl.SearchTextBox);
        }

        public void SetItems(IEnumerable<CommandPaletteItem> items)
        {
            Items.Clear();

            foreach (var i in items)
                Items.Add(i);
        }

        void MainListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustHeight();
        }

        public void AdjustHeight()
        {
            double actualHeight = SearchControl.ActualHeight + MainListView.ActualHeight;
            int dpi = Native.GetDPI(MainForm.Instance.Handle);
            MainForm.Instance.CommandPaletteHost.Height = (int)(actualHeight / 96.0 * dpi);
        }

        void MainListView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Execute();
        }
    }
}
