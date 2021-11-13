
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

public static class TestStringExtension
{
    public static bool ContainsEx(this string instance, string value)
    {
        if (instance != null && value != null)
            return instance.Contains(value);

        return false;
    }

    public static bool StartsWithEx(this string instance, string value)
    {
        if (instance != null && value != null)
            return instance.StartsWith(value);

        return false;
    }
}

public static class ConvertToStringExtension
{
    public static string ToUpperEx(this string instance)
    {
        if (instance != null)
            return instance.ToUpperInvariant();

        return "";
    }

    public static string ToLowerEx(this string instance)
    {
        if (instance != null)
            return instance.ToLowerInvariant();

        return "";
    }

    public static string TrimEx(this string instance)
    {
        if (instance == null)
            return "";

        return instance.Trim();
    }
}

public static class ConvertStringExtension
{
    public static int ToInt(this string instance)
    {
        int.TryParse(instance, out int result);
        return result;
    }

    public static float ToFloat(this string instance)
    {
        float.TryParse(instance.Replace(",", "."), NumberStyles.Float,
            CultureInfo.InvariantCulture, out float result);

        return result;
    }
}

public static class PathStringExtension
{
    // returns the extension with lower case and without preceding dot.
    public static string Ext(this string instance)
    {
        if (instance == null)
            return "";

        return Path.GetExtension(instance).TrimStart('.').ToLower();
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
            instance = instance.Substring(0, 3) + "...\\" + instance.FileName();

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
