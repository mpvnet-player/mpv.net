
using System.Globalization;
using System.Windows.Input;

namespace NGettext.Wpf
{
    public class ChangeCultureCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Any(cultureInfo => cultureInfo.Name == (string)parameter);
        }

        public void Execute(object? parameter)
        {
            if (CultureTracker is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return;
            }

            CultureTracker.CurrentCulture =
                CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Single(cultureInfo => cultureInfo.Name == (string)parameter);
        }

        public event EventHandler? CanExecuteChanged;

        public static ICultureTracker? CultureTracker { get; set; }
    }
}
