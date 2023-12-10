
using MpvNet.ExtensionMethod;
using MpvNet.Help;

namespace MpvNet;

public class InputConf
{
    string? _path;

    public InputConf(string path) { Path = path; }

    public string Content { get; set; } = "";

    public string Path {
        get => _path ?? "";
        set {
            _path = value;
            Content = File.Exists(_path) ? FileHelp.ReadTextFile(_path) : "";
        }
    }

    public bool HasMenu => Content.Contains("#menu:") || Content.Contains("#! ");

    public (List<Binding> menuBindings, List<Binding>? confBindings) GetBindings()
    {
        var confbindings = InputHelp.Parse(Content);

        if (HasMenu)
            return (confbindings, confbindings);

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

        return (defaultBindings, confbindings);
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

    public void CreateBackup()
    {
        if (!File.Exists(Path))
            return;

        string targetPath = System.IO.Path.GetTempPath().AddSep() +
            "mpv.net input.conf backup " + Guid.NewGuid() + ".conf";

        File.Copy(Path, targetPath);
    }
}
