![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/mpvnet.png)

# 🎞 mpv.net

mpv.net is a modern media player for Windows that works just like [mpv](https://mpv.io).

#### Based on libmpv

mpv.net is based on libmpv which offers a straightforward C API that was designed from the ground up to make mpv usable as a library and facilitate easy integration into other applications. mpv is like vlc not based on DirectShow or Media Foundation. 

#### Command Line Interface

mpv.net has the [CLI of mpv](https://mpv.io/manual/master/#options).

#### High quality video output

libmpv has an OpenGL based video output that is capable of many features loved by videophiles, such as video scaling with popular high quality algorithms, color management, frame timing, interpolation, HDR, and more.

#### On Screen Controller

The OSC of libmpv offers play controls with a modern flat design. ([Screenshot](#main-window-screenshot))

#### GPU video decoding

libmpv leverages the FFmpeg hwaccel APIs to support DXVA2 video decoding acceleration.

#### Active development

mpv.net is under active development. Want a feature? Post a [patch](https://github.com/stax76/mpv.net/pulls) or [request it](https://github.com/stax76/mpv.net/issues)!

Table of contents
-----------------

- [Features](#features)
- [Screenshots](#screenshots)
- [Manual](#manual)
- [Context Menu](#context-menu)
- [Settings](#settings)
- [Scripting](#scripting)
- [Add-ons](#add-ons)
- [Architecture](#architecture)
- [Support](#support)
- [Links](#links)
- [Changelog](#changelog)
- [Download](#download)

### Features

- Customizable context menu defined in the same file as the key bindings ([Screenshot](#context-menu-screenshot), [Defaults](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt))
- Searchable config dialog ([Screenshot](#config-editor-screenshot), [Defaults](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpvConf.txt))
- Searchable input (key/mouse) binding editor ([Screenshot](#input-editor-screenshot), [Defaults](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt))
- Searchable command palette to quickly launch commands and look for keys ([Screenshot](#command-palette-screenshot))
- Modern UI with dark mode ([Screenshot](#config-editor-screenshot))
- Addon/extension API for .NET languages
- Scripting API for Python, C#, Lua, JavaScript and PowerShell ([Wiki](https://github.com/stax76/mpv.net/wiki/Scripting))
- mpv's OSC, IPC and conf files
- [Command Line Interface](https://mpv.io/manual/master/#options)
- DXVA2 video decoding acceleration
- OpenGL based video output capable of features loved by videophiles, such as video scaling with popular high quality algorithms, color management, frame timing, interpolation, HDR, and more
- Search feature powered by [Everything](https://www.voidtools.com) to find and play media ([Screenshot](#media-search-screenshot))
- Extension to start mpv.net from Google Chrome ([Manual](Manual.md#chrome-extension))

### Screenshots

#### Main Window Screenshot

![Main Window](https://raw.githubusercontent.com/stax76/mpv.net/master/img/Main.png)

#### Context Menu Screenshot

Context menu defined in the input.conf file with dark mode support.

![Context Menu](https://raw.githubusercontent.com/stax76/mpv.net/master/img/Menu.png)

#### Config Editor Screenshot

A searchable config editor as alternative to edit the mpv.conf file manually.

![](https://raw.githubusercontent.com/stax76/mpv.net/master/img/ConfEditor.png)

#### Input Editor Screenshot

A searchable key and mouse binding editor.

![Input Editor](https://raw.githubusercontent.com/stax76/mpv.net/master/img/InputEditor.png)

#### Command Palette Screenshot

Forgot where a command in the menu is located or what shortcut key it has?
Just press F1 and find it easily in the searchable command palette.

![Command Palette](https://raw.githubusercontent.com/stax76/mpv.net/master/img/CommandPalette.png)

#### Media Search Screenshot

Media search feature powered by [Everything](https://www.voidtools.com) to find and play media.

![Media Search](https://raw.githubusercontent.com/stax76/mpv.net/master/img/MediaSearch.png)

### Manual

[Manual](Manual.md)

### Context Menu

The context menu can be customized via input.conf file located in the config directory:

```Text
C:\Users\%username%\AppData\Roaming\mpv\input.conf
```

if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt>

input.conf defines mpv's key and mouse bindings and mpv.net uses comments to define the context menu.

### Settings

mpv.net is able to share the settings with mpv and mpv.net uses the same logic to decide from where the settings are loaded. The default location is:

```Text
C:\Users\%username%\AppData\Roaming\mpv\mpv.conf
```

If a directory named portable_config next to the mpvnet.exe exists, all config will be loaded from this directory only.

```Text
<startup>\portable_config\mpv.conf
```

In case there isn't any config folder mpv.net asks where to create it. If no mpv.conf file exists mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpvConf.txt>

Config files located in the same directory as mpvnet.exe are loaded with lower priority. Some config files are loaded only once, which means that e.g. of 2 input.conf files located in two config directories, only the one from the directory with higher priority will be loaded.

The key bindings and the context menu definitions are stored in the input.conf file,
if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt>

mpv.net supports almost all mpv settings and features, [limitations are listed in the wiki](https://github.com/stax76/mpv.net/wiki/Limitations).

### Scripting

[Scripting wiki page](https://github.com/stax76/mpv.net/wiki/Scripting)

### Add-ons

[Add-on wiki page](https://github.com/stax76/mpv.net/wiki/Addons)

### Architecture

mpv.net is written in C# 7.0 and runs on the .NET framework 4.7 or higher.

The Add-on implementation is based on the [Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/).

There are no specific extension or scripting interfaces but instead everyting
is accessible for .NET compatible languages (C#, VB.NET, F#, Python, PowerShell),
this decision was made to keep the code simple and lightweight.

Python scripting is implemented with IronPython which uses Python 2.7.

The main/video window is WinForms based, all other windows are WPF based.

The config editor adds it's controls dynamically and uses TOML to define it's
content, there are only two simple types, StringSetting and OptionSetting.

mpv.net was started 2017 and consists of about 6000 lines of code and markup.

IDE, Editor: Visual Studio, Visual Studio Code.

Due to mpv.net being my first WPF app and mpv.net never meant to be a large
application best practices and design pattern are not always applied.

Third party components:

- [libmpv, the heard and soul of mpv.net](https://mpv.io/)
- [MediaInfo, no media related project could do without](https://mediaarea.net/en/MediaInfo)
- [Tommy, a single file TOML parser](https://github.com/dezhidki/Tommy)
- [IronPython, bringing Python to dotnet](https://ironpython.net/)
- [CS-Script, scripting with C#](http://www.csscript.net/)
- [Everything, a blazing fast file search service](https://www.voidtools.com)

### Support

[Support thread in Doom9 forum](https://forum.doom9.org/showthread.php?t=174841)

[Support thread in VideoHelp forum](https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows)

[Issue tracker to report bugs and request features](https://github.com/stax76/mpv.net/issues)

[frank.skare.de@gmail.com](mailto:frank.skare.de@gmail.com?Subject=mpv.net%20support)

Please click on the star at the top of the page and like mpv.net at [alternativeto.net](https://alternativeto.net/software/mpv-net/).

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

### Changelog

[Changelog](Changelog.md)

### Download

<https://github.com/stax76/mpv.net/releases>