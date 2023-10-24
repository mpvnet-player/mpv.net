
namespace MpvNet.Windows.WPF.MsgBox;

public enum MessageBoxButtonEx { OK = 0, OKCancel, AbortRetryIgnore, YesNoCancel, YesNo, RetryCancel }

public enum MessageBoxResultEx { None = 0, OK, Cancel, Abort, Retry, Ignore, Yes, No }

public enum MessageBoxButtonDefault
{
    OK, Cancel, Yes, No, Abort, Retry, Ignore, // specific button
    Button1, Button2, Button3,                 // button by ordinal left-to-right position
    MostPositive, LeastPositive,               // button by positivity
    Forms,                                     // button according to the Windows.Forms standard messagebox
    None                                       // no default button
}
