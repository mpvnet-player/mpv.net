
namespace MpvNet;

public static class Global
{
    public static readonly string BR = Environment.NewLine;
    public static readonly string BR2 = Environment.NewLine + Environment.NewLine;
    public static readonly MainPlayer Player = new MainPlayer();
    public static readonly MainPlayer Core = Player; // backward compatibility
    public static readonly AppClass App = new AppClass();
}
