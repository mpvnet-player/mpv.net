
using Microsoft.Win32;

namespace MpvNet.Windows.Help;

public static class RegistryHelp
{
    static string? _appKey;

    public static string? ProductName { get; set; }

    public static string AppKey {
        get
        {
            if (ProductName == null)
                throw new Exception("ProductName cannot be null.");

            return _appKey ??= @"HKCU\Software\" + ProductName;
        }
    }

    public static void SetInt(string name, object value) => SetValue(AppKey, name, value);

    public static void SetString(string name, string value) => SetValue(AppKey, name, value);

    public static void SetValue(string name, object value)
    {
        using RegistryKey regKey = GetRootKey(AppKey).CreateSubKey(AppKey[5..], RegistryKeyPermissionCheck.ReadWriteSubTree);
        regKey.SetValue(name, value);
    }

    public static void SetValue(string path, string name, object value)
    {
        using RegistryKey regKey = GetRootKey(path).CreateSubKey(path[5..], RegistryKeyPermissionCheck.ReadWriteSubTree);
        regKey.SetValue(name, value);
    }

    public static string GetString(string name, string defaultValue = "") =>
        GetValue(AppKey, name, defaultValue)?.ToString() ?? defaultValue;

    public static int GetInt(string name, int defaultValue = 0) =>
        GetValue(AppKey, name, defaultValue) is int i ? i : defaultValue;

    public static object? GetValue(string name) => GetValue(AppKey, name, null);

    public static object? GetValue(string path, string name, object? defaultValue = null)
    {
        using RegistryKey? regKey = GetRootKey(path).OpenSubKey(path[5..]);
        return regKey?.GetValue(name, defaultValue);
    }

    public static void RemoveKey(string path)
    {
        try {
            GetRootKey(path).DeleteSubKeyTree(path[5..], false);
        } catch { }
    }

    public static void RemoveValue(string path, string name)
    {
        try {
            using RegistryKey? regKey = GetRootKey(path).OpenSubKey(path[5..], true);
            regKey?.DeleteValue(name, false);
        } catch { }
    }

    static RegistryKey GetRootKey(string path) => path[..4] switch
    {
        "HKLM" => Registry.LocalMachine,
        "HKCU" => Registry.CurrentUser,
        "HKCR" => Registry.ClassesRoot,
        _ => throw new Exception(),
    };
}
