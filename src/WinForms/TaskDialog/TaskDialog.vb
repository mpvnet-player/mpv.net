
Imports System.Drawing
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Imports TaskDialog.Msg

Public Class TaskDialog(Of T)
    Inherits TaskDialogBaseForm

    Property CommandDefinitions As New List(Of CommandDefinition)
    Property ButtonDefinitions As New List(Of ButtonDefinition)
    Property SelectedValue As T
    Property SelectedText As String
    Property Title As String
    Property Timeout As Integer
    Property Content As String
    Property ContentLabel As LabelEx
    Property ExpandedContent As String
    Property ExpandedContentLabel As LabelEx

    Overloads Property Icon As TaskIcon
    Overloads Property Owner As IntPtr


    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(defaultValue As T)
        MyClass.New()
        SelectedValue = defaultValue
    End Sub

    Public Sub New(defaultText As String, defaultValue As T)
        MyClass.New(defaultValue)
        SelectedText = defaultText
    End Sub

    Sub Init()
        ShowInTaskbar = False
        Width = FontHeight * 22

        If Content = "" AndAlso Title?.Length > 80 Then
            Content = Title
            Title = ""
        End If

        If MenuButton.Items.Count > 0 Then
            MenuButton.Visible = True

            For Each i In MenuButton.Items
                Dim textWidth = TextRenderer.MeasureText(i.ToString, Font).Width + FontHeight * 3

                If MenuButton.Width < textWidth Then
                    Width += textWidth - MenuButton.Width
                End If
            Next
        End If

        If Content?.Length > 1000 OrElse ExpandedContent?.Length > 1000 Then
            Width = FontHeight * 35
        End If

        ShowIcon = False
        StartPosition = FormStartPosition.CenterScreen

        If Icon <> TaskIcon.None Then
            pbIcon.Visible = True
        End If

        Select Case Icon
            Case TaskIcon.Warning
                pbIcon.Image = StockIcon.GetImage(StockIconIdentifier.Warning)
            Case TaskIcon.Error
                pbIcon.Image = StockIcon.GetImage(StockIconIdentifier.Error)
            Case TaskIcon.Info
                pbIcon.Image = StockIcon.GetImage(StockIconIdentifier.Info)
            Case TaskIcon.Shield
                pbIcon.Image = StockIcon.GetImage(StockIconIdentifier.Shield)
            Case TaskIcon.Question
                pbIcon.Image = StockIcon.GetImage(StockIconIdentifier.Help)
        End Select

        TitleLabel.Font = New Font("Segoe UI", 11)
        TitleLabel.Text = Title

        If Content <> "" Then
            ContentLabel = New LabelEx
            ContentLabel.Margin = New Padding(0)
            ContentLabel.BorderStyle = BorderStyle.None
            ContentLabel.Text = Content
            paMain.Controls.Add(ContentLabel)
        End If

        If ExpandedContent <> "" Then
            ExpandedContentLabel = New LabelEx
            ExpandedContentLabel.Margin = New Padding(0)
            ExpandedContentLabel.BorderStyle = BorderStyle.None
            ExpandedContentLabel.Text = ExpandedContent
            ExpandedContentLabel.Name = "ExpandedInformation"
            blDetails.Visible = True
            paMain.Controls.Add(ExpandedContentLabel)
        End If

        Dim firstCommandButton As CommandButton = Nothing

        For Each cmd In CommandDefinitions
            Dim cb As New CommandButton
            cb.Title = cmd.Text
            cb.Description = cmd.Description
            cb.Tag = cmd

            If TypeOf cmd.Value Is FontFamily Then
                cb.Font = New Font(cmd.Text, Font.Size)
            End If

            AddHandler cb.Click, AddressOf CommandClick
            paMain.Controls.Add(cb)

            If firstCommandButton Is Nothing Then
                firstCommandButton = cb
            End If
        Next

        For Each i In ButtonDefinitions
            If Not flpButtons.Visible Then
                flpButtons.Visible = True
                flpButtons.AutoSize = True
            End If

            Dim b As New ButtonEx
            b.Text = i.Text
            b.Tag = i.Value

            If AcceptButton Is Nothing AndAlso i.Text = "OK" Then
                AcceptButton = b
            End If

            flpButtons.Controls.Add(b)
            i.Button = b
            AddHandler b.Click, AddressOf ButtonClick
        Next

        If Timeout > 0 Then
            Task.Run(Sub()
                         Thread.Sleep(Timeout * 1000)

                         If Not IsDisposingOrDisposed Then
                             Invoke(Sub() Close())
                         End If
                     End Sub)
        End If

        If TypeOf SelectedValue Is DialogResult Then
            If SelectedValue.Equals(DialogResult.None) Then
                If ButtonDefinitions.Where(Function(i) i.Value.Equals(DialogResult.No)).Any Then
                    SelectedValue = CType(CObj(DialogResult.No), T)
                End If

                If ButtonDefinitions.Where(Function(i) i.Value.Equals(DialogResult.Cancel)).Any Then
                    SelectedValue = CType(CObj(DialogResult.Cancel), T)
                End If
            End If

            Dim ok = ButtonDefinitions.Where(Function(i) i.Value.Equals(DialogResult.OK)).FirstOrDefault

            If Not ok Is Nothing Then
                ActiveControl = ok.Button
            End If

            If ActiveControl Is Nothing Then
                Dim yes = ButtonDefinitions.Where(Function(i) i.Value.Equals(DialogResult.Yes)).FirstOrDefault

                If Not yes Is Nothing Then
                    ActiveControl = yes.Button
                End If
            End If
        End If

        If ActiveControl Is Nothing Then
            If firstCommandButton Is Nothing Then
                ActiveControl = TitleLabel
            Else
                ActiveControl = firstCommandButton
            End If
        End If

        If Owner = IntPtr.Zero Then
            Owner = GetHandle()
        End If
    End Sub

    Sub AddCommand(value As T)
        AddCommand(value.ToString, Nothing, value)
    End Sub

    Sub AddCommand(text As String, Optional value As T = Nothing)
        AddCommand(text, Nothing, value)
    End Sub

    Sub AddCommand(text As String, description As String, value As T)
        If value Is Nothing Then
            value = CType(CObj(text), T)
        End If

        CommandDefinitions.Add(New CommandDefinition With {.Text = text, .Description = description, .Value = value})
    End Sub

    Sub AddCommands(values As IEnumerable(Of T))
        For Each i In values
            AddCommand(i)
        Next
    End Sub

    Sub AddButton(text As String)
        AddButton(text, CType(CObj(text), T))
    End Sub

    Sub AddButton(text As String, value As T)
        ButtonDefinitions.Add(New ButtonDefinition With {.Text = text, .Value = value})
    End Sub

    Sub AddButton(value As T)
        ButtonDefinitions.Add(New ButtonDefinition With {.Text = value.ToString, .Value = value})
    End Sub

    Sub AddButtons(values As IEnumerable(Of T))
        For Each i In values
            AddButton(i)
        Next
    End Sub

    WriteOnly Property Buttons As TaskButton
        Set(value As TaskButton)
            For Each i In {TaskButton.OK, TaskButton.Yes, TaskButton.No,
                TaskButton.Cancel, TaskButton.Retry, TaskButton.Close}

                If value.HasFlag(i) Then
                    AddButton(i.ToString, CType(CObj(GetDialogResultFromButton(i)), T))
                End If
            Next
        End Set
    End Property

    Sub CommandClick(sender As Object, e As EventArgs)
        Dim tag = DirectCast(sender, CommandButton).Tag
        Dim cmd = DirectCast(tag, CommandDefinition)
        SelectedText = If(cmd.Text = "", cmd.Description, cmd.Text)
        SelectedValue = cmd.Value
        Close()
    End Sub

    Sub ButtonClick(sender As Object, e As EventArgs)
        Dim button = DirectCast(sender, ButtonEx)
        SelectedText = button.Text
        SelectedValue = DirectCast(button.Tag, T)
        Close()
    End Sub

    Overrides Sub AdjustSize()
        Dim h = tlpTop.Height + tlpTop.Margin.Vertical

        If paMain.Controls.Count > 0 Then
            Dim last = paMain.Controls(paMain.Controls.Count - 1)
            h += last.Top + last.Height + last.Margin.Vertical
        End If

        h += spBottom.Height

        If spBottom.Controls.OfType(Of Control).Where(Function(i) i.Visible).Count > 0 Then
            h += spBottom.Margin.Vertical
        End If

        Dim fh = FontHeight
        h += CInt(fh * 0.7)

        Dim nonClientHeight = Height - ClientSize.Height
        Dim workingArea = Screen.FromControl(Me).WorkingArea
        Dim maxHeight = workingArea.Height
        Dim w = ClientSize.Width
        Dim secondLongestLine = GetSecondLongestLineLength()
        Dim predictedWidth = CInt(secondLongestLine * fh * 0.45)

        If predictedWidth > Width Then
            w = predictedWidth
        End If

        If w > fh * 40 Then
            w = fh * 40
        End If

        Dim ncx = Width - ClientSize.Width
        Dim ncy = Height - ClientSize.Height

        w += ncx
        h += ncy

        If h > maxHeight Then
            h = maxHeight
        End If

        Dim l = (workingArea.Width - w) \ 2
        Dim t = (workingArea.Height - h) \ 2

        Native.SetWindowPos(Handle, IntPtr.Zero, l, t, w, h, 64)
    End Sub

    Function GetSecondLongestLineLength() As Integer
        Dim list As New List(Of Integer)({51, 52})

        If Content <> "" Then
            For Each line In Content.Split(vbLf)
                list.Add(line.Length)
            Next
        End If

        If ExpandedContent <> "" AndAlso ExpandedContentLabel?.Height > 0 Then
            For Each line In ExpandedContent.Split(vbLf)
                list.Add(line.Length)
            Next
        End If

        For Each def In CommandDefinitions
            If def.Description <> "" Then
                For Each line In def.Description.Split(vbLf)
                    list.Add(line.Length)
                Next
            End If
        Next

        For Each def In CommandDefinitions
            If def.Text <> "" Then
                For Each line In def.Text.Split(vbLf)
                    list.Add(CInt(line.Length / 11 * 9))
                Next
            End If
        Next

        list.Sort()
        list.Reverse()
        Return list(1)
    End Function

    WriteOnly Property ShowCopyButton As Boolean
        Set(value As Boolean)
            If value Then
                blCopyMessage.Text = "Copy Message"
                blCopyMessage.Visible = True
                blCopyMessage.ClickAction = Sub()
                                                Clipboard.SetText(GetText)
                                                MsgInfo("Message was copied to clipboard.")
                                            End Sub
            End If
        End Set
    End Property

    Function GetHandle() As IntPtr
        Dim sb As New StringBuilder(500)
        Dim handle = Native.GetForegroundWindow
        Native.GetWindowModuleFileName(handle, sb, CUInt(sb.Capacity))

        If StringHelp.Base(sb.ToString.Replace(".vshost", "")) = StringHelp.Base(Application.ExecutablePath) Then
            Return handle
        End If
    End Function

    Function GetText() As String
        Dim ret = TitleLabel.Text

        If Content <> "" Then
            ret += BR2 + Content
        End If

        If ExpandedContent <> "" Then
            ret += BR2 + ExpandedContent
        End If

        Return ret
    End Function

    Shared Function GetDialogResultFromButton(button As TaskButton) As DialogResult
        Select Case button
            Case TaskButton.OK
                Return DialogResult.OK
            Case TaskButton.Cancel, TaskButton.Close
                Return DialogResult.Cancel
            Case TaskButton.Yes
                Return DialogResult.Yes
            Case TaskButton.No
                Return DialogResult.No
            Case TaskButton.None
                Return DialogResult.None
            Case TaskButton.Retry
                Return DialogResult.Retry
        End Select
    End Function

    Protected Overrides Sub OnLoad(args As EventArgs)
        MyBase.OnLoad(args)
        Font = New Font("Segoe UI", 9)
        Dim fh = FontHeight

        For Each i As ButtonEx In flpButtons.Controls
            i.Height = CInt(fh * 1.5)
            i.Width = fh * 5

            Using g = i.CreateGraphics
                Dim minWidth = CInt(g.MeasureString(i.Text, i.Font).Width + fh)

                If i.Width < minWidth Then
                    i.Width = minWidth
                End If
            End Using

            i.Margin = New Padding(CInt(fh * 0.4), 0, 0, 0)
        Next

        MenuButton.Margin = New Padding(CInt(fh * 0.7), MenuButton.Margin.Top, CInt(fh * 0.7), MenuButton.Margin.Bottom)
        InputTextEdit.Margin = MenuButton.Margin
        flpButtons.Margin = New Padding(0, 0, CInt(fh * 0.7), 0)

        If InputTextEdit.Visible Then
            ActiveControl = InputTextEdit
            InputTextEdit.TextBox.SelectAll()
        End If

        AdjustSize()
        AdjustSize()

        If Owner <> IntPtr.Zero Then
            Dim GWLP_HWNDPARENT = -8
            SetWindowLongPtr(Handle, GWLP_HWNDPARENT, Owner)
        End If
    End Sub

    Overloads Function Show() As T
        Init()

        If Application.MessageLoop Then
            Using Me
                ShowDialog()
            End Using
        Else
            Application.Run(Me)
        End If

        Return SelectedValue
    End Function

    Public Class CommandDefinition
        Property Text As String
        Property Description As String
        Property Value As T
    End Class

    Public Class ButtonDefinition
        Property Text As String
        Property Value As T
        Property Button As ButtonEx
    End Class
End Class

Public Enum TaskIcon
    None
    Info
    Warning
    Question
    [Error]
    Shield
End Enum

Public Enum TaskButton
    None = 0
    OK = 1
    Yes = 2
    No = 4
    Cancel = 8
    Retry = 16
    Close = 32
    OkCancel = OK Or Cancel
    YesNo = Yes Or No
    YesNoCancel = YesNo Or Cancel
    RetryCancel = Retry Or Cancel
End Enum
