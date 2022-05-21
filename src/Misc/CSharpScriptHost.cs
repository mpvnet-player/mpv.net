
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.CSharp;

using static mpvnet.Global;

namespace mpvnet
{
    class CSharpScriptHost
    {
        static List<object> References = new List<object>();

        public static void ExecuteScriptsInFolder(string folder)
        {
            if (Directory.Exists(folder))
                foreach (string file in Directory.GetFiles(folder, "*.cs"))
                    App.RunTask(() => Execute(file));
        }

        static void Execute(string file)
        {
            string code = File.ReadAllText(file);
            string filename = Path.GetFileNameWithoutExtension(file) + " " + StringHelp.GetMD5Hash(code) + "-v6.dll";
            string outputFile = Path.Combine(Path.GetTempPath(), filename);

            if (!File.Exists(outputFile))
                Compile(outputFile, file);

            if (File.Exists(outputFile))
            {
                object instance = Assembly.LoadFile(outputFile).CreateInstance("Script");

                if (instance != null)
                    References.Add(instance);
                else
                    Terminal.WriteError("Failed to initialize script.", outputFile.FileName());
            }
        }

        public static void Compile(string outputFile, string file)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            string[] dependencies = {
                Folder.Startup + "mpvnet.exe",
                "Microsoft.VisualBasic.dll",
                "System.Core.dll", "System.Data.dll", "System.dll", "System.Drawing.dll", "System.Web.dll",
                "System.Windows.Forms.dll", "System.Xaml.dll", "System.Xml.dll", "System.Xml.Linq.dll",
                "WPF\\PresentationCore.dll", "WPF\\PresentationFramework.dll", "WPF\\WindowsBase.dll"
            };

            foreach (string i in dependencies)
                parameters.ReferencedAssemblies.Add(i);

            parameters.OutputAssembly = outputFile;
            CompilerResults results = provider.CompileAssemblyFromFile(parameters, file);

            var errors = results.Errors.Cast<CompilerError>().Select(i => "Line Number " +
                i.Line + "\n" + "Error Number: " + i.ErrorNumber + "\n" + i.ErrorText);

            if (errors.Count() > 0)
                Terminal.WriteError(string.Join(BR2, errors), Path.GetFileName(file));
        }
    }
}
