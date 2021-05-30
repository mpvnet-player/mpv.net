
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

using static mpvnet.Global;

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
                string dir = Core.ConfigFolder + "extensions";

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
