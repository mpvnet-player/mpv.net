
using System;

namespace mpvnet
{
    public class Global
    {
        public static string BR = Environment.NewLine;
        public static string BR2 = Environment.NewLine + Environment.NewLine;

        public static CorePlayer Core { get; } = new CorePlayer();
    }
}
