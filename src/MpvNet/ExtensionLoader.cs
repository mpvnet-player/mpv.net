
using System.Reflection;

using MpvNet.ExtensionMethod;

namespace MpvNet;

public class ExtensionLoader
{
    public event Action<Exception>? UnhandledException;

    readonly List<object?> _refs = new();

    void LoadDll(string path)
    {
        if (!File.Exists(path))
            return;

        try
        {
            Assembly asm = Assembly.LoadFile(path);
            var type = asm.GetTypes().Where(typeof(IExtension).IsAssignableFrom).First();
            _refs.Add(Activator.CreateInstance(type));
        }
        catch (Exception ex)
        {
            UnhandledException?.Invoke(ex);
        }
    }

    public void LoadFolder(string path)
    {
        if (Directory.Exists(path))
            foreach (string dir in Directory.GetDirectories(path))
                LoadDll(dir.AddSep() + Path.GetFileName(dir) + ".dll");
    }
}

public interface IExtension
{
}
