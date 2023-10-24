
namespace MpvNet.Extension.ExampleExtension;

public class Extension : IExtension
{
    public MpvClient Player { get; set; }

    public Extension()
    {
        Player = Global.Player.CreateNewPlayer("example");
        Player.ObservePropertyBool("fullscreen", FullscreenChange);
        Player.FileLoaded += Player_FileLoaded;
    }

    void Player_FileLoaded()
    {
        Terminal.Write("File loaded: " + Player.GetPropertyString("path"));
    }

    void FullscreenChange(bool value)
    {
        Player.CommandV("show-text", "fullscreen: " + value);
    }
}
