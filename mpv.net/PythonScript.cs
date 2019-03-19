using System;

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

using static mpvnet.StaticUsing;

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
                MsgError(ex.ToString());
            }
        }
    }
}