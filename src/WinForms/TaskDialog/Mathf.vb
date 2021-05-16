Public Class Mathf
	Public Shared Function Clamp(value As Integer, min As Integer, max As Integer) As Integer
		If value < min Then
			value = min
		ElseIf value > max Then
			value = max
		End If
		Return value
	End Function

	Public Shared Function Clamp(value As Single, min As Single, max As Single) As Single
		If value < min Then
			value = min
		ElseIf value > max Then
			value = max
		End If
		Return value
	End Function

	Public Shared Function Clamp01(value As Single) As Single
		If value < 0F Then
			value = 0F
		ElseIf value > 1.0F Then
			value = 1.0F
		End If
		Return value
	End Function

	Public Shared Function Min(a As Single, b As Single) As Single
		Return If((a >= b), b, a)
	End Function

	Public Shared Function Min(ParamArray values As Single()) As Single
		Dim num As Integer = values.Length
		Dim result As Single
		If num = 0 Then
			result = 0F
		Else
			Dim num2 As Single = values(0)
			For i As Integer = 1 To num - 1
				If values(i) < num2 Then
					num2 = values(i)
				End If
			Next
			result = num2
		End If
		Return result
	End Function

	Public Shared Function Min(a As Integer, b As Integer) As Integer
		Return If((a >= b), b, a)
	End Function

	Public Shared Function Min(ParamArray values As Integer()) As Integer
		Dim num As Integer = values.Length
		Dim result As Integer
		If num = 0 Then
			result = 0
		Else
			Dim num2 As Integer = values(0)
			For i As Integer = 1 To num - 1
				If values(i) < num2 Then
					num2 = values(i)
				End If
			Next
			result = num2
		End If
		Return result
	End Function

	Public Shared Function Max(a As Single, b As Single) As Single
		Return If((a <= b), b, a)
	End Function

	Public Shared Function Max(ParamArray values As Single()) As Single
		Dim num As Integer = values.Length
		Dim result As Single
		If num = 0 Then
			result = 0F
		Else
			Dim num2 As Single = values(0)
			For i As Integer = 1 To num - 1
				If values(i) > num2 Then
					num2 = values(i)
				End If
			Next
			result = num2
		End If
		Return result
	End Function

	Public Shared Function Max(a As Integer, b As Integer) As Integer
		Return If((a <= b), b, a)
	End Function

	Public Shared Function Max(ParamArray values As Integer()) As Integer
		Dim num As Integer = values.Length
		Dim result As Integer
		If num = 0 Then
			result = 0
		Else
			Dim num2 As Integer = values(0)
			For i As Integer = 1 To num - 1
				If values(i) > num2 Then
					num2 = values(i)
				End If
			Next
			result = num2
		End If
		Return result
	End Function

	Public Shared Function RoundToInt(value As Single) As Integer
		Return CInt(Math.Round(value))
	End Function
End Class
