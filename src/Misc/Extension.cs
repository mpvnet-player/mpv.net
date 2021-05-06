
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;

using static mpvnet.Core;

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
                string dir = Folder.Startup + "Extensions";

                if (Directory.Exists(dir))
                {
                    string[] knownExtensions = { "RatingExtension", "ScriptingExtension" };

                    foreach (string extDir in Directory.GetDirectories(dir))
                    {
                        if (knownExtensions.Contains(Path.GetFileName(extDir)))
                            catalog.Catalogs.Add(new DirectoryCatalog(extDir, Path.GetFileName(extDir) + ".dll"));
                        else
                            ConsoleHelp.WriteError("Failed to load extension:\n\n" +  extDir +
                                "\n\nOnly extensions that ship with mpv.net are allowed in <startup>\\extensions" +
                                "\n\nUser extensions have to use <config folder>\\extensions" +
                                "\n\nNever copy or install a new mpv.net version over a old mpv.net version.");
                    }
                }

                dir = core.ConfigFolder + "extensions";

                if (Directory.Exists(dir))
                    foreach (string extDir in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(extDir, Path.GetFileName(extDir) + ".dll"));

                if (catalog.Catalogs.Count > 0)
                {
                    CompositionContainer = new CompositionContainer(catalog);
                    CompositionContainer.ComposeParts(this);
                }
            }
            catch (Exception ex)
            {
                App.ShowException(ex);
            }
        }
    }

    public interface IExtension
    {
    }
}
