using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace mpvnet
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

                if (args.Length == 2 && args[0] == "--reg-file-assoc")
                {
                    if (args[1] == "audio") FileAssociation.Register(App.AudioTypes);
                    if (args[1] == "video") FileAssociation.Register(App.VideoTypes);
                    if (args[1] == "unreg") FileAssociation.Unregister();
                    return;
                }

                App.Init();
                Mutex mutex = new Mutex(true, "mpvnetProcessInstance", out bool isFirst);

                if ((App.ProcessInstance == "single" || App.ProcessInstance == "queue") && !isFirst)
                {
                    List<string> files = new List<string>();

                    foreach (string arg in args)
                        if (!arg.StartsWith("--") && (File.Exists(arg) || arg == "-" || arg.StartsWith("http")))
                            files.Add(arg);

                    if (files.Count > 0)
                        RegistryHelp.SetObject(App.RegPath, "ShellFiles", files.ToArray());

                    RegistryHelp.SetObject(App.RegPath, "ProcessInstanceMode", App.ProcessInstance);

                    foreach(Process process in Process.GetProcessesByName("mpvnet"))
                    {
                        try {
                            SingleProcess.AllowSetForegroundWindow(process.Id);
                            Native.SendMessage(process.MainWindowHandle, SingleProcess.Message, IntPtr.Zero, IntPtr.Zero);
                        } catch {}
                    }

                    mutex.Dispose();
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                mutex.Dispose();
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }
    }
}