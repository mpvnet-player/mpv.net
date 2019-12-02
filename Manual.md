# mpv.net manual

## Table of contents

* [About mpv.net](#about-mpvnet)
  + [Target Audience](#target-audience)
* [Requirements](#requirements)
* [Installation](#installation)
  + [File Associations](#file-associations)
* [External Tools](#external-tools)
  + [Play with mpv](#play-with-mpv)
  + [External Application Button](#external-application-button)
  + [Open with++](#open-with)
  + [MediaInfo.NET](#mediainfonet)  
* [Command Line Interface](#command-line-interface)
* [Terminal](#terminal)
* [Color Theme](#color-theme)
* [Hidden and secret features](#hidden-and-secret-features)
* [Context Menu](#context-menu)
  + [Open > Open Files](#open--open-files)
  + [Open > Open URL](#open--open-url)
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
  + [Tools > Show REPL](#tools--show-repl)
  + [Tools > OS Setup](#tools--os-setup)
  + [Help > Show mpv manual](#help--show-mpv-manual)
  + [Help > Show mpv.net manual](#help--show-mpvnet-manual)
  + [Help > Check for Updates](#help--check-for-updates)
  + [Help > About mpv.net](#help--about-mpvnet)
  + [Exit](#exit)
  + [Exit Watch Later](#exit-watch-later)

## About mpv.net

mpv.net is a modern desktop media player for Windows. mpv is similar to VLC not based on DirectShow like MPC, mpv.net is based on libmpv which in return is based on ffmpeg.

libmpv provides the majority of the features of the mpv media player, a fork of mplayer. mpv focuses on the usage of the command line interface, mpv.net retains the ability to be used from the command line and adds a modern GUI on top of it.

mpv.net is designed to be mpv compatible, almost all mpv features are available because they are all contained in libmpv, this means the official [mpv manual](https://mpv.io/manual/master/) applies to mpv.net.

### Target Audience

The target audience of mpv and mpv.net are programmers, nerds and software enthusiasts that need a desktop player that is more advanced than a normal desktop player. mpv.net is very similar to mpv so it's well suited to learn mpv.

## Requirements

mpv.net requires minimum .NET Framework 4.8 and Windows 7. For optimal results a modern graphics card is recommended.

## Installation

mpv.net is available as setup and as portable download in the 7zip and Zip archive format, to unpack the portable download 7zip can be used, it is available at www.7-zip.org.

If you are new to mpv.net the portable download is recommended, for regular mpv.net users the setup is typically more easy to use.

The x64 editions require more memory and have the advantage of being typically first and better optimized for x64.

x86 (32-bit) is not recommended but should still be well supported and work but it's not as well tested as x64.

Stable releases are compiled from the source and can be downloaded from the releases tab:

<https://github.com/stax76/mpv.net/releases>

Scoop can be used to install and update it:

```
scoop bucket add extras
scoop install mpv.net
```

Alternatively, Chocolatey can also be used:

`choco install mpvnet.install`

### File Associations

File Associations can be created using the setup or with the context menu under 'Tools > OS Setup'.

After the file associations were registered go to the Windows settings under 'Settings > Apps > Default apps' or shell execute `ms-settings:defaultapps` and choose mpv.net as default app for Video and optionally for Audio and Images.

It's also possible to change the default application using the 'Open with' feature of the context menu in File Explorer.

[Open with++](#open-with) can be used to extend the File Explorer context menu to get menu items for 'Play with mpv.net' and 'Add to mpv.net playlist'.

##  External Tools

### Play with mpv

In order to play videos from sites such as YouTube the Chrome Extension [Play with mpv](https://chrome.google.com/webstore/detail/play-with-mpv/hahklcmnfgffdlchjigehabfbiigleji) can be used.

Due to Chrome Extensions not being able to start a app, another app that communicates with the extension is required, this app can be downloaded [here](http://www.mediafire.com/file/lezj8lwqt5zf75v/play-with-mpvnet-server.7z/file). The extension works only when the app is running, to have the app always running a link can be created in the auto start folder located at:

`C:\Users\%username%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup`

This will start the app on system start and have it running in the background. When the file association registration of mpv.net was executed then the app should find the location of mpv.net, alternativly the mpv.net folder can be added to the Path environment variable.

### External Application Button

Alternatively Chrome/Firefox extension [External Application Button](https://github.com/stax76/mpv.net/issues/106) can be used.

### Open with++

Open with++ allows to customize the File Explorer context menu to add menu items 'Play with mpv.net' and 'Add to mpv.net playlist'.

https://github.com/stax76/OpenWithPlusPlus

https://github.com/stax76/OpenWithPlusPlus#mpvnet

https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist

### MediaInfo.NET

MediaInfo.NET is a media info GUI.

https://github.com/stax76/MediaInfo.NET

To start a external application mpv has the run input command (it does not use the shell).

If the path has spaces then it must be enclosed in quotes and then double backslashes must be used for escaping or alternatively forward slashes.

 `_ run D:\Software\MediaInfoNET.exe "${path}" #menu: Tools > Open file with MediaInfo.NET`

 `_ run "D:\\Soft ware\\MediaInfoNET.exe" "${path}" #menu: Tools > Open file with MediaInfo.NET`

 `_ run "D:/Soft ware/MediaInfoNET.exe" "${path}" #menu: Tools > Open file with MediaInfo.NET`

## Command Line Interface

mpvnet implements a command line interface to set mpv commands.

Supported are all mpv properties which are documented at:

<https://mpv.io/manual/master>

Example:

`mpvnet --mute=yes <file|URL>`

## Terminal

When mpv.net is started from a PowerShell terminal it will attach to the terminal and print status and debug messages.

Certain error and debug messages are only visible on the terminal, JavaScript and Lua scripts must be debugged with the terminal as there is no support for a debugger.

## Color Theme

mpv.net supports custom color themes, the definition of the built-in themes can be found at:

<https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/theme.txt>

Custom themes can be saved at:

`<conf folder>\theme.conf`

The theme.conf file may contain an unlimited amount of themes.

In the config editor under UI there are the settings dark-theme and light-theme to define the themes used in dark and in light mode.

## Hidden and secret features

Selecting multiple files in File Explorer and pressing enter will open the files in mpv.net. Explorer restricts this to maximum 15 files and the order will be random.

Whenever the control key is pressed when files or URLs are opened, the playlist is not cleared but the files or URLs are appended to the playlist. This works in all mpv.net features that open files or URLs.

Pressing the shift key while opening a single file will suppress loading all files in the folder.

## Context Menu

The most important part of the user interface in mpv.net is the context menu, the menu can be customized as it's defined in the same file where the key and mouse bindings are defined (input.conf).

### Open > Open Files

The Open Files menu entry is one way to open files in mpv.net, it supports multi selection.

Another way to open files is the command line, it is used by the File Explorer if file associations exist.

When mpv.net is started from a terminal such as PowerShell, mpv.net attaches to the terminal and outputs status and debug messages. 

A third way is to drag and drop files on the main window.

Whenever the control key is pressed when files or URLs are opened, the playlist is not cleared but the files or URLs are appended to the playlist. This works in all mpv.net features that open files or URLs.

Pressing the shift key while opening a single file will suppress loading all files in the folder.

### Open > Open URL

The Open URL menu entry can be used to open URLs for example from YouTube.

mpv.net monitors the Windows clipboard and ask if URLs should be played in case it finds a URL in the clipboard. This feature uses a keyword whitelist that can be configured in the config editor. 

When mpv.net is started from a terminal such as PowerShell, mpv.net attaches to the terminal and outputs status and debug messages. 

Whenever the control key is pressed when files or URLs are opened, the playlist is not cleared but the files or URLs are appended to the playlist. This works in all mpv.net features that open files or URLs.

### Open > Show media search

mpv.net supports system wide media searches using the Everything indexing service installed by the popular file search tool Everything (www.voidtools.com).

### Open > Load external audio files

Allows to load an external audio file. It's also possible to auto detect external audio files based on the file name, the option for this can be found in the settings under 'Settings > Show Config Editor > Audio > audio-file-auto'.

### Open > Load external subtitle files

Allows to load an external subtitle file. It's also possible to auto detect external subtitle files based on the file name, the option for this can be found in the settings under 'Settings > Show Config Editor > Subtitles > sub-auto'.

### Play/Pause

Play/Pause using the command:

`cycle pause`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down])

[pause property](https://mpv.io/manual/master/#options-pause)

### Stop

Stops the player and unloads the playlist using the command:

`stop`

[stop command](https://mpv.io/manual/master/#command-interface-stop)

### Toggle Fullscreen

Toggles fullscreen using the command:

`cycle fullscreen`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down])

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

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[chapter property](https://mpv.io/manual/master/#command-interface-chapter)

### Navigate > Previous Chapter

Navigates to the previous chapter using the command:

`add chapter -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

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

[seek command](https://mpv.io/manual/master/#command-interface-[relative|absolute|absolute-percent|relative-percent|exact|keyframes])

### Pan & Scan > Increase Size

Adds video zoom using the command:

`add video-zoom  0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)

### Pan & Scan > Decrease Size

Adds negative video zoom using the command:

`add video-zoom  -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)

### Pan & Scan > Move Left

`add video-pan-x -0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)

### Pan & Scan > Move Right

`add video-pan-x 0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)

### Pan & Scan > Move Up

`add video-pan-y -0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)

### Pan & Scan > Move Down

`add video-pan-y 0.01`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)

### Pan & Scan > Decrease Height

`add panscan -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[panscan property](https://mpv.io/manual/master/#options-panscan)

### Pan & Scan > Increase Height

`add panscan  0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[panscan property](https://mpv.io/manual/master/#options-panscan)

### Pan & Scan > Reset

Resets Pan & Scan, multiple commands in the same line are separated with semicolon.

`set video-zoom 0; set video-pan-x 0; set video-pan-y 0`

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)

### Video > Decrease Contrast

Decreases contrast with the following command:

`add contrast -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[contrast property](https://mpv.io/manual/master/#options-contrast)

### Video > Increase Contrast

Increases contrast with the following command:

`add contrast 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[contrast property](https://mpv.io/manual/master/#options-contrast)

### Video > Decrease Brightness

Decreases brightness using the following command:

`add brightness -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[brightness property](https://mpv.io/manual/master/#options-brightness)

### Video > Increase Brightness

Increases brightness using the following command:

`add brightness 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[brightness property](https://mpv.io/manual/master/#options-brightness)

### Video > Decrease Gamma

Decreases gamma using the following command:

`add gamma -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[gamma property](https://mpv.io/manual/master/#options-gamma)

### Video > Increase Gamma

Increases gamma using the following command:

`add gamma 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[gamma property](https://mpv.io/manual/master/#options-gamma)

### Video > Decrease Saturation

Decreases saturation using the following command:

`add saturation -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[saturation property](https://mpv.io/manual/master/#options-saturation)

### Video > Increase Saturation

Increases saturation using the following command:

`add saturation 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[saturation property](https://mpv.io/manual/master/#options-saturation)

### Video > Take Screenshot

`async screenshot`

[async command prefix](https://mpv.io/manual/master/#command-interface-async)

[screenshot command](https://mpv.io/manual/master/#command-interface-[subtitles|video|window|single|each-frame])

### Video > Toggle Deinterlace

Cycles the deinterlace property using the following command:

`cycle deinterlace`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down])

[deinterlace property](https://mpv.io/manual/master/#options-deinterlace)

### Video > Cycle Aspect Ratio

Cycles the aspect ratio using the following command:

`cycle-values video-aspect "16:9" "4:3" "2.35:1" "-1"`

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[video-aspect property](https://mpv.io/manual/master/#options-video-aspect)

### Audio > Cycle/Next

This uses a mpv.net command that shows better info then the mpv preset
and also has the advantage of not showing no audio.

### Audio > Delay +0.1

Adds a audio delay using the following command:

`add audio-delay 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[audio-delay property](https://mpv.io/manual/master/#options-audio-delay)

### Audio > Delay -0.1

Adds a negative audio delay using the following command:

`add audio-delay -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[audio-delay property](https://mpv.io/manual/master/#options-audio-delay)

### Subtitle > Cycle/Next

Shows the next subtitle track using the following command:

`cycle sub`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down])

[sub/sid property](https://mpv.io/manual/master/#options-sid)

### Subtitle > Toggle Visibility

Cycles the subtitle visibility using the following command:

`cycle sub-visibility`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down])

[sub-visibility property](https://mpv.io/manual/master/#options-no-sub-visibility)

### Subtitle > Delay -0.1

Adds a negative subtitle delay using the following command:

`add sub-delay -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[sub-delay property](https://mpv.io/manual/master/#options-sub-delay)

### Subtitle > Delay 0.1

Adds a positive subtitle delay using the following command:

`add sub-delay 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[sub-delay property](https://mpv.io/manual/master/#options-sub-delay)

### Subtitle > Move Up

Moves the subtitle up using the following command:

`add sub-pos -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[sub-pos property](https://mpv.io/manual/master/#options-sub-pos)

### Subtitle > Move Down

Moves the subtitle down using the following command:

`add sub-pos 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[sub-pos property](https://mpv.io/manual/master/#options-sub-pos)

### Subtitle > Decrease Subtitle Font Size

Decreases the subtitle font size using the following command:

`add sub-scale -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[sub-scale property](https://mpv.io/manual/master/#options-sub-scale)

### Subtitle > Increase Subtitle Font Size

Increases the subtitle font size using the following command:

`add sub-scale 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[sub-scale property](https://mpv.io/manual/master/#options-sub-scale)

### Volume > Up

Increases the volume using the following command:

`add volume 10`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)

### Volume > Down

Decreases the volume using the following command:

`add volume -10`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)

### Volume > Mute

Cycles the mute property using the following command:

`cycle mute`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down])

[mute property](https://mpv.io/manual/master/#options-mute)

### Speed > -10%

Decreases the speed by 10% using the following command:

`multiply speed 1/1.1`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cproperty%3E-%3Cfactor%3E)

[speed property](https://mpv.io/manual/master/#options-speed)

### Speed > 10%

Increases the speed by 10% using the following command:

`multiply speed 1.1`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cproperty%3E-%3Cfactor%3E)

[speed property](https://mpv.io/manual/master/#options-speed)

### Speed > Half

Halfs the speed using the following command:

`multiply speed 0.5`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cproperty%3E-%3Cfactor%3E)

[speed property](https://mpv.io/manual/master/#options-speed)

### Speed > Double

Doubles the speed using the following command:

`multiply speed 2`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cproperty%3E-%3Cfactor%3E)

[speed property](https://mpv.io/manual/master/#options-speed)

### Speed > Reset

Resets the speed using the following command:

`set speed 1`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cproperty%3E-%22%3Cvalue%3E%22)

[speed property](https://mpv.io/manual/master/#options-speed)

### Extensions > Rating > 0stars

A plugin the writes the rating to the filename.

### View > On Top > Enable

Forces the player to stay on top of other windows using the following command:

`set ontop yes`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cproperty%3E-%22%3Cvalue%3E%22)

[ontop property](https://mpv.io/manual/master/#options-ontop)

### View > On Top > Disable

Disables the player to stay on top of other windows using the following command:

`set ontop no`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cproperty%3E-%22%3Cvalue%3E%22)

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

### Tools > Show REPL

Shows a on screen REPL.

https://github.com/rossy/mpv-repl

### Tools > OS Setup

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
