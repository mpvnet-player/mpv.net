
using System;
using System.Diagnostics;

public static class ConsoleHelp
{
    public static int Padding { get; set; }

    public static void WriteError(object obj, string module = null) => Write(obj, module, ConsoleColor.Red, false);    

    public static void Write(object obj, string module = "mpv.net")
    {
        Write(obj, module, ConsoleColor.Black, true);
    }

    public static void Write(object obj, string module, ConsoleColor color)
    {
        Write(obj, module, color, false);
    }

    public static void Write(object obj, string module, ConsoleColor color, bool useDefaultColor)
    {
        if (obj == null)
            return;

        string value = obj.ToString();               

        if (!string.IsNullOrEmpty(module))
            module = "[" + module + "] ";

        if (useDefaultColor)
            Console.ResetColor();
        else
            Console.ForegroundColor = color;

        value = module + value;

        if (Padding > 0 && value.Length < Padding)
            value = value.PadRight(Padding);

        Console.WriteLine(value);
        Console.ResetColor();
        Trace.WriteLine(obj);
    }
}
