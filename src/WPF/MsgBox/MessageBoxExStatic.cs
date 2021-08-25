
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MsgBoxEx
{
    public partial class MessageBoxEx : Window, INotifyPropertyChanged
    {
        #region static fields

        private static double screenWidth = SystemParameters.WorkArea.Width - 100;

        private static bool enableCloseButton = true;
        private static bool isSilent = false;
        private static List<string> installedFonts = new List<string>();
        public static MessageBoxButtonDefault staticButtonDefault;

        #endregion static fields

        #region static properties

        public static Color DefaultUrlForegroundColor => Colors.Blue;

        private static string MsgBoxIconToolTip { get; set; }

        protected static MsgBoxExDelegate DelegateObj { get; set; }
        protected static bool ExitAfterErrorAction { get; set; }

        public static ContentControl ParentWindow { get; set; }

        public static string ButtonTemplateName { get; set; }

        public static Brush MessageBackground { get; set; }

        public static Brush MessageForeground { get; set; }

        public static Brush ButtonBackground { get; set; }

        public static double MaxFormWidth { get; set; } = screenWidth;

        public static Visibility ShowDetailsBtn { get; set; } = Visibility.Collapsed;

        public static string DetailsText { get; set; }

        public static Visibility ShowCheckBox { get; set; } = Visibility.Collapsed;

        public static MsgBoxExCheckBoxData CheckBoxData { get; set; } = null;

        public static FontFamily MsgFontFamily { get; set; } = new FontFamily("Segoe UI");

        public static double MsgFontSize { get; set; } = 12;

        public static Uri Url { get; set; } = null;

        public static Visibility ShowUrl { get; set; } = Visibility.Collapsed;

        public static string UrlDisplayName { get; set; } = null;

        public static SolidColorBrush UrlForeground { get; set; } = new SolidColorBrush(DefaultUrlForegroundColor);

        public static string DelegateToolTip { get; set; }

        #endregion static properties

        #region Show and ShowEx

        public static MessageBoxResult OpenMessageBox(
            Window owner, string msg, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            //if (owner == null)
            //{
            //    owner = (Application.Current.MainWindow.Visibility == Visibility.Visible) ? Application.Current.MainWindow : null;
            //}

            MessageBoxEx form = new MessageBoxEx(msg, title, buttons, image) /*{ Owner = owner }*/;

            form.ShowDialog();
            return form.MessageResult;
        }

        public static MessageBoxResultEx OpenMessageBox(Window owner, string msg, string title, MessageBoxButtonEx buttons, MessageBoxImage image)
        {
            //if (owner == null)
            //{
            //    owner = (Application.Current.MainWindow.Visibility == Visibility.Visible) ? Application.Current.MainWindow : null;
            //}
            MessageBoxEx form = new MessageBoxEx(msg, title, buttons, image) /*{ Owner = owner }*/;
            form.ShowDialog();
            return form.MessageResultEx;
        }

        #endregion Show and ShowEx

        #region static configuration methods

        public static void SetMessageBackground(Color color)
        {
            try
            {
                MessageBackground = new SolidColorBrush(color);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        public static void SetMessageForeground(Color color)
        {
            try
            {
                MessageForeground = new SolidColorBrush(color);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        public static void SetButtonBackground(System.Windows.Media.Color color)
        {
            try
            {
                ButtonBackground = new SolidColorBrush(color);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        public static Color ColorFromString(string colorString)
        {
            Color wpfColor = System.Windows.Media.Colors.Black;

            try
            {
                wpfColor = (Color)ColorConverter.ConvertFromString(colorString);
            }
            catch (Exception) { }

            return wpfColor;
        }

        public static void SetFont()
        {
            MsgFontFamily = Application.Current.MainWindow.FontFamily;
            MsgFontSize = Application.Current.MainWindow.FontSize;
        }

        public static void SetFont(ContentControl parent)
        {
            MsgFontFamily = parent.FontFamily;
            MsgFontSize = parent.FontSize;
        }

        public static void SetFont(string familyName, double size)
        {
            if (!IsFontFamilyValid(familyName))
                if (!string.IsNullOrEmpty(familyName))
                    MsgFontFamily = new System.Windows.Media.FontFamily(familyName);
            MsgFontSize = Math.Max(1.0, size);
        }

        private static bool IsFontFamilyValid(string name)
        {
            if (installedFonts.Count == 0)
                using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
                    installedFonts = (from x in fontsCollection.Families select x.Name).ToList();
            return installedFonts.Contains(name);
        }

        public static void SetButtonTemplateName(string name)
        {
            ButtonTemplateName = name;
        }

        public static void SetMaxFormWidth(double value)
        {
            MaxFormWidth = Math.Max(value, 300);
            double minWidth = 300;
            MaxFormWidth = Math.Max(minWidth, Math.Min(value, screenWidth));
        }

        public static void ResetToDefaults()
        {
            MsgFontSize = 12d;
            MsgFontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            DelegateObj = null;
            DetailsText = null;
            MessageForeground = null;
            MessageBackground = null;
            ButtonBackground = null;
            ParentWindow = null;
            isSilent = false;
            enableCloseButton = true;
            ButtonTemplateName = null;
            MsgBoxIconToolTip = null;
            ShowCheckBox = Visibility.Collapsed;
            CheckBoxData = null;
            ExitAfterErrorAction = false;
            MaxFormWidth = 800;
            Url = null;
            ShowUrl = Visibility.Collapsed;
            UrlDisplayName = null;
            UrlForeground = new SolidColorBrush(DefaultUrlForegroundColor);
            staticButtonDefault = MessageBoxButtonDefault.Forms;
        }

        public static void EnableCloseButton(bool enable)
        {
            enableCloseButton = enable;
        }

        public static void SetAsSilent(bool quiet)
        {
            isSilent = quiet;
        }

        public static void SetDefaultButton(MessageBoxButtonDefault buttonDefault)
        {
            staticButtonDefault = buttonDefault;
        }

        #endregion static configuration methods
    }
}
