using System;
using System.Windows.Forms;

using Sys;

namespace mpvnet
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();

                if (args.Length == 3 && args[1] == "--reg-file-assoc")
                {
                    if (args[2] == "audio") FileAssociation.Register(FileAssociation.AudioTypes);
                    if (args[2] == "video") FileAssociation.Register(FileAssociation.VideoTypes);
                    if (args[2] == "unreg") FileAssociation.Unregister();
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }
    }
}