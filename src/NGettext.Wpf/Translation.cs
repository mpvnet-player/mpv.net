
using System.Globalization;

namespace NGettext.Wpf
{
    public static class Translation
    {
        public static string _(string msgId) => Localizer?.Gettext(msgId) ?? "";

        public static string _(string msgId, params object[] parameters)
        {
            return parameters.Any()
                ? Localizer?.Gettext(msgId, parameters) ?? ""
                : Localizer?.Gettext(msgId) ?? "";
        }

        public static ILocalizer? Localizer { get; set; }

        public static string Noop(string msgId) => msgId;

        [Obsolete("Use GetPluralString() instead.  This method will be removed in 2.x")]
        public static string PluralGettext(int n, string singularMsgId, string pluralMsgId, params object[] @params)
        {
            return GetPluralString(singularMsgId, pluralMsgId, n, @params);
        }

        public static string GetPluralString(string singularMsgId, string pluralMsgId, int n, params object[] args)
        {
            if (Localizer is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return string.Format(CultureInfo.InvariantCulture, n == 1 ? singularMsgId : pluralMsgId, args);
            }

            return args.Any()
                ? Localizer.Catalog.GetPluralString(singularMsgId, pluralMsgId, n, args)
                : Localizer.Catalog.GetPluralString(singularMsgId, pluralMsgId, n);
        }

        public static string GetParticularPluralString(string context, string text, string pluralText, int n, params object[] args)
        {
            if (Localizer is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return string.Format(CultureInfo.InvariantCulture, n == 1 ? text : pluralText, args);
            }

            return args.Any()
                ? Localizer.Catalog.GetParticularPluralString(context, text, pluralText, n, args)
                : Localizer.Catalog.GetParticularPluralString(context, text, pluralText, n);
        }

        public static string GetParticularString(string context, string text, params object[] args)
        {
            if (Localizer is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return (args.Any() ? string.Format(CultureInfo.InvariantCulture, text, args) : text);
            }
            return args.Any()
                ? Localizer.Catalog.GetParticularString(context, text, args)
                : Localizer.Catalog.GetParticularString(context, text);
        }
    }
}
