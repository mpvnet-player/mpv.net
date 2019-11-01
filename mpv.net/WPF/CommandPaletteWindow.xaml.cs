using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace mpvnet
{
    public partial class CommandPaletteWindow : Window
    {
        ICollectionView CollectionView;

        public CommandPaletteWindow()
        {
            InitializeComponent();
            CollectionViewSource collectionViewSource = new CollectionViewSource() { Source = CommandItem.Items };
            CollectionView = collectionViewSource.View;
            var yourCostumFilter = new Predicate<object>(item => Filter((CommandItem)item));
            CollectionView.Filter = yourCostumFilter;
            ListView.ItemsSource = CollectionView;
        }

        bool Filter(CommandItem item)
        {
            if (item.Command == "" || item.Path == "")
                return false;
            string filter = FilterTextBox.Text.ToLower();
            if (filter == "") return true;
            if (item.Command.ToLower().Contains(filter) ||
                item.Input.ToLower().Contains(filter) ||
                item.Path.ToLower().Contains(filter))
                return true;
            return false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            Keyboard.Focus(FilterTextBox);
            SelectFirst();
        }

        void SelectFirst()
        {
            if (ListView.Items.Count > 0)
                ListView.SelectedIndex = 0;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x200 /*WM_MOUSEMOVE*/ && Mouse.LeftButton != MouseButtonState.Pressed)
                handled = true;
            return IntPtr.Zero;
        }

        private void FilterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        int index = ListView.SelectedIndex;
                        index -= 1;
                        if (index < 0) index = 0;
                        ListView.SelectedIndex = index;
                        ListView.ScrollIntoView(ListView.SelectedItem);
                    }
                    break;
                case Key.Down:
                    {
                        int index = ListView.SelectedIndex;
                        index += 1;
                        if (index > ListView.Items.Count - 1) index = ListView.Items.Count - 1;
                        ListView.SelectedIndex = index;
                        ListView.ScrollIntoView(ListView.SelectedItem);
                    }
                    break;
                case Key.Escape:
                    Close();
                    break;
                case Key.Enter:
                    Execute();
                    break;
            }
        }

        void Execute()
        {
            if (ListView.SelectedItem != null)
            {
                CommandItem item = ListView.SelectedItem as CommandItem;
                Close();
                mp.command(item.Command);
            }
        }

        private void ListView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Execute();
        }

        private void FilterTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionView.Refresh();
            SelectFirst();
        }
    }
}