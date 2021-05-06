
$code = {
    $isMinimized = $args[0]
    $isPaused = $mp.get_property_bool('pause')

    if ($isMinimized)
    {
        if (-not $isPaused)
        {
            $mp.set_property_bool('pause', $true)
            $script:wasPaused = $true
        }
    }
    else
    {
        if ($script:wasPaused -and $isPaused)
        {
            $mp.set_property_bool('pause', $false)
        }

        $script:wasPaused = $false
    }
}

$mp.observe_property('window-minimized', 'bool', $code)
