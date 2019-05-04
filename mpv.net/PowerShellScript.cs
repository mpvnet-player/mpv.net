using System;
using System.IO;
using System.Threading;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Threading.Tasks;

namespace mpvnet
{
    public class PowerShellScript
    {
        public static object Execute(string code, string[] parameters)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.ApartmentState = ApartmentState.STA;
                runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
                runspace.Open();

                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.AddScript(
@"Using namespace mpvnet;
Using namespace System;
[System.Reflection.Assembly]::LoadWithPartialName(""mpvnet"")");

                    pipeline.Commands.AddScript(code);

                    try
                    {
                        var ret = pipeline.Invoke(parameters);

                        if (ret.Count > 0)
                            return ret[0];
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            using (Pipeline pipeline2 = runspace.CreatePipeline())
                            {
                                pipeline2.Commands.AddScript("$PSVersionTable.PSVersion.Major * 10 +" +
                                    "$PSVersionTable.PSVersion.Minor");

                                if (Convert.ToInt32(pipeline2.Invoke()[0].ToString()) < 51)
                                    throw new Exception();
                            }
                        }
                        catch (Exception ex2)
                        {
                            Msg.ShowError("PowerShell Setup Problem\n\nEnsure you have at least PowerShell 5.1 installed.", ex2.ToString());
                            return null;
                        }
                        Msg.ShowException(ex);
                    }
                }
            }
            return null;
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
                    {
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.Invoke));
                    }
                    else if (eventInfo.EventHandlerType == typeof(Action<EndFileEventMode>))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.InvokeEndFileEventMode));
                    }
                    else if (eventInfo.EventHandlerType == typeof(Action<string[]>))
                    {
                        mi = eventObject.GetType().GetMethod(nameof(PowerShellEventObject.InvokeStrings));
                    }
                    else
                        throw new Exception();

                    eventObject.EventInfo = eventInfo;
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, eventObject, mi);
                    eventObject.Delegate = handler;
                    eventInfo.AddEventHandler(eventObject, handler);
                    return;
                }
            }
            Task.Run(() =>
            {
                PowerShellScript.Execute(File.ReadAllText(filePath), new string[] {});
            });
        }
    }

    public class PowerShellEventObject
    {
        public EventInfo EventInfo { get; set; }
        public Delegate Delegate { get; set; }
        public string FilePath { get; set; }

        public void Invoke()
        {
            Task.Run(() => { PowerShellScript.Execute(File.ReadAllText(FilePath), new string[] { }); });
        }

        public void InvokeEndFileEventMode(EndFileEventMode arg)
        {
            Task.Run(() =>
            {
                PowerShellScript.Execute(File.ReadAllText(FilePath), new string[] { arg.ToString() });
            });
        }

        public void InvokeStrings(string[] args)
        {
            Task.Run(() => {
                PowerShellScript.Execute(File.ReadAllText(FilePath), args);
            });
        }
    }
}