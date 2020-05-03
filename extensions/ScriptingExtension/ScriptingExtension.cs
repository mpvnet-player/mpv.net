
// This extension implements the C# scripting feature of mpv.net which
// is based on CS-Script (https://www.cs-script.net).

// I also use this extension to code scripts in order to have full
// code completion and debugger support, once the script code is
// finished I move it from the extension to a standalone script.

using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.IO;

using mpvnet;
using CSScriptLibrary;
using static mpvnet.Core;

// the file name of extensions must end with 'Extension'
namespace ScriptingExtension
{
    [Export(typeof(IExtension))]
    public class ScriptingExtension : IExtension
    {
        //Script Script;

        public ScriptingExtension()
        {
            //Script = new Script();
            List<string> files = new List<string>();

            if (Directory.Exists(core.ConfigFolder + "scripts-cs"))
                files.AddRange(Directory.GetFiles(core.ConfigFolder + "scripts-cs", "*.cs"));

            if (Directory.Exists(Folder.Startup + "scripts"))
                foreach (string path in Directory.GetFiles(Folder.Startup + "scripts", "*.cs"))
                    files.AddRange(Directory.GetFiles(Folder.Startup + "scripts", "*.cs"));

            if (files.Count == 0)
                return;

            CSScriptLibrary.CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom;

            foreach (string file in files)
            {
                try
                {
                    CSScriptLibrary.CSScript.Evaluator.LoadCode(File.ReadAllText(file));
                }
                catch (Exception e)
                {
                    App.ShowException(e);
                }
            }
        }
    }
}
