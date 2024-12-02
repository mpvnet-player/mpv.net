
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace NGettext.Wpf
{
    [MarkupExtensionReturnType(typeof(string))]
    public class GettextExtension : MarkupExtension, IWeakCultureObserver
    {
        private DependencyObject _dependencyObject;
        private DependencyProperty _dependencyProperty;

        [ConstructorArgument("params")] public object[] Params { get; set; }

        [ConstructorArgument("msgId")] public string MsgId { get; set; }

        public GettextExtension(string msgId)
        {
            MsgId = msgId;
            Params = new object[] { };
        }

        public GettextExtension(string msgId, params object[] @params)
        {
            MsgId = msgId;
            Params = @params;
        }

        public static ILocalizer Localizer { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (provideValueTarget.TargetObject is DependencyObject dependencyObject)
            {
                _dependencyObject = dependencyObject;
                if (DesignerProperties.GetIsInDesignMode(_dependencyObject))
                {
                    return Gettext();
                }

                AttachToCultureChangedEvent();

                _dependencyProperty = (DependencyProperty)provideValueTarget.TargetProperty;

                KeepGettextExtensionAliveForAsLongAsDependencyObject();
            }
            else
            {
                System.Console.WriteLine("NGettext.Wpf: Target object of type {0} is not yet implemented", provideValueTarget.TargetObject?.GetType());
            }

            return Gettext();
        }

        private string Gettext()
        {
            return Params.Any() ? Localizer.Gettext(MsgId, Params) : Localizer.Gettext(MsgId);
        }

        void KeepGettextExtensionAliveForAsLongAsDependencyObject()
        {
            SetGettextExtension(_dependencyObject, this);
        }

        void AttachToCultureChangedEvent()
        {
            if (Localizer is null)
            {
                Console.Error.WriteLine("NGettext.WPF.GettextExtension.Localizer not set.  Localization is disabled.");
                return;
            }

            Localizer.CultureTracker.AddWeakCultureObserver(this);
        }

        public void HandleCultureChanged(ICultureTracker sender, CultureEventArgs eventArgs)
        {
            _dependencyObject.SetValue(_dependencyProperty, Gettext());
        }

        public static readonly DependencyProperty GettextExtensionProperty = DependencyProperty.RegisterAttached(
            "GettextExtension", typeof(GettextExtension), typeof(GettextExtension), new PropertyMetadata(default(GettextExtension)));

        public static void SetGettextExtension(DependencyObject element, GettextExtension value)
        {
            element.SetValue(GettextExtensionProperty, value);
        }

        public static GettextExtension GetGettextExtension(DependencyObject element)
        {
            return (GettextExtension)element.GetValue(GettextExtensionProperty);
        }
    }
}