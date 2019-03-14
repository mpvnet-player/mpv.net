$scriptDir = Split-Path -Path $PSCommandPath -Parent
$exePath = $scriptDir + "\mpv.net\bin\Debug\mpvnet.exe"
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$desktopDir = [Environment]::GetFolderPath("Desktop")
$targetDir = $desktopDir + "\mpv.net-" + $version
if (Test-Path $targetDir) { rd $targetDir -recurse }
Copy-Item $scriptDir\mpv.net\bin\Debug $targetDir -recurse
$addonDir = $targetDir + "\Addons"
Remove-Item $addonDir -Recurse -Include *mpvnet.exe, *mpvnet.exe.config, *mpvnet.pdb
$7zPath = "C:\Program Files\7-Zip\7z.exe"
$args = "a -t7z -mx9 $targetDir.7z -r $targetDir\*"
#Start-Process -FilePath $7zPath -ArgumentList $args