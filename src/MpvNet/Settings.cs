
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;

namespace MpvNet;

[Serializable()]
public class AppSettings
{
    public bool InputDefaultBindingsFixApplied;
    public bool ShowMenuFixApplied;
    public int MenuUpdateVersion;
    public int Volume = 70;
    public List<string> RecentFiles = new List<string>();
    public Point WindowLocation;
    public Point WindowPosition;
    public Size WindowSize;
    public string AudioDevice = "";
    public string ConfigEditorSearch = "Video:";
    public string Mute = "no";
    public string StartupFolder = "";
}

class SettingsManager
{
    public static string SettingsFile => Player.ConfigFolder + "settings.xml";

    public static AppSettings Load()
    {
        if (!File.Exists(SettingsFile))
            return new AppSettings();

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            using FileStream fs = new FileStream(SettingsFile, FileMode.Open);
            return (AppSettings)serializer.Deserialize(fs)!;
        }
        catch (Exception ex)
        {
            Terminal.WriteError(ex.ToString());
            return new AppSettings();
        }
    }

    public static void Save(object obj)
    {
        try
        {
            using XmlTextWriter writer = new XmlTextWriter(SettingsFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
        }
        catch (Exception ex)
        {
            Terminal.WriteError(ex.ToString());
        }
    }
}
