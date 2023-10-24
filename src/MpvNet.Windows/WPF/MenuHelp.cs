
using System.Windows.Controls;

namespace MpvNet.Windows.WPF;

public class MenuHelp
{
    public static MenuItem? Add(ItemCollection? items, string path)
    {
        string[] parts = path.Split(new[] { " > ", " | " }, StringSplitOptions.RemoveEmptyEntries);

        for (int x = 0; x < parts.Length; x++)
        {
            bool found = false;

            foreach (MenuItem i in items!.OfType<MenuItem>())
            {
                if (x < parts.Length - 1)
                {
                    if ((string)i.Header == parts[x])
                    {
                        found = true;
                        items = i.Items;
                    }
                }
            }

            if (!found)
            {
                if (x == parts.Length - 1)
                {
                    if (parts[x] == "-")
                        items?.Add(new Separator());
                    else
                    {
                        MenuItem item = new MenuItem() { Header = parts[x] };
                        items?.Add(item);
                        items = item.Items;
                        return item;
                    }
                }
                else
                {
                    MenuItem item = new MenuItem() { Header = parts[x] };
                    items?.Add(item);
                    items = item.Items;
                }
            }
        }

        return null;
    }
}
