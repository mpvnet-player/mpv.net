
# Shows the current file in File Explorer

# In input.conf add: <key> script-message show-in-file-explorer

$code = {
    if ($args[0] -eq 'show-in-file-explorer')
    {
        # probably works only with shell execute for which powershell has no built-in support
        [Diagnostics.Process]::Start('explorer.exe', '/n, /select, "' + $mp.get_property_string('path') + '"')
    }
}

$mp.register_event("client-message", $code)
