Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text

Imports vbnet.UI

Public Class OSVersion
    Shared Property Windows7 As Single = 6.1
    Shared Property Windows8 As Single = 6.2
    Shared Property Windows10 As Single = 10.0

    Shared ReadOnly Property Current As Single
        Get
            Return CSng(Environment.OSVersion.Version.Major + Environment.OSVersion.Version.Minor / 10)
        End Get
    End Property
End Class

Public Class ProcessHelp
    Shared Sub Start(cmd As String, Optional args As String = Nothing)
        Try
            Process.Start(cmd, args)
        Catch ex As Exception
            If cmd Like "http*://*" Then
                MsgError("Failed to open URL with browser." + BR2 + cmd, ex.Message)
            ElseIf File.Exists(cmd) Then
                MsgError("Failed to launch file." + BR2 + cmd, ex.Message)
            ElseIf Directory.Exists(cmd) Then
                MsgError("Failed to launch directory." + BR2 + cmd, ex.Message)
            Else
                MsgException(ex, "Failed to execute command:" + BR2 + cmd + BR2 + "Arguments:" + BR2 + args)
            End If
        End Try
    End Sub
End Class

Public Class KeysHelp
    Private Shared Converter As TypeConverter = TypeDescriptor.GetConverter(GetType(Keys))
    Private Shared KeysTexts As Dictionary(Of Keys, String)

    Shared Sub New()
        KeysTexts = New Dictionary(Of Keys, String)
        KeysTexts(Keys.Add) = "+ (Numpad)"
        KeysTexts(Keys.Back) = "Back"
        KeysTexts(Keys.Decimal) = "Decimal"
        KeysTexts(Keys.Delete) = "Delete"
        KeysTexts(Keys.Divide) = "Divide"
        KeysTexts(Keys.Down) = "Down"
        KeysTexts(Keys.End) = "End"
        KeysTexts(Keys.Enter) = "Enter"
        KeysTexts(Keys.Escape) = "Escape"
        KeysTexts(Keys.Home) = "Home"
        KeysTexts(Keys.Insert) = "Insert"
        KeysTexts(Keys.Left) = "Left"
        KeysTexts(Keys.Multiply) = "Multiply"
        KeysTexts(Keys.Next) = "Page Down"
        KeysTexts(Keys.Prior) = "Page Up"
        KeysTexts(Keys.Right) = "Right"
        KeysTexts(Keys.Space) = "Space"
        KeysTexts(Keys.Subtract) = "- (Numpad)"
        KeysTexts(Keys.Up) = "Up"
        KeysTexts(Keys.Control) = "Control"
        KeysTexts(Keys.Alt) = "Alt"
        KeysTexts(Keys.Shift) = "Shift"

        KeysTexts(Keys.D0) = "0"
        KeysTexts(Keys.D1) = "1"
        KeysTexts(Keys.D2) = "2"
        KeysTexts(Keys.D3) = "3"
        KeysTexts(Keys.D4) = "4"
        KeysTexts(Keys.D5) = "5"
        KeysTexts(Keys.D6) = "6"
        KeysTexts(Keys.D7) = "7"
        KeysTexts(Keys.D8) = "8"
        KeysTexts(Keys.D9) = "9"

        KeysTexts(Keys.NumPad0) = "0 (Numpad)"
        KeysTexts(Keys.NumPad1) = "1 (Numpad)"
        KeysTexts(Keys.NumPad2) = "2 (Numpad)"
        KeysTexts(Keys.NumPad3) = "3 (Numpad)"
        KeysTexts(Keys.NumPad4) = "4 (Numpad)"
        KeysTexts(Keys.NumPad5) = "5 (Numpad)"
        KeysTexts(Keys.NumPad6) = "6 (Numpad)"
        KeysTexts(Keys.NumPad7) = "7 (Numpad)"
        KeysTexts(Keys.NumPad8) = "8 (Numpad)"
        KeysTexts(Keys.NumPad9) = "9 (Numpad)"
    End Sub

    Shared Function GetKeyString(k As Keys) As String
        If k = Keys.None Then Return ""

        Dim s = ""

        If (k And Keys.Control) = Keys.Control Then
            k = k Xor Keys.Control
            s += "Ctrl+"
        End If

        If (k And Keys.Alt) = Keys.Alt Then
            k = k Xor Keys.Alt
            s += "Alt+"
        End If

        If (k And Keys.Shift) = Keys.Shift Then
            k = k Xor Keys.Shift
            s += "Shift+"
        End If

        If KeysTexts.ContainsKey(k) Then
            s += KeysTexts(k)
        Else
            Dim value = MapVirtualKey(CInt(k), 2) 'MAPVK_VK_TO_CHAR

            If value = 0 OrElse (value And 1 << 31) = 1 << 31 Then
                s += k.ToString
            Else
                s += Convert.ToChar(value)
            End If
        End If

        Return s
    End Function

    <DllImport("user32.dll")>
    Shared Function MapVirtualKey(wCode As Integer, wMapType As Integer) As Integer
    End Function
End Class

<Serializable>
Public Class StringPair
    Implements IComparable(Of StringPair)

    Property Name As String
    Property Value As String

    Sub New()
    End Sub

    Sub New(name As String, text As String)
        Me.Name = name
        Me.Value = text
    End Sub

    Function CompareTo(other As StringPair) As Integer Implements System.IComparable(Of StringPair).CompareTo
        Return Name.CompareTo(other.Name)
    End Function
End Class

<Serializable()>
Public Class StringPairList
    Inherits List(Of StringPair)

    Sub New()
    End Sub

    Sub New(list As IEnumerable(Of StringPair))
        AddRange(list)
    End Sub

    Overloads Sub Add(name As String, text As String)
        Add(New StringPair(name, text))
    End Sub
End Class

Public Class Folder

#Region "System"

    Shared ReadOnly Property Desktop() As String
        Get
            Return Environment.GetFolderPath(Environment.SpecialFolder.Desktop).FixDir
        End Get
    End Property

    Shared ReadOnly Property Startup() As String
        Get
            Return Application.StartupPath.FixDir
        End Get
    End Property

    Shared ReadOnly Property Current() As String
        Get
            Return Environment.CurrentDirectory.FixDir
        End Get
    End Property

    Shared ReadOnly Property Temp() As String
        Get
            Return Path.GetTempPath.FixDir
        End Get
    End Property

    Shared ReadOnly Property System() As String
        Get
            Return Environment.SystemDirectory.FixDir
        End Get
    End Property

    Shared ReadOnly Property Programs() As String
        Get
            Return GetFolderPath(Environment.SpecialFolder.ProgramFiles).FixDir
        End Get
    End Property

    Shared ReadOnly Property Home() As String
        Get
            Return GetFolderPath(Environment.SpecialFolder.UserProfile).FixDir
        End Get
    End Property

    Shared ReadOnly Property AppDataCommon() As String
        Get
            Return GetFolderPath(Environment.SpecialFolder.CommonApplicationData).FixDir
        End Get
    End Property

    Shared ReadOnly Property AppDataLocal() As String
        Get
            Return GetFolderPath(Environment.SpecialFolder.LocalApplicationData).FixDir
        End Get
    End Property

    Shared ReadOnly Property AppDataRoaming() As String
        Get
            Return GetFolderPath(Environment.SpecialFolder.ApplicationData).FixDir
        End Get
    End Property

    Shared ReadOnly Property Windows() As String
        Get
            Return GetFolderPath(Environment.SpecialFolder.Windows).FixDir
        End Get
    End Property

#End Region

#Region "StaxRip"

    Shared ReadOnly Property Apps As String
        Get
            Return Folder.Startup + "Apps\"
        End Get
    End Property

#End Region

    <DllImport("shfolder.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function SHGetFolderPath(hwndOwner As IntPtr, nFolder As Integer, hToken As IntPtr, dwFlags As Integer, lpszPath As StringBuilder) As Integer
    End Function

    Private Shared Function GetFolderPath(folder As Environment.SpecialFolder) As String
        Dim sb As New StringBuilder(260)
        SHGetFolderPath(IntPtr.Zero, CInt(folder), IntPtr.Zero, 0, sb)
        Dim ret = sb.ToString.FixDir '.NET fails on 'D:'
        Call New FileIOPermission(FileIOPermissionAccess.PathDiscovery, ret).Demand()
        Return ret
    End Function
End Class

Public Class PathBase
    Shared ReadOnly Property Separator() As Char
        Get
            Return Path.DirectorySeparatorChar
        End Get
    End Property

    Shared Function IsSameBase(a As String, b As String) As Boolean
        Return FilePath.GetBase(a).EqualIgnoreCase(FilePath.GetBase(b))
    End Function

    Shared Function IsSameDir(a As String, b As String) As Boolean
        Return FilePath.GetDir(a).EqualIgnoreCase(FilePath.GetDir(b))
    End Function

    Shared Function IsValidFileSystemName(name As String) As Boolean
        If name = "" Then Return False
        Dim chars = """*/:<>?\|^".ToCharArray

        For Each i In name.ToCharArray
            If chars.Contains(i) Then Return False
            If Convert.ToInt32(i) < 32 Then Return False
        Next

        Return True
    End Function

    Shared Function RemoveIllegalCharsFromName(name As String) As String
        If name = "" Then Return ""

        Dim chars = """*/:<>?\|^".ToCharArray

        For Each i In name.ToCharArray
            If chars.Contains(i) Then
                name = name.Replace(i, "_")
            End If
        Next

        For x = 1 To 31
            If name.Contains(Convert.ToChar(x)) Then
                name = name.Replace(Convert.ToChar(x), "_"c)
            End If
        Next

        Return name
    End Function
End Class

Public Class DirPath
    Inherits PathBase

    Shared Function TrimTrailingSeparator(path As String) As String
        If path = "" Then Return ""

        If path.EndsWith(Separator) AndAlso Not path.Length <= 3 Then
            Return path.TrimEnd(Separator)
        End If

        Return path
    End Function

    Shared Function FixSeperator(path As String) As String
        If path.Contains("\") AndAlso Separator <> "\" Then
            path = path.Replace("\", Separator)
        End If

        If path.Contains("/") AndAlso Separator <> "/" Then
            path = path.Replace("/", Separator)
        End If

        Return path
    End Function

    Shared Function GetParent(path As String) As String
        If path = "" Then Return ""
        Dim temp = TrimTrailingSeparator(path)
        If temp.Contains(Separator) Then path = temp.LeftLast(Separator) + Separator
        Return path
    End Function

    Shared Function GetName(path As String) As String
        If path = "" Then Return ""
        path = TrimTrailingSeparator(path)
        Return path.RightLast(Separator)
    End Function

    Shared Function IsInSysDir(path As String) As Boolean
        If path = "" Then Return False
        If Not path.EndsWith("\") Then path += "\"
        Return path.ToUpper.Contains(Folder.Programs.ToUpper)
    End Function

    Shared Function IsFixedDrive(path As String) As Boolean
        Try
            If path <> "" Then Return New DriveInfo(path).DriveType = DriveType.Fixed
        Catch ex As Exception
        End Try
    End Function
End Class

Public Class FilePath
    Inherits PathBase

    Private Value As String

    Sub New(path As String)
        Value = path
    End Sub

    Shared Function GetDir(path As String) As String
        If path = "" Then Return ""
        If path.Contains("\") Then path = path.LeftLast("\") + "\"
        Return path
    End Function

    Shared Function GetDirAndBase(path As String) As String
        Return GetDir(path) + GetBase(path)
    End Function

    Shared Function GetName(path As String) As String
        If Not path Is Nothing Then
            Dim index = path.LastIndexOf(IO.Path.DirectorySeparatorChar)

            If index > -1 Then
                Return path.Substring(index + 1)
            End If
        End If

        Return path
    End Function

    Shared Function GetExtFull(filepath As String) As String
        Return GetExt(filepath, True)
    End Function

    Shared Function GetExt(filepath As String) As String
        Return GetExt(filepath, False)
    End Function

    Shared Function GetExt(filepath As String, dot As Boolean) As String
        If filepath = "" Then Return ""
        Dim chars = filepath.ToCharArray

        For x = filepath.Length - 1 To 0 Step -1
            If chars(x) = Separator Then Return ""
            If chars(x) = "."c Then Return filepath.Substring(x + If(dot, 0, 1)).ToLower
        Next

        Return ""
    End Function

    Shared Function GetDirNoSep(path As String) As String
        path = GetDir(path)
        If path.EndsWith(Separator) Then path = TrimSep(path)
        Return path
    End Function

    Shared Function GetBase(path As String) As String
        If path = "" Then Return ""
        Dim ret = path
        If ret.Contains(Separator) Then ret = ret.RightLast(Separator)
        If ret.Contains(".") Then ret = ret.LeftLast(".")
        Return ret
    End Function

    Shared Function TrimSep(path As String) As String
        If path = "" Then Return ""

        If path.EndsWith(Separator) AndAlso Not path.EndsWith(":" + Separator) Then
            Return path.TrimEnd(Separator)
        End If

        Return path
    End Function

    Shared Function GetDirNameOnly(path As String) As String
        Return FilePath.GetDirNoSep(path).RightLast("\")
    End Function
End Class

Public Class OS
    Shared Function GetTextEditor() As String
        Dim ret = GetAssociatedApplication(".txt")
        If ret <> "" Then Return ret
        Return "notepad.exe"
    End Function

    Shared Function GetAssociatedApplication(ext As String) As String
        Dim c = 0UI

        'ASSOCF_VERIFY, ASSOCSTR_EXECUTABLE
        If 1 = Native.AssocQueryString(&H40, 2, ext, Nothing, Nothing, c) Then
            If c > 0 Then
                Dim sb As New StringBuilder(CInt(c))

                'ASSOCF_VERIFY, ASSOCSTR_EXECUTABLE
                If 0 = Native.AssocQueryString(&H40, 2, ext, Nothing, sb, c) Then
                    Dim ret = sb.ToString
                    If File.Exists(ret) Then Return ret
                End If
            End If
        End If
    End Function
End Class