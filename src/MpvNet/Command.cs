
using System.Globalization;
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
        ["cycle-audio"] = args => CycleAudio(),
        ["cycle-subtitles"] = args => CycleSubtitles(),
        ["playlist-first"] = args => PlaylistFirst(),
        ["playlist-last"] = args => PlaylistLast(),


        // deprecated
        ["playlist-add"] = args => PlaylistAdd(Convert.ToInt32(args[0])), // deprecated
        ["show-progress"] = args => Player.Command("show-progress"), // deprecated
        ["playlist-random"] = args => PlaylistRandom(), // deprecated
    };

    string FormatTime(double value) => ((int)value).ToString("00");

    void PlayPause(IList<string> args)
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

    void CycleAudio()
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

    void CycleSubtitles()
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
    void PlaylistAdd(int value)
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

    void PlaylistFirst()
    {
        if (Player.PlaylistPos != 0)
            Player.SetPropertyInt("playlist-pos", 0);
    }

    void PlaylistLast()
    {
        int count = Player.GetPropertyInt("playlist-count");

        if (Player.PlaylistPos < count - 1)
            Player.SetPropertyInt("playlist-pos", count - 1);
    }

    // deprecated
    void PlaylistRandom()
    {
        int count = Player.GetPropertyInt("playlist-count");
        Player.SetPropertyInt("playlist-pos", new Random().Next(count));
    }

    // deprecated
    void ShowProgress()
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
