
using System.Windows.Media;

namespace MpvNet.Windows.WPF.MsgBox;

public class MsgBoxUrl
{
    public Uri? URL { get; set; }
    public string? DisplayName { get; set; }
    public Color Foreground { get; set; }

    public MsgBoxUrl() => Foreground = MessageBoxEx.DefaultUrlForegroundColor;
}
