
using System.Globalization;
using System.IO;

public static class Extensions
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

    public static string ToUpperEx(this string instance)
    {
        if (instance != null)
            return instance.ToUpper();

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

    public static string Ext(this string instance)
    {
        if (instance == null)
            return "";

        return Path.GetExtension(instance).TrimStart('.').ToLower();
    }

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
