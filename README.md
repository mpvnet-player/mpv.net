# mpv.net

mpv.net is a libmpv based media player for Windows, it looks and works like mpv and also shares the same settings as mpv and therefore the mpv documentation applies.

mpv and mpv.net have a learning curve.

mpv manual: <https://mpv.io/manual/master/>

Table of contents
-----------------

- [Features](#features)
- [Screenshots](#screenshots)
- [Context Menu](#context-menu)
- [Settings](#settings)
- [Scripting](#scripting)
- [Support](#support)
- [Changelog](#changelog)

### Features

- Customizable context menu defined in the same file as the keybindings
- Addon API for .NET languages
- Scripting API for Python, C#, Lua, JavaScript and PowerShell
- mpv's OSC, IPC, conf files and more

### Screenshots

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/mpvnet.png)

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/mpvConfEdit.png)

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/mpvInputEdit.png)

### Context Menu

The context menu can be customized via input.conf file located at:
```
C:\Users\username\AppData\Roaming\mpv\input.conf
```
if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt>

### Settings

mpv.net shares the settings with mpv, settings can be edited in a settings dialog or in a config file called mpv.conf located at:
```
C:\Users\user\AppData\Roaming\mpv\mpv.conf
```
if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpv.conf.txt>

### Scripting

Scripting is supported for Python, C#, Lua, JavaScript and PowerShell

https://github.com/stax76/mpv.net/wiki/Scripting-(CSharp,-Python,-JavaScript,-Lua,-PowerShell)

### Support

<https://forum.doom9.org/showthread.php?t=174841>

<https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows>

<https://github.com/stax76/mpv.net/issues>

### Changelog

### soon

- messages boxes had always the info icon even if a different icon (error, warning, question) was intended
- instead of silently do nothing on unknown commands there is now a error message listing available commands and showing the location of the default bindings, this helps when commands are removed or renamed
- there was a problem fixed that made the cursor hidden when it should be visible

### 2.1 (2019-03-30)

- new input editor added, default key binding is here: <https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt#L89>

### 2.0 (2019-03-28)

- setting track-auto-selection added to settings editor (<https://mpv.io/manual/master/#options-track-auto-selection>)
- setting loop-playlist added to settings editor (<https://mpv.io/manual/master/#options-loop-playlist>)
- setting audio-file-auto added to settings editor (<https://mpv.io/manual/master/#options-audio-file-auto>)
- setting video-sync added to settings editor (<https://mpv.io/manual/master/#options-video-sync>)
- command execute-mpv-command added to menu: Tools > Enter a mpv command for execution
- added youtube-dl.exe, please note this will only work when a certain Visual C++ runtime is installed
- added drag & drop support to drag & drop a youtube URL on mpv.net
- added support to open a youtube URL from command line
- added support for opening a URL from the menu: Open > Open URL