
using System.Globalization;

namespace MpvNet.Extensions;

public static class StringExtensions
{
    public static string ToUpperEx(this string instance) => instance?.ToUpperInvariant() ?? "";

    public static string ToLowerEx(this string instance) => instance?.ToLowerInvariant() ?? "";

    public static string TrimEx(this string? instance) => instance?.Trim() ?? "";

    public static int ToInt(this string instance, int defaultValue = 0)
    {
        if (int.TryParse(instance, out int result))
            return result;

        return defaultValue;
    }

    public static float ToFloat(this string instance) =>
        float.Parse(instance.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

    public static bool ContainsEx(this string? instance, string? value) =>
        !string.IsNullOrEmpty(instance) && !string.IsNullOrEmpty(value) && instance.Contains(value);

    public static bool StartsWithEx(this string? instance, string? value) =>
        (instance != null && value != null) && instance.StartsWith(value);
}
