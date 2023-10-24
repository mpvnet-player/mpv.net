
using System.Runtime.InteropServices;

public class StockIcon
{
    [DllImport("shell32.dll")]
    public static extern int SHGetStockIconInfo(SHSTOCKICONID siid, SHSTOCKICONFLAGS uFlags, ref SHSTOCKICONINFO info);

    [DllImport("user32.dll")]
    public static extern bool DestroyIcon(IntPtr handle);

    public static IntPtr GetIcon(SHSTOCKICONID identifier, SHSTOCKICONFLAGS flags)
    {
        SHSTOCKICONINFO info = new SHSTOCKICONINFO();
        info.cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(SHSTOCKICONINFO)));
        Marshal.ThrowExceptionForHR(SHGetStockIconInfo(identifier, flags, ref info));
        return info.hIcon;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHSTOCKICONINFO
    {
        public uint cbSize;
        public IntPtr hIcon;
        int iSysImageIndex;
        int iIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        string szPath;
    }

    public enum SHSTOCKICONFLAGS : uint
    {
        SHGSI_ICONLOCATION = 0,
        SHGSI_ICON = 0x000000100,
        SHGSI_SYSICONINDEX = 0x000004000,
        SHGSI_LINKOVERLAY = 0x000008000,
        SHGSI_SELECTED = 0x000010000,
        SHGSI_LARGEICON = 0x000000000,
        SHGSI_SMALLICON = 0x000000001,
        SHGSI_SHELLICONSIZE = 0x000000004
    }

    public enum SHSTOCKICONID : uint
    {
        DocumentNotAssociated = 0,
        DocumentAssociated = 1,
        Application = 2,
        Folder = 3,
        FolderOpen = 4,
        Drive525 = 5,
        Drive35 = 6,
        DriveRemove = 7,
        DriveFixed = 8,
        DriveNetwork = 9,
        DriveNetworkDisabled = 10,
        DriveCD = 11,
        DriveRAM = 12,
        World = 13,
        Server = 15,
        Printer = 16,
        MyNetwork = 17,
        Find = 22,
        Help = 23,
        Share = 28,
        Link = 29,
        SlowFile = 30,
        Recycler = 31,
        RecyclerFull = 32,
        MediaCDAudio = 40,
        Lock = 47,
        AutoList = 49,
        PrinterNet = 50,
        ServerShare = 51,
        PrinterFax = 52,
        PrinterFaxNet = 53,
        PrinterFile = 54,
        Stack = 55,
        MediaSVCD = 56,
        StuffedFolder = 57,
        DriveUnknown = 58,
        DriveDVD = 59,
        MediaDVD = 60,
        MediaDVDRAM = 61,
        MediaDVDRW = 62,
        MediaDVDR = 63,
        MediaDVDROM = 64,
        MediaCDAudioPlus = 65,
        MediaCDRW = 66,
        MediaCDR = 67,
        MediaCDBurn = 68,
        MediaBlankCD = 69,
        MediaCDROM = 70,
        AudioFiles = 71,
        ImageFiles = 72,
        VideoFiles = 73,
        MixedFiles = 74,
        FolderBack = 75,
        FolderFront = 76,
        Shield = 77,
        Warning = 78,
        Info = 79,
        Error = 80,
        Key = 81,
        Software = 82,
        Rename = 83,
        Delete = 84,
        MediaAudioDVD = 85,
        MediaMovieDVD = 86,
        MediaEnhancedCD = 87,
        MediaEnhancedDVD = 88,
        MediaHDDVD = 89,
        MediaBluRay = 90,
        MediaVCD = 91,
        MediaDVDPlusR = 92,
        MediaDVDPlusRW = 93,
        DesktopPC = 94,
        MobilePC = 95,
        Users = 96,
        MediaSmartMedia = 97,
        MediaCompactFlash = 98,
        DeviceCellPhone = 99,
        DeviceCamera = 100,
        DeviceVideoCamera = 101,
        DeviceAudioPlayer = 102,
        NetworkConnect = 103,
        Internet = 104,
        ZipFile = 105,
        Settings = 106
    }
}
