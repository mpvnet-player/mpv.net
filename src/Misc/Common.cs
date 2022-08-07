
using System.Collections.Generic;
using System.Linq;

namespace mpvnet
{
    public class StringPair
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ConfParser
    {
        public static List<ConfSection> Parse(string content)
        {
            string[] lines = content.Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            var sections = new List<ConfSection>();
            ConfSection currentGroup = null;

            foreach (string i in lines)
            {
                string line = i.Trim();

                if (line == "")
                {
                    currentGroup = new ConfSection();
                    sections.Add(currentGroup);
                }
                else if (line.Contains("="))
                {
                    string name = line.Substring(0, line.IndexOf("=")).Trim();
                    string value = line.Substring(line.IndexOf("=") + 1).Trim();

                    currentGroup.Items.Add(new StringPair() { Name = name, Value = value });
                }
            }

            return sections;
        }
    }

    public class ConfSection
    {
        public List<StringPair> Items { get; set; } = new List<StringPair>();

        public bool HasName(string name)
        {
            foreach (var i in Items)
                if (i.Name == name)
                    return true;
            return false;
        }

        public string GetValue(string name)
        {
            foreach (var i in Items)
                if (i.Name == name)
                    return i.Value;
            return null;
        }

        public List<StringPair> GetValues(string name) => Items.Where(i => i.Name == name).ToList();
    }
}
