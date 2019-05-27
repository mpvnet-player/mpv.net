# mpv.net manual **(under construction)**

## About mpv.net

mpv.net is a media player for Windows. Similar like VLC mpv.net is not based on DirectShow like MPC, mpv.net is based on libmpv which in return is based on ffmpeg.

libmpv provides the majority of the features of the mpv media player, a fork of mplayer. mpv focuses on the usage of the command line interface, mpv.net retains the ability to be used from the command line and adds a simple and easy to use GUI on top of it.

mpv.net is meant to be a small single person project.

### Target Audience

The target audience of mpv.net are Windows programmers and users that need something more advanced than common media players.

Furthermore mpv.net is well suited for Windows users who are interested to learn about the Linux operating system and portable apps, even though mpv.net self is not portable.

## Requirements

mpv.net is based on the .NET Framework and requires Windows 7 as minimum version of the Windows operating system. As of the writing of this manual the minimum required .NET Framework version is 4.7.2.

For optimal results a modern graphics card is recommended.

## Installation

mpv.net is available as setup and as portable download in the 7zip and zip archive format, to unpack the portable download 7zip can be used, it is available at www.7-zip.org.

If you are new to mpv.net the portable download is recommended, for regular mpv.net users the setup is typically more easy to use.

The x64 editions require more memory and have the advantage of decoders being typically first and better optimized for x64.

x86 should though still be well supported and work.

### File Associations

File Associations can be created using the setup or from within the apps context menu under 'Tools > Manage File Associations'.

Windows 10 prevents apps to register as the default app, to define the default video or audio player app in Windows 10 go to the Windows settings under 'Settings > Apps > Default apps' or shell execute 'ms-settings:defaultapps'.

It's also possible to change the default application using the Open With feature of the context menu in Windows File Explorer.

## Context Menu

The most important part of the user interface in mpv.net is the context menu, the menu can be customized as it is defined in the same file where the key bindings are defined.

### Open > Open Files

The Open Files menu entry is one way to open files in mpv.net, it supports multi selection.

Another way to open files is the command line, it is used by the Windows File Explorer if file associations exist.

A third way is to drag and drop files on the main window.

### Open > Open URL

The Open URL menu entry can be used to open URLs for example from YouTube.

mpv.net monitors the Windows clipboard and ask if URLs should be played in case it finds a URL in the clipboard.

### Open > Show media search

mpv.net supports system wide media searches using the Everything indexing service installed by the popular file search tool Everything (www.voidtools.com).

### Open > Load external audio files

Allows to load an external audio file. It's also possible to auto detect external audio files based on the file name, the option for this can be found in the settings under 'Settings > Show Config Editor > Audio > audio-file-auto'.

### Open > Load external subtitle files

Allows to load an external subtitle file. It's also possible to auto detect external subtitle files based on the file name, the option for this can be found in the settings under 'Settings > Show Config Editor > Subtitles > sub-auto'.

### Play/Pause

Play/Pause using the command:

`cycle pause`

https://mpv.io/manual/master/#command-interface-cycle-%3Cproperty%3E-[up|down]

https://mpv.io/manual/master/#options-pause

### Stop

Stops the player and unloads the playlist using the command:

`stop`

https://mpv.io/manual/master/#command-interface-stop

### Toggle Fullscreen

Toggles fullscreen using the command:

`cycle fullscreen`

https://mpv.io/manual/master/#options-fs

### Navigate > Previous File

Navigates to the previous file in the playlist using the command:

`playlist-prev`

https://mpv.io/manual/master/#command-interface-playlist-prev

### Navigate > Next File

Navigates to the next file in the playlist using the command:

`playlist-next`

https://mpv.io/manual/master/#command-interface-playlist-next

### Navigate > Next Chapter

Navigates to the next chapter using the command:

`add chapter 1`

https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E]

https://mpv.io/manual/master/#command-interface-chapter

### Navigate > Previous Chapter

Navigates to the previous chapter using the command:

`add chapter -1`

https://mpv.io/manual/master/#command-interface-add-%3Cproperty%3E-[%3Cvalue%3E]

https://mpv.io/manual/master/#command-interface-chapter