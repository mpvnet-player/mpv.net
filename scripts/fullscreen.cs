using mpvnet;

class Script
{
    public Script()
    {
        var fs = mp.get_property_string("fullscreen");
        mp.commandv("show-text", "fullscreen: " + fs);
        mp.observe_property_bool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool val)
    {
        mp.commandv("show-text", "fullscreen: " + val.ToString());
    }
}