
#define MyAppName "mpv.net"
#define MyAppExeName "mpvnet.exe"

#ifndef arch
  #define arch "x64"
#endif

#if arch == "x64"
  #define MyAppSourceDir "mpv.net\bin\x64"
  #define MyAppVersion GetFileVersion("mpv.net\bin\x64\mpvnet.exe")
#else
  #define MyAppSourceDir "mpv.net\bin\x86"
  #define MyAppVersion GetFileVersion("mpv.net\bin\x86\mpvnet.exe")
#endif

[Setup]
AppId={{9AA2B100-BEF3-44D0-B819-D8FC3C4D557D}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Frank Skare (stax76)

#if arch == "x64"
  ArchitecturesInstallIn64BitMode=x64
#endif

Compression=lzma2
DefaultDirName={commonpf}\{#MyAppName}
OutputBaseFilename=mpv.net-{#MyAppVersion}-setup-{#arch}
OutputDir={#GetEnv('USERPROFILE')}\Desktop
DefaultGroupName={#MyAppName}
SetupIconFile=mpv.net\mpvnet.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Files]
Source: "{#MyAppSourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyAppSourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs;

[UninstallRun]
Filename: "powershell.exe"; Flags: runhidden; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Setup\remove file associations.ps1"""
Filename: "powershell.exe"; Flags: runhidden; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Setup\remove start menu shortcut.ps1"""
Filename: "powershell.exe"; Flags: runhidden; Parameters: "-ExecutionPolicy Bypass -File ""{app}\Setup\remove environment variable.ps1"""
