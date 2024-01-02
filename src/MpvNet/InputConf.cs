
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
            if (_path != value)
            {
                _path = value;
                Content = File.Exists(_path) ? FileHelp.ReadTextFile(_path) : "";
            }
        }
    }

    public bool HasMenu => Content.Contains(App.MenuSyntax + " ");

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
        {
            try
            {
                if (App.Settings.MenuUpdateVersion != 1)
                {
                    string updatedContent = UpdateContent(Content);

                    if (updatedContent != Content)
                    {
                        File.Copy(Path, Path + ".backup", true);
                        File.WriteAllText(Path, Content = updatedContent);
                    }

                    App.Settings.MenuUpdateVersion = 1;
                }
            }
            catch (Exception ex)
            {
                Terminal.WriteError("Failed to update menu." + BR + ex.Message);
            }

            return Content;
        }
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

    static string UpdateContent(string content) => content
        .Replace("script-message mpv.net", "script-message-to mpvnet")
        .Replace("/docs/Manual.md", "/docs/manual.md")
        .Replace("https://github.com/stax76/mpv.net", "https://github.com/mpvnet-player/mpv.net");
}
