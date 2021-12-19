
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

using static mpvnet.Global;

namespace mpvnet
{
    public class PowerShell
    {
        public Runspace Runspace { get; set; }
        public Pipeline Pipeline { get; set; }
        public string Module { get; set; }
        public bool Print { get; set; }
        public List<string> Scripts { get; } = new List<string>();
        public List<KeyValuePair<string, object>> Variables = new List<KeyValuePair<string, object>>();
        public string[] Arguments { get; }
        public event Action<string, object[]> Event;
        public event Action<string, object> PropertyChanged;
        public List<KeyValuePair<string, ScriptBlock>> EventHandlers = new List<KeyValuePair<string, ScriptBlock>>();
        public List<KeyValuePair<string, ScriptBlock>> PropChangedHandlers = new List<KeyValuePair<string, ScriptBlock>>();

        public static List<PowerShell> References { get; } = new List<PowerShell>();

        public object Invoke() => Invoke(null, null);

        public object Invoke(string variable, object obj)
        {
            try
            {
                Runspace = RunspaceFactory.CreateRunspace();
                Runspace.ApartmentState = ApartmentState.STA;
                Runspace.Open();
                Pipeline = Runspace.CreatePipeline();

                foreach (string script in Scripts)
                    Pipeline.Commands.AddScript(script);

                if (Arguments != null)
                    foreach (string param in Arguments)
                        foreach (Command command in Pipeline.Commands)
                            command.Parameters.Add(null, param);

                Runspace.SessionStateProxy.SetVariable("mp", this);

                foreach (var i in Variables)
                    Runspace.SessionStateProxy.SetVariable(i.Key, i.Value);

                if (!string.IsNullOrEmpty(variable))
                    Runspace.SessionStateProxy.SetVariable(variable, obj);

                if (Print)
                {
                    Pipeline.Output.DataReady += Output_DataReady;
                    Pipeline.Error.DataReady += Error_DataReady;
                }

                return Pipeline.Invoke();
            }
            catch (RuntimeException e)
            {
                string message = e.Message + BR + BR + e.ErrorRecord.ScriptStackTrace.Replace(
                    " <ScriptBlock>, <No file>", "") + BR + BR + Module + BR;

                throw new PowerShellException(message);
            }
            catch (Exception e)
            {
                throw e;
            }        
        }

        public static string InvokeAndReturnString(string code, string varName, object varValue)
        {
            PowerShell ps = new PowerShell() { Print = false };
            ps.Scripts.Add(code);
            string ret = string.Join(Environment.NewLine, (ps.Invoke(varName, varValue)
                as IEnumerable<object>).Select(item => item.ToString())).ToString();
            ps.Runspace.Dispose();
            return ret;
        }

        public void Output_DataReady(object sender, EventArgs e)
        {
            var output = sender as PipelineReader<PSObject>;

            while (output.Count > 0)
                Terminal.Write(output.Read(), Module);
        }

        public void Error_DataReady(object sender, EventArgs e)
        {
            var output = sender as PipelineReader<object>;

            while (output.Count > 0)
                Terminal.WriteError(output.Read(), Module);
        }

        public void RedirectStreams(PSEventJob job)
        {
            if (Print)
            {
                job.Output.DataAdded += Output_DataAdded;
                job.Error.DataAdded += Error_DataAdded;
            }
        }

        public void CommandV(params string[] args) => Core.CommandV(args);

        public void Command(string command) => Core.Command(command);

        public bool GetPropertyBool(string name) => Core.GetPropertyBool(name);

        public void SetPropertyBool(string name, bool value) => Core.SetPropertyBool(name, value);

        public int GetPropertyInt(string name) => Core.GetPropertyInt(name);

        public void SetPropertyInt(string name, int value) => Core.SetPropertyInt(name, value);

        public double GetPropertyDouble(string name) => Core.GetPropertyDouble(name);

        public void SetPropertyDouble(string name, double value) => Core.SetPropertyDouble(name, value);

        public string GetPropertyString(string name) => Core.GetPropertyString(name);

        public void SetPropertyString(string name, string value) => Core.SetPropertyString(name, value);

        public void ObserveProperty(string name, string type, ScriptBlock sb)
        {
            PropChangedHandlers.Add(new KeyValuePair<string, ScriptBlock>(name, sb));

            switch (type)
            {
                case "bool": case "boolean":
                    Core.ObservePropertyBool(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;

                case "string":
                    Core.ObservePropertyString(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;

                case "int": case "integer":
                    Core.ObservePropertyInt(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;

                case "float": case "double":
                    Core.ObservePropertyDouble(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;

                case "nil": case "none": case "native":
                    Core.ObserveProperty(name, () => App.RunTask(() => PropertyChanged.Invoke(name, null)));
                    break;

                default:
                    App.ShowError("Invalid Type, valid types are: bool or boolean, string, int or integer, float or double, nil or none or native");
                    break;
            }
        }

        public void RegisterEvent(string name, ScriptBlock sb)
        {
            EventHandlers.Add(new KeyValuePair<string, ScriptBlock>(name, sb));

            switch (name)
            {
                case "log-message":
                    Core.LogMessageAsync += (level, msg) => Event.Invoke("log-message", new object[] { level, msg });
                    break;

                case "end-file":
                    Core.EndFileAsync += reason => Event.Invoke("end-file", new object[] { reason });
                    break;

                case "client-message":
                    Core.ClientMessageAsync += args => Event.Invoke("client-message", args);
                    break;

                case "shutdown":
                    Core.Shutdown += () => Event.Invoke("shutdown", null);
                    break;

                case "get-property-reply":
                    Core.GetPropertyReplyAsync += () => Event.Invoke("get-property-reply", null);
                    break;

                case "set-property-reply":
                    Core.SetPropertyReplyAsync += () => Event.Invoke("set-property-reply", null);
                    break;

                case "command-reply":
                    Core.CommandReplyAsync += () => Event.Invoke("command-reply", null);
                    break;

                case "start-file":
                    Core.StartFileAsync += () => Event.Invoke("start-file", null);
                    break;

                case "file-loaded":
                    Core.FileLoadedAsync += () => Event.Invoke("file-loaded", null);
                    break;

                case "video-reconfig":
                    Core.VideoReconfigAsync += () => Event.Invoke("video-reconfig", null);
                    break;

                case "audio-reconfig":
                    Core.AudioReconfigAsync += () => Event.Invoke("audio-reconfig", null);
                    break;

                case "seek":
                    Core.SeekAsync += () => Event.Invoke("seek", null);
                    break;

                case "playback-restart":
                    Core.PlaybackRestartAsync += () => Event.Invoke("playback-restart", null);
                    break;
            }
        }

        void Output_DataAdded(object sender, DataAddedEventArgs e)
        {
            var output = sender as PSDataCollection<PSObject>;
            Terminal.Write(output[e.Index], Module);
        }

        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            var error = sender as PSDataCollection<ErrorRecord>;
            Terminal.WriteError(error[e.Index], Module);
        }
    }

    public class PowerShellException : Exception
    {
        public PowerShellException(string message) : base(message)
        {
        }
    }
}
