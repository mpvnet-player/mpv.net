
$code = {
    $isMinimized = $args[0]
    $isPaused = $mp.GetPropertyBool('pause')

    if ($isMinimized)
    {
        if (-not $isPaused)
        {
            $mp.SetPropertyBool('pause', $true)
            $script:wasPaused = $true
        }
    }
    else
    {
        if ($script:wasPaused -and $isPaused)
        {
            $mp.SetPropertyBool('pause', $false)
        }

        $script:wasPaused = $false
    }
}

$mp.ObserveProperty('window-minimized', 'bool', $code)
