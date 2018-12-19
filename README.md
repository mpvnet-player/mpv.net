# mpv.net

mpv.net is a libmpv based media player for Windows, it looks and works like mpv, even shares the settings and therefore the mpv documentation applies, it can be found at:

https://mpv.io/manual/master/

### Features

- mpv's OSC, IPC, Lua/JS, conf files and more
- Context menu which can be customized
- Addons implemented with the Managed Extension Framework (MEF)
- C# scripts implemented with CS-Script

![](https://github.com/stax76/mpvnet/blob/master/mpvnet/screenshot.jpg)

### Context Menu

The context menu can be customized via input.conf file located at:

C:\Users\Frank\AppData\Roaming\mpv\input.conf

https://github.com/stax76/mpvnet/blob/master/mpvnet/Resources/input_conf.txt

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

### Changes

### 0.2.3

- fixed mpv.net not working with new mpv lib
- the track name in the title bar was sometimes wrong
- mpv lib updated to 2018-12-16
- quit-watch-later added to context menu (Shift+Q) to exit and resume at the last position
- ab loop added to menu
- added the possibility to modify mpv.conf settings using the context menu
- added link to the manual and default keys to the menu

### 0.2.2

- history feature added
- mpv lib updated

### 0.2.1

- right-click in fullscreen in the right-left corner closes the app