/*
 * IDM DLL Injector
 */

#include <windows.h>
#include <tlhelp32.h>
#include <stdio.h>
#include <string>

DWORD FindProcessId(const char* processName) {
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if (hSnapshot == INVALID_HANDLE_VALUE) {
        return 0;
    }
    
    PROCESSENTRY32 pe32;
    pe32.dwSize = sizeof(PROCESSENTRY32);
    
    if (Process32First(hSnapshot, &pe32)) {
        do {
            if (_stricmp(pe32.szExeFile, processName) == 0) {
                CloseHandle(hSnapshot);
                return pe32.th32ProcessID;
            }
        } while (Process32Next(hSnapshot, &pe32));
    }
    
    CloseHandle(hSnapshot);
    return 0;
}

bool InjectDLL(DWORD processId, const char* dllPath) {
    printf("[*] Opening process %d...\n", processId);
    
    HANDLE hProcess = OpenProcess(
        PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | 
        PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
        FALSE, processId
    );
    
    if (!hProcess) {
        printf("[-] Failed to open process! Error: %d\n", GetLastError());
        return false;
    }
    
    printf("[+] Process opened\n");
    
    size_t dllPathLen = strlen(dllPath) + 1;
    LPVOID pDllPath = VirtualAllocEx(
        hProcess, NULL, dllPathLen,
        MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE
    );
    
    if (!pDllPath) {
        printf("[-] Failed to allocate memory! Error: %d\n", GetLastError());
        CloseHandle(hProcess);
        return false;
    }
    
    printf("[+] Memory allocated at 0x%p\n", pDllPath);
    
    if (!WriteProcessMemory(hProcess, pDllPath, dllPath, dllPathLen, NULL)) {
        printf("[-] Failed to write memory! Error: %d\n", GetLastError());
        VirtualFreeEx(hProcess, pDllPath, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return false;
    }
    
    printf("[+] DLL path written\n");
    
    HMODULE hKernel32 = GetModuleHandleA("kernel32.dll");
    LPTHREAD_START_ROUTINE pLoadLibrary = (LPTHREAD_START_ROUTINE)GetProcAddress(hKernel32, "LoadLibraryA");
    
    if (!pLoadLibrary) {
        printf("[-] Failed to get LoadLibraryA address!\n");
        VirtualFreeEx(hProcess, pDllPath, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return false;
    }
    
    printf("[+] LoadLibraryA at 0x%p\n", pLoadLibrary);
    
    HANDLE hThread = CreateRemoteThread(
        hProcess, NULL, 0,
        pLoadLibrary, pDllPath,
        0, NULL
    );
    
    if (!hThread) {
        printf("[-] Failed to create remote thread! Error: %d\n", GetLastError());
        VirtualFreeEx(hProcess, pDllPath, 0, MEM_RELEASE);
        CloseHandle(hProcess);
        return false;
    }
    
    printf("[+] Remote thread created\n");
    printf("[*] Waiting for injection to complete...\n");
    
    WaitForSingleObject(hThread, INFINITE);
    
    DWORD exitCode;
    GetExitCodeThread(hThread, &exitCode);
    
    CloseHandle(hThread);
    VirtualFreeEx(hProcess, pDllPath, 0, MEM_RELEASE);
    CloseHandle(hProcess);
    
    if (exitCode == 0) {
        printf("[-] DLL injection failed!\n");
        return false;
    }
    
    printf("[+] DLL injected successfully! Module handle: 0x%08X\n", exitCode);
    return true;
}

bool LaunchWithDLL(const char* exePath, const char* dllPath) {
    printf("[*] Launching %s...\n", exePath);
    
    STARTUPINFOA si = { sizeof(si) };
    PROCESS_INFORMATION pi;
    
    if (!CreateProcessA(
        exePath, NULL, NULL, NULL, FALSE,
        CREATE_SUSPENDED, NULL, NULL, &si, &pi
    )) {
        printf("[-] Failed to create process! Error: %d\n", GetLastError());
        return false;
    }
    
    printf("[+] Process created (PID: %d)\n", pi.dwProcessId);
    printf("[*] Injecting DLL...\n");
    
    bool success = InjectDLL(pi.dwProcessId, dllPath);
    
    if (success) {
        printf("[+] DLL injected, resuming process...\n");
        ResumeThread(pi.hThread);
        printf("[+] Process started with patch!\n");
    } else {
        printf("[-] Injection failed, terminating process...\n");
        TerminateProcess(pi.hProcess, 1);
    }
    
    CloseHandle(pi.hThread);
    CloseHandle(pi.hProcess);
    
    return success;
}

int main(int argc, char* argv[]) {
    if (argc >= 3) {
        std::string mode = argv[1];
        
        if (mode == "launch" && argc >= 4) {
            // idm_injector.exe launch "path\to\IDMan.exe" "path\to\patch.dll"
            const char* exePath = argv[2];
            const char* dllPath = argv[3];
            
            if (LaunchWithDLL(exePath, dllPath)) {
                return 0;
            }
            return 1;
        }
        else if (mode == "inject") {
            
            DWORD pid = 0;
            const char* dllPath = NULL;
            
            if (argc >= 4) {
                pid = atoi(argv[2]);
                dllPath = argv[3];
            } else if (argc >= 3) {
                dllPath = argv[2];
                pid = FindProcessId("IDMan.exe");
            }
            
            if (pid == 0) {
                printf("[-] IDMan.exe not found!\n");
                return 1;
            }
            
            if (InjectDLL(pid, dllPath)) {
                return 0;
            }
            return 1;
        }
    }
    
    printf("========================================\n");
    printf("IDM DLL Injector\n");
    printf("========================================\n\n");
    
    char dllPath[MAX_PATH];
    GetFullPathNameA("idm_patch.dll", MAX_PATH, dllPath, NULL);
    
    if (GetFileAttributesA(dllPath) == INVALID_FILE_ATTRIBUTES) {
        printf("[-] DLL not found: %s\n", dllPath);
        printf("[-] Make sure idm_patch.dll is in the same directory!\n");
        return 1;
    }
    
    printf("[+] DLL found: %s\n\n", dllPath);
    
    printf("Select mode:\n");
    printf("1. Inject into running IDMan.exe\n");
    printf("2. Launch IDMan.exe with patch\n");
    printf("Choice: ");
    
    int choice;
    scanf("%d", &choice);
    
    if (choice == 1) {
        printf("\n[*] Searching for IDMan.exe...\n");
        DWORD pid = FindProcessId("IDMan.exe");
        
        if (pid == 0) {
            printf("[-] IDMan.exe not found!\n");
            printf("[-] Please start IDM first or use mode 2\n");
            return 1;
        }
        
        printf("[+] Found IDMan.exe (PID: %d)\n\n", pid);
        
        if (InjectDLL(pid, dllPath)) {
            printf("\n[+] SUCCESS! IDM is now patched!\n");
            printf("[+] Check %%TEMP%%\\idm_patch.log for details\n");
            return 0;
        } else {
            printf("\n[-] FAILED! Check errors above\n");
            return 1;
        }
    }
    else if (choice == 2) {
        const char* idmPath = "C:\\Program Files (x86)\\Internet Download Manager\\IDMan.exe";
        
        printf("\n[*] IDM path: %s\n", idmPath);
        printf("[*] Press Enter to continue...");
        getchar();
        getchar();
        
        if (LaunchWithDLL(idmPath, dllPath)) {
            printf("\n[+] SUCCESS! IDM started with patch!\n");
            printf("[+] Check %%TEMP%%\\idm_patch.log for details\n");
            return 0;
        } else {
            printf("\n[-] FAILED! Check errors above\n");
            return 1;
        }
    }
    else {
        printf("[-] Invalid choice!\n");
        return 1;
    }
}
