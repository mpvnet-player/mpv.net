
using System.Globalization;

namespace NGettext.Wpf;

public interface ICultureTracker
{
    [Obsolete("Use AddWeakCultureObserver() instead.  Otherwise the culture tracker (which is probably a singleton) will keep your object alive for longer than it needs to. This method will be removed in 2.x")]
    event EventHandler<CultureEventArgs> CultureChanged;

    event EventHandler<CultureEventArgs> CultureChanging;

    CultureInfo CurrentCulture { get; set; }

    void AddWeakCultureObserver(IWeakCultureObserver weakCultureObserver);
}

public class CultureTracker : ICultureTracker
{
    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;
    private List<WeakReference<IWeakCultureObserver>> _weakObservers = new List<WeakReference<IWeakCultureObserver>>();

    public event EventHandler<CultureEventArgs> CultureChanged;

    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture == value)
                return;

            CultureChanging?.Invoke(this, new CultureEventArgs(value));
            _currentCulture = value;
            RaiseCultureChanged();
        }
    }

    protected virtual void RaiseCultureChanged()
    {
        var cultureEventArgs = new CultureEventArgs(CurrentCulture);
        CultureChanged?.Invoke(this, cultureEventArgs);

        var weakObserversStillAlive = new List<WeakReference<IWeakCultureObserver>>();

        foreach (var weakReference in _weakObservers)
        {
            if (!weakReference.TryGetTarget(out var observer))
                continue;

            observer.HandleCultureChanged(this, cultureEventArgs);
            weakObserversStillAlive.Add(weakReference);
        }

        _weakObservers = weakObserversStillAlive;
    }

    public event EventHandler<CultureEventArgs> CultureChanging;

    public void AddWeakCultureObserver(IWeakCultureObserver weakCultureObserver)
    {
        _weakObservers.Add(new WeakReference<IWeakCultureObserver>(weakCultureObserver));
    }
}