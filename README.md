# mpv.net

mpv.net is a libmpv based media player for Windows, it looks and works like mpv, even shares the settings and therefore the mpv documentation applies.

mpv and mpv.net have a learning curve and are only suitable for experienced users.

mpv manual: https://mpv.io/manual/master/

### Features

- Customizable context menu defined in the same file as the keybindings
- Addons support for using .NET languages
- C# scripts implemented with CS-Script
- mpv's OSC, IPC, Lua/JS, conf files and more

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshot.png)

### Context Menu

The context menu can be customized via input.conf file located at:

C:\Users\Frank\AppData\Roaming\mpv\input.conf

if it misses mpv.net generates it with the following defaults:

https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input_conf.txt

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

### 1.1

- added support for Python scripting via IronPython
- show tracks and show playlist didn't work because the duration wasn't defined in the key bindings

### 1.0

- much more feature packed context menu

### 0.2.5

- mpv lib updated to 2019-02-24
- UI glitch fixed the appeared when started in fullscreen mode
- fixed default video output mode which caused video playback to fail

### 0.2.4

- changed minimum runtime to .NET 4.7.2
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
