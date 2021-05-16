
Imports System.Windows.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TaskDialogBaseForm
    Inherits Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpTop = New System.Windows.Forms.TableLayoutPanel()
        Me.TitleLabel = New TaskDialog.TaskDialogBaseForm.LabelEx()
        Me.pbIcon = New System.Windows.Forms.PictureBox()
        Me.paMain = New TaskDialog.TaskDialogBaseForm.TaskDialogPanel()
        Me.spBottom = New TaskDialog.TaskDialogBaseForm.StackPanel()
        Me.InputTextEdit = New TaskDialog.TaskDialogBaseForm.TextEdit()
        Me.MenuButton = New TaskDialog.TaskDialogBaseForm.MenuButtonEx()
        Me.CheckBox = New TaskDialog.TaskDialogBaseForm.CheckBoxEx()
        Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.blDetails = New TaskDialog.TaskDialogBaseForm.ButtonLabel()
        Me.blCopyMessage = New TaskDialog.TaskDialogBaseForm.ButtonLabel()
        Me.tlpMain.SuspendLayout()
        Me.tlpTop.SuspendLayout()
        CType(Me.pbIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.spBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'tlpMain
        '
        Me.tlpMain.ColumnCount = 1
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.Controls.Add(Me.tlpTop, 0, 0)
        Me.tlpMain.Controls.Add(Me.paMain, 0, 1)
        Me.tlpMain.Controls.Add(Me.spBottom, 0, 2)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.RowCount = 3
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tlpMain.Size = New System.Drawing.Size(705, 683)
        Me.tlpMain.TabIndex = 0
        '
        'tlpTop
        '
        Me.tlpTop.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tlpTop.AutoSize = True
        Me.tlpTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpTop.ColumnCount = 2
        Me.tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpTop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpTop.Controls.Add(Me.TitleLabel, 1, 0)
        Me.tlpTop.Controls.Add(Me.pbIcon, 0, 0)
        Me.tlpTop.Location = New System.Drawing.Point(0, 15)
        Me.tlpTop.Margin = New System.Windows.Forms.Padding(0, 15, 0, 20)
        Me.tlpTop.Name = "tlpTop"
        Me.tlpTop.RowCount = 1
        Me.tlpTop.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpTop.Size = New System.Drawing.Size(705, 150)
        Me.tlpTop.TabIndex = 1
        '
        'TitleLabel
        '
        Me.TitleLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TitleLabel.AutoSize = True
        Me.TitleLabel.Location = New System.Drawing.Point(160, 56)
        Me.TitleLabel.Margin = New System.Windows.Forms.Padding(20)
        Me.TitleLabel.Size = New System.Drawing.Size(155, 37)
        Me.TitleLabel.Text = "TitleLabel"
        Me.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pbIcon
        '
        Me.pbIcon.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbIcon.Location = New System.Drawing.Point(40, 30)
        Me.pbIcon.Margin = New System.Windows.Forms.Padding(40, 30, 0, 20)
        Me.pbIcon.Name = "pbIcon"
        Me.pbIcon.Size = New System.Drawing.Size(100, 100)
        Me.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.pbIcon.TabIndex = 2
        Me.pbIcon.TabStop = False
        Me.pbIcon.Visible = False
        '
        'paMain
        '
        Me.paMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.paMain.AutoScroll = True
        Me.paMain.Form = Nothing
        Me.paMain.LineBreaks = 0
        Me.paMain.Location = New System.Drawing.Point(0, 185)
        Me.paMain.Margin = New System.Windows.Forms.Padding(0)
        Me.paMain.Name = "paMain"
        Me.paMain.Size = New System.Drawing.Size(705, 63)
        Me.paMain.TabIndex = 2
        '
        'spBottom
        '
        Me.spBottom.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.spBottom.AutoSize = True
        Me.spBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.spBottom.Controls.Add(Me.InputTextEdit)
        Me.spBottom.Controls.Add(Me.MenuButton)
        Me.spBottom.Controls.Add(Me.CheckBox)
        Me.spBottom.Controls.Add(Me.flpButtons)
        Me.spBottom.Controls.Add(Me.blDetails)
        Me.spBottom.Controls.Add(Me.blCopyMessage)
        Me.spBottom.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.spBottom.Location = New System.Drawing.Point(0, 248)
        Me.spBottom.Margin = New System.Windows.Forms.Padding(0, 0, 0, 30)
        Me.spBottom.Name = "spBottom"
        Me.spBottom.Size = New System.Drawing.Size(705, 405)
        Me.spBottom.TabIndex = 3
        Me.spBottom.WrapContents = False
        '
        'InputTextEdit
        '
        Me.InputTextEdit.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.InputTextEdit.Location = New System.Drawing.Point(30, 0)
        Me.InputTextEdit.Margin = New System.Windows.Forms.Padding(30, 0, 30, 30)
        Me.InputTextEdit.Name = "InputTextEdit"
        Me.InputTextEdit.Size = New System.Drawing.Size(645, 60)
        Me.InputTextEdit.Text = "InputTextEdit"
        Me.InputTextEdit.Visible = False
        '
        'MenuButton
        '
        Me.MenuButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MenuButton.FlatAppearance.BorderSize = 2
        Me.MenuButton.Location = New System.Drawing.Point(30, 90)
        Me.MenuButton.Margin = New System.Windows.Forms.Padding(30, 0, 30, 30)
        Me.MenuButton.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.MenuButton.ShowMenuSymbol = True
        Me.MenuButton.ShowPath = False
        Me.MenuButton.Size = New System.Drawing.Size(645, 70)
        Me.MenuButton.Text2 = "MenuButton"
        Me.MenuButton.Visible = False
        '
        'CheckBox
        '
        Me.CheckBox.AutoSize = True
        Me.CheckBox.FlatAppearance.BorderSize = 2
        Me.CheckBox.Location = New System.Drawing.Point(30, 190)
        Me.CheckBox.Margin = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.CheckBox.Size = New System.Drawing.Size(366, 41)
        Me.CheckBox.Text = "VerificationCheckBox"
        Me.CheckBox.UseVisualStyleBackColor = False
        Me.CheckBox.Visible = False
        '
        'flpButtons
        '
        Me.flpButtons.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.flpButtons.Location = New System.Drawing.Point(505, 231)
        Me.flpButtons.Margin = New System.Windows.Forms.Padding(0)
        Me.flpButtons.Name = "flpButtons"
        Me.flpButtons.Size = New System.Drawing.Size(200, 100)
        Me.flpButtons.TabIndex = 2
        Me.flpButtons.Visible = False
        '
        'blDetails
        '
        Me.blDetails.AutoSize = True
        Me.blDetails.ClickAction = Nothing
        Me.blDetails.Location = New System.Drawing.Point(20, 331)
        Me.blDetails.Margin = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.blDetails.Name = "blDetails"
        Me.blDetails.Size = New System.Drawing.Size(203, 37)
        Me.blDetails.TabIndex = 8
        Me.blDetails.Text = "Show Details"
        Me.blDetails.Visible = False
        '
        'blCopyMessage
        '
        Me.blCopyMessage.AutoSize = True
        Me.blCopyMessage.ClickAction = Nothing
        Me.blCopyMessage.Location = New System.Drawing.Point(20, 368)
        Me.blCopyMessage.Margin = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.blCopyMessage.Name = "blCopyMessage"
        Me.blCopyMessage.Size = New System.Drawing.Size(228, 37)
        Me.blCopyMessage.TabIndex = 7
        Me.blCopyMessage.Text = "Copy Message"
        Me.blCopyMessage.Visible = False
        '
        'TaskDialogBaseForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(288.0!, 288.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(705, 683)
        Me.Controls.Add(Me.tlpMain)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TaskDialogBaseForm"
        Me.ShowIcon = False
        Me.Text = "StaxRip"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.tlpTop.ResumeLayout(False)
        Me.tlpTop.PerformLayout()
        CType(Me.pbIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.spBottom.ResumeLayout(False)
        Me.spBottom.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tlpMain As TableLayoutPanel
    Friend WithEvents CheckBox As CheckBoxEx
    Friend WithEvents flpButtons As FlowLayoutPanel
    Friend WithEvents InputTextEdit As TextEdit
    Friend WithEvents tlpTop As TableLayoutPanel
    Friend WithEvents TitleLabel As LabelEx
    Friend WithEvents MenuButton As MenuButtonEx
    Friend WithEvents paMain As TaskDialogPanel
    Friend WithEvents spBottom As StackPanel
    Friend WithEvents blCopyMessage As ButtonLabel
    Friend WithEvents blDetails As ButtonLabel
    Friend WithEvents pbIcon As PictureBox
End Class
