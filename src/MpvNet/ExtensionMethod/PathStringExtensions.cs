
namespace MpvNet.ExtensionMethod;

public static class PathStringExtensions
{
    extension(string filepath)
    {
        public string Ext => GetExt(filepath, false);

        static string GetExt(string path, bool includeDot)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            char[] chars = path.ToCharArray();

            for (int x = path.Length - 1; x >= 0; x--)
            {
                if (chars[x] == '/')
                    return "";

                if (chars[x] == '\\')
                    return "";

                if (chars[x] == '.')
                    return path[(x + (includeDot ? 0 : 1))..].ToLowerInvariant();
            }

            return "";
        }
    }

    public static string FileName(this string instance)
    {
        if (string.IsNullOrEmpty(instance))
            return "";

        int index = instance.LastIndexOf('\\');

        if (index > -1)
            return instance[(index + 1)..];

        index = instance.LastIndexOf('/');

        if (index > -1)
            return instance[(index + 1)..];

        return instance;
    }

    public static string ShortPath(this string instance, int maxLength)
    {
        if (string.IsNullOrEmpty(instance))
            return "";

        if (instance.Length > maxLength && instance.Substring(1, 2) == ":\\")
            instance = instance[..3] + "...\\" + instance.FileName();

        return instance;
    }

    // Ensure trailing directory separator char
    public static string AddSep(this string instance)
    {
        if (string.IsNullOrEmpty(instance))
            return "";

        if (!instance.EndsWith(Path.DirectorySeparatorChar.ToString()))
            instance = instance + Path.DirectorySeparatorChar;

        return instance;
    }
}
