
using System.Windows;

namespace MpvNet.Windows;

public abstract class Setting
{
    public string? Default { get; set; }
    public string? File { get; set; }
    public string? Directory { get; set; }
    public string? Help { get; set; }
    public string? Name { get; set; }
    public string? StartValue { get; set; }
    public string? Type { get; set; }
    public string? URL { get; set; }
    public string? Value { get; set; }

    public int Width { get; set; }

    public ConfItem? ConfItem { get; set; }
}

public class StringSetting : Setting
{
}

public class OptionSetting : Setting
{
    public List<OptionSettingOption> Options { get; } = new List<OptionSettingOption>();
}

public class OptionSettingOption
{
    string? _text;

    public string? Name { get; set; }
    public string? Help { get; set; }

    public OptionSetting? OptionSetting { get; set; }

    public string? Text
    {
        get => string.IsNullOrEmpty(_text) ? Name : _text;
        set => _text = value;
    }

    public bool Checked
    {
        get => OptionSetting?.Value == Name;
        set
        {
            if (value)
                OptionSetting!.Value = Name;
        }
    }

    public Visibility Visibility
    {
        get => string.IsNullOrEmpty(Help) ? Visibility.Collapsed : Visibility.Visible;
    }
}
