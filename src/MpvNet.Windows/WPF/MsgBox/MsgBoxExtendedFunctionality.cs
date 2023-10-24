
namespace MpvNet.Windows.WPF.MsgBox;

public class MsgBoxExtendedFunctionality
{
    public MessageBoxButtonDefault ButtonDefault { get; set; }
    public string? DetailsText { get; set; }
    public MsgBoxExCheckBoxData? CheckBoxData { get; set; }
    public MsgBoxExDelegate? MessageDelegate { get; set; }
    public bool ExitAfterAction { get; set; }
    public string DelegateToolTip { get; set; }

    public MsgBoxUrl? URL { get; set; }

    public MsgBoxExtendedFunctionality()
    {
        ButtonDefault = MessageBoxButtonDefault.Forms;
        DetailsText = null;
        CheckBoxData = null;
        MessageDelegate = null;
        URL = null;
        DelegateToolTip = "Click this icon for additional info/actions.";
    }
}
