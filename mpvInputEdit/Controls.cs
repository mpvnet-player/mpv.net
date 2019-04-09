using System;
using System.Windows;
using System.Windows.Media;

namespace Controls
{
    class Controls
    {
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