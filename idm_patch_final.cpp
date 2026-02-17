#include <windows.h>
#include <stdio.h>

FILE* g_logFile = NULL;
HMODULE g_hModule = NULL;
HANDLE g_hUnloadEvent = NULL;
HANDLE g_hUnloadThread = NULL;

void Log(const char* format, ...) {
    if (!g_logFile) {
        char path[MAX_PATH];
        GetTempPathA(MAX_PATH, path);
        strcat_s(path, "idm_patch.log");
        fopen_s(&g_logFile, path, "a");
    }
    if (g_logFile) {
        SYSTEMTIME st;
        GetLocalTime(&st);
        fprintf(g_logFile, "[%02d:%02d:%02d] ", st.wHour, st.wMinute, st.wSecond);
        
        va_list args;
        va_start(args, format);
        vfprintf(g_logFile, format, args);
        va_end(args);
        fprintf(g_logFile, "\n");
        fflush(g_logFile);
    }
}

typedef int (WINAPI *MessageBoxAFunc)(HWND, LPCSTR, LPCSTR, UINT);
typedef int (WINAPI *MessageBoxWFunc)(HWND, LPCWSTR, LPCWSTR, UINT);
typedef void (WINAPI *ExitProcessFunc)(UINT);
typedef LONG (WINAPI *RegQueryValueExAFunc)(HKEY, LPCSTR, LPDWORD, LPDWORD, LPBYTE, LPDWORD);

MessageBoxAFunc OriginalMessageBoxA = NULL;
MessageBoxWFunc OriginalMessageBoxW = NULL;
ExitProcessFunc OriginalExitProcess = NULL;
RegQueryValueExAFunc OriginalRegQueryValueExA = NULL;

int WINAPI MessageBoxA_Hook(HWND hWnd, LPCSTR lpText, LPCSTR lpCaption, UINT uType) {
    if (lpText) {
        if (strstr(lpText, "Trial") || strstr(lpText, "trial") ||
            strstr(lpText, "register") || strstr(lpText, "Register") ||
            strstr(lpText, "Serial") || strstr(lpText, "serial") ||
            strstr(lpText, "expired") || strstr(lpText, "Expired") ||
            strstr(lpText, "days left") || strstr(lpText, "counterfeit") ||
            strstr(lpText, "30 дней") || strstr(lpText, "30 days") ||
            strstr(lpText, "Демонстрационный") || strstr(lpText, "закрывается") ||
            strstr(lpText, "зарегистрирован") || strstr(lpText, "деинсталлировать") ||
            strstr(lpText, "осталось") || strstr(lpText, "дня") || strstr(lpText, "дней") ||
            strstr(lpText, "зарегистрировать") || strstr(lpText, "копию IDM") ||
            strstr(lpText, "Internet Download Manager") ||
            strstr(lpText, "использования") || strstr(lpText, "хотите зарегистрировать") ||
            strstr(lpText, "fake") || strstr(lpText, "Fake") || strstr(lpText, "FAKE") ||
            strstr(lpText, "cracked") || strstr(lpText, "Cracked") ||
            strstr(lpText, "registered with") || strstr(lpText, "has been registered") ||
            strstr(lpText, "purchase") || strstr(lpText, "Purchase")) {
            
            Log("MessageBoxA blocked: %.80s", lpText);
            
            if (uType & MB_YESNO) return IDNO;
            if (uType & MB_OKCANCEL) return IDCANCEL;
            return IDOK;
        }
    }
    
    return MessageBoxA(hWnd, lpText, lpCaption, uType);
}

int WINAPI MessageBoxW_Hook(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, UINT uType) {
    if (lpText) {
        if (wcsstr(lpText, L"Trial") || wcsstr(lpText, L"trial") ||
            wcsstr(lpText, L"register") || wcsstr(lpText, L"Register") ||
            wcsstr(lpText, L"Serial") || wcsstr(lpText, L"serial") ||
            wcsstr(lpText, L"expired") || wcsstr(lpText, L"Expired") ||
            wcsstr(lpText, L"days left") || wcsstr(lpText, L"counterfeit") ||
            wcsstr(lpText, L"30 дней") || wcsstr(lpText, L"30 days") ||
            wcsstr(lpText, L"Демонстрационный") || wcsstr(lpText, L"закрывается") ||
            wcsstr(lpText, L"зарегистрирован") || wcsstr(lpText, L"деинсталлировать") ||
            wcsstr(lpText, L"осталось") || wcsstr(lpText, L"дня") || wcsstr(lpText, L"дней") ||
            wcsstr(lpText, L"зарегистрировать") || wcsstr(lpText, L"копию IDM") ||
            wcsstr(lpText, L"Internet Download Manager") ||
            wcsstr(lpText, L"использования") || wcsstr(lpText, L"хотите зарегистрировать") ||
            wcsstr(lpText, L"fake") || wcsstr(lpText, L"Fake") || wcsstr(lpText, L"FAKE") ||
            wcsstr(lpText, L"cracked") || wcsstr(lpText, L"Cracked") ||
            wcsstr(lpText, L"registered with") || wcsstr(lpText, L"has been registered") ||
            wcsstr(lpText, L"purchase") || wcsstr(lpText, L"Purchase")) {
            
            Log("MessageBoxW blocked: %.80S", lpText);
            
            if (uType & MB_YESNO) return IDNO;
            if (uType & MB_OKCANCEL) return IDCANCEL;
            return IDOK;
        }
    }
    
    return MessageBoxW(hWnd, lpText, lpCaption, uType);
}

void WINAPI ExitProcess_Hook(UINT uExitCode) {
    Log("ExitProcess called (code: %d) - allowing exit", uExitCode);
    if (OriginalExitProcess) {
        OriginalExitProcess(uExitCode);
    }
}

LONG WINAPI RegQueryValueExA_Hook(
    HKEY hKey, LPCSTR lpValueName, LPDWORD lpReserved,
    LPDWORD lpType, LPBYTE lpData, LPDWORD lpcbData
) {
    if (lpValueName && strcmp(lpValueName, "MData") == 0) {
        Log("RegQueryValueExA: MData blocked");
        return ERROR_FILE_NOT_FOUND;
    }
    
    return RegQueryValueExA(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);
}

bool HookIAT(HMODULE hModule, const char* dllName, const char* funcName, void* hookFunc, void** originalFunc) {
    PIMAGE_DOS_HEADER pDosHeader = (PIMAGE_DOS_HEADER)hModule;
    if (pDosHeader->e_magic != IMAGE_DOS_SIGNATURE) return false;
    
    PIMAGE_NT_HEADERS pNtHeaders = (PIMAGE_NT_HEADERS)((BYTE*)hModule + pDosHeader->e_lfanew);
    if (pNtHeaders->Signature != IMAGE_NT_SIGNATURE) return false;
    
    PIMAGE_IMPORT_DESCRIPTOR pImportDesc = (PIMAGE_IMPORT_DESCRIPTOR)((BYTE*)hModule + 
        pNtHeaders->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);
    
    while (pImportDesc->Name) {
        char* importDllName = (char*)((BYTE*)hModule + pImportDesc->Name);
        
        if (_stricmp(importDllName, dllName) == 0) {
            PIMAGE_THUNK_DATA pThunk = (PIMAGE_THUNK_DATA)((BYTE*)hModule + pImportDesc->FirstThunk);
            PIMAGE_THUNK_DATA pOrigThunk = (PIMAGE_THUNK_DATA)((BYTE*)hModule + pImportDesc->OriginalFirstThunk);
            
            while (pThunk->u1.Function) {
                if (!(pOrigThunk->u1.Ordinal & IMAGE_ORDINAL_FLAG)) {
                    PIMAGE_IMPORT_BY_NAME pImport = (PIMAGE_IMPORT_BY_NAME)((BYTE*)hModule + pOrigThunk->u1.AddressOfData);
                    
                    if (strcmp((char*)pImport->Name, funcName) == 0) {
                        DWORD oldProtect;
                        VirtualProtect(&pThunk->u1.Function, sizeof(DWORD), PAGE_READWRITE, &oldProtect);
                        
                        *originalFunc = (void*)pThunk->u1.Function;
                        pThunk->u1.Function = (DWORD)hookFunc;
                        
                        VirtualProtect(&pThunk->u1.Function, sizeof(DWORD), oldProtect, &oldProtect);
                        return true;
                    }
                }
                
                pThunk++;
                pOrigThunk++;
            }
        }
        
        pImportDesc++;
    }
    
    return false;
}

void InstallMessageHooks() {
    Log("Installing message hooks...");
    
    HMODULE hIDM = GetModuleHandle(NULL);
    
    void* origMsgBoxA = NULL;
    void* origMsgBoxW = NULL;
    void* origExit = NULL;
    void* origReg = NULL;
    
    if (HookIAT(hIDM, "user32.dll", "MessageBoxA", (void*)MessageBoxA_Hook, &origMsgBoxA)) {
        Log("IAT: MessageBoxA hooked");
        OriginalMessageBoxA = (MessageBoxAFunc)origMsgBoxA;
    }
    
    if (HookIAT(hIDM, "user32.dll", "MessageBoxW", (void*)MessageBoxW_Hook, &origMsgBoxW)) {
        Log("IAT: MessageBoxW hooked");
        OriginalMessageBoxW = (MessageBoxWFunc)origMsgBoxW;
    }
    
    if (HookIAT(hIDM, "kernel32.dll", "ExitProcess", (void*)ExitProcess_Hook, &origExit)) {
        Log("IAT: ExitProcess hooked");
        OriginalExitProcess = (ExitProcessFunc)origExit;
    }
    
    if (HookIAT(hIDM, "advapi32.dll", "RegQueryValueExA", (void*)RegQueryValueExA_Hook, &origReg)) {
        Log("IAT: RegQueryValueExA hooked");
        OriginalRegQueryValueExA = (RegQueryValueExAFunc)origReg;
    }
    
    Log("Message hooks installed!");
}

const char* g_clsidKeys[] = {
    "{6DDF00DB-1234-46EC-8356-27E7B2051192}",
    "{7B8E9164-324D-4A2E-A46D-0165FB2000EC}",
    "{D5B91409-A8CA-4973-9A0B-59F713D25671}",
    "{5ED60779-4DE2-4E07-B862-974CA4FF2E9C}",
    "{07999AC3-058B-40BF-984F-69EB1E554CA7}",
    NULL
};

LONG DeleteRegistryKey(HKEY hKeyRoot, const char* subKey) {
    HKEY hKey;
    LONG result = RegOpenKeyExA(hKeyRoot, subKey, 0, KEY_ALL_ACCESS, &hKey);
    
    if (result != ERROR_SUCCESS) return result;
    
    char keyName[256];
    DWORD keyNameSize;
    
    while (true) {
        keyNameSize = sizeof(keyName);
        result = RegEnumKeyExA(hKey, 0, keyName, &keyNameSize, NULL, NULL, NULL, NULL);
        
        if (result == ERROR_NO_MORE_ITEMS) break;
        
        if (result == ERROR_SUCCESS) {
            char fullPath[512];
            sprintf_s(fullPath, "%s\\%s", subKey, keyName);
            DeleteRegistryKey(hKeyRoot, fullPath);
        }
    }
    
    RegCloseKey(hKey);
    return RegDeleteKeyA(hKeyRoot, subKey);
}

void DeleteRegistryValue(HKEY hKeyRoot, const char* subKey, const char* valueName) {
    HKEY hKey;
    if (RegOpenKeyExA(hKeyRoot, subKey, 0, KEY_SET_VALUE, &hKey) == ERROR_SUCCESS) {
        RegDeleteValueA(hKey, valueName);
        RegCloseKey(hKey);
    }
}

void ResetTrial() {
    Log("Starting trial reset...");
    
    for (int i = 0; g_clsidKeys[i] != NULL; i++) {
        char keyPath[512];
        
        sprintf_s(keyPath, "Software\\Classes\\CLSID\\%s", g_clsidKeys[i]);
        DeleteRegistryKey(HKEY_CURRENT_USER, keyPath);
        
        sprintf_s(keyPath, "Software\\Classes\\Wow6432Node\\CLSID\\%s", g_clsidKeys[i]);
        DeleteRegistryKey(HKEY_CURRENT_USER, keyPath);
        
        sprintf_s(keyPath, "Software\\Classes\\CLSID\\%s", g_clsidKeys[i]);
        DeleteRegistryKey(HKEY_LOCAL_MACHINE, keyPath);
        
        sprintf_s(keyPath, "Software\\Classes\\Wow6432Node\\CLSID\\%s", g_clsidKeys[i]);
        DeleteRegistryKey(HKEY_LOCAL_MACHINE, keyPath);
    }
    
    DeleteRegistryValue(HKEY_CURRENT_USER, "Software\\DownloadManager", "FName");
    DeleteRegistryValue(HKEY_CURRENT_USER, "Software\\DownloadManager", "LName");
    DeleteRegistryValue(HKEY_CURRENT_USER, "Software\\DownloadManager", "Email");
    DeleteRegistryValue(HKEY_CURRENT_USER, "Software\\DownloadManager", "Serial");
    
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Internet Download Manager", "FName");
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Internet Download Manager", "LName");
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Internet Download Manager", "Email");
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Internet Download Manager", "Serial");
    
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Wow6432Node\\Internet Download Manager", "FName");
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Wow6432Node\\Internet Download Manager", "LName");
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Wow6432Node\\Internet Download Manager", "Email");
    DeleteRegistryValue(HKEY_LOCAL_MACHINE, "Software\\Wow6432Node\\Internet Download Manager", "Serial");
    
    Log("Trial reset complete!");
}

DWORD WINAPI UnloadThreadProc(LPVOID lpParameter) {
    Log("Unload thread started, waiting for signal...");
    
    WaitForSingleObject(g_hUnloadEvent, INFINITE);
    Log("Unload signal received, unloading DLL...");
    
    if (g_logFile) {
        fclose(g_logFile);
        g_logFile = NULL;
    }
    
    FreeLibraryAndExitThread(g_hModule, 0);
    return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID lpReserved) {
    switch (reason) {
        case DLL_PROCESS_ATTACH:
            DisableThreadLibraryCalls(hModule);
            g_hModule = hModule;
            
            Log("IDM Patch DLL loaded");
            
            g_hUnloadEvent = CreateEventA(NULL, TRUE, FALSE, "Global\\IDMPatchUnloadEvent");
            if (g_hUnloadEvent) {
                g_hUnloadThread = CreateThread(NULL, 0, UnloadThreadProc, NULL, 0, NULL);
            }
            
            ResetTrial();
            InstallMessageHooks();
            break;
            
        case DLL_PROCESS_DETACH:
            if (g_hUnloadEvent) CloseHandle(g_hUnloadEvent);
            if (g_hUnloadThread) CloseHandle(g_hUnloadThread);
            if (g_logFile) {
                fclose(g_logFile);
                g_logFile = NULL;
            }
            break;
    }
    
    return TRUE;
}
