
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

using static mpvnet.Global;

namespace mpvnet
{
    [Serializable()]
    public class AppSettings
    {
        public int LastUpdateCheck;
        public int Volume = 70;
        public List<string> RecentFiles = new List<string>();
        public Point WindowLocation;
        public Point WindowPosition;
        public Size WindowSize;
        public string ConfigEditorSearch = "";
        public string Mute = "no";
        public string UpdateCheckVersion = "";
    }

    class SettingsManager
    {
        public static string SettingsFile {
            get => Core.ConfigFolder + "settings.xml";
        }

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsFile))
                return new AppSettings();

            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));

            using (FileStream fs = new FileStream(SettingsFile, FileMode.Open))
                return (AppSettings)serializer.Deserialize(fs);
        }

        public static void Save(object obj)
        {
            using (XmlTextWriter writer = new XmlTextWriter(SettingsFile, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
            }
        }
    }
}
