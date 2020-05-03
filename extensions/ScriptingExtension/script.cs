
using System.IO;

using mpvnet;
using static mpvnet.Core;

class Script
{
    public Script()
    {
        core.Shutdown += Shutdown;
    }

    void Shutdown()
    {
        foreach (string file in Directory.GetFiles(@"C:\Users\frank\Desktop\aaa"))
            File.Delete(file);
    }
}
