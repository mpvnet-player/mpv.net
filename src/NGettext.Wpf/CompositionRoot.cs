
using System.Globalization;

using NGettext.Wpf.Common;
using NGettext.Wpf.EnumTranslation;

namespace NGettext.Wpf
{
    public static class CompositionRoot
    {
        public static void Compose(string domainName, CultureInfo cultureInfo, string localeFolder)
        {
            var cultureTracker = new CultureTracker();
            cultureTracker.CurrentCulture = cultureInfo;
            var localizer = new Localizer(cultureTracker, domainName, localeFolder);
            ChangeCultureCommand.CultureTracker = cultureTracker;
            GettextExtension.Localizer = localizer;
            TrackCurrentCultureBehavior.CultureTracker = cultureTracker;
            LocalizeEnumConverter.EnumLocalizer = new EnumLocalizer(localizer);
            Translation.Localizer = localizer;
            GettextStringFormatConverter.Localizer = localizer;
        }

        internal static void WriteMissingInitializationErrorMessage()
        {
            Console.Error.WriteLine("NGettext.Wpf: NGettext.Wpf.CompositionRoot.Compose() must be called at the entry point of the application for localization to work");
        }
    }
}