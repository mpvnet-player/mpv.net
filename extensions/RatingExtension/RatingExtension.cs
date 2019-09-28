// This extension writes a rating to the filename of rated videos when mpv.net shuts down.

// The input.conf defaults contain key bindings for this extension to set ratings.

using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.IO;

using mpvnet;

namespace RatingExtension // the assembly name must end with 'Extension'
{
    [Export(typeof(IExtension))]
    public class RatingExtension : IExtension
    {
        // dictionory to store the filename and the rating
        Dictionary<string, int> Dic = new Dictionary<string, int>();

        public RatingExtension() // plugin initialization
        {
            mp.ClientMessage += ClientMessage; //handles keys defined in input.conf
            mp.Shutdown += Shutdown; // handles MPV_EVENT_SHUTDOWN
        }

        // handles MPV_EVENT_SHUTDOWN
        void Shutdown()
        {
            foreach (var i in Dic)
            {
                string filepath = i.Key;
                int rating = i.Value;
                if (String.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                    return;
                string basename = Path.GetFileNameWithoutExtension(filepath);

                for (int x = 0; x < 6; x++)
                    if (basename.Contains(" (" + x + "stars)"))
                        basename = basename.Replace(" (" + x + "stars)", "");

                basename += $" ({rating}stars)";
                string newPath = Path.Combine(Path.GetDirectoryName(filepath), basename + Path.GetExtension(filepath));
                if (filepath.ToLower() != newPath.ToLower())
                    File.Move(filepath, newPath);
                File.SetLastWriteTime(newPath, DateTime.Now);
            }            
        }

        //handles keys defined in input.conf
        void ClientMessage(string[] args)
        {
            if (args[0] != "rate-file") return;

            if (int.TryParse(args[1], out int rating))
            {
                string path = mp.get_property_string("path");
                if (!File.Exists(path)) return;
                Dic[path] = rating;
                mp.commandv("show-text", $"Rating: {rating}");
            }
            else if (args[1] == "about")
                Msg.Show("Rating Extension", "This extension writes a rating to the filename of rated videos when mpv.net shuts down.\n\nThe input.conf defaults contain key bindings for this extension to set ratings.");
        }
    }
}
