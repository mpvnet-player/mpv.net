
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DynamicGUI;

using static mpvnet.Global;

namespace mpvnet
{
    public partial class ConfWindow : Window
    {
        List<SettingBase> SettingsDefinitions = Conf.LoadConf(Properties.Resources.editor_conf);
        List<ConfItem> ConfItems = new List<ConfItem>();
        public ObservableCollection<string> FilterStrings { get; } = new ObservableCollection<string>();
        string InitialContent;

        public ConfWindow()
        {
            InitializeComponent();
            DataContext = this;
            SearchControl.SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            LoadConf(Core.ConfPath);
            LoadConf(App.ConfPath);
            LoadSettings();
            InitialContent = GetCompareString();
            SearchControl.Text = App.Settings.ConfigEditorSearch;
            FilterListBox.SelectedItem = SearchControl.Text.TrimEnd(':');
        }

        public Theme Theme => Theme.Current;

        void LoadSettings()
        {
            foreach (SettingBase setting in SettingsDefinitions)
            {
                setting.StartValue = setting.Value;

                if (!FilterStrings.Contains(setting.Filter))
                    FilterStrings.Add(setting.Filter);

                foreach (ConfItem confItem in ConfItems)
                {
                    if (setting.Name == confItem.Name && confItem.Section == "" && !confItem.IsSectionItem)
                    {
                        setting.Value = confItem.Value.Trim('\'', '"');
                        setting.StartValue = setting.Value;
                        setting.ConfItem = confItem;
                        confItem.SettingBase = setting;
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App.Settings.ConfigEditorSearch = SearchControl.Text;

            if (InitialContent == GetCompareString())
                return;

            File.WriteAllText(Core.ConfPath, GetContent("mpv"));
            File.WriteAllText(App.ConfPath, GetContent("mpvnet"));

            foreach (SettingBase item in SettingsDefinitions)
            {
                if (item.Value != item.StartValue)
                {
                    if (item.File == "mpv")
                    {
                        Core.ProcessProperty(item.Name, item.Value);

                        try
                        {
                            Core.SetPropertyString(item.Name, item.Value, true);
                        }
                        catch (Exception ex)
                        {
                            App.ShowError(ex.Message);
                        }
                    }
                    else if (item.File == "mpvnet")
                        App.ProcessProperty(item.Name, item.Value, true);
                }
            }

            App.InitTheme();
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
                    string value;

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
                    string value;

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

        void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
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
        
        void ConfWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            SearchControl.SearchTextBox.SelectAll();
            Keyboard.Focus(SearchControl.SearchTextBox);

            foreach (var i in MainStackPanel.Children.OfType<StringSettingControl>())
                i.Update();
        }

        void FilterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SearchControl.Text = e.AddedItems[0] + ":";
        }

        void OpenSettingsTextBlock_MouseUp(object sender, MouseButtonEventArgs e) =>
            ProcessHelp.ShellExecute(Path.GetDirectoryName(Core.ConfPath));

        void PreviewTextBlock_MouseUp(object sender, MouseButtonEventArgs e) =>
            Msg.ShowInfo("mpv.conf Preview", GetContent("mpv"));

        void ShowManualTextBlock_MouseUp(object sender, MouseButtonEventArgs e) =>
            ProcessHelp.ShellExecute("https://mpv.io/manual/master/");

        void SupportTextBlock_MouseUp(object sender, MouseButtonEventArgs e) =>
            ProcessHelp.ShellExecute("https://github.com/stax76/mpv.net/blob/master/docs/Manual.md#support");

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Close();
        }
    }
}
