
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
    CreateFolder $folder
    $moPath = Resolve-Path $folder/mpvnet.mo -ErrorAction Ignore
    msgfmt --output-file=$moPath $it.FullName

    if ($LASTEXITCODE -ne 0)
    {
        throw
    }

    $moPath
}
