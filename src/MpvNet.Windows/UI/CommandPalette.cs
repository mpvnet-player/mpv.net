
using MpvNet.Windows.WPF.Controls;

namespace MpvNet.Windows.UI;

public class CommandPalette
{
    public static CommandPaletteControl Instance { get; } = new CommandPaletteControl();

    public static IEnumerable<CommandPaletteItem> GetItems()
    {
        return InputHelp.GetBindingsFromContent(App.InputConf.GetContent())
            .Where(i => i.Command != "")
            .Select(i => new CommandPaletteItem()
            {
                Text = i.Comment,
                SecondaryText = i.Input,
                Action = () => Core.Command(i.Command),
                Binding = i
            });
    }
}
