
using CommunityToolkit.Mvvm.ComponentModel;

namespace MpvNet;

public class Binding : ObservableObject
{
    public string Path { get; set; }
    public string Command { get; set; }
    public string Comment { get; set; }

    public bool IsMenu { get; set; }

    string _input = "";

    public Binding()
    {
        Path = Command = Comment = Input = "";
    }

    public Binding(string folder = "",
                   string name = "",
                   string command = "",
                   string input = "",
                   string comment = "")
    {
        if (folder != "" && name != "")
        {
            Path = folder + " > " + name;
            IsMenu = true;
        }
        else if (name != "")
        {
            Path = name;
            IsMenu = true;
        }
        else
            Path = "";

        Command = command;
        Input = input;
        Comment = comment == "" ? Path : comment;
    }

    public string Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }

    public bool IsEmpty() => Path == "" && Command == "" && Comment == "" && Input == "";
}
