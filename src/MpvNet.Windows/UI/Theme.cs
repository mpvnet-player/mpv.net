
using System.Windows;
using System.Windows.Media;

using Microsoft.Win32;

namespace MpvNet.Windows.UI;

public class Theme
{
    public string? Name { get; set; }
    public Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>();

    public static List<Theme>? DefaultThemes { get; set; }
    public static List<Theme>? CustomThemes { get; set; }

    public static Theme? Current { get; set; }

    public Brush? Background { get; set; }
    public Brush? Foreground { get; set; }
    public Brush? Foreground2 { get; set; }
    public Brush? Heading { get; set; }
    public Brush? MenuBackground { get; set; }
    public Brush? MenuHighlight { get; set; }

    public Color BackgroundColor { get; set; }
    public Color ForegroundColor { get; set; }
    public Color Foreground2Color { get; set; }
    public Color HeadingColor { get; set; }
    public Color MenuBackgroundColor { get; set; }
    public Color MenuHighlightColor { get; set; }

    public Brush GetBrush(string key)
    {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Dictionary[key]));
    }

    public Color GetColor(string key) => (Color)ColorConverter.ConvertFromString(Dictionary[key]);

    public static void Init()
    {
        string? themeContent = null;

        if (File.Exists(Player.ConfigFolder + "theme.conf"))
            themeContent = File.ReadAllText(Player.ConfigFolder + "theme.conf");

        Init(themeContent, Properties.Resources.theme, DarkMode ? App.DarkTheme : App.LightTheme);
    }

    public static void Init(string? customContent, string defaultContent, string activeTheme)
    {
        Current = null;

        DefaultThemes = Load(defaultContent);
        CustomThemes = Load(customContent);

        foreach (Theme theme in CustomThemes)
        {
            if (theme.Name == activeTheme)
            {
                bool isKeyMissing = false;

                foreach (string key in DefaultThemes[0].Dictionary.Keys)
                {
                    if (!theme.Dictionary.ContainsKey(key))
                    {
                        isKeyMissing = true;
                        Terminal.WriteError($"Theme '{activeTheme}' misses '{key}'");
                        break;
                    }
                }

                if (!isKeyMissing)
                    Current = theme;

                break;
            }
        }

        if (Current == null)
            foreach (Theme theme in DefaultThemes)
                if (theme.Name == activeTheme)
                    Current = theme;

        if (Current == null)
            Current = DefaultThemes[0];

        Current.Background = Current.GetBrush("background");
        Current.Foreground = Current.GetBrush("foreground");
        Current.Foreground2 = Current.GetBrush("foreground2");
        Current.Heading = Current.GetBrush("heading");
        Current.MenuBackground = Current.GetBrush("menu-background");
        Current.MenuHighlight = Current.GetBrush("menu-highlight");

        Current.BackgroundColor = Current.GetColor("background");
        Current.ForegroundColor = Current.GetColor("foreground");
        Current.Foreground2Color = Current.GetColor("foreground2");
        Current.HeadingColor = Current.GetColor("heading");
        Current.MenuBackgroundColor = Current.GetColor("menu-background");
        Current.MenuHighlightColor = Current.GetColor("menu-highlight");
    }

    static List<Theme> Load(string? content)
    {
        List<Theme> list = new List<Theme>();
        Theme? theme = null;

        foreach (string currentLine in (content ?? "").Split('\r', '\n'))
        {
            string line = currentLine.Trim();

            if (line.StartsWith("[") && line.EndsWith("]"))
                list.Add(theme = new Theme() { Name = line[1..^1].Trim() });

            if (line.Contains('=') && theme != null)
            {
                string left = line[..line.IndexOf("=")].Trim();
                theme.Dictionary[left] = line[(line.IndexOf("=") + 1)..].Trim();
            }
        }

        return list;
    }

    public static void UpdateWpfColors()
    {
        var dic = Application.Current.Resources;

        dic.Remove("BorderColor");
        dic.Add("BorderColor", Current!.GetColor("menu-highlight"));

        dic.Remove("RegionColor");
        dic.Add("RegionColor", Current.GetColor("menu-background"));

        dic.Remove("SecondaryRegionColor");
        dic.Add("SecondaryRegionColor", Current.GetColor("menu-highlight"));

        dic.Remove("PrimaryTextColor");
        dic.Add("PrimaryTextColor", Current.GetColor("menu-foreground"));

        dic.Remove("HighlightColor");
        dic.Add("HighlightColor", Current.GetColor("highlight"));
    }

    static bool DarkModeSystem
    {
        get
        {
            string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            return (int)(Registry.GetValue(key, "AppsUseLightTheme", 1) ?? 1) == 0;
        }
    }

    public static bool DarkMode => App.DarkMode == "system" && DarkModeSystem || App.DarkMode == "always";
}
