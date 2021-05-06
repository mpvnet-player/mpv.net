
using System;
using System.Windows;

namespace mpvnet
{
    public class WPF
    {
        public static void Init()
        {
            if (Application.Current == null)
            {
                new Application();

                Application.Current.Resources.MergedDictionaries.Add(
                    Application.LoadComponent(new Uri("mpvnet;component/WPF/Resources.xaml",
                        UriKind.Relative)) as ResourceDictionary);

                Application.Current.DispatcherUnhandledException += (sender, e) => App.ShowException(e.Exception);
            }
        }
    }
}
