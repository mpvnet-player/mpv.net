Imports System.ComponentModel

Namespace UI
    Public Class MenuItemEx
        Inherits ToolStripMenuItem

        Shared Property UseTooltips As Boolean

        Sub New()
        End Sub

        Sub New(text As String)
            MyBase.New(text)
        End Sub

        Public Overrides Function GetPreferredSize(constrainingSize As Size) As Size
            Dim ret = MyBase.GetPreferredSize(constrainingSize)
            ret.Height = CInt(Font.Height * 1.4)
            Return ret
        End Function

        Sub SetImage(symbol As Symbol)
            SetImage(symbol, Me)
        End Sub

        Shared Async Sub SetImage(symbol As Symbol, mi As ToolStripMenuItem)
            If symbol = Symbol.None Then
                mi.Image = Nothing
                Exit Sub
            End If

            Dim img = Await ImageHelp.GetSymbolImageAsync(symbol)

            Try
                If Not mi.IsDisposed Then
                    mi.ImageScaling = ToolStripItemImageScaling.None
                    mi.Image = img
                End If
            Catch
            End Try
        End Sub

        Private Function ShouldSerializeHelpText() As Boolean
            Return HelpValue <> ""
        End Function

        Private HelpValue As String

        Property Help() As String
            Get
                Return HelpValue
            End Get
            Set(Value As String)
                HelpValue = Value

                If UseTooltips Then
                    If HelpValue <> "" Then
                        If HelpValue.Length < 80 Then
                            ToolTipText = HelpValue.TrimEnd("."c)
                        Else
                            ToolTipText = "Right-click for help"
                        End If
                    End If
                End If
            End Set
        End Property

        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            If e.Button = MouseButtons.Right AndAlso Help <> "" Then
                CloseAll(Me)
                ShowHelp(Text, Help)
            End If

            MyBase.OnMouseDown(e)
        End Sub

        Sub ShowHelp(title As String, content As String)
            If title <> "" Then title = title.TrimEnd("."c, ":"c)
            MsgInfo(title, content)
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
            MyBase.OnClick(e)
        End Sub
    End Class

    Public Class ActionMenuItem
        Inherits MenuItemEx

        Private Action As Action

        Property EnabledFunc As Func(Of Boolean)
        Property VisibleFunc As Func(Of Boolean)

        Property Form As Form

        Sub New()
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
            Me.Help = tooltip
            Me.Enabled = enabled
        End Sub

        Private ShortcutValue As Keys

        Property Shortcut As Keys
            Get
                Return ShortcutValue
            End Get
            Set(value As Keys)
                ShortcutValue = value
                ShortcutKeyDisplayString = KeysHelp.GetKeyString(value) + "  "
                AddHandler Form.KeyDown, AddressOf KeyDown
            End Set
        End Property

        Sub KeyDown(sender As Object, e As KeyEventArgs)
            If Enabled AndAlso e.KeyData = Shortcut AndAlso
                If(EnabledFunc Is Nothing, True, EnabledFunc.Invoke) AndAlso
                If(VisibleFunc Is Nothing, True, VisibleFunc.Invoke) Then

                PerformClick()
                e.Handled = True
            End If
        End Sub

        Sub Opening(sender As Object, e As CancelEventArgs)
            If Not EnabledFunc Is Nothing Then Enabled = EnabledFunc.Invoke
            If Not VisibleFunc Is Nothing Then Visible = VisibleFunc.Invoke
        End Sub

        Protected Overrides Sub OnClick(e As EventArgs)
            Application.DoEvents()
            If Not Action Is Nothing Then Action()
            MyBase.OnClick(e)
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            If Not Form Is Nothing Then RemoveHandler Form.KeyDown, AddressOf KeyDown
            Action = Nothing
            EnabledFunc = Nothing
            VisibleFunc = Nothing
            Form = Nothing
        End Sub

        Shared Function Add(Of T)(items As ToolStripItemCollection,
                                  path As String,
                                  action As Action(Of T),
                                  value As T,
                                  Optional help As String = Nothing) As ActionMenuItem

            Return Add(items, path, Sub() action(value), help)
        End Function

        Shared Function Add(items As ToolStripItemCollection,
                            path As String) As ActionMenuItem

            Return Add(items, path, Nothing)
        End Function

        Shared Function Add(items As ToolStripItemCollection,
                            path As String,
                            action As Action) As ActionMenuItem

            Return Add(items, path, action, Symbol.None, Nothing)
        End Function

        Shared Function Add(items As ToolStripItemCollection,
                            path As String,
                            action As Action,
                            tip As String) As ActionMenuItem

            Return Add(items, path, action, Symbol.None, tip)
        End Function

        Shared Function Add(items As ToolStripItemCollection,
                            path As String,
                            action As Action,
                            symbol As Symbol,
                            Optional tip As String = Nothing) As ActionMenuItem

            Dim a = path.SplitNoEmpty(" | ")
            Dim l = items

            For x = 0 To a.Length - 1
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
                            Dim item As New ActionMenuItem(a(x), action, tip)
                            item.SetImage(symbol)
                            l.Add(item)
                            l = item.DropDownItems
                            Return item
                        End If
                    Else
                        Dim item As New ActionMenuItem()
                        item.Text = a(x)
                        l.Add(item)
                        l = item.DropDownItems
                    End If
                End If
            Next
        End Function
    End Class

    Public Class ContextMenuStripEx
        Inherits ContextMenuStrip

        Private FormValue As Form

        Sub New()
        End Sub

        Sub New(container As IContainer)
            MyBase.New(container)
        End Sub

        Protected Overrides Sub OnOpening(e As CancelEventArgs)
            MyBase.OnOpening(e)
            MenuHelp.SetRenderer(Me)
        End Sub

        Protected Overrides Sub OnHandleCreated(e As EventArgs)
            MyBase.OnHandleCreated(e)
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
            End Set
        End Property

        Function Add(path As String) As ActionMenuItem
            Return Add(path, Nothing)
        End Function

        Function Add(path As String,
                     action As Action) As ActionMenuItem

            Return Add(path, action, Nothing)
        End Function

        Function Add(path As String,
                     action As Action,
                     help As String) As ActionMenuItem

            Return Add(path, action, help, True)
        End Function

        Function Add(path As String,
                     action As Action,
                     help As String,
                     enabled As Boolean) As ActionMenuItem

            Dim ret = ActionMenuItem.Add(Items, path, action)
            If ret Is Nothing Then Exit Function

            ret.Form = Form
            ret.Help = help
            ret.Enabled = enabled

            AddHandler Opening, AddressOf ret.Opening

            Return ret
        End Function

        Function Add(path As String,
                     action As Action,
                     shortcut As Keys,
                     enabledFunc As Func(Of Boolean),
                     Optional help As String = Nothing) As ActionMenuItem

            Dim ret = ActionMenuItem.Add(Items, path, action)

            ret.Form = Form
            ret.Shortcut = shortcut
            ret.EnabledFunc = enabledFunc
            ret.Help = help

            AddHandler Opening, AddressOf ret.Opening

            Return ret
        End Function

        Function GetTips() As StringPairList
            Dim ret As New StringPairList

            For Each i In GetItems.OfType(Of ActionMenuItem)()
                If i.Help <> "" Then
                    Dim pair As New StringPair

                    If i.Text.EndsWith("...") Then
                        pair.Name = i.Text.TrimEnd("."c)
                    Else
                        pair.Name = i.Text
                    End If

                    pair.Value = i.Help
                    ret.Add(pair)
                End If
            Next

            Return ret
        End Function

        Function GetKeys() As StringPairList
            Dim ret As New StringPairList

            For Each i In GetItems.OfType(Of ActionMenuItem)()
                If i.ShortcutKeyDisplayString <> "" Then
                    Dim sp As New StringPair

                    If i.Text.EndsWith("...") Then
                        sp.Name = i.Text.TrimEnd("."c)
                    Else
                        sp.Name = i.Text
                    End If

                    sp.Value = i.ShortcutKeyDisplayString
                    ret.Add(sp)
                End If
            Next

            Return ret
        End Function

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

    Public Class MenuHelp
        Shared Sub SetRenderer(ms As ToolStrip)
            ms.Renderer = New ToolStripRendererEx(ToolStripRenderModeEx.SystemAuto)
        End Sub
    End Class

    Public Enum ToolStripRenderModeEx
        SystemAuto
        SystemDefault
        Win7Auto
        Win7Default
        Win10Auto
        Win10Default
    End Enum
End Namespace