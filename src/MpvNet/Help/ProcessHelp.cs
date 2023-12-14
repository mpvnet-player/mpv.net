
namespace MpvNet.Help;

public static class ProcessHelp
{
    public static void Execute(string file, string arguments = "", bool shellExecute = false)
    {
        try
        {
            using Process proc = new Process();
            proc.StartInfo.FileName = file;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = shellExecute;
            proc.Start();
        }
        catch (Exception ex)
        {
            Terminal.WriteError(ex.ToString());
        }
    }

    public static void ShellExecute(string file, string arguments = "") => Execute(file, arguments, true);
}
