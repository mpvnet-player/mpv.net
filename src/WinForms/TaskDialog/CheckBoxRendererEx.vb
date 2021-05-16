
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles

Public Class CheckBoxRendererEx
    Public Shared Sub DrawCheckBox(g As Graphics, glyphLocation As Point, state As CheckBoxState)
        DrawCheckBox(g, New Rectangle(glyphLocation, GetGlyphSize(g, state)), state)
    End Sub

    Public Shared Sub DrawCheckBox(g As Graphics, rect As Rectangle, state As CheckBoxState)
        'Dim smoothingModeBackup = g.SmoothingMode
        'g.SmoothingMode = SmoothingMode.AntiAlias

        'If ThemeManager.CurrentTheme.UsesSystemColors OrElse DesignHelp.IsDesignMode Then
        '    CheckBoxRenderer.DrawCheckBox(g, rect.Location, state)
        '    Return
        'End If

        'Dim theme = ThemeManager.CurrentTheme.General.Controls.CheckBox
        'Dim checked = state = CheckBoxState.CheckedDisabled OrElse state = CheckBoxState.CheckedHot OrElse state = CheckBoxState.CheckedNormal OrElse state = CheckBoxState.CheckedPressed
        'Dim backColor = If(checked, theme.BoxCheckedColor, theme.BoxColor)
        'Dim borderColor = If(checked, theme.BorderCheckedColor, theme.BorderColor)
        'Dim borderStrength = 2
        'Dim checkmarkColor = theme.CheckmarkColor
        'Dim checkmarkStrength = rect.Width \ 5

        'Select Case state
        '    Case CheckBoxState.CheckedNormal
        '        Using brush As New SolidBrush(backColor)
        '            Using pen As New Pen(borderColor, borderStrength)
        '                g.FillRectangle(brush, rect)
        '                g.DrawRectangle(pen, rect)
        '            End Using
        '        End Using

        '        Dim startX1 = rect.Left + rect.Width / 4.5F
        '        Dim startY1 = rect.Top + rect.Height / 2.75F
        '        Dim endX1 = rect.Left + rect.Width / 3.0F
        '        Dim endY1 = rect.Top + rect.Height / 1.2F

        '        Dim startX2 = rect.Left + rect.Width / 3.6F
        '        Dim startY2 = rect.Top + rect.Height / 1.3F
        '        Dim endX2 = rect.Left + rect.Width / 1.2F
        '        Dim endY2 = rect.Top + rect.Height / 5.0F

        '        Using pen As New Pen(checkmarkColor, checkmarkStrength)
        '            g.DrawLine(pen, startX1, startY1, endX1, endY1)
        '            g.DrawLine(pen, startX2, startY2, endX2, endY2)
        '        End Using

        '    Case CheckBoxState.UncheckedNormal
        '        Using brush As New SolidBrush(backColor)
        '            Using pen As New Pen(borderColor, borderStrength)
        '                g.FillRectangle(brush, rect)
        '                g.DrawRectangle(pen, rect)
        '            End Using
        '        End Using
        'End Select

        'g.SmoothingMode = smoothingModeBackup
    End Sub

    Shared Function GetGlyphSize(g As Graphics, state As CheckBoxState) As Size
        Dim defaultSize = CheckBoxRenderer.GetGlyphSize(g, state)
        Dim uiSize = New Size(CInt(defaultSize.Width), CInt(defaultSize.Height))
        Return uiSize
    End Function
End Class
