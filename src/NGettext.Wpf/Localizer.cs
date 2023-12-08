
using System.Globalization;
using System.IO;

namespace NGettext.Wpf
{
    public interface ILocalizer
    {
        ICatalog Catalog { get; }
        ICatalog GetCatalog(CultureInfo cultureInfo);
        ICultureTracker CultureTracker { get; }
    }

    public class Localizer : IDisposable, ILocalizer
    {
        string _domainName;
        string _localeFolder;

        public Localizer(ICultureTracker cultureTracker, string domainName, string localeFolder)
        {
            _domainName = domainName;
            _localeFolder = localeFolder;
            CultureTracker = cultureTracker;

            if (cultureTracker == null)
                throw new ArgumentNullException(nameof(cultureTracker));

            cultureTracker.CultureChanging += ResetCatalog;
            ResetCatalog(cultureTracker.CurrentCulture);
        }

        void ResetCatalog(object sender, CultureEventArgs e)
        {
            ResetCatalog(e.CultureInfo);
        }

        void ResetCatalog(CultureInfo cultureInfo)
        {
            Catalog = GetCatalog(cultureInfo);
        }

        public ICatalog GetCatalog(CultureInfo cultureInfo) =>
            new Catalog(_domainName, _localeFolder, cultureInfo);

        public ICatalog Catalog { get; private set; }

        public ICultureTracker CultureTracker { get; }

        public void Dispose()
        {
            CultureTracker.CultureChanging -= ResetCatalog;
        }
    }

    public static class LocalizerExtensions
    {
        internal struct MsgIdWithContext
        {
            internal string Context { get; set; }
            internal string MsgId { get; set; }
        }

        internal static MsgIdWithContext ConvertToMsgIdWithContext(string msgId)
        {
            var result = new MsgIdWithContext { MsgId = msgId };

            if (msgId.Contains("|"))
            {
                var pipePosition = msgId.IndexOf('|');
                result.Context = msgId.Substring(0, pipePosition);
                result.MsgId = msgId.Substring(pipePosition + 1);
            }

            return result;
        }

        internal static string Gettext(this ILocalizer localizer, string msgId, params object[] values)
        {
            if (msgId == null)
                return "";
            
            var msgIdWithContext = ConvertToMsgIdWithContext(msgId);

            if (localizer is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return string.Format(msgIdWithContext.MsgId, values);
            }

            if (msgIdWithContext.Context != null)
            {
                return localizer.Catalog.GetParticularString(msgIdWithContext.Context, msgIdWithContext.MsgId, values);
            }

            return localizer.Catalog.GetString(msgIdWithContext.MsgId, values);
        }

        internal static string? Gettext(this ILocalizer localizer, string msgId)
        {
            if (msgId is null) 
                return null;

            var msgIdWithContext = ConvertToMsgIdWithContext(msgId);

            if (localizer is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return msgIdWithContext.MsgId;
            }

            if (msgIdWithContext.Context != null)
                return localizer.Catalog.GetParticularString(msgIdWithContext.Context, msgIdWithContext.MsgId);

            return localizer.Catalog.GetString(msgIdWithContext.MsgId);
        }
    }
}