
$ErrorActionPreference = 'Stop'

if ($args.Length -ne 2) {
    Write-Host 'Invalid arguments' -ForegroundColor Red
    pause
    exit
}

if (-not (Get-Command curl.exe)) {
    Write-Host 'Error using curl.exe' -ForegroundColor Red
    pause
    exit
}

if ($PSVersionTable.PSVersion.Major -lt 5) {
    Write-Host 'PowerShell 5.1 not found' -ForegroundColor Red
    pause
    exit
}

Add-Type -AssemblyName Microsoft.VisualBasic

# Delete directory using recycle bin
function Remove-Directory($directory) {
    if (Test-Path $directory) {
        [Microsoft.VisualBasic.FileIO.FileSystem]::
            DeleteDirectory($directory, 'OnlyErrorDialogs', 'SendToRecycleBin')
    }
}

$currentDir = $args[1];

$targetDir = (Split-Path $currentDir) + "\new version"
Remove-Directory $targetDir
$targetFile = $targetDir + '.7z'
Remove-Item $targetFile -Force -ErrorAction Ignore

Write-Host 'Download new version' -ForegroundColor Green
curl.exe $args[0] --location --output $targetFile
if ($LastExitCode) { throw $LastExitCode }

Write-Host 'Unpack new version' -ForegroundColor Green
& ($currentDir + '\7z\7za.exe') x -y $targetFile -o"$targetDir"
if ($LastExitCode) { throw $LastExitCode }

Write-Host 'Delete downloaded file' -ForegroundColor Green
Remove-Item $targetFile -Force -Recurse

$portableConfigDir = $currentDir + '\portable_config'
$portableConfigTempDir = (Split-Path $currentDir) + '\portable config dir temp'
Remove-Item $portableConfigTempDir -Force -ErrorAction Ignore

if (Test-Path $portableConfigDir) {
    Write-Host 'Backup portable config' -ForegroundColor Green
    Copy-Item $portableConfigDir $portableConfigTempDir -Recurse -Force
}

Write-Host 'Delete current version' -ForegroundColor Green
Remove-Directory $currentDir

Write-Host 'Rename directory' -ForegroundColor Green
Rename-Item $targetDir (Split-Path $currentDir -Leaf)

if (Test-Path $portableConfigTempDir) {
    Write-Host 'Restore portable config' -ForegroundColor Green
    Move-Item $portableConfigTempDir $portableConfigDir -Force
}

Write-Host 'Update is complete' -ForegroundColor Green
