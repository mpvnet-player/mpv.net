
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

public class Msg
{
    private static string ShownMessages;

    public static string SupportURL { get; set; }

    public static void Show(string mainInstruction, string content = null)
    {
        Msg.Show(mainInstruction, content, MsgIcon.Info, MsgButtons.Ok, MsgResult.None);
    }

    public static void ShowError(string mainInstruction, string content = null)
    {
        try
        {
            using (TaskDialog<string> td = new TaskDialog<string>())
            {
                td.AllowCancel = false;

                if (string.IsNullOrEmpty(content))
                {
                    if (mainInstruction.Length < 80)
                        td.MainInstruction = mainInstruction;
                    else
                        td.Content = mainInstruction;
                }
                else
                {
                    td.MainInstruction = mainInstruction;
                    td.Content = content;
                }

                td.MainIcon = MsgIcon.Error;
                td.Footer = "[Copy Message](copymsg)";

                if (!string.IsNullOrEmpty(Msg.SupportURL))
                    td.Footer += $"   [Contact Support]({SupportURL})";

                td.Show();
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.GetType().Name + "\n\n" + e.Message + "\n\n" + e,
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void ShowException(Exception exception)
    {
        try
        {
            using (TaskDialog<string> td = new TaskDialog<string>())
            {
                td.MainInstruction = exception.GetType().Name;
                td.Content = exception.Message;
                td.MainIcon = MsgIcon.Error;
                td.ExpandedInformation = exception.ToString();
                td.Footer = "[Copy Message](copymsg)";

                if (!string.IsNullOrEmpty(Msg.SupportURL))
                    td.Footer += $"   [Contact Support]({SupportURL})";

                td.Show();
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.GetType().Name + "\n\n" + e.Message + "\n\n" + e,
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void ShowWarning(string mainInstruction,
                                   string content = null,
                                   bool onlyOnce = false)
    {
        if (onlyOnce && Msg.ShownMessages != null &&
            Msg.ShownMessages.Contains(mainInstruction + content))
            return;

        Msg.Show(mainInstruction, content, MsgIcon.Warning, MsgButtons.Ok, MsgResult.None);
        if (!onlyOnce) return;
        Msg.ShownMessages += mainInstruction + content;
    }

    public static MsgResult ShowQuestion(string mainInstruction,
                                         MsgButtons buttons = MsgButtons.OkCancel)
    {
        return Msg.Show(mainInstruction, null, MsgIcon.None, buttons, MsgResult.None);
    }

    public static MsgResult ShowQuestion(string mainInstruction,
                                         string content,
                                         MsgButtons buttons = MsgButtons.OkCancel)
    {
        return Msg.Show(mainInstruction, content, MsgIcon.None, buttons, MsgResult.None);
    }

    public static MsgResult Show(string mainInstruction,
                                 string content,
                                 MsgIcon icon,
                                 MsgButtons buttons,
                                 MsgResult defaultButton = MsgResult.None)
    {
        try
        {
            using (TaskDialog<MsgResult> td = new TaskDialog<MsgResult>())
            {
                td.AllowCancel = false;
                td.DefaultButton = defaultButton;
                td.MainIcon = icon;

                if (content == null)
                {
                    if (mainInstruction.Length < 80)
                        td.MainInstruction = mainInstruction;
                    else
                        td.Content = mainInstruction;
                }
                else
                {
                    td.MainInstruction = mainInstruction;
                    td.Content = content;
                }
                if (buttons == MsgButtons.OkCancel)
                {
                    td.AddButton("OK", MsgResult.OK);
                    td.AddButton("Cancel", MsgResult.Cancel);
                }
                else
                    td.CommonButtons = buttons;
                return td.Show();
            }
        }
        catch (Exception e)
        {
            return (MsgResult)MessageBox.Show(e.GetType().Name + "\n\n" + e.Message + "\n\n" + e,
                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public class TaskDialog<T> : TaskDialogNative, IDisposable
{
    private Dictionary<int, T> IdValueDic;
    private Dictionary<int, string> IdTextDic;
    private List<int> CommandLinkShieldList;
    private IntPtr ButtonArray;
    private IntPtr RadioButtonArray;
    private List<TaskDialogNative.TASKDIALOG_BUTTON> Buttons;
    private List<TaskDialogNative.TASKDIALOG_BUTTON> RadioButtons;
    private TaskDialogNative.TASKDIALOGCONFIG Config;
    const int TDE_CONTENT = 0;
    const int TDE_EXPANDED_INFORMATION = 1;
    const int TDE_FOOTER = 2;
    const int TDE_MAIN_INSTRUCTION = 3;
    const int TDN_CREATED = 0;
    const int TDN_NAVIGATED = 1;
    const int TDN_BUTTON_CLICKED = 2;
    const int TDN_HYPERLINK_CLICKED = 3;
    const int TDN_TIMER = 4;
    const int TDN_DESTROYED = 5;
    const int TDN_RADIO_BUTTON_CLICKED = 6;
    const int TDN_DIALOG_CONSTRUCTED = 7;
    const int TDN_VERIFICATION_CLICKED = 8;
    const int TDN_HELP = 9;
    const int TDN_EXPANDO_BUTTON_CLICKED = 10;
    const int TDM_NAVIGATE_PAGE = 1125;
    const int TDM_CLICK_BUTTON = 1126;
    const int TDM_SET_MARQUEE_PROGRESS_BAR = 1127;
    const int TDM_SET_PROGRESS_BAR_STATE = 1128;
    const int TDM_SET_PROGRESS_BAR_RANGE = 1129;
    const int TDM_SET_PROGRESS_BAR_POS = 1130;
    const int TDM_SET_PROGRESS_BAR_MARQUEE = 1131;
    const int TDM_SET_ELEMENT_TEXT = 1132;
    const int TDM_CLICK_RADIO_BUTTON = 1134;
    const int TDM_ENABLE_BUTTON = 1135;
    const int TDM_ENABLE_RADIO_BUTTON = 1136;
    const int TDM_CLICK_VERIFICATION = 1137;
    const int TDM_UPDATE_ELEMENT_TEXT = 1138;
    const int TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE = 1139;
    const int TDM_UPDATE_ICON = 1140;
    private T SelectedValueValue;
    private string SelectedTextValue;
    private int TimeoutValue;
    private int ExitTickCount;
    private bool disposed;

    public TaskDialog()
    {
        IdValueDic = new Dictionary<int, T>();
        IdTextDic = new Dictionary<int, string>();
        CommandLinkShieldList = new List<int>();
        Buttons = new List<TaskDialogNative.TASKDIALOG_BUTTON>();
        RadioButtons = new List<TaskDialogNative.TASKDIALOG_BUTTON>();
        _SelectedID = -1;
        Config = new TaskDialogNative.TASKDIALOGCONFIG();
        Config.cbSize = (uint)Marshal.SizeOf(Config);
        Config.hwndParent = GetHandle();
        Config.hInstance = IntPtr.Zero;
        Config.dwFlags = TaskDialogNative.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
        Config.dwCommonButtons = MsgButtons.None;
        Config.MainIcon = new TaskDialogNative.TASKDIALOGCONFIG_ICON_UNION(0);
        Config.FooterIcon = new TaskDialogNative.TASKDIALOGCONFIG_ICON_UNION(0);
        Config.cxWidth = 0U;
        Config.cButtons = 0U;
        Config.cRadioButtons = 0U;
        Config.pButtons = IntPtr.Zero;
        Config.pRadioButtons = IntPtr.Zero;
        Config.nDefaultButton = 0;
        Config.nDefaultRadioButton = 0;
        Config.pszWindowTitle = ((AssemblyProductAttribute)Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0]).Product;
        Config.pszMainInstruction = "";
        Config.pszContent = "";
        Config.pfCallback = new PFTASKDIALOGCALLBACK(this.DialogProc);
    }

    public IntPtr GetHandle()
    {
        StringBuilder lpszFileName = new StringBuilder(260);
        IntPtr foregroundWindow = TaskDialogNative.GetForegroundWindow();
        TaskDialogNative.GetWindowModuleFileName(foregroundWindow, lpszFileName, 260U);

        if (Path.GetFileName(lpszFileName.ToString().Replace(".vshost", "")) ==
            Path.GetFileName(Assembly.GetEntryAssembly().Location))

            return foregroundWindow;

        return IntPtr.Zero;
    }

    public bool AllowCancel {
        set {
            if (value)
                Config.dwFlags |= TaskDialogNative.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
            else
                Config.dwFlags ^= TaskDialogNative.TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION;
        }
    }

    public string MainInstruction {
        get => Config.pszMainInstruction;
        set => Config.pszMainInstruction = value;
    }

    public string Content {
        get => Config.pszContent;
        set => Config.pszContent = ExpandMarkdownMarkup(value);
    }

    public string ExpandedInformation {
        get => Config.pszExpandedInformation;
        set => Config.pszExpandedInformation = ExpandMarkdownMarkup(value);
    }

    public string VerificationText {
        get => Config.pszVerificationText;
        set => Config.pszVerificationText = value;
    }

    public MsgResult DefaultButton {
        get => (MsgResult)Config.nDefaultButton;
        set => Config.nDefaultButton = (int)value;
    }

    public string Footer {
        get => Config.pszFooter;
        set => Config.pszFooter = ExpandMarkdownMarkup(value);
    }

    public MsgIcon MainIcon {
        set => Config.MainIcon = new TaskDialogNative.TASKDIALOGCONFIG_ICON_UNION((int)value);
    }

    private int _SelectedID;

    public int SelectedID {
        get => _SelectedID;
        set {
            foreach (var i in IdValueDic)
                if (i.Key == value) _SelectedID = value;
        }
    }

    public T SelectedValue {
        get {
            if (IdValueDic.ContainsKey(SelectedID))
                return IdValueDic[SelectedID];
            return SelectedValueValue;
        }
        set => SelectedValueValue = value;
    }

    public string SelectedText {
        get {
            if (IdTextDic.ContainsKey(SelectedID))
                return IdTextDic[SelectedID];
            return SelectedTextValue;
        }
        set => SelectedTextValue = value;
    }

    public bool CheckBoxChecked {
        get => (Config.dwFlags & TaskDialogNative.TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED) == TaskDialogNative.TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED;
        set {
            if (value)
                Config.dwFlags |= TaskDialogNative.TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED;
            else
                Config.dwFlags ^= TaskDialogNative.TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED;
        }
    }

    public MsgButtons CommonButtons {
        get => Config.dwCommonButtons;
        set => Config.dwCommonButtons = value;
    }

    public int Timeout {
        get => Convert.ToInt32(TimeoutValue / 1000.0);
        set {
            TimeoutValue = value * 1000;
            Config.dwFlags |= TaskDialogNative.TASKDIALOG_FLAGS.TDF_CALLBACK_TIMER;
        }
    }

    public void AddButton(string text, T value)
    {
        int n = 1000 + IdValueDic.Count + 1;
        IdValueDic[n] = value;
        Buttons.Add(new TaskDialogNative.TASKDIALOG_BUTTON(n, text));
    }

    public string ExpandMarkdownMarkup(string value)
    {
        if (value.Contains("["))
        {
            Regex regex = new Regex(@"\[(.+)\]\((.+)\)");

            if (regex.Match(value).Success)
            {
                Config.dwFlags |= TaskDialogNative.TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS;
                value = regex.Replace(value, "<a href=\"$2\">$1</a>");
            }
        }
        return value;
    }

    public void AddCommandLink(string text, T value)
    {
        int n = 1000 + IdValueDic.Count + 1;
        IdValueDic[n] = value;
        IdTextDic[n] = text;
        Buttons.Add(new TaskDialogNative.TASKDIALOG_BUTTON(n, text));
        Config.dwFlags |= TaskDialogNative.TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS;
    }

    public void AddCommandLink(string text, string description, T value, bool setShield = false)
    {
        int n = 1000 + IdValueDic.Count + 1;
        IdValueDic[n] = value;
        if (setShield) CommandLinkShieldList.Add(n);
        if (!string.IsNullOrEmpty(description)) text += "\n" + description;
        Buttons.Add(new TaskDialogNative.TASKDIALOG_BUTTON(n, text));
        Config.dwFlags |= TaskDialogNative.TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS;
    }

    public void AddRadioButton(string text, T value)
    {
        int n = 1000 + IdValueDic.Count + 1;
        IdValueDic[n] = value;
        RadioButtons.Add(new TaskDialogNative.TASKDIALOG_BUTTON(n, text));
    }

    public T Show()
    {
        MarshalDialogControlStructs();
        TaskDialogNative.TASKDIALOGCONFIG config = Config;
        int errorCode = TaskDialogNative.TaskDialogIndirect(config, out int dummy1, out int dummy2, out bool isChecked);
        if (errorCode < 0) Marshal.ThrowExceptionForHR(errorCode);
        CheckBoxChecked = isChecked;
        if (SelectedValue is MsgResult) SelectedValue = (T)(object)SelectedID;
        return SelectedValue;
    }

    public int DialogProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr lpRefData)
    {
        switch (msg)
        {
            case 0: //TDN_CREATED
                foreach (var i in CommandLinkShieldList)
                    SendMessage(hwnd, TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE, new IntPtr(i), new IntPtr(1));
                break;
            case 2: //TDN_BUTTON_CLICKED
            case 6: //TDN_RADIO_BUTTON_CLICKED
                if (SelectedValue is MsgResult)
                    _SelectedID = wParam.ToInt32();
                else
                    SelectedID = wParam.ToInt32();
                break;
            case 3: //TDN_HYPERLINK_CLICKED
                string stringUni = Marshal.PtrToStringUni(lParam);
                if (stringUni.StartsWith("mailto") || stringUni.StartsWith("http"))
                    Process.Start(stringUni);
                if (stringUni == "copymsg")
                {
                    Thread thread = new Thread((ThreadStart)(() => {
                        Clipboard.SetText(MainInstruction + "\r\n\r\n" + Content + "\r\n\r\n" + ExpandedInformation);
                        MessageBox.Show("Message was copied to clipboard.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                break;
            case 4: //TDN_TIMER
                if (ExitTickCount == 0) ExitTickCount = Environment.TickCount + Timeout * 1000;
                if (Environment.TickCount > ExitTickCount)
                    TaskDialogNative.SendMessage(hwnd, 1126, new IntPtr(1), IntPtr.Zero);
                break;
        }
        return 0;
    }

    public void MarshalDialogControlStructs()
    {
        if (Buttons != null && Buttons.Count > 0)
        {
            ButtonArray = TaskDialog<T>.AllocateAndMarshalButtons(Buttons);
            Config.pButtons = ButtonArray;
            Config.cButtons = (uint)Buttons.Count;
        }

        if (RadioButtons == null || RadioButtons.Count <= 0) return;
        RadioButtonArray = TaskDialog<T>.AllocateAndMarshalButtons(RadioButtons);
        Config.pRadioButtons = RadioButtonArray;
        Config.cRadioButtons = (uint)RadioButtons.Count;
    }

    public static IntPtr AllocateAndMarshalButtons(List<TaskDialogNative.TASKDIALOG_BUTTON> structs)
    {
        var initialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TASKDIALOG_BUTTON)) * structs.Count);
        var currentPtr = initialPtr;

        foreach (var button in structs)
        {
            Marshal.StructureToPtr(button, currentPtr, false);
            currentPtr = (IntPtr)(currentPtr.ToInt64() + Marshal.SizeOf(button));
        }

        return initialPtr;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~TaskDialog()
    {
        Dispose(false);
    }

    protected void Dispose(bool disposing)
    {
        if (disposed) return;
        disposed = true;

        if (ButtonArray != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(ButtonArray);
            ButtonArray = IntPtr.Zero;
        }

        if (RadioButtonArray != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(RadioButtonArray);
            RadioButtonArray = IntPtr.Zero;
        }
    }
}

public delegate int PFTASKDIALOGCALLBACK(
    IntPtr hwnd,
    uint msg,
    IntPtr wParam,
    IntPtr lParam,
    IntPtr lpRefData);

public class TaskDialogNative
{
    [DllImport("comctl32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int TaskDialogIndirect(
        [In] TaskDialogNative.TASKDIALOGCONFIG pTaskConfig,
        out int pnButton,
        out int pnRadioButton,
        [MarshalAs(UnmanagedType.Bool)] out bool pVerificationFlagChecked);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern uint GetWindowModuleFileName(
        IntPtr hwnd,
        StringBuilder lpszFileName,
        uint cchFileNameMax);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(
        IntPtr handle,
        int message,
        IntPtr wParam,
        IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public class TASKDIALOGCONFIG
    {
        public uint cbSize;
        public IntPtr hwndParent;
        public IntPtr hInstance;
        public TaskDialogNative.TASKDIALOG_FLAGS dwFlags;
        public MsgButtons dwCommonButtons;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszWindowTitle;
        public TaskDialogNative.TASKDIALOGCONFIG_ICON_UNION MainIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszMainInstruction;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszContent;
        public uint cButtons;
        public IntPtr pButtons;
        public int nDefaultButton;
        public uint cRadioButtons;
        public IntPtr pRadioButtons;
        public int nDefaultRadioButton;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszVerificationText;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszExpandedInformation;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszExpandedControlText;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszCollapsedControlText;
        public TaskDialogNative.TASKDIALOGCONFIG_ICON_UNION FooterIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszFooter;
        public PFTASKDIALOGCALLBACK pfCallback;
        public IntPtr lpCallbackData;
        public uint cxWidth;
    }

    public enum TASKDIALOG_FLAGS
    {
        NONE = 0,
        TDF_ENABLE_HYPERLINKS = 1,
        TDF_USE_HICON_MAIN = 2,
        TDF_USE_HICON_FOOTER = 4,
        TDF_ALLOW_DIALOG_CANCELLATION = 8,
        TDF_USE_COMMAND_LINKS = 16,
        TDF_USE_COMMAND_LINKS_NO_ICON = 32,
        TDF_EXPAND_FOOTER_AREA = 64,
        TDF_EXPANDED_BY_DEFAULT = 128,
        TDF_VERIFICATION_FLAG_CHECKED = 256,
        TDF_SHOW_PROGRESS_BAR = 512,
        TDF_SHOW_MARQUEE_PROGRESS_BAR = 1024,
        TDF_CALLBACK_TIMER = 2048,
        TDF_POSITION_RELATIVE_TO_WINDOW = 4096,
        TDF_RTL_LAYOUT = 8192,
        TDF_NO_DEFAULT_RADIO_BUTTON = 16384,
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct TASKDIALOGCONFIG_ICON_UNION
    {
        [FieldOffset(0)]
        public int hMainIcon;
        [FieldOffset(0)]
        public int pszIcon;
        [FieldOffset(0)]
        public IntPtr spacer;

        public TASKDIALOGCONFIG_ICON_UNION(int i)
        {
            this = new TaskDialogNative.TASKDIALOGCONFIG_ICON_UNION();
            spacer = IntPtr.Zero;
            pszIcon = 0;
            hMainIcon = i;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct TASKDIALOG_BUTTON
    {
        public int nButtonID;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszButtonText;

        public TASKDIALOG_BUTTON(int n, string txt)
        {
            this = new TaskDialogNative.TASKDIALOG_BUTTON();
            nButtonID = n;
            pszButtonText = txt;
        }
    }
}

public enum MsgButtons
{
    None = 0,
    Ok = 1,
    Yes = 2,
    No = 4,
    YesNo = 6,
    Cancel = 8,
    OkCancel = 9,
    YesNoCancel = 14,
    Retry = 16,
    RetryCancel = 24,
    Close = 32,
}

public enum MsgResult
{
    None,
    OK,
    Cancel,
    Abort,
    Retry,
    Ignore,
    Yes,
    No,
}

public enum MsgIcon
{
    None = 0,
    SecurityShieldGray = 65527,
    SecuritySuccess = 65528,
    SecurityError = 65529,
    SecurityWarning = 65530,
    SecurityShieldBlue = 65531,
    Shield = 65532,
    Info = 65533,
    Error = 65534,
    Warning = 65535,
}