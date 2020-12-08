
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace mpvnet
{
    public static class ProcessHelp
    {
        public static void Execute(string file, string arguments = null)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = file;
                proc.StartInfo.Arguments = arguments;
                // default is true in .NET Framework and false in .NET Core
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
            }
        }

        public static void ShellExecute(string file, string arguments = null)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = file;
                proc.StartInfo.Arguments = arguments;
                // default is true in .NET Framework and false in .NET Core
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }
    }

    public static class ConsoleHelp
    {
        public static int Padding { get; set; }

        public static void WriteError(object obj, string module = "mpv.net")
        {
            Write(obj, module, ConsoleColor.Red, false);    
        }

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

            if (color == ConsoleColor.Red)
                Console.Error.WriteLine(value);
            else
                Console.WriteLine(value);

            Console.ResetColor();
            Trace.WriteLine(obj);
        }
    }

    public class CursorHelp
    {
        static bool IsVisible = true;

        public static void Show()
        {
            if (!IsVisible)
            {
                Cursor.Show();
                IsVisible = true;
            }
        }

        public static void Hide()
        {
            if (IsVisible)
            {
                Cursor.Hide();
                IsVisible = false;
            }
        }

        public static bool IsPosDifferent(Point screenPos)
        {
            return
                Math.Abs(screenPos.X - Control.MousePosition.X) > 10 ||
                Math.Abs(screenPos.Y - Control.MousePosition.Y) > 10;
        }
    }

    public class mpvHelp
    {
        public static string WM_APPCOMMAND_to_mpv_key(int value)
        {
            switch (value)
            {
                case 5:  return "SEARCH";       // BROWSER_SEARCH
                case 6:  return "FAVORITES";    // BROWSER_FAVORITES
                case 7:  return "HOMEPAGE";     // BROWSER_HOME
                case 15: return "MAIL";         // LAUNCH_MAIL
                case 33: return "PRINT";        // PRINT
                case 11: return "NEXT";         // MEDIA_NEXTTRACK
                case 12: return "PREV";         // MEDIA_PREVIOUSTRACK
                case 13: return "STOP";         // MEDIA_STOP
                case 14: return "PLAYPAUSE";    // MEDIA_PLAY_PAUSE
                case 46: return "PLAY";         // MEDIA_PLAY
                case 47: return "PAUSE";        // MEDIA_PAUSE
                case 48: return "RECORD";       // MEDIA_RECORD
                case 49: return "FORWARD";      // MEDIA_FAST_FORWARD
                case 50: return "REWIND";       // MEDIA_REWIND
                case 51: return "CHANNEL_UP";   // MEDIA_CHANNEL_UP
                case 52: return "CHANNEL_DOWN"; // MEDIA_CHANNEL_DOWN
            }

            return null;
        }
    }
}
