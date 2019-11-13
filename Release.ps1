
$ErrorActionPreference = 'Stop'

$desktopDir = [Environment]::GetFolderPath('Desktop')
$exePath    = (Get-Location).Path + '\mpv.net\bin\x64\mpvnet.exe'
$version    = [Reflection.Assembly]::LoadFile($exePath).GetName().Version
$msbuild    = 'C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe'
$iscc       = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
$7z         = 'C:\Program Files\7-Zip\7z.exe'

if ($version.Revision -ne 0)
{
    & $msbuild mpv.net.sln /p:Configuration=Debug /p:Platform=x64
    if ($LastExitCode) { throw $LastExitCode }

    $targetDir = "$desktopDir\mpv.net-portable-x64-$version-beta"
    Copy-Item .\mpv.net\bin\x64 $targetDir -Recurse -Exclude System.Management.Automation.xml
    & $7z a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }

    $targetDirectories = 'C:\Users\frank\OneDrive\StaxRip\TestBuilds\',
                         'C:\Users\frank\Dropbox\public\StaxRip\Builds\'

    foreach ($dir in $targetDirectories)
    {
        if (Test-Path $dir)
        {
            Copy-Item ($targetDir + '.7z') ($dir + (Split-Path $targetDir -Leaf) + '.7z')
            Invoke-Item $dir
        }
    }
}
else
{
    & $msbuild mpv.net.sln /p:Configuration=Debug /p:Platform=x64
    if ($LastExitCode) { throw $LastExitCode }

    & $msbuild mpv.net.sln /p:Configuration=Debug /p:Platform=x86
    if ($LastExitCode) { throw $LastExitCode }

    & $iscc /Darch=x64 setup.iss
    if ($LastExitCode) { throw $LastExitCode }

    & $iscc /Darch=x86 setup.iss
    if ($LastExitCode) { throw $LastExitCode }

    $targetDir = $desktopDir + "\mpv.net-portable-x64-$version"
    Copy-Item .\mpv.net\bin\x64 $targetDir -Recurse -Exclude System.Management.Automation.xml

    & $7z a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }

    & $7z a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }

    $targetDir = $desktopDir + "\mpv.net-portable-x86-$version"
    Copy-Item .\mpv.net\bin\x86 $targetDir -Recurse -Exclude System.Management.Automation.xml

    & $7z a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }

    & $7z a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }
}

Write-Host 'successfully finished' -ForegroundColor Green
