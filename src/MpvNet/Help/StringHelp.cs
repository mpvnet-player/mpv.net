
using System.Security.Cryptography;
using System.Text;

namespace MpvNet.Help;

public static class StringHelp
{
    public static string GetMD5Hash(string txt)
    {
        using MD5 md5 = MD5.Create();
        byte[] inputBuffer = Encoding.UTF8.GetBytes(txt);
        byte[] hashBuffer = md5.ComputeHash(inputBuffer);
        return BitConverter.ToString(md5.ComputeHash(inputBuffer)).Replace("-", "");
    }
}
