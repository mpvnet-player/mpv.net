using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows.Forms;

using VBNET;

namespace mpvnet
{
    public class Addon
    {
        [ImportMany]
        public IEnumerable<IAddon> Addons = null;

        readonly CompositionContainer CompositionContainer;

        public Addon()
        {
            try
            {
                AggregateCatalog catalog = new AggregateCatalog();

                string dir = Application.StartupPath + "\\Addons";

                if (Directory.Exists(dir))
                    foreach (string i in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(i, "*Addon.dll"));

                dir = mp.MpvConfFolderPath + "\\Addons";

                if (Directory.Exists(dir))
                    foreach (string i in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(i, "*Addon.dll"));

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

    public interface IAddon
    {
    }
}