###

- opening a URL manually no longer uses a input box but uses the clipboard directly

### 4.4

- clipboard-monitoring was replaced by url-whitelist:
  Keyword whitelist to monitor the clipboard for URLs to play.
  Default: tube vimeo ard zdf 
- some settings like colors didn't work because enclosing quotes were missing
- when single process queue is used the folder is no longer loaded
- the playlist is never cleared whenever the control key is down but
  files and URLs are appended instead
- powershell script hosting bugs were fixed and a new powershell example script
  was added to the [scripting wiki page](https://github.com/stax76/mpv.net/wiki/Scripting#powershell)
- the menu entry for the command palette was renamed to 'Show All Commands' and
  the default key binding was changed to F1 which is also the default in VS Code
- the default key binding of the Everything media search was changed to F3
- support for the mpv property 'border' was added to the config editor
  to show/hide the window decoration (titlebar, border). A toggle menu item and
  key binding (b) was added as well ([Default Binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L135))

### 4.3.1

- there was a old bug setting the screen property

### 4.3

- there was new bug in file association feature

### 4.2

- the help and layout in the config editor was improved
- clipboard monitoring for URLs can be disabled in the settings
- the context menu has a new feature: Open > Add files to playlist,
  it appends files to the playlist [(Default binding)](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L33)
- a setting was added to force using a single mpv.net process instance

### 4.1

- drag & drop support for subtitles was added
- libmpv was updated
- command line support for stdin and URLs was added
- there was a crash happening when the player is
  minimized in the taskbar

### 4.0

- on the start screen the mpv.NET icon is shown instead of the mpv icon,
  feedback and contributions regarding the icon are welcome! The paint.net
  pdn and png source is located [here](https://github.com/stax76/mpv.net/tree/master/img)
- everytime only one file is opened the complete folder is loaded in the playlist
- the info command (i key) shows the audio format
- new options osd-font-size, sub-font, sub-font-size
- new color options with dedicated GUI support: sub-color, sub-border-color, sub-back-color
- the config editor no longer shows the command line switches
- the github start page was greatly improved 
- the setup.ps1 PowerShell script was greatly improved in regard of error handling and readability
- a [manual to mpv.net](Manual.md) was created

### 3.7

- new icon design, probably better then before but still too simple
- the radio buttons in the config editor have now a Windows 10 like design,
  they are larger and use the Windows theme color

### 3.6.1

- there was a bug causing an exception if both the input editor and config editor
  is opened, as soon as one is opened, the other can't be opened

### 3.6

- playing files from rar archives caused an exception
- there was a bug that caused underscores beeing removed from input like MBTN_LEFT_DBL
- the search clear button in the input editor had a render issue in dark mode
- new search feature added to search and play media files, requires
  [Everything](https://www.voidtools.com) to be installed. [Default Binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L29)

### 3.5

- when the main windows gets activated and the clipboard content starts with http
  mpv.net will ask to play the URL, previously this was restricted to YouTube URLs
- Python script errors show line and column whenever it is supported by IronPython
- if conf files exist in the startup directory mpv.net will use the startup
  directory as config directory instead of creating default conf files in appdata
- renamed commands are handled now by migration code instead of being broken

### 3.4

- new feature added to manage file associations from within the app. It can be found in the menu at: Tools > Manage... [Default Binding](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L149)
- new zip download option added
- new x86 download option added