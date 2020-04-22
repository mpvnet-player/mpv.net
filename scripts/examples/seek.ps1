
# Display position in window title bar when seeking

$job = Register-ObjectEvent -InputObject ([mpvnet.mp]) -EventName Seek -Action {
    [MainForm]::Instance.Text = [mp]::get_property_number("time-pos")
}

$ScriptHost.RedirectEventJobStreams($job)
