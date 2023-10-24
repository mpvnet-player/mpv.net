
using MpvNet.Help;

namespace MpvNet;

public class InputConf
{
    string? _content;
    bool? _hasMenu;

    public InputConf(string path) { Path = path; }

    public string Path { get; }

    public string Content
    {
        get => _content ??= FileHelp.ReadTextFile(Path);
        set => _content = value;
    }
        
    public bool HasMenu => _hasMenu ??= Content.Contains("#menu:");

    public List<Binding> GetMenuBindings()
    {
        var confbindings = InputHelp.Parse(Content);

        if (HasMenu)
            return confbindings;

        var defaultBindings = InputHelp.GetDefaults();

        foreach (Binding defaultBinding in defaultBindings)
            foreach (Binding confBinding in confbindings)
                if (defaultBinding.Input == confBinding.Input &&
                    defaultBinding.Command != confBinding.Command)
                {
                    defaultBinding.Input = "";
                }

        foreach (Binding defaultBinding in defaultBindings)
            foreach (Binding confBinding in confbindings)
                if (defaultBinding.Command == confBinding.Command)
                    defaultBinding.Input = confBinding.Input;

        return defaultBindings;
    }

    public string GetContent()
    {
        if (HasMenu)
            return Content;
        else
        {
            var defaults = InputHelp.GetDefaults();
            var removed = new List<Binding>();
            var conf = InputHelp.Parse(Content);

            foreach (Binding defaultBinding in defaults)
                foreach (Binding confBinding in conf)
                    if (defaultBinding.Command == confBinding.Command &&
                        defaultBinding.Comment == confBinding.Comment)
                    {
                        defaultBinding.Input = confBinding.Input;
                        removed.Add(confBinding);
                    }

            foreach (Binding binding in removed)
                conf.Remove(binding);

            defaults.AddRange(conf);
            return InputHelp.ConvertToString(defaults);
        }
    }
}
