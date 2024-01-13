
namespace MpvNet;

public class Translator
{
    public static ITranslator? Current;
}

public interface ITranslator
{
    public string Gettext(string msgId);
    public string GetParticularString(string context, string text);
}
