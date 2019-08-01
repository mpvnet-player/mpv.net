using System.Globalization;
using System.IO;

public static class Extensions
{
    public static string FileName(this string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        int index = path.LastIndexOf('\\');
        if (index > -1) return path.Substring(index + 1);
        index = path.LastIndexOf('/');
        if (index > -1) return path.Substring(index + 1);
        return path;
    }

    public static string ShortExt(this string path)
    {
        return Path.GetExtension(path).ToLower().TrimStart('.');
    }

    public static int Int(this string value)
    {
        int.TryParse(value, out int result);
        return result;
    }

    public static float Float(this string value)
    {
        float.TryParse(value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out float result);
        return result;
    }
}