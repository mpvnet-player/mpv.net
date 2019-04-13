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
- Searchable options dialog with modern UI as mpv compatible standalone application
- Searchable input (key/mouse) binding editor with modern UI as mpv compatible standalone application
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

### 2.9 (2019-

- clicking the right top corner in fullscreen mode closes the player but it did not work on all displays

### 2.8 (2019-04-12)

- Win 7 dark-mode render issue fix

### 2.7 (2019-04-12)

- the autofit mpv property was added to the conf editor
- the routine that writes the mpv.conf file in the conf editor was completely rewritten
- the conf editor has a dedicated page for mpv.net specific settings,
  these settings are saved in the same folder as mpv.conf using mpvnet.conf as filename,
  the first setting there is dark-mode
- new optional dark theme 

[go to download page](https://github.com/stax76/mpv.net/releases)

### 2.6 (2019-04-09)

- on Win 7 controls in the conf editor were using a difficult too read too light color
- context menu renderer changed to look like Win 10 design, except colors are still system theme colors

### 2.5 (2019-04-08)

- in case the input conf don't contain a menu definition mpv.net creates the default menu instead no menu like before
- all message boxes were migrated to use the TaskDialog API
- an improvement in the previous release unfortunately introduced a bug
  causing the conf editor not to save settings

### 2.4 (2019-04-06)

- new options added to the conf GUI editor: gpu-context, gpu-api, scale, cscale,
  dscale, dither-depth, correct-downscaling, sigmoid-upscaling, deband
- the conf edit GUI has a 'Apply' feature added to write the conf to mpv.conf
  without the need to close the conf edit GUI
- the input edit GUI shows a message box when a duplicate is detected and it has
  a new feature to reduce the filter scope to eather of input, menu or command and
  the editor writes always the same help on top of input.conf as it is found in the defaults
- the conf edit GUI was often starting out of working area bounds and is now starting with center screen
- the startup size was reduced and a issue was fixed that when the screen property
  was defined for a screen that isn't connected the startup size wasn't applied
- added feature to load external audio and subtitle files in the menu under:
  Open > Load external audio|subtitle files (default binding at:
  [input.conf](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt))
- previously the conf edit GUI removed settings from the conf file if the setting
  was set to the default, the new behavior is not to remove anything
- the autofit mpv property was partly implemented, you can use 'autofit = 50%' in mpv.conf or
  '--autofit=50%' on the command line, WxH isn't implemented and only percent values are accepted.
  There is a new wiki page explaining the mpv.net limitations compared to the original mpv:
  [Limitations](https://github.com/stax76/mpv.net/wiki/Limitations)