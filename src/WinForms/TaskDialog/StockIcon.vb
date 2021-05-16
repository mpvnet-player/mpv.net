
Imports System.Drawing
Imports System.Runtime.InteropServices

<Flags()>
Enum StockIconOptions As UInteger
    Small = &H1
    ShellSize = &H4
    Handle = &H100
    SystemIndex = &H4000
    LinkOverlay = &H8000
    Selected = &H10000
End Enum

Enum StockIconIdentifier As UInteger
    DocumentNotAssociated = 0
    DocumentAssociated = 1
    Application = 2
    Folder = 3
    FolderOpen = 4
    Drive525 = 5
    Drive35 = 6
    DriveRemove = 7
    DriveFixed = 8
    DriveNetwork = 9
    DriveNetworkDisabled = 10
    DriveCD = 11
    DriveRAM = 12
    World = 13
    Server = 15
    Printer = 16
    MyNetwork = 17
    Find = 22
    Help = 23
    Share = 28
    Link = 29
    SlowFile = 30
    Recycler = 31
    RecyclerFull = 32
    MediaCDAudio = 40
    Lock = 47
    AutoList = 49
    PrinterNet = 50
    ServerShare = 51
    PrinterFax = 52
    PrinterFaxNet = 53
    PrinterFile = 54
    Stack = 55
    MediaSVCD = 56
    StuffedFolder = 57
    DriveUnknown = 58
    DriveDVD = 59
    MediaDVD = 60
    MediaDVDRAM = 61
    MediaDVDRW = 62
    MediaDVDR = 63
    MediaDVDROM = 64
    MediaCDAudioPlus = 65
    MediaCDRW = 66
    MediaCDR = 67
    MediaCDBurn = 68
    MediaBlankCD = 69
    MediaCDROM = 70
    AudioFiles = 71
    ImageFiles = 72
    VideoFiles = 73
    MixedFiles = 74
    FolderBack = 75
    FolderFront = 76
    Shield = 77
    Warning = 78
    Info = 79
    [Error] = 80
    Key = 81
    Software = 82
    Rename = 83
    Delete = 84
    MediaAudioDVD = 85
    MediaMovieDVD = 86
    MediaEnhancedCD = 87
    MediaEnhancedDVD = 88
    MediaHDDVD = 89
    MediaBluRay = 90
    MediaVCD = 91
    MediaDVDPlusR = 92
    MediaDVDPlusRW = 93
    DesktopPC = 94
    MobilePC = 95
    Users = 96
    MediaSmartMedia = 97
    MediaCompactFlash = 98
    DeviceCellPhone = 99
    DeviceCamera = 100
    DeviceVideoCamera = 101
    DeviceAudioPlayer = 102
    NetworkConnect = 103
    Internet = 104
    ZipFile = 105
    Settings = 106
End Enum

Class StockIcon
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Structure StockIconInfo
        Friend StuctureSize As UInt32
        Friend Handle As IntPtr
        Friend ImageIndex As Int32
        Friend Identifier As Int32
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Friend Path As String
    End Structure

    <DllImport("shell32.dll")>
    Shared Function SHGetStockIconInfo(
            identifier As StockIconIdentifier,
            flags As StockIconOptions,
            ByRef info As StockIconInfo) As Integer
    End Function

    <DllImport("user32.dll")>
    Shared Function DestroyIcon(handle As IntPtr) As Boolean
    End Function

    Shared Function GetSmallImage(identifier As StockIconIdentifier) As Bitmap
        Dim ptr = GetIcon(identifier, StockIconOptions.Handle Or StockIconOptions.Small)
        Dim bmp = Icon.FromHandle(ptr).ToBitmap
        DestroyIcon(ptr)
        Return bmp
    End Function

    Shared Function GetImage(identifier As StockIconIdentifier) As Image
        Dim ptr = GetIcon(identifier, StockIconOptions.Handle Or StockIconOptions.ShellSize)
        Dim bmp = Icon.FromHandle(ptr).ToBitmap
        DestroyIcon(ptr)
        Return bmp
    End Function

    Shared Function GetIcon(identifier As StockIconIdentifier, flags As StockIconOptions) As IntPtr
        Dim info As New StockIconInfo()
        info.StuctureSize = CType(Marshal.SizeOf(GetType(StockIconInfo)), UInt32)
        Marshal.ThrowExceptionForHR(SHGetStockIconInfo(identifier, flags, info))
        Return info.Handle
    End Function
End Class
