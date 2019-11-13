
$ErrorActionPreference = 'Stop'
$targetDir = (Split-Path $args[1]) + "\new version"
$targetFile = $targetDir + '.7z'

Write-Host 'Download new version' -ForegroundColor Green
curl.exe $args[0] --location --output $targetFile
if (-not $?) { throw "curl error: $LastExitCode" }

Write-Host 'Unpack new version' -ForegroundColor Green
& ($args[1] + '\7z\7za.exe') x -y $targetFile -o"$targetDir"
if (-not $?) { throw "7zip error: $LastExitCode" }

Write-Host 'Delete downloaded file' -ForegroundColor Green
Remove-Item $targetFile -Force -Recurse

Write-Host 'Delete current version' -ForegroundColor Green
Remove-Item $args[1] -Force -Recurse

Write-Host 'Rename directory' -ForegroundColor Green
Rename-Item $targetDir (Split-Path $args[1] -Leaf)

Write-Host 'Update is complete' -ForegroundColor Green
