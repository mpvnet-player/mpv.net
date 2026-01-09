
namespace MpvNet.Extensions;

public static class PathStringExtensions
{
    extension(string filepath)
    {
        public string Ext => GetExt(filepath, false);

        static string GetExt(string path, bool includeDot)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            char[] chars = path.ToCharArray();

            for (int x = path.Length - 1; x >= 0; x--)
            {
                if (chars[x] == '/')
                    return "";
                if (chars[x] == '\\')
                    return "";
                if (chars[x] == '.')
                    return path[(x + (includeDot ? 0 : 1))..].ToLowerInvariant();
            }

            return "";
        }

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(filepath))
                    return "";

                int index = filepath.LastIndexOf('\\');

                if (index > -1)
                    return filepath[(index + 1)..];

                index = filepath.LastIndexOf('/');

                if (index > -1)
                    return filepath[(index + 1)..];

                return filepath;
            }
        }

        public string ShortPath(int maxLength)
        {
            if (string.IsNullOrEmpty(filepath))
                return "";

            if (filepath.Length > maxLength && filepath.Substring(1, 2) == ":\\")
                filepath = $"{filepath[..3]}...\\{filepath.FileName}";

            return filepath;
        }

        // Ensure trailing directory separator char
        public string Separator
        {
            get
            {
                if (string.IsNullOrEmpty(filepath))
                    return "";

                if (!filepath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    filepath = filepath + Path.DirectorySeparatorChar;

                return filepath;
            }
        }
    }
}
