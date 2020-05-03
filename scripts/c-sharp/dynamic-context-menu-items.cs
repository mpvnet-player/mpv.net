
// This script creates context menu items dynamically.

using mpvnet;
using System.ComponentModel;
using System.Linq;

class Script
{
    MainForm MainForm;
    Core core;

    public Script()
    {
        core = Core.core;
        MainForm = mpvnet.MainForm.Instance;
        MainForm.ContextMenu.Opening += ContextMenu_Opening;
    }

    void ContextMenu_Opening(object sender, CancelEventArgs e)
    {
        // edit input.conf and add 'Edition' menu item there
        MenuItem menuItem = MainForm.FindMenuItem("Edition");

        if (menuItem == null)
            return;

        menuItem.DropDownItems.Clear();
        var editionTracks = core.MediaTracks.Where(track => track.Type == "e");

        foreach (MediaTrack track in editionTracks)
        {
            MenuItem mi = new MenuItem(track.Text);
            mi.Action = () => { core.commandv("set", "edition", track.ID.ToString()); };
            mi.Checked = core.Edition == track.ID;
            menuItem.DropDownItems.Add(mi);
        }
    }
}
