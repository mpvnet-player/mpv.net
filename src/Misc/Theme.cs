
using System.Collections.Generic;
using System.Windows.Media;

namespace mpvnet
{
    public class Theme
    {
        public string Name { get; set; }
        public Dictionary<string, string> Dictionary { get; } = new Dictionary<string, string>();

        public static List<Theme> DefaultThemes { get; set; }
        public static List<Theme> CustomThemes { get; set; }

        public static Theme Current { get; set; }

        public Brush Background { get; set; }
        public Brush Foreground { get; set; }
        public Brush Foreground2 { get; set; }
        public Brush Heading { get; set; }
        public Brush MenuBackground { get; set; }
        public Brush MenuHighlight { get; set; }

        public System.Drawing.Color GetWinFormsColor(string key)
        {
            return System.Drawing.ColorTranslator.FromHtml(Dictionary[key]);
        }

        public Brush GetBrush(string key)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Dictionary[key]));
        }

        public Color GetColor(string key)
        {
            return (Color)ColorConverter.ConvertFromString(Dictionary[key]);
        }

        public static void Init(string customContent, string defaultContent, string activeTheme)
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
        }

        static List<Theme> Load(string content)
        {
            List<Theme> list = new List<Theme>();
            Theme theme = null;

            foreach (string currentLine in (content ?? "").Split(new [] { '\r', '\n' }))
            {
                string line = currentLine.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                    list.Add(theme = new Theme() { Name = line.Substring(1, line.Length - 2).Trim() });

                if (line.Contains("=") && theme != null)
                {
                    string left = line.Substring(0, line.IndexOf("=")).Trim();
                    theme.Dictionary[left] = line.Substring(line.IndexOf("=") + 1).Trim();
                }
            }

            return list;
        }
    }
}
