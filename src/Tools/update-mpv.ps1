
<#

Updates mpv (x64) and libmpv (x64 , ARM64).

Files are downloaded from:
    x64:   github.com/zhongfly/mpv-winbuild
    ARM64: github.com/Andarwinux/mpv-winbuild

Requires 7zip being installed at 'C:\Program Files\7-Zip\7z.exe'.

Needs 3 positional CLI arguments:
    1. Directory where mpv x64 is located. To skip pass '-'.
    2. Directory where libmpv x64 is located. To skip pass '-'.
    3. Directory where libmpv ARM64 is located. To skip pass '-'.
#>

$7ZipPath = 'C:\Program Files\7-Zip\7z.exe'

$MpvDirX64 = $args[0]
$LibmpvDirX64 = $args[1]
$LibmpvDirARM64 = $args[2]

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
function Download($apiURL, $pattern) {
    $json = Invoke-WebRequest $apiURL -MaximumRedirection 0 -ErrorAction Ignore -UseBasicParsing | ConvertFrom-Json
    $filename = ($json.assets | Where-Object { $_.name -Match $pattern }).name
    $path = Join-Path $env:TEMP $filename
    $link = ($json.assets | Where-Object { $_.name -Match $pattern }).browser_download_url
    Invoke-WebRequest -Uri $link -UserAgent "mpv-win-updater" -OutFile $path
    return Test $path
}

# Unpack archive
function Unpack($archieveFile, $outputRootDir) {
    $outputDir = Join-Path $outputRootDir $archieveFile.BaseName
    if (Test-Path $outputDir) { Remove-Item $outputDir -Recurse }
    $process = Start-Process (Test $7ZipPath) @('x', $archieveFile.FullName, "-o$outputDir") -NoNewWindow -Wait
    if ($process.ExitCode) { throw $process.ExitCode }
    return Test $outputDir
}

# Update mpv x64

if (Test-Path (Join-Path $MpvDirX64 'mpv.exe')) {
    $apiURL = "https://api.github.com/repos/zhongfly/mpv-winbuild/releases/latest"
    $archiveFile = Get-Item (Download $apiURL "mpv-x86_64-[0-9]{8}")
    $archiveDir = Unpack $archiveFile $env:TEMP
    Remove-Item "$MpvDirX64\*" -Force -Recurse
    Copy-Item "$archiveDir\*" $MpvDirX64 -Force -Recurse
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
} else {
    "mpv x64 location not found:`n$MpvDirX64"
}

# Update libmpv x64

if (Test-Path (Join-Path $LibmpvDirX64 'libmpv-2.dll')) {
    $apiURL = "https://api.github.com/repos/zhongfly/mpv-winbuild/releases/latest"
    $archiveFile = Get-Item (Download $apiURL "mpv-dev-x86_64-[0-9]{8}")
    $archiveDir = Unpack $archiveFile $env:TEMP
    Copy-Item $archiveDir\libmpv-2.dll $LibmpvDirX64 -Force
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
} else {
    "libmpv x64 location not found:`n$LibmpvDirX64"
}

# Update libmpv ARM64

if (Test-Path (Join-Path $LibmpvDirARM64 'libmpv-2.dll')) {
    $apiURL = "https://api.github.com/repos/Andarwinux/mpv-winbuild/releases/latest"
    $archiveFile = Get-Item (Download $apiURL "mpv-dev-aarch64-[0-9]{8}")
    $archiveDir = Unpack $archiveFile $env:TEMP
    Copy-Item $archiveDir\libmpv-2.dll $LibmpvDirARM64 -Force
    Remove-Item $archiveFile.FullName
    Remove-Item $archiveDir -Recurse
} else {
    "libmpv ARM64 location not found:`n$LibmpvDirARM64"
}

if (Test-Path (Join-Path $MpvDirX64 'mpv.exe')) {
    Get-Item (Join-Path $MpvDirX64 'mpv.exe')
}

if (Test-Path (Join-Path $LibmpvDirX64 'libmpv-2.dll')) {
    Get-Item (Join-Path $LibmpvDirX64 'libmpv-2.dll')
}

if (Test-Path (Join-Path $LibmpvDirARM64 'libmpv-2.dll')) {
    Get-Item (Join-Path $LibmpvDirARM64 'libmpv-2.dll')
}
