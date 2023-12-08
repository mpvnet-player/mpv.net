
# # Extract msgids from xaml files in project into pot file.
# . $PSScriptRoot/XGetText-Xaml.ps1
# $xamlFiles = Get-ChildItem -Recurse -File -Filter *.xaml |
#                 Where { $_.FullName -NotLike '*\obj\*' } | ForEach-Object { $_.FullName }
# XGetText-Xaml -o obj/xamlmessages.pot -k Gettext,GettextFormatConverter $xamlFiles


# # Merge two pot files into one
# msgcat.exe --use-first -o obj/result.pot obj/source.pot obj/xamlmessages.pot


# $Header = @"
# # Translation of mpv.net to ???
# # Copyright (C) 2023 Frank Skare (stax76) <frank.skare.de@gmail.com>
# # Copyright (C) 2023 ???
# # This file is distributed under the same license as the mpv.net package.
# msgid ""
# msgstr ""
# "Project-Id-Version: mpv.net\n"
# "Report-Msgid-Bugs-To: Frank Skare (stax76) <frank.skare.de@gmail.com>\n"
# "POT-Creation-Date: 2023-11-04 16:50+0100\n"
# "PO-Revision-Date: 2023-11-04 16:54+0100\n"
# "Last-Translator: ???\n"
# "Language-Team: ???\n"
# "Language: ???\n"
# "MIME-Version: 1.0\n"
# "Content-Type: text/plain; charset=UTF-8\n"
# "Content-Transfer-Encoding: 8bit\n"
# "Plural-Forms: nplurals=2; plural=(n != 1);\n"

# "@

# $Locales = @('bg', 'ca', 'cs', 'de', 'es', 'eu', 'fr', 'it', 'ja', 'ko', 'lt', 'nl', 'pl', 'pt', 'pt_BR', 'ro', 'ru', 'sr_RS', 'sr_RS@latin', 'sv', 'tr', 'uk', 'zh_CN', 'zh_SG', 'zh_TW')
# $Locales | foreach { $Header -replace '"Language: \?\?\?\\n"', "`"Language: $($_)\n`"" |
#     Out-File $PSScriptRoot\PO\$_.po }
