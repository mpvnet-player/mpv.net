using System;
using System.Windows.Forms;

using mpvnet;

class Script
{
    MainForm Form;

    bool WasPlaying;
    bool WasPaused;

    public Script()
    {
        Form = MainForm.Instance;
        Form.Resize += Form_Resize;
    }

    private void Form_Resize(object sender, EventArgs e)
    {
        if (Form.WindowState == FormWindowState.Minimized)
        {
            WasPlaying = mp.get_property_string("pause") == "no";

            if (WasPlaying)
            {
                mp.command("set pause yes");
                WasPaused = true;
            }
        }
        else
        {
            if (WasPaused)
            {
                mp.command("set pause no");
                WasPaused = false;
            }
        }
    }
}