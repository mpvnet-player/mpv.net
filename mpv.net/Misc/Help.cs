using System;

class ConsoleHelp
{
    public static void WriteError(object obj)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(obj?.ToString());
        Console.ResetColor();
    }
}