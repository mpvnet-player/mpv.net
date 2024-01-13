
$ErrorActionPreference = 'Stop'

# Write list of .cs files into cs-files.txt file
Get-ChildItem $PSScriptRoot/.. -Recurse -File -Filter '*.cs' |
    Where-Object { $_ -notmatch '[/\\]obj[/\\]' } |
    ForEach-Object { $_.FullName } |
    Out-File $PSScriptRoot/cs-files.txt

# Create .pot file
xgettext -k_ -k_n:1,2 -k_p:1c,2 -k_pn:1c,2,3 --force-po --from-code=UTF-8 '--language=c#' -o $PSScriptRoot/source.pot --files-from=$PSScriptRoot/cs-files.txt --keyword=_
if ($LastExitCode) { throw $LastExitCode }

# Backup .po files
$BackupTargetFolder = $env:TEMP + '/mpv.net po backup ' + (Get-Date -Format 'yyyy-MM-dd HH_mm_ss')
Copy-Item $PSScriptRoot/po $BackupTargetFolder -Force -Recurse
'PO file backup: ' + (Resolve-Path $BackupTargetFolder)

# Update .po files
(Get-ChildItem $PSScriptRoot/PO -Filter '*.po').FullName |
    ForEach-Object { msgmerge --sort-output --backup=none --update $_ $PSScriptRoot/source.pot }

if ($LastExitCode) { throw $LastExitCode }
