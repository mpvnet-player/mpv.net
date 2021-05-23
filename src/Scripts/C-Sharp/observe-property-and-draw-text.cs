
// This script observes the fullscreen property and
// draws text on screen when the property changes.

using mpvnet;

class Script
{
    CorePlayer Core;

    public Script()
    {
        Core = Global.Core;
        Core.observe_property_bool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool value)
    {
        Core.commandv("show-text", "fullscreen: " + value);
    }
}
