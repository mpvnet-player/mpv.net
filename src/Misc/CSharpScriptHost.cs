
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using Microsoft.CSharp;

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
            string filename = Path.GetFileNameWithoutExtension(file) + " " + GetMD5(code) + ".dll";
            string outputFile = Path.Combine(Path.GetTempPath(), filename);

            if (!File.Exists(outputFile))
                Compile(outputFile, file);

            if (File.Exists(outputFile))
                References.Add(Assembly.LoadFile(outputFile).CreateInstance("Script"));
        }

        public static void Compile(string outputFile, string file)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            string[] dependencies = {
                "Microsoft.VisualBasic.dll",
                "System.Core.dll", "System.Data.dll", "System.dll", "System.Drawing.dll", "System.Web.dll",
                "System.Windows.Forms.dll", "System.Xaml.dll", "System.Xml.dll", "System.Xml.Linq.dll",
                "WPF\\PresentationCore.dll", "WPF\\PresentationFramework.dll", "WPF\\WindowsBase.dll"
            };

            foreach (string i in dependencies)
                parameters.ReferencedAssemblies.Add(i);

            parameters.OutputAssembly = outputFile;
            CompilerResults results = provider.CompileAssemblyFromFile(parameters, file);

            var errors = results.Errors.Cast<CompilerError>().Select((i) => "Line Number " +
                i.Line + "\r\n" + "Error Number: " + i.ErrorNumber + "\r\n" + i.ErrorText);

            if (errors.Count() > 0)
                ConsoleHelp.WriteError(string.Join("\r\n\r\n", errors), Path.GetFileName(file));
        }

        static string GetMD5(string code)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBuffer = Encoding.UTF8.GetBytes(code);
                byte[] hashBuffer = md5.ComputeHash(inputBuffer);
                return BitConverter.ToString(md5.ComputeHash(inputBuffer)).Replace("-", "");
            }
        }
    }
}
