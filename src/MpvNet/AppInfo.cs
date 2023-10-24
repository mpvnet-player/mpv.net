
using System.Reflection;

namespace MpvNet;

public static class AppInfo
{
    public static string Product => GetAssemblyAttribute<AssemblyProductAttribute>().Product;
    public static Version Version => Assembly.GetEntryAssembly()!.GetName().Version!;
    static T GetAssemblyAttribute<T>() => (T)(object)Assembly.GetEntryAssembly()!.GetCustomAttributes(typeof(T)).First();
}
