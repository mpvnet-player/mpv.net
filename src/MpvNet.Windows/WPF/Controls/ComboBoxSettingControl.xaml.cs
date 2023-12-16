
using System.Windows;
using System.Windows.Controls;

using MpvNet.Windows.UI;

namespace MpvNet.Windows.WPF;

public partial class ComboBoxSettingControl : UserControl, ISettingControl
{
    OptionSetting OptionSetting;

    public ComboBoxSettingControl(OptionSetting optionSetting)
    {
        OptionSetting = optionSetting;
        InitializeComponent();
        DataContext = this;
        TitleTextBox.Text = optionSetting.Name;

        if (string.IsNullOrEmpty(optionSetting.Help))
            HelpTextBox.Visibility = Visibility.Collapsed;

        HelpTextBox.Text = optionSetting.Help;
        ComboBoxControl.ItemsSource = optionSetting.Options;

        foreach (var item in optionSetting.Options)
            if (item.Name == optionSetting.Value)
                ComboBoxControl.SelectedItem = item;

        if (string.IsNullOrEmpty(optionSetting.URL))
            LinkTextBlock.Visibility = Visibility.Collapsed;

        Link.SetURL(optionSetting.URL);
    }

    public Theme? Theme => Theme.Current;

    public Setting Setting => OptionSetting;

    public bool Contains(string searchString) => ContainsInternal(searchString.ToLower());

    public bool ContainsInternal(string search)
    {
        if (TitleTextBox.Text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
            return true;

        if (HelpTextBox.Text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
            return true;

        foreach (var i in OptionSetting.Options)
        {
            if (i.Text?.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
                return true;

            if (i.Help?.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
                return true;

            if (i.Name?.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) > -1)
                return true;
        }

        return false;
    }

    void ComboBoxControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OptionSetting.Value = (ComboBoxControl.SelectedItem as OptionSettingOption)?.Name;
    }
}
