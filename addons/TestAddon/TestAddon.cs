using System.ComponentModel.Composition;

using mpvnet;

namespace TestAddon
{
    [Export(typeof(IAddon))]
    public class TestAddon : IAddon
    {
        // do some init work in constructor
        public TestAddon()
        {
            // Observe changes of the fullscreen property.
            // You can find a list of available mpv properties
            // in mpv.net's wiki on github or use mpv --list-properties.
            // You can test properties in mpv.net in the menu at:
            // Tools > Execute mpv command
            // where you can enter: show-text ${fullscreen}
            mp.observe_property_bool("fullscreen", OnFullscreenChange);
        }

        void OnFullscreenChange(bool val)
        {
            mp.commandv("show-text", "fullscreen: " + val.ToString());
        }
    }
}