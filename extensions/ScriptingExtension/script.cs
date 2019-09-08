using System.IO;

using mpvnet;

class Script
{
    public Script()
    {
        mp.Shutdown += Shutdown;
    }

    private void Shutdown()
    {
        foreach (string file in Directory.GetFiles(@"C:\Users\frank\Desktop\aaa"))
            File.Delete(file);
    }
}