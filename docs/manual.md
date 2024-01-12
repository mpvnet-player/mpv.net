
mpv.net manual
==============

**ENGLISH** | **[简体中文](manual_chs.md)**

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
* [mpv.net specific commands](#mpvnet-specific-commands)
* [mpv.net specific options](#mpvnet-specific-options)
* [External Tools](#external-tools)
* [Scripting](#scripting)
* [Extensions](#extensions)
* [Color Theme](#color-theme)
* [Advanced Features](#advanced-features)
* [Hidden Features](#hidden-features)
* [Differences compared to mpv](#differences-compared-to-mpv)
* [Environment Variables](#environment-variables)
* [user-data](#user-data)
* [Context Menu Commands](#context-menu)


About
-----

mpv.net is a media player for Windows that has a modern GUI.

The player is based on the popular [mpv](https://mpv.io) media player.
mpv.net is designed to be mpv compatible, almost all mpv features are available,
this means the official [mpv manual](https://mpv.io/manual/master/) applies to mpv.net,
differences are documented in this manual under [Differences compared to mpv](#differences-compared-to-mpv).


Download
--------

1. [Stable and beta portable and setup via GitHub download](../../../releases)
2. Stable via command line with winget: `winget install mpv.net`
3. [Automated nightly portable builds](https://github.com/mpvnet-player/mpv.net/actions)

[Changelog](changelog.md)


Installation
------------

1. Windows 10 or higher.
2. [.NET Desktop Runtime 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

Internet streaming requires:

- Downloading [yt-dlp](https://github.com/yt-dlp/yt-dlp) and adding its folder
  to the [user environment variable PATH](https://www.google.com/search?q=user+environment+variable+PATH).
- In case of proxy server usage, [manual configuration](https://github.com/mpvnet-player/mpv.net/issues/401).

#### File Associations

File Associations can be registered using the context menu under 'Settings > Setup'.

After the file associations were registered, it might still be necessary to change the
default app in the Windows settings.

Another way to register file associations is using Windows File Explorer,
select a media file and select 'Open with > Choose another app' in the context menu.

[Open with++](#open-with) can be used to extend the File Explorer context menu
to get menu items for [Play with mpv.net](https://github.com/stax76/OpenWithPlusPlus#play-with-mpvnet) and
[Add to mpv.net playlist](https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist).

When multiple files are selected in File Explorer and enter is pressed then
the files are opened in mpv.net in random order, this works with maximum 15 files.

#### Path environment variable

In order to use mpv.net in a terminal for advanced use cases,
mpv.net must be added to the Path environment variable,
this can be achieved with the context menu (Settings/Setup).

Support
-------

Before making a support request, please try the newest [beta version](../../../releases) first.

Support can be requested here:

mpv.net bug reports, feature requests and advanced questions:

https://github.com/mpvnet-player/mpv.net/issues

Beginner mpv questions:

https://www.reddit.com/r/mpv

Advanced mpv questions:

https://github.com/mpv-player/mpv/issues


Settings
--------

mpv.net searches the config folder at:

1. Folder defined via MPVNET_HOME environment variable.
2. startup\portable_config (startup means the directory containing mpvnet.exe)
3. `%APPDATA%\mpv.net` (`C:\Users\Username\AppData\Roaming\mpv.net`)

mpv options are stored in the file mpv.conf,
mpv.net options are stored in the file mpvnet.conf,
mpv.net options are documented [here](#mpvnet-specific-options).


Input and context menu
----------------------

Global keyboard shortcuts are supported via `global-input.conf` file.

The config folder can be opened from the context menu: `Settings > Open Config Folder`

A input and config editor can be found in the context menu under 'Settings'.

The input test mode can be started via command line: --input-test

The input key list can be printed with --input-keylist

mpv input.conf defaults:  
https://github.com/mpv-player/mpv/blob/master/etc/input.conf

mpv input commands:  
https://mpv.io/manual/master/#list-of-input-commands

mpv input options:  
https://mpv.io/manual/master/#input

Before version v7 all bindings and the context menu definition
were defined in the input.conf file, which mpv.net created
in case it didn't exist. This had the disadvantage that mpv.net
lost control over all default bindings and context menu
defaults. This was unfortunate, v7 introduces a new bindings
and context menu design fixing it.

In v7 no input.conf file is created, the default bindings and
context menu is defined internally. input.conf only contains
what is different from the internally defined defaults,
so it works the same as it works in mpv.

For backward compatibility the old input.conf context menu
format with the menu definition using `#menu: ` is still
supported. The new design also allows for a menu customization,
in a sub section called `Custom`. In input.conf it can be
defined like so:

`Ctrl+a  show-text Test  #custom-menu: Test > Test`

Users that have their bindings and context menu customized
before v7 can easily migrate to the new design by deleting
bindings they don't use and remember the shortcut and remove
`#menu:` everywhere, it's important to remove `#menu:`
everywhere in order to enable the new mode/design.


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


mpv has a few non property based switches which are generally not supported in mpv.net.


Terminal
--------

When mpv.net is started from a terminal it will output status,
error and debug messages to the terminal and accept input keys from the terminal.


mpv.net specific commands
-------------------------

`script-message-to mpvnet <command> <arguments>`

mpv.net commands are used when mpv commands don't exist or lack a feature.

### add-to-path
Adds mpv.net to the Path environment variable.

### remove-from-path
Removes mpv.net from the Path environment variable.

### edit-conf-file [mpv.conf|input.conf]
Opens mpv.conf or input.conf in a text editor.

### load-audio
Shows a file browser dialog to open external audio files.

### load-sub
Shows a file browser dialog to open external subtitle files.

### move-window [left|top|right|bottom|center]
Moves the Window to the screen edge (Alt+Arrow) or center (Alt+BS).

### open-conf-folder
Opens the config folder with Windows File Explorer.

### open-files [\<flags\>]
**append**  
Appends files to the playlist.

Opens a file browser dialog in order to select files to be opened.
The file browser dialog supports multiselect to load multiple files
at once. Pressing CTRL appends the files to the playlist.

### open-optical-media
Shows a folder browser dialog to open a DVD or BD folder.
ISO images don't have to be mounted, but instead can be
opened directly with the open-files command.

### open-clipboard [\<flags\>]
Opens a single URL or filepath from the clipboard,
or multiple files in the file clipboard format.

**append**  
Appends files/URLs to the playlist.

### play-pause
Cycles the pause property. In case the playlist is empty,
the most recent file from the recent files list is loaded.

### reg-file-assoc \<audio|video|image\>
Registers the file associations.

### scale-window \<factor\>
Decreases or increases the Window size.

### shell-execute \<file|URL\>
Shell executes a single file or URL.

### show-about
Shows the about dialog.

### show-audio-devices
Shows available audio devices in a message box.

### show-commands
Shows available [mpv input commands](https://mpv.io/manual/master/#list-of-input-commands).

### show-properties
Shows available [properties](https://mpv.io/manual/master/#properties).

### show-keys
Shows available [input keys](https://mpv.io/manual/master/#options-input-keylist).

### show-protocols
Shows available [protocols](https://mpv.io/manual/master/#options-list-protocols).

### show-decoders
Shows available decoders.

### show-demuxers
Shows available demuxers.

### show-conf-editor
Shows the conf editor.

### show-input-editor
Shows the input editor.

### show-media-info [\<flags\>]
**msgbox**  
Shows media info in a messsage box.

**editor**  
Shows media info in a text editor.

**osd**
Shows media info on screen.

**full**  
Shows fully detailed media info.

**raw**  
Shows media info with raw property names.

### show-menu
Shows the context menu.

### show-playlist
Shows the playlist in a message box. For a playlist menu
the following user scripts exist:  

- https://github.com/stax76/mpv-scripts#command_palette
- https://github.com/stax76/mpv-scripts#search_menu
- https://github.com/tomasklaen/uosc
- https://github.com/jonniek/mpv-playlistmanager

### show-profiles
Shows available profiles with a message box.

### show-text \<text\> \<duration\> \<font-size\>
Shows a OSD message with given text, duration and font size.

### window-scale \<factor\>
Works similar as the [window-scale](https://mpv.io/manual/master/#command-interface-window-scale) mpv property.


mpv.net specific options
------------------------

mpv.net specific options can be found in the conf editor searching for 'mpv.net'.

The options are saved in the mpvnet.conf file.

#### --autofit-audio \<integer\>
Initial window height in percent for audio files. Default: 70

#### --autofit-image \<integer\>
Initial window height in percent for image files. Default: 80

#### --queue \<files\>

Adds files to the playlist, requires [--process-instance=single](#--process-instancevalue).
[Open with++](#open-with) can be used to add files to the playlist using File Explorer.

#### --command=\<input command\>

Sends a input command to a running mpv.net instance via command line, for instance
to create global keyboard shortcuts with AutoHotkey. Requires [process-instance=single](#--process-instancevalue).

### Audio

#### --remember-audio-device=\<yes|no\>

Save and restore the audio device chosen in the context menu. Default: yes

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

#### --minimum-aspect-ratio=\<float\>

Minimum aspect ratio of the window. Useful to force
a wider window and therefore a larger OSC.

#### --minimum-aspect-ratio-audio=\<float\>

Same as minimum-aspect-ratio but used for audio files.

#### --remember-window-position=\<yes|no\>

Save the window position on exit. Default: no

#### --start-threshold=\<milliseconds\>

Threshold in milliseconds to wait for libmpv returning the video
resolution before the window is shown, otherwise default dimensions
are used as defined by autofit and start-size. Default: 1500

### Playback

#### --auto-load-folder=\<yes|no\>

For single files automatically load the entire directory into the playlist.


### General

#### --menu-syntax=\<value\>

Used menu syntax for defining the context menu in input.conf.\nmpv.net by default uses `#menu:`, uosc uses `#!` by default.

#### --process-instance=\<value\>

Defines if more then one mpv.net process is allowed.

Whenever the CTRL key is pressed when files or URLs are opened,
the playlist is not cleared but the files or URLs are appended to the playlist.
This not only works on process startup but in all mpv.net features that open files and URLs.

Multi can alternatively be enabled by pressing the SHIFT key.

**multi**  
Create a new process everytime the shell starts mpv.net.

**single**  
Force a single process everytime the shell starts mpv.net. Default

**queue**  
Force a single process and add files to playlist.

#### --recent-count=\<int\>

Amount of recent files to be remembered. Default: 15

#### --media-info=\<yes|no\>

Usage of the media info library instead of mpv to access media information. Default: yes (mpv.net specific option)

#### --video-file-extensions=\<string\>

Video file extensions used to create file associations and used by the auto-load-folder feature.

#### --audio-file-extensions=\<string\>

Audio file extensions used to create file associations and used by the auto-load-folder feature.

#### --image-file-extensions=\<string\>

Image file extensions used to create file associations and used by the auto-load-folder feature.

#### --debug-mode=\<yes|no\>

Enable this only when a developer asks for it. Default: no


### UI

#### --language=\<value\>

User interface display language.
mpv.net must be restarted after a change.

Interested joining our translation team?:  
https://app.transifex.com/stax76/teams/

#### --dark-mode=\<value\>

Enables a dark theme.

**always**  
Default

**system**  
Available on Windows 10 or higher.

**never**

#### --dark-theme=\<string\>

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


### External Application Button

Videos can be streamed or downloaded easily with the Chrome extension
External Application Button, for download (recommended):

path: `wt`

args: `-- pwsh -NoLogo -Command "yt-dlp --ignore-errors --download-archive 'C:\External Application Button.txt' --output 'C:\YouTube\%(channel)s - %(title)s.%(ext)s' ('[HREF]' -replace '&list=.+','')"`


Scripting
---------

#### Lua

A very large collection of user scripts can be found in the GitHub repository
[awesome-mpv](https://github.com/stax76/awesome-mpv). 

Lua scripting is documented in the mpv.net wiki [here](https://github.com/mpvnet-player/mpv.net/wiki/Extending-mpv-and-mpv.net-via-Lua-scripting).

#### JavaScript

[mpv JavaScript documentation](https://mpv.io/manual/master/#javascript)


.NET Extensions
---------------

.NET Extensions are located in a subfolder _extensions_ in the config folder,
the filename must have the same name as the directory:

```Text
<config folder>\extensions\ExampleExtension\ExampleExtension.dll
```


Color Theme
-----------

mpv.net supports custom color themes, the definition of the built-in themes can be found at:

[theme.txt](../../../tree/main/src/Resources/theme.txt)


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

In fullscreen mode clicking the top right corner closes the player.


Differences compared to mpv
---------------------------

mpv.net is designed to work exactly like mpv, there are a few
differences and limitations:

The settings folder is named `mpv.net` instead of `mpv`:

`C:\Users\username\AppData\Roaming\mpv.net`

### Window Limitations

mpv.net implements an own main window which means only mpv window
features are supported that have an own implementation in mpv.net.

A window free mode is currently not supported, the main window is always
visible, even when mpv.net is started from the terminal and music is played.

For mpv.net it's currently not possible to find out where OSC menus are located,
but there are 3 features that require this information, therefore mpv.net
makes the assumption that near the window borders might be OSC menus. As a result
the following three features, work only when invoked from the center of the window:

1. Window dragging (moving the window with the mouse).
2. Showing the context menu.
3. Auto hiding the mouse cursor.

When the mouse is near a window border, these 3 features are not available.
The dead zone sizes are 10% left, top, right and 22% bottom.

The documentation of mpv's window features can be found here:

https://mpv.io/manual/master/#window


**mpv.net has currently implemented the following window properties:**

- [border](https://mpv.io/manual/master/#options-border)
- [fullscreen](https://mpv.io/manual/master/#options-fullscreen)
- [keepaspect-window](https://mpv.io/manual/master/#options-keepaspect-window)
- [ontop](https://mpv.io/manual/master/#options-ontop)
- [screen](https://mpv.io/manual/master/#options-screen)
- [snap-window](https://mpv.io/manual/master/#options-snap-window)
- [title-bar](https://mpv.io/manual/master/#options-title-bar)
- [title](https://mpv.io/manual/master/#options-title)
- [window-maximized](https://mpv.io/manual/master/#options-window-maximized)
- [window-minimized](https://mpv.io/manual/master/#options-window-minimized)
- [window-scale](https://mpv.io/manual/master/#options-window-scale)


**Partly implemented or modified:**

#### --autofit=\<int\>

\<int\> Initial window height in percent. Default: 60

#### --autofit-smaller=\<int\>

\<int\> Minimum window height in percent. Default: 10

#### --autofit-larger=\<int\>

\<int\> Maximum window height in percent. Default: 80

#### --geometry\<x:y\>

Initial window location in percent. Default: 50:50 (centered)

Requires Windows 11, on Windows 10 it works slightly incorrect due to invisible borders.

x=0 docks the window to the left side.  
x=100 docks the window to the right side.  

y=0 docks the window to the top side.  
y=100 docks the window to the bottom side.

#### --title-bar=\<yes|no\>

Shows the window title bar. Default: yes

**mpv.net specific window features:**

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

### Other Limitations

The mpv property [idle](https://mpv.io/manual/master/#options-idle) can be
used and mpv.net functions accordingly, but Lua scripts always see `idle=yes`
because mpv.net has to set it to function correctly, this is a difficult
to overcome libmpv limitation.


### mpv.net specific options

Options that are specific to mpv.net can be found by entering _mpv.net_
in the search field of the config editor, in the mpv.net manual they are
documented [here](#mpvnet-specific-options).

mpv.net specific options are saved in the file mpvnet.conf and are just
as mpv properties available on the command line.


Environment Variables
---------------------

### MPVNET_HOME

Directory where mpv.net looks for user settings.


user-data
---------

Script authors can access the following
[user-data](https://mpv.io/manual/master/#command-interface-user-data) properties:

```
user-data/frontend/name
user-data/frontend/version
user-data/frontend/process-path
```

Context Menu Commands
---------------------

### Open > Open Files

The Open Files menu entry is one way to open files in mpv.net, it supports multi selection.

Another way to open files is the command line which is used by
File Explorer for existing associations.

A third way is to drag and drop files on the main window.

Blu-ray and DVD ISO image files are supported.


### Open > Open URL or file path from clipboard

Opens files and URLs from the clipboard. Shift key appends to the playlist.
How to open URLs directly from the browser from sites like YouTube is described in the
[External Tools section](#external-tools).


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


### Audio > Next

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

`add volume 2`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)


### Volume > Down

Decreases the volume using the following command:

`add volume -2`

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

Shows the [mpv.net web site](https://github.com/mpvnet-player/mpv.net).


### Help > Show mpv.net manual

Shows the [mpv.net manual](https://github.com/mpvnet-player/mpv.net/blob/main/manual.md).


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
