
Set-Variable wasPaused $false -Option AllScope

$code = {
    $isMinimized = $args[0]
    $isPaused = $mp.get_property_bool('pause')

    if ($isMinimized)
    {
        if (-not $isPaused)
        {
            $mp.set_property_bool('pause', $true)
            $wasPaused = $true
        }
    }
    else
    {
        if ($wasPaused -and $isPaused)
        {
            $mp.set_property_bool('pause', $false)
        }

        $wasPaused = $false
    }
}

$mp.observe_property('window-minimized', 'bool', $code)
