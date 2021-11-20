// When seeking displays position and duration like so: 70:00 / 80:00
// Which is different from most players which use: 01:10:00 / 01:20:00
// In input.conf set the input command prefix no-osd infront of the seek command.

function add_zero(val) {
    val = Math.round(val);
    return val > 9 ? "" + val : "0" + val;
}

function format(val) {
    var sec = Math.round(val);

    if (sec < 3600) {
        if (sec < 0)
            sec = 0;
        pos_min_floor = Math.floor(sec / 60);
        sec_rest = sec - pos_min_floor * 60;
        return add_zero(pos_min_floor) + ":" + add_zero(sec_rest);
    }

    hours = Math.floor(sec / 3600);
    sec %= 3600;
    minutes = Math.floor(sec / 60);
    seconds = sec % 60;
    return add_zero(hours) + ":" + add_zero(minutes) + ":" + add_zero(seconds);
}

function on_seek(_) {
    pos = mp.get_property_number("time-pos");
    dur = mp.get_property_number("duration");

    if (pos > dur)
        pos = dur;

    mp.commandv("show-text", format(pos) + " / " + format(dur));
}

mp.register_event("seek", on_seek);
