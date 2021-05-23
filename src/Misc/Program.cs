
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using static mpvnet.Global;

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

                if (Core.ConfigFolder == "")
                    return;

                string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

                if (args.Length >= 2 && args[0] == "--reg-file-assoc")
                {
                    if (args[1] == "audio")
                        FileAssociation.Register(CorePlayer.AudioTypes);
                    else if (args[1] == "video")
                        FileAssociation.Register(CorePlayer.VideoTypes);
                    else if (args[1] == "image")
                        FileAssociation.Register(CorePlayer.ImageTypes);
                    else
                        FileAssociation.Register(args.Skip(1).ToArray());

                    return;
                }

                App.Init();
                Mutex mutex = new Mutex(true, "mpvnetProcessInstance", out bool isFirst);

                if ((App.ProcessInstance == "single" || App.ProcessInstance == "queue") && !isFirst)
                {
                    List<string> args2 = new List<string>();
                    args2.Add(App.ProcessInstance);

                    foreach (string arg in args)
                    {
                        if (!arg.StartsWith("--") && (arg == "-" || arg.Contains("://") ||
                            arg.Contains(":\\") || arg.StartsWith("\\\\")))

                            args2.Add(arg);
                        else if (arg == "--queue")
                            args2[0] = "queue";
                        else if (arg.StartsWith("--command="))
                        {
                            args2[0] = "command";
                            args2.Add(arg.Substring(10));
                        }
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
                                data.lpData = string.Join("\n", args2.ToArray());
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
