using System;
using System.Reflection;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using VBNET;
using PyRT = IronPython.Runtime;

namespace mpvnet
{
    public class PythonScript
    {
        ScriptEngine engine;
        ScriptScope scope;

        public PythonScript(string code)
        {
            try
            {
                engine = Python.CreateEngine();
                scope = engine.CreateScope();
                scope.ImportModule("clr");
                engine.Execute("import clr", scope);
                engine.Execute("clr.AddReference(\"mpvnet\")", scope);
                engine.Execute("from mpvnet import *", scope);
                engine.Execute(code, scope);
            }
            catch (Exception ex)
            {
                Msg.ShowException(ex);
            }
        }
    }

    public class PythonEventObject
    {
        public PyRT.PythonFunction PythonFunction { get; set; }
        public EventInfo EventInfo { get; set; }
        public Delegate Delegate { get; set; }

        public void Invoke()
        {
            PyRT.Operations.PythonCalls.Call(PythonFunction);
        }

        public void InvokeEndFileEventMode(EndFileEventMode arg)
        {
            PyRT.Operations.PythonCalls.Call(PythonFunction, new[] { arg });
        }

        public void InvokeStrings(string[] arg)
        {
            PyRT.Operations.PythonCalls.Call(PythonFunction, new[] { arg });
        }
    }
}