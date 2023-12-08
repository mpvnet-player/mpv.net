namespace NGettext.Wpf
{
    public interface IWeakCultureObserver
    {
        void HandleCultureChanged(ICultureTracker sender, CultureEventArgs eventArgs);
    }
}