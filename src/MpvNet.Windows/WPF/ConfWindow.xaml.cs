
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Input;

using MpvNet.Help;
using MpvNet.Windows.UI;
using MpvNet.Windows.WPF.Controls;
using MpvNet.Windows.WPF.ViewModels;

namespace MpvNet.Windows.WPF;

public partial class ConfWindow : Window, INotifyPropertyChanged
{
    List<Setting> Settings = Conf.LoadConf(Properties.Resources.editor_conf.TrimEnd());
    List<ConfItem> ConfItems = new List<ConfItem>();
    public ObservableCollection<string> FilterStrings { get; } = new();
    string InitialContent;
    string ThemeConf = GetThemeConf();
    string? _searchText;
    List<NodeViewModel>? _nodes;
    public event PropertyChangedEventHandler? PropertyChanged;

    public ConfWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadConf(Player.ConfPath);
        LoadConf(App.ConfPath);
        LoadSettings();
        InitialContent = GetCompareString();

        if (string.IsNullOrEmpty(App.Settings.ConfigEditorSearch))
            SearchControl.Text = "General:";
        else
            SearchControl.Text = App.Settings.ConfigEditorSearch;

        foreach (var node in Nodes)
            SelectNodeFromSearchText(node);

        foreach (var node in Nodes)
            ExpandNode(node);
    }

    public Theme? Theme => Theme.Current;

    public string SearchText
    {
        get => _searchText ?? "";
        set
        {
            _searchText = value;
            SearchTextChanged();
            OnPropertyChanged();
        }
    }

    public List<NodeViewModel> Nodes
    {
        get
        {
            if (_nodes == null)
            {
                var rootNode = new TreeNode();

                foreach (Setting setting in Settings)
                    AddNode(rootNode.Children, setting.Directory!);

                _nodes = new NodeViewModel(rootNode).Children;
            }

            return _nodes;
        }
    }

    public static TreeNode? AddNode(IList<TreeNode> nodes, string path)
    {
        string[] parts = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

        for (int x = 0; x < parts.Length; x++)
        {
            bool found = false;
 
            foreach (var node in nodes)
            {
                if (x < parts.Length - 1)
                {
                    if (node.Name == parts[x])
                    {
                        found = true;
                        nodes = node.Children;
                    }
                }
                else if (x == parts.Length - 1 && node.Name == parts[x])
                {
                    found = true;
                }
            }

            if (!found)
            {
                if (x == parts.Length - 1)
                {
                    var item = new TreeNode() { Name = parts[x] };
                    nodes?.Add(item);
                    return item;
                }
            }
        }

        return null;
    }

    void LoadSettings()
    {
        foreach (Setting setting in Settings)
        {
            setting.StartValue = setting.Value;

            if (!FilterStrings.Contains(setting.Directory!))
                FilterStrings.Add(setting.Directory!);

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
                    MainStackPanel.Children.Add(new StringSettingControl(s) { Visibility = Visibility.Collapsed });
                    break;
                case OptionSetting s:
                    MainStackPanel.Children.Add(new OptionSettingControl(s) { Visibility = Visibility.Collapsed });
                    break;
            }
        }
    }

    static string GetThemeConf() => Theme.DarkMode + App.DarkTheme + App.LightTheme;

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        App.Settings.ConfigEditorSearch = SearchControl.Text;

        if (InitialContent == GetCompareString())
            return;

        File.WriteAllText(Player.ConfPath, GetContent("mpv"));
        File.WriteAllText(App.ConfPath, GetContent("mpvnet"));

        foreach (Setting it in Settings)
        {
            if (it.Value != it.StartValue)
            {
                if (it.File == "mpv")
                {
                    Player.ProcessProperty(it.Name, it.Value);
                    Player.SetPropertyString(it.Name!, it.Value!);
                }
                else if (it.File == "mpvnet")
                    App.ProcessProperty(it.Name ?? "", it.Value ?? "", true);
            }
        }

        Theme.Init();
        Theme.UpdateWpfColors();

        if (ThemeConf != GetThemeConf())
            MessageBox.Show("Changed theme settings require mpv.net being restarted.", "Info");
    }

    bool _shown;

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (_shown)
            return;

        _shown = true;

        Application.Current.Dispatcher.BeginInvoke(() => {
            SearchControl.SearchTextBox.SelectAll();
        },
        DispatcherPriority.Background);
    }

    string GetCompareString()
    {
        return string.Join("", Settings.Select(item => item.Name + item.Value).ToArray());
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
                string? value;

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

        foreach (Setting setting in Settings)
        {
            if (filename != setting.File || namesWritten.Contains(setting.Name!))
                continue;

            if ((setting.Value ?? "") != setting.Default)
            {
                string? value;

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

    void SearchTextChanged()
    {
        string activeFilter = "";

        foreach (string i in FilterStrings)
            if (SearchText == i + ":")
                activeFilter = i;

        if (activeFilter == "")
        {
            foreach (UIElement i in MainStackPanel.Children)
                if ((i as ISettingControl)!.Contains(SearchText) && SearchText.Length > 1)
                    i.Visibility = Visibility.Visible;
                else
                    i.Visibility = Visibility.Collapsed;

            foreach (var node in Nodes)
                UnselectNode(node);
        }
        else
            foreach (UIElement i in MainStackPanel.Children)
                if ((i as ISettingControl)!.Setting.Directory == activeFilter)
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

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Close();

        if (e.Key == Key.F3 || e.Key == Key.F6 || (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl)))
        {
            Keyboard.Focus(SearchControl.SearchTextBox);
            SearchControl.SearchTextBox.SelectAll();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var node = TreeView.SelectedItem as NodeViewModel;

        if (node == null)
            return;

        Application.Current.Dispatcher.BeginInvoke(() => {
            SearchText = node!.Path + ":";
        },
        DispatcherPriority.Background);
    }

    void SelectNodeFromSearchText(NodeViewModel node)
    {
        if (node.Path + ":" == SearchControl.Text)
        {
            node.IsSelected = true;
            return;
        }

        foreach (var it in node.Children)
            SelectNodeFromSearchText(it);
    }

    void UnselectNode(NodeViewModel node)
    {
        if (node.IsSelected)
            node.IsSelected = false;

        foreach (var it in node.Children)
            UnselectNode(it);
    }

    void ExpandNode(NodeViewModel node)
    {
        node.IsExpanded = true;

        foreach (var it in node.Children)
            ExpandNode(it);
    }

    [RelayCommand] void ShowMpvNetSpecificSettings() => SearchControl.Text = "mpv.net";

    [RelayCommand] void PreviewMpvConfFile() => Msg.ShowInfo(GetContent("mpv"));
    
    [RelayCommand] void PreviewMpvNetConfFile() => Msg.ShowInfo(GetContent("mpvnet"));

    [RelayCommand] void ShowMpvManual() => ProcessHelp.ShellExecute("https://mpv.io/manual/master/");
    
    [RelayCommand] void ShowMpvNetManual() => ProcessHelp.ShellExecute("https://github.com/mpvnet-player/mpv.net/blob/main/docs/manual.md");
}
