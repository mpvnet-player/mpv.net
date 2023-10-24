
using MpvNet.ExtensionMethod;

namespace MpvNet;

public class Chapter
{
    public string Title { get; set; } = "";
    public double Time { get; set; }

    string? _timeDisplay;

    public string TimeDisplay
    {
        get
        {
            if (_timeDisplay == null)
            {
                _timeDisplay = TimeSpan.FromSeconds(Time).ToString();

                if (_timeDisplay.ContainsEx("."))
                    _timeDisplay = _timeDisplay[.._timeDisplay.LastIndexOf(".")];
            }

            return _timeDisplay;
        }
    }
}
