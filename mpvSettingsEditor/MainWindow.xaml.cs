using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DynamicGUI;

namespace DynamicGUI
{
    public partial class MainWindow : Window
    {
        public string mpvConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\mpv.conf";
        private List<SettingBase> mpvSettings = Settings.LoadSettings("Definitions.toml");

        public MainWindow()
        {
            InitializeComponent();
            Title = (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            foreach (var setting in mpvSettings)
            {
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
                        MainWrapPanel.Children.Add(new StringSettingControl(s));
                        break;
                    case OptionSetting s:
                        MainWrapPanel.Children.Add(new OptionSettingControl(s));
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            foreach (var mpvSetting in mpvSettings)
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

            foreach (var mpvSetting in mpvSettings)
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

            foreach (var mpvSetting in mpvSettings)
            {
                foreach (var line in lines.ToArray())
                {
                    string test = line.Replace("#", "").Replace(" ", "");

                    if (test.StartsWith(mpvSetting.Name + "=") && !mpvConf.ContainsKey(mpvSetting.Name))
                        lines.Remove(line);
                }
            }

            File.WriteAllText(mpvConfPath, String.Join(Environment.NewLine, lines));
            MessageBox.Show("If running, restart mpv/mpv.net", Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = MainWrapPanel.Children.Count - 1; i >= 0; i--)
            {
                if ((MainWrapPanel.Children[i] as ISearch).Contains(SearchTextBox.Text))
                    MainWrapPanel.Children[i].Visibility = Visibility.Visible;
                else
                    MainWrapPanel.Children[i].Visibility = Visibility.Collapsed;
            }
        }
    }
}