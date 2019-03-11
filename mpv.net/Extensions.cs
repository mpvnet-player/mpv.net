using System.Collections.Generic;
using System.Linq;

namespace mpvnet
{
    public static class Extensions
    {
        public static string Join(this IEnumerable<string> instance, string delimiter, bool removeEmpty = false)
        {
            if (instance == null)
                return null;

            bool containsEmpty = false;

            foreach (var i in instance)
            {
                if (string.IsNullOrEmpty(i))
                {
                    containsEmpty = true;
                    break;
                }
            }

            if (containsEmpty && removeEmpty)
                instance = instance.Where(arg => !string.IsNullOrEmpty(arg));

            return string.Join(delimiter, instance);
        }
    }
}