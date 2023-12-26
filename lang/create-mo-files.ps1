
$ErrorActionPreference = 'Stop'

$PoFiles = Get-ChildItem $PSScriptRoot/po
$ExeFolder = "$PSScriptRoot/../src/MpvNet.Windows/bin/Debug"

function CreateFolder
{
    param($path)

    if (-not (Test-Path $path))
    {
        mkdir $path
    }

    if (-not (Test-Path $path))
    {
        throw
    }
}

foreach ($it in $PoFiles)
{
    $folder = "$ExeFolder/Locale/$($it.BaseName)/LC_MESSAGES"

    if (-not (Test-Path $folder))
    {
        New-Item -ItemType Directory -Path $folder
    }

    $moPath = "$folder/mpvnet.mo"
    msgfmt --output-file=$moPath $it.FullName
    if ($LastExitCode) { throw $LastExitCode }
    $moPath
}
