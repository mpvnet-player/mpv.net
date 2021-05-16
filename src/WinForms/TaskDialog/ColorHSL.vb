Imports System.Drawing

<Serializable>
Public Structure ColorHSL
    Private ReadOnly _h As Integer
    Private ReadOnly _s As Single
    Private ReadOnly _l As Single
    Private ReadOnly _a As Single


    Public ReadOnly Property H As Integer
        Get
            Return _h
        End Get
    End Property

    Public ReadOnly Property S As Single
        Get
            Return _s
        End Get
    End Property

    Public ReadOnly Property L As Single
        Get
            Return _l
        End Get
    End Property

    Public ReadOnly Property A As Single
        Get
            Return _a
        End Get
    End Property

    Public Sub New(h As Integer, s As Single, l As Single, Optional a As Single = 1.0F)
        Dim newHue = h Mod 360
        _h = Mathf.Clamp(If(newHue < 0, newHue + 360, newHue), 0, 359)
        _s = Mathf.Clamp01(s)
        _l = Mathf.Clamp01(l)
        _a = Mathf.Clamp01(a)
    End Sub

    Public Shared Widening Operator CType(colorHSL As ColorHSL) As Color
        Dim hue As Integer = Mathf.Clamp(colorHSL.H Mod 360, 0, 359)
        Dim saturation As Single = Mathf.Clamp01(colorHSL.S)
        Dim lightness As Single = Mathf.Clamp01(colorHSL.L)
        Dim alpha As Single = Mathf.Clamp01(colorHSL.A)
        Dim c As Single = (1.0F - Math.Abs(2.0F * lightness - 1.0F)) * saturation
        Dim x As Single = c * (1.0F - Math.Abs((hue / 60.0F) Mod 2.0F - 1.0F))
        Dim m As Single = lightness - c / 2.0F
        Dim r As Single = c
        Dim g As Single = 0
        Dim b As Single = x

        If hue < 300 Then
            r = x
            g = 0
            b = c
        End If

        If hue < 240 Then
            r = 0
            g = x
            b = c
        End If

        If hue < 180 Then
            r = 0
            g = c
            b = x
        End If

        If hue < 120 Then
            r = x
            g = c
            b = 0
        End If

        If hue < 60 Then
            r = c
            g = x
            b = 0
        End If

        Return Color.FromArgb(CType(alpha * 255.0F, Byte), CType(Mathf.Clamp01(r + m) * 255.0F, Byte), CType(Mathf.Clamp01(g + m) * 255.0F, Byte), CType(Mathf.Clamp01(b + m) * 255.0F, Byte))
    End Operator

    Public Shared Widening Operator CType(color As Color) As ColorHSL
        Dim r As Single = color.R / 255.0F
        Dim g As Single = color.G / 255.0F
        Dim b As Single = color.B / 255.0F
        Dim max As Single = Mathf.Max(r, g, b)
        Dim min As Single = Mathf.Min(r, g, b)
        Dim delta As Single = max - min
        Dim alpha As Single = color.A / 255.0F
        Dim lightness As Single = (min + max) / 2
        Dim saturation As Single = 0F
        saturation = If(delta <> 0F, (delta / (1 - Math.Abs(2 * lightness - 1))), saturation)
        Dim hue As Single = 0F
        If delta <> 0F Then
            hue = If(max = r, 60.0F * (((g - b) / delta) Mod 6.0F), hue)
            hue = If(max = g, 60.0F * (((b - r) / delta) + 2.0F), hue)
            hue = If(max = b, 60.0F * (((r - g) / delta) + 4.0F), hue)
        End If
        Return New ColorHSL(Mathf.RoundToInt(hue), saturation, lightness, alpha)
    End Operator

    Public Shared Operator =(colorA As ColorHSL, colorB As ColorHSL) As Boolean
        Return colorA.Equals(colorB)
    End Operator

    Public Shared Operator <>(colorA As ColorHSL, colorB As ColorHSL) As Boolean
        Return Not colorA.Equals(colorB)
    End Operator

    Public Function AddHue(offset As Integer) As ColorHSL
        Return New ColorHSL(H + offset, S, L, A)
    End Function

    Public Function AddSaturation(offset As Single) As ColorHSL
        Return New ColorHSL(H, S + offset, L, A)
    End Function

    Public Function AddLuminance(offset As Single) As ColorHSL
        Return New ColorHSL(H, S, L + offset, A)
    End Function

    Public Function AddAlpha(offset As Single) As ColorHSL
        Return New ColorHSL(H, S, L, A + offset)
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        Dim objColor = CType(obj, ColorHSL)
        Return H = objColor.H AndAlso S = objColor.S AndAlso L = objColor.L AndAlso A = objColor.A
    End Function

    Public Function SetHue(value As Integer) As ColorHSL
        Return New ColorHSL(value, S, L, A)
    End Function

    Public Function SetSaturation(value As Single) As ColorHSL
        Return New ColorHSL(H, value, L, A)
    End Function

    Public Function SetLuminance(value As Single) As ColorHSL
        Return New ColorHSL(H, S, value, A)
    End Function

    Public Function SetAlpha(value As Single) As ColorHSL
        Return New ColorHSL(H, S, L, value)
    End Function

    Public Function ToColor() As Color
        Return CType(Me, Color)
    End Function

    Public Function ToHTML() As String
        Return ColorTranslator.ToHtml(Me)
    End Function

    Public Overrides Function ToString() As String
        Return String.Format($"HSLA({H:0}, {S:0.00}, {L:0.00}, {A:0.00})")
    End Function
End Structure
