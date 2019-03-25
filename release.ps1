$scriptDir = Split-Path -Path $PSCommandPath -Parent
$exePath = $scriptDir + "\mpv.net\bin\Debug\mpvnet.exe"
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$desktopDir = [Environment]::GetFolderPath("Desktop")
$targetDir = $desktopDir + "\mpv.net-" + $version
Copy-Item C:\Users\frank\Daten\Projekte\CS\mpv.net\mpvSettingsEditor\bin\mpvSettingsEditor.exe "C:\Users\frank\Daten\Projekte\CS\mpv.net\mpv.net\bin\Debug\mpvSettingsEditor.exe" -Force
Copy-Item C:\Users\frank\Daten\Projekte\CS\mpv.net\mpvSettingsEditor\bin\Definitions.toml      "C:\Users\frank\Daten\Projekte\CS\mpv.net\mpv.net\bin\Debug\Definitions.toml" -Force
Copy-Item $scriptDir\mpv.net\bin\Debug $targetDir -Recurse -Exclude System.Management.Automation.xml -Force
Copy-Item $scriptDir\README.md $targetDir\README.md -Force
$7zPath = "C:\Program Files\7-Zip\7z.exe"
$args = "a -t7z -mx9 $targetDir.7z -r $targetDir\*"
Start-Process -FilePath $7zPath -ArgumentList $args