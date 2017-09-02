Public Structure HSLColor
    Public Sub New(color As Color)
        SetRGB(color.R, color.G, color.B)
    End Sub

    Public Sub New(h As Integer, s As Integer, l As Integer)
        Hue = h
        Saturation = s
        Luminosity = l
    End Sub

    Private HueValue As Double

    Property Hue As Integer
        Get
            Return CInt(HueValue * 240)
        End Get
        Set(value As Integer)
            HueValue = CheckRange(value / 240)
        End Set
    End Property

    Private SaturationValue As Double

    Property Saturation As Integer
        Get
            Return CInt(SaturationValue * 240)
        End Get
        Set(value As Integer)
            SaturationValue = CheckRange(value / 240)
        End Set
    End Property

    Private LuminosityValue As Double

    Property Luminosity As Integer
        Get
            Return CInt(LuminosityValue * 240)
        End Get
        Set(value As Integer)
            LuminosityValue = CheckRange(value / 240)
        End Set
    End Property

    Private Function CheckRange(value As Double) As Double
        If value < 0 Then
            value = 0
        ElseIf value > 1 Then
            value = 1
        End If

        Return value
    End Function

    Function ToColorAddLuminosity(luminosity As Integer) As Color
        Me.Luminosity += luminosity
        Return ToColor()
    End Function

    Function ToColorSetLuminosity(luminosity As Integer) As Color
        Me.Luminosity = luminosity
        Return ToColor()
    End Function

    Function ToColor() As Color
        Dim r, g, b As Double

        If LuminosityValue <> 0 Then
            If SaturationValue = 0 Then
                b = LuminosityValue
                g = LuminosityValue
                r = LuminosityValue
            Else
                Dim temp2 = GetTemp2(Me)
                Dim temp1 = 2.0 * LuminosityValue - temp2

                r = GetColorComponent(temp1, temp2, HueValue + 1.0 / 3.0)
                g = GetColorComponent(temp1, temp2, HueValue)
                b = GetColorComponent(temp1, temp2, HueValue - 1.0 / 3.0)
            End If
        End If

        Return Color.FromArgb(CInt(255 * r), CInt(255 * g), CInt(255 * b))
    End Function

    Private Shared Function GetColorComponent(temp1 As Double,
                                              temp2 As Double,
                                              temp3 As Double) As Double
        temp3 = MoveIntoRange(temp3)

        If temp3 < 1 / 6 Then
            Return temp1 + (temp2 - temp1) * 6.0 * temp3
        ElseIf temp3 < 0.5 Then
            Return temp2
        ElseIf temp3 < 2 / 3 Then
            Return temp1 + ((temp2 - temp1) * ((2 / 3) - temp3) * 6)
        Else
            Return temp1
        End If
    End Function

    Private Shared Function MoveIntoRange(temp3 As Double) As Double
        If temp3 < 0 Then
            temp3 += 1
        ElseIf temp3 > 1 Then
            temp3 -= 1
        End If

        Return temp3
    End Function

    Private Shared Function GetTemp2(hslColor As HSLColor) As Double
        Dim temp2 As Double

        If hslColor.LuminosityValue < 0.5 Then
            temp2 = hslColor.LuminosityValue * (1.0 + hslColor.SaturationValue)
        Else
            temp2 = hslColor.LuminosityValue + hslColor.SaturationValue - (hslColor.LuminosityValue * hslColor.SaturationValue)
        End If

        Return temp2
    End Function

    Public Shared Function Convert(c As Color) As HSLColor
        Dim r As New HSLColor()

        r.HueValue = c.GetHue() / 360.0
        r.LuminosityValue = c.GetBrightness()
        r.SaturationValue = c.GetSaturation()

        Return r
    End Function

    Public Sub SetRGB(red As Integer, green As Integer, blue As Integer)
        Dim hc = HSLColor.Convert(Color.FromArgb(red, green, blue))

        HueValue = hc.HueValue
        SaturationValue = hc.SaturationValue
        LuminosityValue = hc.LuminosityValue
    End Sub
End Structure