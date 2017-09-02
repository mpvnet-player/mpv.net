Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.Win32
Imports VB6 = Microsoft.VisualBasic

Imports vbnet.UI

Module StringExtensions
    <Extension>
    Public Function Multiply(instance As String, multiplier As Integer) As String
        Dim sb As New StringBuilder(multiplier * instance.Length)

        For i = 0 To multiplier - 1
            sb.Append(instance)
        Next

        Return sb.ToString()
    End Function

    <Extension>
    Function IsValidFileName(instance As String) As Boolean
        If instance = "" Then Return False
        Dim chars = """*/:<>?\|"

        For Each i In instance
            If chars.Contains(i) Then Return False
            If Convert.ToInt32(i) < 32 Then Return False
        Next

        Return True
    End Function

    <Extension>
    Function IsANSICompatible(instance As String) As Boolean
        If instance = "" Then Return True
        Dim bytes = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(instance))
        Return instance = Encoding.Unicode.GetString(Encoding.Convert(Encoding.Default, Encoding.Unicode, bytes))
    End Function

    <Extension()>
    Function FileName(instance As String) As String
        If instance = "" Then Return ""
        Dim index = instance.LastIndexOf(Path.DirectorySeparatorChar)
        If index > -1 Then Return instance.Substring(index + 1)
        Return instance
    End Function

    <Extension()>
    Function Upper(instance As String) As String
        If instance = "" Then Return ""
        Return instance.ToUpperInvariant
    End Function

    <Extension()>
    Function Lower(instance As String) As String
        If instance = "" Then Return ""
        Return instance.ToLowerInvariant
    End Function

    <Extension()>
    Function ChangeExt(instance As String, value As String) As String
        If instance = "" Then Return ""
        If value = "" Then Return instance
        If Not value.StartsWith(".") Then value = "." + value
        Return instance.DirAndBase + value.ToLower
    End Function

    <Extension()>
    Function Escape(instance As String) As String
        If instance = "" Then Return ""

        Dim chars = " ()".ToCharArray

        For Each i In chars
            If instance.Contains(i) Then Return """" + instance + """"
        Next

        Return instance
    End Function

    <Extension()>
    Function Parent(instance As String) As String
        Return DirPath.GetParent(instance)
    End Function

    <Extension()>
    Function ExistingParent(instance As String) As String
        Dim ret = instance.Parent
        If Not Directory.Exists(ret) Then ret = ret.Parent Else Return ret
        If Not Directory.Exists(ret) Then ret = ret.Parent Else Return ret
        If Not Directory.Exists(ret) Then ret = ret.Parent Else Return ret
        If Not Directory.Exists(ret) Then ret = ret.Parent Else Return ret
        If Not Directory.Exists(ret) Then ret = ret.Parent Else Return ret
        Return ret
    End Function

    <Extension()>
    Function Ext(instance As String) As String
        Return FilePath.GetExt(instance)
    End Function

    <Extension()>
    Function ExtFull(instance As String) As String
        Return FilePath.GetExtFull(instance)
    End Function

    <Extension()>
    Function Base(instance As String) As String
        Return FilePath.GetBase(instance)
    End Function

    <Extension()>
    Function Dir(instance As String) As String
        Return FilePath.GetDir(instance)
    End Function

    <Extension()>
    Function DirName(instance As String) As String
        Return DirPath.GetName(instance)
    End Function

    <Extension()>
    Function DirAndBase(instance As String) As String
        Return FilePath.GetDirAndBase(instance)
    End Function

    <Extension()>
    Function ContainsAll(instance As String, all As IEnumerable(Of String)) As Boolean
        If instance <> "" Then Return all.All(Function(arg) instance.Contains(arg))
    End Function

    <Extension()>
    Function ContainsAny(instance As String, any As IEnumerable(Of String)) As Boolean
        If instance <> "" Then Return any.Any(Function(arg) instance.Contains(arg))
    End Function

    <Extension()>
    Function EqualsAny(instance As String, ParamArray values As String()) As Boolean
        If instance = "" OrElse values.NothingOrEmpty Then Return False
        Return values.Contains(instance)
    End Function

    <Extension()>
    Function FixDir(instance As String) As String
        If instance = "" Then Return ""

        While instance.EndsWith(DirPath.Separator + DirPath.Separator)
            instance = instance.Substring(0, instance.Length - 1)
        End While

        If instance.EndsWith(DirPath.Separator) Then Return instance
        Return instance + DirPath.Separator
    End Function

    <Extension()>
    Function FixBreak(value As String) As String
        value = value.Replace(VB6.ChrW(13) + VB6.ChrW(10), VB6.ChrW(10))
        value = value.Replace(VB6.ChrW(13), VB6.ChrW(10))
        Return value.Replace(VB6.ChrW(10), VB6.ChrW(13) + VB6.ChrW(10))
    End Function

    <Extension()>
    Function ContainsUnicode(value As String) As Boolean
        If value = "" Then Return False

        For Each i In value
            If Convert.ToInt32(i) > 255 Then Return True
        Next
    End Function

    <Extension()>
    Function ToTitleCase(value As String) As String
        'TextInfo.ToTitleCase won't work on all upper strings
        Return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower)
    End Function

    <Extension()>
    Function IsInt(value As String) As Boolean
        Return Integer.TryParse(value, Nothing)
    End Function

    <Extension()>
    Function ToInt(value As String, Optional defaultValue As Integer = 0) As Integer
        If Not Integer.TryParse(value, Nothing) Then Return defaultValue
        Return CInt(value)
    End Function

    <Extension()>
    Function IsSingle(value As String) As Boolean
        If value <> "" Then
            If value.Contains(",") Then value = value.Replace(",", ".")

            Return Single.TryParse(value,
                                   NumberStyles.Float Or NumberStyles.AllowThousands,
                                   CultureInfo.InvariantCulture,
                                   Nothing)
        End If
    End Function

    <Extension()>
    Function ToSingle(value As String, Optional defaultValue As Single = 0) As Single
        If value <> "" Then
            If value.Contains(",") Then value = value.Replace(",", ".")

            Dim ret As Single

            If Single.TryParse(value,
                               NumberStyles.Float Or NumberStyles.AllowThousands,
                               CultureInfo.InvariantCulture,
                               ret) Then
                Return ret
            End If
        End If

        Return defaultValue
    End Function

    <Extension()>
    Function IsDouble(value As String) As Boolean
        If value <> "" Then
            If value.Contains(",") Then value = value.Replace(",", ".")

            Return Double.TryParse(value,
                                   NumberStyles.Float Or NumberStyles.AllowThousands,
                                   CultureInfo.InvariantCulture,
                                   Nothing)
        End If
    End Function

    <Extension()>
    Function ToDouble(value As String, Optional defaultValue As Single = 0) As Double
        If value <> "" Then
            If value.Contains(",") Then value = value.Replace(",", ".")

            Dim ret As Double

            If Double.TryParse(value,
                               NumberStyles.Float Or NumberStyles.AllowThousands,
                               CultureInfo.InvariantCulture,
                               ret) Then
                Return ret
            End If
        End If

        Return defaultValue
    End Function

    <Extension()>
    Function FormatColumn(value As String, delimiter As String) As String
        If value = "" Then Return ""
        Dim lines = value.SplitKeepEmpty(BR)
        Dim leftSides As New List(Of String)

        For Each i In lines
            Dim pos = i.IndexOf(delimiter)

            If pos > 0 Then
                leftSides.Add(i.Substring(0, pos).Trim)
            Else
                leftSides.Add(i)
            End If
        Next

        Dim highest = Aggregate i In leftSides Into Max(i.Length)
        Dim ret As New List(Of String)

        For i = 0 To lines.Length - 1
            Dim line = lines(i)

            If line.Contains(delimiter) Then
                ret.Add(leftSides(i).PadRight(highest) + " " + delimiter + " " + line.Substring(line.IndexOf(delimiter) + 1).Trim)
            Else
                ret.Add(leftSides(i))
            End If
        Next

        Return ret.Join(BR)
    End Function

    <Extension()>
    Sub WriteANSIFile(instance As String, path As String)
        WriteFile(instance, path, Encoding.Default)
    End Sub

    <Extension()>
    Sub WriteUTF8File(instance As String, path As String)
        WriteFile(instance, path, Encoding.UTF8)
    End Sub

    <Extension()>
    Sub WriteFile(value As String, path As String, encoding As Encoding)
        Try
            File.WriteAllText(path, value, encoding)
        Catch ex As Exception
            MsgException(ex)
        End Try
    End Sub

    <Extension()>
    Function Left(value As String, index As Integer) As String
        If value = "" OrElse index < 0 Then Return ""
        If index > value.Length Then Return value
        Return value.Substring(0, index)
    End Function

    <Extension()>
    Function Left(value As String, start As String) As String
        If value = "" OrElse start = "" Then Return ""
        If Not value.Contains(start) Then Return ""
        Return value.Substring(0, value.IndexOf(start))
    End Function

    <Extension()>
    Function LeftLast(value As String, start As String) As String
        If Not value.Contains(start) Then Return ""
        Return value.Substring(0, value.LastIndexOf(start))
    End Function

    <Extension()>
    Function Right(value As String, start As String) As String
        If value = "" OrElse start = "" Then Return ""
        If Not value.Contains(start) Then Return ""
        Return value.Substring(value.IndexOf(start) + start.Length)
    End Function

    <Extension()>
    Function RightLast(value As String, start As String) As String
        If value = "" OrElse start = "" Then Return ""
        If Not value.Contains(start) Then Return ""
        Return value.Substring(value.LastIndexOf(start) + start.Length)
    End Function

    <Extension()>
    Function EqualIgnoreCase(a As String, b As String) As Boolean
        If a = "" OrElse b = "" Then Return False
        Return String.Compare(a, b, StringComparison.OrdinalIgnoreCase) = 0
    End Function

    <Extension()>
    Function Shorten(value As String, maxLength As Integer) As String
        If value = "" OrElse value.Length <= maxLength Then
            Return value
        End If

        Return value.Substring(0, maxLength)
    End Function

    <Extension()>
    Function SplitNoEmpty(value As String, ParamArray delimiters As String()) As String()
        Return value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
    End Function

    <Extension()>
    Function SplitKeepEmpty(value As String, ParamArray delimiters As String()) As String()
        Return value.Split(delimiters, StringSplitOptions.None)
    End Function

    <Extension()>
    Function SplitNoEmptyAndWhiteSpace(value As String, ParamArray delimiters As String()) As String()
        If value = "" Then Return {}

        Dim a = SplitNoEmpty(value, delimiters)

        For i = 0 To a.Length - 1
            a(i) = a(i).Trim
        Next

        Dim l = a.ToList

        While l.Contains("")
            l.Remove("")
        End While

        Return l.ToArray
    End Function

    <Extension()>
    Function SplitLinesNoEmpty(value As String) As String()
        Return SplitNoEmpty(value, Environment.NewLine)
    End Function

    <Extension()>
    Function RemoveChars(value As String, chars As String) As String
        Dim ret = value

        For Each i In value
            If chars.IndexOf(i) >= 0 Then
                ret = ret.Replace(i, "")
            End If
        Next

        Return ret
    End Function

    <Extension()>
    Function DeleteRight(value As String, count As Integer) As String
        Return Left(value, value.Length - count)
    End Function

    <Extension()>
    Function ReplaceUnicode(value As String) As String
        If value.Contains(Convert.ToChar(&H2212)) Then
            value = value.Replace(Convert.ToChar(&H2212), "-"c)
        End If

        Return value
    End Function

    <Extension()>
    Function SHA512Hash(value As String) As String
        Dim crypt = SHA512CryptoServiceProvider.Create()
        Dim hash = crypt.ComputeHash(ASCIIEncoding.ASCII.GetBytes(value))
        Dim sb As New StringBuilder()

        For Each i In hash
            sb.Append(i.ToString("x2"))
        Next

        Return sb.ToString()
    End Function

    <Extension()>
    Sub ToClipboard(value As String)
        If value <> "" Then
            Clipboard.SetText(value)
        Else
            Clipboard.Clear()
        End If
    End Sub
End Module

Module MiscExtensions
    <Extension()>
    Function ToInvariantString(instance As Double, format As String) As String
        Dim ret = instance.ToString(format, CultureInfo.InvariantCulture)

        If (ret.Contains(".") OrElse ret.Contains(",")) AndAlso ret.EndsWith("0") Then
            ret = ret.TrimEnd("0"c)
        End If

        Return ret
    End Function

    <Extension()>
    Function ToInvariantString(instance As IConvertible) As String
        If Not instance Is Nothing Then Return instance.ToString(CultureInfo.InvariantCulture)
    End Function

    <Extension()>
    Function ContainsAny(Of T)(instance As IEnumerable(Of T), ParamArray values As T()) As Boolean
        Return instance.Where(Function(arg) values.Contains(arg)).Count > 0
    End Function

    <Extension()>
    Function Sort(Of T)(instance As IEnumerable(Of T)) As IEnumerable(Of T)
        Dim ret = instance.ToArray
        Array.Sort(Of T)(ret)
        Return ret
    End Function

    <Extension()>
    Function Join(instance As IEnumerable(Of String),
                  delimiter As String,
                  Optional removeEmpty As Boolean = False) As String

        If instance Is Nothing Then Return Nothing
        Dim containsEmpty As Boolean

        For Each item In instance
            If item = "" Then
                containsEmpty = True
                Exit For
            End If
        Next

        If containsEmpty AndAlso removeEmpty Then instance = instance.Where(Function(arg) arg <> "")
        Return String.Join(delimiter, instance)
    End Function

    <Extension()>
    Function GetAttribute(Of T)(mi As MemberInfo) As T
        Dim attributes = mi.GetCustomAttributes(True)

        If Not attributes.NothingOrEmpty Then
            If attributes.Length = 1 Then
                If TypeOf attributes(0) Is T Then
                    Return DirectCast(attributes(0), T)
                End If
            Else
                For Each i In attributes
                    If TypeOf i Is T Then
                        Return DirectCast(i, T)
                    End If
                Next
            End If
        End If
    End Function

    <Extension()>
    Function IsDigit(c As Char) As Boolean
        Return Char.IsDigit(c)
    End Function

    <Extension()>
    Function EnsureRange(value As Integer, min As Integer, max As Integer) As Integer
        If value < min Then
            value = min
        ElseIf value > max Then
            value = max
        End If

        Return value
    End Function

    <Extension()>
    Function NeutralCulture(ci As CultureInfo) As CultureInfo
        If ci.IsNeutralCulture Then Return ci Else Return ci.Parent
    End Function

    <Extension()>
    Function NothingOrEmpty(strings As IEnumerable(Of String)) As Boolean
        If strings Is Nothing OrElse strings.Count = 0 Then Return True

        For Each i In strings
            If i = "" Then Return True
        Next
    End Function

    <Extension()>
    Function NothingOrEmpty(objects As IEnumerable(Of Object)) As Boolean
        If objects Is Nothing OrElse objects.Count = 0 Then Return True

        For Each i In objects
            If i Is Nothing Then Return True
        Next
    End Function
End Module

Module RegistryKeyExtensions
    Private Function GetValue(Of T)(rootKey As RegistryKey, key As String, name As String) As T
        Using k = rootKey.OpenSubKey(key)
            If Not k Is Nothing Then
                Dim r = k.GetValue(name)

                If Not r Is Nothing Then
                    Try
                        Return CType(r, T)
                    Catch ex As Exception
                    End Try
                End If
            End If
        End Using
    End Function

    <Extension()>
    Function GetString(rootKey As RegistryKey, subKey As String, name As String) As String
        Return GetValue(Of String)(rootKey, subKey, name)
    End Function

    <Extension()>
    Function GetInt(rootKey As RegistryKey, subKey As String, name As String) As Integer
        Return GetValue(Of Integer)(rootKey, subKey, name)
    End Function

    <Extension()>
    Function GetBoolean(rootKey As RegistryKey, subKey As String, name As String) As Boolean
        Return GetValue(Of Boolean)(rootKey, subKey, name)
    End Function

    <Extension()>
    Function GetValueNames(rootKey As RegistryKey, subKeyName As String) As IEnumerable(Of String)
        Using k = rootKey.OpenSubKey(subKeyName)
            If Not k Is Nothing Then
                Return k.GetValueNames
            End If
        End Using

        Return {}
    End Function

    <Extension()>
    Sub GetSubKeys(rootKey As RegistryKey, keys As List(Of RegistryKey))
        If Not rootKey Is Nothing Then
            keys.Add(rootKey)

            For Each i In rootKey.GetSubKeyNames
                GetSubKeys(rootKey.OpenSubKey(i), keys)
            Next
        End If
    End Sub

    <Extension()>
    Sub Write(rootKey As RegistryKey, subKey As String, valueName As String, valueValue As Object)
        Dim k = rootKey.OpenSubKey(subKey, True)

        If k Is Nothing Then
            k = rootKey.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree)
        End If

        k.SetValue(valueName, valueValue)
        k.Close()
    End Sub

    <Extension()>
    Sub DeleteValue(rootKey As RegistryKey, key As String, valueName As String)
        Using k = rootKey.OpenSubKey(key, True)
            If Not k Is Nothing Then
                k.DeleteValue(valueName, False)
            End If
        End Using
    End Sub
End Module

Module ControlExtension
    <Extension()>
    Sub ScaleClientSize(instance As Control, width As Single, height As Single)
        instance.ClientSize = New Size(CInt(instance.Font.Height * width), CInt(instance.Font.Height * height))
    End Sub

    <Extension()>
    Sub SetFontStyle(instance As Control, style As FontStyle)
        instance.Font = New Font(instance.Font.FontFamily, instance.Font.Size, style)
    End Sub

    <Extension()>
    Sub AddClickAction(instance As Control, action As Action)
        AddHandler instance.Click, Sub() action()
    End Sub

    <Extension()>
    Function ClientMousePos(instance As Control) As Point
        Return instance.PointToClient(Control.MousePosition)
    End Function

    <Extension()>
    Function GetMaxTextSpace(instance As Control, ParamArray values As String()) As String
        Dim ret As String

        For x = 4 To 2 Step -1
            ret = values.Join("".PadRight(x))
            Dim testWidth = TextRenderer.MeasureText(ret, instance.Font).Width
            If testWidth < instance.Width - 2 OrElse x = 2 Then Return ret
        Next

        Return ret
    End Function
End Module

Module UIExtensions
    <Extension()>
    Sub ClearAndDisplose(instance As ToolStripItemCollection)
        For Each i In instance.OfType(Of IDisposable).ToArray
            i.Dispose()
        Next

        instance.Clear()
    End Sub

    <Extension()>
    Function ResizeToSmallIconSize(img As Image) As Image
        If Not img Is Nothing AndAlso img.Size <> SystemInformation.SmallIconSize Then
            Dim s = SystemInformation.SmallIconSize
            Dim r As New Bitmap(s.Width, s.Height)

            Using g = Graphics.FromImage(DirectCast(r, Image))
                g.SmoothingMode = SmoothingMode.AntiAlias
                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.DrawImage(img, 0, 0, s.Width, s.Height)
            End Using

            Return r
        End If

        Return img
    End Function

    <Extension()>
    Function ResizeImage(image As Image, ByVal height As Integer) As Image
        Dim percentHeight = height / image.Height
        Dim ret = New Bitmap(CInt(image.Width * percentHeight), CInt(height))

        Using g = Graphics.FromImage(ret)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.DrawImage(image, 0, 0, ret.Width, ret.Height)
        End Using

        Return ret
    End Function

    <Extension()>
    Sub SetSelectedPath(d As FolderBrowserDialog, path As String)
        If Not Directory.Exists(path) Then path = path.ExistingParent
        If Directory.Exists(path) Then d.SelectedPath = path
    End Sub

    <Extension()>
    Sub SetInitDir(d As FileDialog, ParamArray paths As String())
        For Each i In paths
            If Not Directory.Exists(i) Then i = i.ExistingParent

            If Directory.Exists(i) Then
                d.InitialDirectory = i
                Exit For
            End If
        Next
    End Sub

    <Extension()>
    Sub SetFilter(d As FileDialog, values As IEnumerable(Of String))
        d.Filter = GetFilter(values)
    End Sub

    Function GetFilter(values As IEnumerable(Of String)) As String
        Return "*." + values.Join(";*.") + "|*." + values.Join(";*.") + "|All Files|*.*"
    End Function

    <Extension()>
    Sub SendMessageCue(tb As TextBox, value As String, hideWhenFocused As Boolean)
        Dim wParam = If(hideWhenFocused, 0, 1)
        Native.SendMessage(tb.Handle, Native.EM_SETCUEBANNER, wParam, value)
    End Sub

    <Extension()>
    Sub SendMessageCue(c As ComboBox, value As String)
        Native.SendMessage(c.Handle, Native.CB_SETCUEBANNER, 1, value)
    End Sub

    Function GetPropertyValue(obj As String, propertyName As String) As Object
        obj.GetType.GetProperty(propertyName).GetValue(obj)
    End Function

    <Extension()>
    Sub RemoveSelection(dgv As DataGridView)
        For Each i As DataGridViewRow In dgv.SelectedRows
            dgv.Rows.Remove(i)
        Next

        If dgv.SelectedRows.Count = 0 AndAlso dgv.RowCount > 0 Then
            dgv.Rows(dgv.RowCount - 1).Selected = True
        End If
    End Sub

    <Extension()>
    Function CanMoveUp(dgv As DataGridView) As Boolean
        Return dgv.SelectedRows.Count > 0 AndAlso dgv.SelectedRows(0).Index > 0
    End Function

    <Extension()>
    Function CanMoveDown(dgv As DataGridView) As Boolean
        Return dgv.SelectedRows.Count > 0 AndAlso dgv.SelectedRows(0).Index < dgv.RowCount - 1
    End Function

    <Extension()>
    Sub MoveSelectionUp(dgv As DataGridView)
        If CanMoveUp(dgv) Then
            Dim bs = DirectCast(dgv.DataSource, BindingSource)
            Dim pos = bs.Position
            bs.RaiseListChangedEvents = False
            Dim current = bs.Current
            bs.Remove(current)
            pos -= 1
            bs.Insert(pos, current)
            bs.Position = pos
            bs.RaiseListChangedEvents = True
            bs.ResetBindings(False)
        End If
    End Sub

    <Extension()>
    Sub MoveSelectionDown(dgv As DataGridView)
        If CanMoveDown(dgv) Then
            Dim bs = DirectCast(dgv.DataSource, BindingSource)
            Dim pos = bs.Position
            bs.RaiseListChangedEvents = False
            Dim current = bs.Current
            bs.Remove(current)
            pos += 1
            bs.Insert(pos, current)
            bs.Position = pos
            bs.RaiseListChangedEvents = True
            bs.ResetBindings(False)
        End If
    End Sub
End Module