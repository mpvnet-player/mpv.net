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
        public static object Execute(string code, string[] parameters)
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

                    pipeline.Commands.AddScript(code);

                    if (parameters != null)
                        foreach (var i in parameters)
                            pipeline.Commands[1].Parameters.Add(null, i);

                    try
                    {
                        pipeline.Output.DataReady += Output_DataReady;
                        pipeline.Error.DataReady += Error_DataReady;

                        var ret = pipeline.Invoke();
                        if (ret.Count > 0) return ret[0];

                        pipeline.Output.DataReady -= Output_DataReady;
                        pipeline.Error.DataReady -= Error_DataReady;
                    }
                    catch (Exception e)
                    {
                        Msg.ShowException(e);
                    }
                }
            }
            return null;
        }

        private static void Output_DataReady(object sender, EventArgs e)
        {
            var output = sender as PipelineReader<PSObject>;
            while (output.Count > 0) Console.WriteLine(output.Read().ToString());
        }

        private static void Error_DataReady(object sender, EventArgs e)
        {
            var output = sender as PipelineReader<Object>;
            Console.ForegroundColor = ConsoleColor.Red;
            while (output.Count > 0) Console.WriteLine(output.Read().ToString());
            Console.ResetColor();
        }

        public static void Init(string filePath)
        {
            foreach (var eventInfo in typeof(mp).GetEvents())
            {
                if (eventInfo.Name.ToLower() ==
                    Path.GetFileNameWithoutExtension(filePath).ToLower().Replace("-", ""))
                {
                    PowerShellEventObject eventObject = new PowerShellEventObject();
                    MethodInfo mi;
                    eventObject.FilePath = filePath;

                    if (eventInfo.EventHandlerType == typeof(Action))
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.Invoke));
                    else if (eventInfo.EventHandlerType == typeof(Action<EndFileEventMode>))
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.InvokeEndFileEventMode));
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
            PowerShellScript.Execute(File.ReadAllText(filePath), null);
        }
    }

    public class PowerShellEventObject
    {
        public EventInfo EventInfo { get; set; }
        public Delegate Delegate { get; set; }
        public string FilePath { get; set; }

        public void Invoke() => PowerShellScript.Execute(File.ReadAllText(FilePath), null);

        public void InvokeEndFileEventMode(EndFileEventMode arg)
        {
            PowerShellScript.Execute(File.ReadAllText(FilePath), new[] { arg.ToString() });
        }

        public void InvokeStrings(string[] args)
        {
            PowerShellScript.Execute(File.ReadAllText(FilePath), args);
        }
    }
}