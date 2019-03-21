# mpv.net

mpv.net is a libmpv based media player for Windows, it looks and works like mpv, even shares the settings and therefore the mpv documentation applies.

mpv and mpv.net have a learning curve and are only suitable for experienced users.

mpv manual: https://mpv.io/manual/master/

### Features

- Customizable context menu defined in the same file as the keybindings
- Addon API for .NET languages
- 5 different scripting languages are supported, Python scripting implemented with IronPython, C# implemented with CS-Script, Lua and JavaScript implemented in libmpv and PowerShell
- mpv's OSC, IPC, conf files and more

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshot.png)

### Context Menu

The context menu can be customized via input.conf file located at:

C:\Users\Frank\AppData\Roaming\mpv\input.conf

if it's missing mpv.net generates it with the following defaults:

https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt

### C# Scripting

A simple C# script located at:

C:\Users\Frank\AppData\Roaming\mpv\scripts\test.cs

or

startup\scripts\test.cs

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

### Python Scripting

A simple Python script located at:

C:\Users\user\AppData\Roaming\mpv\scripts

or

startup\scripts

```
# when seeking displays position and
# duration like so: 70:00 / 80:00
# which is different from mpv which
# uses 01:10:00 / 01:20:00

import math

def seek():
    mp.commandv('show-text',
        format(mp.get_property_number("time-pos")) + " / " + format(mp.get_property_number("duration")))

def format(f):
    sec = round(f)
    
    if sec < 0:
        sec = 0
    
    pos_min_floor = math.floor(sec / 60)
    sec_rest = sec - pos_min_floor * 60
    return add_zero(pos_min_floor) + ":" + add_zero(sec_rest)

def add_zero(val):
    val = round(val)
    return "" + str(int(val)) if (val > 9) else "0" + str(int(val))

mp.register_event("seek", seek) # or use: mp.Seek += seek
```

### PowerShell Scripting

A simple PowerShell script located at:

C:\Users\user\AppData\Roaming\mpv\scripts

or

startup\scripts

Please note that PowerShell don't allow assigning to events and mpv.net uses as workaround the script filename.

```
$position = [mp]::get_property_number("time-pos");
[mp]::commandv("show-text", $position.ToString() + " seconds")
```

### Changes

### not yet released

### 1.5

- the info command supports now info for music files instead of showing an exception on music files
- added support for WM_APPCOMMAND API to support media keyboards
- fixed Alt key input not working
- mpv.net API methods renamed to match the names used in the Lua/JS API

### 1.4

- the last thread sync fix wasn't working well, the delayed shutdown should be gone for good now
- libmpv updated

### 1.3

- besides Lua/JavaScript/C#/Python there is now PowerShell supported as fifth scripting language

- in case there isn't yet a mpv.conf file mpv.net creates the file with certain default settings that were previously set on every mpv.net start. This was changed to provide transparency on which settings mpv.net uses. These default settings can be seen here: https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpv.conf.txt

### 1.2

- a thread synchonisation bug which caused the shutdown to be delayed or frozen was fixed, it also caused the Shutdown event not to fire which caused the rating plugin not to work

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
