
<#

This script updates mpv and libmpv using github.com/zhongfly/mpv-winbuild

Two positional command line arguments need to be passed into the script:

1. The directory containing libmpv to be updated.
2. The directory containing mpv to be updated.

To skip one of both pass 'no' instead of the path.

Requires 7zip being installed at 'C:\Program Files\7-Zip\7z.exe'

#>

$zip7Path = 'C:\Program Files\7-Zip\7z.exe'
$ScriptArgs = $args

# Stop when the first error occurs
$ErrorActionPreference = 'Stop'

# Throw exception if file or folder does not exist
function Test($path) {
    if (-not (Test-Path $path)) {
        throw $path
    }
    return $path
}

# Download file to temp dir and return file path
function Download($pattern) {
    $api = "https://api.github.com/repos/zhongfly/mpv-winbuild/releases/latest"
    $json = Invoke-WebRequest $api -MaximumRedirection 0 -ErrorAction Ignore -UseBasicParsing | ConvertFrom-Json
    $filename = ($json.assets | Where-Object { $_.name -Match $pattern }).name
    $path = Join-Path $env:TEMP $filename
    $link = ($json.assets | Where-Object { $_.name -Match $pattern }).browser_download_url
    Invoke-WebRequest -Uri $link -UserAgent "mpv-win-updater" -OutFile $path
    return Test $path
}

function Unpack($archieveFile, $outputRootDir) {
    $outputDir = Join-Path $outputRootDir $archieveFile.BaseName
    if (Test-Path $outputDir) { Remove-Item $outputDir -Recurse }
    $process = Start-Process (Test $zip7Path) @('x', $archieveFile.FullName, "-o$outputDir") -NoNewWindow -Wait
    if ($process.ExitCode) { throw $process.ExitCode }
    return Test $outputDir
}

function UpdateLibmpv {
    $targetFolder = $ScriptArgs[0]
    if ($targetFolder -eq 'no') { return }
    $archiveFile = Get-Item (Download "mpv-dev-x86_64-[0-9]{8}")
    $archiveDir =  Unpack $archiveFile $env:TEMP
    Copy-Item $archiveDir\libmpv-2.dll (Test $targetFolder) -Force
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
}

function UpdateMpv() {
    $targetFolder = $ScriptArgs[1]
    if ($targetFolder -eq 'no') { return }
    $archiveFile = Get-Item (Download "mpv-x86_64-[0-9]{8}")
    $archiveDir =  Unpack $archiveFile $env:TEMP
    Copy-Item "$archiveDir\mpv\*" $targetFolder -Force -Recurse
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
}

UpdateLibmpv
UpdateMpv

Write-Host 'Script finished successfully' -ForegroundColor Green
