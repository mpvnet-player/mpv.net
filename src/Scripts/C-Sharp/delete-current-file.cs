
// C# Script that deletes the current file

// In input.conf add:

// KP0 script-message delete-current-file delete   #menu: Script > Delete current file > Delete
//   0 script-message delete-current-file delete   #menu: Script > Delete current file > Delete

// KP1 script-message delete-current-file confirm  #menu: Script > Delete current file > Confirm
//   1 script-message delete-current-file confirm  #menu: Script > Delete current file > Confirm

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

        if (args[1] == "delete")
        {
            FileToDelete = Core.GetPropertyString("path");
            DeleteTime = DateTime.Now;
            Core.CommandV("show-text", "Press 1 to delete file", "10000");
        }
        else if (args[1] == "confirm")
        {
            TimeSpan ts = DateTime.Now - DeleteTime;
            string path = Core.GetPropertyString("path");

            if (FileToDelete == path && ts.TotalSeconds < 10 && File.Exists(FileToDelete))
            {
                Core.CommandV("show-text", "");

                int count = Core.GetPropertyInt("playlist-count");
                int pos = Core.GetPropertyInt("playlist-pos");
                int newPos = pos == count - 1 ? pos - 1 : pos + 1;

                if (newPos > -1)
                    Core.SetPropertyNumber("playlist-pos", newPos);

                Core.Command("playlist-remove " + pos);

                App.RunTask(() => {
                    Thread.Sleep(2000);
                    FileSystem.DeleteFile(FileToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);                
                });
            }
        }
    }
}
