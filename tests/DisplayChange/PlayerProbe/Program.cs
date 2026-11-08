using System.Diagnostics;
using System.Text.Json.Nodes;

string root=Path.GetFullPath(args[0]), output=Path.GetFullPath(args[1]), modeTool=Path.GetFullPath(args[2]), debugger=Path.GetFullPath(args[3]);
if(Directory.Exists(output)) throw new Exception("Choose a fresh output directory");
Directory.CreateDirectory(output);
string saved=Path.Combine(output,"original-mode.bin");
var evidence=new JsonArray();
async Task<string> Tool(string exe, params string[] arguments) {
    var info=new ProcessStartInfo(exe){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};
    foreach(var arg in arguments)info.ArgumentList.Add(arg);
    using var p=Process.Start(info)!;var stdout=p.StandardOutput.ReadToEndAsync();var stderr=p.StandardError.ReadToEndAsync();
    try{await p.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));}
    catch{if(!p.HasExited)p.Kill(true);throw;}
    string text=await stdout+await stderr;if(p.ExitCode!=0)throw new Exception(text);return text;
}
await Tool(modeTool,"save",saved);
await using var player=new Player(root,Path.Combine(root,"animejanai"),output,"mpvnet.exe",Path.Combine(root,"animejanai/benchmarks/1920x1080.mp4"));
try {
    await player.Connect();await player.Command("set_property","loop-file","inf");
    await Task.Delay(5000);
    using(var process=Process.GetProcessById(player.Id)) {
        process.Refresh();
        var initial=JsonNode.Parse(await Tool(modeTool,"window",process.MainWindowHandle.ToString()))!;
        if(args.Contains("--expect-fit") && initial["perMonitorV2"]?.GetValue<bool>() != true)
            throw new Exception("The player must run as PerMonitorV2; check the embedded application manifest");
        int dpi=initial["dpi"]!.GetValue<int>();
        await Tool(modeTool,"dpi",process.MainWindowHandle.ToString(),(dpi+48).ToString(),"6827","3840");
        await Task.Delay(1500);
        var changed=JsonNode.Parse(await Tool(modeTool,"window",process.MainWindowHandle.ToString()))!;
        evidence.Add(new JsonObject{["test"]="synthetic-dpi-change",["before"]=initial,["after"]=changed,
            ["surface"]=await player.Command("get_property","osd-dimensions")});
        Console.WriteLine("DPI rectangle: "+changed.ToJsonString());
        if(args.Contains("--expect-fit") &&
            (changed["client"]![0]!.GetValue<int>() != changed["monitor"]![2]!.GetValue<int>()-changed["monitor"]![0]!.GetValue<int>() ||
             changed["client"]![1]!.GetValue<int>() != changed["monitor"]![3]!.GetValue<int>()-changed["monitor"]![1]!.GetValue<int>()))
            throw new Exception("Fullscreen client does not fit the monitor after a DPI change");
        await Tool(modeTool,"dpi",process.MainWindowHandle.ToString(),dpi.ToString(),
            initial["client"]![0]!.ToString(),initial["client"]![1]!.ToString());
        await Task.Delay(1000);
    }
    for(int i=0;i<(args.Contains("--dpi-only")?0:6);i++) {
        if(i%2==0)await Tool(modeTool,"set","1920","1080","120");
        else await Tool(modeTool,"restore",saved);
        await Task.Delay(5000);
        double before=(await player.Command("get_property","time-pos"))!.GetValue<double>();
        await Task.Delay(1000);
        double after=(await player.Command("get_property","time-pos"))!.GetValue<double>();
        using var process=Process.GetProcessById(player.Id);process.Refresh();
        evidence.Add(new JsonObject{["switch"]=i+1,["before"]=before,["after"]=after,
            ["displayFps"]=await player.Command("get_property","display-fps"),
            ["surface"]=await player.Command("get_property","osd-dimensions"),
            ["geometry"]=JsonNode.Parse(await Tool(modeTool,"window",process.MainWindowHandle.ToString()))});
        if(after<=before)throw new Exception("Playback position did not advance");
        var item=evidence[evidence.Count-1]!;
        if(args.Contains("--expect-fit") &&
            (item["surface"]!["w"]!.GetValue<int>() != item["geometry"]!["monitor"]![2]!.GetValue<int>()-item["geometry"]!["monitor"]![0]!.GetValue<int>() ||
             item["surface"]!["h"]!.GetValue<int>() != item["geometry"]!["monitor"]![3]!.GetValue<int>()-item["geometry"]!["monitor"]![1]!.GetValue<int>()))
            throw new Exception("Video surface does not fit the monitor after a mode switch");
        Console.WriteLine($"PASS mode switch {i+1}: playback continues");
    }
    await player.Quit();
    File.WriteAllText(Path.Combine(output,"results.json"),new JsonObject{["passed"]=true,["evidence"]=evidence}.ToJsonString());
} catch(Exception error) {
    File.WriteAllText(Path.Combine(output,"results.json"),new JsonObject{["passed"]=false,["error"]=error.ToString(),["evidence"]=evidence}.ToJsonString());
    try{File.WriteAllText(Path.Combine(output,"threads.txt"),await Tool(debugger,"-nx","-batch","-p",player.Id.ToString(),"-ex","set pagination off","-ex","thread apply all bt","-ex","detach"));}
    catch(Exception e){File.WriteAllText(Path.Combine(output,"dump-error.txt"),e.ToString());}
    throw;
} finally {await Tool(modeTool,"restore",saved);}
