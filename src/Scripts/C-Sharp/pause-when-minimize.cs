
// Pauses playback when window is minimized and resumes afterwards.

using System;
using System.Windows.Forms;

using mpvnet;

class Script
{
    MainForm MainForm;
    CorePlayer Core;

    bool WasPlaying;
    bool WasPaused;

    public Script()
    {
        Core = Global.Core;
        MainForm = MainForm.Instance;
        MainForm.Resize += Form_Resize;
    }

    void Form_Resize(object sender, EventArgs e)
    {
        if (MainForm.WindowState == FormWindowState.Minimized)
        {
            WasPlaying = !Core.get_property_bool("pause");

            if (WasPlaying)
            {
                Core.set_property_bool("pause", true, true);
                WasPaused = true;
            }
        }
        else
        {
            if (WasPaused)
            {
                Core.set_property_bool("pause", false, true);
                WasPaused = false;
            }
        }
    }
}
