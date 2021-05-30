
// This script writes a rating to the filename of rated videos when mpv.net shuts down.

// In input.conf add:

// KP0 script-message rate-file 0     #menu: Script > Rating > 0stars
// KP1 script-message rate-file 1     #menu: Script > Rating > 1stars
// KP2 script-message rate-file 2     #menu: Script > Rating > 2stars
// KP3 script-message rate-file 3     #menu: Script > Rating > 3stars
// KP4 script-message rate-file 4     #menu: Script > Rating > 4stars
// KP5 script-message rate-file 5     #menu: Script > Rating > 5stars

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using mpvnet;

class Script
{
    // dictionory to store the filename and the rating
    Dictionary<string, int> Dic = new Dictionary<string, int>();
    CorePlayer Core;

    public Script() // plugin initialization
    {
        Core = Global.Core;
        Core.ClientMessage += ClientMessage; //handles keys defined in input.conf
        Core.Shutdown += Shutdown; // handles MPV_EVENT_SHUTDOWN
    }

    // handles MPV_EVENT_SHUTDOWN
    void Shutdown()
    {
        foreach (var i in Dic)
        {
            string filepath = i.Key;
            int rating = i.Value;

            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                return;

            string basename = Path.GetFileNameWithoutExtension(filepath);

            for (int x = 0; x < 6; x++)
                if (basename.Contains(" (" + x + "stars)"))
                    basename = basename.Replace(" (" + x + "stars)", "");

            basename += " (" + rating + "stars)";

            string newPath = Path.Combine(Path.GetDirectoryName(filepath),
                basename + Path.GetExtension(filepath));

            if (filepath.ToLower() != newPath.ToLower())
                File.Move(filepath, newPath);

            File.SetLastWriteTime(newPath, DateTime.Now);
        }
    }

    //handles keys defined in input.conf
    void ClientMessage(string[] args)
    {
        if (args[0] != "rate-file")
            return;

        int rating;

        if (int.TryParse(args[1], out rating))
        {
            string path = Core.get_property_string("path");

            if (!File.Exists(path))
                return;

            Dic[path] = rating;
            Core.commandv("show-text", "Rating: " + rating);
        }
    }
}
