
using System.Windows.Documents;
using System.Windows.Navigation;

using MpvNet.Help;

namespace MpvNet.Windows.WPF;

public class HyperlinkEx : Hyperlink
{
    void HyperLinkEx_RequestNavigate(object sender, RequestNavigateEventArgs e) =>
        ProcessHelp.ShellExecute(e.Uri.AbsoluteUri);

    public void SetURL(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        NavigateUri = new Uri(url);
        RequestNavigate += HyperLinkEx_RequestNavigate;
        Inlines.Clear();
        Inlines.Add("Manual");
    }
}
