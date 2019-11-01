using System;
using System.IO;
using System.Threading;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Management.Automation;

namespace mpvnet
{
    public class PowerShellScript
    {
        public static void Execute(string filepath, params string[] parameters)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.ApartmentState = ApartmentState.STA;
                runspace.Open();

                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.AddScript(
                        "Using namespace mpvnet\n" +
                        "Using namespace System\n" +
                        "[System.Reflection.Assembly]::LoadWithPartialName(\"mpvnet\")\n");

                    pipeline.Commands.AddScript(File.ReadAllText(filepath));

                    if (parameters != null)
                        foreach (string i in parameters)
                            pipeline.Commands[1].Parameters.Add(null, i);

                    PowerShellOutput output = new PowerShellOutput();
                    output.ModuleName = Path.GetFileName(filepath);

                    pipeline.Output.DataReady += output.Output_DataReady;
                    pipeline.Error.DataReady += output.Error_DataReady;

                    runspace.SessionStateProxy.SetVariable("Output", output);

                    try {
                        pipeline.Invoke();
                    }
                    catch (RuntimeException e) {
                        Msg.ShowError("PowerShell Exception", e.Message + "\n\n" +
                            e.ErrorRecord.ScriptStackTrace.Replace(" <ScriptBlock>, <No file>", "") +
                            "\n\n" + Path.GetFileName(filepath));
                    }
                    catch (Exception e) {
                        Msg.ShowException(e);
                    }

                    pipeline.Output.DataReady -= output.Output_DataReady;
                    pipeline.Error.DataReady -= output.Error_DataReady;
                }
            }
        }

        public static void Init(string filepath)
        {
            foreach (var eventInfo in typeof(mp).GetEvents())
            {
                if (eventInfo.Name.ToLower() ==
                    Path.GetFileNameWithoutExtension(filepath).ToLower().Replace("-", ""))
                {
                    PowerShellEventObject eventObject = new PowerShellEventObject();
                    MethodInfo mi;
                    eventObject.Filepath = filepath;

                    if (eventInfo.EventHandlerType == typeof(Action))
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.Invoke));
                    else if (eventInfo.EventHandlerType == typeof(Action<EndFileEventMode>))
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.InvokeEndFile));
                    else if (eventInfo.EventHandlerType == typeof(Action<string[]>))
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.InvokeStrings));
                    else
                        throw new Exception();

                    eventObject.EventInfo = eventInfo;
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, eventObject, mi);
                    eventObject.Delegate = handler;
                    eventInfo.AddEventHandler(eventObject, handler);
                    return;
                }
            }
            Execute(filepath);
        }

        class PowerShellOutput
        {
            public string ModuleName { get; set; }

            public bool WriteStandard { get; set; } = true;
            public bool WriteError { get; set; } = true;

            public void Output_DataReady(object sender, EventArgs e)
            {
                if (!WriteStandard)
                    return;

                var output = sender as PipelineReader<PSObject>;

                while (output.Count > 0)
                    Console.WriteLine("[" + ModuleName + "] " + output.Read().ToString());
            }

            public void Error_DataReady(object sender, EventArgs e)
            {
                if (!WriteError)
                    return;

                var output = sender as PipelineReader<Object>;

                while (output.Count > 0)
                    ConsoleHelp.WriteError("[" + ModuleName + "] " + output.Read().ToString());
            }
        }
    }

    public class PowerShellEventObject
    {
        public EventInfo EventInfo { get; set; }
        public Delegate Delegate { get; set; }
        public string Filepath { get; set; }

        public void Invoke() => PowerShellScript.Execute(Filepath);
        public void InvokeEndFile(EndFileEventMode arg) => PowerShellScript.Execute(Filepath, arg.ToString());
        public void InvokeStrings(string[] args) => PowerShellScript.Execute(Filepath, args);
    }
}