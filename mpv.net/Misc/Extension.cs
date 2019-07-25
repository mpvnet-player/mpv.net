using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows.Forms;

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
                    foreach (string i in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(i, "*Extension.dll"));

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