
mpv.net manual
==============

**ENGLISH** | **[简体中文](Manual_chs.md)**

Table of contents
-----------------

* [About](#about)
* [Download](#download)
* [Installation](#installation)
* [Support](#support)
* [Settings](#settings)
* [Input and context menu](#input-and-context-menu)
* [Command Line Interface](#command-line-interface)
* [Terminal](#terminal)
* [mpv.net specific options](#mpvnet-specific-options)
* [External Tools](#external-tools)
* [Scripting](#scripting)
* [Extensions](#extensions)
* [Color Theme](#color-theme)
* [Advanced Features](#advanced-features)
* [Hidden Features](#hidden-features)
* [Differences compared to mpv](#differences-compared-to-mpv)
* [Technical Overview](#technical-overview)
* [Context Menu Commands](#context-menu)


About
-----

mpv.net is a modern desktop media player for Windows based on the popular mpv player.

mpv.net is designed to be mpv compatible, almost all mpv features are available
because they are all contained in libmpv, this means the official
[mpv manual](https://mpv.io/manual/master/) applies to mpv.net.

mpv focuses on the usage of the command line and the terminal,
mpv.net retains the ability to be used from the command line and
the terminal and adds a modern Windows GUI on top of it.


Download
--------

[Changelog](Changelog.md)


### Stable

[Release page](../../../releases)


### Beta

[OneDrive](https://1drv.ms/u/s!ArwKS_ZUR01g1ldoLA90tX9DzKTj?e=xITXbC)

[DropBox](https://www.dropbox.com/sh/t54p9igdwvllbpl/AADKyWpaFnIhdyosxyP5d3_xa?dl=0)


Installation
------------

mpv.net requires the .NET Framework 4.8 and Windows 7 or higher and a modern graphics card.

There is a setup exe and a portable zip file download.

An old version should be uninstalled before installing a new version,
it's generally not a good idea to install a new version on top of an old version,
the setup don't enforce it because it's not easy to implement.

For internet streaming youtube-dl must be downloaded and installed manually,
meaning it must be located in the PATH environment variable or in the startup directory.

mpvnet.exe is platform agnostic, users that need x86 have to replace 3 native tools:

- mpv-1.dll
- MediaInfo.dll
- mpvnet.com


#### File Associations

File Associations can be created using the context menu under 'Tools > Setup'.

After the file associations were registered, go to the Windows settings under
'Settings > Apps > Default apps' or shell execute `ms-settings:defaultapps` and choose
mpv.net as default app for Video and optionally for Audio and Images.

It's possible to change the default application using the 'Open with' feature
of the context menu in File Explorer.

[Open with++](#open-with) can be used to extend the File Explorer context menu
to get menu items for [Play with mpv.net](https://github.com/stax76/OpenWithPlusPlus#play-with-mpvnet) and
[Add to mpv.net playlist](https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist).

When multiple files are selected in File Explorer and enter is pressed then
the files are opened in mpv.net in random order, this works with maximum 15 files.


Support
-------

Before making a support request, please try a newer [beta version](#beta) first.

Bugs and feature requests can be made on the github [issue tracker](../../../issues),
feel free to use for anything mpv.net related, usage questions are welcome.

Or use the [support thread](https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows) in the VideoHelp forum.


Settings
--------

mpv.net searches the config folder at:

1. startup\portable_config
2. %APPDATA%\mpv.net (`C:\Users\%USERNAME%\AppData\Roaming\mpv.net`)

mpv options are stored in the file mpv.conf,
mpv.net options are stored in the file mpvnet.conf,
mpv.net options are documented [here](#mpvnet-specific-options).


Input and context menu
----------------------

The input (key/mouse) bindings and the context menu definitions are stored in the
input.conf file, if it's missing mpv.net generates it with the following defaults:

[input.conf defaults](../../../tree/master/src/Resources/input.conf.txt)

Global hotkeys are supported via global-input.conf file.

The config folder can be opened from the context menu: `Settings > Open Config Folder`


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

https://mpv.io/manual/master/#properties


mpv.net has a feature to list all properties:

_Context Menu > View > Show Properties_


Non property switches are generally not supported in mpv.net!


Terminal
--------

When mpv.net is started from a terminal it will output status,
error and debug messages to the terminal and accept input keys from the terminal.

In the context menu under _Tools > Setup_ a button can be found to add
mpv.net to the path environment variable.

A common task for the terminal is debugging scripts.


mpv.net specific options
------------------------

mpv.net specific options can be found in the conf editor searching for 'mpv.net'.

The options are saved in the mpvnet.conf file.

#### --queue \<files\>

Adds files to the playlist, requires [--process-instance=single](#--process-instancevalue).
[Open with++](#open-with) can be used to add files to the playlist using File Explorer.

#### --command=\<input command\>

Sends a input command to a running mpv.net instance via command line, for instance
to create global hotkeys with AutoHotkey. Requires [process-instance=single](#--process-instancevalue).

### Audio

#### --remember-volume=\<yes|no\>

Save volume and mute on exit and restore it on start. Default: yes


### Screen

#### --start-size=\<value\>

Setting to remember the window size.

**width-session**  
Width is remembered in the current session.

**width-always**  
Width is always remembered.

**height-session**  
Height is remembered in the current session. Default

**height-always**  
Height is always remembered.

**video**  
Window size is set to video resolution.

**session**  
Window size is remembered in the current session.

**always**  
Window size is always remembered.


#### --start-threshold=\<milliseconds\>

Threshold in milliseconds to wait for libmpv returning the video
resolution before the window is shown, otherwise default dimensions
are used as defined by autofit and start-size. Default: 1500


#### --minimum-aspect-ratio=\<float\>

Minimum aspect ratio, if the AR is smaller than the defined value then
the window AR is set to 16/9. This avoids a square window for Music
with cover art. Default: 1.2


#### --remember-window-position=\<yes|no\>

Save the window position on exit. Default: no


### Playback

#### --auto-load-folder=\<yes|no\>

For single files automatically load the entire directory into the playlist.
Can be suppressed via shift key. Default: yes


### General

#### --process-instance=\<value\>

Defines if more then one mpv.net process is allowed.

Tip: Whenever the control key is pressed when files or URLs are opened,
the playlist is not cleared but the files or URLs are appended to the playlist.
This not only works on process startup but in all mpv.net features that open files and URLs.

**multi**  
Create a new process everytime the shell starts mpv.net.

**single**  
Force a single process everytime the shell starts mpv.net. Default

**queue**  
Force a single process and add files to playlist.


#### --recent-count=\<int\>

Amount of recent files to be remembered. Default: 15


#### --video-file-extensions=\<string\>

Video file extensions used to create file associations and used by the auto-load-folder feature.


#### --audio-file-extensions=\<string\>

Audio file extensions used to create file associations and used by the auto-load-folder feature.


#### --image-file-extensions=\<string\>

Image file extensions used to create file associations and used by the auto-load-folder feature.


#### --debug-mode=\<yes|no\>

Enable this only when a developer asks for it. Default: no


### UI

#### --dark-mode=\<value\>

Enables a dark theme.

**always**  
Default

**system**  
Available on Windows 10 or higher.

**never**


#### ---dark-theme=\<string\>

Color theme used in dark mode. Default: dark

[Color Themes](#color-theme)


#### --light-theme=\<string\>

Color theme used in light mode. Default: light

[Color Themes](#color-theme)


External Tools
--------------

### Play with mpv

In order to play videos from sites such as YouTube the Chrome Extension [Play with mpv](https://chrome.google.com/webstore/detail/play-with-mpv/hahklcmnfgffdlchjigehabfbiigleji) can be used.

Due to Chrome Extensions not being able to start a app, another app that communicates with the extension is required, this app can be downloaded [here](http://www.mediafire.com/file/lezj8lwqt5zf75v/play-with-mpvnet-server.7z/file). The extension works only when the app is running, to have the app always running a link can be created in the auto start folder located at:

`C:\Users\%username%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup`

This will start the app on system start and have it running in the background. When the file association registration of mpv.net was executed then the app should find the location of mpv.net, alternativly the mpv.net folder can be added to the Path environment variable.


### Open With

Alternatively the Chrome/Firefox extension [Open With](../../../issues/119) can be used.


### Open with++

[Open with++](https://github.com/stax76/OpenWithPlusPlus) can be used to extend the File Explorer context menu to get menu items for [Play with mpv.net](https://github.com/stax76/OpenWithPlusPlus#play-with-mpvnet) and [Add to mpv.net playlist](https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist).


### Universal Remote Android app

Universal Remote is Android remote control app which costs 5 €.

https://www.unifiedremote.com

https://play.google.com/store/apps/details?id=com.Relmtech.Remote

https://play.google.com/store/apps/details?id=com.Relmtech.RemotePaid

https://www.unifiedremote.com/tutorials/how-to-create-a-custom-keyboard-shortcuts-remote

https://www.unifiedremote.com/tutorials/how-to-install-a-custom-remote

[My config](./Universal%20Remote)

Very useful is the Universal Remote File Browser feature.


### One For All Contour URC1210 and FLIRC USB

My primary remote control solution however is a One For All Contour URC1210
using Philips code 0556 together with FLIRC USB (gen2).


Scripting
---------

#### Lua

File Type: `lua`

Location: `<config folder>\scripts`

The Lua script host is built into libmpv.

There is no debugging support, only error and debug messages printed on the terminal.

Lua scripts are loaded before the first media file loads.

[mpv Lua documentation](https://mpv.io/manual/master/#lua-scripting)

[mpv user scripts](https://github.com/mpv-player/mpv/wiki/User-Scripts)


#### JavaScript

File Type: `js`

Location: `<config folder>\scripts`

The JavaScript script host is built into libmpv.

There is no debugging support, only error and debug messages printed on the terminal.

JavaScript scripts are loaded before the first media file loads.

[mpv JavaScript documentation](https://mpv.io/manual/master/#javascript)

[mpv user scripts](https://github.com/mpv-player/mpv/wiki/User-Scripts)


#### PowerShell

File Type: `ps1`

Location: `<config folder>\scripts-ps`

The PowerShell scripting host is like extensions not
initialized before media files are loaded.

mpv.net does not define scripting interfaces but instead exposed
its complete internals, there are no compatibility guaranties.

[Example Scripts](../../../tree/master/src/Scripts)


#### C#

File Type: `cs`

Location: `<config folder>\scripts-cs`

mpv.net does not define scripting interfaces but instead exposed
its complete internals, there are no compatibility guaranties.

Script code can be written within a C# [extension](../../../tree/master/src/Extensions),
that way full code completion and debugger support is available.
Once the code was developed and debugged, it can be moved
from the extension to a lightweight standalone script.

The C# scripting host is like [extensions](../../../tree/master/src/Extensions)
not initialized before media files are loaded.

[Example Scripts](../../../tree/master/src/Scripts)


Extensions
----------

Extensions are located in a subfolder _extensions_ in the config folder
and the filename must have the same name as the directory:

```Text
<config folder>\extensions\ExampleExtension\ExampleExtension.dll
```

mpv.net does not define extension interfaces but instead exposed
its complete internals, there are no compatibility guaranties.


### Walkthrough creating an extension

- Download and install [Visual Studio Community](https://visualstudio.microsoft.com).
- Create a new project of type **Class Library .NET Framework**
  and ensure the project name ends with **Extension**.
- Add a reference to **System.ComponentModel.Composition**.
- Add a reference to mpvnet.exe, select the mpvnet reference
  in the Solution Explorer, open the Properties window and set
  **Copy Local** to false to prevent mpvnet.exe being copied
  to the output directory when the project is built.
- Now open the project properties and set the output path in the Build tab,
  extensions are like scripts located in your config folder, example:
  `<config folder>\extensions\ExampleExtension\ExampleExtension.dll`
- Also in the project properties choose the option **Start external program**
  in the Debug tab and define the path to mpvnet.exe. In the Debug tab you may also
  define command line arguments like a video file to be played when you start debugging.


### Sample Code

#### RatingExtension

This extension writes a rating to the filename of rated videos when mpv.net shuts down.

The input.conf defaults contain key bindings for this extension to set ratings.

[Source Code](../../../tree/master/src/Extensions)


Color Theme
-----------

mpv.net supports custom color themes, the definition of the built-in themes can be found at:

[theme.txt](../../../tree/master/src/Resources/theme.txt)


Custom themes can be saved at:

`<conf folder>\theme.conf`

The theme.conf file may contain an unlimited amount of themes.

In the config editor under UI there are the settings dark-theme and
light-theme to define the themes used in dark and in light mode.


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

Selecting multiple files in File Explorer and pressing enter will
open the files in mpv.net. Explorer restricts this to maximum 15 files
and the order will be random.

Whenever the control key is pressed when files or URLs are opened,
the playlist is not cleared but the files or URLs are appended to the playlist.
This works in all mpv.net features that open files or URLs.

Pressing the shift key while opening a single file will suppress loading
all files of the folder into the playlist.

In fullscreen mode clicking the top right corner closes the player.


Differences compared to mpv
---------------------------

mpv.net is designed to work exactly like mpv, there are a few limitations:


### Window Limitations

mpv.net implements an own main window which means only mpv window
features are supported that have an own implementation in mpv.net.

A window free mode is currently not supported, the main window is always
visible, even when mpv.net is started from the terminal and music is played.

The documentation of mpv's window features can be found here:

https://mpv.io/manual/master/#window


**mpv.net has currently implemented the following window properties:**

- [border](https://mpv.io/manual/master/#options-border)
- [fullscreen](https://mpv.io/manual/master/#options-fullscreen)
- [keepaspect-window](https://mpv.io/manual/master/#options-keepaspect-window)
- [ontop](https://mpv.io/manual/master/#options-ontop)
- [screen](https://mpv.io/manual/master/#options-screen)
- [title](https://mpv.io/manual/master/#options-title)
- [window-maximized](https://mpv.io/manual/master/#options-window-maximized)
- [window-minimized](https://mpv.io/manual/master/#options-window-minimized)
- [window-scale](https://mpv.io/manual/master/#options-window-scale)


**Partly implemented are:**

- [autofit-larger](https://mpv.io/manual/master/#options-autofit-larger)
- [autofit-smaller](https://mpv.io/manual/master/#options-autofit-smaller)
- [autofit](https://mpv.io/manual/master/#options-autofit)


mpv.net specific window features are documented in the [screen section](#screen).


### Command Line Limitations

mpv.net supports property based mpv command line options which means it supports
almost all mpv command line options.

What is not supported are non property bases options. Non property based options
need an own implementation in mpv.net, so far implemented are:

--ad=help  
--audio-device=help  
--input-keylist  
--profile=help  
--vd=help  
--version  


### mpv.net specific options

Options that are specific to mpv.net can be found by entering _mpv.net_
in the search field of the config editor, in the mpv.net manual they are
documented [here](#mpvnet-specific-options).

mpv.net specific options are saved in the file mpvnet.conf and are just
as mpv properties available on the command line.


Technical Overview
------------------

mpv.net is written in C# 7 and runs on the .NET Framework 4.8.

The Extension implementation is based on the
[Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/).

The main window is WinForms based because WinForms allows better libmpv integration
compared to WPF, all other windows are WPF based.

Third party components are:

- [libmpv provides the core functionality](https://mpv.io/)
- [MediaInfo](https://mediaarea.net/en/MediaInfo)


Context Menu
------------

The context menu of mpv.net is defined in the file input.conf which is
located in the config directory.

If the input.conf file does not exists mpv.net generates it with the following defaults:

<https://github.com/stax76/mpv.net/tree/master/src/Resources/input.conf.txt>

input.conf defines mpv's key and mouse bindings and mpv.net uses
comments to define the context menu.


### Open > Open Files

The Open Files menu entry is one way to open files in mpv.net, it supports multi selection.

Another way to open files is the command line which is used by
File Explorer for existing associations.

A third way is to drag and drop files on the main window.

Whenever the control key is pressed when files or URLs are opened,
the playlist is not cleared but the files or URLs are appended to the
playlist. This works in all mpv.net features that open files or URLs.

Pressing the shift key while opening a single file will suppress loading all files in the folder.

Blu-ray and DVD ISO image files are supported.


### Open > Open URL or file path from clipboard

Opens files and URLs from the clipboard. How to open URLs directly
from the browser from sites like YouTube is described in the
[External Tools section](#external-tools).

For internet streaming youtube-dl must be downloaded and installed manually,
meaning it must be located in the PATH environment variable or in the startup directory.


### Open > Open DVD/Blu-ray Drive/Folder

Opens a DVD/Blu-ray Drive/Folder.


### Open > Load external audio files

Allows to load an external audio file. It's also possible to auto detect
external audio files based on the file name, the option for this can be
found in the settings under 'Settings > Show Config Editor > Audio > audio-file-auto'.


### Open > Load external subtitle files

Allows to load an external subtitle file. It's also possible to auto detect
external subtitle files based on the file name, the option for this can be
found in the settings under 'Settings > Show Config Editor > Subtitles > sub-auto'.


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

`cycle-values video-aspect 16:9 4:3 2.35:1 -1`

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

Shows a text file that contains the file history. If the file don't exist
it asks if the file should be created in the settings folder. Once the file
exist then the history is logged. It logges the playback history containing
the time and filename.

To ignore certain paths:

script-opt = history-discard=path1;path2

### Tools > Set/clear A-B loop points

Enables to set loop start and end points using the following command:

`ab-loop`

[ab-loop command](https://mpv.io/manual/master/#command-interface-ab-loop)


### Tools > Toggle infinite file looping

Loops the current file infinitely using the following command:

`cycle-values loop-file "inf" "no"`

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


### Help > About mpv.net

Shows the mpv.net about dialog which shows a copyright notice, the versions of mpv.net and libmpv and a license notice (GPL v2).


### Exit

Exits mpv.net using the following command:

`quit`

[quit command](https://mpv.io/manual/master/#command-interface-quit-[%3Ccode%3E])


### Exit Watch Later

Exits mpv.net and remembers the position in the file using the following command:

`quit-watch-later`

[quit-watch-later command](https://mpv.io/manual/master/#command-interface-quit-watch-later)
