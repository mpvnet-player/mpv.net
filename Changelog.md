
###

- fix: update routine did only work when mpv.net was located in 'Program Files'

### 5.4.4.0

- new: forecolors in the dark theme are slightly darker now
- new: readme/website and manual were improved
- new: source code includes now the release script to build the archives and setup files
- new: the history feature now uses the full path
- new: install via Scoop and Chocolatey added to readme/website (Restia666Ashdoll)
- new: update check, it must be enabled first in the conf editor under General
- new: update feature, requires PowerShell 5 and curl,
       an up to date Windows 10 system has both included.
       Main menu (input.conf) must be reset or updated manually ([defaults](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt))

- update: libmpv shinchiro 2019-11-10 

- fix: often the OSC was shown when fullscreen was entered
       or on app startup, this is now suppressed
- fix: the file association routine uses no longer 'Play with mpv.net' for the
       default open verb caption because it doesn't support multi selection,
       it shows now only Open, the manual explains how to get multi selection
       in File Explorer, read about it [here](https://github.com/stax76/mpv.net/blob/master/Manual.md#open-with)
- fix: x86 builds had an older version included because
       of a misconfiguration in the solution file

### 5.4.3.0

- new: the color themes can now be customized ([manual](https://github.com/stax76/mpv.net/blob/master/Manual.md#color-theme))
- new: three new sections were added to the [manual](https://github.com/stax76/mpv.net/blob/master/Manual.md):
       1. [Color Theme](https://github.com/stax76/mpv.net/blob/master/Manual.md#color-theme)
       2. [Hidden and secret features](https://github.com/stax76/mpv.net/blob/master/Manual.md#hidden-and-secret-features)
       3. [External Tools](https://github.com/stax76/mpv.net/blob/master/Manual.md#external-tools)

- fix: window restore from maximized and from minimized was broken
- fix: it's possible to multi select files in File Explorer and press
       enter to open the files, this did however only work when the
	   auto load folder feature was disabled or the shift key was
	   pressed (blocks auto load folder). Now it should also work
	   without shift key and with auto load folder being enabled

- update: libmpv shinchiro 2019-10-27
- update: youtube-dl 2019-10-31

### 5.4.2.1 Beta

- pressing shift key suppresses auto-load-folder
- switch --queue added, this will not clear the playlist but append
  and it will suppress auto-load-folder. To get a 'Add to mpv.net playlist'
  context menu item in explorer with multi selection support use my
  [Open with++](https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist) shell extension, as far as I know multi selection
  can not be done using the Registry but only via shell extension
- window-size mpv property support added ([default bindings](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/inputConf.txt#L137))
- the config editor keeps profiles and comments in mpv.conf intact!
- the options in the config editor are better organized

### 5.4.2

- new: the [scripting wiki page](https://github.com/stax76/mpv.net/wiki/Scripting#powershell) was improved
- new: Toggle Shuffle has been added to the menu defaults
- new: for URLs the media title is shown in the title bar and in the info command
       instead of displaying the URL, mpv.conf defaults were changed to use
       [protocol.https] osd-playing-msg = '${media-title}'

- fix: on the very first start volume was set to 0 and mute was set to yes
- fix: there was a sound when closed from taskbar
- fix: the log feature was not working
- fix: clipboard monitoring removed because it was causing to many issues
- fix: restore resulted in collapsed window when maximized = yes was used

- update: libmpv shinchiro 2019-10-06
- update: youtube-dl 2019-10-01

### 5.4.1.1

- a issue with the taskbar-progress feature was fixed
- improved PowerShell scrip host

### 5.4.1

- fix ArgumentOutOfRangeException

### 5.4

- added taskbar-progress implementation
- added new setting to start with maximized window
- long file paths work now even if not enabled by the OS
- fixed history being written even when history file wasn't created prior
- libmpv updated to shinchiro 2019-08-31

### 5.3

- added new feature: Open > Open DVD/Blu-ray Drive/Folder...
- the default of remember-volume has been set to yes
- scale, cscale, dscale defaults have been set to spline36,
  profile=gpu-hq is not used in the defaults because it starts very slow
- new menu items have been added to navigate to the first and
  last playlist position, key bindings: Home, End
- new config setting recent-count added, amount of menu items
  shown under 'Open > Recent'
- in the config dialog the description for keep-open was corrected
  because unlike mpv, mpv.net will never terminate automatically
- there was a rare occasion where the mpv.net logo wasn't shown
- fix excessive memory usage using `osd-scale-by-window = no`
- mpv setting osd-scale-by-window added to config dialog

### 5.2.1

- fixed race condition causing various features to fail

### 5.2

- bug fix for single-instance not working with unicode filenames
- bug fix for logo not shown on start
- osd-visibility.js script was removed because the OSC uses too much memory
- youtube-dl was updated
- libmpv was updated to shinchiro 2019-08-04
- in case mpv.net was started from a terminal it sets now the mpv property input-terminal to yes,
  this means mpv.net will now receive and handle input keys from the terminal
- certain command line properties didn't work (input-terminal, terminal, input-file,
  config, config-dir, input-conf, load-scripts, script, scripts, player-operation-mode)
- the about dialog shows now the mpv version and build date
- the dialog that asks for a config folder has now a cancel option

### 5.1

- 'Tools > Execute mpv command' was replaced with [mpv-repl](https://github.com/rossy/mpv-repl)
- many [wiki pages](https://github.com/stax76/mpv.net/wiki) were improved
- the logo/icon had a very small cosmetic change
- the help in the context menu was improved,
  for quick access consider the command palette (F1 key)
- config options specific to mpv.net are now available from the command line
- the input editor no longer has known limitations, 'alt gr' and ctrl+alt are working now
- the help in the input editor was simplified and the filter logic was improved
- fixed issue in file associations causing mpv.net not to appear in OS default apps
- 'Tools > Manage File Associations' was replaced by 'Tools > OS Setup',
  it has now a feature to add and remove mpv.net to and from the Path
  environment variable and the OS default apps settings can be opened (Win 10 only)
- startup folder and config folder being identical was causing a critical issue
  as mpv.net was loading extensions twice and scripts four times, now identical
  folders are no longer permitted
- error messages are shown when unknown scripts and extensions are found in the startup folder
  because user scripts and extensions are supposed to be located in the config folder instead
- changing from maximized to fullscreen did not work
- the search field in the config editor was not always remembered
- new setting remember-volume added, saves volume and mute on exit
  and restores it on start.
- it's now enforced that mpv properties on the command line and in
  the mpv.conf config file are lowercase, if not a error is shown
- gpu-api vulkan was not working if media files were opened via
  command line (that included Explorer)
- new setting minimum-aspect-ratio added, minimum aspect ratio for the window,
  this was previously hard coded to 1.3
- new setting auto-load-folder added, for single files automatically load
  the entire directory into the playlist, previously this was forced,
  now it can be disabled
- new setting themed-menu added, follow theme color in context menu,
  default: no. UI related settings have now a separate UI tab in the config editor

### 5.0

- [changed icon design](https://github.com/stax76/mpv.net/blob/master/img/mpvnet.png)
- libmpv was updated to shinchiro 2019-07-14
- new or improved config editor settings: screenshot-directory,
  screenshot-format, screenshot-tag-colorspace, screenshot-high-bit-depth,
  screenshot-jpeg-source-chroma, screenshot-template, screenshot-jpeg-quality,
  screenshot-png-compression, screenshot-png-filter
- mpv.conf preview feature added to config editor, it previews the mpv.conf content
- in the entire project the term addon was replaced with the term extension,
  unfortunately this will break user extensions. The reason for this drastic
  change is that there exist too many different terms, addons, addins,
  extensions, modules, packages etc.. mpv.net follows Google Chrome as the worlds
  most popular extendable app, Chrome uses the term Extension.
- a thread synchronization bug was fixed, the shutdown thread was aborted
  if it was running more than 3 seconds, this caused the rating extension
  to fail if it was waiting for a drive to wake up
- a new JavaScript was included to show the playlist with a smaller font size,
  the script is located at startup/scripts
- terminal support added using mpvnet.com !
- script engine performance and error handling was improved
- the [scripting wiki page](https://github.com/stax76/mpv.net/wiki/Scripting) was improved
- the C# scripting host extension was converted from VB to C# because it's not
  only used for hosting but I also use it now to code and debug script code
- there was a copy paste bug in the file association feature resulting in keys
  from mpv being overwritten instead of using mpv.net keys. Thanks to floppyD!
- there was a exception fixed that happened opening rar files
- instead of using the OS theme color there are now default colors
  for dark-color and light-color

### 4.7.7

- on Win 7 the theme color was hardcoded to DarkSlateGrey because
  WPF was returning a bad color on Win 7, this was fixed by reading
  the theme color from the Registry on Win 7. If the theme color
  is identical to the background color it's corrected
- dark-color setting was added to overwrite the OS theme color used in dark mode,
  find the setting on the General tab
- light-color setting was added to overwrite the OS theme color used in non dark mode,
  find the setting on the General tab
- various changes regarding input handling, multi media keys and
  mouse forward/backward were successfully tested
- it's now possible to use a custom folder as config folder,
  A TaskDialog shows five options: 1. appdata mpv.net, 2. appdata mpv (shared with mpv),
  3. portable_config, 4. startup, 5. custom
- slightly increased startup performance, start-threshold setting added.
  Threshold in milliseconds to wait for libmpv returning the video resolution
  before the window is shown, otherwise default dimensions are used as defined
  by autofit and start-size. Default: 1500
- autofit-smaller setting added. Minimum window height in percent. Default: 40%
- autofit-larger setting added. Maximum window height in percent. Default: 75%

### 4.7.3

- fix cursor showing load activity on startup
- added file size and type to info command using audio files
- added image format support to the info command, to file
  association management, to folder loading, to Everything
- readme/github have a updated features and architecture section

### 4.7.1

- few layout problems were fixed, autosize for instance did not work

### 4.7

- remember-height was replaced with start-size, when start-size is set
  to video the main video starts directly with the native video size,
  before it was starting with the autofit size first and was only
  afterwards resized to the native video size
- on exit the window location can be saved with remember-position
- in the learn window of the input editor underscores were stripped
  because they have a special meaning in WPF labels 
- fix for keys/input not working for MBTN_LEFT_DBL, MBTN_BACK, MBTN_FORWARD
- in the learn window of the input editor support was added for
  mouse left, mouse left double, mouse mid, mouse forward, mouse back
- libmpv was updated to shinchiro 2019-07-07
- when border is none it wasn't possible to minimize the window from
  the task bar because this is the WinForms default behavier. This
  was fixed by calling Spy++ to the rescue and adding WS_MINIMIZEBOX
  in CreateParams

### 4.6

- fix for middle mouse button not working
- fix of logo overlay using a huge amount of memory (thx for the [ghacks article](https://www.ghacks.net/2019/07/05/a-look-at-mpv-net-a-mpv-frontend-with-everything-integration/))
- fix config dialog showing a message about app restart without reason
- when multiple files are selected in File Explorer and enter is
  pressed, the files are opened as selected, the order is random though
  because Explorer starts multiple mpv.net processes concurrently
- libmpv was updated to shinchiro 2019-06-30
- the [mpv.conf defaults](https://github.com/stax76/mpv.net/blob/master/mpv.net/Resources/mpvConf.txt) were changed to show a larger OSC
- in case a file is opened that has a aspect ratio smaller then 1.2 then
  the window size will use a aspect ratio of 1.8
- new JavaScript script osc-visibility.js included in the distribution
  under startup\scripts. It sets the OSC to be always on for audio files
  and auto for non audio files

### 4.5

- opening a URL manually no longer uses a input box but uses the clipboard directly
- the manifest was missing the company attribute which caused
  mpv.net not appearing in the 'Open with' menu of the File Explorer,
  thanks to 44vince44 for pointing this out!!!
- new Python and C# script examples were added to the wiki and the scripting and
  extension documentation was improved
- invalid command line arguments were ignored, now an error message is shown
- a description on how to start mpv.net from Google Chrome was added to the
  manual, it's useful to play videos from sites like YouTube, find the
  description [here](https://github.com/stax76/mpv.net/blob/master/Manual.md#chrome-extension)
- new config setting remember-height added to remember the window height,
  otherwise the video's native resolution is used
- support for protocols other then http added

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