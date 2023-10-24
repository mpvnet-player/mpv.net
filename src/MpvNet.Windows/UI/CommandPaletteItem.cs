
namespace MpvNet.Windows.UI;

public class CommandPaletteItem
{
    public CommandPaletteItem() { }

    public CommandPaletteItem(string text, Action action)
    {
        Text = text;
        Action = action;
    }

    public CommandPaletteItem(string text, string secondaryText, Action action)
    {
        Text = text;
        Action = action;
        SecondaryText = secondaryText;
    }

    public string Text { get; set; } = "";
    public string SecondaryText { get; set; } = "";
    public Action? Action { get; set; }
    public Binding? Binding { get; set; }
}
