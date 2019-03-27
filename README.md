# mpv.net

mpv.net is a libmpv based media player for Windows, it looks and works like mpv, even shares the settings and therefore the mpv documentation applies.

mpv and mpv.net have a learning curve and are only suitable for experienced users.

mpv manual: https://mpv.io/manual/master/

Table of contents
-------

- [Features](#features)
- [Screenshots](#Screenshots)
- [Context Menu](#context-menu)
- [Settings](#settings)
- [C# Scripting](#cs-scripting)
- [Python Scripting](#python-scripting)
- [PowerShell Scripting](#powershell-scripting)
- [Support](#Support)
- [Changelog](#changelog)

### Features

- Customizable context menu defined in the same file as the keybindings
- Addon API for .NET languages
- 5 different scripting languages are supported, Python scripting implemented with IronPython, C# implemented with CS-Script, Lua and JavaScript implemented in libmpv and PowerShell
- mpv's OSC, IPC, conf files and more

### Screenshots

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/screenshot.png)

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/mpvSettingsEditor.png)

### Context Menu

The context menu can be customized via input.conf file located at:
```
C:\Users\username\AppData\Roaming\mpv\input.conf
```
if it's missing mpv.net generates it with the following defaults:

https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt

### Settings

mpv.net shares the settings with mpv, settings have to be edited in a config file called mpv.conf located at:
```
C:\Users\username\AppData\Roaming\mpv\mpv.conf
```
if it's missing mpv.net generates it with the following defaults:

https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpv.conf.txt

### C# Scripting

A simple C# script located at:
```
C:\Users\username\AppData\Roaming\mpv\scripts\fullscreen.cs
```
or
```
startup\scripts\fullscreen.cs
```
```
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
```

### Python Scripting

A simple Python script located at:
```
C:\Users\user\AppData\Roaming\mpv\scripts\seek-show-position.py
```
or
```
startup\scripts\seek-show-position.py
```
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
```
C:\Users\user\AppData\Roaming\mpv\scripts\seek.ps1
```
or
```
startup\scripts\seek.ps1
```
```
$position = [mp]::get_property_number("time-pos");
[mp]::commandv("show-text", $position.ToString() + " seconds")
```
Please note that PowerShell don't allow assigning to events and mpv.net uses as workaround a matching script filename, a list of available events can be found in the mpv manual or in the file mp.cs in the mpv.net source code.

### Support

<https://forum.doom9.org/showthread.php?t=174841>

<https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows>

<https://github.com/stax76/mpv.net/issues>

### Changelog

### 2.0

- setting track-auto-selection added to settings editor (<https://mpv.io/manual/master/#options-track-auto-selection>)
- setting loop-playlist added to settings editor (<https://mpv.io/manual/master/#options-loop-playlist>)
- setting audio-file-auto added to settings editor (<https://mpv.io/manual/master/#options-audio-file-auto>)
- setting video-sync added to settings editor (<https://mpv.io/manual/master/#options-video-sync>)
- command execute-mpv-command added to menu: Tools > Enter a mpv command for execution
- added youtube-dl.exe, please note this will only work when a certain Visual C++ runtime is installed
- added drag & drop support to drag & drop a youtube URL on mpv.net
- added support to open a youtube URL from command line
- added support for opening a URL from the menu: Open > Open URL

### 1.9

- improved settings editor
- all info and error messages are shown now on the main window thread having the main window as parent

### 1.8

- new config editor added

### 1.7

- showing the conf files mpv.net uses now the app that is registered for txt files, before it just shell executed the conf file which only worked if conf files were associated with an application
- leaving fullscreen mode the previous window size and position wasn't restored
- when the source video aspect ratio changes the height is kept and the width is adjusted and a check is performed to assure the window is within screen bounds

### 1.6

- a crash caused by WM_APPCOMMAND (multimedia keyboards) commands was fixed
- support for the 'screen' property was added, it should work both from mpv.conf (screen = 1) and from command line (--screen=1)
- per monitor DPI awareness and better multi monitor support was added

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