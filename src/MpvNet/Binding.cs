
using CommunityToolkit.Mvvm.ComponentModel;

namespace MpvNet;

public class Binding : ObservableObject
{
    public string Command { get; set; }
    public string Comment { get; set; }

    public bool IsCustomMenu { get; set; }
    public bool IsMenu { get; set; }
    public bool IsShortMenuSyntax { get; set; }

    string _input = "";

    public Binding()
    {
        Command = Comment = Input = "";
    }

    public Binding(string folder = "",
                   string name = "",
                   string command = "",
                   string input = "",
                   string comment = "")
    {
        if (folder != "" && name != "")
        {
            Comment = folder + " > " + name;
            IsMenu = true;
        }
        else if (name != "")
        {
            Comment = name;
            IsMenu = true;
        }
        else
            Comment = comment;

        Command = command;
        Input = input;
    }

    public string Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }

    public bool IsEmpty() => Command == "" && Comment == "" && Input == "";
}
