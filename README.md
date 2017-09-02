# mpv.net

mpv.net is a libmpv based media player for Windows, it looks and works exactly like mpv, even shares the same settings.

### Features

- Context menu which can be customized
- Addons implemented with the Managed Extension Framework (MEF)
- C# scripts implemented with CS-Script

![](https://github.com/stax76/mpvnet/blob/master/mpvnet/screenshot.jpg)

### C# Scripting

A simple C# script located at: C:\Users\Frank\AppData\Roaming\mpv\scripts\test.cs

```
using mpvnet;

class Script
{
    public Script()
    {
        var fs = mpv.GetStringProp("fullscreen");
        mpv.Command("show-text", "fullscreen: " + fs);
        mpv.ObserveBoolProp("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool val)
    {
        mpv.Command("show-text", "fullscreen: " + val.ToString());
    }
}
```
