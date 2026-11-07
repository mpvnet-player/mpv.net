# Display-change regression

These Windows integration probes use the actual mpv.net executable and libmpv.
They are manual opt-in tests because one changes the primary display mode.

## Native asynchronous APIs

With .NET 10 and a complete player directory:

```powershell
dotnet run --project tests/DisplayChange/NativeApiProbe -c Release -- 'D:/TestPlayer'
```

Checks a flag write, command completion, double property reply, unavailable
playback-position reply, and shutdown against the real DLL. No video or display
mode change is needed. The project references the modified managed bindings.

## Fullscreen and display changes

Compile `display-mode.c` using a Windows C compiler (for example
`gcc -O2 display-mode.c -o display-mode.exe -luser32`). `display-mode.exe list`
prints supported modes. This probe currently requires 1920x1080 at 120 Hz and
alternates it with the original display mode. Run only if that mode is listed;
adjust the explicit mode in PlayerProbe/Program.cs for a different test display.

Use a disposable complete AJN preview 6 directory, containing the bundled
`animejanai/benchmarks/1920x1080.mp4`. Do not use your normal player folder.

```powershell
dotnet run --project tests/DisplayChange/PlayerProbe -c Release -- `
  'D:/TestPlayer' 'D:/NewTestResults' 'D:/Tools/display-mode.exe' `
  'D:/Tools/gdb.exe' --expect-fit
```

The test launches its own fullscreen Vulkan player, sends a synthetic
WM_DPICHANGED with an oversized suggested rectangle, and checks monitor fit.
It then performs six actual mode changes, checks playback progress and video
surface dimensions, and closes its player. It saves/restores the original mode
in a finally block. On a freeze it attempts a bounded thread capture with GDB.
`--dpi-only` skips the physical mode changes. Output must be a new directory.

The synthetic message is a sizing regression, not a substitute for physical
DPI changes. The playback test disables AJN scripts/upscaling and subtitles to
isolate window handling; it does not reproduce an 8K projector configuration.

## Recorded results, 2026-11-07

- Original 7.1.2.0: six 4K144/1080p120 switches completed. No freeze reproduced.
- Original DPI probe: fullscreen client and video surface became 3866x2186 on a
  3840x2160 monitor after the synthetic message.
- Patched DPI probe: client and video surface remain 3840x2160.
- Patched display probe: six switches completed, playback advanced every time,
  and every video surface matched the current monitor dimensions. Original mode
  restored; clean player exit.
- Native API probe: passed all four asynchronous operations and shutdown.
- All three player runs logged the same pre-existing startup libplacebo
  `dst->params.blit_dst` validation warning before the probes began. It did not
  prevent playback. This patch does not change that native code.

The intermittent RTX 5090 / 8K23.976 to 4K119.88 freeze reported in AJN issue 242
still needs confirmation on the affected setup. The wrapper removes synchronous
mouse, resize and periodic progress calls from the window message thread, where
they could block Vulkan WSI message processing. There is no native inference,
GPU sampling, CPU frame copy or per-frame observer added by this change.
