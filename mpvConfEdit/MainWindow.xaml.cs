using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using DynamicGUI;
using Microsoft.Win32;

namespace mpvConfEdit
{
    public partial class MainWindow : Window
    {
        public string MpvConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpv.conf";
        public string MpvNetConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpvnet.conf";
        private List<SettingBase> MpvSettingsDefinitions = Settings.LoadSettings(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\mpvConf.toml");
        private List<SettingBase> MpvNetSettingsDefinitions = Settings.LoadSettings(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\mpvNetConf.toml");
        public ObservableCollection<string> FilterStrings { get; } = new ObservableCollection<string>();
        private Dictionary<string, Dictionary<string, string>> Comments = new Dictionary<string, Dictionary<string, string>>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Title = (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            LoadSettings(MpvSettingsDefinitions, MpvConf);
            LoadSettings(MpvNetSettingsDefinitions, MpvNetConf);
            SearchControl.Text = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\mpv.net", "conf editor search", "");
            SetDarkTheme();
        }

        public Brush Foreground2 {
            get { return (Brush)GetValue(Foreground2Property); }
            set { SetValue(Foreground2Property, value); }
        }

        public static readonly DependencyProperty Foreground2Property =
            DependencyProperty.Register("Foreground2", typeof(Brush), typeof(MainWindow), new PropertyMetadata(Brushes.DarkSlateGray));

        void SetDarkTheme()
        {
            string darkMode = MpvNetSettingsDefinitions.Where(item => item.Name == "dark-mode").First().Value;

            object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
            if (value is null) value = 1;
            bool isDarkTheme = (int)value == 0;

            if (!((darkMode == "system" && isDarkTheme) || darkMode == "always"))
                return;

            Foreground = Brushes.White;
            Foreground2 = Brushes.Silver;
            Background = Brushes.Black;
        }

        private void LoadSettings(List<SettingBase> settingsDefinitions,
                                  Dictionary<string, string> confSettings)
        {
            foreach (var setting in settingsDefinitions)
            {
                if (!FilterStrings.Contains(setting.Filter))
                    FilterStrings.Add(setting.Filter);

                foreach (var pair in confSettings)
                {
                    if (setting.Name == pair.Key)
                    {
                        setting.Value = pair.Value;
                        setting.StartValue = pair.Value;
                        continue;
                    }
                }

                switch (setting)
                {
                    case StringSetting s:
                        var sc = new StringSettingControl(s);
                        sc.TitleTextBox.Foreground = Controls.Controls.ThemeBrush;
                        MainStackPanel.Children.Add(sc);
                        break;
                    case OptionSetting s:
                        var oc = new OptionSettingControl(s);
                        oc.TitleTextBox.Foreground = Controls.Controls.ThemeBrush;
                        MainStackPanel.Children.Add(oc);
                        break;
                }
            }
        }

        private Dictionary<string, string> _mpvConf;

        public Dictionary<string, string> MpvConf {
            get {
                if (_mpvConf == null) _mpvConf = LoadConf(MpvConfPath);
                return _mpvConf;
            }
        }

        private Dictionary<string, string> _mpvNetConf;

        public Dictionary<string, string> MpvNetConf {
            get {
                if (_mpvNetConf == null) _mpvNetConf = LoadConf(MpvNetConfPath);
                return _mpvNetConf;
            }
        }

        private Dictionary<string, string> LoadConf(string filePath)
        {
            Dictionary<string, string> conf = new Dictionary<string, string>();
            Comments[filePath] = new Dictionary<string, string>();

            if (File.Exists(filePath))
            {
                foreach (string i in File.ReadAllLines(filePath))
                {
                    if (i.Contains("="))
                    {
                        int pos = i.IndexOf("=");
                        string left = i.Substring(0, pos).Replace(" ", "").ToLower();
                        string right = i.Substring(pos + 1).Trim();

                        if (left.StartsWith("#"))
                        {
                            Comments[filePath][left.TrimStart("#".ToCharArray())] = right;
                            continue;
                        }

                        if (left == "fs") left = "fullscreen";
                        if (left == "loop") left = "loop-file";
                        conf[left] = right;
                    }
                }
            }
            return conf;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WriteToDisk();
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\mpv.net", "conf editor search", SearchControl.Text);
        }

        void WriteToDisk()
        {
            bool isDirty = false;

            foreach (SettingBase i in MpvSettingsDefinitions)
                if (i.StartValue != i.Value)
                    isDirty = true;

            foreach (SettingBase i in MpvNetSettingsDefinitions)
                if (i.StartValue != i.Value)
                    isDirty = true;

            if (!isDirty)
                return;

            WriteToDisk(MpvConfPath, MpvConf, MpvSettingsDefinitions);
            WriteToDisk(MpvNetConfPath, MpvNetConf, MpvNetSettingsDefinitions);

            MessageBox.Show("Changes will be available on next startup of mpv(.net).",
                Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void WriteToDisk(string filePath,
                         Dictionary<string, string> confSettings,
                         List<SettingBase> settings)
        {
            string content = "";

            foreach (var i in Comments[filePath])
                content += $"#{i.Key} = {i.Value}\r\n";

            foreach (var setting in settings)
            {
                if ((setting.Value ?? "") != setting.Default)
                    confSettings[setting.Name] = setting.Value;

                if (confSettings.ContainsKey(setting.Name) &&
                    (setting.Value ?? "") == setting.Default ||
                    (setting.Value ?? "") == "")
                {
                    confSettings.Remove(setting.Name);
                }
            }

            foreach (var i in confSettings)
                content = content + $"{i.Key} = {i.Value}\r\n";

            File.WriteAllText(filePath, content);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string activeFilter = "";

            foreach (var i in FilterStrings)
                if (SearchControl.Text == i + ":")
                    activeFilter = i;

            if (activeFilter == "")
            {
                foreach (UIElement i in MainStackPanel.Children)
                    if ((i as ISettingControl).Contains(SearchControl.Text))
                        i.Visibility = Visibility.Visible;
                    else
                        i.Visibility = Visibility.Collapsed;

                FilterListBox.SelectedItem = null;
            }
            else
                foreach (UIElement i in MainStackPanel.Children)
                    if ((i as ISettingControl).SettingBase.Filter == activeFilter)
                        i.Visibility = Visibility.Visible;
                    else
                        i.Visibility = Visibility.Collapsed;

            MainScrollViewer.ScrollToTop();
        }
        
        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            SearchControl.SearchTextBox.SelectAll();
            Keyboard.Focus(SearchControl.SearchTextBox);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SearchControl.Text = e.AddedItems[0].ToString() + ":";
        }

        private void OpenSettingsTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(MpvConfPath));
        }

        private void ShowManualTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://mpv.io/manual/master/");
        }

        private void SupportTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/stax76/mpv.net#Support");
        }

        private void ApplyTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WriteToDisk();
        }
    }
}