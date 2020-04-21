
// Draws text on screen when full screen property changes.

using mpvnet;

class Script
{
    public Script()
    {
        mp.observe_property_bool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool value)
    {
        mp.commandv("show-text", "fullscreen: " + value.ToString());
    }
}
