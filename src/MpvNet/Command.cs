
using System.Globalization;
using System.Text;
using System.Text.Json;
using MpvNet.Help;

namespace MpvNet;

public class Command
{
    Dictionary<string, Action<IList<string>>>? _commands;

    public static Command Current { get; } = new();

    public Dictionary<string, Action<IList<string>>> Commands => _commands ??= new()
    {
        ["open-conf-folder"] = args => ProcessHelp.ShellExecute(Player.ConfigFolder),
        ["play-pause"] = PlayPause,
        ["shell-execute"] = args => ProcessHelp.ShellExecute(args[0]),
        ["show-text"] = args => ShowText(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2])),
        ["show-commands"] = args => ShowCommands(),
        ["cycle-audio"] = args => CycleAudio(),
        ["cycle-subtitles"] = args => CycleSubtitles(),
        ["playlist-first"] = args => PlaylistFirst(),
        ["playlist-last"] = args => PlaylistLast(),


        // deprecated
        ["playlist-add"] = args => PlaylistAdd(Convert.ToInt32(args[0])), // deprecated
        ["show-progress"] = args => ShowProgress(), // deprecated
        ["playlist-random"] = args => PlaylistRandom(), // deprecated
    };

    public string FormatTime(double value) => ((int)value).ToString("00");

    public static void PlayPause(IList<string> args)
    {
        int count = Player.GetPropertyInt("playlist-count");

        if (count > 0)
            Player.Command("cycle pause");
        else if (App.Settings.RecentFiles.Count > 0)
        {
            foreach (string i in App.Settings.RecentFiles)
            {
                if (i.Contains("://") || File.Exists(i))
                {
                    Player.LoadFiles(new[] { i }, true, false);
                    break;
                }
            }
        }
    }

    public static void ShowCommands()
    {
        string json = Core.GetPropertyString("command-list");
        var enumerator = JsonDocument.Parse(json).RootElement.EnumerateArray();
        var commands = enumerator.OrderBy(it => it.GetProperty("name").GetString());
        StringBuilder sb = new StringBuilder();

        foreach (var cmd in commands)
        {
            sb.AppendLine();
            sb.AppendLine(cmd.GetProperty("name").GetString());

            foreach (var args in cmd.GetProperty("args").EnumerateArray())
            {
                string value = args.GetProperty("name").GetString() + " <" +
                    args.GetProperty("type").GetString()!.ToLower() + ">";

                if (args.GetProperty("optional").GetBoolean())
                    value = "[" + value + "]";

                sb.AppendLine("    " + value);
            }
        }

        string header = BR +
            "https://mpv.io/manual/master/#list-of-input-commands" + BR2 +
            "https://github.com/stax76/mpv-scripts#command_palette" + BR;

        ShowTextWithEditor("Input Commands", header + sb.ToString());
    }

    public static void ShowText(string text, int duration = 0, int fontSize = 0)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (duration == 0)
            duration = Player.GetPropertyInt("osd-duration");

        if (fontSize == 0)
            fontSize = Player.GetPropertyInt("osd-font-size");

        Player.Command("show-text \"${osd-ass-cc/0}{\\\\fs" + fontSize +
            "}${osd-ass-cc/1}" + text + "\" " + duration);
    }

    public static void ShowTextWithEditor(string name, string text)
    {
        string file = Path.Combine(Path.GetTempPath(), name + ".txt");
        App.TempFiles.Add(file);
        File.WriteAllText(file, BR + text.Trim() + BR);
        ProcessHelp.ShellExecute(file);
    }

    public static void CycleAudio()
    {
        Player.UpdateExternalTracks();

        lock (Player.MediaTracksLock)
        {
            MediaTrack[] tracks = Player.MediaTracks.Where(track => track.Type == "a").ToArray();

            if (tracks.Length < 1)
            {
                Player.CommandV("show-text", "No audio tracks");
                return;
            }

            int aid = Player.GetPropertyInt("aid");

            if (tracks.Length > 1)
            {
                if (++aid > tracks.Length)
                    aid = 1;

                Player.SetPropertyInt("aid", aid);
            }

            Player.CommandV("show-text", aid + "/" + tracks.Length + ": " + tracks[aid - 1].Text[3..], "5000");
        }
    }

    public static void CycleSubtitles()
    {
        Player.UpdateExternalTracks();

        lock (Player.MediaTracksLock)
        {
            MediaTrack[] tracks = Player.MediaTracks.Where(track => track.Type == "s").ToArray();

            if (tracks.Length < 1)
            {
                Player.CommandV("show-text", "No subtitles");
                return;
            }

            int sid = Player.GetPropertyInt("sid");

            if (tracks.Length > 1)
            {
                if (++sid > tracks.Length)
                    sid = 0;

                Player.SetPropertyInt("sid", sid);
            }

            if (sid == 0)
                Player.CommandV("show-text", "No subtitle");
            else
                Player.CommandV("show-text", sid + "/" + tracks.Length + ": " + tracks[sid - 1].Text[3..], "5000");
        }
    }

    // deprecated
    public static void PlaylistAdd(int value)
    {
        int pos = Player.PlaylistPos;
        int count = Player.GetPropertyInt("playlist-count");

        if (count < 2)
            return;

        pos += value;

        if (pos < 0)
            pos = count - 1;

        if (pos > count - 1)
            pos = 0;

        Player.SetPropertyInt("playlist-pos", pos);
    }

    public static void PlaylistFirst()
    {
        if (Player.PlaylistPos != 0)
            Player.SetPropertyInt("playlist-pos", 0);
    }

    public static void PlaylistLast()
    {
        int count = Player.GetPropertyInt("playlist-count");

        if (Player.PlaylistPos < count - 1)
            Player.SetPropertyInt("playlist-pos", count - 1);
    }

    // deprecated
    public static void PlaylistRandom()
    {
        int count = Player.GetPropertyInt("playlist-count");
        Player.SetPropertyInt("playlist-pos", new Random().Next(count));
    }

    // deprecated
    public void ShowProgress()
    {
        TimeSpan position = TimeSpan.FromSeconds(Player.GetPropertyDouble("time-pos"));
        TimeSpan duration = TimeSpan.FromSeconds(Player.GetPropertyDouble("duration"));

        string text = FormatTime(position.TotalMinutes) + ":" +
                      FormatTime(position.Seconds) + " / " +
                      FormatTime(duration.TotalMinutes) + ":" +
                      FormatTime(duration.Seconds) + "    " +
                      DateTime.Now.ToString("H:mm dddd d MMMM", CultureInfo.InvariantCulture);

        Player.CommandV("show-text", text, "5000");
    }
}
