
Imports System.Windows.Forms

Public Class Msg
    Shared Sub MsgInfo(title As Object, Optional content As Object = Nothing)
        Dim title1 = title?.ToString
        Dim content1 = content?.ToString
        Msg(title1, content1, TaskIcon.Info, TaskButton.OK)
    End Sub

    Shared Sub MsgError(title As String, Optional content As String = Nothing)
        MsgError(title, content, IntPtr.Zero)
    End Sub

    Shared Sub MsgError(title As String, content As String, handle As IntPtr)
        If title = "" Then
            title = content
        End If

        If title = "" Then
            Exit Sub
        End If

        Using td As New TaskDialog(Of String)
            td.Title = title
            td.Content = content
            td.Owner = handle
            td.Icon = TaskIcon.Error
            td.ShowCopyButton = True
            td.AddButton("OK")
            td.Show()
        End Using
    End Sub

    Private Shared ShownMessages As String

    Shared Sub MsgWarn(text As String, Optional content As String = Nothing, Optional onlyOnce As Boolean = False)
        If onlyOnce AndAlso ShownMessages?.Contains(text + content) Then
            Exit Sub
        End If

        Msg(text, content, TaskIcon.Warning, TaskButton.OK)

        If onlyOnce Then
            ShownMessages += text + content
        End If
    End Sub

    Shared Function MsgOK(title As String) As Boolean
        Return Msg(title, Nothing, TaskIcon.Question, TaskButton.OkCancel) = DialogResult.OK
    End Function

    Shared Function MsgQuestion(
        title As String,
        Optional buttons As TaskButton = TaskButton.OkCancel) As DialogResult

        Return Msg(title, Nothing, TaskIcon.Question, buttons)
    End Function

    Shared Function MsgQuestion(
        title As String,
        content As String,
        Optional buttons As TaskButton = TaskButton.OkCancel) As DialogResult

        Return Msg(title, content, TaskIcon.Question, buttons)
    End Function

    Shared Function Msg(
        title As String,
        content As String,
        icon As TaskIcon,
        buttons As TaskButton) As DialogResult

        Using td As New TaskDialog(Of DialogResult)
            td.Icon = icon
            td.Title = title
            td.Content = content
            td.Buttons = buttons
            Return td.Show()
        End Using
    End Function
End Class
