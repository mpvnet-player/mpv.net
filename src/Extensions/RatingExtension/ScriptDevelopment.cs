
//// This script adds dynamic menu items for profile switching.

//// In input.conf add a menu item called 'Profiles'

//using mpvnet;
//using System.ComponentModel;
//using System.Linq;

//class Script
//{
//    MainForm MainForm;
//    CorePlayer Core;

//    public Script()
//    {
//        Core = Global.Core;
//        MainForm = MainForm.Instance;
//        MainForm.ContextMenu.Opening += ContextMenu_Opening;
//    }

//    void ContextMenu_Opening(object sender, CancelEventArgs e)
//    {
//        MenuItem menuItem = MainForm.FindMenuItem("My Menu");

//        if (menuItem == null)
//        {
//            Terminal.WriteError("Profiles menu item not found.", "switch-profile-context-menu.cs");
//            return;
//        }

//        menuItem.DropDownItems.Clear();
//        var editionTracks = Core.MediaTracks.Where(track => track.Type == "e");

//        foreach (int i in new[] {1, 2, 3})
//        {
//            MenuItem mi = new MenuItem(i.ToString());
//            mi.Action = () => { Core.commandv("show-text", i.ToString()); };
//            menuItem.DropDownItems.Add(mi);
//        }
//    }
//}
