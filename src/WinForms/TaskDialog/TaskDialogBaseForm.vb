
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles

Public Class TaskDialogBaseForm
    Overridable Sub AdjustSize()
    End Sub

    Public Shared Property BR As String = Environment.NewLine
    Public Shared Property BR2 As String = Environment.NewLine + Environment.NewLine

    Public Sub New()
        InitializeComponent()
        'ApplyTheme()

        AddHandler InputTextEdit.TextBox.KeyDown, AddressOf InputTextEditTextBoxKeyDown
        'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
    End Sub

    'Sub OnThemeChanged(theme As Theme)
    '    ApplyTheme(theme)
    'End Sub

    'Sub ApplyTheme()
    '    ApplyTheme(ThemeManager.CurrentTheme)
    'End Sub

    'Sub ApplyTheme(theme As Theme)
    '    If DesignHelp.IsDesignMode Then
    '        Exit Sub
    '    End If

    '    SuspendLayout()
    '    BackColor = theme.TaskDialog.BackColor
    '    ForeColor = theme.TaskDialog.ForeColor
    '    blCopyMessage.BackColor = BackColor
    '    blDetails.BackColor = BackColor
    '    ResumeLayout()
    'End Sub

    <DllImport("user32.dll", EntryPoint:="SetWindowLong")>
    Shared Function SetWindowLong32(hWnd As IntPtr, nIndex As Integer, dwNewLong As Integer) As Integer
    End Function

    <DllImport("user32.dll", EntryPoint:="SetWindowLongPtr")>
    Shared Function SetWindowLongPtr64(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
    End Function

    Shared Function SetWindowLongPtr(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
        If IntPtr.Size = 8 Then
            Return SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
        Else
            Return New IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32))
        End If
    End Function

    Class TaskDialogPanel
        Inherits PanelEx

        Property Form As TaskDialogBaseForm
        Property LineBreaks As Integer

        Protected Overrides Sub OnLayout(levent As LayoutEventArgs)
            MyBase.OnLayout(levent)

            If DesignMode OrElse Controls.Count = 0 Then
                MyBase.OnLayout(levent)
                Exit Sub
            End If

            If Form Is Nothing Then
                Form = DirectCast(FindForm(), TaskDialogBaseForm)
            End If

            Dim fh = FontHeight
            Dim previous As Control = Nothing

            Using g = CreateGraphics()
                For x = 0 To Controls.Count - 1
                    Dim c = Controls(x)

                    If x = 0 Then
                        c.Top = 0
                    Else
                        c.Top = previous.Top + previous.Height + CInt(fh * 0.2)
                    End If

                    c.Left = CInt(fh * 0.7)
                    c.Width = ClientSize.Width - CInt(fh * 0.7 * 2)

                    If TypeOf c Is LabelEx Then
                        If c.Name = "ExpandedInformation" AndAlso Form.blDetails.Text = "Show Details" Then
                            c.Visible = False
                            c.Height = 0
                        Else
                            c.Visible = True
                            Dim sz = g.MeasureString(c.Text, c.Font, c.Width)
                            c.Height = CInt(sz.Height + fh / 2)
                        End If
                    End If

                    If TryCast(c, CommandButton)?.AdjustSize() Then
                        LineBreaks += 1
                    End If

                    previous = c
                Next
            End Using
        End Sub
    End Class

    <AttributeUsage(AttributeTargets.All)>
    Public Class DispNameAttribute
        Inherits DisplayNameAttribute

        Sub New(name As String)
            DisplayNameValue = name
        End Sub

        Shared Function GetValueForEnum(value As Object) As String
            For Each i In value.GetType.GetFields
                If i.GetValue(value).Equals(value) Then
                    For Each i2 In i.GetCustomAttributes(False)
                        If i2.GetType Is GetType(DispNameAttribute) Then
                            Return DirectCast(i2, DispNameAttribute).DisplayName
                        End If
                    Next

                    Return i.Name
                End If
            Next

            Return "Unknown Type"
        End Function

        Shared Function GetNamesForEnum(Of T)() As String()
            Dim l As New List(Of String)

            For Each i As T In System.Enum.GetValues(GetType(T))
                l.Add(GetValueForEnum(i))
            Next

            Return l.ToArray
        End Function

        Shared Function GetValue(attributes As Object()) As String
            For Each i In attributes
                If TypeOf i Is DispNameAttribute Then
                    Return DirectCast(i, DispNameAttribute).DisplayName
                End If
            Next

            Return Nothing
        End Function
    End Class

    Public Class TextBoxEx
        Inherits TextBox

        Private _blockOnTextChanged As Boolean = False
        Private _borderColor As Color
        Private _borderFocusedColor As Color

        Public Property BorderColor As Color
            Get
                Return _borderColor
            End Get
            Set(value As Color)
                _borderColor = value
            End Set
        End Property

        Private Function ShouldSerializeBorderColor() As Boolean
            Return BorderColor <> Color.Empty
        End Function

        Public Property BorderFocusedColor As Color
            Get
                Return _borderFocusedColor
            End Get
            Set(value As Color)
                _borderFocusedColor = value
                'Native.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME Or RDW_IUPDATENOW Or RDW_INVALIDATE)
            End Set
        End Property

        Private Function ShouldSerializeBorderFocusedColor() As Boolean
            Return BorderFocusedColor <> Color.Empty
        End Function

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property Name() As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property TabIndex As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        Sub New()
            'ApplyTheme()
            BorderStyle = BorderStyle.None

            'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        End Sub

        'Sub OnThemeChanged(theme As Theme)
        '    ApplyTheme(theme)
        'End Sub

        'Sub ApplyTheme()
        '    ApplyTheme(ThemeManager.CurrentTheme)
        'End Sub

        'Sub ApplyTheme(theme As Theme)
        '    If DesignHelp.IsDesignMode Then
        '        Exit Sub
        '    End If

        '    SuspendLayout()
        '    BackColor = theme.General.Controls.TextBox.BackColor
        '    ForeColor = theme.General.Controls.TextBox.ForeColor
        '    BorderColor = theme.General.Controls.TextBox.BorderColor
        '    BorderFocusedColor = theme.General.Controls.TextBox.BorderFocusedColor
        '    ResumeLayout()
        'End Sub

        Public Sub SetTextWithoutTextChangedEvent(text As String)
            _blockOnTextChanged = True
            Me.Text = text
            _blockOnTextChanged = False
        End Sub

        Protected Overrides Sub OnTextChanged(e As EventArgs)
            If Not _blockOnTextChanged Then
                MyBase.OnTextChanged(e)
            End If
        End Sub
    End Class

    Public Class TextEdit
        Inherits UserControl

        Private _backReadonlyColor As Color
        Private _borderColor As Color
        Private _borderFocusedColor As Color
        Private _borderHoverColor As Color
        Private _drawBorder As Integer = -1

        Public WithEvents TextBox As New TextBoxEx

        Public Shadows Event DoubleClick(sender As Object, e As EventArgs)
        Public Shadows Event KeyDown(sender As Object, e As KeyEventArgs)
        Public Shadows Event MouseDown(sender As Object, e As MouseEventArgs)
        Public Shadows Event MouseWheel(sender As Object, e As MouseEventArgs)
        Public Shadows Event TextChanged(sender As Object, e As EventArgs)

        Public Property BackReadonlyColor As Color
            Get
                Return _backReadonlyColor
            End Get
            Set(value As Color)
                _backReadonlyColor = value
                Invalidate()
            End Set
        End Property

        Private Function ShouldSerializeBackReadonlyColor() As Boolean
            Return BackReadonlyColor <> Color.Empty
        End Function

        Public Property BorderColor As Color
            Get
                Return _borderColor
            End Get
            Set(value As Color)
                _borderColor = value
                Invalidate()
            End Set
        End Property

        Private Function ShouldSerializeBorderColor() As Boolean
            Return BorderColor <> Color.Empty
        End Function

        Public Property BorderFocusedColor As Color
            Get
                Return _borderFocusedColor
            End Get
            Set(value As Color)
                _borderFocusedColor = value
                Invalidate()
            End Set
        End Property

        Private Function ShouldSerializeBorderFocusedColor() As Boolean
            Return BorderFocusedColor <> Color.Empty
        End Function

        Public Property BorderHoverColor As Color
            Get
                Return _borderHoverColor
            End Get
            Set(value As Color)
                _borderHoverColor = value
                Invalidate()
            End Set
        End Property

        Private Function ShouldSerializeBorderHoverColor() As Boolean
            Return BorderHoverColor <> Color.Empty
        End Function

        ReadOnly Property DrawBorder As Integer
            Get
                If _drawBorder < 0 Then
                    _drawBorder = 1
                End If

                Return _drawBorder
            End Get
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property [ReadOnly] As Boolean
            Get
                Return TextBox.ReadOnly
            End Get
            Set(value As Boolean)
                TextBox.ReadOnly = value
                Invalidate()
            End Set
        End Property

        <Browsable(True)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        Overrides Property Text As String
            Get
                Return TextBox.Text
            End Get
            Set(value As String)
                TextBox.Text = value
                MyBase.Text = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property TabIndex As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        Sub New()
            SetStyle(ControlStyles.ResizeRedraw, True)
            TextBox.BorderStyle = BorderStyle.None
            Controls.Add(TextBox)

            AddHandler TextBox.DoubleClick, Sub(sender As Object, e As EventArgs) RaiseEvent DoubleClick(sender, e)
            AddHandler TextBox.KeyDown, Sub(sender As Object, e As KeyEventArgs) RaiseEvent KeyDown(sender, e)
            AddHandler TextBox.MouseDown, Sub(sender As Object, e As MouseEventArgs) RaiseEvent MouseDown(sender, e)
            AddHandler TextBox.MouseWheel, Sub(sender As Object, e As MouseEventArgs) RaiseEvent MouseWheel(sender, e)
            AddHandler TextBox.TextChanged, Sub(sender As Object, e As EventArgs) RaiseEvent TextChanged(sender, e)
            AddHandler TextBox.GotFocus, AddressOf TextBox_GotFocus
            AddHandler TextBox.LostFocus, AddressOf TextBox_LostFocus
            AddHandler TextBox.MouseEnter, AddressOf TextBox_GotFocus
            AddHandler TextBox.MouseLeave, AddressOf TextBox_LostFocus

            'ApplyTheme()

            'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        End Sub

        'Sub OnThemeChanged(theme As Theme)
        '    ApplyTheme(theme)
        'End Sub

        'Sub ApplyTheme()
        '    ApplyTheme(ThemeManager.CurrentTheme)
        'End Sub

        'Sub ApplyTheme(theme As Theme)
        '    If DesignHelp.IsDesignMode Then
        '        Exit Sub
        '    End If

        '    SuspendLayout()
        '    BackColor = theme.General.Controls.TextEdit.BackColor
        '    BackReadonlyColor = theme.General.Controls.TextEdit.BackReadonlyColor
        '    ForeColor = theme.General.Controls.TextEdit.ForeColor
        '    BorderColor = theme.General.Controls.TextEdit.BorderColor
        '    BorderFocusedColor = theme.General.Controls.TextEdit.BorderFocusedColor
        '    BorderHoverColor = theme.General.Controls.TextEdit.BorderHoverColor
        '    ResumeLayout()
        'End Sub

        Sub TextBox_GotFocus(sender As Object, e As EventArgs)
            Invalidate()
        End Sub

        Sub TextBox_LostFocus(sender As Object, e As EventArgs)
            Invalidate()
        End Sub

        Protected Overrides Sub OnLayout(args As LayoutEventArgs)
            MyBase.OnLayout(args)

            If TextBox.Multiline Then
                TextBox.Top = 2
                TextBox.Left = 2
                TextBox.Width = ClientSize.Width - 4
                TextBox.Height = ClientSize.Height - 4
            Else
                TextBox.Top = ((ClientSize.Height - TextBox.Height) \ 2)
                TextBox.Left = 2
                TextBox.Width = ClientSize.Width - 4
                Dim h = TextRenderer.MeasureText("gG", TextBox.Font).Height

                If TextBox.Height < h Then
                    TextBox.Multiline = True
                    TextBox.MinimumSize = New Size(0, h)
                    TextBox.Multiline = False
                End If
            End If
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            If Not [ReadOnly] AndAlso DrawBorder > 0 Then
                Dim borderCol = If(TextBox.Focused OrElse Not GetChildAtPoint(PointToClient(Cursor.Position)) Is Nothing, BorderFocusedColor, BorderColor)
                borderCol = If(GetChildAtPoint(PointToClient(Cursor.Position)) IsNot Nothing, BorderHoverColor, borderCol)
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, borderCol, ButtonBorderStyle.Solid)
            End If
        End Sub
    End Class

    Public Class StackPanel
        Inherits FlowLayoutPanel

        Protected Overrides Sub OnLayout(levent As LayoutEventArgs)
            MyBase.OnLayout(levent)

            If Not WrapContents AndAlso (FlowDirection = FlowDirection.TopDown OrElse FlowDirection = FlowDirection.BottomUp) Then
                For Each i As Control In Controls
                    If (i.Anchor And AnchorStyles.Right) = AnchorStyles.Right AndAlso
                        (i.Anchor And AnchorStyles.Left) = AnchorStyles.Left Then

                        i.Left = i.Margin.Left
                        i.Width = ClientSize.Width - i.Margin.Horizontal
                    ElseIf (i.Anchor And AnchorStyles.Right) = AnchorStyles.Right Then
                        i.Left = ClientSize.Width - (i.Width + i.Margin.Right)
                    End If
                Next
            End If
        End Sub
    End Class

    Public Class PanelEx
        Inherits Panel

        Private ShowNiceBorderValue As Boolean

        Sub New()
            SetStyle(ControlStyles.ResizeRedraw, True)
        End Sub

        <DefaultValue(False)>
        <Description("Nicer border if themes are enabled.")>
        Property ShowNiceBorder() As Boolean
            Get
                Return ShowNiceBorderValue
            End Get
            Set(Value As Boolean)
                ShowNiceBorderValue = Value
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            If ShowNiceBorder AndAlso VisualStyleInformation.IsEnabledByUser Then
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                    VisualStyleInformation.TextControlBorder, ButtonBorderStyle.Solid)
            End If
        End Sub
    End Class

    Public Class ButtonLabel
        Inherits Label

        Property LinkColor As Color = Color.Empty
        Property LinkHoverColor As Color = Color.Empty

        Property ClickAction As Action

        Sub New()
            ApplyTheme()
        End Sub

        Private Function ShouldSerializeLinkColor() As Boolean
            Return LinkColor <> Color.Empty
        End Function

        Private Function ShouldSerializeLinkHoverColor() As Boolean
            Return LinkHoverColor <> Color.Empty
        End Function

        Sub ApplyTheme()
            If DesignHelp.IsDesignMode Then
                Exit Sub
            End If

            BackColor = Colors.Window
            ForeColor = Colors.Link
            LinkColor = Colors.Link
            LinkHoverColor = Colors.LinkHover
        End Sub

        Protected Overrides Sub OnMouseEnter(e As EventArgs)
            ForeColor = LinkHoverColor
            MyBase.OnMouseEnter(e)
        End Sub

        Protected Overrides Sub OnMouseLeave(e As EventArgs)
            ForeColor = LinkColor
            MyBase.OnMouseLeave(e)
        End Sub

        Protected Overrides Sub OnClick(e As EventArgs)
            MyBase.OnClick(e)
            ClickAction?.Invoke
        End Sub
    End Class

    Class CommandButton
        Inherits Button

        Property Title As String
        Property Description As String

        Private _TitleFont As Font

        ReadOnly Property TitleFont As Font
            Get
                If _TitleFont Is Nothing Then
                    _TitleFont = New Font(Font.FontFamily, 11)
                End If

                Return _TitleFont
            End Get
        End Property

        Sub New()
            'If Not DesignHelp.IsDesignMode Then
            '    MinimumSize = New Size(20, 20)
            '    UseCompatibleTextRendering = False
            '    UseVisualStyleBackColor = False
            '    FlatStyle = FlatStyle.Flat
            'End If

            FlatAppearance.BorderSize = 2

            'ApplyTheme()

            'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        End Sub

        'Sub OnThemeChanged(theme As Theme)
        '    ApplyTheme(theme)
        'End Sub

        'Sub ApplyTheme()
        '    ApplyTheme(ThemeManager.CurrentTheme)
        'End Sub

        'Sub ApplyTheme(theme As Theme)
        '    If DesignHelp.IsDesignMode Then
        '        Exit Sub
        '    End If

        '    SuspendLayout()
        '    BackColor = theme.TaskDialog.CommandButton.BackColor
        '    ForeColor = theme.TaskDialog.CommandButton.ForeColor
        '    FlatAppearance.BorderColor = theme.TaskDialog.CommandButton.BorderColor
        '    ResumeLayout()
        'End Sub


        Function AdjustSize() As Boolean
            Dim titleFontHeight = TitleFont.Height
            Dim h As Integer
            Dim hasLineBreak As Boolean

            If Title <> "" AndAlso Description <> "" Then
                Dim ts = GetTitleSize()
                Dim ds = GetDescriptionSize()
                h = CInt(titleFontHeight * 0.2 * 2) + ts.Height + ds.Height

                If titleFontHeight * 2 < ts.Height Then
                    hasLineBreak = True
                End If
            ElseIf Title <> "" Then
                Dim ts = GetTitleSize()
                h = CInt(titleFontHeight * 0.2 * 2) + ts.Height

                If titleFontHeight * 2 < ts.Height Then
                    hasLineBreak = True
                End If
            ElseIf Description <> "" Then
                Dim ds = GetDescriptionSize()
                h = CInt(titleFontHeight * 0.2 * 2) + ds.Height

                If FontHeight * 2 < ds.Height Then
                    hasLineBreak = True
                End If
            End If

            ClientSize = New Size(ClientSize.Width, h)
            Return hasLineBreak
        End Function

        Function GetTitleSize(Optional g1 As Graphics = Nothing) As Size
            If Title = "" Then
                Exit Function
            End If

            Dim g2 = g1

            If g2 Is Nothing Then
                g2 = CreateGraphics()
            End If

            Dim tf = TitleFont.Height
            Dim w = ClientSize.Width - CInt(tf * 0.3 * 2)
            Dim sz = g2.MeasureString(Title, TitleFont, w)

            If g1 Is Nothing Then
                g2.Dispose()
            End If

            Return New Size(CInt(sz.Width), CInt(sz.Height))
        End Function

        Function GetDescriptionSize(Optional g1 As Graphics = Nothing) As Size
            If Description = "" Then
                Exit Function
            End If

            Dim g2 = g1

            If g2 Is Nothing Then
                g2 = CreateGraphics()
            End If

            Dim tf = TitleFont.Height
            Dim w = ClientSize.Width - CInt(tf * 0.3 * 2)
            Dim sz = g2.MeasureString(Description, Font, w)

            If g1 Is Nothing Then
                g2.Dispose()
            End If

            Return New Size(CInt(sz.Width), CInt(sz.Height))
        End Function

        Protected Overrides Sub OnGotFocus(e As EventArgs)
            MyBase.OnGotFocus(e)
        End Sub

        Protected Overrides Sub OnLostFocus(e As EventArgs)
            MyBase.OnLostFocus(e)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            Dim g = e.Graphics
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            Dim titleFontHeight = TitleFont.Height

            Dim x = CInt(titleFontHeight * 0.3)
            Dim y = CInt(titleFontHeight * 0.2)
            Dim w = ClientSize.Width - x * 2
            Dim h = ClientSize.Height - CInt(titleFontHeight * 0.2 * 2)
            Dim r = New Rectangle(x, y, w, h)

            If Title <> "" AndAlso Description <> "" Then
                TextRenderer.DrawText(g, Title, TitleFont, r, ForeColor, TextFormatFlags.Left Or TextFormatFlags.WordBreak)
                y = CInt(titleFontHeight * 0.2) + GetTitleSize().Height
                r = New Rectangle(x, y, w, h)
                TextRenderer.DrawText(g, Description, Font, r, ForeColor, TextFormatFlags.Left Or TextFormatFlags.WordBreak)
            ElseIf Title <> "" Then
                TextRenderer.DrawText(g, Title, TitleFont, r, ForeColor, TextFormatFlags.Left Or TextFormatFlags.WordBreak)
            ElseIf Description <> "" Then
                TextRenderer.DrawText(g, Description, Font, r, ForeColor, TextFormatFlags.Left Or TextFormatFlags.WordBreak)
            End If
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            'RemoveHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
            TitleFont.Dispose()
            MyBase.Dispose(disposing)
        End Sub
    End Class

    Public Class ButtonEx
        Inherits Button

        Private _backDisabledColor As Color
        Private _foreDisabledColor As Color
        Private _text As String = ""

        <DefaultValue(False)>
        Property ShowMenuSymbol As Boolean

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property ClickAction As Action

        Public Property BackDisabledColor As Color
            Get
                Return _backDisabledColor
            End Get
            Set(value As Color)
                _backDisabledColor = value
            End Set
        End Property

        Private Function ShouldSerializeBackDisabledColor() As Boolean
            Return BackDisabledColor <> Color.Empty
        End Function

        Public Property ForeDisabledColor As Color
            Get
                Return _foreDisabledColor
            End Get
            Set(value As Color)
                _foreDisabledColor = value
            End Set
        End Property

        Private Function ShouldSerializeForeDisabledColor() As Boolean
            Return ForeDisabledColor <> Color.Empty
        End Function

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property Name As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <DefaultValue(0), Browsable(False),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property TabIndex As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        Protected Overrides ReadOnly Property ShowFocusCues As Boolean
            Get
                Return True
            End Get
        End Property

        'The designer is not able to serialize the Text property probably
        'because it shadows the base Text property, Text2 is used as workaround
        Property Text2 As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value
            End Set
        End Property

        Shadows Property Text As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value

                If AutoSize Then
                    Dim textSize = TextRenderer.MeasureText(_text, Font)
                    Dim fh = Font.Height
                    MinimumSize = New Size(CInt(fh * 2.2), fh)
                    AutoSizeMode = AutoSizeMode.GrowOnly
                    Size = New Size(textSize.Width + Padding.Horizontal + fh, textSize.Height + CInt(fh * 0.45))
                End If

                Invalidate(True)
            End Set
        End Property

        Private Function ShouldSerializeUseVisualStyleBackColor() As Boolean
            Return False
        End Function

        Protected Overrides ReadOnly Property DefaultSize As Size
            Get
                Return New Size(250, 70)
            End Get
        End Property

        Sub New()
            SetStyle(ControlStyles.ResizeRedraw, True)

            Padding = New Padding(0)

            If Not DesignHelp.IsDesignMode Then
                MinimumSize = New Size(20, 20)
                UseCompatibleTextRendering = True
                UseVisualStyleBackColor = False
                FlatStyle = FlatStyle.Flat
            End If

            FlatAppearance.BorderSize = 2

            'ApplyTheme()

            'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        End Sub

        'Sub OnThemeChanged(theme As Theme)
        '    ApplyTheme(theme)
        'End Sub

        'Sub ApplyTheme()
        '    ApplyTheme(ThemeManager.CurrentTheme)
        'End Sub

        'Sub ApplyTheme(theme As Theme)
        '    If DesignHelp.IsDesignMode Then
        '        Exit Sub
        '    End If

        '    SuspendLayout()
        '    BackColor = theme.General.Controls.Button.BackColor
        '    BackDisabledColor = theme.General.Controls.Button.BackDisabledColor
        '    ForeColor = theme.General.Controls.Button.ForeColor
        '    ForeDisabledColor = theme.General.Controls.Button.ForeDisabledColor
        '    FlatAppearance.BorderColor = theme.General.Controls.Button.BorderColor
        '    ResumeLayout()
        'End Sub

        Protected Overrides Sub OnClick(e As EventArgs)
            ClickAction?.Invoke()
            ContextMenuStrip?.Show(Me, 0, Height)
            MyBase.OnClick(e)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

            Dim fore = If(Enabled, ForeColor, ForeDisabledColor)
            Dim flags As TextFormatFlags

            Select Case TextAlign
                Case Drawing.ContentAlignment.BottomCenter
                    flags = TextFormatFlags.Bottom Or TextFormatFlags.HorizontalCenter
                Case Drawing.ContentAlignment.BottomLeft
                    flags = TextFormatFlags.Bottom Or TextFormatFlags.Left
                Case Drawing.ContentAlignment.BottomRight
                    flags = TextFormatFlags.Bottom Or TextFormatFlags.Right
                Case Drawing.ContentAlignment.MiddleCenter
                    flags = TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter
                Case Drawing.ContentAlignment.MiddleLeft
                    flags = TextFormatFlags.VerticalCenter Or TextFormatFlags.Left
                Case Drawing.ContentAlignment.MiddleRight
                    flags = TextFormatFlags.VerticalCenter Or TextFormatFlags.Right
                Case Drawing.ContentAlignment.TopCenter
                    flags = TextFormatFlags.Top Or TextFormatFlags.HorizontalCenter
                Case Drawing.ContentAlignment.TopLeft
                    flags = TextFormatFlags.Top Or TextFormatFlags.Left
                Case Drawing.ContentAlignment.TopRight
                    flags = TextFormatFlags.Top Or TextFormatFlags.Right
            End Select

            Using foreBrush = New SolidBrush(fore)
                Dim rect = ClientRectangle
                rect.Offset(0, Padding.Top - If(FlatStyle.HasFlag(FlatStyle.Flat), FlatAppearance.BorderSize \ 2, 0))

                TextRenderer.DrawText(e.Graphics, Text, Font, rect, fore, flags)
            End Using

            If ShowMenuSymbol Then
                Dim h = CInt(Font.Height * 0.3)
                Dim w = h * 2

                Dim x1 = If(Text = "", Width \ 2 - w \ 2, Width - w - CInt(w * 0.7))
                Dim y1 = CInt(Height / 2 - h / 2)

                Dim x2 = CInt(x1 + w / 2)
                Dim y2 = y1 + h

                Dim x3 = x1 + w
                Dim y3 = y1

                Using pen = New Pen(fore, Font.Height / 16.0F)
                    e.Graphics.DrawLine(pen, x1, y1, x2, y2)
                    e.Graphics.DrawLine(pen, x2, y2, x3, y3)
                End Using
            End If
        End Sub
    End Class

    Public Class CheckBoxEx
        Inherits CheckBox

        'Private _theme As Theme

        'Public ReadOnly Property Theme As Theme
        '    Get
        '        Return _theme
        '    End Get
        'End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property Name() As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property TabIndex As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property

        Sub New()
            SetStyle(ControlStyles.ResizeRedraw, True)
            SetStyle(ControlStyles.UserPaint, True)
            UseVisualStyleBackColor = False
            FlatStyle = FlatStyle.Standard
            FlatAppearance.BorderSize = 2

            'ApplyTheme()
            'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        End Sub

        'Sub OnThemeChanged(theme As Theme)
        '    ApplyTheme(theme)
        'End Sub

        'Sub ApplyTheme()
        '    ApplyTheme(ThemeManager.CurrentTheme)
        'End Sub

        'Sub ApplyTheme(theme As Theme)
        '    _theme = theme

        '    If DesignHelp.IsDesignMode Then
        '        Exit Sub
        '    End If

        '    SuspendLayout()
        '    If Appearance.HasFlag(Appearance.Normal) Then
        '        FlatStyle = FlatStyle.Standard
        '        FlatAppearance.BorderSize = 0

        '        BackColor = theme.General.Controls.CheckBox.BackColor
        '        ForeColor = theme.General.Controls.CheckBox.ForeColor
        '        FlatAppearance.BorderColor = theme.General.Controls.CheckBox.BorderColor
        '        FlatAppearance.CheckedBackColor = theme.General.Controls.CheckBox.BackColor
        '        FlatAppearance.MouseOverBackColor = theme.General.Controls.CheckBox.BackHighlightColor
        '    Else
        '        FlatStyle = FlatStyle.Flat
        '        FlatAppearance.BorderSize = 0

        '        BackColor = theme.General.Controls.CheckBox.BackColor
        '        ForeColor = theme.General.Controls.Button.ForeColor
        '        FlatAppearance.BorderColor = theme.General.Controls.Button.BorderColor
        '        FlatAppearance.CheckedBackColor = theme.General.Controls.Button.BackColor
        '        FlatAppearance.MouseOverBackColor = theme.General.Controls.CheckBox.BackHoverColor
        '    End If
        '    ResumeLayout()
        'End Sub

        'Protected Overrides Sub OnPaint(pevent As PaintEventArgs)
        '    InvokePaintBackground(Me, pevent)

        '    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        '    Dim state As CheckBoxState = If(CheckState = CheckState.Checked, CheckBoxState.CheckedNormal, CheckBoxState.UncheckedNormal)
        '    Dim glyphSize As Size = CheckBoxRendererEx.GetGlyphSize(pevent.Graphics, state)
        '    Dim vPos As Integer = (Height - glyphSize.Height) \ 2
        '    Dim hPos As Integer = 1
        '    Dim glyphLocation As Point = New Point(hPos, vPos)
        '    Dim textLocation As Point = New Point(hPos + glyphSize.Width + hPos, 1 + Height - (Height - CInt(pevent.Graphics.MeasureString(Text, Font).Height)) \ 3)
        '    Dim textFlags As TextFormatFlags = TextFormatFlags.SingleLine Or TextFormatFlags.VerticalCenter
        '    Dim fColor As ColorHSL = If(Checked, _theme.General.Controls.CheckBox.ForeCheckedColor, _theme.General.Controls.CheckBox.ForeColor)

        '    CheckBoxRendererEx.DrawCheckBox(pevent.Graphics, glyphLocation, state)
        '    TextRenderer.DrawText(pevent.Graphics, Text, Font, textLocation, fColor, textFlags)
        'End Sub

        Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
            MyBase.OnPaintBackground(pevent)
        End Sub

        Protected Overrides Sub OnCheckedChanged(e As EventArgs)
            MyBase.OnCheckedChanged(e)
        End Sub

        Private Function ShouldSerializeUseVisualStyleBackColor() As Boolean
            Return False
        End Function
    End Class

    Public Class MenuButtonEx
        Inherits ButtonEx

        Event ValueChangedUser(value As Object)

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Property Items As New List(Of Object)
        Property Menu As New ContextMenuStripEx
        Property ShowPath As Boolean

        Sub New()
            Menu.ShowImageMargin = False
            ShowMenuSymbol = True
            Padding = New Padding(4, 0, 0, 0)
            AddHandler Menu.Opening, AddressOf MenuOpening
        End Sub

        Sub MenuOpening(sender As Object, e As CancelEventArgs)
            Menu.MinimumSize = New Size(Width, 0)
            Dim minItemSize As Integer = Menu.MinimumSize.Width

            For Each mi As MenuItemEx In Menu.Items
                mi.Font = New Font("Segoe UI", 9, If(Value IsNot Nothing AndAlso Value.Equals(mi.Tag), FontStyle.Bold, FontStyle.Regular))
                mi.AutoSize = True
                minItemSize = Math.Max(minItemSize, mi.Width)
            Next

            For Each mi As MenuItemEx In Menu.Items
                mi.AutoSize = False
                mi.Width = minItemSize
            Next
        End Sub

        Private ValueValue As Object

        <DefaultValue(CStr(Nothing))>
        Property Value As Object
            Get
                Return ValueValue
            End Get
            Set(value As Object)
                If Menu.Items.Count = 0 Then
                    If TypeOf value Is System.Enum Then
                        For Each i In System.Enum.GetValues(value.GetType)
                            Dim text = DispNameAttribute.GetValueForEnum(i)
                            Dim temp = i

                            MenuItemEx.Add(Menu.Items, text, Sub(o As Object) OnAction(text, o), temp, Nothing).Tag = temp
                        Next
                    End If
                End If

                If Not value Is Nothing Then
                    For Each i In Menu.Items.OfType(Of MenuItemEx)()
                        If value.Equals(i.Tag) Then Text = i.Text

                        If i.DropDownItems.Count > 0 Then
                            For Each i2 In i.DropDownItems.OfType(Of MenuItemEx)()
                                If value.Equals(i2.Tag) Then
                                    If ShowPath Then
                                        Text = i2.Path
                                    Else
                                        Text = i2.Text
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If

                If Text = "" AndAlso Not value Is Nothing Then
                    Text = value.ToString
                End If

                ValueValue = value
            End Set
        End Property

        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            If e.Button = MouseButtons.Left Then
                Menu.Show(Me, 0, Height)
            Else
                MyBase.OnMouseDown(e)
            End If
        End Sub

        Protected Overridable Sub OnValueChanged(value As Object)
            RaiseEvent ValueChangedUser(value)
        End Sub

        Sub AddRange(items As IEnumerable(Of Object))
            For Each i In items
                Add(i.ToString, i, Nothing)
            Next
        End Sub

        Function Add(path As String, obj As Object, Optional tip As String = Nothing) As MenuItemEx
            Items.Add(obj)
            Dim name = path

            If path.Contains("|") Then
                name = StringHelp.RightLast(path, "|").Trim
            End If

            Dim ret = MenuItemEx.Add(Menu.Items, path, Sub(o As Object) OnAction(name, o), obj, tip)
            ret.Tag = obj
            ret.Path = path
            Return ret
        End Function

        Sub Clear()
            Items.Clear()
            ClearAndDisposeToolStripItems(Menu.Items)
        End Sub

        Shared Sub ClearAndDisposeToolStripItems(instance As ToolStripItemCollection)
            For Each i In instance.OfType(Of IDisposable).ToArray
                i.Dispose()
            Next

            instance.Clear()
        End Sub

        Sub OnAction(text As String, value As Object)
            Me.Text = text
            Me.Value = value
            OnValueChanged(value)
        End Sub

        Function GetValue(Of T)() As T
            Return DirectCast(Value, T)
        End Function

        Function GetInt() As Integer
            Return DirectCast(Value, Integer)
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Menu.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Public Class ContextMenuStripEx
            Inherits ContextMenuStrip

            Private FormValue As Form

            Sub New()
                MyBase.New()
                'ApplyTheme()
                'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
            End Sub

            Sub New(container As IContainer)
                MyBase.New(container)
                'ApplyTheme()
                'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
            End Sub

            'Sub OnThemeChanged(theme As Theme)
            '    ApplyTheme(theme)
            'End Sub

            'Sub ApplyTheme()
            '    ApplyTheme(ThemeManager.CurrentTheme)
            'End Sub

            'Sub ApplyTheme(theme As Theme)
            '    If DesignHelp.IsDesignMode Then
            '        Exit Sub
            '    End If

            '    SuspendLayout()
            '    BackColor = theme.General.Controls.ToolStrip.DropdownBackgroundDefaultColor
            '    ResumeLayout()
            'End Sub

            Protected Overrides Sub OnHandleCreated(e As EventArgs)
                MyBase.OnHandleCreated(e)
                'g.SetRenderer(Me)
                Font = New Font("Segoe UI", 9)
            End Sub

            <DefaultValue(GetType(Form), Nothing)>
            Property Form As Form
                Get
                    Return FormValue
                End Get
                Set(value As Form)
                    AddHandler value.Disposed, Sub() Dispose()
                    FormValue = value
                    value.KeyPreview = True
                End Set
            End Property

            Function Add(path As String) As MenuItemEx
                Return Add(path, Nothing)
            End Function

            Function Add(path As String, action As Action) As MenuItemEx
                Return Add(path, action, True)
            End Function

            Function Add(path As String, action As Action, key As Keys) As MenuItemEx
                Return Add(path, action, key, True, Nothing, Nothing)
            End Function

            Function Add(path As String, action As Action, key As Keys, symbol As Symbol) As MenuItemEx
                Return Add(path, action, key, True, Nothing, Nothing, symbol)
            End Function

            Function Add(path As String, action As Action, key As Keys, enabled As Boolean) As MenuItemEx
                Return Add(path, action, key, enabled, Nothing, Nothing)
            End Function

            Function Add(path As String, action As Action, help As String) As MenuItemEx
                Return Add(path, action, True, help)
            End Function

            Function Add(path As String, action As Action, enabled As Boolean) As MenuItemEx
                Return Add(path, action, enabled, Nothing)
            End Function

            Function Add(path As String, action As Action, key As Keys, help As String) As MenuItemEx
                Return Add(path, action, key, True, Nothing, help)
            End Function

            Function Add(path As String, action As Action, key As Keys, enabledFunc As Func(Of Boolean)) As MenuItemEx
                Return Add(path, action, key, True, enabledFunc)
            End Function

            Function Add(path As String, action As Action, key As Keys, enabledFunc As Func(Of Boolean), help As String) As MenuItemEx
                Return Add(path, action, key, True, enabledFunc, help)
            End Function

            Function Add(path As String, action As Action, enabled As Boolean, help As String) As MenuItemEx
                Return Add(path, action, Keys.None, enabled, Nothing, help)
            End Function

            Function Add(path As String, action As Action, enabledFunc As Func(Of Boolean), help As String) As MenuItemEx
                Return Add(path, action, Keys.None, True, enabledFunc, help)
            End Function

            Function Add(
                path As String,
                action As Action,
                key As Keys,
                enabled As Boolean,
                enabledFunc As Func(Of Boolean),
                Optional help As String = Nothing,
                Optional symbol As Symbol = Symbol.None) As MenuItemEx

                Dim ret = MenuItemEx.Add(Items, path, action)

                If ret Is Nothing Then
                    Exit Function
                End If

                ret.Form = Form
                ret.Enabled = enabled
                ret.EnabledFunc = enabledFunc

                AddHandler Opening, AddressOf ret.Opening

                Return ret
            End Function

            ''I wasn't able to find out why it's only needed in some menus 
            'Sub ApplyMarginFix()
            '    For Each i In GetItems.OfType(Of MenuItemEx)
            '        i.ShortcutKeyDisplayString += " ".Multiply(CInt(g.DPI / 96))
            '    Next
            'End Sub

            Function GetItems() As List(Of ToolStripItem)
                Dim ret As New List(Of ToolStripItem)
                AddItemsRecursive(Items, ret)
                Return ret
            End Function

            Shared Sub AddItemsRecursive(searchList As ToolStripItemCollection, returnList As List(Of ToolStripItem))
                For Each i As ToolStripItem In searchList
                    returnList.Add(i)

                    If TypeOf i Is ToolStripDropDownItem Then
                        AddItemsRecursive(DirectCast(i, ToolStripDropDownItem).DropDownItems, returnList)
                    End If
                Next
            End Sub
        End Class

        Public Class MenuItemEx
            Inherits ToolStripMenuItem

            Shared Property UseTooltips As Boolean

            Private Action As Action

            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Property Form As Form

            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Property EnabledFunc As Func(Of Boolean)

            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Property VisibleFunc As Func(Of Boolean)

            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Property Path As String

            Sub New()
                MyBase.New()
            End Sub

            Sub New(text As String)
                MyBase.New(text)
            End Sub

            Sub New(text As String, a As Action)
                Me.New(text, a, Nothing)
            End Sub

            Sub New(text As String,
                    action As Action,
                    Optional tooltip As String = Nothing,
                    Optional enabled As Boolean = True)

                Me.Text = text
                Me.Action = action
                Me.Enabled = enabled
            End Sub

            Sub Opening(sender As Object, e As CancelEventArgs)
                If Not EnabledFunc Is Nothing Then
                    Enabled = EnabledFunc.Invoke
                End If

                If Not VisibleFunc Is Nothing Then
                    Visible = VisibleFunc.Invoke
                End If
            End Sub

            Shared Function Add(Of T)(
                items As ToolStripItemCollection,
                path As String,
                action As Action(Of T),
                value As T,
                Optional help As String = Nothing) As MenuItemEx

                Return Add(items, path, Sub() action(value), help)
            End Function

            Shared Function Add(items As ToolStripItemCollection, path As String) As MenuItemEx
                Return Add(items, path, Nothing)
            End Function

            Shared Function Add(
                items As ToolStripItemCollection, path As String, action As Action) As MenuItemEx

                Return Add(items, path, action, Symbol.None, Nothing)
            End Function

            Shared Function Add(
                items As ToolStripItemCollection,
                path As String,
                action As Action,
                tip As String) As MenuItemEx

                Return Add(items, path, action, Symbol.None, tip)
            End Function

            Shared Function Add(
                items As ToolStripItemCollection,
                path As String,
                action As Action,
                symbol As Symbol,
                Optional tip As String = Nothing) As MenuItemEx

                Dim a = path.Split({" | "}, StringSplitOptions.RemoveEmptyEntries)
                Dim l = items
                Dim p = ""

                For x = 0 To a.Length - 1
                    p += If(x = 0, "", " | ") + a(x)
                    Dim found = False

                    For Each i In l.OfType(Of ToolStripMenuItem)()
                        If x < a.Length - 1 Then
                            If i.Text = a(x) Then
                                found = True
                                l = i.DropDownItems
                            End If
                        End If
                    Next

                    If Not found Then
                        If x = a.Length - 1 Then
                            If a(x) = "-" Then
                                l.Add(New ToolStripSeparator)
                            Else
                                Dim item As New MenuItemEx(a(x), action, tip)
                                item.Path = p
                                l.Add(item)
                                l = item.DropDownItems
                                Return item
                            End If
                        Else
                            Dim item As New MenuItemEx()
                            item.Text = a(x)
                            item.Path = p
                            l.Add(item)
                            l = item.DropDownItems
                        End If
                    End If
                Next
            End Function

            Overrides Function GetPreferredSize(constrainingSize As Size) As Size
                Dim ret = MyBase.GetPreferredSize(constrainingSize)
                ret.Height = CInt(Font.Height * 1.4)
                Return ret
            End Function

            Protected Overrides Sub Dispose(disposing As Boolean)
                MyBase.Dispose(disposing)
                Action = Nothing
                EnabledFunc = Nothing
                VisibleFunc = Nothing
                Form = Nothing
            End Sub

            Sub CloseAll(item As Object)
                If TypeOf item Is ToolStripItem Then
                    Dim d = DirectCast(item, ToolStripItem)
                    CloseAll(d.Owner)
                End If

                If TypeOf item Is ToolStripDropDown Then
                    Dim d = DirectCast(item, ToolStripDropDown)
                    d.Close()
                    CloseAll(d.OwnerItem)
                End If
            End Sub

            Protected Overrides Sub OnClick(e As EventArgs)
                Application.DoEvents()

                If Not Action Is Nothing Then
                    Action()
                End If

                MyBase.OnClick(e)
            End Sub
        End Class
    End Class

    Public Class LabelEx
        Inherits Label

        Sub New()
            TextAlign = Drawing.ContentAlignment.MiddleLeft
            'ApplyTheme()

            'AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        End Sub

        'Sub OnThemeChanged(theme As Theme)
        '    ApplyTheme(theme)
        'End Sub

        'Sub ApplyTheme()
        '    ApplyTheme(ThemeManager.CurrentTheme)
        'End Sub

        'Sub ApplyTheme(theme As Theme)
        '    If DesignHelp.IsDesignMode Then
        '        Exit Sub
        '    End If

        '    SuspendLayout()
        '    BackColor = theme.General.Controls.Label.BackColor
        '    ForeColor = theme.General.Controls.Label.ForeColor
        '    ResumeLayout()
        'End Sub

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property Name() As String
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Shadows Property TabIndex As Integer
            Get
                Return MyBase.TabIndex
            End Get
            Set(value As Integer)
                MyBase.TabIndex = value
            End Set
        End Property
    End Class

    Public Class DesignHelp
        Private Shared IsDesignModeValue As Boolean?

        Shared ReadOnly Property IsDesignMode As Boolean
            Get
                If Not IsDesignModeValue.HasValue Then
                    IsDesignModeValue = Process.GetCurrentProcess.ProcessName = "devenv"
                End If

                Return IsDesignModeValue.Value
            End Get
        End Property
    End Class

    Public Class Colors
        Shared Property Link As Color = Color.RoyalBlue
        Shared Property LinkHover As Color = Color.CornflowerBlue
        Shared Property Window As Color = SystemColors.Control
    End Class

    Sub blDetails_Click(sender As Object, e As EventArgs) Handles blDetails.Click
        paMain.ScrollControlIntoView(paMain.Controls(0))

        If blDetails.Text = "Show Details" Then
            blDetails.Text = "Hide Details"
        Else
            Width = FontHeight * 22
            blDetails.Text = "Show Details"
        End If

        paMain.PerformLayout()
        AdjustSize()
        AdjustSize()
    End Sub

    Sub InputTextEditTextBoxKeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyData = Keys.Enter AndAlso Not AcceptButton Is Nothing Then
            AcceptButton.PerformClick()
        End If
    End Sub

    ReadOnly Property IsDisposingOrDisposed As Boolean
        Get
            Try
                Return Disposing OrElse IsDisposed
            Catch
                Return True
            End Try
        End Get
    End Property
End Class
