
function XGetText-Xaml {
    param(
    [Parameter(Mandatory=$true,
               Position=0,
               ParameterSetName="sourceFiles",
               HelpMessage="XAML files to extract msgids from.")]
    [ValidateNotNullOrEmpty()]
    [string[]]$sourceFiles,
    [Parameter(Mandatory=$true,
    HelpMessage="Additional keywords that match MarkupExtensions enclosing msgids to be extracted.")]
    [Alias("k")]
    [string[]]
    $Keywords,
    [Parameter(Mandatory=$false,
    HelpMessage="Write output to specified file.")]
    [Alias("o")]
    [string]$output="messages.pot")

    $extractedIds = New-Object -TypeName System.Collections.Hashtable

	ForEach ($keyword in $Keywords)
	{
		ForEach ($sourceFile in $sourceFiles)
		{
			Select-String $sourceFile -Pattern $("{[a-z]?[a-z0-9]*:"+$keyword+ " (([^}{]|{[^}]*})*)}") -AllMatches | ForEach-Object {
				$filename = $sourceFile
				$lineNumber = $_.LineNumber
				$_.Matches | ForEach-Object {
					$msgid = $_.Groups[1].ToString()

					if ($msgid.StartsWith("'") -and $msgid.EndsWith("'")){
						$msgid  = $msgid.Substring(1, $msgid.Length-2);
					}

					$msgid = $msgid.Replace("\'", "'")
					$msgid = $msgid.Replace("\,", ",")

					if (-Not $extractedIds.ContainsKey($msgid))
					{
					   $extractedIds.Add($msgid, @{Locations = New-Object System.Collections.ArrayList})
					}
					[void] $extractedIds[$msgid].Locations.Add('#: ' + $Filename + ':' + $LineNumber)
				}
			}
		}
	}

    $result = '#, fuzzy
msgid ""
msgstr ""
"POT-Creation-Date: ' + $(Get-Date -Format 'yyyy-mm-dd HH:mmK') + '\n"
"MIME-Version: 1.0\n"
"Content-Type: text/plain; charset=utf-8\n"
"Content-Transfer-Encoding: 8bit\n\n"' + [System.Environment]::NewLine + [System.Environment]::NewLine
    $extractedIds.GetEnumerator() | ForEach-Object {
		if ($_.Key -like '*|*' ) {
			$msgid = $_.Key.Substring($_.Key.indexof("|") +1)
			$msgctxt = $_.Key.Substring(0, $_.Key.indexof("|"))

			$result = $result + $($_.Value.Locations -join [System.Environment]::NewLine) + [System.Environment]::NewLine + "msgctxt """ + $msgctxt + """" + [System.Environment]::NewLine + "msgid """ + $msgid + """" + [System.Environment]::NewLine + "msgstr """"" + [System.Environment]::NewLine + [System.Environment]::NewLine + [System.Environment]::NewLine
		}
		else {
			$result = $result + $($_.Value.Locations -join [System.Environment]::NewLine) + [System.Environment]::NewLine + "msgid """ + $_.Key + """" + [System.Environment]::NewLine + "msgstr """"" + [System.Environment]::NewLine + [System.Environment]::NewLine + [System.Environment]::NewLine
		}
    }

    if ($output -eq '-') {
        Write-Output $result.ToString()
    } else {
		[System.IO.File]::WriteAllLines($ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($output), ($result -replace "\r", ""))
    }
}
