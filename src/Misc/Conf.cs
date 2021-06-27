
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace mpvnet
{
    public class Conf
    {
        public static List<SettingBase> LoadConf(string content)
        {
            List<SettingBase> settingsList = new List<SettingBase>();

            foreach (ConfSection section in ConfParser.Parse(content))
            {
                SettingBase baseSetting = null;

                if (section.HasName("option"))
                {
                    OptionSetting optionSetting = new OptionSetting();
                    baseSetting = optionSetting;
                    optionSetting.Default = section.GetValue("default");
                    optionSetting.Value = optionSetting.Default;

                    foreach (var i in section.GetValues("option"))
                    {
                        var opt = new OptionSettingOption();

                        if (i.Value.Contains(" "))
                        {
                            opt.Name = i.Value.Substring(0, i.Value.IndexOf(" "));
                            opt.Help = i.Value.Substring(i.Value.IndexOf(" ")).Trim();
                        }
                        else
                            opt.Name = i.Value;

                        if (opt.Name == optionSetting.Default)
                            opt.Text = opt.Name + " (Default)";

                        opt.OptionSetting = optionSetting;
                        optionSetting.Options.Add(opt);
                    }
                }
                else
                {
                    StringSetting stringSetting = new StringSetting();
                    baseSetting = stringSetting;
                    stringSetting.Default = section.HasName("default") ? section.GetValue("default") : "";
                }

                baseSetting.Name   = section.GetValue("name");
                baseSetting.File   = section.GetValue("file");
                baseSetting.Filter = section.GetValue("filter");

                if (section.HasName("help"))  baseSetting.Help = section.GetValue("help");
                if (section.HasName("url"))   baseSetting.URL = section.GetValue("url");
                if (section.HasName("width")) baseSetting.Width = Convert.ToInt32(section.GetValue("width"));
                if (section.HasName("type"))  baseSetting.Type = section.GetValue("type");

                settingsList.Add(baseSetting);
            }

            return settingsList;
        }
    }

    public class ConfItem
    {
        public string Comment { get; set; } = "";
        public string File { get; set; } = "";
        public string LineComment { get; set; } = "";
        public string Name { get; set; } = "";
        public string Section { get; set; } = "";
        public string Value { get; set; } = "";

        public bool IsSectionItem { get; set; }
        public SettingBase SettingBase { get; set; }
    }

    public abstract class SettingBase
    {
        public string Default { get; set; }
        public string File { get; set; }
        public string Filter { get; set; }
        public string Help { get; set; }
        public string Name { get; set; }
        public string StartValue { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
        public string Value { get; set; }

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

        string _Text;

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
        void HyperLinkEx_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ProcessHelp.ShellExecute(e.Uri.AbsoluteUri);
        }

        public void SetURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            NavigateUri = new Uri(url);
            RequestNavigate += HyperLinkEx_RequestNavigate;
            Inlines.Clear();
            Inlines.Add(url);
        }
    }
}
