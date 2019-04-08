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

using DynamicGUI;

namespace mpvConfEdit
{
    public partial class MainWindow : Window
    {
        public string mpvConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpv.conf";
        private List<SettingBase> DynamicSettings = Settings.LoadSettings(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\mpvConfEdit.toml");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Title = (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;

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
            WriteToDisk();
        }

        void WriteToDisk()
        {
            foreach (var mpvSetting in DynamicSettings)
            {
                switch (mpvSetting)
                {
                    case StringSetting s:
                        if ((s.Value ?? "") != s.Default || mpvConf.ContainsKey(s.Name) || mpvConf.ContainsKey(s.Alias ?? ""))
                            mpvConf[s.Name] = s.Value;
                        break;
                    case OptionSetting s:
                        if ((s.Value ?? "") != s.Default || mpvConf.ContainsKey(s.Name) || mpvConf.ContainsKey(s.Alias ?? ""))
                            mpvConf[s.Name] = s.Value;
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
            MessageBox.Show("Changes will be available on next startup of mpv(.net).",
                Title, MessageBoxButton.OK, MessageBoxImage.Information);
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
            Keyboard.Focus(SearchControl.SearchTextBox);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SearchControl.Text = e.AddedItems[0].ToString() + ":";
        }

        private void OpenSettingsTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(mpvConfPath));
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