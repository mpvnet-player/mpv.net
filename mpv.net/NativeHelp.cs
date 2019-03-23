using System;
using System.IO;
using System.Text;

namespace mpvnet
{
    public static class NativeHelp
    {
        public static int GetResizeBorder(int v)
        {
            switch (v)
            {
                case 1 /* WMSZ_LEFT */       : return 3;
                case 3 /* WMSZ_TOP */        : return 2;
                case 2 /* WMSZ_RIGHT */      : return 3;
                case 6 /* WMSZ_BOTTOM */     : return 2;
                case 4 /* WMSZ_TOPLEFT */    : return 1;
                case 5 /* WMSZ_TOPRIGHT */   : return 1;
                case 7 /* WMSZ_BOTTOMLEFT */ : return 3;
                case 8 /* WMSZ_BOTTOMRIGHT */: return 3;
                default: return -1;
            }
        }

        public static void SubtractWindowBorders(IntPtr hwnd, ref Native.RECT rc)
        {
            var b = new Native.RECT(0, 0, 0, 0);
            AddWindowBorders(hwnd, ref b);
            rc.Left -= b.Left;
            rc.Top -= b.Top;
            rc.Right -= b.Right;
            rc.Bottom -= b.Bottom;
        }

        public static void AddWindowBorders(IntPtr hwnd, ref Native.RECT rc)
        {
            Native.AdjustWindowRect(ref rc, (uint)Native.GetWindowLongPtrW(hwnd, -16 /* GWL_STYLE */), false);
        }

        public static string GetAssociatedApplication(string ext)
        {
            uint returnValue = 0U;
            // ASSOCF_VERIFY, ASSOCSTR_EXECUTABLE
            if (1 == Native.AssocQueryString(0x40, 2, ext, null, null, ref returnValue))
            {
                if (returnValue > 0)
                {
                    StringBuilder sb = new StringBuilder(Convert.ToInt32(returnValue));
                    // ASSOCF_VERIFY, ASSOCSTR_EXECUTABLE
                    if (0 == Native.AssocQueryString(0x40, 2, ext, null, sb, ref returnValue))
                    {
                        var ret = sb.ToString();
                        if (File.Exists(ret)) return ret;
                    }
                }
            }
            return "";
        }
    }
}