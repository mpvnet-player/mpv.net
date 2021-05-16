
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Class StringHelp
    Shared Function Base(instance As String) As String
        If instance = "" Then
            Return ""
        End If

        Dim ret = instance

        If ret.Contains(Path.DirectorySeparatorChar) Then
            ret = RightLast(ret, Path.DirectorySeparatorChar)
        End If

        If ret.Contains(".") Then
            ret = LeftLast(ret, ".")
        End If

        Return ret
    End Function

    Shared Function RightLast(value As String, start As String) As String
        If value = "" OrElse start = "" Then
            Return ""
        End If

        If Not value.Contains(start) Then
            Return ""
        End If

        Return value.Substring(value.LastIndexOf(start) + start.Length)
    End Function

    Shared Function LeftLast(value As String, start As String) As String
        If Not value.Contains(start) Then
            Return ""
        End If

        Return value.Substring(0, value.LastIndexOf(start))
    End Function
End Class

Public Class Native
    <DllImport("user32.dll")>
    Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Shared Function GetWindowModuleFileName(
        hwnd As IntPtr,
        lpszFileName As StringBuilder,
        cchFileNameMax As UInteger) As UInteger
    End Function

    <DllImport("user32.dll")>
    Shared Function SetWindowPos(
        hWnd As IntPtr,
        hWndInsertAfter As IntPtr,
        X As Integer,
        Y As Integer,
        cx As Integer,
        cy As Integer,
        uFlags As UInteger) As Boolean
    End Function
End Class