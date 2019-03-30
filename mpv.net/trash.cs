//public static string GetAssociatedApplication(string ext)
//{
//    uint returnValue = 0U;
//    // ASSOCF_VERIFY, ASSOCSTR_EXECUTABLE
//    if (1 == Native.AssocQueryString(0x40, 2, ext, null, null, ref returnValue))
//    {
//        if (returnValue > 0)
//        {
//            StringBuilder sb = new StringBuilder(Convert.ToInt32(returnValue));
//            // ASSOCF_VERIFY, ASSOCSTR_EXECUTABLE
//            if (0 == Native.AssocQueryString(0x40, 2, ext, null, sb, ref returnValue))
//            {
//                var ret = sb.ToString();
//                if (File.Exists(ret)) return ret;
//            }
//        }
//    }
//    return "";
//}

//[DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
//public static extern uint AssocQueryString(uint flags, uint str, string pszAssoc, string pszExtra, StringBuilder pszOut, ref uint pcchOut);