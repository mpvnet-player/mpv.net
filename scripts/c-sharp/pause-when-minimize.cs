
// Pauses playback when window is minimized and resumes afterwards.

using System;
using System.Windows.Forms;

using mpvnet;

class Script
{
    MainForm Form;
    Core core;

    bool WasPlaying;
    bool WasPaused;

    public Script()
    {
        core = Core.core;
        Form = MainForm.Instance;
        Form.Resize += Form_Resize;
    }

    private void Form_Resize(object sender, EventArgs e)
    {
        if (Form.WindowState == FormWindowState.Minimized)
        {
            WasPlaying = !core.get_property_bool("pause");

            if (WasPlaying)
            {
                core.set_property_bool("pause", true, true);
                WasPaused = true;
            }
        }
        else
        {
            if (WasPaused)
            {
                core.set_property_bool("pause", false, true);
                WasPaused = false;
            }
        }
    }
}
