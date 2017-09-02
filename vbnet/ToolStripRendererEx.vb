Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports Microsoft.Win32

Namespace UI
    Public Class ToolStripRendererEx
        Inherits ToolStripSystemRenderer

        Shared RenderMode As ToolStripRenderModeEx

        Shared Property ColorChecked As Color
        Shared Property ColorBorder As Color
        Shared Property ColorTop As Color
        Shared Property ColorBottom As Color
        Shared Property ColorBackground As Color

        Shared Property ColorToolStrip1 As Color
        Shared Property ColorToolStrip2 As Color
        Shared Property ColorToolStrip3 As Color
        Shared Property ColorToolStrip4 As Color

        Private TextOffset As Integer

        Sub New(mode As ToolStripRenderModeEx)
            RenderMode = mode
            InitColors(mode)
        End Sub

        Shared Function IsAutoRenderMode() As Boolean
            Return _
                RenderMode = ToolStripRenderModeEx.SystemAuto OrElse
                RenderMode = ToolStripRenderModeEx.Win7Auto OrElse
                RenderMode = ToolStripRenderModeEx.Win10Auto
        End Function

        Shared Sub InitColors(renderMode As ToolStripRenderModeEx)
            If ToolStripRendererEx.IsAutoRenderMode Then
                Dim argb = CInt(Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", 0))
                If argb = 0 Then argb = Color.LightBlue.ToArgb
                InitColors(Color.FromArgb(argb))
            Else
                ColorBorder = Color.FromArgb(&HFF83ABDC)
                ColorTop = Color.FromArgb(&HFFE7F0FB)
                ColorBottom = Color.FromArgb(&HFFCCE1FB)
                ColorBackground = SystemColors.Control

                ColorToolStrip1 = Color.FromArgb(&HFFFDFEFF)
                ColorToolStrip2 = Color.FromArgb(&HFFE6F0FA)
                ColorToolStrip3 = Color.FromArgb(&HFFDCE6F4)
                ColorToolStrip4 = Color.FromArgb(&HFFDDE9F7)
            End If
        End Sub

        Shared Sub InitColors(c As Color)
            ColorBorder = HSLColor.Convert(c).ToColorSetLuminosity(100)
            ColorChecked = HSLColor.Convert(c).ToColorSetLuminosity(200)
            ColorBottom = HSLColor.Convert(c).ToColorSetLuminosity(220)
            ColorBackground = HSLColor.Convert(c).ToColorSetLuminosity(230)
            ColorTop = HSLColor.Convert(c).ToColorSetLuminosity(240)

            ColorToolStrip1 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 1)))
            ColorToolStrip2 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 0.7)))
            ColorToolStrip3 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 0.1)))
            ColorToolStrip4 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 0.4)))
        End Sub

        Protected Overrides Sub OnRenderToolStripBorder(e As ToolStripRenderEventArgs)
            ControlPaint.DrawBorder(e.Graphics, e.AffectedBounds, Color.FromArgb(160, 175, 195), ButtonBorderStyle.Solid)
        End Sub

        Protected Overloads Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias

            If TypeOf e.Item Is ToolStripMenuItem AndAlso Not TypeOf e.Item.Owner Is MenuStrip Then
                Dim r = e.TextRectangle

                Dim dropDown = TryCast(e.ToolStrip, ToolStripDropDownMenu)

                If dropDown Is Nothing OrElse dropDown.ShowImageMargin OrElse dropDown.ShowCheckMargin Then
                    TextOffset = CInt(e.Item.Height * 1.1)
                Else
                    TextOffset = CInt(e.Item.Height * 0.2)
                End If

                e.TextRectangle = New Rectangle(TextOffset, CInt((e.Item.Height - r.Height) / 2), r.Width, r.Height)
            End If

            MyBase.OnRenderItemText(e)
        End Sub

        Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)
            If Not TypeOf e.ToolStrip Is ToolStripDropDownMenu AndAlso
                Not e.ToolStrip.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow Then

                Dim r As New Rectangle(-1, -1, e.AffectedBounds.Width, e.AffectedBounds.Height)

                If IsFlat() Then
                    Using b As New SolidBrush(ColorToolStrip2)
                        e.Graphics.FillRectangle(b, r)
                    End Using
                Else
                    Dim cb As New ColorBlend()
                    cb.Colors = {ColorToolStrip1, ColorToolStrip2, ColorToolStrip3, ColorToolStrip4}
                    cb.Positions = {0.0F, 0.5F, 0.5F, 1.0F}

                    Using b As New LinearGradientBrush(r, ColorToolStrip1, ColorToolStrip4, 90)
                        b.InterpolationColors = cb
                        e.Graphics.FillRectangle(b, r)
                    End Using
                End If
            End If
        End Sub

        Protected Overrides Sub OnRenderMenuItemBackground(e As ToolStripItemRenderEventArgs)
            e.Item.ForeColor = Color.Black

            Dim left = 22
            Dim r = New Rectangle(Point.Empty, e.Item.Size)
            Dim g = e.Graphics

            If Not TypeOf e.Item.Owner Is MenuStrip Then
                g.Clear(ColorBackground)
            End If

            If e.Item.Selected AndAlso e.Item.Enabled Then
                If TypeOf e.Item.Owner Is MenuStrip Then
                    DrawButton(e)
                Else
                    g.SmoothingMode = SmoothingMode.AntiAlias

                    Dim r2 = New Rectangle(r.X + 2, r.Y, r.Width - 4, r.Height - 1)

                    If IsFlat() Then
                        Using pen As New Pen(ColorBorder)
                            g.DrawRectangle(pen, r2)
                        End Using

                        r2.Inflate(-1, -1)

                        Using b As New SolidBrush(ColorBottom)
                            g.FillRectangle(b, r2)
                        End Using
                    Else
                        Using path = CreateRoundRectangle(r2, 3)
                            Using b As New LinearGradientBrush(r2,
                                                               ControlPaint.LightLight(ControlPaint.LightLight(ColorTop)),
                                                               ControlPaint.LightLight(ControlPaint.LightLight(ColorBottom)),
                                                               90.0F)
                                g.FillPath(b, path)
                            End Using

                            Using p As New Pen(ColorBorder)
                                g.DrawPath(p, path)
                            End Using
                        End Using

                        r2.Inflate(-1, -1)

                        Using path = CreateRoundRectangle(r2, 3)
                            Using b As New LinearGradientBrush(r2, ColorTop, ColorBottom, 90.0F)
                                g.FillPath(b, path)
                            End Using
                        End Using
                    End If
                End If
            End If
        End Sub

        Sub DrawButton(e As ToolStripItemRenderEventArgs)
            Dim g = e.Graphics
            Dim r = New Rectangle(Point.Empty, e.Item.Size)
            Dim r2 = New Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1)

            If IsFlat() Then
                Using pen As New Pen(ColorBorder)
                    g.DrawRectangle(pen, r2)
                End Using

                r2.Inflate(-1, -1)

                Dim tsb = TryCast(e.Item, ToolStripButton)

                If Not tsb Is Nothing AndAlso tsb.Checked Then
                    Using brush As New SolidBrush(ColorChecked)
                        g.FillRectangle(brush, r2)
                    End Using
                Else
                    Using brush As New SolidBrush(ColorBottom)
                        g.FillRectangle(brush, r2)
                    End Using
                End If
            Else
                g.SmoothingMode = SmoothingMode.AntiAlias

                Dim c1 = HSLColor.Convert(ColorToolStrip1).ToColorAddLuminosity(15)
                Dim c2 = HSLColor.Convert(ColorToolStrip2).ToColorAddLuminosity(15)
                Dim c3 = HSLColor.Convert(ColorToolStrip3).ToColorAddLuminosity(15)
                Dim c4 = HSLColor.Convert(ColorToolStrip4).ToColorAddLuminosity(15)

                Dim cb As New ColorBlend()

                cb.Colors = {c1, c2, c3, c4}
                cb.Positions = {0.0F, 0.5F, 0.5F, 1.0F}

                Using path = CreateRoundRectangle(r2, 3)
                    Using b As New LinearGradientBrush(r2, c1, c4, 90)
                        b.InterpolationColors = cb
                        g.FillPath(b, path)
                    End Using

                    Using p As New Pen(ColorBorder)
                        g.DrawPath(p, path)
                    End Using
                End Using

                r2.Inflate(-1, -1)

                c1 = HSLColor.Convert(ColorToolStrip1).ToColorAddLuminosity(5)
                c2 = HSLColor.Convert(ColorToolStrip2).ToColorAddLuminosity(5)
                c3 = HSLColor.Convert(ColorToolStrip3).ToColorAddLuminosity(-10)
                c4 = HSLColor.Convert(ColorToolStrip4).ToColorAddLuminosity(-10)

                cb.Colors = {c1, c2, c3, c4}
                cb.Positions = {0.0F, 0.5F, 0.5F, 1.0F}

                Using b As New LinearGradientBrush(r2, c1, c4, 90)
                    b.InterpolationColors = cb

                    Using path = CreateRoundRectangle(r2, 3)
                        g.FillPath(b, path)
                    End Using
                End Using
            End If
        End Sub

        Protected Overrides Sub OnRenderDropDownButtonBackground(e As ToolStripItemRenderEventArgs)
            If e.Item.Selected Then DrawButton(e)
        End Sub

        Protected Overrides Sub OnRenderButtonBackground(e As ToolStripItemRenderEventArgs)
            Dim button = DirectCast(e.Item, ToolStripButton)
            If e.Item.Selected OrElse button.Checked Then DrawButton(e)
        End Sub

        Protected Overloads Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
            Dim value = If(e.Direction = ArrowDirection.Down, &H36, &H34)
            Dim s = Convert.ToChar(value).ToString
            Dim font = New Font("Marlett", e.Item.Font.Size - 2)
            Dim size = e.Graphics.MeasureString(s, font)
            Dim x = CInt(e.Item.Width - size.Width)
            Dim y = CInt((e.Item.Height - size.Height) / 2) + 1
            e.Graphics.DrawString(s, font, Brushes.Black, x, y)
        End Sub

        Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)
            Dim x = CInt(e.ImageRectangle.Height * 0.2)
            e.Graphics.DrawImage(e.Image, New Point(x, x))
        End Sub

        Protected Overloads Overrides Sub OnRenderSeparator(e As ToolStripSeparatorRenderEventArgs)
            If e.Item.IsOnDropDown Then
                e.Graphics.Clear(ColorBackground)
                Dim right = e.Item.Width - CInt(TextOffset / 5)
                Dim top = e.Item.Height \ 2
                top -= 1
                Dim b = e.Item.Bounds

                Using p As New Pen(Color.Gray)
                    e.Graphics.DrawLine(p, New Point(TextOffset, top), New Point(right, top))
                End Using
            ElseIf e.Vertical Then
                Dim b = e.Item.Bounds

                Using p As New Pen(SystemColors.ControlDarkDark)
                    e.Graphics.DrawLine(p, CInt(b.Width / 2), CInt(b.Height * 0.15), CInt(b.Width / 2), CInt(b.Height * 0.85))
                End Using
            End If
        End Sub

        Public Shared Function CreateRoundRectangle(r As Rectangle, radius As Integer) As GraphicsPath
            Dim path As New GraphicsPath()

            Dim l = r.Left
            Dim t = r.Top
            Dim w = r.Width
            Dim h = r.Height
            Dim d = radius << 1

            path.AddArc(l, t, d, d, 180, 90)
            path.AddLine(l + radius, t, l + w - radius, t)
            path.AddArc(l + w - d, t, d, d, 270, 90)
            path.AddLine(l + w, t + radius, l + w, t + h - radius)
            path.AddArc(l + w - d, t + h - d, d, d, 0, 90)
            path.AddLine(l + w - radius, t + h, l + radius, t + h)
            path.AddArc(l, t + h - d, d, d, 90, 90)
            path.AddLine(l, t + h - radius, l, t + radius)
            path.CloseFigure()

            Return path
        End Function

        Shared Function IsFlat() As Boolean
            If RenderMode = ToolStripRenderModeEx.Win10Default Then Return True
            If RenderMode = ToolStripRenderModeEx.Win10Auto Then Return True

            If (RenderMode = ToolStripRenderModeEx.SystemDefault OrElse
                RenderMode = ToolStripRenderModeEx.SystemAuto) AndAlso
                OSVersion.Current >= OSVersion.Windows8 Then Return True
        End Function
    End Class
End Namespace
