
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (App.IsStartedFromTerminal)
                    Native.AttachConsole(-1 /*ATTACH_PARENT_PROCESS*/);

                if (mp.ConfigFolder == "")
                    return;

                string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

                if (args.Length == 2 && args[0] == "--reg-file-assoc")
                {
                    if (args[1] == "audio") FileAssociation.Register(App.AudioTypes);
                    if (args[1] == "video") FileAssociation.Register(App.VideoTypes);
                    if (args[1] == "image") FileAssociation.Register(App.ImageTypes);
                    if (args[1] == "unreg") FileAssociation.Unregister();
                    return;
                }

                App.Init();
                Mutex mutex = new Mutex(true, "mpvnetProcessInstance", out bool isFirst);

                if ((App.ProcessInstance == "single" || App.ProcessInstance == "queue") && !isFirst)
                {
                    List<string> files = new List<string>();
                    files.Add(App.ProcessInstance);

                    foreach (string arg in args)
                    {
                        if (!arg.StartsWith("--") && (arg == "-" || arg.Contains("://") ||
                            arg.Contains(":\\") || arg.StartsWith("\\\\")))

                            files.Add(arg);
                        else if (arg == "--queue")
                            files[0] = "queue";
                    }

                    Process[] procs = Process.GetProcessesByName("mpvnet");

                    for (int i = 0; i < 20; i++)
                    {
                        foreach (Process proc in procs)
                        {
                            if (proc.MainWindowHandle != IntPtr.Zero)
                            {
                                Native.AllowSetForegroundWindow(proc.Id);
                                var data = new Native.COPYDATASTRUCT();
                                data.lpData = string.Join("\n", files.ToArray());
                                data.cbData = data.lpData.Length * 2 + 1;
                                Native.SendMessage(proc.MainWindowHandle, 0x004A /*WM_COPYDATA*/, IntPtr.Zero, ref data);
                                mutex.Dispose();

                                if (App.IsStartedFromTerminal)
                                    Native.FreeConsole();

                                return;
                            }
                        }

                        Thread.Sleep(50);
                    }

                    mutex.Dispose();
                    return;
                }

                Application.Run(new MainForm());

                if (App.IsStartedFromTerminal)
                    Native.FreeConsole();

                mutex.Dispose();
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }
    }
}