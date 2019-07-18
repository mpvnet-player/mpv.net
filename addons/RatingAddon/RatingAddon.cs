// this addon writes a rating to the filename of rated videos when mpv.net
// shuts down. The input.conf defaults contain key bindings for this addon.

using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.IO;

using mpvnet;
using System.Diagnostics;

// the assembly name must end with 'Addon'
namespace RatingAddon
{
    [Export(typeof(IAddon))]
    public class RatingAddon : IAddon
    {
        // dictionory to store the filename and the rating
        Dictionary<string, int> Dic = new Dictionary<string, int>();

        public RatingAddon() // plugin initialization
        {
            mp.ClientMessage += ClientMessage; //handles keys defined in input.conf
            mp.Shutdown += Shutdown; // handles MPV_EVENT_SHUTDOWN
        }

        // handles MPV_EVENT_SHUTDOWN
        void Shutdown()
        {
            if (App.DebugMode) Trace.WriteLine("aaa");

            foreach (var i in Dic)
            {
                string filepath = i.Key;
                int rating = i.Value;
                if (App.DebugMode) Trace.WriteLine("bbb");
                if (String.IsNullOrEmpty(filepath) || ! File.Exists(filepath))
                    return;
                if (App.DebugMode) Trace.WriteLine("ccc");
                string basename = Path.GetFileNameWithoutExtension(filepath);

                for (int x = 0; x < 6; x++)
                    if (basename.Contains(" (" + x.ToString() + "stars)"))
                        basename = basename.Replace(" (" + x.ToString() + "stars)", "");

                basename += $" ({rating}stars)";
                string newPath = Path.Combine(Path.GetDirectoryName(filepath), basename + Path.GetExtension(filepath));
                if (App.DebugMode) Trace.WriteLine("ddd");
                if (filepath.ToLower() != newPath.ToLower())
                    File.Move(filepath, newPath);
                if (App.DebugMode) Trace.WriteLine("eee");
                File.SetLastWriteTime(newPath, DateTime.Now);
                if (App.DebugMode) Trace.WriteLine("fff");
            }            
        }

        //handles keys defined in input.conf
        void ClientMessage(string[] args)
        {
            if (args[0] != "rate-file" || ! int.TryParse(args[1], out int rating))
                return;
            string path = mp.get_property_string("path");
            if (!File.Exists(path)) return;
            Dic[path] = rating;
            mp.commandv("show-text", $"Rating: {rating}");
        }
    }
}