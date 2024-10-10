
namespace MpvNet.Windows.WinForms;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        CursorTimer = new System.Windows.Forms.Timer(components);
        ProgressTimer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        // 
        // CursorTimer
        // 
        CursorTimer.Enabled = true;
        CursorTimer.Interval = 500;
        CursorTimer.Tick += CursorTimer_Tick;
        // 
        // ProgressTimer
        // 
        ProgressTimer.Tick += ProgressTimer_Tick;
        // 
        // MainForm
        // 
        AllowDrop = true;
        AutoScaleDimensions = new System.Drawing.SizeF(288F, 288F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        BackColor = System.Drawing.Color.Black;
        ClientSize = new System.Drawing.Size(1243, 720);
        Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        Name = "MainForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Timer CursorTimer;
    private System.Windows.Forms.Timer ProgressTimer;
}