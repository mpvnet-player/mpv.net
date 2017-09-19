using System;
using System.Linq;
using System.IO;

public static class StringExtensions
{
    //    public static string Multiply(this string instance, int multiplier)
    //    {
    //        StringBuilder sb = new StringBuilder(multiplier * instance.Length);

    //        for (var i = 0; i <= multiplier - 1; i++)
    //        {
    //            sb.Append(instance);
    //        }

    //        return sb.ToString();
    //    }

    //    public static bool IsValidFileName(string instance)
    //    {
    //        if (string.IsNullOrEmpty(instance))
    //            return false;

    //        string chars = "\"*/:<>?\\|";

    //        foreach (var i in instance)
    //        {
    //            if (chars.Contains(i.ToString()))
    //                return false;

    //            if (Convert.ToInt32(i) < 32)
    //                return false;
    //        }

    //        return true;
    //    }

    //    [Extension()]
    //    public static string FileName(string instance)
    //    {
    //        if (string.IsNullOrEmpty(instance))
    //            return "";
    //        dynamic index = instance.LastIndexOf(Path.DirectorySeparatorChar);
    //        if (index > -1)
    //            return instance.Substring(index + 1);
    //        return instance;
    //    }

    //    [Extension()]
    //    public static string Upper(string instance)
    //    {
    //        if (string.IsNullOrEmpty(instance))
    //            return "";
    //        return instance.ToUpperInvariant;
    //    }

    //    [Extension()]
    //    public static string Lower(string instance)
    //    {
    //        if (string.IsNullOrEmpty(instance))
    //            return "";
    //        return instance.ToLowerInvariant;
    //    }

    //    [Extension()]
    //    public static string ChangeExt(string instance, string value)
    //    {
    //        if (string.IsNullOrEmpty(instance))
    //            return "";
    //        if (string.IsNullOrEmpty(value))
    //            return instance;
    //        if (!value.StartsWith("."))
    //            value = "." + value;
    //        return instance.DirAndBase + value.ToLower;
    //    }

    //    [Extension()]
    //    public static string Escape(string instance)
    //    {
    //        if (string.IsNullOrEmpty(instance))
    //            return "";

    //        dynamic chars = " ()".ToCharArray;

    //        foreach (void i_loopVariable in chars)
    //        {
    //            i = i_loopVariable;
    //            if (instance.Contains(i))
    //                return "\"" + instance + "\"";
    //        }

    //        return instance;
    //    }

    //    [Extension()]
    //    public static string Parent(string instance)
    //    {
    //        return DirPath.GetParent(instance);
    //    }

    //    [Extension()]
    //    public static string ExistingParent(string instance)
    //    {
    //        dynamic ret = instance.Parent;
    //        if (!Directory.Exists(ret))
    //            ret = ret.Parent;
    //        else
    //            return ret;
    //        if (!Directory.Exists(ret))
    //            ret = ret.Parent;
    //        else
    //            return ret;
    //        if (!Directory.Exists(ret))
    //            ret = ret.Parent;
    //        else
    //            return ret;
    //        if (!Directory.Exists(ret))
    //            ret = ret.Parent;
    //        else
    //            return ret;
    //        if (!Directory.Exists(ret))
    //            ret = ret.Parent;
    //        else
    //            return ret;
    //        return ret;
    //    }

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

    //    [Extension()]
    //    public static string Base(string instance)
    //    {
    //        return FilePath.GetBase(instance);
    //    }

    //    [Extension()]
    //    public static string Dir(string instance)
    //    {
    //        return FilePath.GetDir(instance);
    //    }

    //    [Extension()]
    //    public static string DirName(string instance)
    //    {
    //        return DirPath.GetName(instance);
    //    }

    //    [Extension()]
    //    public static string DirAndBase(string instance)
    //    {
    //        return FilePath.GetDirAndBase(instance);
    //    }

    //    [Extension()]
    //    public static bool ContainsAll(string instance, IEnumerable<string> all)
    //    {
    //        if (!string.IsNullOrEmpty(instance))
    //            return all.All(arg => instance.Contains(arg));
    //    }

    //    [Extension()]
    //    public static bool ContainsAny(string instance, IEnumerable<string> any)
    //    {
    //        if (!string.IsNullOrEmpty(instance))
    //            return any.Any(arg => instance.Contains(arg));
    //    }

    //    [Extension()]
    //    public static string ToTitleCase(string value)
    //    {
    //        //TextInfo.ToTitleCase won't work on all upper strings
    //        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower);
    //    }

    //    [Extension()]
    //    public static bool IsInt(string value)
    //    {
    //        return int.TryParse(value, null);
    //    }

    //    [Extension()]
    //    public static int ToInt(string value, int defaultValue = 0)
    //    {
    //        if (!int.TryParse(value, null))
    //            return defaultValue;
    //        return Convert.ToInt32(value);
    //    }

    //    [Extension()]
    //    public static bool IsSingle(string value)
    //    {
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            if (value.Contains(","))
    //                value = value.Replace(",", ".");

    //            return float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, null);
    //        }
    //    }

    //    [Extension()]
    //    public static float ToSingle(string value, float defaultValue = 0)
    //    {
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            if (value.Contains(","))
    //                value = value.Replace(",", ".");

    //            float ret = 0;

    //            if (float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, ret))
    //            {
    //                return ret;
    //            }
    //        }

    //        return defaultValue;
    //    }

    //    [Extension()]
    //    public static bool IsDouble(string value)
    //    {
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            if (value.Contains(","))
    //                value = value.Replace(",", ".");

    //            return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, null);
    //        }
    //    }

    //    [Extension()]
    //    public static double ToDouble(string value, float defaultValue = 0)
    //    {
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            if (value.Contains(","))
    //                value = value.Replace(",", ".");

    //            double ret = 0;

    //            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, ret))
    //            {
    //                return ret;
    //            }
    //        }

    //        return defaultValue;
    //    }

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

    //    [Extension()]
    //    public static bool EqualIgnoreCase(string a, string b)
    //    {
    //        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
    //            return false;
    //        return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
    //    }

    //    [Extension()]
    //    public static string Shorten(string value, int maxLength)
    //    {
    //        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
    //        {
    //            return value;
    //        }

    //        return value.Substring(0, maxLength);
    //    }
    
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

    //    [Extension()]
    //    public static string RemoveChars(string value, string chars)
    //    {
    //        dynamic ret = value;

    //        foreach (void i_loopVariable in value)
    //        {
    //            i = i_loopVariable;
    //            if (chars.IndexOf(i) >= 0)
    //            {
    //                ret = ret.Replace(i, "");
    //            }
    //        }

    //        return ret;
    //    }

    //    [Extension()]
    //    public static string DeleteRight(string value, int count)
    //    {
    //        return Strings.Left(value, value.Length - count);
    //    }

    //    [Extension()]
    //    public static string ReplaceUnicode(string value)
    //    {
    //        if (value.Contains(Convert.ToChar(0x2212)))
    //        {
    //            value = value.Replace(Convert.ToChar(0x2212), '-');
    //        }

    //        return value;
    //    }

    //    [Extension()]
    //    public static void ToClipboard(string value)
    //    {
    //        if (!string.IsNullOrEmpty(value))
    //        {
    //            Clipboard.SetText(value);
    //        }
    //        else
    //        {
    //            Clipboard.Clear();
    //        }
    //    }
    //}

    public class DirPath : PathBase
    {

    //    public static string TrimTrailingSeparator(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //            return "";

    //        if (path.EndsWith(Separator) && !(path.Length <= 3))
    //        {
    //            return path.TrimEnd(Separator);
    //        }

    //        return path;
    //    }

    //    public static string FixSeperator(string path)
    //    {
    //        if (path.Contains("\\") && Separator != "\\")
    //        {
    //            path = path.Replace("\\", Separator);
    //        }

    //        if (path.Contains("/") && Separator != "/")
    //        {
    //            path = path.Replace("/", Separator);
    //        }

    //        return path;
    //    }

    //    public static string GetParent(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //            return "";
    //        dynamic temp = TrimTrailingSeparator(path);
    //        if (temp.Contains(Separator))
    //            path = temp.LeftLast(Separator) + Separator;
    //        return path;
    //    }

    //    public static string GetName(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //            return "";
    //        path = TrimTrailingSeparator(path);
    //        return path.RightLast(Separator);
    //    }

    //    public static bool IsFixedDrive(string path)
    //    {
    //        try
    //        {
    //            if (!string.IsNullOrEmpty(path))
    //                return new DriveInfo(path).DriveType == DriveType.Fixed;
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //    }
    }

    public class FilePath : PathBase
    {


        //    private string Value;
        //    public FilePath(string path)
        //    {
        //        Value = path;
        //    }

        //    public static string GetDir(string path)
        //    {
        //        if (string.IsNullOrEmpty(path))
        //            return "";
        //        if (path.Contains("\\"))
        //            path = path.LeftLast("\\") + "\\";
        //        return path;
        //    }

        //    public static string GetDirAndBase(string path)
        //    {
        //        return GetDir(path) + GetBase(path);
        //    }

        //    public static string GetName(string path)
        //    {
        //        if ((path != null))
        //        {
        //            dynamic index = path.LastIndexOf(IO.Path.DirectorySeparatorChar);

        //            if (index > -1)
        //            {
        //                return path.Substring(index + 1);
        //            }
        //        }

        //        return path;
        //    }

        //    public static string GetDirNoSep(string path)
        //    {
        //        path = GetDir(path);
        //        if (path.EndsWith(Separator))
        //            path = TrimSep(path);
        //        return path;
        //    }

        //    public static string GetBase(string path)
        //    {
        //        if (string.IsNullOrEmpty(path))
        //            return "";
        //        dynamic ret = path;
        //        if (ret.Contains(Separator))
        //            ret = ret.RightLast(Separator);
        //        if (ret.Contains("."))
        //            ret = ret.LeftLast(".");
        //        return ret;
        //    }

        //    public static string TrimSep(string path)
        //    {
        //        if (string.IsNullOrEmpty(path))
        //            return "";

        //        if (path.EndsWith(Separator) && !path.EndsWith(":" + Separator))
        //        {
        //            return path.TrimEnd(Separator);
        //        }

        //        return path;
        //    }

        //    public static string GetDirNameOnly(string path)
        //    {
        //        return FilePath.GetDirNoSep(path).RightLast("\\");
        //    }
    }

    public class PathBase
    {
        //public static char Separator {
        //    get { return Path.DirectorySeparatorChar; }
        //}

        //    public static bool IsSameBase(string a, string b)
        //    {
        //        return FilePath.GetBase(a).EqualIgnoreCase(FilePath.GetBase(b));
        //    }

        //    public static bool IsSameDir(string a, string b)
        //    {
        //        return FilePath.GetDir(a).EqualIgnoreCase(FilePath.GetDir(b));
        //    }

        //    public static bool IsValidFileSystemName(string name)
        //    {
        //        if (string.IsNullOrEmpty(name))
        //            return false;
        //        dynamic chars = "\"*/:<>?\\|^".ToCharArray;

        //        foreach (void i_loopVariable in name.ToCharArray)
        //        {
        //            i = i_loopVariable;
        //            if (chars.Contains(i))
        //                return false;
        //            if (Convert.ToInt32(i) < 32)
        //                return false;
        //        }

        //        return true;
        //    }

        //    public static string RemoveIllegalCharsFromName(string name)
        //    {
        //        if (string.IsNullOrEmpty(name))
        //            return "";

        //        dynamic chars = "\"*/:<>?\\|^".ToCharArray;

        //        foreach (void i_loopVariable in name.ToCharArray)
        //        {
        //            i = i_loopVariable;
        //            if (chars.Contains(i))
        //            {
        //                name = name.Replace(i, "_");
        //            }
        //        }

        //        for (x = 1; x <= 31; x++)
        //        {
        //            if (name.Contains(Convert.ToChar(x)))
        //            {
        //                name = name.Replace(Convert.ToChar(x), '_');
        //            }
        //        }

        //        return name;
    }
}