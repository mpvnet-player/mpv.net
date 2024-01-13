
using System.Text;

namespace MpvNet;

public static class InputHelp
{
    public static List<Binding> GetDefaults()
    {
        List<Binding> bindings = new List<Binding>()
            {
                new (_("File"), _("Open Files..."), "script-message-to mpvnet open-files", "o"),
                new (_("File"), _("Open URL or file from clipboard"), "script-message-to mpvnet open-clipboard", "Ctrl+v"),
                new (_("File"), _("Open DVD/Blu-ray Drive/Folder..."), "script-message-to mpvnet open-optical-media"),
                new (_("File"), "-"),
                new (_("File"), _("Add external audio files..."), "script-message-to mpvnet load-audio", "Alt+a"),
                new (_("File"), _("Add external subtitle files..."), "script-message-to mpvnet load-sub", "Alt+s"),
                new (_("File"), "-"),
                new (_("File"), _("Add files to playlist..."), "script-message-to mpvnet open-files append"),
                new (_("File"), _("Add files/URLs to playlist from clipboard"), "script-message-to mpvnet open-clipboard append", "Ctrl+Shift+v"),
                new (_("File"), "-"),
                new (_("File"), _("Recent Files")),
                new (_("File"), _("Exit"), "quit", "Esc"),

                new (_("Playback"), _("Play/Pause"), "script-message-to mpvnet play-pause", "Space"),
                new (_("Playback"), _("Stop"), "stop", "Ctrl+s"),

                new (_("Navigate"), _("Previous File"), "playlist-prev", "F11"),
                new (_("Navigate"), _("Next File"), "playlist-next", "F12"),
                new (_("Navigate"), "-"),
                new (_("Navigate"), _("First File"), "script-message-to mpvnet playlist-first", "Home"),
                new (_("Navigate"), _("Last File"), "script-message-to mpvnet playlist-last", "End"),

                new (_("Navigate"), "-"),
                new (_("Navigate"), _("Next Chapter"), "add chapter 1", "PGUP"),
                new (_("Navigate"), _("Previous Chapter"), "add chapter -1", "PGDWN"),
                new (_("Navigate"), "-"),
                new (_("Navigate"), _("Jump To Next Frame"), "frame-step", "."),
                new (_("Navigate"), _("Jump To Previous Frame"), "frame-back-step", ","),
                new (_("Navigate"), "-"),
                new (_("Navigate"), _("Jump 5 sec forward"), "seek 5", "Right"),
                new (_("Navigate"), _("Jump 5 sec backward"), "seek -5", "Left"),
                new (_("Navigate"), "-"),
                new (_("Navigate"), _("Jump 30 sec forward"), "seek 30", "Up"),
                new (_("Navigate"), _("Jump 30 sec backward"), "seek -30", "Down"),
                new (_("Navigate"), "-"),
                new (_("Navigate"), _("Jump 5 min forward"), "seek 300", "Ctrl+Right"),
                new (_("Navigate"), _("Jump 5 min backward"), "seek -300", "Ctrl+Left"),
                new (_("Navigate"), "-"),
                new (_("Navigate"), _("Title")),
                new (_("Navigate"), _("Chapter")),

                new (_("Pan & Scan"), _("Decrease Size"), "add video-zoom -0.1", "Ctrl+-"),
                new (_("Pan & Scan"), _("Increase Size"), "add video-zoom 0.1", "Ctrl++"),
                new (_("Pan & Scan"), "-"),
                new (_("Pan & Scan"), _("Move Left"), "add video-pan-x -0.01", "Ctrl+KP4"),
                new (_("Pan & Scan"), _("Move Right"), "add video-pan-x 0.01", "Ctrl+KP6"),
                new (_("Pan & Scan"), "-"),
                new (_("Pan & Scan"), _("Move Up"), "add video-pan-y -0.01", "Ctrl+KP8"),
                new (_("Pan & Scan"), _("Move Down"), "add video-pan-y 0.01", "Ctrl+KP2"),
                new (_("Pan & Scan"), "-"),
                new (_("Pan & Scan"), _("Decrease Height"), "add panscan -0.1", "w"),
                new (_("Pan & Scan"), _("Increase Height"), "add panscan 0.1", "W"),
                new (_("Pan & Scan"), "-"),
                new (_("Pan & Scan"), _("Reset"), "set video-zoom 0; set video-pan-x 0; set video-pan-y 0", "Ctrl+BS"),

                new (_("Video"), _("Decrease Contrast"), "add contrast -1", "Ctrl+1"),
                new (_("Video"), _("Increase Contrast"), "add contrast 1", "Ctrl+2"),
                new (_("Video"), "-"),
                new (_("Video"), _("Decrease Brightness"), "add brightness -1", "Ctrl+3"),
                new (_("Video"), _("Increase Brightness"), "add brightness 1", "Ctrl+4"),
                new (_("Video"), "-"),
                new (_("Video"), _("Decrease Gamma"), "add gamma -1", "Ctrl+5"),
                new (_("Video"), _("Increase Gamma"), "add gamma 1", "Ctrl+6"),
                new (_("Video"), "-"),
                new (_("Video"), _("Decrease Saturation"), "add saturation -1", "Ctrl+7"),
                new (_("Video"), _("Increase Saturation"), "add saturation 1", "Ctrl+8"),
                new (_("Video"), "-"),
                new (_("Video"), _("Take Screenshot"), "async screenshot", "s"),
                new (_("Video"), _("Take Screenshot without subtitles"), "async screenshot video", "S"),
                new (_("Video"), _("Toggle Deinterlace"), "cycle deinterlace", "d"),
                new (_("Video"), _("Change Aspect Ratio"), "cycle-values video-aspect-override 16:9 4:3 2.35:1 -1", "a"),
                new (_("Video"), _("Rotate Video"), "cycle-values video-rotate 90 180 270 0", "Ctrl+r"),

                new (_("Audio"), _("Audio Device")),
                new (_("Audio"), _("Next Track"), "script-message-to mpvnet cycle-audio", "KP7"),
                new (_("Audio"), "-"),
                new (_("Audio"), _("Delay +0.1"), "add audio-delay 0.1", "Ctrl+d"),
                new (_("Audio"), _("Delay -0.1"), "add audio-delay -0.1", "Ctrl+D"),

                new (_("Subtitle"), _("Next Track"), "script-message-to mpvnet cycle-subtitles", "KP8"),
                new (_("Subtitle"), _("Toggle Visibility"), "cycle sub-visibility", "v"),
                new (_("Subtitle"), "-"),
                new (_("Subtitle"), _("Delay -0.1"), "add sub-delay -0.1", "z"),
                new (_("Subtitle"), _("Delay +0.1"), "add sub-delay 0.1", "Z"),
                new (_("Subtitle"), "-"),
                new (_("Subtitle"), _("Move Up"), "add sub-pos -1", "r"),
                new (_("Subtitle"), _("Move Down"), "add sub-pos 1", "R"),
                new (_("Subtitle"), "-"),
                new (_("Subtitle"), _("Decrease Font Size"), "add sub-scale -0.1", "F"),
                new (_("Subtitle"), _("Increase Font Size"), "add sub-scale 0.1", "G"),
                new (_("Subtitle"), "-"),
                new (_("Subtitle") + " > " + _("More"), _("Toggle overriding SSA/ASS styles with normal styles"), "cycle-values sub-ass-override force no", "u"),

                new ("", _("Track")),

                new (_("Volume"), _p("Volume", "Up"), "add volume 2", "+"),
                new (_("Volume"), _p("Volume", "Down"), "add volume -2", "-"),
                new (_("Volume"), "-"),
                new (_("Volume"), _("Mute"), "cycle mute", "m"),

                new (_("Speed"), _("-10%"), "multiply speed 1/1.1", "["),
                new (_("Speed"), _("+10%"), "multiply speed 1.1", "]"),
                new (_("Speed"), "-"),
                new (_("Speed"), _("Half"), "multiply speed 0.5", "{"),
                new (_("Speed"), _("Double"), "multiply speed 2.0", "}"),
                new (_("Speed"), "-"),
                new (_("Speed"), _("Reset"), "set speed 1", "BS"),

                new (_("View"), _("Show Playlist"), "script-message-to mpvnet show-playlist", "F8"),
                new (_("View"), _("Toggle Statistics"), "script-binding stats/display-stats-toggle", "t"),
                new (_("View"), _("Toggle OSC Visibility"), "script-binding osc/visibility", "Del"),
                new (_("View"), _("Show Media Info On-Screen"),   "script-message-to mpvnet show-media-info osd", "i"),
                new (_("View"), _("Show Media Info Message Box"), "script-message-to mpvnet show-media-info msgbox", "Ctrl+m"),
                new (_("View"), _("Show Progress"), "show-progress", "p"),
                new (_("View") + " > " + _("More"), _("Show Console"), "script-binding console/enable", "`"),
                new (_("View") + " > " + _("More"), _("Show Audio Devices"), "script-message-to mpvnet show-audio-devices"),
                new (_("View") + " > " + _("More"), _("Show Commands"), "script-message-to mpvnet show-commands", "F2"),
                new (_("View") + " > " + _("More"), _("Show Bindings"), "script-message-to mpvnet show-bindings"),
                new (_("View") + " > " + _("More"), _("Show Properties"), "script-message-to mpvnet show-properties", "F3"),
                new (_("View") + " > " + _("More"), _("Show Keys"), "script-message-to mpvnet show-keys", "Alt+k"),
                new (_("View") + " > " + _("More"), _("Show Protocols"), "script-message-to mpvnet show-protocols", "Alt+p"),
                new (_("View") + " > " + _("More"), _("Show Decoders"), "script-message-to mpvnet show-decoders", "Alt+d"),
                new (_("View") + " > " + _("More"), _("Show Demuxers"), "script-message-to mpvnet show-demuxers"),

                new (_("Window"), _("Fullscreen"), "cycle fullscreen", "Enter"),
                new (_("Window") + " > " + _("Zoom"), _("Enlarge"), "script-message-to mpvnet scale-window 1.2", "Alt++"),
                new (_("Window") + " > " + _("Zoom"), _("Shrink"), "script-message-to mpvnet scale-window 0.8", "Alt+-"),
                new (_("Window") + " > " + _("Zoom"), "-"),
                new (_("Window") + " > " + _("Zoom"), _("50 %"), "script-message-to mpvnet window-scale 0.5", "Alt+0"),
                new (_("Window") + " > " + _("Zoom"), _("100 %"), "script-message-to mpvnet window-scale 1.0", "Alt+1"),
                new (_("Window") + " > " + _("Zoom"), _("200 %"), "script-message-to mpvnet window-scale 2.0", "Alt+2"),
                new (_("Window") + " > " + _("Zoom"), _("300 %"), "script-message-to mpvnet window-scale 3.0", "Alt+3"),
                new (_("Window") + " > " + _("Move"), _p("Move", "Left"), "script-message-to mpvnet move-window left", "Alt+Left"),
                new (_("Window") + " > " + _("Move"), _p("Move", "Right"), "script-message-to mpvnet move-window right", "Alt+Right"),
                new (_("Window") + " > " + _("Move"), _p("Move", "Up"), "script-message-to mpvnet move-window top", "Alt+Up"),
                new (_("Window") + " > " + _("Move"), _p("Move", "Down"), "script-message-to mpvnet move-window bottom", "Alt+Down"),
                new (_("Window") + " > " + _("Move"), _p("Move", "Center"), "script-message-to mpvnet move-window center", "Alt+BS"),
                new (_("Window"), _("Toggle Border"), "cycle border", "b"),
                new (_("Window"), _("Toggle On Top"), "cycle ontop", "Ctrl+t"),

                new ("", _("Profile")),
                
                new (_("Settings"), _("Show Config Editor"), "script-message-to mpvnet show-conf-editor", "Ctrl+,"),
                new (_("Settings"), _("Show Input Editor"), "script-message-to mpvnet show-input-editor", "Ctrl+i"),
                new (_("Settings"), "-"),
                new (_("Settings"), _("Edit mpv.conf"), "script-message-to mpvnet edit-conf-file mpv.conf", "c"),
                new (_("Settings"), _("Edit input.conf"), "script-message-to mpvnet edit-conf-file input.conf", "k"),
                new (_("Settings"), "-"),
                new (_("Settings"), _("Open Config Folder"), "script-message-to mpvnet open-conf-folder", "Ctrl+f"),
                new (_("Settings") + " > " + _("Setup"), _("Register video file associations"), "script-message-to mpvnet reg-file-assoc video"),
                new (_("Settings") + " > " + _("Setup"), _("Register audio file associations"), "script-message-to mpvnet reg-file-assoc audio"),
                new (_("Settings") + " > " + _("Setup"), _("Register image file associations"), "script-message-to mpvnet reg-file-assoc image"),
                new (_("Settings") + " > " + _("Setup"), _("Unregister file associations"), "script-message-to mpvnet reg-file-assoc unreg"),
                new (_("Settings") + " > " + _("Setup"), "-"),
                new (_("Settings") + " > " + _("Setup"), _("Add mpv.net to Path environment variable"), "script-message-to mpvnet add-to-path"),
                new (_("Settings") + " > " + _("Setup"), _("Remove mpv.net from Path environment variable"), "script-message-to mpvnet remove-from-path"),
          
                new (_("Tools"), _("Set/clear A-B loop points"), "ab-loop", "l"),
                new (_("Tools"), _("Toggle infinite file looping"), "cycle-values loop-file inf no", "L"),
                new (_("Tools"), _("Shuffle Playlist"), "playlist-shuffle"),
                new (_("Tools"), _("Toggle Hardware Decoding"), "cycle-values hwdec auto no", "Ctrl+h"),
                new (_("Tools"), _("Exit Watch Later"), "quit-watch-later", "Q"),
         
                new ("", _("Custom")),

                new (_("Help"), _("Website mpv"), "script-message-to mpvnet shell-execute https://mpv.io", "Ctrl+Home"),
                new (_("Help"), _("Website mpv.net"), "script-message-to mpvnet shell-execute https://github.com/mpvnet-player/mpv.net"),
                new (_("Help"), "-"),
                new (_("Help"), _("Manual mpv"), "script-message-to mpvnet shell-execute https://mpv.io/manual/stable", "Ctrl+F1"),
                new (_("Help"), _("Manual mpv.net"), "script-message-to mpvnet shell-execute https://github.com/mpvnet-player/mpv.net/blob/main/docs/manual.md", "Ctrl+F2"),
                new (_("Help"), "-"),
                new (_("Help"), _("awesome-mpv"), "script-message-to mpvnet shell-execute https://github.com/stax76/awesome-mpv", "Ctrl+a"),
                new (_("Help"), _("About mpv.net"), "script-message-to mpvnet show-about"),

                new ("", "", "quit", "q", _("Exit")),
                new ("", "", "script-message-to mpvnet show-menu", "MBTN_Right", _("Show Menu")),
                new ("", "", "script-message-to mpvnet play-pause", "Play", _("Play/Pause")),
                new ("", "", "script-message-to mpvnet play-pause", "Pause", _("Play/Pause")),
                new ("", "", "script-message-to mpvnet play-pause", "PlayPause", _("Play/Pause")),
                new ("", "", "script-message-to mpvnet play-pause", "MBTN_Mid", _("Play/Pause")),
                new ("", "", "stop", "Stop", _("Stop")),
                new ("", "", "seek  60", "Forward", _("Forward")),
                new ("", "", "seek -60", "Rewind", _("Backward")),
                new ("", "", "add volume 2", "Wheel_Up", _("Volume Up")),
                new ("", "", "add volume -2", "Wheel_Down", _("Volume Down")),
                new ("", "", "add volume 2", "Wheel_Right", _("Volume Up")),
                new ("", "", "add volume -2", "Wheel_Left", _("Volume Down")),
                new ("", "", "playlist-prev", "Prev", _("Previous File")),
                new ("", "", "playlist-next", "Next", _("Next File")),
                new ("", "", "playlist-prev", "MBTN_Back", _("Previous File")),
                new ("", "", "playlist-next", "MBTN_Forward", _("Next File")),
                new ("", "", "playlist-prev", "<", _("Previous File")),
                new ("", "", "playlist-next", ">", _("Next File")),
                new ("", "", "ignore", "MBTN_Left", _("Ignore left mouse butten")),
                new ("", "", "cycle fullscreen", "f", _("Fullscreen")),
                new ("", "", "cycle fullscreen", "MBTN_Left_DBL", _("Fullscreen")),
                new ("", "", "no-osd seek  1 exact", "Shift+Right", _("Seek Forward")),
                new ("", "", "no-osd seek -1 exact", "Shift+Left", _("Seek Backward")),
                new ("", "", "no-osd seek  5 exact", "Shift+Up", _("Seek Forward")),
                new ("", "", "no-osd seek -5 exact", "Shift+Down", _("Seek Backward")),
                new ("", "", "revert-seek", "Shift+BS", _("Undo previous (or marked) seek")),
                new ("", "", "revert-seek mark", "Shift+Ctrl+BS", _("Mark position for revert-seek")),
                new ("", "", "no-osd sub-seek -1", "Ctrl+Shift+Left", _("Seek to previous subtitle")),
                new ("", "", "no-osd sub-seek  1", "Ctrl+Shift+Right", _("Seek to next subtitle")),
                new ("", "", "no-osd seek  5", "Ctrl+Wheel_Up", _("Seek Forward")),
                new ("", "", "no-osd seek -5", "Ctrl+Wheel_Down", _("Seek Backward")),
                new ("", "", "quit", "Power", _("Exit")),

                //new (_("Command Palette"), _("Commands"), "script-message-to mpvnet show-command-palette", "F1"),
            };

        return bindings;
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

    // only used by dead command palette
    public static List<Binding> GetBindingsFromContent(string content)
    {
        var bindings = new List<Binding>();

        if (!string.IsNullOrEmpty(content))
        {
            foreach (string line in content.Split('\r', '\n'))
            {
                string value = line.Trim();

                if (value.StartsWith("#"))
                    continue;

                if (!value.Contains(' '))
                    continue;

                Binding binding = new Binding();
                binding.Input = value[..value.IndexOf(" ")];

                if (binding.Input == "_")
                    binding.Input = "";

                value = value[(value.IndexOf(" ") + 1)..];

                if (value.Contains(App.MenuSyntax))
                {
                    binding.Comment = value[(value.IndexOf(App.MenuSyntax) + App.MenuSyntax.Length)..].Trim();
                    value = value[..value.IndexOf(App.MenuSyntax)];

                    if (binding.Comment.Contains(';'))
                        binding.Comment = binding.Comment[(binding.Comment.IndexOf(";") + 1)..].Trim();
                }

                binding.Command = value.Trim();

                if (binding.Command == "")
                    continue;

                if (binding.Command.ToLower() == "ignore")
                    binding.Command = "";

                bindings.Add(binding);
            }
        }
        return bindings;
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
