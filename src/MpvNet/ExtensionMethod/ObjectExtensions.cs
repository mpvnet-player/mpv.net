
namespace MpvNet.Extensions;

public static class ObjectExtensions
{
    extension(object instance)
    {
        public string ToStringEx() => instance?.ToString() ?? "";
    }
}
