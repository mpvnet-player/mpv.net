
namespace MpvNet;

public class CommandLine
{
    static List<StringPair>? _arguments;

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
