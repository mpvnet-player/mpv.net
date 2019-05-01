# mpv.net

mpv.net is a modern media player for Windows that looks and works just like [mpv](https://mpv.io).

#### Based on libmpv

mpv.net is based on libmpv which offers a straightforward C API that was designed from the ground up to make mpv usable as a library and facilitate easy integration into other applications.

#### CLI options

mpv.net has the same [CLI options](https://mpv.io/manual/master/#options) as mpv.

#### High quality video output

mpv/libmpv has an OpenGL based video output that is capable of many features loved by videophiles, such as video scaling with popular high quality algorithms, color management, frame timing, interpolation, HDR, and more.

#### On Screen Controller

mpv.net uses the OSC of mpv/libmpv offering play controls with a modern flat design.

#### GPU video decoding

mpv/libmpv leverages the FFmpeg hwaccel APIs to support DXVA2 video decoding acceleration.

#### Active development

mpv.net is under active development. Want a feature? Post a [patch](https://github.com/stax76/mpv.net/pulls) or [request it](https://github.com/stax76/mpv.net/issues)!

Table of contents
-----------------

- [Features](#features)
- [Screenshots](#screenshots)
- [Context Menu](#context-menu)
- [Settings](#settings)
- [Scripting](#scripting)
- [Add-ons](#add-ons)
- [Architecture](#architecture)
- [Support](#support)
- [Links](#links)
- [Download](#download)
- [Changelog](#changelog)

### Features

- Customizable context menu defined in the same file as the key bindings ([Screenshot](#context-menu))
- Searchable config dialog ([Screenshot](#config-editor))
- Searchable input (key/mouse) binding editor ([Screenshot](#input-editor))
- Searchable command palette to quickly launch commands and look for keys ([Screenshot](#command-palette))
- Modern UI with dark mode ([Screenshot](#config-editor))
- Addon/extension API for .NET languages
- Scripting API for Python, C#, Lua, JavaScript and PowerShell ([wiki](https://github.com/stax76/mpv.net/wiki/Scripting))
- mpv's OSC, IPC and conf files
- Support of the same [CLI options](https://mpv.io/manual/master/#options) as mpv
- DXVA2 video decoding acceleration
- OpenGL based video output capable of features loved by videophiles, such as video scaling with popular high quality algorithms, color management, frame timing, interpolation, HDR, and more

### Screenshots

#### Main Window

![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/Main.png)

#### Context Menu

Context menu defined in the input.conf file with dark mode support.

![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/Menu.png)

#### Config Editor

A searchable config editor as alternative to edit the mpv.conf file manually.

![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/ConfEditor.png)

#### Input Editor

A searchable key and mouse binding editor.

![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/InputEditor.png)

#### Command Palette

Forgot where a command in the menu is located or what shortcut key it has?
Just press Ctrl+Shift+P and find it easily in the searchable command palette.

![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/CommandPalette.png)

### Context Menu

The context menu can be customized via input.conf file located at:
```
C:\Users\%username%\AppData\Roaming\mpv\input.conf
```
if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt>

input.conf defines mpv's key and mouse bindings and mpv.net uses comments to define the context menu.

### Settings

mpv.net shares the settings with mpv, settings can be edited in a settings dialog or in a config file called mpv.conf located at:
```
C:\Users\%username%\AppData\Roaming\mpv\mpv.conf
```
or alternativly at:
```
<startup>\portable_config\mpv.conf
```
if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpvConf.txt>

### Scripting

Scripting is supported via Python, C#, Lua, JavaScript and PowerShell

[Scripting wiki page](https://github.com/stax76/mpv.net/wiki/Scripting)

### Add-ons

Add-ons have to be located at:

C:\Users\%username%\AppData\Roaming\mpv\Addons\ExampleAddon\ExampleAddon.dll

\<startup\>\portable_config\Addons\ExampleAddon\ExampleAddon.dll

The add-on filename must end with 'Addon.dll'.

Examples:

[RatingAddon.cs](https://github.com/stax76/mpv.net/blob/master/RatingAddon/RatingAddon.cs)

[CSScriptAddon.vb](https://github.com/stax76/mpv.net/blob/master/CSScriptAddon/CSScriptAddon.vb)

### Architecture

mpv.net is written in C# 7.0 and runs on the .NET framework 4.7 or higher.

The Add-on implementation is based on the Managed Extensibility Framework,
the entire application code is accessible for add-ons and Python scripts.

Python scripting is implemented with IronPython which uses Python 2.7.

The main/video window is WinForms based, other windows are WPF based.

The config editor adds it's controls dynamically and uses TOML to define it's
content, there are only two simple types, StringSetting and OptionSetting.

mpv.net was started 2017 and consists of about 9000 lines of code.

Third party components are:

- [libmpv](https://mpv.io/)
- [MediaInfo](https://mediaarea.net/en/MediaInfo)
- [Tommy (TOML parser)](https://github.com/dezhidki/Tommy)
- [IronPython](https://ironpython.net/)

### Support

[Support thread in Doom9 forum](https://forum.doom9.org/showthread.php?t=174841)

[Support thread in VideoHelp forum](https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows)

[Issue tracker to report bugs and request features](https://github.com/stax76/mpv.net/issues)

### Links

mpv manual: <https://mpv.io/manual/master/>

mpv wiki: <https://github.com/mpv-player/mpv/wiki>

mpv.net wiki: <https://github.com/stax76/mpv.net/wiki>

mpv apps: <https://github.com/mpv-player/mpv/wiki/Applications-using-mpv>

mpv user scripts: <https://github.com/mpv-player/mpv/wiki/User-Scripts>

mpv default key bindings: <https://github.com/mpv-player/mpv/blob/master/etc/input.conf>

mpv.net default key bindings: <https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt>

mpv download: <https://mpv.io/installation/>

mpv.net download: <https://github.com/stax76/mpv.net/releases>

mpv bugs and requests: <https://mpv.io/bug-reports/>

mpv.net bugs and requests: <https://github.com/stax76/mpv.net/issues>

### Download

<https://github.com/stax76/mpv.net/releases>

### Changelog

### 3.3 (2019-??-??)

- dark mode support was added to the command palette
- a new icon was designed. [Website](https://mpv-net.github.io/mpv.net-web-site/)
- all windows (main, conf, input, about, command palette) can now be closed
  by just pressing the Escape key
- new feature added to open recent files and URLs with the context menu. [Default Binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L33)
- the info command (i key) now works also for URLs
- CSScriptAddon add-on didn't load cs scripts from \<startup\>\\scripts and it didn't use the task dialog api to show errors

### 3.2 (2019-04-27)

- mpvInputEdit and mpvConfEdit were discontinued and merged into
  mpvnet because separate apps were to difficult to work with
- portable mode: in case no config folder exists and the
  startup folder has write access mpvnet will ask where
  the config folder should be created (portable or appdata)
- there was an issue causing keys not working after a modal window was shown
- there was a crash when no script folder existed in the conf folder
- MediaInfo and youtube-dl were updated
- a new JavaScript example script was added to the wiki and the
  script descriptions were improved. [Scripting Page](https://github.com/stax76/mpv.net/wiki/Scripting).
- greatly improved README.md file and [github startpage](https://github.com/stax76/mpv.net)
- About dialog added
- the input editor shows only a closing message if actually a change was made
- the input editor don't show confusing menu separators any longer. [Screenshot](https://github.com/stax76/mpv.net#input-editor)
- new Command Palette feature added. [Screenshot](https://github.com/stax76/mpv.net#command-palette), [Default input binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L141)
- the history feature had a bug causing files to be logged more than once

### 3.1 (2019-04-23)

- the Tracks and Chapters menu are now only added if default bindings exist and
  it's now possible to move the chapters menu to the top level by editing input.conf
- mpvnet supports now like mpv a portable settings directory. If a directory named portable_config
  next to the mpvnet.exe exists, all config will be loaded and written in this directory.
- there is now a portable download in 7zip format.

### 3.0 (2019-04-20)

- the history feature logs now only files that were opened longer than 90 seconds
- the default input command for cycling the audio tracks was replaced with an
  mpv.net command that shows detailed track info and has no 'no audio' track. [Default binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L89).
- new website at <https://mpv-net.github.io/mpv.net-web-site/>
- the Tracks menu supports now MKV edition selection. [Default binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L106).
- the Navigate menu supports now chapter selection. [Default binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L57).
- opening the context menu was crashing if the default binding for Tracks was missing

### 2.9 (2019-04-16)

- clicking the right top corner in full screen mode
  closes the player but it did not work on all displays
- the info display was changed to display the filename on top
  so it's not displayed in the middle of the screen
- on start up of the config editor all text is now selected in the
  search text box so it's ready for a new search to be typed
- the config editor was changed to write the settings to disk
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
  tracks with various metadata. [Menu default definition](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L104).
  The screenshots were updated showing the [new track menu](https://github.com/stax76/mpv.net#screenshots).