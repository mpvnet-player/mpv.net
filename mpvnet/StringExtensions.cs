using System;
using System.Linq;
using System.IO;

public static class StringExtensions
{
    public static string ExtFull(this string filepath)
    {
        return Ext(filepath, true);
    }

    public static string Ext(this string filepath)
    {
        return Ext(filepath, false);
    }

    public static string Ext(this string filepath, bool dot)
    {
        if (string.IsNullOrEmpty(filepath))
            return "";

        var chars = filepath.ToCharArray();

        for (var x = filepath.Length - 1; x >= 0; x += -1)
        {
            if (chars[x] == Path.DirectorySeparatorChar)
                return "";

            if (chars[x] == '.')
                return filepath.Substring(x + (dot ? 0 : 1)).ToLower();
        }

        return "";
    }

    public static string Left(this string value, int index)
    {
        if (string.IsNullOrEmpty(value) || index < 0)
            return "";

        if (index > value.Length)
            return value;

        return value.Substring(0, index);
    }

    public static string Left(this string value, string start)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(start))
            return "";

        if (!value.Contains(start))
            return "";

        return value.Substring(0, value.IndexOf(start));
    }

    public static string LeftLast(this string value, string start)
    {
        if (!value.Contains(start))
            return "";

        return value.Substring(0, value.LastIndexOf(start));
    }

    public static string Right(this string value, string start)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(start))
            return "";

        if (!value.Contains(start))
            return "";

        return value.Substring(value.IndexOf(start) + start.Length);
    }

    public static string RightLast(this string value, string start)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(start))
            return "";

        if (!value.Contains(start))
            return "";

        return value.Substring(value.LastIndexOf(start) + start.Length);
    }
    
    public static string[] SplitNoEmpty(this string value, params string[] delimiters)
    {
        return value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] SplitKeepEmpty(this string value, params string[] delimiters)
    {
        return value.Split(delimiters, StringSplitOptions.None);
    }

    public static string[] SplitNoEmptyAndWhiteSpace(this string value, params string[] delimiters)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var a = SplitNoEmpty(value, delimiters);

        for (var i = 0; i <= a.Length - 1; i++)
            a[i] = a[i].Trim();

        var l = a.ToList();

        while (l.Contains(""))
            l.Remove("");

        return l.ToArray();
    }

    public static string[] SplitLinesNoEmpty(this string value)
    {
        return SplitNoEmpty(value, Environment.NewLine);
    }
}