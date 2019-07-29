using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace mpvnet
{
    public class Extension
    {
        [ImportMany]
        public IEnumerable<IExtension> Extensions = null;

        readonly CompositionContainer CompositionContainer;

        public Extension()
        {
            try
            {
                AggregateCatalog catalog = new AggregateCatalog();
                string dir = Application.StartupPath + "\\Extensions";

                if (Directory.Exists(dir))
                {
                    string[] knownExtensions = { "RatingExtension", "ScriptingExtension" };

                    foreach (string path in Directory.GetDirectories(dir))
                    {
                        if (knownExtensions.Contains(Path.GetFileName(path)))
                            catalog.Catalogs.Add(new DirectoryCatalog(path, "*Extension.dll"));
                        else
                            Msg.ShowError("Failed to load extension", "Only extensions that ship with mpv.net are allowed in <startup>\\extensions.\n\nUser extensions have to use <config folder>\\extensions.\n\nNever copy a new mpv.net version over a old mpv.net version.\n\nNever install a new mpv.net version on top of a old mpv.net version.\n\n" + path);
                    }
                }

                dir = mp.ConfigFolder + "\\Extensions";

                if (Directory.Exists(dir))
                    foreach (string i in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(i, "*Extension.dll"));

                if (catalog.Catalogs.Count > 0)
                {
                    CompositionContainer = new CompositionContainer(catalog);
                    CompositionContainer.ComposeParts(this);
                }
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }
    }

    public interface IExtension
    {
    }
}