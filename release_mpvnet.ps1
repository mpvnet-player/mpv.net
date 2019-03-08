using namespace System.Diagnostics
$exePath = "C:\Users\frank\C-Daten\Projekte\VS\CS\mpvnet\mpvnet\bin\Debug\mpvnet.exe"
$version = [FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$targetDir = "C:\Users\Frank\Desktop\mpv.net-" + $version
Copy-Item C:\Users\frank\C-Daten\Projekte\VS\CS\mpvnet\mpvnet\bin\Debug $targetDir -recurse
$addonDir = $targetDir + "\Addons"
remove-item $addonDir -Recurse -Include *vbnet.pdb, *mpvnet.exe, *mpvnet.exe.config, *mpvnet.pdb, *vbnet.dll
C:\Users\frank\C-Daten\Projekte\VS\VB\util\bin\util.exe -pack $targetDir