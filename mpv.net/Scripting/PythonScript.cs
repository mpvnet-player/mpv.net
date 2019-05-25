using System;
using System.Reflection;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Operations;

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
                engine.Execute("import mpvnet", scope);
                engine.Execute("from mpvnet import *", scope);
                engine.Execute(code, scope);
            }
            catch (Exception ex)
            {
                if (ex is SyntaxErrorException e)
                    Msg.ShowError($"{e.Line}, {e.Column}: " + ex.Message);
                else
                    Msg.ShowException(ex);
            }
        }
    }

    public class PythonEventObject
    {
        public PythonFunction PythonFunction { get; set; }
        public EventInfo EventInfo { get; set; }
        public Delegate Delegate { get; set; }

        public void Invoke() => PythonCalls.Call(PythonFunction);

        public void InvokeEndFileEventMode(EndFileEventMode arg)
        {
            PythonCalls.Call(PythonFunction, new[] { arg });
        }

        public void InvokeStrings(string[] arg)
        {
            PythonCalls.Call(PythonFunction, new[] { arg });
        }
    }
}