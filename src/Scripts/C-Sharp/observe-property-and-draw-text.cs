
// This script observes the fullscreen property and
// draws text on screen when the property changes.

using mpvnet;

class Script
{
    CorePlayer Core;

    public Script()
    {
        Core = Global.Core;
        Core.ObservePropertyBool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool value)
    {
        Core.CommandV("show-text", "fullscreen: " + value);
    }
}
