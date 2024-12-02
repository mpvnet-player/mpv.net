
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using MpvNet.Help;

namespace MpvNet.Windows.WPF.MsgBox;

public partial class MessageBoxEx : Window, INotifyPropertyChanged
{
    #region INotifyPropertyChanged

    private bool isModified = false;

    public bool IsModified {
        get { return isModified; }
        set {
            if (value != isModified)
            {
                isModified = true;
                NotifyPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName != "IsModified")
                IsModified = true;
        }
    }

    #endregion INotifyPropertyChanged

    #region fields

    private string? message;
    private string? messageTitle;
    private MessageBoxButton? buttons;
    private MessageBoxResult messageResult;
    private MessageBoxButtonEx? buttonsEx;
    private MessageBoxResultEx messageResultEx;
    private ImageSource? messageIcon;
    private MessageBoxImage msgBoxImage;
    private double buttonWidth = 0d;
    private bool expanded = false;
    private bool isDefaultOK;
    private bool isDefaultCancel;
    private bool isDefaultYes;
    private bool isDefaultNo;
    private bool isDefaultAbort;
    private bool isDefaultRetry;
    private bool isDefaultIgnore;

    private bool usingExButtons = false;

    #endregion fields

    #region properties

    public string Message {
        get { return message; }
        set {
            if (value != message)
            {
                message = value;
                NotifyPropertyChanged();
            }
        }
    }

    public string MessageTitle {
        get { return messageTitle; }
        set {
            if (value != messageTitle)
            {
                messageTitle = value;
                NotifyPropertyChanged();
            }
        }
    }

    public MessageBoxResult MessageResult { get { return this.messageResult; } set { this.messageResult = value; } }
    public MessageBoxResultEx MessageResultEx { get { return this.messageResultEx; } set { this.messageResultEx = value; } }

    public MessageBoxButton? Buttons {
        get { return buttons; }
        set {
            if (value != buttons)
            {
                buttons = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ShowOk");
                NotifyPropertyChanged("ShowCancel");
                NotifyPropertyChanged("ShowYes");
                NotifyPropertyChanged("ShowNo");
            }
        }
    }
    public MessageBoxButtonEx? ButtonsEx {
        get { return buttonsEx; }
        set {
            if (value != buttonsEx)
            {
                buttonsEx = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ShowOk");
                NotifyPropertyChanged("ShowCancel");
                NotifyPropertyChanged("ShowYes");
                NotifyPropertyChanged("ShowNo");
                NotifyPropertyChanged("ShowAbort");
                NotifyPropertyChanged("ShowRetry");
                NotifyPropertyChanged("ShowIgnore");
            }
        }
    }

    public Visibility ShowOk => (!usingExButtons && Buttons == MessageBoxButton.OK ||
        !usingExButtons && Buttons == MessageBoxButton.OKCancel ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.OK ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.OKCancel) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowCancel => (!usingExButtons && Buttons == MessageBoxButton.OKCancel ||
        !usingExButtons && Buttons == MessageBoxButton.YesNoCancel ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.OKCancel ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.YesNoCancel ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.RetryCancel) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowYes => (!usingExButtons && Buttons == MessageBoxButton.YesNo ||
        !usingExButtons && Buttons == MessageBoxButton.YesNoCancel ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.YesNo ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowNo => (!usingExButtons && Buttons == MessageBoxButton.YesNo ||
        !usingExButtons && Buttons == MessageBoxButton.YesNoCancel ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.YesNo ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowRetry => (usingExButtons && ButtonsEx == MessageBoxButtonEx.AbortRetryIgnore ||
        usingExButtons && ButtonsEx == MessageBoxButtonEx.RetryCancel) ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ShowAbort => (usingExButtons && ButtonsEx == MessageBoxButtonEx.AbortRetryIgnore)
        ? Visibility.Visible
        : Visibility.Collapsed;

    public Visibility ShowIgnore => (usingExButtons && ButtonsEx == MessageBoxButtonEx.AbortRetryIgnore)
        ? Visibility.Visible
        : Visibility.Collapsed;

    public Visibility ShowIcon => (MessageIcon != null) ? Visibility.Visible : Visibility.Collapsed;

    public ImageSource? MessageIcon {
        get => messageIcon;
        set {
            if (value != messageIcon)
            {
                messageIcon = value;
                NotifyPropertyChanged();
            }
        }
    }

    public double ButtonWidth {
        get => buttonWidth;
        set {
            if (value != buttonWidth)
            { 
                buttonWidth = value; 
                NotifyPropertyChanged();
            } 
        }
    }

    public bool Expanded {
        get => expanded;
        set {
            if (value != expanded)
            {
                expanded = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsDefaultOK {
        get => isDefaultOK;
        set {
            if (value != isDefaultOK)
            {
                isDefaultOK = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsDefaultCancel {
        get => isDefaultCancel;
        set {
            if (value != isDefaultCancel)
            {
                isDefaultCancel = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsDefaultYes {
        get => isDefaultYes;
        set {
            if (value != isDefaultYes)
            {
                isDefaultYes = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsDefaultNo {
        get => isDefaultNo;
        set {
            if (value != isDefaultNo)
            {
                isDefaultNo = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsDefaultAbort {
        get => isDefaultAbort;
        set {
            if (value != isDefaultAbort)
            {
                isDefaultAbort = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsDefaultRetry {
        get => isDefaultRetry;
        set {
            if (value != isDefaultRetry)
            { isDefaultRetry = value; NotifyPropertyChanged(); }
        }
    }

    public bool IsDefaultIgnore {
        get => isDefaultIgnore;
        set {
            if (value != isDefaultIgnore)
            {
                isDefaultIgnore = value;
                NotifyPropertyChanged();
            }
        }
    }


    #endregion properties

    #region constructors

    private MessageBoxEx()
    {
        InitializeComponent();
        DataContext = this;
        LargestButtonWidth();
    }

    public MessageBoxEx(string msg, string title, MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage image = MessageBoxImage.None)
    {
        InitializeComponent();
        DataContext = this;
        Init(msg, title, buttons, image);
    }

    public MessageBoxEx(string msg, string title, MessageBoxButtonEx buttons = MessageBoxButtonEx.OK,
        MessageBoxImage image = MessageBoxImage.None)
    {
        InitializeComponent();
        DataContext = this;
        Init(msg, title, buttons, image);
    }

    #endregion constructors

    #region non-static methods

    protected virtual void Init(string msg, string title, MessageBoxButton buttons, MessageBoxImage image)
    {
        InitTop(msg, title);
        usingExButtons = false;
        ButtonsEx = null;
        Buttons = buttons;
        SetButtonTemplates();
        InitBottom(image);
        FindDefaultButton(staticButtonDefault);
    }

    protected virtual void Init(string msg, string title, MessageBoxButtonEx buttons, MessageBoxImage image)
    {
        InitTop(msg, title);
        usingExButtons = true;
        Buttons = null;
        ButtonsEx = buttons;
        SetButtonTemplates();
        InitBottom(image);
        FindDefaultButtonEx(staticButtonDefault);
    }

    void InitTop(string msg, string title)
    {
        // determine whether or not to show the details pane and checkbox
        ShowDetailsBtn = (string.IsNullOrEmpty(DetailsText)) ? Visibility.Collapsed : Visibility.Visible;
        ShowCheckBox = (CheckBoxData == null) ? Visibility.Collapsed : Visibility.Visible;

        // Well, the binding for family/size don't appear to be working, so I have to set them 
        // manually. Weird...
        FontFamily = MsgFontFamily;
        FontSize = MsgFontSize;
        LargestButtonWidth();

        // configure the form based on specified criteria
        Message = msg;
        MessageTitle = (string.IsNullOrEmpty(title.Trim())) ? "Application Message" : title;

        // url (if specified)
        if (Url != null)
        {
            tbUrl.Text = (string.IsNullOrEmpty(UrlDisplayName)) ? Url.ToString() : UrlDisplayName;
            tbUrl.ToolTip = new ToolTip() { Content = Url.ToString() };
        }
    }

    void InitBottom(MessageBoxImage image)
    {
        MessageBackground = (MessageBackground == null) ? new SolidColorBrush(Colors.White) : MessageBackground;
        MessageForeground = (MessageForeground == null) ? new SolidColorBrush(Colors.Black) : MessageForeground;
        ButtonBackground = (ButtonBackground == null) ? new SolidColorBrush(ColorFromString("#cdcdcd")) : ButtonBackground;

        MessageIcon = null;

        msgBoxImage = image;

        if (DelegateObj != null)
        {
            Style style = (Style)(FindResource("ImageOpacityChanger"));

            if (style != null)
            {
                imgMsgBoxIcon.Style = style;

                if (!string.IsNullOrEmpty(DelegateToolTip))
                {
                    System.Windows.Controls.ToolTip tooltip = new System.Windows.Controls.ToolTip() { Content = DelegateToolTip };
                    // for some reason, Image elements can't do tooltips, so I assign the tootip 
                    // to the parent grid. This seems to work fine.
                    imgGrid.ToolTip = tooltip;
                }
            }
        }

        // multiple images have the same ordinal value, and are indicated in the comments below. 
        // WTF Microsoft? 
        switch ((int)image)
        {
            case 16: // MessageBoxImage.Error, MessageBoxImage.Stop, MessageBox.Image.Hand
                {
                    MessageIcon = GetIcon(SystemIcons.Error);

                    if (!isSilent)
                        SystemSounds.Hand.Play();
                }
                break;

            case 64: // MessageBoxImage.Information, MessageBoxImage.Asterisk 
                {
                    MessageIcon = GetIcon(SystemIcons.Information);

                    if (!isSilent)
                        SystemSounds.Asterisk.Play();
                }
                break;

            case 32: // MessageBoxImage.Question
                {
                    MessageIcon = GetIcon(SystemIcons.Question);

                    if (!isSilent)
                        SystemSounds.Question.Play();
                }
                break;

            case 48: // MessageBoxImage.Warning, MessageBoxImage.Exclamation
                {
                    MessageIcon = GetIcon(SystemIcons.Warning);

                    if (!isSilent)
                        SystemSounds.Exclamation.Play();
                }
                break;

            default:
                MessageIcon = null;
                break;
        }
    }

    public ImageSource GetIcon(Icon icon)
    {
        BitmapSource image = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        return image;
    }

    protected virtual void CenterInScreen()
    {
        double width = ActualWidth;
        double height = ActualHeight;
        Left = (SystemParameters.WorkArea.Width - width) / 2 + SystemParameters.WorkArea.Left;
        Top = (SystemParameters.WorkArea.Height - height) / 2 + SystemParameters.WorkArea.Top;
    }

    protected void LargestButtonWidth()
    {
        Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

        StackPanel panel = (StackPanel)stackButtons.Child;
        double width = 0;
        string largestName = string.Empty;
        foreach (System.Windows.Controls.Button button in panel.Children)
        {
            // Using the FormattedText object 
            // will strip whitespace before measuring the text, so we convert spaces to double 
            // hyphens to compensate (I like to pad button Content with a leading and trailing 
            // space) so that the button is wide enough to present a more padded appearance.
            FormattedText formattedText = new FormattedText(
                (button.Name == "btnDetails") ? "--Details--" : ((string)(button.Content)).Replace(" ", "--"),
                CultureInfo.CurrentUICulture,
                System.Windows.FlowDirection.LeftToRight,
                typeface,
                FontSize = FontSize,
                System.Windows.Media.Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            if (width < formattedText.Width)
                largestName = button.Name;

            width = Math.Max(width, formattedText.Width);
        }
        ButtonWidth = Math.Ceiling(width/*width + polyArrow.Width+polyArrow.Margin.Right+Margin.Left*/);
    }

    void SetButtonTemplates()
    {
        // set the button template (if specified)
        if (!string.IsNullOrEmpty(ButtonTemplateName))
        {
            bool foundResource = true;

            try
            {
                FindResource(ButtonTemplateName);
            }
            catch (Exception)
            {
                foundResource = false;
            }

            if (foundResource)
            {
                btnOK.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
                btnYes.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
                btnNo.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
                btnCancel.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
                btnAbort.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
                btnRetry.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
                btnIgnore.SetResourceReference(Control.TemplateProperty, ButtonTemplateName);
            }
        }
    }

    void FindDefaultButtonEx(MessageBoxButtonDefault buttonDefault)
    {
        // determine default button
        IsDefaultOK = false;
        IsDefaultCancel = false;
        IsDefaultYes = false;
        IsDefaultNo = false;
        IsDefaultAbort = false;
        IsDefaultRetry = false;
        IsDefaultIgnore = false;

        if (buttonDefault != MessageBoxButtonDefault.None)
        {
            switch (ButtonsEx)
            {
                case MessageBoxButtonEx.OK:
                    IsDefaultOK = true;
                    break;
                case MessageBoxButtonEx.OKCancel:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.OK:
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultOK = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.Cancel:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultCancel = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultOK = true;
                                break;
                        }
                    }
                    break;
                case MessageBoxButtonEx.YesNoCancel:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.Yes:
                                break;
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultYes = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.No:
                                IsDefaultNo = true;
                                break;
                            case MessageBoxButtonDefault.Button3:
                            case MessageBoxButtonDefault.Cancel:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultCancel = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultYes = true;
                                break;
                        }
                    }
                    break;
                case MessageBoxButtonEx.YesNo:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.Yes:
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultYes = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.No:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultNo = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultYes = true;
                                break;
                        }
                    }
                    break;
                case MessageBoxButtonEx.RetryCancel:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.Retry:
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultRetry = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.Cancel:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultCancel = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultRetry = true;
                                break;
                        }
                    }
                    break;
                case MessageBoxButtonEx.AbortRetryIgnore:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.Abort:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultAbort = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.Retry:
                                IsDefaultRetry = true;
                                break;
                            case MessageBoxButtonDefault.Button3:
                            case MessageBoxButtonDefault.Ignore:
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultIgnore = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultAbort = true;
                                break;
                        }
                    }
                    break;
            }
        }
    }

    void FindDefaultButton(MessageBoxButtonDefault buttonDefault)
    {
        // determine default button
        IsDefaultOK = false;
        IsDefaultCancel = false;
        IsDefaultYes = false;
        IsDefaultNo = false;
        IsDefaultAbort = false;
        IsDefaultRetry = false;
        IsDefaultIgnore = false;

        if (buttonDefault != MessageBoxButtonDefault.None)
        {
            switch (Buttons)
            {
                case MessageBoxButton.OK:
                    IsDefaultOK = true;
                    break;
                case MessageBoxButton.OKCancel:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.OK:
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultOK = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.Cancel:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultCancel = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultOK = true;
                                break;
                        }
                    }
                    break;
                case MessageBoxButton.YesNoCancel:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.Yes:
                                break;
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultYes = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.No:
                                IsDefaultNo = true;
                                break;
                            case MessageBoxButtonDefault.Button3:
                            case MessageBoxButtonDefault.Cancel:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultCancel = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultYes = true;
                                break;
                        }
                    }
                    break;
                case MessageBoxButton.YesNo:
                    {
                        switch (buttonDefault)
                        {
                            case MessageBoxButtonDefault.Button1:
                            case MessageBoxButtonDefault.Yes:
                            case MessageBoxButtonDefault.MostPositive:
                                IsDefaultYes = true;
                                break;
                            case MessageBoxButtonDefault.Button2:
                            case MessageBoxButtonDefault.No:
                            case MessageBoxButtonDefault.LeastPositive:
                                IsDefaultNo = true;
                                break;
                            case MessageBoxButtonDefault.Forms:
                            default:
                                IsDefaultYes = true;
                                break;
                        }
                    }
                    break;
            }
        }
    }

    #endregion non-static methods

    #region event handlers

    #region buttons

    /// <summary>
    /// Handle the click event for the OK button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnOK_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.OK;
        this.MessageResultEx = MessageBoxResultEx.OK;
        this.DialogResult = true;
    }

    /// <summary>
    /// Handle the click event for the Yes button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnYes_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.Yes;
        this.MessageResultEx = MessageBoxResultEx.Yes;
        this.DialogResult = true;
    }

    /// <summary>
    /// Handle the click event for the No button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnNo_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.No;
        this.MessageResultEx = MessageBoxResultEx.No;
        this.DialogResult = true;
    }

    void BtnAbort_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.None;
        this.MessageResultEx = MessageBoxResultEx.Abort;
        this.DialogResult = true;
    }

    void BtnRetry_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.None;
        this.MessageResultEx = MessageBoxResultEx.Retry;
        this.DialogResult = true;
    }

    void BtnIgnore_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.None;
        this.MessageResultEx = MessageBoxResultEx.Ignore;
        this.DialogResult = true;
    }

    /// <summary>
    /// Handle the click event for the Cancel button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.MessageResult = MessageBoxResult.Cancel;
        this.MessageResultEx = MessageBoxResultEx.Cancel;
        this.DialogResult = true;
    }

    #endregion buttons

    void NotifiableWindow_SizeChanged(object sender, SizeChangedEventArgs e) => CenterInScreen();

    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        imgMsgBoxIcon.ToolTip = (msgBoxImage == MessageBoxImage.Error) ? MsgBoxIconToolTip : null;
    }

    void Window_Closing(object sender, CancelEventArgs e)
    {
        DetailsText = null;
        CheckBoxData = null;
        staticButtonDefault = MessageBoxButtonDefault.Forms;

        if (MessageResult == MessageBoxResult.None)
        {
            if (usingExButtons)
            {
                switch (ButtonsEx)
                {
                    case MessageBoxButtonEx.OK:
                        MessageResultEx = MessageBoxResultEx.OK;
                        break;
                    case MessageBoxButtonEx.YesNoCancel:
                    case MessageBoxButtonEx.OKCancel:
                    case MessageBoxButtonEx.RetryCancel:
                    case MessageBoxButtonEx.AbortRetryIgnore:
                        MessageResultEx = MessageBoxResultEx.Cancel;
                        break;
                    case MessageBoxButtonEx.YesNo:
                        MessageResultEx = MessageBoxResultEx.No;
                        break;
                }
            }
            else
            {
                switch (Buttons)
                {
                    case MessageBoxButton.OK:
                        MessageResult = MessageBoxResult.OK;
                        break;
                    case MessageBoxButton.YesNoCancel:
                    case MessageBoxButton.OKCancel:
                        MessageResult = MessageBoxResult.Cancel;
                        break;
                    case MessageBoxButton.YesNo:
                        MessageResult = MessageBoxResult.No;
                        break;
                }
            }
        }
    }

    void ImgMsgBoxIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DelegateObj != null && msgBoxImage == MessageBoxImage.Error && Buttons == MessageBoxButton.OK)
        {
            DelegateObj.PerformAction(Message);

            if (ExitAfterErrorAction)
            {
                MessageResult = MessageBoxResult.None;
                DialogResult = true;
            }
        }
    }

    void TbUrl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => ProcessHelp.ShellExecute(Url?.ToString());

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    private const uint MF_BYCOMMAND = 0x00000000;
    private const uint MF_GRAYED = 0x00000001;
    private const uint SC_CLOSE = 0xF060;

    void Window_SourceInitialized(object sender, EventArgs e)
    {
        if (!enableCloseButton)
        {
            var hWnd = new WindowInteropHelper(this);
            var sysMenu = GetSystemMenu(hWnd.Handle, false);
            EnableMenuItem(sysMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }
    }

    #endregion event handlers
}
