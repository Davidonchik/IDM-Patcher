@echo off
echo ========================================
echo COMPILING FINAL IDM PATCH (32-bit)
echo ========================================

REM Setup Visual Studio environment for 32-bit
call "C:\Program Files\Microsoft Visual Studio\18\Community\VC\Auxiliary\Build\vcvars32.bat"

echo.
echo [1/4] Compiling DLL with self-unload...
cl /LD /O2 /MD idm_patch_final.cpp advapi32.lib user32.lib /link /OUT:idm_patch.dll

echo.
echo [2/4] Compiling signal utility...
cl /O2 /MD idm_signal_unload.cpp /link /OUT:idm_signal_unload.exe

echo.
echo [3/4] Compiling injector...
cl /O2 /MD idm_injector.cpp /link /OUT:idm_injector.exe

echo.
echo [4/4] Compiling launcher (GUI mode)...
cl /O2 /MD IDMLauncher.cpp user32.lib /link /SUBSYSTEM:WINDOWS /OUT:IDMLauncher.exe

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo SUCCESS! All files compiled
    echo ========================================
    dir idm_patch.dll idm_signal_unload.exe idm_injector.exe IDMLauncher.exe
) else (
    echo.
    echo ========================================
    echo ERROR: Compilation failed!
    echo ========================================
)

echo.
pause
