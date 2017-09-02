Namespace UI
    Public Module MainModule
        Public ReadOnly BR As String = Environment.NewLine
        Public ReadOnly BR2 As String = Environment.NewLine + Environment.NewLine

        Sub MsgInfo(text As String, Optional content As String = Nothing)
            Msg(text, content, MsgIcon.Info, TaskDialogButtons.Ok)
        End Sub

        Sub MsgError(text As String, Optional content As String = Nothing)
            If text = "" Then text = content
            If text = "" Then Exit Sub

            Using td As New TaskDialog(Of String)
                td.AllowCancel = False

                If content = "" Then
                    If text.Length < 80 Then
                        td.MainInstruction = text
                    Else
                        td.Content = text
                    End If
                Else
                    td.MainInstruction = text
                    td.Content = content
                End If

                td.MainIcon = TaskDialogIcon.Error
                td.Footer = "[copymsg: Copy Message]"
                td.Show()
            End Using
        End Sub

        Sub MsgWarn(text As String, Optional content As String = Nothing)
            Msg(text, content, MsgIcon.Warning, TaskDialogButtons.Ok)
        End Sub

        Function MsgOK(text As String) As Boolean
            Return Msg(text, Nothing, MsgIcon.Question, TaskDialogButtons.OkCancel) = DialogResult.OK
        End Function

        Function MsgQuestion(text As String,
                             Optional buttons As TaskDialogButtons = TaskDialogButtons.OkCancel) As DialogResult
            Return Msg(text, Nothing, MsgIcon.Question, buttons)
        End Function

        Function MsgQuestion(heading As String,
                             content As String,
                             Optional buttons As TaskDialogButtons = TaskDialogButtons.OkCancel) As DialogResult
            Return Msg(heading, content, MsgIcon.Question, buttons)
        End Function

        Function Msg(mainInstruction As String,
                     content As String,
                     icon As MsgIcon,
                     buttons As TaskDialogButtons,
                     Optional defaultButton As DialogResult = DialogResult.None) As DialogResult

            Try
                If mainInstruction Is Nothing Then mainInstruction = ""

                Using td As New TaskDialog(Of DialogResult)
                    td.AllowCancel = False
                    td.DefaultButton = defaultButton

                    If content Is Nothing Then
                        If mainInstruction.Length < 80 Then
                            td.MainInstruction = mainInstruction
                        Else
                            td.Content = mainInstruction
                        End If
                    Else
                        td.MainInstruction = mainInstruction
                        td.Content = content
                    End If

                    Select Case icon
                        Case MsgIcon.Error
                            td.MainIcon = TaskDialogIcon.Error
                        Case MsgIcon.Warning
                            td.MainIcon = TaskDialogIcon.Warning
                        Case MsgIcon.Info
                            td.MainIcon = TaskDialogIcon.Info
                    End Select

                    If buttons = TaskDialogButtons.OkCancel Then
                        td.AddButton("OK", DialogResult.OK)
                        td.AddButton("Cancel", DialogResult.Cancel) 'don't use system language
                    Else
                        td.CommonButtons = buttons
                    End If

                    Return td.Show()
                End Using
            Catch ex As Exception
                MsgBox(mainInstruction + content, MessageBoxIcon.Error)
            End Try
        End Function

        Sub MsgException(e As Exception, Optional msg As String = Nothing, Optional timeout As Integer = 0)
            Try
                Using td As New TaskDialog(Of String)
                    If msg = "" Then
                        td.MainInstruction = e.GetType.Name + $" ({Application.ProductVersion})"
                    Else
                        td.MainInstruction = msg
                    End If

                    td.Timeout = timeout
                    td.Content = e.Message
                    td.MainIcon = TaskDialogIcon.Error
                    td.ExpandedInformation = e.ToString
                    td.Footer = "[copymsg: Copy Message]"
                    td.Show()
                End Using
            Catch
                MsgBox(e.GetType.Name + BR2 + e.Message + BR2 + e.ToString, MessageBoxIcon.Error)
            End Try
        End Sub

        Sub MsgBox(text As String, icon As MessageBoxIcon)
            MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, icon)
        End Sub
    End Module

    Public Enum MsgIcon
        None = MessageBoxIcon.None
        Info = MessageBoxIcon.Information
        [Error] = MessageBoxIcon.Error
        Warning = MessageBoxIcon.Warning
        Question = MessageBoxIcon.Question
    End Enum
End Namespace