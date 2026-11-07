using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json.Nodes;
sealed class Player:IAsyncDisposable{
    readonly Process process;readonly Task<string> stdout,stderr;readonly string outputPath;readonly string name="AJN.LightingCheck."+Guid.NewGuid().ToString("N");NamedPipeClientStream? pipe;StreamReader? reader;StreamWriter? writer;int serial;
    public int Id=>process.Id;
    public Player(string root,string data,string output,string exe,string media){
        string config=Path.Combine(root,"portable_config");Directory.CreateDirectory(config);
        var info=new ProcessStartInfo(Path.Combine(root,exe)){UseShellExecute=false,CreateNoWindow=true,WindowStyle=ProcessWindowStyle.Hidden,WorkingDirectory=root,RedirectStandardOutput=true,RedirectStandardError=true};
        string? resume=Environment.GetEnvironmentVariable("AJN_SWITCH_RESUME_DIR");
        if(resume is not null)foreach(var a in new[]{"--watch-later-directory="+resume,"--resume-playback=yes","--save-position-on-quit=no"})info.ArgumentList.Add(a);
        info.Environment["MPVNET_HOME"]=config;info.Environment["ANIMEJANAI_ROOT"]=root;info.Environment["ANIMEJANAI_DATA_DIR"]=data;
        if(exe=="mpvnet.exe")foreach(var a in new[]{"--process-instance=multi","--auto-load-folder=no"})info.ArgumentList.Add(a);
        foreach(var a in new[]{"--config-dir="+config,"--hwdec=nvdec","--vo=gpu-next","--gpu-api=vulkan","--gpu-context=winvk","--force-window=yes","--fullscreen=yes","--no-config","--load-scripts=no","--video-sync=display-resample","--ao=null","--audio=no","--sub=no","--keep-open=yes","--idle=yes","--input-terminal=no","--terminal=no","--start=0",
            @"--input-ipc-server=\\.\pipe\"+name,"--log-file="+Path.Combine(output,exe+".log"),media})if(a!="--start=0"||resume is null)info.ArgumentList.Add(a);
        outputPath=output;process=Process.Start(info)??throw new Exception("Player did not start.");stdout=process.StandardOutput.ReadToEndAsync();stderr=process.StandardError.ReadToEndAsync();
    }
    public async Task Connect(){pipe=new(".",name,PipeDirection.InOut,PipeOptions.Asynchronous|PipeOptions.CurrentUserOnly);await pipe.ConnectAsync(45000);reader=new(pipe,leaveOpen:true);writer=new(pipe,leaveOpen:true){AutoFlush=true};}
    public async Task<JsonNode?> Command(params string[] command){using var c=new CancellationTokenSource(TimeSpan.FromSeconds(4));int id=++serial;
        await writer!.WriteLineAsync(new JsonObject{["command"]=new JsonArray(command.Select(s=>(JsonNode)JsonValue.Create(s)!).ToArray()),["request_id"]=id}.ToJsonString());
        while(true){var line=await reader!.ReadLineAsync(c.Token)??throw new IOException("Player disconnected.");var r=JsonNode.Parse(line)!;if(r["request_id"]?.GetValue<int>()==id){if(r["error"]?.GetValue<string>()!="success")throw new Exception(r.ToJsonString());return r["data"]?.DeepClone();}}}
    public async Task Quit(){if(process.HasExited)return;await writer!.WriteLineAsync("{\"command\":[\"quit\"]}");await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(8));}
    public async ValueTask DisposeAsync(){try{if(!process.HasExited){process.Kill();await process.WaitForExitAsync();}File.WriteAllText(Path.Combine(outputPath,"player-console.log"),await stdout+await stderr);}finally{writer?.Dispose();reader?.Dispose();pipe?.Dispose();process.Dispose();}}
}
