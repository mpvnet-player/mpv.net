
// This script observes the fullscreen property and
// draws text on screen when the property changes.

using mpvnet;

class Script
{
    Core core;
    
    public Script()
    {
        core = Core.core;
        core.observe_property_bool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool value)
    {
        core.commandv("show-text", "fullscreen: " + value);
    }
}
