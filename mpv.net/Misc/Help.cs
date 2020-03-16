
using System;
using System.Diagnostics;

class ConsoleHelp
{
    public static void WriteError(object obj)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[mpvnet] " + obj);
        Console.ResetColor();
        Trace.WriteLine(obj);
    }
}
