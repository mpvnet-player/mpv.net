using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace mpvInputEdit
{
    public partial class App : Application
    {
        public static string InputConfPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\input.conf";

        private static ObservableCollection<InputItem> _InputItems;

        public static ObservableCollection<InputItem> InputItems
        {
            get
            {
                if (_InputItems is null)
                {
                    _InputItems = new ObservableCollection<InputItem>();

                    if (File.Exists(InputConfPath))
                    {
                        foreach (string line in File.ReadAllLines(InputConfPath))
                        {
                            string l = line.Trim();
                            if (l.StartsWith("#")) continue;
                            if (!l.Contains(" ")) continue;
                            InputItem item = new InputItem();
                            item.Key = l.Substring(0, l.IndexOf(" "));
                            if (item.Key == "") continue;
                            l = l.Substring(l.IndexOf(" ") + 1);

                            if (l.Contains("#menu:"))
                            {
                                item.Menu = l.Substring(l.IndexOf("#menu:") + 6).Trim();
                                l = l.Substring(0, l.IndexOf("#menu:"));

                                if (item.Menu.Contains(";"))
                                    item.Menu = item.Menu.Substring(item.Menu.IndexOf(";") + 1).Trim();
                            }

                            item.Command = l.Trim();
                            if (item.Command == "")
                                continue;
                            if (item.Command.ToLower() == "ignore")
                                item.Command = "";
                            _InputItems.Add(item);
                        }
                    }
                }
                return _InputItems;
            }
        }
    }
}