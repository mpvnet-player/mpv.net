Here is a list of scripts that users of mpv(.net) have published, adding functionality that is not part of the core mpv(.net) player. Anyone can add their own script by editing this wiki. Scripts are usually placed in C:\Users\user\AppData\Roaming\mpv\scripts

### C#

```
using mpvnet;

class Script
{
    public Script()
    {
        var fs = mp.get_property_string("fullscreen");
        mp.commandv("show-text", "fullscreen: " + fs);
        mp.observe_property_bool("fullscreen", FullscreenChange);
    }

    void FullscreenChange(bool val)
    {
        mp.commandv("show-text", "fullscreen: " + val.ToString());
    }
}
```

### Python

```
# when seeking displays position and
# duration like so: 70:00 / 80:00
# which is different from mpv which
# uses 01:10:00 / 01:20:00

import math


def seek():
    pos = mp.get_property_number("time-pos")
    dur = mp.get_property_number("duration")

    if pos > dur:
        pos = dur

    mp.commandv('show-text', format(pos) + " / " + format(dur))

def format(f):
    sec = round(f)
    
    if sec < 0:
        sec = 0
    
    pos_min_floor = math.floor(sec / 60)
    sec_rest = sec - pos_min_floor * 60
    return add_zero(pos_min_floor) + ":" + add_zero(sec_rest)

def add_zero(val):
    val = round(val)
    return "" + str(int(val)) if (val > 9) else "0" + str(int(val))

mp.register_event("seek", seek) # or use: mp.Seek += seek
```

### PowerShell

```
$position = [mp]::get_property_number("time-pos");
[mp]::commandv("show-text", $position.ToString() + " seconds")
```
Please note that PowerShell don't allow assigning to events and mpv.net uses as workaround a matching script filename, a list of available events can be found in the mpv manual or in the file mp.cs in the mpv.net source code.