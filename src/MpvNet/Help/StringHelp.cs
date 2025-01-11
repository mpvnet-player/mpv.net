
using System.Security.Cryptography;
using System.Text;

namespace MpvNet.Help;

public static class StringHelp
{
    public static string GetMD5Hash(string txt)
    {
        byte[] inputBuffer = Encoding.UTF8.GetBytes(txt);
        return Convert.ToHexString(MD5.HashData(inputBuffer));
    }
}
