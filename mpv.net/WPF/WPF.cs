using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using Microsoft.Win32;
using mpvnet;

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

        public static Brush ThemeBrush { get; } = new SolidColorBrush(ThemeColor);

        static bool WasThemeColorSet;

        static Color _ThemeColor;

        public static Color ThemeColor {
            get {
                if (!WasThemeColorSet)
                {
                    Color? color = null;

                    try {
                        if (App.IsDarkMode && !string.IsNullOrEmpty(App.DarkColor))
                            color = (Color)ColorConverter.ConvertFromString(App.DarkColor);
                        else if (!App.IsDarkMode && !string.IsNullOrEmpty(App.LightColor))
                            color = (Color)ColorConverter.ConvertFromString(App.LightColor);
                    } catch { }

                    if (!color.HasValue)
                    {
                        if (Environment.OSVersion.Version.Major < 10)
                        {
                            int argb = Convert.ToInt32(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", 0));
                            if (argb == 0) System.Drawing.Color.Blue.ToArgb();
                            var c = System.Drawing.Color.FromArgb(argb);
                            color = Color.FromArgb(c.A, c.R, c.G, c.B);
                        }
                        else
                            color = SystemParameters.WindowGlassColor;
                    }

                    _ThemeColor = color.Value;
                    WasThemeColorSet = true;
                }
                return _ThemeColor;
            }
        }
    }
}