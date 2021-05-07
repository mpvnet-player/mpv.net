
// This script shows the playlist.

function showPlaylist()
{
    // set font size
    mp.set_property_number("osd-font-size", 40);

    // show playlist for 5 seconds
    mp.command("show-text ${playlist} 5000");

    // restore original font size in 6 seconds
    setTimeout(resetFontSize, 6000);
}

// restore original font size
function resetFontSize()
{
    mp.set_property_number("osd-font-size", size);
}

// save original font size
var size = mp.get_property_number("osd-font-size");

// input.conf: key script-binding show-playlist
mp.add_key_binding(null, "show-playlist", showPlaylist);
