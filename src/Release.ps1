
$tmpDir      = 'D:\Work'
$exePath     = $PSScriptRoot + '\bin\mpvnet.exe'
$versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath)
$inno        = 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe'
$7z          = 'C:\Program Files\7-Zip\7z.exe'

$cloudDirectories = 'C:\Users\frank\OneDrive\Public\mpv.net\',
                    'C:\Users\frank\Dropbox\Public\mpv.net\'

function UploadBeta($sourceFile)
{
    foreach ($cloudDirectory in $cloudDirectories)
    {
        if (-not (Test-Path $cloudDirectory))
        {
            throw $cloudDirectory
        }

        $targetFile = $cloudDirectory + (Split-Path $sourceFile -Leaf)

        if (Test-Path $targetFile)
        {
            throw $targetFile
        }

        Copy-Item $sourceFile $targetFile
    }
}

if ($versionInfo.FilePrivatePart -eq 0)
{
    & $inno $PSScriptRoot\setup.iss
    if ($LastExitCode) { throw $LastExitCode }

    $targetDir = $tmpDir + "\mpv.net-$($versionInfo.FileVersion)-portable"
    Copy-Item $PSScriptRoot\bin $targetDir -Recurse -Exclude 'System.Management.Automation.xml', 'settings-directory.txt'
    & $7z a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }

    Set-Clipboard ($versionInfo.FileVersion + "`n`nChangelog:`n`n" +
        'https://github.com/stax76/mpv.net/blob/master/Changelog.md' + "`n`nDownload:`n`n" +
        'https://github.com/stax76/mpv.net/blob/master/Manual.md#stable')
}
else
{
    $targetDir = "$tmpDir\mpv.net-$($versionInfo.FileVersion)-portable-beta"
    Copy-Item $PSScriptRoot\bin $targetDir -Recurse -Exclude 'System.Management.Automation.xml', 'settings-directory.txt'
    & $7z a -t7z -mx9 "$targetDir.7z" -r "$targetDir\*"
    if ($LastExitCode) { throw $LastExitCode }
    UploadBeta "$targetDir.7z"

    foreach ($cloudDirectory in $cloudDirectories)
    {
        Invoke-Item $cloudDirectory
    }

    Set-Clipboard ($versionInfo.FileVersion + " Beta`n`nChangelog:`n`n" +
        'https://github.com/stax76/mpv.net/blob/master/Changelog.md' + "`n`nDownload:`n`n" +
        'https://github.com/stax76/mpv.net/blob/master/Manual.md#beta')
}

Write-Host 'successfully finished' -ForegroundColor Green
