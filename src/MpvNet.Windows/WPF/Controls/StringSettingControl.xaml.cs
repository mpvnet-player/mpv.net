
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Forms = System.Windows.Forms;

using MpvNet.Windows.UI;

namespace MpvNet.Windows.WPF;

public partial class StringSettingControl : UserControl, ISettingControl
{
    StringSetting StringSetting;
    
    public StringSettingControl(StringSetting stringSetting)
    {
        StringSetting = stringSetting;
        InitializeComponent();
        DataContext = this;
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

        if (string.IsNullOrEmpty(stringSetting.Help))
            HelpTextBox.Visibility = Visibility.Collapsed;
    }

    public Theme? Theme => Theme.Current;

    public bool Contains(string search)
    {
        if (TitleTextBox.Text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
            return true;

        if (HelpTextBox.Text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
            return true;

        if (ValueTextBox.Text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
            return true;

        return false;
    }

    public Setting Setting => StringSetting;

    public string? Text
    {
        get => StringSetting.Value;
        set => StringSetting.Value = value;
    }

    void Button_Click(object sender, RoutedEventArgs e)
    {
        switch (StringSetting.Type)
        {
            case "folder":
                {
                    var dialog = new Forms.FolderBrowserDialog { InitialDirectory = ValueTextBox.Text };

                    if (dialog.ShowDialog() == Forms.DialogResult.OK)
                        ValueTextBox.Text = dialog.SelectedPath;
                }
                break;
            case "color":
                using (var dialog = new Forms.ColorDialog())
                {
                    dialog.FullOpen = true;

                    try
                    {
                        if (!string.IsNullOrEmpty(ValueTextBox.Text))
                        {
                            Color col = GetColor(ValueTextBox.Text);
                            dialog.Color = System.Drawing.Color.FromArgb(col.A, col.R, col.G, col.B); 
                        }
                    } catch {}

                    if (dialog.ShowDialog() == Forms.DialogResult.OK)
                        ValueTextBox.Text = "#" + dialog.Color.ToArgb().ToString("X8");
                }
                break;
        }
    }

    void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e) => Update();

    Color GetColor(string value)
    {
        if (value.Contains('/'))
        {
            string[] a = value.Split('/');

            if (a.Length == 3)
                return Color.FromRgb(ToByte(a[0]), ToByte(a[1]), ToByte(a[2]));
            else if (a.Length == 4)
                return Color.FromArgb(ToByte(a[3]), ToByte(a[0]), ToByte(a[1]), ToByte(a[2]));
        }

        return (Color)ColorConverter.ConvertFromString(value);

        byte ToByte(string val) => Convert.ToByte(Convert.ToSingle(val, CultureInfo.InvariantCulture) * 255);
    }

    public void Update()
    {
        if (StringSetting.Type == "color")
        {
            Color color = Colors.Transparent;

            if (ValueTextBox.Text != "")
                try {
                    color = GetColor(ValueTextBox.Text);
                } catch {}

            ValueTextBox.Background = new SolidColorBrush(color);
        }
    }
}
