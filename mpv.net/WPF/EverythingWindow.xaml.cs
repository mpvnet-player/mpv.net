
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using static mpvnet.Core;

namespace mpvnet
{
    public partial class EverythingWindow : Window
    {
        public EverythingWindow()
        {
            InitializeComponent();
        }

        const int EVERYTHING_REQUEST_FILE_NAME = 0x00000001;
        const int EVERYTHING_REQUEST_PATH = 0x00000002;

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        public static extern int Everything_SetSearch(string lpSearchString);

        [DllImport("Everything.dll")]
        public static extern void Everything_SetRequestFlags(UInt32 dwRequestFlags);

        [DllImport("Everything.dll")]
        public static extern void Everything_SetSort(UInt32 dwSortType);

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        public static extern bool Everything_Query(bool bWait);

        [DllImport("Everything.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount);

        [DllImport("Everything.dll")]
        public static extern bool Everything_GetResultSize(UInt32 nIndex, out long lpFileSize);

        [DllImport("Everything.dll")]
        public static extern UInt32 Everything_GetNumResults();

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            Keyboard.Focus(FilterTextBox);
        }

        void SelectFirst()
        {
            if (ListView.Items.Count > 0)
                ListView.SelectedIndex = 0;
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x200 /*WM_MOUSEMOVE*/ && Mouse.LeftButton != MouseButtonState.Pressed)
                handled = true;

            return IntPtr.Zero;
        }

        void FilterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        int index = ListView.SelectedIndex;

                        if (--index < 0)
                            index = 0;

                        ListView.SelectedIndex = index;
                        ListView.ScrollIntoView(ListView.SelectedItem);
                    }
                    break;
                case Key.Down:
                    {
                        int index = ListView.SelectedIndex;

                        if (++index > ListView.Items.Count - 1)
                            index = ListView.Items.Count - 1;

                        ListView.SelectedIndex = index;
                        ListView.ScrollIntoView(ListView.SelectedItem);
                    }
                    break;
                case Key.Escape: Close(); break;
                case Key.Enter:  Execute(); break;
            }
        }

        void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.Enter)
                Execute();
        }

        void Execute()
        {
            if (ListView.SelectedItem != null)
                core.LoadFiles(new[] { ListView.SelectedItem as string }, true, Keyboard.Modifiers == ModifierKeys.Control);

            Keyboard.Focus(FilterTextBox);
        }

        void ListView_MouseUp(object sender, MouseButtonEventArgs e) => Execute();

        void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchtext = FilterTextBox.Text;
            App.RunAction(() => Search(searchtext));
        }

        object LockObject = new object();

        void Search(string searchText)
        {
            lock (LockObject)
            {
                try
                {
                    List<string> items = new List<string>();
                    StringBuilder sb = new StringBuilder(500);
                    Everything_SetSearch(searchText);
                    Everything_SetRequestFlags(EVERYTHING_REQUEST_FILE_NAME | EVERYTHING_REQUEST_PATH);
                    Everything_Query(true);
                    uint count = Everything_GetNumResults();

                    for (uint i = 0; i < count; i++)
                    {
                        Everything_GetResultFullPathName(i, sb, (uint)sb.Capacity);
                        string ext = sb.ToString().Ext();

                        if (Core.AudioTypes.Contains(ext) || Core.VideoTypes.Contains(ext) || Core.ImageTypes.Contains(ext))
                            items.Add(sb.ToString());

                        if (items.Count > 100)
                            break;
                    }

                    Application.Current.Dispatcher.Invoke(() => {
                        ListView.ItemsSource = items;
                        SelectFirst();
                    });
                }
                catch (Exception)
                {
                    Msg.ShowError("Search query failed.",
                        "The search feature depends on [Everything](https://www.voidtools.com) being installed.");
                }
            }
        }
    }
}
