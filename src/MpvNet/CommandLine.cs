
namespace MpvNet;

public class CommandLine
{
    static List<StringPair>? _arguments;

    static string[] _preInitProperties { get; } = {
        "input-terminal", "terminal", "input-file", "config", "o",
        "config-dir", "input-conf", "load-scripts", "scripts", "player-operation-mode",
        "idle", "log-file", "msg-color", "dump-stats", "msg-level", "really-quiet" };

    public static List<StringPair> Arguments
    {
        get
        {
            if (_arguments != null)
                return _arguments;

            _arguments = new();

            foreach (string i in Environment.GetCommandLineArgs().Skip(1))
            {
                string arg = i;

                if (!arg.StartsWith("--"))
                    continue;

                if (!arg.Contains('='))
                {
                    if (arg.Contains("--no-"))
                    {
                        arg = arg.Replace("--no-", "--");
                        arg += "=no";
                    }
                    else
                        arg += "=yes";
                }

                string left = arg[2..arg.IndexOf("=")];
                string right = arg[(left.Length + 3)..];

                if (string.IsNullOrEmpty(left))
                    continue;

                switch (left)
                {
                    case "script": left = "scripts"; break;
                    case "audio-file": left = "audio-files"; break;
                    case "sub-file": left = "sub-files"; break;
                    case "external-file": left = "external-files"; break;
                }

                _arguments.Add(new StringPair(left, right));
            }

            return _arguments;
        }
    }

    public static void ProcessCommandLineArgsPreInit()
    {
        foreach (var pair in Arguments)
        {
            if (pair.Name.EndsWith("-add") ||
                pair.Name.EndsWith("-set") ||
                pair.Name.EndsWith("-pre") ||
                pair.Name.EndsWith("-clr") ||
                pair.Name.EndsWith("-append") ||
                pair.Name.EndsWith("-remove") ||
                pair.Name.EndsWith("-toggle"))
            {
                continue;
            }

            Player.ProcessProperty(pair.Name, pair.Value);

            if (!App.ProcessProperty(pair.Name, pair.Value))
                Player.SetPropertyString(pair.Name, pair.Value);
        }
    }

    public static void ProcessCommandLineArgsPostInit()
    {
        foreach (var pair in Arguments)
        {
            if (_preInitProperties.Contains(pair.Name))
                continue;

            if (pair.Name.EndsWith("-add"))
                Player.CommandV("change-list", pair.Name[..^4], "add", pair.Value);
            else if (pair.Name.EndsWith("-set"))
                Player.CommandV("change-list", pair.Name[..^4], "set", pair.Value);
            else if (pair.Name.EndsWith("-append"))
                Player.CommandV("change-list", pair.Name[..^7], "append", pair.Value);
            else if (pair.Name.EndsWith("-pre"))
                Player.CommandV("change-list", pair.Name[..^4], "pre", pair.Value);
            else if (pair.Name.EndsWith("-clr"))
                Player.CommandV("change-list", pair.Name[..^4], "clr", "");
            else if (pair.Name.EndsWith("-remove"))
                Player.CommandV("change-list", pair.Name[..^7], "remove", pair.Value);
            else if (pair.Name.EndsWith("-toggle"))
                Player.CommandV("change-list", pair.Name[..^7], "toggle", pair.Value);
            else
            {
                Player.ProcessProperty(pair.Name, pair.Value);

                if (!App.ProcessProperty(pair.Name, pair.Value))
                    Player.SetPropertyString(pair.Name, pair.Value);
            }
        }
    }

    public static void ProcessCommandLineFiles()
    {
        List<string> files = new List<string>();

        foreach (string arg in Environment.GetCommandLineArgs().Skip(1))
            if (!arg.StartsWith("--") && (arg == "-" || arg.Contains("://") ||
                arg.Contains(":\\") || arg.StartsWith("\\\\") || File.Exists(arg)))

                files.Add(arg);

        Player.LoadFiles(files.ToArray(), !App.Queue, App.Queue);

        if (App.CommandLine.Contains("--shuffle"))
        {
            Player.Command("playlist-shuffle");
            Player.SetPropertyInt("playlist-pos", 0);
        }
    }

    public static bool Contains(string name)
    {
        foreach (StringPair pair in Arguments)
            if (pair.Name == name)
                return true;

        return false;
    }

    public static string GetValue(string name)
    {
        foreach (StringPair pair in Arguments)
            if (pair.Name == name)
                return pair.Value;

        return "";
    }
}
