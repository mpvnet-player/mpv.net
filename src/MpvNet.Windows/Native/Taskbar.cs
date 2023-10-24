
using System.Runtime.InteropServices;

public class Taskbar
{
    public IntPtr Handle { get; set; }

    public Taskbar(IntPtr handle) => Handle = handle;
 
    ITaskbarList3 Instance = (ITaskbarList3)new TaskBarCommunication();

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEFAF")]
    interface ITaskbarList3
    {
        // ITaskbarList
        [PreserveSig] void HrInit();
        [PreserveSig] void AddTab(IntPtr hwnd);
        [PreserveSig] void DeleteTab(IntPtr hwnd);
        [PreserveSig] void ActivateTab(IntPtr hwnd);
        [PreserveSig] void SetActiveAlt(IntPtr hwnd);
        // ITaskbarList2
        [PreserveSig] void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
        // ITaskbarList3
        [PreserveSig] void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
        [PreserveSig] void SetProgressState(IntPtr hwnd, TaskbarStates state);
    }

    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
    class TaskBarCommunication
    {
    }

    public void SetState(TaskbarStates taskbarState)
    {
        Instance.SetProgressState(Handle, taskbarState);
    }

    public void SetValue(double progressValue, double progressMax)
    {
        Instance.SetProgressValue(Handle, (ulong)progressValue, (ulong)progressMax);
    }
}

public enum TaskbarStates
{
    NoProgress = 0,
    Indeterminate = 0x1,
    Normal = 0x2,
    Error = 0x4,
    Paused = 0x8
}
