
# # Extract msgids from xaml files in project into pot file.

# . $PSScriptRoot/XGetText-Xaml.ps1
# $xamlFiles = Get-ChildItem -Recurse -File -Filter *.xaml |
#                 Where { $_.FullName -NotLike '*\obj\*' } | ForEach-Object { $_.FullName }
# XGetText-Xaml -o obj/xamlmessages.pot -k Gettext,GettextFormatConverter $xamlFiles

# # Write list of .cs files into cs-files.txt file
# Get-ChildItem $PSScriptRoot/.. -Recurse -File -Filter '*.cs' |
#     where { $_ -notmatch '[/\\]obj[/\\]' } |
#     foreach { $_.FullName } |
#     Out-File $PSScriptRoot/cs-files.txt

# # Extract msgids from cs files in project into pot file
# xgettext --force-po --from-code=UTF-8 '--language=c#' -o $PSScriptRoot/template.pot --files-from=$PSScriptRoot/cs-files.txt --keyword=_

# # Merge two pot files into one
# msgcat.exe --use-first -o obj/result.pot obj/template.pot obj/xamlmessages.pot

# Update po files with most recent msgids
$Locales = @('bg', 'ca', 'cs', 'de', 'es', 'eu', 'fr', 'it', 'ja', 'ko', 'lt', 'nl', 'pl', 'pt', 'pt_BR', 'ro', 'ru', 'sr_RS', 'sr_RS@latin', 'sv', 'tr', 'uk', 'zh_CN', 'zh_SG', 'zh_TW')
$PoFiles = $Locales | foreach { "$PSScriptRoot/PO/$_.po" }

$PoFiles | foreach {
	msgmerge --sort-output --backup=none --update $_ $PSScriptRoot/template.pot
}
