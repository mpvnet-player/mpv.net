
using System.Text;

namespace MpvNet;

public static class InputHelp
{
    public static List<Binding> GetDefaults()
    {
        List<Binding> b = [];

        Add(b, new (_("File"), _("Open Files..."), "script-message-to mpvnet open-files", "o"));
        Add(b, new (_("File"), _("Open URL or file from clipboard"), "script-message-to mpvnet open-clipboard", "Ctrl+v"));
        Add(b, new (_("File"), _("Open DVD/Blu-ray Drive/Folder..."), "script-message-to mpvnet open-optical-media"));
        Add(b, new (_("File"), "-"));
        Add(b, new (_("File"), _("Add external audio files..."), "script-message-to mpvnet load-audio"));
        Add(b, new (_("File"), _("Add external subtitle files..."), "script-message-to mpvnet load-sub"));
        Add(b, new (_("File"), "-"));
        Add(b, new (_("File"), _("Add files to playlist..."), "script-message-to mpvnet open-files append"));
        Add(b, new (_("File"), _("Add files/URLs to playlist from clipboard"), "script-message-to mpvnet open-clipboard append", "Ctrl+Shift+v"));
        Add(b, new (_("File"), "-"));
        Add(b, new (_("File"), _("Recent Files")));
        Add(b, new (_("File"), _("Exit"), "quit", "Esc"));

        Add(b, new (_("Playback"), _("Play/Pause"), "script-message-to mpvnet play-pause", "Space"));
        Add(b, new (_("Playback"), _("Stop"), "stop", "Ctrl+s"));

        Add(b, new (_("Navigate"), _("Previous File"), "playlist-prev", "F11"));
        Add(b, new (_("Navigate"), _("Next File"), "playlist-next", "F12"));
        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("First File"), "script-message-to mpvnet playlist-first", "Home"));
        Add(b, new (_("Navigate"), _("Last File"), "script-message-to mpvnet playlist-last", "End"));

        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("Next Chapter"), "add chapter 1", "PGUP"));
        Add(b, new (_("Navigate"), _("Previous Chapter"), "add chapter -1", "PGDWN"));
        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("Jump To Next Frame"), "frame-step", "."));
        Add(b, new (_("Navigate"), _("Jump To Previous Frame"), "frame-back-step", ","));
        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("Jump 5 sec forward"), "seek 5", "Right"));
        Add(b, new (_("Navigate"), _("Jump 5 sec backward"), "seek -5", "Left"));
        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("Jump 30 sec forward"), "seek 30", "Up"));
        Add(b, new (_("Navigate"), _("Jump 30 sec backward"), "seek -30", "Down"));
        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("Jump 5 min forward"), "seek 300", "Ctrl+Right"));
        Add(b, new (_("Navigate"), _("Jump 5 min backward"), "seek -300", "Ctrl+Left"));
        Add(b, new (_("Navigate"), "-"));
        Add(b, new (_("Navigate"), _("Title")));
        Add(b, new (_("Navigate"), _("Chapter")));

        Add(b, new (_("Pan & Scan"), _("Decrease Size"), "add video-zoom -0.1", "Ctrl+-"));
        Add(b, new (_("Pan & Scan"), _("Increase Size"), "add video-zoom 0.1", "Ctrl++"));
        Add(b, new (_("Pan & Scan"), "-"));
        Add(b, new (_("Pan & Scan"), _("Move Left"), "add video-pan-x -0.01", "Ctrl+KP4"));
        Add(b, new (_("Pan & Scan"), _("Move Right"), "add video-pan-x 0.01", "Ctrl+KP6"));
        Add(b, new (_("Pan & Scan"), "-"));
        Add(b, new (_("Pan & Scan"), _("Move Up"), "add video-pan-y -0.01", "Ctrl+KP8"));
        Add(b, new (_("Pan & Scan"), _("Move Down"), "add video-pan-y 0.01", "Ctrl+KP2"));
        Add(b, new (_("Pan & Scan"), "-"));
        Add(b, new (_("Pan & Scan"), _("Decrease Height"), "add panscan -0.1", "w"));
        Add(b, new (_("Pan & Scan"), _("Increase Height"), "add panscan 0.1", "W"));
        Add(b, new (_("Pan & Scan"), "-"));
        Add(b, new (_("Pan & Scan"), _("Reset"), "set video-zoom 0; set video-pan-x 0; set video-pan-y 0", "Ctrl+BS"));

        Add(b, new (_("Video"), _("Decrease Contrast"), "add contrast -1", "Ctrl+1"));
        Add(b, new (_("Video"), _("Increase Contrast"), "add contrast 1", "Ctrl+2"));
        Add(b, new (_("Video"), "-"));
        Add(b, new (_("Video"), _("Decrease Brightness"), "add brightness -1", "Ctrl+3"));
        Add(b, new (_("Video"), _("Increase Brightness"), "add brightness 1", "Ctrl+4"));
        Add(b, new (_("Video"), "-"));
        Add(b, new (_("Video"), _("Decrease Gamma"), "add gamma -1", "Ctrl+5"));
        Add(b, new (_("Video"), _("Increase Gamma"), "add gamma 1", "Ctrl+6"));
        Add(b, new (_("Video"), "-"));
        Add(b, new (_("Video"), _("Decrease Saturation"), "add saturation -1", "Ctrl+7"));
        Add(b, new (_("Video"), _("Increase Saturation"), "add saturation 1", "Ctrl+8"));
        Add(b, new (_("Video"), "-"));
        Add(b, new (_("Video"), _("Take Screenshot"), "async screenshot", "s"));
        Add(b, new (_("Video"), _("Take Screenshot without subtitles"), "async screenshot video", "S"));
        Add(b, new (_("Video"), _("Toggle Deinterlace"), "cycle deinterlace", "d"));
        Add(b, new (_("Video"), _("Change Aspect Ratio"), "cycle-values video-aspect-override 16:9 4:3 2.35:1 0 -1", "a"));
        Add(b, new (_("Video"), _("Rotate Video"), "cycle-values video-rotate 90 180 270 0", "Ctrl+r"));

        Add(b, new (_("Audio"), _("Audio Device")));
        Add(b, new (_("Audio"), _("Next Track"), "script-message-to mpvnet cycle-audio", "KP7"));
        Add(b, new (_("Audio"), "-"));
        Add(b, new (_("Audio"), _("Delay +0.1"), "add audio-delay 0.1", "Ctrl+d"));
        Add(b, new (_("Audio"), _("Delay -0.1"), "add audio-delay -0.1", "Ctrl+D"));

        Add(b, new (_("Subtitle"), _("Next Track"), "script-message-to mpvnet cycle-subtitles", "KP8"));
        Add(b, new (_("Subtitle"), _("Toggle Visibility"), "cycle sub-visibility", "v"));
        Add(b, new (_("Subtitle"), "-"));
        Add(b, new (_("Subtitle"), _("Delay -0.1"), "add sub-delay -0.1", "z"));
        Add(b, new (_("Subtitle"), _("Delay +0.1"), "add sub-delay 0.1", "Z"));
        Add(b, new (_("Subtitle"), "-"));
        Add(b, new (_("Subtitle"), _("Move Up"), "add sub-pos -1", "r"));
        Add(b, new (_("Subtitle"), _("Move Down"), "add sub-pos 1", "R"));
        Add(b, new (_("Subtitle"), "-"));
        Add(b, new (_("Subtitle"), _("Decrease Font Size"), "add sub-scale -0.1", "F"));
        Add(b, new (_("Subtitle"), _("Increase Font Size"), "add sub-scale 0.1", "G"));
        Add(b, new (_("Subtitle"), "-"));
        Add(b, new (_("Subtitle") + " > " + _("More"), _("Toggle overriding SSA/ASS styles with normal styles"), "cycle-values sub-ass-override force no", "u"));

        Add(b, new ("", _("Track")));

        Add(b, new (_("Volume"), _p("Volume", "Up"), "add volume 2", "+"));
        Add(b, new (_("Volume"), _p("Volume", "Down"), "add volume -2", "-"));
        Add(b, new (_("Volume"), "-"));
        Add(b, new (_("Volume"), _("Mute"), "cycle mute", "m"));

        Add(b, new (_("Speed"), _("-10%"), "multiply speed 1/1.1", "["));
        Add(b, new (_("Speed"), _("+10%"), "multiply speed 1.1", "]"));
        Add(b, new (_("Speed"), "-"));
        Add(b, new (_("Speed"), _("Half"), "multiply speed 0.5", "{"));
        Add(b, new (_("Speed"), _("Double"), "multiply speed 2.0", "}"));
        Add(b, new (_("Speed"), "-"));
        Add(b, new (_("Speed"), _("Reset"), "set speed 1", "BS"));

        Add(b, new (_("View"), _("Playlist"), "script-binding select/select-playlist", "F8"));
        Add(b, new (_("View"), _("Toggle Statistics"), "script-binding stats/display-stats-toggle", "t"));
        Add(b, new (_("View"), _("Toggle OSC Visibility"), "script-binding osc/visibility", "Del"));
        Add(b, new (_("View"), _("Media Info On-Screen"), "script-message-to mpvnet show-media-info osd", "i"));
        Add(b, new (_("View"), _("Media Info Message Box"), "script-message-to mpvnet show-media-info msgbox", "Ctrl+m"));
        Add(b, new (_("View"), _("Progress"), "show-progress", "p"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("On-Screen Menu"), "script-binding select/menu", "F1"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Playlist"), "script-binding select/select-playlist", "F8"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Bindings"), "script-binding select/select-binding", "F2"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Properties"), "script-binding select/show-properties", "F3"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Chapters"), "script-binding select/select-chapter", "Alt+c"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Tracks"), "script-binding select/select-track", "F9"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Audio Tracks"), "script-binding select/select-aid", "Alt+a"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Subtitle Tracks"), "script-binding select/select-sid", "Alt+s"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Secondary Subtitle"), "script-binding select/select-secondary-sid", "Alt+b"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Video Tracks"), "script-binding select/select-vid", "Alt+v"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Editions"), "script-binding select/select-edition", "Alt+e"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Subtitle Lines"), "script-binding select/select-subtitle-line", "Alt+l"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Audio Devices"), "script-binding select/select-audio-device", "Alt+d"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Watch History"), "script-binding select/select-watch-history", "Alt+h"));
        Add(b, new (_("View") + " > " + _("On-Screen Menu"), _("Watch Later"), "script-binding select/select-watch-later"));
        Add(b, new (_("View"), "-"));
        Add(b, new (_("View") + " > " + _("More"), _("Console"), "script-binding console/enable", "`"));
        Add(b, new (_("View") + " > " + _("More"), _("Commands"), "script-message-to mpvnet show-commands", "F4"));
        Add(b, new (_("View") + " > " + _("More"), _("Active Bindings In Text Editor"), "script-message-to mpvnet show-bindings"));
        Add(b, new (_("View") + " > " + _("More"), _("Active Bindings On-Screen"), "script-binding stats/display-page-4-toggle", "?"));
        Add(b, new (_("View") + " > " + _("More"), _("Keys"), "script-message-to mpvnet show-keys", "Alt+k"));
        Add(b, new (_("View") + " > " + _("More"), _("Protocols"), "script-message-to mpvnet show-protocols", "Alt+p"));
        Add(b, new (_("View") + " > " + _("More"), _("Decoders"), "script-message-to mpvnet show-decoders"));
        Add(b, new (_("View") + " > " + _("More"), _("Demuxers"), "script-message-to mpvnet show-demuxers"));

        Add(b, new (_("Window"), _("Fullscreen"), "cycle fullscreen", "Enter"));
        Add(b, new (_("Window") + " > " + _("Zoom"), _("Enlarge"), "script-message-to mpvnet scale-window 1.2", "Alt++"));
        Add(b, new (_("Window") + " > " + _("Zoom"), _("Shrink"), "script-message-to mpvnet scale-window 0.8", "Alt+-"));
        Add(b, new (_("Window") + " > " + _("Zoom"), "-"));
        Add(b, new (_("Window") + " > " + _("Zoom"), _("50 %"), "script-message-to mpvnet window-scale 0.5", "Alt+0"));
        Add(b, new (_("Window") + " > " + _("Zoom"), _("100 %"), "script-message-to mpvnet window-scale 1.0", "Alt+1"));
        Add(b, new (_("Window") + " > " + _("Zoom"), _("200 %"), "script-message-to mpvnet window-scale 2.0", "Alt+2"));
        Add(b, new (_("Window") + " > " + _("Zoom"), _("300 %"), "script-message-to mpvnet window-scale 3.0", "Alt+3"));
        Add(b, new (_("Window") + " > " + _("Move"), _p("Move", "Left"), "script-message-to mpvnet move-window left", "Alt+Left"));
        Add(b, new (_("Window") + " > " + _("Move"), _p("Move", "Right"), "script-message-to mpvnet move-window right", "Alt+Right"));
        Add(b, new (_("Window") + " > " + _("Move"), _p("Move", "Up"), "script-message-to mpvnet move-window top", "Alt+Up"));
        Add(b, new (_("Window") + " > " + _("Move"), _p("Move", "Down"), "script-message-to mpvnet move-window bottom", "Alt+Down"));
        Add(b, new (_("Window") + " > " + _("Move"), _p("Move", "Center"), "script-message-to mpvnet move-window center", "Alt+BS"));
        Add(b, new (_("Window"), _("Toggle Border"), "cycle border", "b"));
        Add(b, new (_("Window"), _("Toggle On Top"), "cycle ontop", "Ctrl+t"));

        Add(b, new ("", _("Profile")));

        Add(b, new (_("Config"), _("Show Config Editor"), "script-message-to mpvnet show-conf-editor", "Ctrl+,"));
        Add(b, new (_("Config"), _("Show Input Editor"), "script-message-to mpvnet show-input-editor", "Ctrl+i"));
        Add(b, new (_("Config"), "-"));
        Add(b, new (_("Config"), _("Edit mpv.conf"), "script-message-to mpvnet edit-conf-file mpv.conf", "c"));
        Add(b, new (_("Config"), _("Edit input.conf"), "script-message-to mpvnet edit-conf-file input.conf", "k"));
        Add(b, new (_("Config"), "-"));
        Add(b, new (_("Config"), _("Open Config Folder"), "script-message-to mpvnet open-conf-folder", "Ctrl+f"));
        Add(b, new (_("Config") + " > " + _("Setup"), _("Register video file associations"), "script-message-to mpvnet reg-file-assoc video"));
        Add(b, new (_("Config") + " > " + _("Setup"), _("Register audio file associations"), "script-message-to mpvnet reg-file-assoc audio"));
        Add(b, new (_("Config") + " > " + _("Setup"), _("Register image file associations"), "script-message-to mpvnet reg-file-assoc image"));
        Add(b, new (_("Config") + " > " + _("Setup"), _("Unregister file associations"), "script-message-to mpvnet reg-file-assoc unreg"));
        Add(b, new (_("Config") + " > " + _("Setup"), "-"));
        Add(b, new (_("Config") + " > " + _("Setup"), _("Add mpv.net to Path environment variable"), "script-message-to mpvnet add-to-path"));
        Add(b, new (_("Config") + " > " + _("Setup"), _("Remove mpv.net from Path environment variable"), "script-message-to mpvnet remove-from-path"));

        Add(b, new (_("Tools"), _("Set/clear A-B loop points"), "ab-loop", "l"));
        Add(b, new (_("Tools"), _("Toggle infinite file looping"), "cycle-values loop-file inf no", "L"));
        Add(b, new (_("Tools"), _("Shuffle Playlist"), "playlist-shuffle"));
        Add(b, new (_("Tools"), _("Toggle Hardware Decoding"), "cycle-values hwdec auto no", "Ctrl+h"));
        Add(b, new (_("Tools"), _("Exit Watch Later"), "quit-watch-later", "Q"));

        Add(b, new ("", _("Custom")));

        Add(b, new (_("Help"), _("Website mpv"), "script-message-to mpvnet shell-execute https://mpv.io", "Ctrl+Home"));
        Add(b, new (_("Help"), _("Website mpv.net"), "script-message-to mpvnet shell-execute https://github.com/mpvnet-player/mpv.net"));
        Add(b, new (_("Help"), "-"));
        Add(b, new (_("Help"), _("Manual mpv"), "script-message-to mpvnet shell-execute https://mpv.io/manual/stable", "Ctrl+F1"));
        Add(b, new (_("Help"), _("Manual mpv.net"), "script-message-to mpvnet shell-execute https://github.com/mpvnet-player/mpv.net/blob/main/docs/manual.md", "Ctrl+F2"));
        Add(b, new (_("Help"), "-"));
        Add(b, new (_("Help"), _("awesome-mpv"), "script-message-to mpvnet shell-execute https://github.com/stax76/awesome-mpv", "Ctrl+a"));
        Add(b, new (_("Help"), _("About mpv.net"), "script-message-to mpvnet show-about"));

        Add(b, new ("", "", "quit", "q", _("Exit")));
        Add(b, new ("", "", "script-message-to mpvnet show-menu", "MBTN_Right", _("Show Menu")));
        Add(b, new ("", "", "script-message-to mpvnet play-pause", "Play", _("Play/Pause")));
        Add(b, new ("", "", "script-message-to mpvnet play-pause", "Pause", _("Play/Pause")));
        Add(b, new ("", "", "script-message-to mpvnet play-pause", "PlayPause", _("Play/Pause")));
        Add(b, new ("", "", "script-message-to mpvnet play-pause", "MBTN_Mid", _("Play/Pause")));
        Add(b, new ("", "", "stop", "Stop", _("Stop")));
        Add(b, new ("", "", "seek  60", "Forward", _("Forward")));
        Add(b, new ("", "", "seek -60", "Rewind", _("Backward")));
        Add(b, new ("", "", "add volume 2", "Wheel_Up", _("Volume Up")));
        Add(b, new ("", "", "add volume -2", "Wheel_Down", _("Volume Down")));
        Add(b, new ("", "", "add volume 2", "Wheel_Right", _("Volume Up")));
        Add(b, new ("", "", "add volume -2", "Wheel_Left", _("Volume Down")));
        Add(b, new ("", "", "playlist-prev", "Prev", _("Previous File")));
        Add(b, new ("", "", "playlist-next", "Next", _("Next File")));
        Add(b, new ("", "", "playlist-prev", "MBTN_Back", _("Previous File")));
        Add(b, new ("", "", "playlist-next", "MBTN_Forward", _("Next File")));
        Add(b, new ("", "", "playlist-prev", "<", _("Previous File")));
        Add(b, new ("", "", "playlist-next", ">", _("Next File")));
        Add(b, new ("", "", "ignore", "MBTN_Left", _("Ignore left mouse butten")));
        Add(b, new ("", "", "cycle fullscreen", "f", _("Fullscreen")));
        Add(b, new ("", "", "cycle fullscreen", "MBTN_Left_DBL", _("Fullscreen")));
        Add(b, new ("", "", "no-osd seek  1 exact", "Shift+Right", _("Seek Forward")));
        Add(b, new ("", "", "no-osd seek -1 exact", "Shift+Left", _("Seek Backward")));
        Add(b, new ("", "", "no-osd seek  5 exact", "Shift+Up", _("Seek Forward")));
        Add(b, new ("", "", "no-osd seek -5 exact", "Shift+Down", _("Seek Backward")));
        Add(b, new ("", "", "revert-seek", "Shift+BS", _("Undo previous (or marked) seek")));
        Add(b, new ("", "", "revert-seek mark", "Shift+Ctrl+BS", _("Mark position for revert-seek")));
        Add(b, new ("", "", "no-osd sub-seek -1", "Ctrl+Shift+Left", _("Seek to previous subtitle")));
        Add(b, new ("", "", "no-osd sub-seek  1", "Ctrl+Shift+Right", _("Seek to next subtitle")));
        Add(b, new ("", "", "no-osd seek  5", "Ctrl+Wheel_Up", _("Seek Forward")));
        Add(b, new ("", "", "no-osd seek -5", "Ctrl+Wheel_Down", _("Seek Backward")));
        Add(b, new ("", "", "quit", "Power", _("Exit")));
        Add(b, new ("", "", "script-binding select/select-playlist", "g-p", _("Playlist")));
        Add(b, new ("", "", "script-binding select/select-sid", "g-s", _("Subtitles")));
        Add(b, new ("", "", "script-binding select/select-secondary-sid", "g-S", _("Secondary Subtitles")));
        Add(b, new ("", "", "script-binding select/select-aid", "g-a", _("Audio Tracks")));
        Add(b, new ("", "", "script-binding select/select-vid", "g-v", _("Video Tracks")));
        Add(b, new ("", "", "script-binding select/select-track", "g-t", _("Tracks")));
        Add(b, new ("", "", "script-binding select/select-chapter", "g-c", _("Chapters")));
        Add(b, new ("", "", "script-binding select/select-edition", "g-e", _("Editions")));
        Add(b, new ("", "", "script-binding select/select-subtitle-line", "g-l", _("Subtitle Lines")));
        Add(b, new ("", "", "script-binding select/select-audio-device", "g-d", _("Audio Devices")));
        Add(b, new ("", "", "script-binding select/select-watch-history", "g-h", _("Watch History")));
        Add(b, new ("", "", "script-binding select/select-watch-later", "g-w", _("Watch Later")));
        Add(b, new ("", "", "script-binding select/select-binding", "g-b", _("Bindings")));
        Add(b, new ("", "", "script-binding select/show-properties", "g-r", _("Properties")));
        Add(b, new ("", "", "script-binding select/menu", "g-m", _("On-Screen Menu")));
        Add(b, new ("", "", "script-binding select/menu", "MENU", _("On-Screen Menu")));
        Add(b, new ("", "", "script-binding select/menu", "Ctrl+p", _("On-Screen Menu")));

        return b;

        static void Add(List<Binding> bindings, Binding b) => bindings.Add(b);
    }

    public static string ConvertToString(List<Binding> bindings)
    {
        StringBuilder sb = new StringBuilder();
        
        foreach (Binding binding in bindings)
        {
            if (binding.IsEmpty())
            {
                sb.AppendLine();
                continue;
            }

            if (binding.Comment != "" &&
                binding.Command == "" &&
                binding.Input == "" &&
                !binding.IsMenu)
            {
                sb.AppendLine("#" + binding.Comment.Trim());
                continue;
            }

            string command = binding.Command.Trim();
            string input = binding.Input.Trim();
            input = input == "" ? "_" : input;
            string line = input.PadRight(10) + "  ";
            line += command == "" ? "ignore" : command;

            string comment;

            if (binding.IsMenu)
            {
                if (binding.IsShortMenuSyntax)
                    comment = "! " + binding.Comment.Trim();
                else
                    comment = "menu: " + binding.Comment.Trim();
            }
            else if (binding.IsCustomMenu)
                comment = "custom-menu: " + binding.Comment.Trim();
            else
                comment = binding.Comment.Trim();

            if (comment != "")
            {
                if (comment.StartsWith("menu: ") ||
                    comment.StartsWith("custom-menu: ") ||
                    comment.StartsWith("! "))

                    comment = "  #" + comment;
                else
                    comment = "  # " + comment;

                line = line.PadRight(40) + comment;
            }

            sb.AppendLine(line);
        }

        return sb.ToString().TrimEnd() + BR;
    }

    public static List<Binding> Parse(string content)
    {
        var bindings = new List<Binding>();

        if (string.IsNullOrEmpty(content))
            return bindings;

        if (content.Contains('\t'))
            content = content.Replace('\t', ' ');

        foreach (string it in content.Split('\n'))
        {
            string line = it.Trim();

            Binding binding = new Binding();

            if (line == "")
            {
                bindings.Add(binding);
                continue;
            }

            if (line.StartsWith("#"))
            {
                binding.Comment = line[1..].Trim();
                bindings.Add(binding);
                continue;
            }

            binding.Input = line[..line.IndexOf(" ")];

            if (binding.Input == "_")
                binding.Input = "";

            if (binding.Input.Contains("CTRL+"))
                binding.Input = binding.Input.Replace("CTRL+", "Ctrl+");
            if (binding.Input.Contains("ctrl+"))
                binding.Input = binding.Input.Replace("ctrl+", "Ctrl+");

            if (binding.Input.Contains("SHIFT+"))
                binding.Input = binding.Input.Replace("SHIFT+", "Shift+");
            if (binding.Input.Contains("shift+"))
                binding.Input = binding.Input.Replace("shift+", "Shift+");

            if (binding.Input.Contains("ALT+"))
                binding.Input = binding.Input.Replace("ALT+", "Alt+");
            if (binding.Input.Contains("alt+"))
                binding.Input = binding.Input.Replace("alt+", "Alt+");

            line = line[(line.IndexOf(" ") + 1)..];

            if (line.Contains(App.MenuSyntax))
            {
                binding.Comment = line[(line.IndexOf(App.MenuSyntax) + App.MenuSyntax.Length)..].Trim();
                binding.IsMenu = true;
                line = line[..line.IndexOf(App.MenuSyntax)];
            }
            else if (line.Contains("#custom-menu:"))
            {
                binding.Comment = line[(line.IndexOf("#custom-menu:") + 13)..].Trim();
                binding.IsCustomMenu = true;
                line = line[..line.IndexOf("#custom-menu:")];
            }
            else if (line.Contains('#'))
            {
                binding.Comment = line[(line.IndexOf("#") + 1)..].Trim();
                line = line[..line.IndexOf("#")];
            }

            binding.Command = line.Trim();

            if (binding.Command.ToLower() == "ignore")
                binding.Command = "";

            bindings.Add(binding);
        }
        return bindings;
    }

    public static List<Binding> GetReducedBindings(List<Binding> bindings)
    {
        var defaultBindings = GetDefaults();
        var removedBindings = new List<Binding>();

        foreach (Binding defaultBinding in defaultBindings)
        {
            foreach (Binding binding in bindings)
            {
                if (defaultBinding.Input == binding.Input &&
                    defaultBinding.Command == binding.Command &&
                    defaultBinding.Comment == binding.Comment)
                {
                    removedBindings.Add(binding);
                }
            }
        }

        foreach (Binding binding in bindings.ToArray())
            if (removedBindings.Contains(binding))
                bindings.Remove(binding);

        return bindings.ToList();
    }

    public static List<Binding> GetEditorBindings(string content)
    {
        var defaults = GetDefaults();
        var conf = Parse(content);
        var removed = new List<Binding>();

        foreach (Binding defaultBinding in defaults)
        {
            foreach (Binding confBinding in conf)
            {
                if (defaultBinding.Command == confBinding.Command &&
                    defaultBinding.Comment == confBinding.Comment)
                {
                    defaultBinding.Input = confBinding.Input;
                    removed.Add(confBinding);
                }
            }
        }

        foreach (Binding binding in removed)
            conf.Remove(binding);

        defaults.AddRange(conf);
        return defaults;
    }

    public static Dictionary<string, Binding> GetActiveBindings(List<Binding> bindings)
    {
        Dictionary<string, Binding> ret = new();

        foreach (Binding binding in bindings)
        {
            if (binding.Input == "" || binding.Command == "")
                continue;

            ret[binding.Input] = binding;
        }

        return ret;
    }

    public static string GetBindingsForCommand(Dictionary<string, Binding> activeBindings, string command)
    {
        List<string> keys = new();
        int charCount = 0;

        foreach (var it in activeBindings)
        {
            if (it.Value.Command != command)
                continue;

            Binding binding = it.Value;

            if (!keys.Contains(binding.Input) && (charCount + binding.Input.Length) < 15)
            {
                keys.Add(binding.Input);
                charCount += binding.Input.Length;
            }
        }

        return string.Join(", ", keys);
    }
}
