using System;
using System.Runtime.InteropServices;

public class Taskbar
{
    private ITaskbarList3 Instance = (ITaskbarList3)new TaskBarCommunication();

    public IntPtr Handle { get; set; }

    public Taskbar(IntPtr handle) => Handle = handle;

    [ComImportAttribute]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    private interface ITaskbarList3
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
        [PreserveSig] void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        [PreserveSig] void SetProgressState(IntPtr hwnd, TaskbarStates state);
    }

    [ComImportAttribute]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
    private class TaskBarCommunication { }

    public void SetState(TaskbarStates taskbarState)
    {
        Instance.SetProgressState(Handle, taskbarState);
    }

    public void SetValue(double progressValue, double progressMax)
    {
        Instance.SetProgressValue(Handle, (UInt64)progressValue, (UInt64)progressMax);
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