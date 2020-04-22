
# Shows the Open File dialog to open a file without loading its folder into the playlist.

# In input.conf add: <key> script-message load-without-folder

$job = Register-ObjectEvent -InputObject ([mpvnet.mp]) -EventName ClientMessage -Action {

    # exit if message does not equal 'load-without-folder'
    if ($args.Length -ne 1 -or $args[0] -ne 'load-without-folder')
    {
        exit
    }

    $dialog = New-Object Windows.Forms.OpenFileDialog

    if ($dialog.ShowDialog() -ne "OK") {
        $dialog.Dispose()
        exit
    }

    [mp]::Load($dialog.FileNames, $false, $false);
    $dialog.Dispose()
}

$ScriptHost.RedirectEventJobStreams($job)
