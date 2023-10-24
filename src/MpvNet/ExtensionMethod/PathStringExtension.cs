
namespace MpvNet.ExtensionMethod;

public static class PathStringExtension
{
    public static string Ext(this string filepath) => filepath.Ext(false);

    public static string Ext(this string filepath, bool includeDot)
    {
        if (string.IsNullOrEmpty(filepath))
            return "";

        char[] chars = filepath.ToCharArray();

        for (int x = filepath.Length - 1; x >= 0; x--)
        {
            if (chars[x] == Path.DirectorySeparatorChar)
                return "";

            if (chars[x] == '.')
                return filepath.Substring(x + (includeDot ? 0 : 1)).ToLowerInvariant();
        }

        return "";
    }

    public static string FileName(this string instance)
    {
        if (string.IsNullOrEmpty(instance))
            return "";

        int index = instance.LastIndexOf('\\');

        if (index > -1)
            return instance.Substring(index + 1);

        index = instance.LastIndexOf('/');

        if (index > -1)
            return instance.Substring(index + 1);

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
