#define WINVER 0x0A00
#define _WIN32_WINNT 0x0A00
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int main(int argc, char **argv)
{
    SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
    DEVMODEW current = {.dmSize = sizeof(current)}, mode = {.dmSize = sizeof(mode)};
    if (argc < 2 || !EnumDisplaySettingsW(NULL, ENUM_CURRENT_SETTINGS, &current)) return 2;
    if (!strcmp(argv[1], "list")) {
        printf("current %lu %lu %lu\n", current.dmPelsWidth, current.dmPelsHeight, current.dmDisplayFrequency);
        for (DWORD i = 0; EnumDisplaySettingsW(NULL, i, &mode); i++)
            if (mode.dmBitsPerPel == current.dmBitsPerPel && mode.dmDisplayFlags == current.dmDisplayFlags &&
                mode.dmPelsWidth >= 1920 && mode.dmDisplayFrequency >= 60)
                printf("mode %lu %lu %lu\n", mode.dmPelsWidth, mode.dmPelsHeight, mode.dmDisplayFrequency);
        return 0;
    }
    if (argc == 3 && !strcmp(argv[1], "window")) {
        HWND window = (HWND)(uintptr_t)strtoull(argv[2], NULL, 10);
        RECT client, rect; MONITORINFO monitor = {.cbSize = sizeof(monitor)};
        if (!GetClientRect(window, &client) || !GetWindowRect(window, &rect) ||
            !GetMonitorInfoW(MonitorFromWindow(window, MONITOR_DEFAULTTONEAREST), &monitor)) return 3;
        printf("{\"client\":[%ld,%ld],\"window\":[%ld,%ld,%ld,%ld],\"monitor\":[%ld,%ld,%ld,%ld],\"dpi\":%u}\n",
            client.right, client.bottom, rect.left, rect.top, rect.right, rect.bottom,
            monitor.rcMonitor.left, monitor.rcMonitor.top, monitor.rcMonitor.right, monitor.rcMonitor.bottom,
            GetDpiForWindow(window));
        return 0;
    }
    if (argc == 6 && !strcmp(argv[1], "dpi")) {
        HWND window = (HWND)(uintptr_t)strtoull(argv[2], NULL, 10);
        UINT dpi = atoi(argv[3]);
        RECT rect = {0, 0, atoi(argv[4]), atoi(argv[5])};
        DWORD_PTR result;
        return SendMessageTimeoutW(window, WM_DPICHANGED, MAKELONG(dpi, dpi),
            (LPARAM)&rect, SMTO_ABORTIFHUNG, 3000, &result) ? 0 : 7;
    }
    if (argc == 3 && !strcmp(argv[1], "save")) {
        FILE *file = fopen(argv[2], "wb"); if (!file) return 3;
        int ok = fwrite(&current, sizeof(current), 1, file) == 1; fclose(file);
        return ok ? 0 : 4;
    }
    if (argc == 3 && !strcmp(argv[1], "restore")) {
        FILE *file = fopen(argv[2], "rb"); if (!file) return 3;
        int ok = fread(&mode, sizeof(mode), 1, file) == 1; fclose(file); if (!ok) return 4;
    } else if (argc == 5 && !strcmp(argv[1], "set")) {
        DWORD width = atoi(argv[2]), height = atoi(argv[3]), hz = atoi(argv[4]); BOOL found = FALSE;
        for (DWORD i = 0; EnumDisplaySettingsW(NULL, i, &mode); i++)
            if (mode.dmPelsWidth == width && mode.dmPelsHeight == height && mode.dmDisplayFrequency == hz &&
                mode.dmBitsPerPel == current.dmBitsPerPel && mode.dmDisplayFlags == current.dmDisplayFlags) {
                found = TRUE; break;
            }
        if (!found) return 5;
    } else return 2;
    mode.dmFields = DM_DISPLAYFREQUENCY | DM_PELSWIDTH | DM_PELSHEIGHT | DM_BITSPERPEL;
    LONG result = ChangeDisplaySettingsExW(NULL, &mode, NULL, CDS_TEST, NULL);
    if (result == DISP_CHANGE_SUCCESSFUL) result = ChangeDisplaySettingsExW(NULL, &mode, NULL, 0, NULL);
    printf("%ld\n", result);
    return result == DISP_CHANGE_SUCCESSFUL ? 0 : 6;
}
