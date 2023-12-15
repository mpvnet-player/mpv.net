
// https://www.codeproject.com/Articles/5290638/Customizable-WPF-MessageBox

using System.ComponentModel;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace MpvNet.Windows.WPF.MsgBox;

public partial class MessageBoxEx : Window, INotifyPropertyChanged
{
    #region fields

    private static double screenWidth = SystemParameters.WorkArea.Width - 100;

    private static bool enableCloseButton = true;
    private static bool isSilent = false;
    private static List<string> installedFonts = new List<string>();
    public static MessageBoxButtonDefault staticButtonDefault;

    #endregion fields

    #region properties

    public static Color DefaultUrlForegroundColor => Colors.Blue;

    private static string? MsgBoxIconToolTip { get; set; }

    protected static MsgBoxExDelegate? DelegateObj { get; set; }
    protected static bool ExitAfterErrorAction { get; set; }

    public static ContentControl? ParentWindow { get; set; }

    public static string? ButtonTemplateName { get; set; }

    public static Brush? MessageBackground { get; set; }

    public static Brush? MessageForeground { get; set; }

    public static Brush? ButtonBackground { get; set; }

    public static double MaxFormWidth { get; set; } = screenWidth;

    public static Visibility ShowDetailsBtn { get; set; } = Visibility.Collapsed;

    public static string? DetailsText { get; set; }

    public static Visibility ShowCheckBox { get; set; } = Visibility.Collapsed;

    public static MsgBoxExCheckBoxData? CheckBoxData { get; set; } = null;

    public static FontFamily MsgFontFamily { get; set; } = new FontFamily("Segoe UI");

    public static double MsgFontSize { get; set; } = 12;

    public static Uri? Url { get; set; } = null;

    public static Visibility ShowUrl { get; set; } = Visibility.Collapsed;

    public static string? UrlDisplayName { get; set; } = null;

    public static SolidColorBrush UrlForeground { get; set; } = new SolidColorBrush(DefaultUrlForegroundColor);

    public static string? DelegateToolTip { get; set; }

    #endregion properties

    #region methods

    public static void SetFont(string familyName) => MsgFontFamily = new FontFamily(familyName);

    public static MessageBoxResult OpenMessageBox(
        string msg, string title, MessageBoxButton buttons, MessageBoxImage image)
    {
        MessageBoxEx window = new MessageBoxEx(msg, title, buttons, image);
        SetOwner(window);
        window.ShowDialog();
        return window.MessageResult;
    }

    public static MessageBoxResultEx OpenMessageBox(
        string msg,
        string title,
        MessageBoxButtonEx buttons,
        MessageBoxImage image)
    {
        MessageBoxEx window = new MessageBoxEx(msg, title, buttons, image);
        SetOwner(window);
        window.ShowDialog();
        return window.MessageResultEx;
    }

    public static void SetOwner(Window window)
    {
        IntPtr parentHandle = GetParentHandle();

        if (parentHandle != IntPtr.Zero)
            new WindowInteropHelper(window).Owner = parentHandle;
    }

    public static IntPtr GetParentHandle()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        GetWindowThreadProcessId(foregroundWindow, out var procID);

        using (var proc = Process.GetCurrentProcess())
            if (proc.Id == procID)
                return foregroundWindow;

        return IntPtr.Zero;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    public static Color ColorFromString(string colorString)
    {
        Color wpfColor = Colors.Black;

        try {
            wpfColor = (Color)ColorConverter.ConvertFromString(colorString);
        } catch (Exception) { }

        return wpfColor;
    }

    public static void SetFont()
    {
        MsgFontFamily = Application.Current.MainWindow.FontFamily;
        MsgFontSize = Application.Current.MainWindow.FontSize;
    }

    public static void SetFont(ContentControl parent)
    {
        MsgFontFamily = parent.FontFamily;
        MsgFontSize = parent.FontSize;
    }

    public static void SetFont(string familyName, double size)
    {
        if (!IsFontFamilyValid(familyName))
            if (!string.IsNullOrEmpty(familyName))
                MsgFontFamily = new FontFamily(familyName);
        MsgFontSize = Math.Max(1.0, size);
    }

    private static bool IsFontFamilyValid(string name)
    {
        if (installedFonts.Count == 0)
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
                installedFonts = (from x in fontsCollection.Families select x.Name).ToList();
        return installedFonts.Contains(name);
    }

    public static void SetButtonTemplateName(string name)
    {
        ButtonTemplateName = name;
    }

    public static void SetMaxFormWidth(double value)
    {
        MaxFormWidth = Math.Max(value, 300);
        double minWidth = 300;
        MaxFormWidth = Math.Max(minWidth, Math.Min(value, screenWidth));
    }

    public static void ResetToDefaults()
    {
        MsgFontSize = 12d;
        MsgFontFamily = new FontFamily("Segoe UI");
        DelegateObj = null;
        DetailsText = null;
        MessageForeground = null;
        MessageBackground = null;
        ButtonBackground = null;
        ParentWindow = null;
        isSilent = false;
        enableCloseButton = true;
        ButtonTemplateName = null;
        MsgBoxIconToolTip = null;
        ShowCheckBox = Visibility.Collapsed;
        CheckBoxData = null;
        ExitAfterErrorAction = false;
        MaxFormWidth = 800;
        Url = null;
        ShowUrl = Visibility.Collapsed;
        UrlDisplayName = null;
        UrlForeground = new SolidColorBrush(DefaultUrlForegroundColor);
        staticButtonDefault = MessageBoxButtonDefault.Forms;
    }

    public static void EnableCloseButton(bool enable)
    {
        enableCloseButton = enable;
    }

    public static void SetAsSilent(bool quiet)
    {
        isSilent = quiet;
    }

    public static void SetDefaultButton(MessageBoxButtonDefault buttonDefault)
    {
        staticButtonDefault = buttonDefault;
    }

    #endregion methods
}
