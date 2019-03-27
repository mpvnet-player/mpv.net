using System.Windows;
using System.Windows.Controls;

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
            ValueTextBox.Text = stringSetting.Value;
            if (stringSetting.Width > 0)
                ValueTextBox.Width = stringSetting.Width;
            if (!StringSetting.IsFolder)
                Button.Visibility = Visibility.Hidden;
            Link.SetURL(StringSetting.HelpURL);

            if (string.IsNullOrEmpty(stringSetting.HelpURL))
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
            using (var d = new System.Windows.Forms.FolderBrowserDialog())
            {
                d.Description = "Choose a folder.";
                d.SelectedPath = ValueTextBox.Text;
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    ValueTextBox.Text = d.SelectedPath;
            }
        }
    }
}