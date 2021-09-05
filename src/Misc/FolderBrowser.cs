
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mpvnet
{
    public class FolderBrowser
    {
        public string SelectedPath { get; set; }

        string _initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public string InitialDirectory {
            get => _initialDirectory;
            set {
                if (Directory.Exists(value))
                    _initialDirectory = value;
            }
        }

        public bool Show() => Show(GetOwnerHandle());

        public bool Show(IntPtr hWndOwner)
        {
            ShowDialogResult result = VistaDialog.Show(hWndOwner, InitialDirectory);

            if (result.Result)
                SelectedPath = result.FileName;

            return result.Result;
        }

        struct ShowDialogResult
        {
            public bool Result { get; set; }
            public string FileName { get; set; }
        }

        public static IntPtr GetOwnerHandle()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            GetWindowThreadProcessId(foregroundWindow, out var procID);

            using (var proc = Process.GetCurrentProcess())
                if (proc.Id == procID)
                    return foregroundWindow;

            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        static class VistaDialog
        {
            const string foldersFilter = "Folders|\n";
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            static Assembly windowsFormsAssembly = typeof(FileDialog).Assembly;
            static Type iFileDialogType = windowsFormsAssembly.GetType("System.Windows.Forms.FileDialogNative+IFileDialog");
            static MethodInfo createVistaDialogMethodInfo = typeof(OpenFileDialog).GetMethod("CreateVistaDialog", flags);
            static MethodInfo onBeforeVistaDialogMethodInfo = typeof(OpenFileDialog).GetMethod("OnBeforeVistaDialog", flags);
            static MethodInfo getOptionsMethodInfo = typeof(FileDialog).GetMethod("GetOptions", flags);
            static MethodInfo setOptionsMethodInfo = iFileDialogType.GetMethod("SetOptions", flags);
            static uint fosPickFoldersBitFlag = (uint)windowsFormsAssembly
                .GetType("System.Windows.Forms.FileDialogNative+FOS")
                .GetField("FOS_PICKFOLDERS")
                .GetValue(null);
            static ConstructorInfo vistaDialogEventsConstructorInfo = windowsFormsAssembly
                .GetType("System.Windows.Forms.FileDialog+VistaDialogEvents")
                .GetConstructor(flags, null, new[] { typeof(FileDialog) }, null);
            static MethodInfo adviseMethodInfo = iFileDialogType.GetMethod("Advise");
            static MethodInfo unAdviseMethodInfo = iFileDialogType.GetMethod("Unadvise");
            static MethodInfo showMethodInfo = iFileDialogType.GetMethod("Show");

            public static ShowDialogResult Show(IntPtr ownerHandle, string initialDirectory)
            {
                var openFileDialog = new OpenFileDialog
                {
                    AddExtension = false,
                    CheckFileExists = false,
                    DereferenceLinks = true,
                    Filter = foldersFilter,
                    InitialDirectory = initialDirectory,
                    Multiselect = false,
                };

                var iFileDialog = createVistaDialogMethodInfo.Invoke(openFileDialog, new object[] { });
                onBeforeVistaDialogMethodInfo.Invoke(openFileDialog, new[] { iFileDialog });
                setOptionsMethodInfo.Invoke(iFileDialog, new object[] { (uint)getOptionsMethodInfo.Invoke(openFileDialog, new object[] { }) | fosPickFoldersBitFlag });
                var adviseParametersWithOutputConnectionToken = new[] { vistaDialogEventsConstructorInfo.Invoke(new object[] { openFileDialog }), 0U };
                adviseMethodInfo.Invoke(iFileDialog, adviseParametersWithOutputConnectionToken);

                try
                {
                    int retVal = (int)showMethodInfo.Invoke(iFileDialog, new object[] { ownerHandle });

                    return new ShowDialogResult
                    {
                        Result = retVal == 0,
                        FileName = openFileDialog.FileName
                    };
                }
                finally
                {
                    unAdviseMethodInfo.Invoke(iFileDialog, new[] { adviseParametersWithOutputConnectionToken[1] });
                }
            }
        }

        class WindowWrapper : IWin32Window
        {
            IntPtr _handle;
            public WindowWrapper(IntPtr handle) { _handle = handle; }
            public IntPtr Handle => _handle;
        }
    }
}
