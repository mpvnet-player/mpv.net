using System;
using System.Windows.Markup;
using NGettext.Wpf.Common;

namespace NGettext.Wpf
{
    public class GettextFormatConverterExtension : MarkupExtension
    {
        public GettextFormatConverterExtension(string msgId)
        {
            MsgId = msgId;
        }

        [ConstructorArgument("msgId")] public string MsgId { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new GettextStringFormatConverter(MsgId);
        }
    }
}