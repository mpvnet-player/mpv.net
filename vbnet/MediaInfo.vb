Imports System.Runtime.InteropServices

Public Class MediaInfo
    Implements IDisposable

    Private Handle As IntPtr
    Shared Loaded As Boolean

    Sub New(sourcepath As String)
        If Not Loaded Then
            If LoadLibrary("MediaInfo.dll") = IntPtr.Zero Then Throw New Exception("Failed to load MediaInfo.dll.")
            Loaded = True
        End If

        Handle = MediaInfo_New()
        MediaInfo_Open(Handle, sourcepath)
    End Sub

    Public Function GetInfo(streamKind As StreamKind, parameter As String) As String
        Return Marshal.PtrToStringUni(MediaInfo_Get(Handle, streamKind, 0, parameter, InfoKind.Text, InfoKind.Name))
    End Function

#Region "IDisposable"

    Private Disposed As Boolean

    Sub Dispose() Implements IDisposable.Dispose
        If Not Disposed Then
            Disposed = True
            MediaInfo_Close(Handle)
            MediaInfo_Delete(Handle)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Dispose()
    End Sub

#End Region

#Region "native"

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function LoadLibrary(path As String) As IntPtr
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_New() As IntPtr
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Sub MediaInfo_Delete(Handle As IntPtr)
    End Sub

    <DllImport("MediaInfo.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function MediaInfo_Open(Handle As IntPtr, FileName As String) As Integer
    End Function

    <DllImport("MediaInfo.dll")>
    Private Shared Function MediaInfo_Close(Handle As IntPtr) As Integer
    End Function

    <DllImport("MediaInfo.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function MediaInfo_Get(Handle As IntPtr,
                                          StreamKind As StreamKind,
                                          StreamNumber As Integer, Parameter As String,
                                          KindOfInfo As InfoKind,
                                          KindOfSearch As InfoKind) As IntPtr
    End Function

#End Region

End Class

Public Enum StreamKind
    General
    Video
    Audio
    Text
    Chapters
    Image
End Enum

Public Enum InfoKind
    Name
    Text
    Measure
    Options
    NameText
    MeasureText
    Info
    HowTo
End Enum