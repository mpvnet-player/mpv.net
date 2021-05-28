
using System.Windows;
using System.Windows.Controls;

using mpvnet;

namespace DynamicGUI
{
    public partial class OptionSettingControl : UserControl, ISettingControl
    {
        OptionSetting OptionSetting;

        public OptionSettingControl(OptionSetting optionSetting)
        {
            OptionSetting = optionSetting;
            InitializeComponent();
            DataContext = this;
            TitleTextBox.Text = optionSetting.Name;

            if (string.IsNullOrEmpty(optionSetting.Help))
                HelpTextBox.Visibility = Visibility.Collapsed;

            HelpTextBox.Text = optionSetting.Help;
            ItemsControl.ItemsSource = optionSetting.Options;

            if (string.IsNullOrEmpty(optionSetting.URL))
                LinkTextBlock.Visibility = Visibility.Collapsed;

            Link.SetURL(optionSetting.URL);
        }

        public Theme Theme {
            get => Theme.Current;
        }

        string _SearchableText;

        public string SearchableText {
            get {
                if (_SearchableText is null)
                {
                    _SearchableText = TitleTextBox.Text + HelpTextBox.Text;

                    foreach (var i in OptionSetting.Options)
                        _SearchableText += i.Text + i.Help + i.Name;

                    _SearchableText = _SearchableText.ToLower();
                }
                return _SearchableText;
            }
        }

        public SettingBase SettingBase => OptionSetting;
        public bool Contains(string searchString) => SearchableText.Contains(searchString.ToLower());
    }
}
