
namespace MpvNet.Help;

public static class FileHelp
{
    public static void Delete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception ex)
        {
            Terminal.WriteError("Failed to delete file:" + BR + path + BR + ex.Message);
        }
    }

    public static string ReadTextFile(string path) => File.Exists(path) ? File.ReadAllText(path) : "";
}
