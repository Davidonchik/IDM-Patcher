#include <windows.h>
#include <stdio.h>

int main() {
    printf("IDM Patch Unload Signal\n");
    printf("=======================\n\n");

    HANDLE hEvent = OpenEventA(EVENT_MODIFY_STATE, FALSE, "Global\\IDMPatchUnloadEvent");
    if (!hEvent) {
        printf("Failed to open unload event: %d\n", GetLastError());
        printf("DLL is probably not loaded.\n");
        return 0;
    }

    printf("Sending unload signal to DLL...\n");
    
    SetEvent(hEvent);
    CloseHandle(hEvent);

    printf("Signal sent! DLL will unload itself.\n");
    printf("Waiting 2 seconds for DLL to unload...\n");
    
    Sleep(2000);
    
    printf("Done!\n");
    return 0;
}
