/**
 *mpv.net
 *Copyright(C) 2019 stax76
 *
 *This program is free software: you can redistribute it and/or modify
 *it under the terms of the GNU General Public License as published by
 *the Free Software Foundation, either version 3 of the License, or
 *(at your option) any later version.
 *
 *This program is distributed in the hope that it will be useful,
 *but WITHOUT ANY WARRANTY; without even the implied warranty of
 *MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *GNU General Public License for more details.
 *
 *You should have received a copy of the GNU General Public License
 *along with this program. If not, see http://www.gnu.org/licenses/.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows.Forms;

using static mpvnet.StaticUsing;

namespace mpvnet
{
    public class Addon
    {
        [ImportMany]
        public IEnumerable<IAddon> Addons = null;

        private readonly CompositionContainer CompositionContainer;

        public Addon()
        {
            try
            {
                AggregateCatalog catalog = new AggregateCatalog();

                string dir = Application.StartupPath + "\\Addons";

                if (Directory.Exists(dir))
                    foreach (string i in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(i, "*Addon.dll"));

                dir = mpv.mpvConfFolderPath + "\\Addons";

                if (Directory.Exists(dir))
                    foreach (string i in Directory.GetDirectories(dir))
                        catalog.Catalogs.Add(new DirectoryCatalog(i, "*Addon.dll"));

                if (catalog.Catalogs.Count > 0)
                {
                    CompositionContainer = new CompositionContainer(catalog);
                    CompositionContainer.ComposeParts(this);
                }
            }
            catch (Exception e)
            {
                MsgError(e.ToString());
            }
        }
    }

    public interface IAddon
    {
    }
}