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

- Customizable context menu defined in the same file as the key bindings
- Searchable options dialog with modern UI
- Searchable input (key/mouse) binding editor with modern UI
- Rich addon API for .NET languages
- Rich scripting API for Python, C#, Lua, JavaScript and PowerShell
- mpv's OSC (on screen controller (play control bar)), IPC, conf files

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

### 2.3 (2019-04-04)

- dragging a youtube URL on mpv.net would still break something, it should work now
- when the main window gets focus/activation it will check the clibboard for a YouTube video and ask to play it
- libmpv updated
- changing to normal size from fullscreen resulted in a too large window in some circumstances
- some default key bindings and menu structure have changed and the input.conf file has a description added on top <https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt>
- the file association code was completely rewriten, it's now contained within mpvnet.exe instead of a separate application and it adds a few more keys
- various new info added to the wiki: <https://github.com/stax76/mpv.net/wiki>
- On Top feature was implemented using mpv's native property 'ontop', default bindings at: <https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt>

### 2.2 (2019-04-01)

- messages boxes had always the info icon even if a different icon (error, warning, question) was intended
- instead of silently do nothing on unknown commands there is now a error message listing available commands and showing the location of the default bindings, this helps when commands are removed or renamed
- there was a problem fixed that made the cursor hidden when it should be visible
- dragging a youtube URL on mpv.net would break certain input related features
- there is now an installer with file extension registration (limited on Win 10) available
- WM_APPCOMMAND media keys were not working in the input (shortcut) editor and there were no defaults for prev and next defined