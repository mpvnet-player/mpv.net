
// This script observes the fullscreen property and
// draws text on screen when the property changes.

using mpvnet;

class Script
{
    public Script()
    {
        mp.observe_property_bool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool value)
    {
        mp.commandv("show-text", "fullscreen: " + value);
    }
}
