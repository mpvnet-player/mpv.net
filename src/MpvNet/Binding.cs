
using CommunityToolkit.Mvvm.ComponentModel;

namespace MpvNet;

public class Binding : ObservableObject
{
    public string Path { get; set; }
    public string Command { get; set; }
    public string Comment { get; set; }

    public bool IsMenu { get; set; }

    public Binding()
    {
        Path = ""; Command = ""; Comment = "";
    }

    public Binding(string folder = "",
                   string name = "",
                   string command = "",
                   string input = "",
                   string comment = "")
    {
        if (folder != "" && name != "")
            Path = folder + " > " + name;
        else if (name != "")
            Path = name;
        else
            Path = "";

        Command = command;
        Input = input;
        Comment = comment == "" ? Path : comment;
    }

    string _input = "";

    public string Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }
}
