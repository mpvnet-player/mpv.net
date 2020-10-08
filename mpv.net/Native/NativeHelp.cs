
using System;

using static WinAPI;

namespace mpvnet
{
    public static class NativeHelp
    {
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

        public static void SubtractWindowBorders(IntPtr hwnd, ref RECT rc)
        {
            RECT r = new RECT(0, 0, 0, 0);
            AddWindowBorders(hwnd, ref r);
            rc.Left -= r.Left;
            rc.Top -= r.Top;
            rc.Right -= r.Right;
            rc.Bottom -= r.Bottom;
        }

        public static void AddWindowBorders(IntPtr hwnd, ref RECT rc)
        {
            AdjustWindowRect(ref rc, (uint)GetWindowLong(hwnd, -16 /* GWL_STYLE */), false);
        }
    }
}
