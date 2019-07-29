// This extension implements the C# scripting feature of mpv.net which
// is based on CS-Script (https://www.cs-script.net).

// I also use this extension to code scripts in order to have full
// code completion and debugger support, once the script code is
// finished I move it from the extension to a standalone script.

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
        //Script Script;

        public ScriptingExtension()
        {
            //Script = new Script();
            List<string> scriptFiles = new List<string>();

            if (Directory.Exists(mp.ConfigFolder + "scripts"))
                scriptFiles.AddRange(Directory.GetFiles(mp.ConfigFolder + "scripts", "*.cs"));

            if (Directory.Exists(Application.StartupPath + "\\scripts"))
                foreach (string file in Directory.GetFiles(Application.StartupPath + "\\scripts", "*.cs"))
                    Msg.ShowError("Failed to load script.", "Only scripts that ship with mpv.net are allowed in <startup>\\scripts.\n\nUser scripts have to use <config folder>\\scripts.\n\nNever copy a new mpv.net version over a old mpv.net version.\n\nNever install a new mpv.net version on top of a old mpv.net version.\n\n" + file);

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