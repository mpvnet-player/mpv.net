
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

using static mpvnet.Core;
using static mpvnet.NewLine;

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
                ConsoleHelp.Write(output.Read(), Module);
        }

        public void Error_DataReady(object sender, EventArgs e)
        {
            var output = sender as PipelineReader<Object>;

            while (output.Count > 0)
                ConsoleHelp.WriteError(output.Read(), Module);
        }

        public void RedirectStreams(PSEventJob job)
        {
            if (Print)
            {
                job.Output.DataAdded += Output_DataAdded;
                job.Error.DataAdded += Error_DataAdded;
            }
        }

        public void commandv(params string[] args) => core.commandv(args);

        public void command(string command) => core.command(command);

        public bool get_property_bool(string name) => core.get_property_bool(name);

        public void set_property_bool(string name, bool value) => core.set_property_bool(name, value);

        public int get_property_int(string name) => core.get_property_int(name);

        public void set_property_int(string name, int value) => core.set_property_int(name, value);

        public double get_property_number(string name) => core.get_property_number(name);

        public void set_property_number(string name, double value) => core.set_property_number(name, value);

        public string get_property_string(string name) => core.get_property_string(name);

        public void set_property_string(string name, string value) => core.set_property_string(name, value);

        public void observe_property(string name, string type, ScriptBlock sb)
        {
            PropChangedHandlers.Add(new KeyValuePair<string, ScriptBlock>(name, sb));

            switch (type)
            {
                case "bool": case "boolean":
                    core.observe_property_bool(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;
                case "string":
                    core.observe_property_string(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;
                case "int": case "integer":
                    core.observe_property_int(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;
                case "float": case "double":
                    core.observe_property_double(name, value => App.RunTask(() => PropertyChanged.Invoke(name, value)));
                    break;
                case "nil": case "none": case "native":
                    core.observe_property(name, () => App.RunTask(() => PropertyChanged.Invoke(name, null)));
                    break;
                default:
                    App.ShowError("Invalid Type", "Valid types are: bool or boolean, string, int or integer, float or double, nil or none or native");
                    break;
            }
        }

        public void register_event(string name, ScriptBlock sb)
        {
            EventHandlers.Add(new KeyValuePair<string, ScriptBlock>(name, sb));

            switch (name)
            {
                case "log-message":
                    core.LogMessageAsync += (level, msg) => Event.Invoke("log-message", new object[] { level, msg });
                    break;

                case "end-file":
                    core.EndFileAsync += reason => Event.Invoke("end-file", new object[] { reason });
                    break;

                case "client-message":
                    core.ClientMessageAsync += args => Event.Invoke("client-message", args);
                    break;

                case "shutdown":
                    core.Shutdown += () => Event.Invoke("shutdown", null);
                    break;

                case "get-property-reply":
                    core.GetPropertyReplyAsync += () => Event.Invoke("get-property-reply", null);
                    break;

                case "set-property-reply":
                    core.SetPropertyReplyAsync += () => Event.Invoke("set-property-reply", null);
                    break;

                case "command-reply":
                    core.CommandReplyAsync += () => Event.Invoke("command-reply", null);
                    break;

                case "start-file":
                    core.StartFileAsync += () => Event.Invoke("start-file", null);
                    break;

                case "file-loaded":
                    core.FileLoadedAsync += () => Event.Invoke("file-loaded", null);
                    break;

                case "idle":
                    core.IdleAsync += () => Event.Invoke("idle", null);
                    break;

                case "video-reconfig":
                    core.VideoReconfigAsync += () => Event.Invoke("video-reconfig", null);
                    break;

                case "audio-reconfig":
                    core.AudioReconfigAsync += () => Event.Invoke("audio-reconfig", null);
                    break;

                case "seek":
                    core.SeekAsync += () => Event.Invoke("seek", null);
                    break;

                case "playback-restart":
                    core.PlaybackRestartAsync += () => Event.Invoke("playback-restart", null);
                    break;
            }
        }

        void Output_DataAdded(object sender, DataAddedEventArgs e)
        {
            var output = sender as PSDataCollection<PSObject>;
            ConsoleHelp.Write(output[e.Index], Module);
        }

        void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            var error = sender as PSDataCollection<ErrorRecord>;
            ConsoleHelp.WriteError(error[e.Index], Module);
        }
    }

    public class PowerShellException : Exception
    {
        public PowerShellException(string message) : base(message)
        {
        }
    }
}
