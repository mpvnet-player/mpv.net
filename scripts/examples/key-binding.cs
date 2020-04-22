
// This script adds a key binding.

using System.Reflection;

using mpvnet;

class Script
{
    public Script()
    {
        string content = "ctrl+ö script-message my-message-1 my-argument-1";
        string sectionName = Assembly.GetExecutingAssembly().GetName().Name;
        mp.commandv("define-section", sectionName, content, "force");
        mp.commandv("enable-section", sectionName);
        mp.ClientMessage += ClientMessage;
    }

    void ClientMessage(string[] args)
    {
        switch (args[0])
        {
            case "my-message-1":
                Msg.Show(args[1]);
                break;
        }
    }
}
