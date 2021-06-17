
// C# Script that deletes the current file

// In input.conf add:

// KP0 script-message delete-current-file ask      #menu: Script > Delete current file > Ask
// KP1 script-message delete-current-file confirm  #menu: Script > Delete current file > Confirm

using System;
using System.IO;
using System.Threading;

using Microsoft.VisualBasic.FileIO;

using mpvnet;

class Script
{
    string FileToDelete;
    DateTime DeleteTime;
    CorePlayer Core;

    public Script()
    {
        Core = Global.Core;
        Core.ClientMessage += ClientMessage;
    }

    void ClientMessage(string[] args)
    {
        if (args == null || args.Length != 2 || args[0] != "delete-current-file")
            return;

        if (args[1] == "ask")
        {
            FileToDelete = Core.get_property_string("path");
            DeleteTime = DateTime.Now;
            Core.commandv("show-text", "Press 1 to delete file", "10000");
        }
        else if (args[1] == "confirm")
        {
            TimeSpan ts = DateTime.Now - DeleteTime;
            string path = Core.get_property_string("path");

            if (FileToDelete == path && ts.TotalSeconds < 10 && File.Exists(FileToDelete))
            {
                Core.command("playlist-remove current");
                int pos = Core.get_property_int("playlist-pos");

                if (pos == -1)
                {
                    int count = Core.get_property_int("playlist-count");

                    if (count > 0)
                        Core.set_property_int("playlist-pos", count - 1);
                    else
                    {
                        Core.ShowLogo();
                        Core.commandv("show-text", "");
                    }
                }

                Thread.Sleep(2000);
                FileSystem.DeleteFile(FileToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
        }
    }
}
