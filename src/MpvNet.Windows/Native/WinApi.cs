
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace MpvNet.Windows.Native;

public static class WinApi
{
    [DllImport("kernel32.dll")]
    public static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string path);

    [DllImport("user32.dll")]
    public static extern uint ActivateKeyboardLayout(IntPtr hkl, uint flags);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(
        IntPtr parentHandle, IntPtr childAfter, string lclassName, string? windowTitle);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref CopyDataStruct lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int RegisterWindowMessage(string id);

    [DllImport("user32.dll")]
    public static extern bool AllowSetForegroundWindow(int dwProcessId);

    [DllImport("user32.dll")]
    public static extern void ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern int GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern bool AdjustWindowRect(ref Rect lpRect, uint dwStyle, bool bMenu);

    [DllImport("user32.dll")]
    public static extern bool AdjustWindowRectExForDpi(
        ref Rect lpRect, uint dwStyle, bool bMenu, uint dwExStyle, uint dpi);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(
        IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("shlwapi", CharSet = CharSet.Auto)]
    public static extern uint AssocQueryString(
        uint flags, uint str, string? pszAssoc, string? pszExtra, [Out] StringBuilder? pszOut, ref uint pcchOut);

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(
        IntPtr hwnd, uint dwAttribute, out Rect pvAttribute, uint cbAttribute);

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rect(Rectangle r)
        {
            Left = r.Left;
            Top = r.Top;
            Right = r.Right;
            Bottom = r.Bottom;
        }

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rectangle ToRectangle() => Rectangle.FromLTRB(Left, Top, Right, Bottom);
        public Size Size => new Size(Right - Left, Bottom - Top);
        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public static Rect FromRectangle(Rectangle rect)
        {
            return new Rect(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
        }

        public override string ToString()
        {
            return "{Left=" + Left + ",Top=" + Top + ",Right=" + Right + ",Bottom=" + Bottom + "}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpData;
    }
}
