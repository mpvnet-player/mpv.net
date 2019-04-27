#define MyAppName "mpv.net"
#define MyAppVersion GetFileVersion("mpv.net\bin\mpvnet.exe")
#define MyAppExeName "mpvnet.exe"
#define MyAppSourceDir "mpv.net\bin"

[Setup]
AppId={{9AA2B100-BEF3-44D0-B819-D8FC3C4D557D}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=stax76
ArchitecturesInstallIn64BitMode=x64
Compression=lzma2
DefaultDirName={pf}\{#MyAppName}
OutputBaseFilename=mpvnet-setup-x64-{#MyAppVersion}
OutputDir=C:\Users\frank\Desktop
DefaultGroupName={#MyAppName}

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Files]
Source: "{#MyAppSourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppSourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "System.Management.Automation.xml"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Associate video file extensions?"; Flags: postinstall unchecked runascurrentuser runhidden nowait; Parameters: "--reg-file-assoc video"
Filename: "{app}\{#MyAppExeName}"; Description: "Associate audio file extensions?"; Flags: postinstall unchecked runascurrentuser runhidden nowait; Parameters: "--reg-file-assoc audio"

[UninstallRun]
Filename: "{app}\{#MyAppExeName}"; Flags: runascurrentuser runhidden; Parameters: "--reg-file-assoc unregister"