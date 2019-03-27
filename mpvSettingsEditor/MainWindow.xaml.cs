using DynamicGUI;
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

namespace mpvSettingsEditor
{
    public partial class MainWindow : Window
    {
        public string mpvConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpv.conf";
        private List<SettingBase> DynamicSettings = Settings.LoadSettings(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Definitions.toml");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Title = (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            foreach (var setting in DynamicSettings)
            {
                if (!FilterStrings.Contains(setting.Filter))
                    FilterStrings.Add(setting.Filter);
                foreach (var pair in mpvConf)
                {
                    if (setting.Name == pair.Key || setting.Alias == pair.Key)
                        switch (setting)
                        {
                            case StringSetting s:
                                s.Value = pair.Value;
                                continue;
                            case OptionSetting s:
                                s.Value = pair.Value;
                                break;
                        }
                }
                switch (setting)
                {
                    case StringSetting s:
                        MainStackPanel.Children.Add(new StringSettingControl(s));
                        break;
                    case OptionSetting s:
                        MainStackPanel.Children.Add(new OptionSettingControl(s));
                        break;
                }
            }
        }

        private Dictionary<string, string> _mpvConf;

        public Dictionary<string, string> mpvConf {
            get {
                if (_mpvConf == null)
                {
                    _mpvConf = new Dictionary<string, string>();

                    if (File.Exists(mpvConfPath))
                        foreach (var i in File.ReadAllLines(mpvConfPath))
                            if (i.Contains("=") && !i.Trim().StartsWith("#"))
                            {
                                int pos = i.IndexOf("=");
                                _mpvConf[i.Substring(0, pos).Trim()] = i.Substring(pos + 1).Trim();
                            }
                }
                return _mpvConf;
            }
        }

        public ObservableCollection<string> FilterStrings { get; } = new ObservableCollection<string>();

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            foreach (var mpvSetting in DynamicSettings)
            {
                switch (mpvSetting)
                {
                    case StringSetting s:
                        if ((s.Value ?? "") != s.Default)
                            mpvConf[s.Name] = s.Value;
                        else
                            mpvConf.Remove(s.Name);
                        break;
                    case OptionSetting s:
                        if ((s.Value ?? "") != s.Default)
                            mpvConf[s.Name] = s.Value;
                        else
                            mpvConf.Remove(s.Name);
                        break;
                }
            }

            if (!File.Exists(mpvConfPath))
                File.WriteAllText(mpvConfPath, "");

            List<string> lines = File.ReadAllLines(mpvConfPath).ToList();

            foreach (var mpvSetting in DynamicSettings)
            {
                foreach (var line in lines.ToArray())
                {
                    string test = line.Replace("#", "").Replace(" ", "");
                    if (test.StartsWith(mpvSetting.Alias + "="))
                    {
                        lines.Remove(line);
                        foreach (var pair in mpvConf.ToArray())
                            if (test.StartsWith(pair.Key + "="))
                                mpvConf.Remove(pair.Key);
                    }
                }
            }

            foreach (var pair in mpvConf)
            {
                bool changed = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("=") &&
                        lines[i].Substring(0, lines[i].IndexOf("=")).Trim("# ".ToCharArray()) == pair.Key)
                    {
                        lines[i] = pair.Key + " = " + pair.Value;
                        changed = true;
                    }
                }

                if (!changed)
                    lines.Add(pair.Key + " = " + pair.Value);
            }

            foreach (var mpvSetting in DynamicSettings)
            {
                foreach (var line in lines.ToArray())
                {
                    string test = line.Replace("#", "").Replace(" ", "");

                    if (test.StartsWith(mpvSetting.Name + "=") && !mpvConf.ContainsKey(mpvSetting.Name))
                        lines.Remove(line);
                }
            }

            File.WriteAllText(mpvConfPath, String.Join(Environment.NewLine, lines));

            foreach (Process process in Process.GetProcesses())
                if (process.ProcessName == "mpv.net")
                    MessageBox.Show("Restart mpv.net in order to apply changed settings.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                else if (process.ProcessName == "mpv")
                    MessageBox.Show("Restart mpv in order to apply changed settings.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTextBlock.Text = SearchTextBox.Text == "" ? "Find a setting" : "";

            if (SearchTextBox.Text == "")
                SearchClearButton.Visibility = Visibility.Hidden;
            else
                SearchClearButton.Visibility = Visibility.Visible;

            string activeFilter = "";

            foreach (var i in FilterStrings)
                if (SearchTextBox.Text == i + ":")
                    activeFilter = i;

            if (activeFilter == "")
            {
                foreach (UIElement i in MainStackPanel.Children)
                    if ((i as ISettingControl).Contains(SearchTextBox.Text))
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
        }
        
        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SearchTextBox);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SearchTextBox.Text = e.AddedItems[0].ToString() + ":";
        }

        private void SearchClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Keyboard.Focus(SearchTextBox);
        }

        private void OpenSettingsTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(mpvConfPath));
        }

        private void ShowManualTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://mpv.io/manual/master/");
        }
    }
}