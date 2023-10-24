
# # Extract msgids from xaml files in project into pot file.

# . $PSScriptRoot/XGetText-Xaml.ps1
# $xamlFiles = Get-ChildItem -Recurse -File -Filter *.xaml |
#                 Where { $_.FullName -NotLike '*\obj\*' } | ForEach-Object { $_.FullName }
# XGetText-Xaml -o obj/xamlmessages.pot -k Gettext,GettextFormatConverter $xamlFiles

$Root = $PSScriptRoot

# Write list of .cs files into CS-Files.txt file
Get-ChildItem $Root/../.. -Recurse -File -Filter '*.cs' |
    where { $_ -notmatch '[/\\]obj[/\\]' } |
    foreach { $_.FullName } |
    Out-File $Root/CS-Files.txt

# Extract msgids from cs files in project into pot file
xgettext --force-po --from-code=UTF-8 '--language=c#' -o $Root/CS-Messages.pot --files-from=$Root/CS-Files.txt --keyword=_

# # Merge two pot files into one
# msgcat.exe --use-first -o obj/result.pot obj/CS-Messages.pot obj/xamlmessages.pot

# Update po files with most recent msgids
$Locales = @("de")
$PoFiles = $Locales | foreach { 'Locale/' + $_ + '/LC_MESSAGES/MpvNet.po' }

$PoFiles | foreach {
	# msgmerge --sort-output --update $_ $Root/CS-Messages.pot 2> $null
}

# echo "Po files updated with current msgIds: " $poFiles
# echo "You may now edit these files with PoEdit (https://poedit.net/)"
