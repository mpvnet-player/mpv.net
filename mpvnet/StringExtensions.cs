/**
 *mpv.net
 *Copyright(C) 2017 stax76
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

using System.IO;

namespace mpvnet
{
    public static class ExtensionMethods
    {
        public static string Ext(this string instance)
        {
            if (string.IsNullOrEmpty(instance))
                return "";

            string ext = Path.GetExtension(instance);

            if (ext == "")
                return "";

            return ext.ToLowerInvariant().Substring(1);
        }

        public static string ExtFull(this string instance)
        {
            if (string.IsNullOrEmpty(instance))
                return "";

            return Path.GetExtension(instance).ToLowerInvariant();
        }
    }
}