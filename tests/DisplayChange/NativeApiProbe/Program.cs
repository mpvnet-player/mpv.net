using System.Runtime.InteropServices;
using MpvNet;
using static MpvNet.Native.LibMpv;

string root = Path.GetFullPath(args[0]);
NativeLibrary.SetDllImportResolver(typeof(MpvClient).Assembly,
    (name, assembly, path) => name == "libmpv-2.dll"
        ? NativeLibrary.Load(Path.Combine(root, name)) : IntPtr.Zero);
nint handle = mpv_create();
if (handle == IntPtr.Zero) throw new Exception("mpv_create failed");
var client = new MpvClient { Handle = handle };
var propertySet = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var command = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var value = new TaskCompletionSource<(string, double)>(TaskCreationOptions.RunContinuationsAsynchronously);
client.SetPropertyReply += () => propertySet.TrySetResult();
client.CommandReply += () => command.TrySetResult();
client.DoublePropertyReply += (name, number) => value.TrySetResult((name, number));
foreach (var (name, setting) in new[] { ("config", "no"), ("vo", "null"), ("ao", "null"), ("idle", "yes") })
    if (mpv_set_option_string(handle, System.Text.Encoding.UTF8.GetBytes(name + "\0"), System.Text.Encoding.UTF8.GetBytes(setting + "\0")) < 0)
        throw new Exception("Cannot set " + name);
if (mpv_initialize(handle) < 0) throw new Exception("mpv_initialize failed");
var loop = Task.Run(client.EventLoop);
try
{
    client.SetPropertyBoolAsync("pause", true);
    await propertySet.Task.WaitAsync(TimeSpan.FromSeconds(5));
    if (!client.GetPropertyBool("pause")) throw new Exception("Async flag write failed");
    client.CommandVAsync("set", "speed", "1.25");
    await command.Task.WaitAsync(TimeSpan.FromSeconds(5));
    if (!client.RequestPropertyDouble("speed")) throw new Exception("Async read was not accepted");
    if (await value.Task.WaitAsync(TimeSpan.FromSeconds(5)) != ("speed", 1.25))
        throw new Exception("Incorrect asynchronous double reply");
    value = new(TaskCreationOptions.RunContinuationsAsynchronously);
    if (!client.RequestPropertyDouble("time-pos")) throw new Exception("Unavailable read was not accepted");
    if (await value.Task.WaitAsync(TimeSpan.FromSeconds(5)) != ("time-pos", 0.0))
        throw new Exception("Unavailable playback position did not complete");
    Console.WriteLine("PASS actual libmpv async flag write, command, double reply and unavailable-position reply.");
}
finally
{
    client.CommandVAsync("quit");
    await loop.WaitAsync(TimeSpan.FromSeconds(5));
    mpv_destroy(handle);
}
