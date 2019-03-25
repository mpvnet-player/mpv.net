using System;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace DynamicGUI
{
    public class HyperlinkEx : Hyperlink
    {
        private void HyperLinkEx_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }

        public void SetURL(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            NavigateUri = new Uri(url);
            RequestNavigate += HyperLinkEx_RequestNavigate;
            Inlines.Clear();
            Inlines.Add(url);
        }
    }
}