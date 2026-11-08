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

The test launches its own fullscreen Vulkan player. With `--expect-fit`, it
first checks the actual window DPI-awareness context is PerMonitorV2. Checking
the source manifest alone is insufficient: it must be embedded in the EXE and
its `application` element must use the asm.v3 namespace.

The test then sends a synthetic
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

## Real scale-change follow-up, 2026-11-08

The first candidate and original release both contained the default UAC-only
EXE manifest. The project was not selecting `app.manifest`, and the source
manifest's DPI settings were under an `application` element in the wrong
namespace. Both running players were system-DPI-aware, despite the intended
PerMonitorV2 declaration in the source file. The corrected build embeds the
manifest and reports PerMonitorV2 from `GetWindowDpiAwarenessContext`.

During a real Windows Settings change from 150% to 225%, the original player
froze. A native thread capture showed the rendering thread waiting inside
`vkCreateSwapchainKHR` / `NtGdiDdDDIWaitForSynchronizationObjectFromCpu`, the mpv
core waiting in `vo_control(handle_cursor_autohide)`, and the UI thread waiting
in a synchronous mouse command. This establishes a local display-change hang;
it does not establish the cause of every freeze reported with an active addon.

The corrected framework-dependent Release EXE was tested on the same machine
with the same preview-6 native libraries and .NET Desktop runtime 10.0.12.
Real scale changes were made through Windows Settings while the player remained
running. Both the physical client and video surface stayed at 3840x2160.
Each row below records an eight-second sampling interval; zero drops means zero
increase in mpv's `frame-drop-count` during that interval, not a whole-session
or physical scanout guarantee.

| State | Estimated display FPS | Added dropped frames | Window DPI |
| --- | ---: | ---: | ---: |
| Fullscreen at 150%, 4K144 | 143.9979 | 0 | 144 |
| After live change to 225%, 4K144 | 143.9988 | 0 | 216 |
| After switch to 4K120, still 225% | 119.9990 | 0 | 216 |
| Restored 4K144, still 225% | 143.9961 | 0 | 216 |
| Restored 150%, 4K144 | 143.9969 | 0 | 144 |

The updated `--expect-fit --dpi-only` regression passed against this EXE, with
PerMonitorV2 true and a 3840x2160 client. Original scale and refresh were restored
and the test player exited. Publishing succeeds; enabling the manifest also
exposes WinForms warning WFO0003 about manifest-based DPI configuration. It is
not suppressed by this change.

To repeat the real-scale check, use a disposable player installation, start
fullscreen Vulkan playback, and change Windows Settings > System > Display >
Scale from the original value to another supported value and back. Check
`display-mode.exe window <HWND>`, `osd-dimensions`, `estimated-display-fps`, and
the change in `frame-drop-count`. Also switch to a supported 120 Hz mode and
back. Restore the original settings afterward. Do not restore/unmaximize the
player while bringing it to the foreground: that changes the tested geometry.
The synthetic message test does not replace this check.

### Supplied frozen-process dump, 2026-11-08

The later field report withdrew the frame-rate regression attribution: the
original EXE also presented at about 38 Hz in the same PC state. Do not attribute
that result to the wrapper change.

The supplied dump is from the first display-fix EXE, before the manifest
correction. Its libmpv and libplacebo module timestamps, image sizes and PE
checksums match the saved preview-6 binaries. Resolving the supplied stack return
addresses against their DWARF data (return address minus one) gives:

| Module-relative return address | Function and source line |
| --- | --- |
| libplacebo + 0xe827c | `vk_sw_recreate`, `src/vulkan/swapchain.c:800`, `vkCreateSwapchainKHR` |
| libplacebo + 0xe960d | `vk_sw_start_frame`, `src/vulkan/swapchain.c:926`, recreation after `VK_ERROR_OUT_OF_DATE_KHR` |
| libmpv + 0x15fcce | `draw_frame`, `video/out/vo_gpu_next.c:7582` |
| libmpv + 0x1313c4 | `vo_thread` / `render_frame`, `video/out/vo.c:1145/1013` |
| libmpv + 0x760c3 | `mp_dispatch_queue_process`, `misc/dispatch.c:285` |
| libmpv + 0xc9be6 | `run_playloop` / `mp_wait_events`, `player/playloop.c:1449/66` |
| libmpv + 0x1c9e97 | Optional sample worker `readback`, `video/filter/vf_ajn_sample.c:235` |

The rendering thread is inside a driver CPU synchronization wait during
swapchain creation. Both window threads are idle in message waits. Unlike the
earlier local original-player capture, the core is in its ordinary event wait,
not `vo_control(handle_cursor_autohide)`. The optional sampling worker has a
500 ms timeout in its wait, matching its detached/no-pending-work branch; it is
not blocked inside CUDA inference at capture time. The addon status replies
continue after video progress stops and report a ready sample with no analysis
error. These observations do not rule out earlier GPU interactions, or prove
the driver alone caused the freeze.

The final log requests a valid 7680x4320 swapchain when returning to 8K. The DPI
fix does not modify that native creation call, so this dump does not establish
that the latest EXE resolves the freeze. It is a frozen snapshot, not a replay
test of the modified code. No new display-switch stress runs were performed
after the reporter stopped testing because of two watchdog bugchecks; their
kernel dumps are not in this bundle.

The intermittent RTX 5090 / 8K23.976 to 4K119.88 freeze reported in AJN issue 242
remains unresolved. The local test disables AJN upscaling, addon sampling and subtitles.
The wrapper removes synchronous
mouse, resize and periodic progress calls from the window message thread, where
they could block Vulkan WSI message processing. There is no native inference,
GPU sampling, CPU frame copy or per-frame observer added by this change.
