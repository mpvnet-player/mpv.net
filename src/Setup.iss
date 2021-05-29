
#define MyAppName "mpv.net"
#define MyAppExeName "mpvnet.exe"
#define MyAppSourceDir "bin"
#define MyAppVersion GetFileVersion("bin\mpvnet.exe")

[Setup]
AppId={{9AA2B100-BEF3-44D0-B819-D8FC3C4D557D}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Frank Skare (stax76)
ArchitecturesInstallIn64BitMode=x64
Compression=lzma2
DefaultDirName={commonpf}\{#MyAppName}
OutputBaseFilename=mpv.net-{#MyAppVersion}-setup
OutputDir=D:\Work
DefaultGroupName={#MyAppName}
SetupIconFile=mpvnet.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Files]
Source: "{#MyAppSourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppSourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "System.Management.Automation.xml,settings-directory.txt"

[UninstallRun]
Filename: "powershell.exe"; Flags: runhidden; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\Setup\remove file associations.ps1"""
Filename: "powershell.exe"; Flags: runhidden; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\Setup\remove start menu shortcut.ps1"""
Filename: "powershell.exe"; Flags: runhidden; Parameters: "-NoProfile -ExecutionPolicy Bypass -File ""{app}\Setup\remove environment variable.ps1"""
