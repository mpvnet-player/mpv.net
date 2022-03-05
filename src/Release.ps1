
$tmpDir      = 'D:\Work'
$exePath     = $PSScriptRoot + '\bin\mpvnet.exe'
$versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath)
$7z          = 'C:\Program Files\7-Zip\7z.exe'

$targetDir = $tmpDir + "\mpv.net-$($versionInfo.FileVersion)-portable-beta"
Copy-Item $PSScriptRoot\bin $targetDir -Recurse -Exclude 'System.Management.Automation.xml'
& $7z a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"
if ($LastExitCode) { throw $LastExitCode }

Write-Host 'successfully finished' -ForegroundColor Green
