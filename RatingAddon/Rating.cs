using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;

using mpvnet;
using System.IO;

namespace Rating
{
    [Export(typeof(IAddon))]
    public class Rating : IAddon
    {
        private Dictionary<string, int> Dic = new Dictionary<string, int>();

        public Rating()
        {
            mpv.ClientMessage += mpv_ClientMessage;
            mpv.Shutdown += mpv_Shutdown;
        }

        private void mpv_Shutdown()
        {
            foreach (var i in Dic)
            {
                string filepath = i.Key;
                int rating = i.Value;

                if (String.IsNullOrEmpty(filepath) || ! File.Exists(filepath))
                    return;

                string basename = Path.GetFileNameWithoutExtension(filepath);

                for (int x = 0; x < 6; x++)
                    if (basename.Contains(" (" + x.ToString() + "stars)"))
                        basename = basename.Replace(" (" + x.ToString() + "stars)", "");

                basename += $" ({rating}stars)";
                string newPath = Path.Combine(Path.GetDirectoryName(filepath), basename + Path.GetExtension(filepath));

                if (filepath.ToLower() != newPath.ToLower())
                    File.Move(filepath, newPath);

                File.SetLastWriteTime(newPath, DateTime.Now);
            }            
        }

        private void mpv_ClientMessage(string[] args)
        {
            int rating;

            if (args?.Length != 2 || args[0] != "rate-file" || ! int.TryParse(args[1], out rating))
                return;

            Dic[mpv.GetStringProp("path")] = rating;
            mpv.Command("show-text", $"Rating: {rating}");
        }
    }
}