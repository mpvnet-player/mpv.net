
<#

Updates mpv and libmpv used by mpv.net.
It uses the Path environment variable to find mpv and mpv.net.
Files are downloaded from github.com/zhongfly/mpv-winbuild.
Requires 7zip being installed at 'C:\Program Files\7-Zip\7z.exe'.

#>

$Zip7Path = 'C:\Program Files\7-Zip\7z.exe'

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
    $process = Start-Process (Test $Zip7Path) @('x', $archieveFile.FullName, "-o$outputDir") -NoNewWindow -Wait
    if ($process.ExitCode) { throw $process.ExitCode }
    return Test $outputDir
}

function UpdateLibmpv($targetFolder) {
    if ($targetFolder -eq 'no') { return }
    $archiveFile = Get-Item (Download "mpv-dev-x86_64-[0-9]{8}")
    $archiveDir =  Unpack $archiveFile $env:TEMP
    Copy-Item $archiveDir\libmpv-2.dll (Test $targetFolder) -Force
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
}

function UpdateMpv($targetFolder) {
    if ($targetFolder -eq 'no') { return }
    $archiveFile = Get-Item (Download "mpv-x86_64-[0-9]{8}")
    $archiveDir =  Unpack $archiveFile $env:TEMP
    Copy-Item "$archiveDir\*" $targetFolder -Force -Recurse
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
}

# Update mpv

$MpvLocations = @() + (cmd /c where mpv.exe)

if ($MpvLocations.Length -gt 0) {
    $mpvDir = Split-Path ($MpvLocations[0])
    ''; 'mpv found at:'; $mpvDir
    $result = Read-Host 'Update mpv? [y/n]'

    if ($result -eq 'y') {
        UpdateMpv $mpvDir
    }
} else {
    'mpv location not found.'
}

# Update libmpv used by mpv.net

$MpvNetLocations = @() + (cmd /c where mpvnet.exe)

if ($MpvNetLocations.Length -gt 0) {
    $mpvNetDir = Split-Path ($MpvNetLocations[0])
    ''; 'mpv.net found at:'; $mpvNetDir
    $result = Read-Host 'Update libmpv? [y/n]'

    if ($result -eq 'y') {
        UpdateLibmpv $mpvNetDir
    }
} else {
    'mpv.net location not found.'
}
