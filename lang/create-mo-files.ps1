
$ErrorActionPreference = 'Stop'

$PoFiles = Get-ChildItem $PSScriptRoot/po
$ExeFolder = "$PSScriptRoot/../src/MpvNet.Windows/bin/Debug"

foreach ($it in $PoFiles)
{
    $folder = "$ExeFolder/Locale/$($it.BaseName)/LC_MESSAGES"

    if (-not (Test-Path $folder))
    {
        New-Item -ItemType Directory -Path $folder | Out-Null
    }

    $moPath = "$folder/mpvnet.mo"

    if (-not (Test-Path $moPath))
    {
        New-Item -ItemType File -Path $moPath | Out-Null
    }

    msgfmt --output-file=$moPath $it.FullName
    if ($LastExitCode) { throw $LastExitCode }
    $moPath
}
