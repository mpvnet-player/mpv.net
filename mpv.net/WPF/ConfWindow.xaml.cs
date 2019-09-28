using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

using DynamicGUI;

namespace mpvnet
{
    public partial class ConfWindow : Window
    {
        private List<SettingBase> SettingsDefinitions = Settings.LoadSettings(Properties.Resources.mpvConfToml);
        private List<SettingBase> NetSettingsDefinitions = Settings.LoadSettings(Properties.Resources.mpvNetConfToml);
        public ObservableCollection<string> FilterStrings { get; } = new ObservableCollection<string>();
        string InitialContent;
        
        public ConfWindow()
        {
            InitializeComponent();
            DataContext = this;
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            LoadSettings(SettingsDefinitions, Conf);
            LoadSettings(NetSettingsDefinitions, NetConf);
            InitialContent = GetContent(mp.ConfPath, Conf, SettingsDefinitions) +
                             GetContent(App.ConfPath, NetConf, NetSettingsDefinitions);
            SearchControl.Text = RegHelp.GetString(App.RegPath, "ConfigEditorSearch");

            if (App.IsDarkMode)
            {
                Foreground = Brushes.White;
                Foreground2 = Brushes.Silver;
                Background = Brushes.Black;
            }
        }

        public Brush Foreground2 {
            get { return (Brush)GetValue(Foreground2Property); }
            set { SetValue(Foreground2Property, value); }
        }

        public static readonly DependencyProperty Foreground2Property =
            DependencyProperty.Register("Foreground2", typeof(Brush), typeof(ConfWindow), new PropertyMetadata(Brushes.DarkSlateGray));

        private void LoadSettings(List<SettingBase> settingsDefinitions,
                                  Dictionary<string, string> confSettings)
        {
            foreach (SettingBase setting in settingsDefinitions)
            {
                if (!FilterStrings.Contains(setting.Filter))
                    FilterStrings.Add(setting.Filter);

                foreach (var pair in confSettings)
                {
                    if (setting.Name == pair.Key)
                    {
                        setting.Value = pair.Value.Trim('\'', '"');
                        continue;
                    }
                }

                switch (setting)
                {
                    case StringSetting s:
                        var sc = new StringSettingControl(s);
                        sc.TitleTextBox.Foreground = WPF.WPF.ThemeBrush;
                        MainStackPanel.Children.Add(sc);
                        break;
                    case OptionSetting s:
                        var oc = new OptionSettingControl(s);
                        oc.TitleTextBox.Foreground = WPF.WPF.ThemeBrush;
                        MainStackPanel.Children.Add(oc);
                        break;
                }
            }
        }

        private Dictionary<string, string> _Conf;

        public Dictionary<string, string> Conf {
            get {
                if (_Conf == null) _Conf = LoadConf(mp.ConfPath);
                return _Conf;
            }
        }

        private Dictionary<string, string> _NetConf;

        public Dictionary<string, string> NetConf {
            get {
                if (_NetConf == null) _NetConf = LoadConf(App.ConfPath);
                return _NetConf;
            }
        }

        private Dictionary<string, string> LoadConf(string filePath)
        {
            Dictionary<string, string> conf = new Dictionary<string, string>();

            if (File.Exists(filePath))
            {
                foreach (string i in File.ReadAllLines(filePath))
                {
                    if (i.Contains("="))
                    {
                        int pos = i.IndexOf("=");
                        string left = i.Substring(0, pos).Trim().ToLower();
                        string right = i.Substring(pos + 1).Trim();
                        if (left.StartsWith("#")) continue;
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
            RegHelp.SetObject(App.RegPath, "ConfigEditorSearch", SearchControl.Text);
            string content = GetContent(mp.ConfPath, Conf, SettingsDefinitions);
            string netContent = GetContent(App.ConfPath, NetConf, NetSettingsDefinitions);
            if (InitialContent == content + netContent) return;
            File.WriteAllText(mp.ConfPath, content);
            File.WriteAllText(App.ConfPath, netContent);
            Msg.Show("Changes will be available on next mpv.net startup.");            
        }

        string GetContent(string filePath, Dictionary<string, string> confSettings, List<SettingBase> settings)
        {
            string content = "";

            foreach (SettingBase setting in settings)
            {
                if ((setting.Value ?? "") != setting.Default)
                    if (setting.Type == "string" ||
                        setting.Type == "folder" ||
                        setting.Type == "color")

                        confSettings[setting.Name] = "'" + setting.Value + "'";
                    else
                        confSettings[setting.Name] = setting.Value;

                if (confSettings.ContainsKey(setting.Name) &&
                    (setting.Value ?? "") == setting.Default ||
                    (setting.Value ?? "") == "")

                    confSettings.Remove(setting.Name);
            }

            foreach (var i in confSettings)
                content = content + $"{i.Key} = {i.Value}\r\n";

            return content;
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
        
        private void ConfWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            SearchControl.SearchTextBox.SelectAll();
            Keyboard.Focus(SearchControl.SearchTextBox);

            foreach (var i in MainStackPanel.Children.OfType<StringSettingControl>())
                i.Update();
        }

        private void FilterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) SearchControl.Text = e.AddedItems[0] + ":";
        }

        private void OpenSettingsTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(mp.ConfPath));
        }

        private void PreviewTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Msg.Show("mpv.conf Preview", GetContent(mp.ConfPath, Conf, SettingsDefinitions));
        }

        private void ShowManualTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://mpv.io/manual/master/");
        }

        private void SupportTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/stax76/mpv.net#Support");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
                Close();
        }
    }
}