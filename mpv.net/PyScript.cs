using System;

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

using static mpvnet.StaticUsing;

namespace mpvnet
{
    public class PyScript
    {
        ScriptEngine engine;
        ScriptScope scope;

        public PyScript(string code)
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