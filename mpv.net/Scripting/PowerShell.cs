
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace ScriptHost
{
    public class PowerShell
    {
        public Runspace Runspace { get; set; }
        public Pipeline Pipeline { get; set; }
        public string Module { get; set; }
        public bool Print { get; set; }
        public List<string> Scripts { get; } = new List<string>();
        public string[] Parameters { get; }

        public static List<PowerShell> Instances { get; } = new List<PowerShell>();

        string NL = Environment.NewLine;

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

                if (Parameters != null)
                    foreach (string param in Parameters)
                        foreach (Command command in Pipeline.Commands)
                            command.Parameters.Add(null, param);

                Runspace.SessionStateProxy.SetVariable("ScriptHost", this);

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
                string message = e.Message + NL + NL + e.ErrorRecord.ScriptStackTrace.Replace(
                    " <ScriptBlock>, <No file>", "") + NL + NL + Module + NL;

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

        public void RedirectEventJobStreams(PSEventJob job)
        {
            if (Print)
            {
                job.Output.DataAdded += Output_DataAdded;
                job.Error.DataAdded += Error_DataAdded;
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
