
# Shows the Open File dialog to open a file without loading its folder into the playlist.

# In input.conf add: <key> script-message load-without-folder

$code = {
    if ($args[0] -eq 'load-without-folder')
    {
        $dialog = New-Object Windows.Forms.OpenFileDialog

        if ($dialog.ShowDialog() -eq 'OK')
        {
            $core.LoadFiles($dialog.FileNames, $false, $false);
        }

        $dialog.Dispose()
    }
}

$mp.RegisterEvent("client-message", $code)
