
namespace MpvNet.Help;

public class MpvHelp
{
    public static string? WM_APPCOMMAND_to_mpv_key(int value) => value switch
    {
        5 => "SEARCH",         // BROWSER_SEARCH
        6 => "FAVORITES",      // BROWSER_FAVORITES
        7 => "HOMEPAGE",       // BROWSER_HOME
        15 => "MAIL",          // LAUNCH_MAIL
        33 => "PRINT",         // PRINT
        11 => "NEXT",          // MEDIA_NEXTTRACK
        12 => "PREV",          // MEDIA_PREVIOUSTRACK
        13 => "STOP",          // MEDIA_STOP
        14 => "PLAYPAUSE",     // MEDIA_PLAY_PAUSE
        46 => "PLAY",          // MEDIA_PLAY
        47 => "PAUSE",         // MEDIA_PAUSE
        48 => "RECORD",        // MEDIA_RECORD
        49 => "FORWARD",       // MEDIA_FAST_FORWARD
        50 => "REWIND",        // MEDIA_REWIND
        51 => "CHANNEL_UP",    // MEDIA_CHANNEL_UP
        52 => "CHANNEL_DOWN",  // MEDIA_CHANNEL_DOWN
        _ => null,
    };
}
