
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DynamicGUI;

namespace mpvnet
{
    public partial class ConfWindow : Window
    {
        List<SettingBase> SettingsDefinitions = Settings.LoadSettings(Properties.Resources.confToml);
        List<ConfItem> ConfItems = new List<ConfItem>();
        public ObservableCollection<string> FilterStrings { get; } = new ObservableCollection<string>();
        string InitialContent;

        public ConfWindow()
        {
            InitializeComponent();
            DataContext = this;
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            LoadConf(mp.ConfPath);
            LoadConf(App.ConfPath);
            LoadSettings();
            InitialContent = GetCompareString();
            SearchControl.Text = RegistryHelp.GetString(App.RegPath, "ConfigEditorSearch");
        }

        private void LoadSettings()
        {
            foreach (SettingBase setting in SettingsDefinitions)
            {
                if (!FilterStrings.Contains(setting.Filter))
                    FilterStrings.Add(setting.Filter);

                foreach (ConfItem confItem in ConfItems)
                {
                    if (setting.Name == confItem.Name && confItem.Section == "" && !confItem.IsSectionItem)
                    {
                        setting.Value = confItem.Value.Trim('\'', '"');
                        setting.ConfItem = confItem;
                        confItem.SettingBase = setting;
                        continue;
                    }
                }

                switch (setting)
                {
                    case StringSetting s:
                        var sc = new StringSettingControl(s);
                        MainStackPanel.Children.Add(sc);
                        break;
                    case OptionSetting s:
                        var oc = new OptionSettingControl(s);
                        MainStackPanel.Children.Add(oc);
                        break;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            RegistryHelp.SetValue(App.RegPath, "ConfigEditorSearch", SearchControl.Text);
            
            if (InitialContent == GetCompareString())
                return;
            
            File.WriteAllText(mp.ConfPath, GetContent("mpv"));
            File.WriteAllText(App.ConfPath, GetContent("mpvnet"));
            Msg.Show("Changes will be available on next mpv.net startup.");            
        }

        string GetCompareString()
        {
            return string.Join("", SettingsDefinitions.Select(item => item.Name + item.Value).ToArray());
        }

        void LoadConf(string file)
        {
            if (!File.Exists(file))
                return;

            string comment = "";
            string section = "";
            bool isSectionItem = false;

            foreach (string currentLine in File.ReadAllLines(file))
            {
                string line = currentLine.Trim();

                if (line == "")
                {
                    comment += "\r\n";
                }
                else if (line.StartsWith("#"))
                {
                    comment += line.Trim() + "\r\n";
                }
                else if (line.StartsWith("[") && line.Contains("]"))
                {
                    if (!isSectionItem && comment != "" && comment != "\r\n")
                        ConfItems.Add(new ConfItem() {
                            Comment = comment, File = Path.GetFileNameWithoutExtension(file)});

                    section = line.Substring(0, line.IndexOf("]") + 1);
                    comment = "";
                    isSectionItem = true;
                }
                else if (line.Contains("="))
                {
                    ConfItem item = new ConfItem();
                    item.File = Path.GetFileNameWithoutExtension(file);
                    item.IsSectionItem = isSectionItem;
                    item.Comment = comment;
                    comment = "";
                    item.Section = section;
                    section = "";

                    if (line.Contains("#") && !line.Contains("'") && !line.Contains("\""))
                    {
                        item.LineComment = line.Substring(line.IndexOf("#")).Trim();
                        line = line.Substring(0, line.IndexOf("#")).Trim();
                    }

                    int pos = line.IndexOf("=");
                    string left = line.Substring(0, pos).Trim().ToLower();
                    string right = line.Substring(pos + 1).Trim();

                    if (left == "fs")
                        left = "fullscreen";

                    if (left == "loop")
                        left = "loop-file";

                    item.Name = left;
                    item.Value = right;
                    ConfItems.Add(item);
                }
            }
        }

        string GetContent(string filename)
        {
            StringBuilder sb = new StringBuilder();
            List<string> namesWritten = new List<string>();

            foreach (ConfItem item in ConfItems)
            {
                if (filename != item.File || item.Section != "" || item.IsSectionItem)
                    continue;

                if (item.Comment != "")
                    sb.Append(item.Comment);

                if (item.SettingBase == null)
                {
                    if (item.Name != "")
                    {
                        sb.Append(item.Name + " = " + item.Value);

                        if (item.LineComment != "")
                            sb.Append(" " + item.LineComment);

                        sb.AppendLine();
                        namesWritten.Add(item.Name);
                    }
                }
                else if ((item.SettingBase.Value ?? "") != item.SettingBase.Default)
                {
                    string value = "";

                    if (item.SettingBase.Type == "string" ||
                        item.SettingBase.Type == "folder" ||
                        item.SettingBase.Type == "color")

                        value = "'" + item.SettingBase.Value + "'";
                    else
                        value = item.SettingBase.Value;

                    sb.Append(item.Name + " = " + value);

                    if (item.LineComment != "")
                        sb.Append(" " + item.LineComment);

                    sb.AppendLine();
                    namesWritten.Add(item.Name);
                }
            }

            if (!sb.ToString().Contains("# Editor"))
                sb.AppendLine("# Editor");

            foreach (SettingBase setting in SettingsDefinitions)
            {
                if (filename != setting.File || namesWritten.Contains(setting.Name))
                    continue;

                if ((setting.Value ?? "") != setting.Default)
                {
                    string value = "";

                    if (setting.Type == "string" ||
                        setting.Type == "folder" ||
                        setting.Type == "color")

                        value = "'" + setting.Value + "'";
                    else
                        value = setting.Value;

                    sb.AppendLine(setting.Name + " = " + value);
                }
            }

            foreach (ConfItem item in ConfItems)
            {
                if (filename != item.File || (item.Section == "" && !item.IsSectionItem))
                    continue;

                if (item.Section != "")
                {
                    if (!sb.ToString().EndsWith("\r\n\r\n"))
                        sb.AppendLine();

                    sb.AppendLine(item.Section);
                }

                if (item.Comment != "")
                    sb.Append(item.Comment);

                sb.Append(item.Name + " = " + item.Value);

                if (item.LineComment != "")
                    sb.Append(" " + item.LineComment);

                sb.AppendLine();
                namesWritten.Add(item.Name);
            }

            return "\r\n" + sb.ToString().Trim() + "\r\n";
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
            Msg.Show("mpv.conf Preview", GetContent("mpv"));
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