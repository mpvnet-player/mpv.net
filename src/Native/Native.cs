
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace mpvnet
{
    public class Native
    {
        static Version Windows_10_1607 = new Version(10, 0, 14393); // Windows 10 1607

        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string path);

        [DllImport("user32.dll")]
        public static extern uint ActivateKeyboardLayout(IntPtr hkl, uint flags);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(
            IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegisterWindowMessage(string id);

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("user32.dll")]
        public static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRect(ref RECT lpRect, uint dwStyle, bool bMenu);

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectExForDpi(
            ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle, uint dpi);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr(hWnd, nIndex);
            else
                return GetWindowLong32(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, uint dwNewLong);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            else
                return SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(
            IntPtr hwnd, uint dwAttribute, out RECT pvAttribute, uint cbAttribute);

        public static bool GetDwmWindowRect(IntPtr handle, out RECT rect)
        {
            const uint DWMWA_EXTENDED_FRAME_BOUNDS = 9;
            return 0 == DwmGetWindowAttribute(handle, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, (uint)Marshal.SizeOf<RECT>());
        }

        public static Rectangle GetWorkingArea(IntPtr handle, Rectangle workingArea)
        {
            if (handle != IntPtr.Zero && GetDwmWindowRect(handle, out RECT dwmRect) &&
                GetWindowRect(handle, out RECT rect))
            {
                int left = workingArea.Left;
                int top = workingArea.Top;
                int right = workingArea.Right;
                int bottom = workingArea.Bottom;

                left += rect.Left - dwmRect.Left;
                top -= rect.Top - dwmRect.Top;
                right -= dwmRect.Right - rect.Right;
                bottom -= dwmRect.Bottom - rect.Bottom;

                return new Rectangle(left, top, right - left, bottom - top);
            }

            return workingArea;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(Rectangle r)
            {
                Left = r.Left;
                Top = r.Top;
                Right = r.Right;
                Bottom = r.Bottom;
            }

            public RECT(int left, int top, int right, int bottom)
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

            public static RECT FromRectangle(Rectangle rect)
            {
                return new RECT(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            }

            public override string ToString()
            {
                return "{Left=" + Left + ",Top=" + Top + ",Right=" + Right + ",Bottom=" + Bottom + "}";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpData;
        }

        public static int GetResizeBorder(int v)
        {
            switch (v)
            {
                case 1 /* WMSZ_LEFT        */ : return 3;
                case 3 /* WMSZ_TOP         */ : return 2;
                case 2 /* WMSZ_RIGHT       */ : return 3;
                case 6 /* WMSZ_BOTTOM      */ : return 2;
                case 4 /* WMSZ_TOPLEFT     */ : return 1;
                case 5 /* WMSZ_TOPRIGHT    */ : return 1;
                case 7 /* WMSZ_BOTTOMLEFT  */ : return 3;
                case 8 /* WMSZ_BOTTOMRIGHT */ : return 3;
                default: return -1;
            }
        }

        public static void SubtractWindowBorders(IntPtr hwnd, ref RECT rc, int dpi)
        {
            RECT r = new RECT(0, 0, 0, 0);
            AddWindowBorders(hwnd, ref r, dpi);
            rc.Left -= r.Left;
            rc.Top -= r.Top;
            rc.Right -= r.Right;
            rc.Bottom -= r.Bottom;
        }

        public static void AddWindowBorders(IntPtr hwnd, ref RECT rc, int dpi)
        {
            uint windowStyle   = (uint)GetWindowLong(hwnd, -16); // GWL_STYLE
            uint windowStyleEx = (uint)GetWindowLong(hwnd, -20); // GWL_EXSTYLE

            if (Environment.OSVersion.Version >= Windows_10_1607)
                AdjustWindowRectExForDpi(ref rc, windowStyle, false, windowStyleEx, (uint)dpi);
            else
                AdjustWindowRect(ref rc, windowStyle, false);
        }

        public static int GetDPI(IntPtr hwnd)
        {
            if (Environment.OSVersion.Version >= Windows_10_1607 && hwnd != IntPtr.Zero)
                return GetDpiForWindow(hwnd);
            else
                using (Graphics gx = Graphics.FromHwnd(hwnd))
                    return GetDeviceCaps(gx.GetHdc(), 88 /*LOGPIXELSX*/);
        }
    }
}
