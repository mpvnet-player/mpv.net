
using System.Globalization;

namespace NGettext.Wpf
{
    public class CultureEventArgs : EventArgs
    {
        public CultureEventArgs(CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException(nameof(cultureInfo));
            CultureInfo = cultureInfo;
        }

        public CultureInfo CultureInfo { get; }
    }
}
