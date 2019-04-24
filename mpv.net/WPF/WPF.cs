using System;
using System.Windows;
using System.Windows.Media;

namespace WPF
{
    public class WPF
    {
        public static void Init()
        {
            EnsureApplicationResources();
        }

        public static void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                new Application();
                Application.Current.Resources.MergedDictionaries.Add(
                    Application.LoadComponent(new Uri("mpvnet;component/WPF/Resources.xaml",
                        UriKind.Relative)) as ResourceDictionary);
            }
        }

        public static Brush ThemeBrush {
            get {
                if (Environment.OSVersion.Version.Major < 10)
                    return new SolidColorBrush(Colors.DarkSlateGray);
                else
                    return SystemParameters.WindowGlassBrush;
            }
        }
    }
}