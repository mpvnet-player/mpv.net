function CheckExitCode {
    if ($LastExitCode -gt 0) {
        Write-Host "`nExit code $LastExitCode was returned.`n" -ForegroundColor Red
        exit
    }
}

$msbuild   = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
$innoSetup = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
$sevenZip  = "C:\Program Files\7-Zip\7z.exe"

& $msbuild mpv.net.sln /p:Configuration=Debug /p:Platform=x64; CheckExitCode
& $msbuild mpv.net.sln /p:Configuration=Debug /p:Platform=x86; CheckExitCode

& $innoSetup /Darch="x64" setup.iss; CheckExitCode
& $innoSetup /Darch="x86" setup.iss; CheckExitCode

$scriptDir = Split-Path -Path $PSCommandPath -Parent
$desktopDir = [Environment]::GetFolderPath("Desktop")
$exePath = $scriptDir + "\mpv.net\bin\x64\mpvnet.exe"
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$targetDir = $desktopDir + "\mpv.net-portable-x64-" + $version
Copy-Item $scriptDir\mpv.net\bin\x64 $targetDir -Recurse -Exclude System.Management.Automation.xml -Force
& $sevenZip a -t7z  -mx9 "$targetDir.7z"  -r "$targetDir\*"; CheckExitCode
& $sevenZip a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"; CheckExitCode

$exePath = $scriptDir + "\mpv.net\bin\x86\mpvnet.exe"
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exePath).FileVersion
$targetDir = $desktopDir + "\mpv.net-portable-x86-" + $version
Copy-Item $scriptDir\mpv.net\bin\x86 $targetDir -Recurse -Exclude System.Management.Automation.xml -Force
& $sevenZip a -t7z  -mx9 "$targetDir.7z"  -r "$targetDir\*"; CheckExitCode
& $sevenZip a -tzip -mx9 "$targetDir.zip" -r "$targetDir\*"; CheckExitCode