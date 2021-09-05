
// https://github.com/Willy-Kimura/BetterFolderBrowser

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;

namespace mpvnet
{
    public partial class BetterFolderBrowser : CommonDialog
    {
        IContainer components = null;
        BetterFolderBrowserDialog _dialog = new BetterFolderBrowserDialog();

        public BetterFolderBrowser()
        {
            InitializeComponent();
            SetDefaults();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        void InitializeComponent() => components = new Container();

        public string Title
        {
            get { return _dialog.Title; }
            set { _dialog.Title = value; }
        }

        public string RootFolder
        {
            get { return _dialog.InitialDirectory; }
            set { _dialog.InitialDirectory = value; }
        }

        public bool Multiselect
        {
            get { return _dialog.AllowMultiselect; }
            set { _dialog.AllowMultiselect = value; }
        }

        public string SelectedPath => _dialog.FileName;

        public string[] SelectedPaths => _dialog.FileNames;

        public string SelectedFolder => _dialog.FileName;

        public string[] SelectedFolders => _dialog.FileNames;

        void SetDefaults()
        {
            _dialog.AllowMultiselect = false;
            _dialog.Title = "Please select a folder...";
            _dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public new DialogResult ShowDialog()
        {
            DialogResult result;

            if (_dialog.ShowDialog(IntPtr.Zero))
                result = DialogResult.OK;
            else
                result = DialogResult.Cancel;

            return result;
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult result;

            if (_dialog.ShowDialog(owner.Handle))
                result = DialogResult.OK;
            else
                result = DialogResult.Cancel;

            return result;
        }

        protected override bool RunDialog(IntPtr hwndOwner) => _dialog.ShowDialog(hwndOwner);

        public override void Reset() => SetDefaults();

        class BetterFolderBrowserDialog
        {
            OpenFileDialog ofd = null;

            public BetterFolderBrowserDialog()
            {
                ofd = new OpenFileDialog();

                ofd.Filter = "Folders|" + "\n";
                ofd.AddExtension = false;
                ofd.CheckFileExists = false;
                ofd.DereferenceLinks = true;
                ofd.Multiselect = false;
            }

            public bool AllowMultiselect {
                get { return ofd.Multiselect; }
                set { ofd.Multiselect = value; }
            }

            public string[] FileNames => ofd.FileNames;

            public string InitialDirectory {
                get { return ofd.InitialDirectory; }
                set {
                    ofd.InitialDirectory = (value == null || value.Length == 0) ? Environment.CurrentDirectory : value;
                }
            }

            public string Title {
                get { return ofd.Title; }
                set { ofd.Title = (value == null) ? "Select a folder" : value; }
            }

            public string FileName => ofd.FileName;

            public bool ShowDialog() => ShowDialog(IntPtr.Zero);

            public bool ShowDialog(IntPtr hWndOwner)
            {
                bool flag = false;

                var r = new Reflector("System.Windows.Forms");

                uint num = 0;
                Type typeIFileDialog = r.GetTypo("FileDialogNative.IFileDialog");
                object dialog = r.Call(ofd, "CreateVistaDialog");
                r.Call(ofd, "OnBeforeVistaDialog", dialog);

                uint options = Convert.ToUInt32(r.CallAs(typeof(FileDialog), ofd, "GetOptions"));
                options |= Convert.ToUInt32(r.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS"));
                r.CallAs(typeIFileDialog, dialog, "SetOptions", options);

                object pfde = r.New("FileDialog.VistaDialogEvents", ofd);
                object[] parameters = new object[] { pfde, num };
                r.CallAs2(typeIFileDialog, dialog, "Advise", parameters);

                num = Convert.ToUInt32(parameters[1]);

                try
                {
                    int num2 = Convert.ToInt32(r.CallAs(typeIFileDialog, dialog, "Show", hWndOwner));
                    flag = 0 == num2;
                }
                finally
                {
                    r.CallAs(typeIFileDialog, dialog, "Unadvise", num);
                    GC.KeepAlive(pfde);
                }

                return flag;
            }
        }

        class WindowWrapper : IWin32Window
        {
            IntPtr _hwnd;

            public IntPtr Handle => _hwnd;

            public WindowWrapper(IntPtr handle) => _hwnd = handle;
        }

        class Reflector
        {
            string m_ns;
            Assembly m_asmb;

            public Reflector(string ns) : this(ns, ns) { }

            public Reflector(string an__1, string ns)
            {
                m_ns = ns;
                m_asmb = null;

                foreach (AssemblyName aN__2 in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    if (aN__2.FullName.StartsWith(an__1))
                    {
                        m_asmb = Assembly.Load(aN__2);
                        break;
                    }
                }
            }

            public Type GetTypo(string typeName)
            {
                Type type = null;
                string[] names = typeName.Split('.');

                if (names.Length > 0)
                    type = m_asmb.GetType((m_ns + Convert.ToString(".")) + names[0]);

                for (int i = 1; i < names.Length; i++)
                    type = type.GetNestedType(names[i], BindingFlags.NonPublic);

                return type;
            }

            public object New(string name, params object[] parameters)
            {
                Type type = GetTypo(name);
                ConstructorInfo[] ctorInfos = type.GetConstructors();

                foreach (ConstructorInfo ci in ctorInfos)
                {
                    try {
                        return ci.Invoke(parameters);
                    } catch { }
                }

                return null;
            }

            public object Call(object obj, string func, params object[] parameters)
            {
                return Call2(obj, func, parameters);
            }

            public object Call2(object obj, string func, object[] parameters)
            {
                return CallAs2(obj.GetType(), obj, func, parameters);
            }

            public object CallAs(Type type, object obj, string func, params object[] parameters)
            {
                return CallAs2(type, obj, func, parameters);
            }

            public object CallAs2(Type type, object obj, string func, object[] parameters)
            {
                MethodInfo methInfo = type.GetMethod(
                    func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return methInfo.Invoke(obj, parameters);
            }

            public object Get(object obj, string prop) => GetAs(obj.GetType(), obj, prop);

            public object GetAs(Type type, object obj, string prop)
            {
                PropertyInfo propInfo = type.GetProperty(
                    prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                return propInfo.GetValue(obj, null);
            }

            public object GetEnum(string typeName, string name)
            {
                Type type = GetTypo(typeName);
                FieldInfo fieldInfo = type.GetField(name);
                return fieldInfo.GetValue(null);
            }
        }
    }
}
