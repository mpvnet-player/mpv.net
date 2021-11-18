
mpv.net手册
==============

同步源提交_[20211114](https://github.com/stax76/mpv.net/commit/243b45326ef8defa038edacd01fafee13f6a009a#diff-bf7b5e59783955f479505de4969f792255eca0a69945ccfe3ec5dda409495bbe)

**[ENGLISH](Manual.md)** | **简体中文**

目录
-----------------

* [关于](#关于)
* [下载](#下载)
* [安装](#安装)
* [支持](#支持)
* [设置](#设置)
* [快捷键输入绑定](#快捷键输入绑定)
* [命令行界面](#命令行界面)
* [终端](#终端)
* [mpv.net的专属选项](#mpvnet的专属选项)
* [外部工具](#外部工具)
* [脚本](#脚本)
* [扩展](#扩展)
* [配色主题](#配色主题)
* [高级功能](#高级功能)
* [隐藏功能](#隐藏功能)
* [与mpv的差异](#与mpv的差异)
* [技术概览](#技术概览)
* [上下文菜单](#上下文菜单)


关于
-----

mpv.net是基于受欢迎的mpv的Windows现代媒体播放器。

mpv.net出于兼容mpv而设计，几乎所有mpv的功能都可用，因为它们都包含在libmpv中，
这意味着[mpv官方手册](https://mpv.io/manual/master/) 也适用于mpv.net。

mpv专注命令行与终端的使用，而mpv.net保留了这些并加入了现代化的图形界面。


下载
--------

[更新日志](Changelog.md)


### 稳定版

[发布页面](../../../releases)


### 测试版

[OneDrive](https://1drv.ms/u/s!ArwKS_ZUR01g1ldoLA90tX9DzKTj?e=xITXbC)

[DropBox](https://www.dropbox.com/sh/t54p9igdwvllbpl/AADKyWpaFnIhdyosxyP5d3_xa?dl=0)


安装
------------

mpv.net需要.NET Framework 4.8运行库和高于win7版本的系统以及一张不太旧的显卡。

安装版和便携版可供下载。

安装新版本之前应完全卸载旧版本，使用旧版本覆盖安装新版本通常不是推荐的做法，
安装工具不强制执行这项操作，因为它难以实现。

对于网络串流，必须手动下载安装youtube-dl，它必须位于环境变量 PATH 或启动目录中。

mpv.net不限制系统平台，win32的用户必须替换目录中的3个工具：

- mpv-1.dll
- MediaInfo.dll
- mpvnet.com


#### 文件关联

可以使用上下文菜单创建文件关联。 'Tools > Setup'

注册完文件关联后，进入 "Windows设置 > 应用 > 默认应用"，或者从powershell
执行 `ms-settings:defaultapps` ，然后选择mpv. net作为视频/音频/图像的默认程序。

可以使用资源管理器的上下文菜单的'Open with'功能更改默认应用程序。

[Open with++](#open-with) 可用来扩展资源管理器的上下文菜单
[Play with mpv.net](https://github.com/stax76/OpenWithPlusPlus#play-with-mpvnet) 和 
[Add to mpv.net playlist](https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist).
可用来获取 'Play with mpv.net' 和 'Add to mpv.net playlist' 的菜单子项

当在资源管理器中选择多个文件并按 enter 键时，文件会在mpv.net随机排序打开，最多限制15个文件。


支持
-------

在提出支持请求之前，先尝试最新的[测试版](#测试版)。

程序错误和功能请求可以在github的[问题追踪](../../../issues)上提出，
任何与mpv. net相关的东西都可以使用，欢迎提交使用上出现的问题。

或者浏览VideoHelp论坛的讨论帖。 
[support thread](https://forum.videohelp.com/threads/392514-mpv-net-a-extendable-media-player-for-windows)


设置
--------

mpv.net在以下路径寻找设置文件夹：

1. <程序启动目录>\portable_config
2. %APPDATA%\mpv.net (`C:\Users\%USERNAME%\AppData\Roaming\mpv.net`)

mpv的选项保存在mpv.conf文件中，
mpv.net的专属选项保存在 mpvnet.conf 文件中，
参数解释请参阅 [此处](#mpvnet的专属选项)


快捷键输入绑定
----------------------

键鼠的快捷键和上下文菜单的操作保存在 input.conf 文件中，
如果设置目录中不存在，将使用以下文件生成默认的初始设置：

[input.conf defaults](../../../tree/master/src/Resources/input.conf.txt)

通过 global-input.conf 文件支持全局热键。

配置文件夹可以从上下文菜单中打开： `Settings > Open Config Folder`


命令行界面
----------------------

**mpvnet** [options] [file|URL|PLAYLIST|-]  
**mpvnet** [options] files


mpv的参数与mpv.net共通，例如：


启用窗口装饰：

`--border` or `--border=yes`


禁用窗口装饰：

`--no-boder` or `--border=no`


所有支持的mpv属性，参阅此处：

https://mpv.io/manual/master/#properties


mpv.net可以使用以下功能列出所有属性：

'Context Menu > View > Show Properties'


mpv.net通常不支持非属性的运行时状态切换！


终端
--------

当mpv. net从终端启动时，它将输出状态、错误和调试消息，并接受终端的输入。

在菜单中的 'Tools > Setup' 可以将mpv.net添加到环境变量PATH。

终端的常见用处是脚本调试。


mpv.net的专属选项
------------------------

这些专属选项可以在 conf editor 中使用关键词 "mpv.net" 检索。

这些专属选项被修改后被保存在 mpvnet.conf 文件中。

#### --queue \<files\>

添加文件到播放列表，需要设置 [--process-instance=single](#--process-instancevalue) 。
也可以在资源管理器中使用 [Open with++](#open-with) 添加文件。

#### --command=\<input command\>

通过命令行向正在运行的mpv.net实例发送输入命令，例如使用 AutoHotkey 创建全局热键，
必须设置 [process-instance=single](#--process-instancevalue) 。

### Audio

#### --remember-volume=\<yes|no\>

在程序退出时保存音量并静音，在启动时恢复之前的音量。默认：yes


### Screen

#### --start-size=\<value\>

设置为记住窗口大小。

**width-session**  
记住当前的宽度。

**width-always**  
始终记住宽度。

**height-session**  
记住当前的高度。（默认值）

**height-always**  
始终记住高度。

**video**  
窗口大小设置为视频分辨率。

**session**
记住当前进程的大小。

**always**  
始终记住大小。


#### --start-threshold=\<milliseconds\>

在显示窗口之前等待libmpv返回视频分辨率的阈值（毫秒），
否则将使用由 --autofit 和 --start-size 定义的初始大小。默认：1500


#### --minimum-aspect-ratio=\<float\>

最小宽高比，如果窗口宽高比小于定义的值，那么将窗口宽高比设置为16/9。
这避免了音乐封面的方形窗口。默认：1.2


#### --remember-window-position=\<yes|no\>

在退出时保存窗口的位置。默认：no


### Playback

#### --auto-load-folder=\<yes|no\>

打开单个文件时，自动将整个目录加载到播放列表中。可以通过 shift 键临时禁用。默认：yes


### General

#### --process-instance=\<value\>

定义是否允许多个 mpv.net 进程。

提示：当打开文件或 url 时，只要按下CTRL键，就不会清除当前的播放列表，
而只将文件或 url 追加到列表中。这不仅适用于进程启动，也适用于所有打开文件和 url 的功能。

**multi**  
每次从 shell 启动 mpv.net 时创建一个新进程。

**single**  
每次从 shell 启动 mpv.net 只允许唯一一个进程。（默认值）

**queue**  
和 single 类似但会把文件追加到已打开的mpv.net的播放列表中。


#### --recent-count=\<int\>

最近文件的记录数量。默认：15


#### --video-file-extensions=\<string\>

用于创建文件关联的视频文件扩展名，由自动加载文件夹的功能使用。


#### --audio-file-extensions=\<string\>

用于创建文件关联的音频文件扩展名，由自动加载文件夹的功能使用。


#### --image-file-extensions=\<string\>

用于创建文件关联的图片文件扩展名，由自动加载文件夹的功能使用。


#### --debug-mode=\<yes|no\>

只有在开发人员要求时才启用此选项。默认：no


### UI

#### --dark-mode=\<value\>

启用深色模式.

**always**  
始终（默认值）

**system**  
跟随系统。（需要win10及以上的系统）

**never**
从不


#### ---dark-theme=\<string\>

深色模式中使用的配色主题。默认：dark

[配色主题](#配色主题)


#### --light-theme=\<string\>

浅色模式中使用的配色主题。默认：light

[配色主题](#配色主题)


外部工具
--------------

### Play with mpv

[Play with mpv](https://chrome.google.com/webstore/detail/play-with-mpv/hahklcmnfgffdlchjigehabfbiigleji)
是一个支持调用mpv播放YouTube等网站视频的谷歌浏览器的扩展。

由于Chrome扩展无法启动一个应用程序，需要另一个与扩展程序通信的应用程序，该程序可以从
[此处](http://www.mediafire.com/file/lezj8lwqt5zf75v/play-with-mpvnet-server.7z/file)下载。
只有当该程序运行时扩展才能正常工作，为了让应用程序始终运行，应将其放在系统自启动目录中：

`C:\Users\%username%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup`

这将使该程序随系统共同启动，并在后台运行。当mpv.net的文件关联注册被执行后，
该程序应该会找到mpv.net的位置，或者将mpv.net文件夹添加到环境变量PATH。


### Open With

或者可以使用Chrome/Firefox扩展 [Open With](../../../issues/119) 。


### Open with++

[Open with++](#open-with) 可用来扩展资源管理器的上下文菜单
[Play with mpv.net](https://github.com/stax76/OpenWithPlusPlus#play-with-mpvnet) 和 
[Add to mpv.net playlist](https://github.com/stax76/OpenWithPlusPlus#add-to-mpvnet-playlist).
可用来获取 'Play with mpv.net' 和 'Add to mpv.net playlist' 的菜单子项


### Universal Remote 安卓应用

Universal Remote 是一个收费的安卓远程控制APP。

https://www.unifiedremote.com

https://play.google.com/store/apps/details?id=com.Relmtech.Remote

https://play.google.com/store/apps/details?id=com.Relmtech.RemotePaid

https://www.unifiedremote.com/tutorials/how-to-create-a-custom-keyboard-shortcuts-remote

https://www.unifiedremote.com/tutorials/how-to-install-a-custom-remote

[我的配置](./Universal%20Remote)

Universal Remote 的 File Browser 功能十分有用。


### One For All Contour URC1210 and FLIRC USB

然而我的主要远程控制解决方案适用于所有人， 
Philips code 0556 和 FLIRC USB (gen2) 一同被使用。


脚本
---------

#### Lua

文件类型： `lua`

文件位置： `<config folder>\scripts`

Lua脚本的host由libmpv内建。

没有调试支持，只有错误和调试消息输出在终端上。

Lua脚本在第一个媒体文件打开之前加载。

[mpv Lua 文档](https://mpv.io/manual/master/#lua-scripting)

[mpv用户脚本](https://github.com/mpv-player/mpv/wiki/User-Scripts)


#### JavaScript

文件类型： `js`

文件位置： `<config folder>\scripts`

JavaScriptLua脚本的host由libmpv内建。

没有调试支持，只有错误和调试消息输出在终端上。

JavaScript脚本在第一个媒体文件打开之前加载。

[mpv JavaScript 文档](https://mpv.io/manual/master/#javascript)

[mpv用户脚本](https://github.com/mpv-player/mpv/wiki/User-Scripts)


#### PowerShell

文件类型： `ps1`

文件位置： `<config folder>\scripts-ps`

PS脚本的host类似于扩展，在打开媒体文件前尚未初始化。

mpv.net没有定义脚本接口，而是公开了它的完整内部，没有兼容性保证。

[示例脚本](../../../tree/master/src/Scripts)


#### C#

文件类型： `cs`

文件位置： `<config folder>\scripts-cs`

mpv.net没有定义脚本接口，而是公开了它的完整内部，没有兼容性保证。

脚本代码可以在C#[扩展](../../../tree/master/src/Extensions)中编写，这样就可以获得
完整的代码和调试器支持。一旦代码被调试和开发完成，就可以将其从扩展转移到轻量级的独立脚本。

C#脚本的host类似于[扩展](../../../tree/master/src/Extensions)，在打开媒体文件前尚未初始化。

[示例脚本](../../../tree/master/src/Scripts)


扩展
----------

扩展位于设置目录的子文件夹 'extensions' 中，文件名必须与上级文件夹具有相同的名称：

```Text
<config folder>\extensions\ExampleExtension\ExampleExtension.dll
```

mpv.net没有定义脚本接口，而是公开了它的完整内部，没有兼容性保证。


### 创建扩展演示

- 下载安装 [Visual Studio Community](https://visualstudio.microsoft.com) 。
- 创建新的项目类型 **Class Library .NET Framework** 并确保项目名称以 **Extension** 作结尾。
- 增加一个 reference 到 **System.ComponentModel.Composition**.
- 增加一个 reference 到 mpvnet.exe ，在 Solution Explorer 中选择 mpvnet 的 reference ，
  打开 Properties 窗口并设置 **Copy Local** 为 false 来阻止在项目编译时 mpvnet.exe 被复制到输出目录。
- 现在打开 project properties 并在 Build 标签页设置 output path ，
  扩展类似于位于配置文件夹中的脚本，例如：
  `<config folder>\extensions\ExampleExtension\ExampleExtension.dll`
- 同样在 project properties 的 Debug 标签页中选择选项 **Start external program** 
  并且定义到 mpvnet.exe 的路径。
  在 Debug 标签页中你还可以定义 command line arguments ，例如开始调试时要播放的视频文件。


### 代码样本

#### RatingExtension

当mpv.net关闭时，这个扩展程序会对分级视频的文件名进行记录。

'input.conf defaults' 包含了用于设置评级扩展的键位绑定。

[源码](../../../tree/master/src/Extensions)


配色主题
-----------

mpv.net支持自定义配色主题，内置主题的定义可以在以下文件中查看：

[theme.txt](../../../tree/master/src/Resources/theme.txt)


自定义的配色方案保存在以下文件中：

`<conf folder>\theme.conf`

theme.conf 文件可包含无限量的主题。

在 config editor 的 UI 词条中存在 dark-theme 和 light-theme 的设置来定义主题应用在深色/浅色模式。


高级功能
-----------------

### 播放 VapourSynth 脚本

播放vpy文件的功能受 mpv.conf 中的以下参数支持：

```
[extension.vpy]
demuxer-lavf-format = vapoursynth
```

Python和VapourSynth必须在环境变量PATH中。


隐藏功能
---------------

使用mpv.net在资源管理器中选择多个文件并按下回车键打开。
资源管理器将此限制为最多15个并且顺序将是随机的。

当打开文件或url时，只要按下CTRL键，就不会清除播放列表，而是将文件或url附加到播放列表中。
这适用于mpv.net的所有打开文件或url的功能。

在打开单个文件时按下SHIFT键将禁止该文件夹的其它文件添加到播放列表中。

在全屏模式下点击顶部右上角关闭播放器。


与mpv的差异
---------------------------

mpv.net被设计的和mpv大体一致，但是有一些限制：


### 窗口限制

mpv.net实现了一个自己的主窗口，这意味着只有在mpv.net中有自己实现的mpv窗口功能才受支持。

当前不支持无窗口模式，即使从终端启动mpv.net并播放音乐，主窗口也始终可见。

mpv窗口特性的文档可以在此处找到：

https://mpv.io/manual/master/#window


**mpv.net目前已实现了以下窗口属性：**

- [border](https://mpv.io/manual/master/#options-border)
- [fullscreen](https://mpv.io/manual/master/#options-fullscreen)
- [keepaspect-window](https://mpv.io/manual/master/#options-keepaspect-window)
- [ontop](https://mpv.io/manual/master/#options-ontop)
- [screen](https://mpv.io/manual/master/#options-screen)
- [title](https://mpv.io/manual/master/#options-title)
- [window-maximized](https://mpv.io/manual/master/#options-window-maximized)
- [window-minimized](https://mpv.io/manual/master/#options-window-minimized)
- [window-scale](https://mpv.io/manual/master/#options-window-scale)


**部分支持的属性：**

- [autofit](https://mpv.io/manual/master/#options-autofit)
- [autofit-smaller](https://mpv.io/manual/master/#options-autofit-smaller)
- [autofit-larger](https://mpv.io/manual/master/#options-autofit-larger)


mpv.net的专属窗口功能在 [屏幕设置](#screen) 部分。


### 命令行限制

mpv.net支持基于属性的mpv命令行选项，这意味着它支持mpv几乎所有的命令行选项。

不支持的是非基于属性的选项。它们在mpv.net中有自己的实现，到目前为止实现的有：

--ad=help  
--audio-device=help  
--input-keylist  
--profile=help  
--vd=help  
--version  


### mpv.net的专属选项

在config editor中输入 `mpv.net` 检索这些选项，在[此处](#mpvnet的专属选项)的手册中有对应说明。

mpv.net的专属选项保存在 mpvnet.conf 文件中，与mpv一样可由命令行界面获取。


技术概览
------------------

mpv.net使用 C#7 编写并且需要 .NET Framework 4.8 来运行。

扩展的实现基于 
[Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/).

主窗口基于WinForms，与WPF相比对libmpv集成的更友好，所有其他窗口都是基于WPF的。

使用的第三方组件：

- [libmpv提供了核心功能](https://mpv.io/)
- [MediaInfo](https://mediaarea.net/en/MediaInfo)


上下文菜单
------------

mpv.net的上下文菜单由设置目录中的文件 input.conf 定义。

如果 input.conf 文件不存在，mpv.net由以下文件生成默认：

<https://github.com/stax76/mpv.net/tree/master/src/Resources/input.conf.txt>

input.conf 定义mpv的快捷键，同时mpv.net使用注释定义上下文菜单。


### Open > Open Files

菜单中的打开文件是在mpv.net中打开文件的一种方式，它支持多选。

打开文件的另一种方法是现有关联的文件资源管理器使用命令行。

第三种方法是拖放文件到主窗口上。

每当打开文件或URL时按下控制键时，播放列表不会被清除，但文件或URL会追加到播放列表中。
这适用于所有打开文件或URL的mpv.net的功能。

在打开单个文件时按shift键将临时禁止加载文件夹中的其它文件。

支持Blu-ray和DVD的ISO镜像文件。


### Open > Open URL or file path from clipboard

从剪贴板打开文件和URL。如何从YouTube等网站的浏览器中直接打开url在[外部工具](#外部工具)部分进行了描述。

对于网络流媒体，必须手动下载并安装youtube-dl，这意味着它必须位于环境变量PATH或启动目录中。


### Open > Open DVD/Blu-ray Drive/Folder

打开DVD/Blu-ray的驱动器/文件夹。


### Open > Load external audio files

允许加载外部音轨文件。也可以根据文件名自动检测，该选项可在下面的设置中找到 
'Settings > Show Config Editor > Audio > audio-file-auto' 。


### Open > Load external subtitle files

允许加载外部字幕文件。也可以根据文件名自动检测，该选项可在下面的设置中找到 
'Settings > Show Config Editor > Subtitles > sub-auto' 。


### Play/Pause

使用以下命令播放/暂停：

`cycle pause`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[pause property](https://mpv.io/manual/master/#options-pause)


### Stop

使用以下命令中止播放并关闭播放列表：

`stop`

[stop command](https://mpv.io/manual/master/#command-interface-stop)


### Toggle Fullscreen

使用以下命令切换全屏的状态：

`cycle fullscreen`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[fullscreen property](https://mpv.io/manual/master/#options-fs)


### Navigate > Previous File

使用以下命令跳转播放列表中的上一个文件：

`playlist-prev`

[playlist-prev command](https://mpv.io/manual/master/#command-interface-playlist-prev)


### Navigate > Next File

使用以下命令跳转播放列表中的下一个文件：

`playlist-next`

[playlist-next command](https://mpv.io/manual/master/#command-interface-playlist-next)


### Navigate > Next Chapter

使用以下命令跳转下一章节：

`add chapter 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[chapter property](https://mpv.io/manual/master/#command-interface-chapter)


### Navigate > Previous Chapter

使用以下命令跳转上一章节：

`add chapter -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[chapter property](https://mpv.io/manual/master/#command-interface-chapter)


### Navigate > Jump Next Frame

使用以下命令跳转下一帧：

`frame-step`

[frame-step command](https://mpv.io/manual/master/#command-interface-frame-step)


### Navigate > Jump Previous Frame

使用以下命令跳转上一帧：

`frame-back-step`

[frame-back-step command](https://mpv.io/manual/master/#command-interface-frame-back-step)


### Navigate > Jump

使用以下命令跳转（无OSD信息）：

`no-osd seek sec`

sec是跳转的相对秒数，使用no osd前缀是因为mpv.net包含一个脚本，
该脚本显示执行寻道操作时的位置，该脚本使用更简单的时间格式。

[no-osd command prefix](https://mpv.io/manual/master/#command-interface-no-osd)

[seek command](https://mpv.io/manual/master/#command-interface-seek-%3Ctarget%3E-[%3Cflags%3E])


### Pan & Scan > Increase Size

使用以下命令放大0.1视频尺寸：

`add video-zoom  0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)


### Pan & Scan > Decrease Size

使用以下命令缩小0.1视频尺寸：

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

重置平移、缩放的状态，同一行中的多个命令用“分号”分隔。

`set video-zoom 0; set video-pan-x 0; set video-pan-y 0`

[video-zoom property](https://mpv.io/manual/master/#options-video-zoom)

[video-pan-x, video-pan-y property](https://mpv.io/manual/master/#options-video-pan-y)


### Video > Decrease Contrast

使用以下命令减少1对比度：

`add contrast -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[contrast property](https://mpv.io/manual/master/#options-contrast)


### Video > Increase Contrast

使用以下命令增加1对比度：

`add contrast 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[contrast property](https://mpv.io/manual/master/#options-contrast)


### Video > Decrease Brightness

使用以下命令减少1亮度：

`add brightness -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[brightness property](https://mpv.io/manual/master/#options-brightness)


### Video > Increase Brightness

使用以下命令增加1亮度：

`add brightness 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[brightness property](https://mpv.io/manual/master/#options-brightness)


### Video > Decrease Gamma

使用以下命令减少1伽马：

`add gamma -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[gamma property](https://mpv.io/manual/master/#options-gamma)


### Video > Increase Gamma

使用以下命令增加1伽马：

`add gamma 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[gamma property](https://mpv.io/manual/master/#options-gamma)


### Video > Decrease Saturation

使用以下命令减少1饱和度：

`add saturation -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[saturation property](https://mpv.io/manual/master/#options-saturation)


### Video > Increase Saturation

使用以下命令增加1饱和度：

`add saturation 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[saturation property](https://mpv.io/manual/master/#options-saturation)


### Video > Take Screenshot

`async screenshot`

[async command prefix](https://mpv.io/manual/master/#command-interface-async)

[screenshot command](https://mpv.io/manual/master/#command-interface-screenshot-%3Cflags%3E)


### Video > Toggle Deinterlace

使用以下命令改变去隔行扫描的状态：

`cycle deinterlace`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[deinterlace property](https://mpv.io/manual/master/#options-deinterlace)


### Video > Cycle Aspect Ratio

使用以下命令改变长宽比：

`cycle-values video-aspect 16:9 4:3 2.35:1 -1`

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[video-aspect property](https://mpv.io/manual/master/#command-interface-video-aspect)


### Audio > Cycle/Next

使用mpv.net的命令改变到下一个可用的音轨并且不跳过音轨关闭（比mpv原生指令更好）


### Audio > Delay +0.1

使用以下命令增加0.1音频延迟：

`add audio-delay 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[audio-delay property](https://mpv.io/manual/master/#options-audio-delay)


### Audio > Delay -0.1

使用以下命令减少0.1音频延迟：

`add audio-delay -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[audio-delay property](https://mpv.io/manual/master/#options-audio-delay)


### Subtitle > Cycle/Next

使用以下命令改变到下一个可用的字幕轨：

`cycle sub`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[sub/sid property](https://mpv.io/manual/master/#options-sid)


### Subtitle > Toggle Visibility

使用以下命令改变字幕可见性的状态：

`cycle sub-visibility`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[sub-visibility property](https://mpv.io/manual/master/#options-no-sub-visibility)


### Subtitle > Delay -0.1

使用以下命令减少0.1字幕延迟：

`add sub-delay -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-delay property](https://mpv.io/manual/master/#options-sub-delay)


### Subtitle > Delay 0.1

使用以下命令增加0.1字幕延迟：

`add sub-delay 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-delay property](https://mpv.io/manual/master/#options-sub-delay)


### Subtitle > Move Up

使用以下命令上移1字幕位置：

`add sub-pos -1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-pos property](https://mpv.io/manual/master/#options-sub-pos)


### Subtitle > Move Down

使用以下命令下移1字幕位置：

`add sub-pos 1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-pos property](https://mpv.io/manual/master/#options-sub-pos)


### Subtitle > Decrease Subtitle Font Size

使用以下命令减少0.1字幕尺寸：

`add sub-scale -0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-scale property](https://mpv.io/manual/master/#options-sub-scale)


### Subtitle > Increase Subtitle Font Size

使用以下命令增加0.1字幕尺寸：

`add sub-scale 0.1`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[sub-scale property](https://mpv.io/manual/master/#options-sub-scale)


### Volume > Up

使用以下命令增加10音量：

`add volume 10`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)


### Volume > Down

使用以下命令减少10音量：

`add volume -10`

[add command](https://mpv.io/manual/master/#command-interface-add-%3Cname%3E-[%3Cvalue%3E])

[volume property](https://mpv.io/manual/master/#options-volume)


### Volume > Mute

使用以下命令改变静音状态：

`cycle mute`

[cycle command](https://mpv.io/manual/master/#command-interface-cycle-%3Cname%3E-[%3Cvalue%3E])

[mute property](https://mpv.io/manual/master/#options-mute)


### Speed > -10%

使用以下命令递退1.1倍的播放速度：

`multiply speed 1/1.1`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > 10%

使用以下命令递进1.1倍的播放速度：

`multiply speed 1.1`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > Half

使用以下命令半速播放：

`multiply speed 0.5`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > Double

使用以下命令倍速播放：

`multiply speed 2`

[multiply command](https://mpv.io/manual/master/#command-interface-multiply-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Speed > Reset

使用以下命令重置播放速度：

`set speed 1`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cname%3E-%3Cvalue%3E)

[speed property](https://mpv.io/manual/master/#options-speed)


### Extensions > Rating > 0stars

一个将评级写入文件名的插件。


### View > On Top > Enable

使用以下命令启用播放器置顶：

`set ontop yes`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cname%3E-%3Cvalue%3E)

[ontop property](https://mpv.io/manual/master/#options-ontop)


### View > On Top > Disable

使用以下命令禁用播放器置顶：

`set ontop no`

[set command](https://mpv.io/manual/master/#command-interface-set-%3Cname%3E-%3Cvalue%3E)

[ontop property](https://mpv.io/manual/master/#options-ontop)


### View > File Info

使用mpv.net的命令显示当前文件的信息（持续时间、位置、格式、大小和文件名）。


### View > Show Statistics

使用以下命令临时显示数据统计：

`script-binding stats/display-stats`

[script-binding command](https://mpv.io/manual/master/#command-interface-script-binding)


### View > Toggle Statistics

使用以下命令切换数据统计的状态：

`script-binding stats/display-stats-toggle`

[script-binding command](https://mpv.io/manual/master/#command-interface-script-binding)


### View > Toggle OSC Visibility

使用以下命令切换OSC可见性的状态：

`script-binding osc/visibility`

[script-binding command](https://mpv.io/manual/master/#command-interface-script-binding)


### View > Show Playlist

使用以下命令显示5秒的（原生OSD）播放列表：

`show-text ${playlist} 5000`

[show-text command](https://mpv.io/manual/master/#command-interface-show-text)


### View > Show Audio/Video/Subtitle List

使用以下命令显示5秒的轨道信息列表：

`show-text ${track-list} 5000`

[show-text command](https://mpv.io/manual/master/#command-interface-show-text)


### Settings > Show Config Editor

显示mpv.net的设置编辑器


### Settings > Show Input Editor

显示mpv.net的快捷键编辑器


### Settings > Open Config Folder

打开包含以下文件的设置文件夹：

mpv.conf 文件内涵mpv的设置

mpvnet.conf 文件内涵mpvnet的设置

input.conf 文件内涵mpv的键鼠绑定

用户脚本和扩展


### Tools > Command Palette

显示命令面板，该窗口允许快速查找、执行命令和快捷键。


### Tools > Show History

显示包含历史记录的文本文件。如果文件不存在，则会询问是否在设置文件夹中创建该文件。
一旦文件存在，则写入历史记录（包括时间和文件名）

屏蔽部分路径的参数：

script-opt = history-discard=path1;path2

### Tools > Set/clear A-B loop points

使用以下命令创建单个片段循环：

`ab-loop`

[ab-loop command](https://mpv.io/manual/master/#command-interface-ab-loop)


### Tools > Toggle infinite file looping

使用以下命令改变当前文件的循环播放的状态：

`cycle-values loop-file "inf" "no"`

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[loop-file command](https://mpv.io/manual/master/#options-loop)


### Tools > Toggle Hardware Decoding

使用以下命令改变硬件解码的状态：

`cycle-values hwdec "auto" "no"`

[cycle-values command](https://mpv.io/manual/master/#command-interface-cycle-values)

[hwdec property](https://mpv.io/manual/master/#options-hwdec)


### Tools > Setup

允许管理文件关联。


### Help > Show mpv manual

显示 [mpv官方手册](https://mpv.io/manual/stable/).


### Help > Show mpv.net web site

显示 [mpv.net 网站](https://mpv-net.github.io/mpv.net-web-site/).


### Help > Show mpv.net manual

显示 [mpv.net手册](https://github.com/stax76/mpv.net/blob/master/Manual.md).


### Help > About mpv.net

显示mpv.net的关于信息。


### Exit

使用以下命令退出mpv.net：

`quit`

[quit command](https://mpv.io/manual/master/#command-interface-quit-[%3Ccode%3E])


### Exit Watch Later

使用以下命令退出mpv.net并且记住文件状态：

`quit-watch-later`

[quit-watch-later command](https://mpv.io/manual/master/#command-interface-quit-watch-later)
