
# Shows the current file in File Explorer

# In input.conf add: <key> script-message show-in-file-explorer

$code = {
    if ($args[0] -eq 'show-in-file-explorer')
    {
        Start-Process explorer.exe '/n,','/select,',"$($mp.GetPropertyString('path'))"
    }
}

$mp.RegisterEvent("client-message", $code)
