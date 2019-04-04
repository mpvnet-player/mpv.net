using System;
using System.Windows.Forms;

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
                    if (args[2] == "unregister") FileAssociation.Unregister();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}