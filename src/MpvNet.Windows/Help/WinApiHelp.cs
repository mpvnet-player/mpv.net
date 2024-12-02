
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

using static MpvNet.Windows.Native.WinApi;

namespace MpvNet.Windows.Help;

public static class WinApiHelp
{
    public static Version WindowsTen1607 { get; } = new Version(10, 0, 14393); // Windows 10 1607

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

    public static void AdjustWindowRectangle(IntPtr hwnd, ref RECT rc, int dpi)
    {
        uint style = (uint)GetWindowLongPtr(hwnd, -16); // GWL_STYLE
        uint styleEx = (uint)GetWindowLongPtr(hwnd, -20); // GWL_EXSTYLE

        if (Environment.OSVersion.Version >= WindowsTen1607)
            AdjustWindowRectExForDpi(ref rc, style, false, styleEx, (uint)dpi);
        else
            AdjustWindowRect(ref rc, style, false);
    }

    public static void AddWindowBorders(IntPtr hwnd, ref RECT rc, int dpi, bool changeTop)
    {
        RECT win = rc;
        AdjustWindowRectangle(hwnd, ref rc, dpi);

        if (changeTop)
        {
            int top = rc.Top;
            top -= rc.Top - win.Top;
            rc = new RECT(rc.Left, top, rc.Right, rc.Bottom);
        }
    }

    public static void SubtractWindowBorders(IntPtr hwnd, ref RECT rc, int dpi, bool changeTop)
    {
        RECT r = new RECT();
        AddWindowBorders(hwnd, ref r, dpi, changeTop);
        rc.Left -= r.Left;
        rc.Top -= r.Top;
        rc.Right -= r.Right;
        rc.Bottom -= r.Bottom;
    }

    public static int GetTitleBarHeight(IntPtr hwnd, int dpi)
    {
        RECT rect = new RECT();
        AdjustWindowRectangle(hwnd, ref rect, dpi);
        return -rect.Top;
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

    public static bool GetDwmWindowRect(IntPtr handle, out RECT rect)
    {
        const uint DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        return 0 == DwmGetWindowAttribute(handle, DWMWA_EXTENDED_FRAME_BOUNDS,
            out rect, (uint)Marshal.SizeOf<RECT>());
    }

    public static string GetAppPathForExtension(params string[] extensions)
    {
        foreach (string it in extensions)
        {
            string extension = it;

            if (!extension.StartsWith("."))
                extension = "." + extension;

            uint c = 0U;

            if (AssocQueryString(0x40, 2, extension, null, null, ref c) == 1)
            {
                if (c > 0L)
                {
                    var sb = new StringBuilder((int)c);

                    if (0 == AssocQueryString(0x40, 2, extension, default, sb, ref c))
                    {
                        string ret = sb.ToString();

                        if (File.Exists(ret))
                            return ret;
                    }
                }
            }
        }

        return "";
    }
}
