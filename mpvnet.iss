#define MyAppName "mpv.net"
#define MyAppVersion GetFileVersion("mpv.net\bin\mpvnet.exe")
#define MyAppExeName "mpvnet.exe"

[Setup]
AppId={{9AA2B100-BEF3-44D0-B819-D8FC3C4D557D}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=stax76
ArchitecturesInstallIn64BitMode=x64
Compression=lzma2
DefaultDirName={commonpf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=mpvnet-{#MyAppVersion}
OutputDir=C:\Users\frank\Desktop
ChangesAssociations=yes

[Files]
Source: "C:\Users\frank\Daten\Projekte\CS\mpv.net\mpv.net\bin\mpvnet.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\frank\Daten\Projekte\CS\mpv.net\mpv.net\bin\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "System.Management.Automation.xml"

[Run]
Filename: "{app}\defapp.exe"; Description: "Associate video file extensions?"; Flags: postinstall unchecked runascurrentuser runhidden nowait; Parameters: """{app}\{#MyAppExeName}"" mpg avi vob mp4 mkv avs 264 mov wmv flv h264 asf webm mpeg mpv y4m avc hevc 265 h265 m2v m2ts vpy mts webm m4v"
Filename: "{app}\defapp.exe"; Description: "Associate audio file extensions?"; Flags: postinstall unchecked runascurrentuser runhidden nowait; Parameters: """{app}\{#MyAppExeName}"" mp2 mp3 ac3 wav w64 m4a dts dtsma dtshr dtshd eac3 thd thd+ac3 ogg mka aac opus flac mpa"

[UninstallRun]
Filename: "{app}\defapp.exe"; Flags: runascurrentuser runhidden nowait; Parameters: """{app}\{#MyAppExeName}"""