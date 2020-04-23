
mpv.net manual
==============

Table of contents
-----------------

* [About](#about)
* [Download](#download)
* [Installation](#installation)
* [Support](#support)
* [Settings](#settings)
* [Command Line Interface](#command-line-interface)
* [Terminal](#terminal)
* [External Tools](#external-tools)
* [Scripting](#scripting)
* [Extensions](#extensions)
* [Color Theme](#color-theme)
* [Advanced Features](#advanced-features)
* [Differences](#differences)
* [Technical Overview](#technical-overview)
* [Context Menu](#context-menu)
  + [Open > Open Files](#open--open-files)
  + [Open > Open URL or file path from clipboard](#open--open-url-or-file-path-from-clipboard)
  + [Open > Open DVD/Blu-ray Drive/Folder](#open--open-dvd-blu-ray-drive-folder)
  + [Open > Show media search](#open--show-media-search)
  + [Open > Load external audio files](#open--load-external-audio-files)
  + [Open > Load external subtitle files](#open--load-external-subtitle-files)
  + [Play/Pause](#play-pause)
  + [Stop](#stop)
  + [Toggle Fullscreen](#toggle-fullscreen)
  + [Navigate > Previous File](#navigate--previous-file)
  + [Navigate > Next File](#navigate--next-file)
  + [Navigate > Next Chapter](#navigate--next-chapter)
  + [Navigate > Previous Chapter](#navigate--previous-chapter)
  + [Navigate > Jump Next Frame](#navigate--jump-next-frame)
  + [Navigate > Jump Previous Frame](#navigate--jump-previous-frame)
  + [Navigate > Jump](#navigate--jump)
  + [Pan & Scan > Increase Size](#pan--scan--increase-size)
  + [Pan & Scan > Decrease Size](#pan--scan--decrease-size)
  + [Pan & Scan > Move Left](#pan--scan--move-left)
  + [Pan & Scan > Move Right](#pan--scan--move-right)
  + [Pan & Scan > Move Up](#pan--scan--move-up)
  + [Pan & Scan > Move Down](#pan--scan--move-down)
  + [Pan & Scan > Decrease Height](#pan--scan--decrease-height)
  + [Pan & Scan > Increase Height](#pan--scan--increase-height)
  + [Pan & Scan > Reset](#pan--scan--reset)
  + [Video > Decrease Contrast](#video--decrease-contrast)
  + [Video > Increase Contrast](#video--increase-contrast)
  + [Video > Decrease Brightness](#video--decrease-brightness)
  + [Video > Increase Brightness](#video--increase-brightness)
  + [Video > Decrease Gamma](#video--decrease-gamma)
  + [Video > Increase Gamma](#video--increase-gamma)
  + [Video > Decrease Saturation](#video--decrease-saturation)
  + [Video > Increase Saturation](#video--increase-saturation)
  + [Video > Take Screenshot](#video--take-screenshot)
  + [Video > Toggle Deinterlace](#video--toggle-deinterlace)
  + [Video > Cycle Aspect Ratio](#video--cycle-aspect-ratio)
  + [Audio > Cycle/Next](#audio--cycle-next)
  + [Audio > Delay +0.1](#audio--delay--01)
  + [Audio > Delay -0.1](#audio--delay--01)
  + [Subtitle > Cycle/Next](#subtitle--cycle-next)
  + [Subtitle > Toggle Visibility](#subtitle--toggle-visibility)
  + [Subtitle > Delay -0.1](#subtitle--delay--01)
  + [Subtitle > Delay 0.1](#subtitle--delay-01)
  + [Subtitle > Move Up](#subtitle--move-up)
  + [Subtitle > Move Down](#subtitle--move-down)
  + [Subtitle > Decrease Subtitle Font Size](#subtitle--decrease-subtitle-font-size)
  + [Subtitle > Increase Subtitle Font Size](#subtitle--increase-subtitle-font-size)
  + [Volume > Up](#volume--up)
  + [Volume > Down](#volume--down)
  + [Volume > Mute](#volume--mute)
  + [Speed > -10%](#speed---10-)
  + [Speed > 10%](#speed--10-)
  + [Speed > Half](#speed--half)
  + [Speed > Double](#speed--double)
  + [Speed > Reset](#speed--reset)
  + [Extensions > Rating > 0stars](#extensions--rating--0stars)
  + [View > On Top > Enable](#view--on-top--enable)
  + [View > On Top > Disable](#view--on-top--disable)
  + [View > File Info](#view--file-info)
  + [View > Show Statistics](#view--show-statistics)
  + [View > Toggle Statistics](#view--toggle-statistics)
  + [View > Toggle OSC Visibility](#view--toggle-osc-visibility)
  + [View > Show Playlist](#view--show-playlist)
  + [View > Show Audio/Video/Subtitle List](#view--show-audio-video-subtitle-list)
  + [Settings > Show Config Editor](#settings--show-config-editor)
  + [Settings > Show Input Editor](#settings--show-input-editor)
  + [Settings > Open Config Folder](#settings--open-config-folder)
  + [Tools > Command Palette](#tools--command-palette)
  + [Tools > Show History](#tools--show-history)
  + [Tools > Set/clear A-B loop points](#tools--set-clear-a-b-loop-points)
  + [Tools > Toggle infinite file looping](#tools--toggle-infinite-file-looping)
  + [Tools > Toggle Hardware Decoding](#tools--toggle-hardware-decoding)
  + [Tools > Setup](#tools--setup)
  + [Help > Show mpv manual](#help--show-mpv-manual)
  + [Help > Show mpv.net manual](#help--show-mpvnet-manual)
  + [Help > Check for Updates](#help--check-for-updates)
  + [Help > About mpv.net](#help--about-mpvnet)
  + [Exit](#exit)
  + [Exit Watch Later](#exit-watch-later)


About
-----

mpv.net is a modern desktop media player for Windows based on mpv. mpv is a media player based on MPlayer and mplayer2.

libmpv provides the majority of the features of the mpv player. mpv focuses on the usage of the command line interface, mpv.net retains the ability to be used from the command line and adds a modern Windows GUI on top of it.

mpv.net is designed to be mpv compatible, almost all mpv features are available because they are all contained in libmpv, this means the official [mpv manual](https://mpv.io/manual/master/) applies to mpv.net.


Download
--------

[Changelog](Changelog.md)


### Stable

[Release page](releases)


### Beta

[OneDrive](https://1drv.ms/u/s!ArwKS_ZUR01g1ldoLA90tX9DzKTj?e=xITXbC)

[DropBox](https://www.dropbox.com/sh/t54p9igdwvllbpl/AADKyWpaFnIhdyosxyP5d3_xa?dl=0)


Installation
------------

mpv.net requires the .NET Framework 4.8 and Windows 7 or 10 and a modern graphics card.

There is a setup and a portable download in the 7zip and Zip archive format.

x64 editions have the advantage of being typically better optimized and tested.


### Scoop

```
scoop bucket add extras
scoop install mpv.net
```


### AppGet

`appget install mpv-net`


### Chocolatey

`choco install mpvnet.install`


#### File Associations

File Associations can be created using the setup or with the context menu under 'Tools > Setup'.

After the file associations were registered, go to the Windows settings under 'Settings > Apps > Default apps' or shell execute `ms-settings:defaultapps` and choose mpv.net as default app for Video and optionally for Audio and Images.

It's possible to change the default application using the 'Open with' feature of the context menu in File Explorer.

[Open with++](#open-with) can be used to extend the File Explorer context menu to get menu items for 'Play with mpv.net' and 'Add to mpv.net playlist'.


Support
-------

Before making a support request, please try a newer [beta version](#beta) first.

[Support thread in VideoHelp forum](https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows)

[Issue tracker](https://github.com/stax76/mpv.net/issues), feel free to use for anything mpv.net related.

You can support my work with a PayPal donation. The input hardware support in mpv.net is not 100% mpv compatible, people use all kind of weird input hardware and sometimes I have to buy those to support them.

<https://www.paypal.me/stax76>


Settings
--------

When mpv.net finds no config folder on startup it will ask for a location.

If a folder named portable_config next to the mpvnet.exe exists,
all config will be loaded from this folder only.

```Text
<startup>\portable_config\
```

mpv specific settings are stored in the file mpv.conf, if no mpv.conf file exists
mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpv.conf.txt>

mpv.net specific settings are stored in the file mpvnet.conf.

The input (key/mouse) bindings and the context menu definitions are stored in the
input.conf file, if it's missing mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt>

mpv.net supports almost all mpv settings and features.

The config folder can be opened from the context menu (`Settings > Open Config Folder`).


Command Line Interface
----------------------

**mpvnet** [options] [file|URL|PLAYLIST|-]  
**mpvnet** [options] files


mpv properties can be set with the same syntax as mpv, that is:


To enable the border property:

`--border` or `--border=yes`


To disable the border property:

`--no-boder` or `--border=no`


Supported are all mpv properties, they are documented here:

<https://mpv.io/manual/master/#properties>


mpv.net has a feature to list all properties:

_Context Menu > View > Show Properties_


Non property switches are generally not supported in mpv.net!


Terminal
--------

When mpv.net is started from a terminal it will output status, error and debug messages to the terminal and accept input keys from the terminal.

In the context menu under 'Tools > Setup' a button can be found to add mpv.net to the path environment variable.

A common task for the terminal is debugging scripts.


External Tools
--------------

### Play with mpv

In order to play videos from sites such as YouTube the Chrome Extension [Play with mpv](https://chrome.google.com/webstore/detail/play-with-mpv/hahklcmnfgffdlchjigehabfbiigleji) can be used.

Due to Chrome Extensions not being able to start a app, another app that communicates with the extension is required, this app can be downloaded [here](http://www.mediafire.com/file/lezj8lwqt5zf75v/play-with-mpvnet-server.7z/file). The extension works only when the app is running, to have the app always running a link can be created in the auto start folder located at:

`C:\Users\%username%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup`

This will start the app on system start and have it running in the background. When the file association registration of mpv.net was executed then the app should find the location of mpv.net, alternativly the mpv.net folder can be added to the Path environment variable.


### Open With

Alternatively he Chrome/Firefox extension [Open With](https://github.com/stax76/mpv.net/issues/119) can be used.


### Open with++

Open with++ allows to customize the File Explorer context menu to add menu items 'Play with mpv.net' and 'Add to mpv.net playlist'.

https://github.com/stax76/OpenWithPlusPlus

https://github.com/stax76/OpenWithPlusPlus#mpvnet

https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist


### MediaInfo.NET

MediaInfo.NET is a media info GUI.

https://github.com/stax76/MediaInfo.NET

To start a external application mpv has the run input command (it does not use shell execute).

If the path has spaces then it must be enclosed in quotes and then double backslashes must be used for escaping or alternatively forward slashes.

 `_ run D:\Software\MediaInfoNET.exe "${path}" #menu: Tools > Open file with MediaInfo.NET`

 `_ run "D:\\Soft ware\\MediaInfoNET.exe" "${path}" #menu: Tools > Open file with MediaInfo.NET`

 `_ run "D:/Soft ware/MediaInfoNET.exe" "${path}" #menu: Tools > Open file with MediaInfo.NET`


Scripting
---------

There is no debugger support available for the scripting hosts, mpv.net can be started from a terminal, 
script errors and debug messages are printed then on the terminal.


#### Lua

File Type: `lua`

Location: `<config folder>\scripts`

Lua and JavaScript scripts are loaded before the first media file loads.

[mpv Lua documentation](https://mpv.io/manual/master/#lua-scripting)

[mpv user scripts](https://github.com/mpv-player/mpv/wiki/User-Scripts)


#### JavaScript

File Type: `js`

Location: `<config folder>\scripts`

Lua and JavaScript scripts are loaded before the first media file loads.

[mpv JavaScript documentation](https://mpv.io/manual/master/#javascript)

[mpv user scripts](https://github.com/mpv-player/mpv/wiki/User-Scripts)


#### C#

File Type: `cs`

Location: `<config folder>\scripts-cs`

C# scripting in mpv.net is implemented with a C# [extension](#extensions) and [CS-Script](https://www.cs-script.net/).

mpv.net does not define scripting interfaces but instead exposed its complete internals, there are no compatibility guaranties.

Script code can be written within a C# [extension](#extensions), that way full code completion and debugger support is available. Once the code was developed and debugged, the code can be moved from the extension to a lightweight standalone script.

The C# scripting host is like [extensions](#extensions) not initialized before media files are loaded.

[Example Scripts](scripts/examples)


#### PowerShell

File Type: `ps1`

Location: `<config folder>\scripts-ps`

The PowerShell scripting host is like extensions not initialized before media files are loaded.

mpv.net does not define scripting interfaces but instead exposed its complete internals, there are no compatibility guaranties.

[Example Scripts](scripts/examples)


Extensions
----------

Extensions are located in the config folder and the filename must end with 'Extension.dll':

```Text
<config folder>\Extensions\ExampleExtension\ExampleExtension.dll
```

mpv.net does not define extension interfaces but instead exposed its complete internals, there are no compatibility guaranties.


### Walkthrough creating an extension

- Download and install [Visual Studio Community](https://visualstudio.microsoft.com).
- Create a new project of type **Class Library .NET Framework** and ensure the project name ends with **Extension**.
- Add a reference to **System.ComponentModel.Composition**.
- Add a reference to mpvnet.exe, select the mpvnet reference in the Solution Explorer, open the Properties window and set **Copy Local** to false to prevent mpvnet.exe being copied to the output directory when the project is built.
- Now open the project properties and set the output path in the Build tab, extensions are like scripts located in your config folder, example: `<config folder>\extensions\ExampleExtension\ExampleExtension.dll`
- Also in the project properties choose the option **Start external program** in the Debug tab and define the path to mpvnet.exe. In the Debug tab you may also define command line arguments like a video file to be played when you start debugging.


### Sample Code

#### ScriptingExtension

The ScriptingExtension implements the C# scripting host using [CS-Script](https://www.cs-script.net/).

I use this extension as well to develop and debug C# scripts. Once the code was developed and debugged, I move the code from the extension to a standalone script.

[Source Code](extensions)


#### RatingExtension

This extension writes a rating to the filename of rated videos when mpv.net shuts down.

The input.conf defaults contain key bindings for this extension to set ratings.

[Source Code](extensions)


Color Theme
-----------

mpv.net supports custom color themes, the definition of the built-in themes can be found at:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/theme.txt>


Custom themes can be saved at:

`<conf folder>\theme.conf`

The theme.conf file may contain an unlimited amount of themes.

In the config editor under UI there are the settings dark-theme and light-theme to define the themes used in dark and in light mode.


Advanced Features
-----------------

### Playback of VapourSynth scripts

vpy files are supported with following mpv.conf configuration:

```
[extension.vpy]
demuxer-lavf-format = vapoursynth
```

Python and VapourSynth must be in the path environment variable.


Hidden Features
---------------

Selecting multiple files in File Explorer and pressing enter will open the files in mpv.net. Explorer restricts this to maximum 15 files and the order will be random.

Whenever the control key is pressed when files or URLs are opened, the playlist is not cleared but the files or URLs are appended to the playlist. This works in all mpv.net features that open files or URLs.

Pressing the shift key while opening a single file will suppress loading all files in the folder.


Differences
-----------

mpv.net is designed to work exactly like mpv, there are a few limitations:


### Window Limitations

mpv.net implements an own main window which means only mpv window features are supported that have an own implementation in mpv.net.

A window free mode is currently not supported.

The documentation of mpvs window features can be found here:

<https://mpv.io/manual/master/#window>


mpv.net has currently implemented the following window features:

[screen](https://mpv.io/manual/master/#options-screen)

[fullscreen](https://mpv.io/manual/master/#options-fullscreen)

[ontop](https://mpv.io/manual/master/#options-ontop)

[border](https://mpv.io/manual/master/#options-border)

[window-minimized](https://mpv.io/manual/master/#options-window-minimized)

[window-maximized](https://mpv.io/manual/master/#options-window-maximized)


**Partly implemented are:**

[autofit](https://mpv.io/manual/master/#options-autofit)

[autofit-smaller](https://mpv.io/manual/master/#options-autofit-smaller)

[autofit-larger](https://mpv.io/manual/master/#options-autofit-larger)


### Command Line Limitations

mpv.net supports only property switches, it does not support non property switches.


Technical Overview
------------------

mpv.net is written in C# 7 and runs on the .NET Framework 4.8.

The Extension implementation is based on the [Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/).

The main window is WinForms based because WinForms allows better libmpv integration compared to WPF, all other windows are WPF based.

The config editor adds it's controls dynamically and uses [TOML](https://en.wikipedia.org/wiki/TOML) to define it's
content.


Third party components:

- [libmpv provides the core functionality](https://mpv.io/)
- [MediaInfo](https://mediaarea.net/en/MediaInfo)
- [Tommy, a single file TOML parser](https://github.com/dezhidki/Tommy)
- [CS-Script, scripting with C#](http://www.csscript.net/)
- [Everything, a fast file search service](https://www.voidtools.com)


Context Menu
------------

The context menu of mpv.net is defined in the file input.conf which is located in the config directory.

If the input.conf file does not exists mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/input.conf.txt>

input.conf defines mpvs key and mouse bindings and mpv.net uses comments to define the context menu.


### Open > Open Files

The Open Files menu entry is one way to open files in mpv.net, it supports multi selection.

Another way to open files is the command line, it is used by the File Explorer if file associations exist.

When mpv.net is started from a terminal, mpv.net outputs status and debug messages on the terminal. 

A third way is to drag and drop files on the main window.

Whenever the control key is pressed when files or URLs are opened, the playlist is not cleared but the files or URLs are appended to the playlist. This works in all mpv.net features that open files or URLs.

Pressing the shift key while opening a single file will suppress loading all files in the folder.


### Open > Open URL or file path from clipboard

Opens files and URLs from the clipboard. How to open URLs directly from the browser from sites like YouTube is described in the [External Tools section](#external-tools).


### Open > Open DVD/Blu-ray Drive/Folder

Opens a DVD/Blu-ray Drive/Folder.


### Open > Show media search

mpv.net supports system wide media searches using the Everything indexing service installed by the popular file search tool [Everything](www.voidtools.com).


### Open > Load external audio files

Allows to load an external audio file. It's also possible to auto detect external audio files based on the file name, the option for this can be found in the settings under 'Settings > Show Config Editor > Audio > audio-file-auto'.


### Open > Load external subtitle files

Allows to load an external subtitle file. It's also possible to auto detect external subtitle files based on the file name, the option for this can be found in the settings under 'Settings > Show Config Editor > Subtitles > sub-auto'.


### Play/Pause

Play/Pause using the command:

`cycle pause`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[pause property](https://mpv.io/manual/master/#options-pause)


### Stop

Stops the player and unloads the playlist using the command:

`stop`

[stop command](https://mpv.io/manual/master/#command-interface-stop)


### Toggle Fullscreen

Toggles fullscreen using the command:

`cycle fullscreen`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[fullscreen property](https://mpv.io/manual/master/#options-fs)


### Navigate > Previous File

Navigates to the previous file in the playlist using the command:

`playlist-prev`

[playlist-prev command](https://mpv.io/manual/master/#command-interface-playlist-prev)


### Navigate > Next File

Navigates to the next file in the playlist using the command:

`playlist-next`

[playlist-next command](https://mpv.io/manual/master/#command-interface-playlist-next)


### Navigate > Next Chapter

Navigates to the next chapter using the command:

`add chapter 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[chapter property](https://mpv.io/manual/master/#command-interface-chapter)


### Navigate > Previous Chapter

Navigates to the previous chapter using the command:

`add chapter -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[chapter property](https://mpv.io/manual/master/#command-interface-chapter)


### Navigate > Jump Next Frame

Jumps to the next frame using the command:

`frame-step`

[frame-step command](https://mpv.io/manual/master/#command-interface-frame-step)


### Navigate > Jump Previous Frame

Jumps to the previous frame using the command:

`frame-back-step`

[frame-back-step command](https://mpv.io/manual/master/#command-interface-frame-back-step)


### Navigate > Jump

Seeking using the command:

`no-osd seek sec`

sec is the relative amount of seconds to jump, the no-osd prefix
is used because mpv.net includes a script that shows the position
when a seek operation is performed, the script uses a more simple
time format.

[no-osd command prefix](https://mpv.io/manual/master/#command-interface-no-osd)

[seek command](https://mpv.io/manual/master/#command-interface-seek-%3Ctarget%3E-[%3Cflags%3E])


### Pan & Scan > Increase Size

Adds video zoom using the command:

`add video-zoom  0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)


### Pan & Scan > Decrease Size

Adds negative video zoom using the command:

`add video-zoom  -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)


### Pan & Scan > Move Left

`add video-pan-x -0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)


### Pan & Scan > Move Right

`add video-pan-x 0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)


### Pan & Scan > Move Up

`add video-pan-y -0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)


### Pan & Scan > Move Down

`add video-pan-y 0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)


### Pan & Scan > Decrease Height

`add panscan -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[panscan property](https://mpv.io/manual/master/#options-panscan)


### Pan & Scan > Increase Height

`add panscan  0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[panscan property](https://mpv.io/manual/master/#options-panscan)


### Pan & Scan > Reset

Resets Pan & Scan, multiple commands in the same line are separated with semicolon.

`set video-zoom 0; set video-pan-x 0; set video-pan-y 0`

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)


### Video > Decrease Contrast

Decreases contrast with the following command:

`add contrast -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[contrast property](https://mpv.io/manual/master/#options-contrast)


### Video > Increase Contrast

Increases contrast with the following command:

`add contrast 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[contrast property](https://mpv.io/manual/master/#options-contrast)


### Video > Decrease Brightness

Decreases brightness using the following command:

`add brightness -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[brightness property](https://mpv.io/manual/master/#options-brightness)


### Video > Increase Brightness

Increases brightness using the following command:

`add brightness 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[brightness property](https://mpv.io/manual/master/#options-brightness)


### Video > Decrease Gamma

Decreases gamma using the following command:

`add gamma -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[gamma property](https://mpv.io/manual/master/#options-gamma)


### Video > Increase Gamma

Increases gamma using the following command:

`add gamma 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[gamma property](https://mpv.io/manual/master/#options-gamma)


### Video > Decrease Saturation

Decreases saturation using the following command:

`add saturation -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[saturation property](https://mpv.io/manual/master/#options-saturation)


### Video > Increase Saturation

Increases saturation using the following command:

`add saturation 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[saturation property](https://mpv.io/manual/master/#options-saturation)


### Video > Take Screenshot

`async screenshot`

[async command prefix](https://mpv.io/manual/master/#command-interface-async)

[screenshot command](https://mpv.io/manual/master/#command-interface-screenshot-%3Cflags%3E)


### Video > Toggle Deinterlace

Cycles the deinterlace property using the following command:

`cycle deinterlace`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[deinterlace property](https://mpv.io/manual/master/#options-deinterlace)


### Video > Cycle Aspect Ratio

Cycles the aspect ratio using the following command:

`cycle-values video-aspect "16:9" "4:3" "2.35:1" "-1"`

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[video-aspect property](https://mpv.io/manual/master/#command-interface-video-aspect)


### Audio > Cycle/Next

This uses a mpv.net command that shows better info then the mpv preset
and also has the advantage of not showing no audio.


### Audio > Delay +0.1

Adds a audio delay using the following command:

`add audio-delay 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[audio-delay property](https://mpv.io/manual/master/#options-audio-delay)


### Audio > Delay -0.1

Adds a negative audio delay using the following command:

`add audio-delay -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[audio-delay property](https://mpv.io/manual/master/#options-audio-delay)


### Subtitle > Cycle/Next

Shows the next subtitle track using the following command:

`cycle sub`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[sub/sid property](https://mpv.io/manual/master/#options-sid)


### Subtitle > Toggle Visibility

Cycles the subtitle visibility using the following command:

`cycle sub-visibility`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[sub-visibility property](https://mpv.io/manual/master/#options-no-sub-visibility)


### Subtitle > Delay -0.1

Adds a negative subtitle delay using the following command:

`add sub-delay -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-delay property](https://mpv.io/manual/master/#options-sub-delay)


### Subtitle > Delay 0.1

Adds a positive subtitle delay using the following command:

`add sub-delay 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-delay property](https://mpv.io/manual/master/#options-sub-delay)


### Subtitle > Move Up

Moves the subtitle up using the following command:

`add sub-pos -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-pos property](https://mpv.io/manual/master/#options-sub-pos)


### Subtitle > Move Down

Moves the subtitle down using the following command:

`add sub-pos 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-pos property](https://mpv.io/manual/master/#options-sub-pos)


### Subtitle > Decrease Subtitle Font Size

Decreases the subtitle font size using the following command:

`add sub-scale -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-scale property](https://mpv.io/manual/master/#options-sub-scale)


### Subtitle > Increase Subtitle Font Size

Increases the subtitle font size using the following command:

`add sub-scale 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-scale property](https://mpv.io/manual/master/#options-sub-scale)


### Volume > Up

Increases the volume using the following command:

`add volume 10`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)


### Volume > Down

Decreases the volume using the following command:

`add volume -10`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)


### Volume > Mute

Cycles the mute property using the following command:

`cycle mute`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[mute property](https://mpv.io/manual/master/#options-mute)


### Speed > -10%

Decreases the speed by 10% using the following command:

`multiply speed 1/1.1`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > 10%

Increases the speed by 10% using the following command:

`multiply speed 1.1`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > Half

Halfs the speed using the following command:

`multiply speed 0.5`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > Double

Doubles the speed using the following command:

`multiply speed 2`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > Reset

Resets the speed using the following command:

`set speed 1`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Extensions > Rating > 0stars

A plugin the writes the rating to the filename.


### View > On Top > Enable

Forces the player to stay on top of other windows using the following command:

`set ontop yes`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cname%3E-%3Cvalue%3E)

[ontop property](https://mpv.io/manual/master/#options-ontop)


### View > On Top > Disable

Disables the player to stay on top of other windows using the following command:

`set ontop no`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cname%3E-%3Cvalue%3E)

[ontop property](https://mpv.io/manual/master/#options-ontop)


### View > File Info

Shows info using a mpv.net command about the current file, shows length, position, formats, size and filename.


### View > Show Statistics

Show statistics using the following command:

`script-binding stats/display-stats`

[script-binding command](https://mpv.io/manual/master/#command-interface-script-binding)


### View > Toggle Statistics

Toggles statistics using the following command:

`script-binding stats/display-stats-toggle`

[script-binding command](https://mpv.io/manual/master/#command-interface-script-binding)


### View > Toggle OSC Visibility

Toggles OSC Visibility using the following command:

`script-binding osc/visibility`

[script-binding command](https://mpv.io/manual/master/#command-interface-script-binding)


### View > Show Playlist

Shows the playlist for 5 seconds using the following command:

`show-text ${playlist} 5000`

[show-text command](https://mpv.io/manual/master/#command-interface-show-text)


### View > Show Audio/Video/Subtitle List

Shows the Audio/Video/Subtitle list for 5 seconds using the following command:

`show-text ${track-list} 5000`

[show-text command](https://mpv.io/manual/master/#command-interface-show-text)


### Settings > Show Config Editor

Shows mpv.net's config editor.


### Settings > Show Input Editor

Shows mpv.net's key binding editor.


### Settings > Open Config Folder

Opens the config folder which contains:

mpv.conf file containing mpv settings

mpvnet.conf file containing mpv.net settings

input.conf containing mpv key and mouse bindings

User scripts and user extensions


### Tools > Command Palette

Shows the command palette window which allows to quickly find and execute commands and key shortcuts.


### Tools > Show History

Shows a text file that contains the file history. If the file don't exist it asks if the file should be created in the settings folder. Once the file exist then the history is logged. It logges the playback history containing the time and filename.


### Tools > Set/clear A-B loop points

Enables to set loop start and end points using the following command:

`ab-loop`

[ab-loop command](https://mpv.io/manual/master/#command-interface-ab-loop)


### Tools > Toggle infinite file looping

Loops the current file infinitely using the following command:

cycle-values loop-file "inf" "no"

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[loop-file command](https://mpv.io/manual/master/#options-loop)


### Tools > Toggle Hardware Decoding

Cycles the hwdec property to enable/disable hardware decoding using the following command:

`cycle-values hwdec "auto" "no"`

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[hwdec property](https://mpv.io/manual/master/#options-hwdec)


### Tools > Setup

Allows to manage file associations.


### Help > Show mpv manual

Shows the [mpv manual](https://mpv.io/manual/stable/).


### Help > Show mpv.net web site

Shows the [mpv.net web site](https://mpv-net.github.io/mpv.net-web-site/).


### Help > Show mpv.net manual

Shows the [mpv.net manual](https://github.com/stax76/mpv.net/blob/master/Manual.md).


### Help > Check for Updates

Checks for updates and allows to execute the update routine.

The update routine requires PowerShell 5 and curl, an up to date Windows 10 system has both included.


### Help > About mpv.net

Shows the mpv.net about dialog which shows a copyright notice, the versions of mpv.net and libmpv and a license notice (MIT).


### Exit

Exits mpv.net using the following command:

`quit`

[quit command](https://mpv.io/manual/master/#command-interface-quit-[%3Ccode%3E])


### Exit Watch Later

Exits mpv.net and remembers the position in the file using the following command:

`quit-watch-later`

[quit-watch-later command](https://mpv.io/manual/master/#command-interface-quit-watch-later)
