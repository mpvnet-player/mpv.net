
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
    New-Item -ItemType Directory -Path $folder
    $moPath = "$folder/mpvnet.mo"
    New-Item -ItemType File -Path $moPath
    msgfmt --output-file=$moPath $it.FullName

    if ($LASTEXITCODE -ne 0)
    {
        throw
    }

    $moPath
}
