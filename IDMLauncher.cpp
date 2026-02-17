/*
 * IDM Launcher
 */

#include <windows.h>
#include <stdio.h>

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
    char exePath[MAX_PATH];
    char originalExe[MAX_PATH];
    char injectorPath[MAX_PATH];
    char dllPath[MAX_PATH];
    
    GetModuleFileNameA(NULL, exePath, MAX_PATH);
    
    char* lastSlash = strrchr(exePath, '\\');
    if (lastSlash) {
        *lastSlash = '\0';
    }
    
    sprintf_s(originalExe, "%s\\IDMan_original.exe", exePath);
    sprintf_s(injectorPath, "%s\\idm_injector.exe", exePath);
    sprintf_s(dllPath, "%s\\idm_patch.dll", exePath);
    
    if (GetFileAttributesA(originalExe) == INVALID_FILE_ATTRIBUTES) {
        MessageBoxA(NULL, "IDMan_original.exe not found!", "Error", MB_OK | MB_ICONERROR);
        return 1;
    }
    
    if (GetFileAttributesA(injectorPath) == INVALID_FILE_ATTRIBUTES) {
        MessageBoxA(NULL, "idm_injector.exe not found!", "Error", MB_OK | MB_ICONERROR);
        return 1;
    }
    
    if (GetFileAttributesA(dllPath) == INVALID_FILE_ATTRIBUTES) {
        MessageBoxA(NULL, "idm_patch.dll not found!", "Error", MB_OK | MB_ICONERROR);
        return 1;
    }
    
    char cmdLine[1024];
    sprintf_s(cmdLine, "\"%s\" launch \"%s\" \"%s\"", injectorPath, originalExe, dllPath);
    
    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;
    
    if (!CreateProcessA(NULL, cmdLine, NULL, NULL, FALSE, 0, NULL, exePath, &si, &pi)) {
        MessageBoxA(NULL, "Failed to launch IDM with patch!", "Error", MB_OK | MB_ICONERROR);
        return 1;
    }
    
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    
    return 0;
}
