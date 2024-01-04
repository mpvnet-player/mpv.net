
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows;

using MpvNet.ExtensionMethod;
using MpvNet.Windows.WinForms;
using MpvNet.Windows.WPF.Views;
using MpvNet.Windows.WPF;
using MpvNet.Windows.WPF.MsgBox;
using MpvNet.Windows.Help;
using MpvNet.Help;
using System;

namespace MpvNet;

public class GuiCommand
{
    Dictionary<string, Action<IList<string>>>? _commands;

    public event Action<float>? ScaleWindow;
    public event Action<string>? MoveWindow;
    public event Action<float>? WindowScaleNet;
    public event Action? ShowMenu;

    public static GuiCommand Current { get; } = new();

    public Dictionary<string, Action<IList<string>>> Commands => _commands ??= new()
    {
        ["add-to-path"] = args => AddToPath(),
        ["edit-conf-file"] = EditCongFile,
        ["load-audio"] = LoadAudio,
        ["load-sub"] = LoadSubtitle,
        ["move-window"] = args => MoveWindow?.Invoke(args[0]),
        ["open-clipboard"] = OpenFromClipboard,
        ["open-files"] = OpenFiles,
        ["open-optical-media"] = Open_DVD_Or_BD_Folder,
        ["reg-file-assoc"] = RegisterFileAssociations,
        ["remove-from-path"] = args => RemoveFromPath(),
        ["scale-window"] = args => ScaleWindow?.Invoke(float.Parse(args[0], CultureInfo.InvariantCulture)),
        ["show-about"] = args => ShowDialog(typeof(AboutWindow)),
        ["show-audio-devices"] = args => Msg.ShowInfo(Player.GetPropertyOsdString("audio-device-list")),
        ["show-bindings"] = args => ShowBindings(),
        ["show-commands"] = args => ShowCommands(),
        ["show-conf-editor"] = args => ShowDialog(typeof(ConfWindow)),
        ["show-decoders"] = args => ShowDecoders(),
        ["show-demuxers"] = args => ShowDemuxers(),
        ["show-info"] = args => ShowMediaInfo(new[] { "osd" }),
        ["show-input-editor"] = args => ShowDialog(typeof(InputWindow)),
        ["show-keys"] = args => ShowKeys(),
        ["show-media-info"] = ShowMediaInfo,
        ["show-menu"] = args => ShowMenu?.Invoke(),
        ["show-profiles"] = args => Msg.ShowInfo(Player.GetProfiles()),
        ["show-properties"] = args => ShowProperties(),
        ["show-protocols"] = args => ShowProtocols(),
        ["window-scale"] = args => WindowScaleNet?.Invoke(float.Parse(args[0], CultureInfo.InvariantCulture)),


        // deprecated
        ["show-recent"] = args => ShowRemoved(), // deprecated
        ["show-playlist"] = args => ShowPlaylist(), // deprecated
        ["quick-bookmark"] = args => QuickBookmark(), // deprecated
        ["show-history"] = args => ShowHistory(), // deprecated
        ["show-command-palette"] = args => ShowCommandPalette(), // deprecated
        ["show-audio-tracks"] = args => ShowTracks(), // deprecated
        ["show-subtitle-tracks"] = args => ShowTracks(), // deprecated
    };

    void ShowDialog(Type winType)
    {
        Window? win = Activator.CreateInstance(winType) as Window;
        new WindowInteropHelper(win).Owner = MainForm.Instance!.Handle;
        win?.ShowDialog();
    }

    void LoadSubtitle(IList<string> args)
    {
        using var dialog = new OpenFileDialog();
        string path = Player.GetPropertyString("path");

        if (File.Exists(path))
            dialog.InitialDirectory = Path.GetDirectoryName(path);

        dialog.Multiselect = true;

        if (dialog.ShowDialog() == DialogResult.OK)
            foreach (string filename in dialog.FileNames)
                Player.CommandV("sub-add", filename);
    }

    void OpenFiles(IList<string> args)
    {
        bool append = false;

        foreach (string arg in args)
            if (arg == "append")
                append = true;

        using var dialog = new OpenFileDialog() { Multiselect = true };

        if (dialog.ShowDialog() == DialogResult.OK)
            Player.LoadFiles(dialog.FileNames, true, append);
    }

    void Open_DVD_Or_BD_Folder(IList<string> args)
    {
        var dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == DialogResult.OK)
            Player.LoadDiskFolder(dialog.SelectedPath);
    }

    void EditCongFile(IList<string> args)
    {
        string file = Player.ConfigFolder + args[0];

        if (File.Exists(file))
            ProcessHelp.ShellExecute(WinApiHelp.GetAppPathForExtension("txt"), "\"" + file + "\"");
    }

    void ShowTextWithEditor(string name, string text)
    {
        string file = Path.Combine(Path.GetTempPath(), name + ".txt");
        App.TempFiles.Add(file);
        File.WriteAllText(file, BR + text.Trim() + BR);
        ProcessHelp.ShellExecute(WinApiHelp.GetAppPathForExtension("txt"), "\"" + file + "\"");
    }

    void ShowCommands()
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

    void ShowProperties() =>
        ShowTextWithEditor("Properties", Core.GetPropertyString("property-list").Replace(",", BR));

    void ShowKeys() =>
        ShowTextWithEditor("Keys", Core.GetPropertyString("input-key-list").Replace(",", BR));

    void ShowProtocols() =>
        ShowTextWithEditor("Protocols", Core.GetPropertyString("protocol-list").Replace(",", BR));

    void ShowDecoders() =>
        ShowTextWithEditor("Decoders", Core.GetPropertyOsdString("decoder-list").Replace(",", BR));

    void ShowDemuxers() =>
        ShowTextWithEditor("Demuxers", Core.GetPropertyOsdString("demuxer-lavf-list").Replace(",", BR));

    void OpenFromClipboard(IList<string> args)
    {
        bool append = args.Count == 1 && args[0] == "append";

        if (System.Windows.Forms.Clipboard.ContainsFileDropList())
        {
            string[] files = System.Windows.Forms.Clipboard.GetFileDropList().Cast<string>().ToArray();
            Player.LoadFiles(files, false, append);

            if (append)
                Player.CommandV("show-text", _("Files/URLs were added to the playlist"));
        }
        else
        {
            string clipboard = System.Windows.Forms.Clipboard.GetText();
            List<string> files = new List<string>();

            foreach (string i in clipboard.Split(BR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                if (i.Contains("://") || File.Exists(i))
                    files.Add(i);

            if (files.Count == 0)
            {
                Terminal.WriteError(_("The clipboard does not contain a valid URL or file."));
                return;
            }

            Player.LoadFiles(files.ToArray(), false, append);

            if (append)
                Player.CommandV("show-text", _("Files/URLs were added to the playlist"));
        }
    }

    void LoadAudio(IList<string> args)
    {
        using var dialog = new OpenFileDialog();
        string path = Player.GetPropertyString("path");

        if (File.Exists(path))
            dialog.InitialDirectory = Path.GetDirectoryName(path);

        dialog.Multiselect = true;

        if (dialog.ShowDialog() == DialogResult.OK)
            foreach (string i in dialog.FileNames)
                Player.CommandV("audio-add", i);
    }

    void RegisterFileAssociations(IList<string> args)
    {
        string perceivedType = args[0];
        string[] extensions = Array.Empty<string>();

        switch (perceivedType)
        {
            case "video": extensions = FileTypes.Video; break;
            case "audio": extensions = FileTypes.Audio; break;
            case "image": extensions = FileTypes.Image; break;
        }

        try
        {
            using Process proc = new Process();
            proc.StartInfo.FileName = Environment.ProcessPath;
            proc.StartInfo.Arguments = "--register-file-associations " +
                perceivedType + " " + string.Join(" ", extensions);
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            proc.WaitForExit();

            if (proc.ExitCode == 0)
            {
                string msgRestart = _("File Explorer icons will refresh after process restart.");

                if (perceivedType == "unreg")
                    Msg.ShowInfo(_("File associations were successfully removed.") + BR2 + msgRestart);
                else
                    Msg.ShowInfo(_("File associations were successfully created.") + BR2 + msgRestart);
            }
            else
                Msg.ShowError(_("Error creating file associations."));
        }
        catch { }
    }

    void ShowMediaInfo(IList<string> args)
    {
        if (Player.PlaylistPos == -1)
            return;

        bool full = args.Contains("full");
        bool raw = args.Contains("raw");
        bool editor = args.Contains("editor");
        bool osd = args.Contains("osd") || args == null || args.Count == 0;

        long fileSize = 0;

        string text = "";
        string path = Player.GetPropertyString("path");

        if (File.Exists(path) && osd)
        {
            if (FileTypes.Audio.Contains(path.Ext()))
            {
                text = Player.GetPropertyOsdString("filtered-metadata");
                Player.CommandV("show-text", text, "5000");
                return;
            }
            else if (FileTypes.Image.Contains(path.Ext()))
            {
                fileSize = new FileInfo(path).Length;

                text = "Width: " + Player.GetPropertyInt("width") + "\n" +
                       "Height: " + Player.GetPropertyInt("height") + "\n" +
                       "Size: " + Convert.ToInt32(fileSize / 1024.0) + " KB\n" +
                       "Type: " + path.Ext().ToUpper();

                Player.CommandV("show-text", text, "5000");
                return;
            }
        }

        if (path.Contains("://"))
        {
            if (path.Contains("://"))
                path = Player.GetPropertyString("media-title");
            string videoFormat = Player.GetPropertyString("video-format").ToUpper();
            string audioCodec = Player.GetPropertyString("audio-codec-name").ToUpper();
            int width = Player.GetPropertyInt("video-params/w");
            int height = Player.GetPropertyInt("video-params/h");
            TimeSpan len = TimeSpan.FromSeconds(Player.GetPropertyDouble("duration"));
            text = path.FileName() + "\n";
            text += FormatTime(len.TotalMinutes) + ":" + FormatTime(len.Seconds) + "\n";
            if (fileSize > 0)
                text += Convert.ToInt32(fileSize / 1024.0 / 1024.0) + " MB\n";
            text += $"{width} x {height}\n";
            text += $"{videoFormat}\n{audioCodec}";
            Player.CommandV("show-text", text, "5000");
            return;
        }

        if (App.MediaInfo && !osd && File.Exists(path) && !path.Contains(@"\\.\pipe\"))
            using (MediaInfo mediaInfo = new MediaInfo(path))
                text = Regex.Replace(mediaInfo.GetSummary(full, raw), "Unique ID.+", "");
        else
        {
            Player.UpdateExternalTracks();
            text = "N: " + Player.GetPropertyString("filename") + BR;
            lock (Player.MediaTracksLock)
                foreach (MediaTrack track in Player.MediaTracks)
                    text += track.Text + BR;
        }

        text = text.TrimEx();

        if (editor)
            ShowTextWithEditor("media-info", text);
        else if (osd)
            Command.ShowText(text.Replace("\r", ""), 5000, 16);
        else
        {
            MessageBoxEx.SetFont("Consolas");
            Msg.ShowInfo(text);
            MessageBoxEx.SetFont("Segoe UI");
        }
    }

    string FormatTime(double value) => ((int)value).ToString("00");

    void ShowBindings() => ShowTextWithEditor("Bindings", Player.UsedInputConfContent);

    void AddToPath()
    {
        string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)!;

        if (path.ToLower().Contains(Folder.Startup.TrimEnd(Path.DirectorySeparatorChar).ToLower()))
        {
            Msg.ShowWarning(_("mpv.net is already in the Path environment variable."));
            return;
        }

        Environment.SetEnvironmentVariable("Path",
            Folder.Startup.TrimEnd(Path.DirectorySeparatorChar) + ";" + path,
            EnvironmentVariableTarget.User);

        Msg.ShowInfo(_("mpv.net was successfully added to the Path environment variable."));
    }

    void RemoveFromPath()
    {
        string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)!;

        if (!path.Contains(Folder.Startup.TrimEnd(Path.DirectorySeparatorChar)))
        {
            Msg.ShowWarning(_("mpv.net was not found in the Path environment variable."));
            return;
        }

        path = path.Replace(Folder.Startup.TrimEnd(Path.DirectorySeparatorChar), "");
        path = path.Replace(";;", ";").Trim(';');

        Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.User);

        Msg.ShowInfo(_("mpv.net was successfully removed from the Path environment variable."));
    }

    // deprecated
    void ShowTracks() =>
        Msg.ShowInfo(_("This feature was removed, but there are user scripts:") + BR2 +
            "https://github.com/stax76/mpv-scripts#command_palette" + BR +
            "https://github.com/stax76/mpv-scripts#search_menu" + BR +
            "https://github.com/tomasklaen/uosc");

    // deprecated
    void ShowPlaylist() =>
        Msg.ShowInfo(_("This feature was removed, but there are user scripts:") + BR2 +
            "https://github.com/stax76/mpv-scripts#command_palette" + BR +
            "https://github.com/stax76/mpv-scripts#search_menu" + BR +
            "https://github.com/tomasklaen/uosc" + BR +
            "https://github.com/jonniek/mpv-playlistmanager");

    // deprecated
    void ShowCommandPalette() =>
        Msg.ShowInfo(_("This feature was removed, but there are user scripts:") + BR2 +
            "https://github.com/stax76/mpv-scripts#command_palette" + BR +
            "https://github.com/stax76/mpv-scripts#search_menu" + BR +
            "https://github.com/tomasklaen/uosc");

    // deprecated
    void QuickBookmark() =>
        Msg.ShowInfo(_("This feature was removed, but there are user scripts:") + BR2 +
            "https://github.com/stax76/mpv-scripts/blob/main/misc.lua");

    // deprecated
    void ShowHistory() =>
        Msg.ShowInfo(_("This feature was removed, but there are user scripts:") + BR2 +
            "https://github.com/stax76/mpv-scripts/blob/main/history.lua");

    // deprecated
    void ShowRemoved() => Msg.ShowInfo(_("This feature was removed."));
}


//public void ShowCommandPalette()
//{
//    MainForm.Instance?.BeginInvoke(() => {
//        CommandPalette.Instance.SetItems(CommandPalette.GetItems());
//        MainForm.Instance.ShowCommandPalette();
//        CommandPalette.Instance.SelectFirst();
//    });
//}
