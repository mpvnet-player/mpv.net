$position = [mp]::get_property_number("time-pos");
[mp]::commandv("show-text", $position.ToString() + " seconds")