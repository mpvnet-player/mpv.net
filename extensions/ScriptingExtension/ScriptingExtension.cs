// This extension implements the C# scripting feature of mpv.net which
// is based on CS-Script (https://www.cs-script.net).

// Furthermore the extension is used to code and debug scripts
// because writing script code without debugger is not an option :-)

using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using mpvnet;
using CSScriptLibrary;

namespace ScriptingExtension // the file name of extensions must end with 'Extension'
{
    [Export(typeof(IExtension))]
    public class ScriptingExtension : IExtension
    {
        Script Script;

        public ScriptingExtension()
        {
            //Script = new Script();
            List<string> scriptFiles = new List<string>();

            if (Directory.Exists(mp.ConfigFolder + "scripts"))
                scriptFiles.AddRange(Directory.GetFiles(mp.ConfigFolder + "scripts", "*.cs"));

            if (Directory.Exists(Application.StartupPath + "\\scripts"))
                scriptFiles.AddRange(Directory.GetFiles(Application.StartupPath + "\\scripts", "*.cs"));

            if (scriptFiles.Count == 0) return;
            CSScriptLibrary.CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom;

            foreach (var i in scriptFiles)
            {
                try {
                    CSScriptLibrary.CSScript.Evaluator.LoadCode(File.ReadAllText(i));
                } catch (Exception e) {
                    Msg.ShowException(e);
                }
            }
        }
    }
}
