using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

using Tommy;

namespace DynamicGUI
{
    public class Settings
    {
        public static List<SettingBase> LoadSettings(string content)
        {
            TomlTable table;

            using (StringReader reader = new StringReader(content))
                table = TOML.Parse(reader);

            List<SettingBase> settingsList = new List<SettingBase>(); 

            foreach (TomlTable setting in table["settings"])
            {
                SettingBase baseSetting = null;

                if (setting.HasKey("options"))
                {
                    OptionSetting optionSetting = new OptionSetting();
                    baseSetting = optionSetting;
                    optionSetting.Default = setting["default"];
                    optionSetting.Value = optionSetting.Default;

                    foreach (TomlTable option in setting["options"])
                    {
                        var opt = new OptionSettingOption();
                        opt.Name = option["name"];

                        if (option.HasKey("help"))
                            opt.Help = option["help"];

                        if (option.HasKey("text"))
                            opt.Text = option["text"];
                        else if (opt.Name == optionSetting.Default)
                            opt.Text = opt.Name + " (Default)";

                        opt.OptionSetting = optionSetting;
                        optionSetting.Options.Add(opt);
                    }
                }
                else
                {
                    StringSetting stringSetting = new StringSetting();
                    baseSetting = stringSetting;
                    stringSetting.Default = setting.HasKey("default") ? setting["default"].ToString() : "";
                }

                baseSetting.Name = setting["name"];
                baseSetting.File = setting["file"];
                baseSetting.Filter = setting["filter"];

                if (setting.HasKey("help")) baseSetting.Help = setting["help"];
                if (setting.HasKey("url")) baseSetting.URL = setting["url"];
                if (setting.HasKey("width")) baseSetting.Width = setting["width"];
                if (setting.HasKey("type")) baseSetting.Type = setting["type"];

                settingsList.Add(baseSetting);
            }
            return settingsList;
        }
    }

    public class ConfItem
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Comment { get; set; } = "";
        public string LineComment { get; set; } = "";
        public string Section { get; set; } = "";
        public string File { get; set; } = "";
        public bool IsSectionItem { get; set; }
        public SettingBase SettingBase { get; set; }
    }

    public abstract class SettingBase
    {
        public string Name { get; set; }
        public string File { get; set; }
        public string Value { get; set; }
        public string Help { get; set; }
        public string Default { get; set; }
        public string URL { get; set; }
        public string Filter { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public ConfItem ConfItem { get; set; }
    }

    public class StringSetting : SettingBase
    {
    }

    public class OptionSetting : SettingBase
    {
        public List<OptionSettingOption> Options = new List<OptionSettingOption>();
    }

    public class OptionSettingOption
    {
        public string Name { get; set; }
        public string Help { get; set; }

        public OptionSetting OptionSetting { get; set; }

        private string _Text;

        public string Text
        {
            get => string.IsNullOrEmpty(_Text) ? Name : _Text;
            set => _Text = value;
        }

        public bool Checked
        {
            get => OptionSetting.Value == Name ;
            set {
                if (value)
                    OptionSetting.Value = Name;
            }
        }

        public Visibility Visibility
        {
            get => string.IsNullOrEmpty(Help) ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    interface ISettingControl
    {
        bool Contains(string searchString);
        SettingBase SettingBase { get; }
    }

    public class HyperlinkEx : Hyperlink
    {
        private void HyperLinkEx_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }

        public void SetURL(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            NavigateUri = new Uri(url);
            RequestNavigate += HyperLinkEx_RequestNavigate;
            Inlines.Clear();
            Inlines.Add(url);
        }
    }
}