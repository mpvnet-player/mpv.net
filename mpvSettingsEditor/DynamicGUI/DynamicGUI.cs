using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Tommy;

namespace DynamicGUI
{
    public class Settings
    {
        public static List<SettingBase> LoadSettings(string filepath)
        {
            TomlTable table;
            using (StreamReader reader = new StreamReader(File.OpenRead(filepath)))
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
                        opt.OptionSetting = optionSetting;
                        optionSetting.Options.Add(opt);
                    }
                }
                else if (setting["default"].IsString)
                {
                    StringSetting stringSetting = new StringSetting();
                    baseSetting = stringSetting;
                    stringSetting.Default = setting["default"];
                    if (setting.HasKey("folder")) stringSetting.IsFolder = true;
                }

                baseSetting.Name = setting["name"];
                if (setting.HasKey("help")) baseSetting.Help = setting["help"];
                if (setting.HasKey("helpurl")) baseSetting.HelpURL = setting["helpurl"];
                if (setting.HasKey("alias")) baseSetting.Alias = setting["alias"];
                if (setting.HasKey("width")) baseSetting.Width = setting["width"];
                settingsList.Add(baseSetting);
            }
            return settingsList;
        }
    }

    public abstract class SettingBase
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Help { get; set; }
        public string HelpURL { get; set; }
        public int Width { get; set; }
    }

    public class StringSetting : SettingBase
    {
        public string Default { get; set; }
        public string Value { get; set; }
        public bool IsFolder { get; set; }
    }

    public class OptionSetting : SettingBase
    {
        public string Default { get; set; }
        public string Value { get; set; }
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

        public bool IsChecked
        {
            get => OptionSetting.Value == Name ;
            set {
                if (value)
                    OptionSetting.Value = Name;
            }
        }
    }
}