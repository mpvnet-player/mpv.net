
$tmpDir      = 'D:\Work'
$exePath     = $PSScriptRoot + '\bin\mpvnet.exe'
$versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath)
$7z          = 'C:\Program Files\7-Zip\7z.exe'

$targetDir = $tmpDir + "\mpvnet-$($versionInfo.FileVersion)-beta"
Copy-Item $PSScriptRoot\bin $targetDir -Recurse -Exclude System.Management.Automation.xml

$folders = 'Debug', 'Release', 'x64', 'x86', 'Arm'

foreach ($folder in $folders) {
    Remove-Item (Join-Path $targetDir $folder) -Recurse -ErrorAction SilentlyContinue
}

& $7z a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"

if ($LastExitCode)
    { throw $LastExitCode }

Write-Host 'successfully finished' -ForegroundColor Green
