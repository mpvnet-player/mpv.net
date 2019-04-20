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
- Modern UI using the OS theme color and dark mode
- Rich addon/extension API for .NET languages, over 700 available mpv properties
- Rich scripting API for Python, C#, Lua, JavaScript and PowerShell
- mpv's OSC (on screen controller (play control bar)), IPC, conf files

### Screenshots

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/mpvnet.png)

![](https://raw.githubusercontent.com/stax76/mpv.net/master/screenshots/mpvnetContextMenu.png)

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

Scripting is supported via Python, C#, Lua, JavaScript and PowerShell

[Scripting wiki page](https://github.com/stax76/mpv.net/wiki/Scripting-(CSharp,-Python,-JavaScript,-Lua,-PowerShell))

### Support

[Support thread in Doom9 forum](https://forum.doom9.org/showthread.php?t=174841)

[Support thread in VideoHelp forum](https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows)

[Issue tracker to report bugs and request features](https://github.com/stax76/mpv.net/issues)

### Changelog

### 3.0 (2019-04-20)

- the history feature logs now only files that were opened longer than 90 seconds
- the default input command for cycling the audio tracks was replaced with an
  mpv.net command that shows detailed track info and has no 'no audio' track
- new web site for mpv.net <https://mpv-net.github.io/mpv.net-web-site/>
- the Tracks menu supports now MKV edition selection. [Default binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt#L106).
- the Navigate menu supports now chapter selection. [Default binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt#L57).
- opening the context menu was crashing if the default binding for Tracks was missing

[go to download page](https://github.com/stax76/mpv.net/releases)

### 2.9 (2019-04-16)

- clicking the right top corner in full screen mode
  closes the player but it did not work on all displays
- the info display was changed to display the filename on top
  so it's not displayed in the middle of the screen
- on start up of the conf editor all text is now selected in the
  search text box so it's ready for a new search to be typed
- the conf editor was changed to write the settings to disk
  only if the settings were actually modified, also the message
  that says that the settings will be available on next start
  is now only shown if the settings were actually modified.
- there was an instance in the context menu where the sub menu
  arrow was overlapping with the text
- in the input editor when only one character is entered in the
  search text box the search is performed only in the input and
  not in the command or menu
- in the input editor the routine that generates the input string
  was completely rewritten because it was adding Shift where it
  wasn't necessary (it took a huge amount of time to implement)
- the context menu has a new track menu where the active track
  can be seen and selected, it shows video, audio and subtitle
  tracks with various metadata. [Menu default definition](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt#L104).
  The screenshots were updated showing the [new track menu](https://github.com/stax76/mpv.net#screenshots).