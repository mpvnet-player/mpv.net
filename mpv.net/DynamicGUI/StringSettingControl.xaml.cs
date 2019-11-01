using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinForms = System.Windows.Forms;

namespace DynamicGUI
{
    public partial class StringSettingControl : UserControl, ISettingControl
    {
        private StringSetting StringSetting;
        
        public StringSettingControl(StringSetting stringSetting)
        {
            StringSetting = stringSetting;
            InitializeComponent();
            TitleTextBox.Text = stringSetting.Name;
            HelpTextBox.Text = stringSetting.Help;
            ValueTextBox.Text = StringSetting.Value;
            if (StringSetting.Width > 0)
                ValueTextBox.Width = StringSetting.Width;
            if (StringSetting.Type != "folder" && StringSetting.Type != "color")
                Button.Visibility = Visibility.Hidden;
            Link.SetURL(StringSetting.URL);
            if (string.IsNullOrEmpty(stringSetting.URL))
                LinkTextBlock.Visibility = Visibility.Collapsed;
        }

        private string _SearchableText;

        public string SearchableText {
            get {
                if (_SearchableText is null)
                    _SearchableText = (TitleTextBox.Text + HelpTextBox.Text +ValueTextBox.Text).ToLower();
                return _SearchableText;
            }
        }

        public bool Contains(string searchString) => SearchableText.Contains(searchString.ToLower());
        public SettingBase SettingBase => StringSetting;

        public string Text
        {
            get => StringSetting.Value;
            set => StringSetting.Value = value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (StringSetting.Type)
            {
                case "folder":
                    using (var d = new WinForms.FolderBrowserDialog())
                    {
                        d.Description = "Choose a folder.";
                        d.SelectedPath = ValueTextBox.Text;
                        if (d.ShowDialog() == WinForms.DialogResult.OK)
                            ValueTextBox.Text = d.SelectedPath;
                    }
                    break;
                case "color":
                    using (var dialog = new WinForms.ColorDialog())
                    {
                        dialog.FullOpen = true;

                        try {
                            if (!string.IsNullOrEmpty(ValueTextBox.Text))
                            {
                                Color col = GetColor(ValueTextBox.Text);
                                dialog.Color = System.Drawing.Color.FromArgb(col.A, col.R, col.G, col.B); 
                            }
                        } catch {}

                        if (dialog.ShowDialog() == WinForms.DialogResult.OK)
                            ValueTextBox.Text = "#" + dialog.Color.ToArgb().ToString("X8");
                    }
                    break;
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e) => Update();

        Color GetColor(string value)
        {
            if (value.Contains("/"))
            {
                string[] a = value.Split('/');

                if (a.Length == 3)
                    return Color.FromRgb(ToByte(a[0]), ToByte(a[1]), ToByte(a[2]));
                else if (a.Length == 4)
                    return Color.FromArgb(ToByte(a[3]), ToByte(a[0]), ToByte(a[1]), ToByte(a[2]));
            }

            return (Color)ColorConverter.ConvertFromString(value);

            Byte ToByte(string val) => Convert.ToByte(Convert.ToSingle(val, CultureInfo.InvariantCulture) * 255);
        }

        public void Update()
        {
            if (StringSetting.Type == "color")
            {
                Color c = Colors.Transparent;
                if (ValueTextBox.Text != "") try { c = GetColor(ValueTextBox.Text); } catch {}
                ValueTextBox.Background = new SolidColorBrush(c);
            }
        }
    }
}