# Build Instructions

## Prerequisites

### Required Software

1. **Visual Studio 2022 or later**
   - Workload: "Desktop development with C++"
   - Workload: ".NET desktop development"
   - Component: Windows 10/11 SDK

2. **.NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0

3. **Git** (optional, for cloning)

### Verify Installation

```cmd
# Check Visual Studio C++ compiler
cl

# Check .NET SDK
dotnet --version
```

## Build Steps

### 1. Clone Repository

```cmd
git clone https://github.com/yourusername/idm-patcher.git
cd idm-patcher
```

### 2. Compile Native Components

Run the build script to compile all C++ components (DLL, injector, launcher, utilities):

```cmd
compile_final.bat
```

This will create:
- `idm_patch.dll` - Main patch DLL (32-bit)
- `idm_injector.exe` - DLL injector (32-bit)
- `IDMLauncher.exe` - Launcher replacement (32-bit)
- `idm_signal_unload.exe` - Unload utility (32-bit)

### 3. Copy Files to Installer

```cmd
copy idm_patch.dll Installer\
copy idm_injector.exe Installer\
copy IDMLauncher.exe Installer\
copy idm_signal_unload.exe Installer\
```

### 4. Build Installer

```cmd
dotnet publish Installer\IDMPatcherInstaller.csproj -c Release -r win-x64 --self-contained -o Release
```

The final installer will be at: `Release\IDMPatcherInstaller.exe`

## Manual Compilation

### Compile DLL Only

```cmd
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars32.bat"
cl /LD /O2 /MD idm_patch_final.cpp advapi32.lib user32.lib /link /OUT:idm_patch.dll
```

### Compile Injector

```cmd
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars32.bat"
cl /O2 /MD idm_injector.cpp /link /OUT:idm_injector.exe
```

### Compile Launcher

```cmd
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars32.bat"
cl /O2 /MD IDMLauncher.cpp /link /OUT:IDMLauncher.exe
```

### Compile Signal Utility

```cmd
call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars32.bat"
cl /O2 /MD idm_signal_unload.cpp /link /OUT:idm_signal_unload.exe
```

## Troubleshooting

### "cl is not recognized"

Visual Studio environment not set up. Run:

```cmd
"C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars32.bat"
```

Adjust path if using different VS version or edition (Professional, Enterprise).

### "dotnet is not recognized"

.NET SDK not installed or not in PATH. Download from:
https://dotnet.microsoft.com/download/dotnet/8.0

### Build Errors

1. Check Visual Studio installation includes C++ and C# workloads
2. Verify Windows SDK is installed
3. Ensure .NET 8.0 SDK is installed
4. Run Visual Studio Installer to repair/modify installation

## Clean Build

To clean all build artifacts:

```cmd
del *.obj *.dll *.exe *.lib *.exp *.pdb
rmdir /s /q Release
rmdir /s /q Installer\bin
rmdir /s /q Installer\obj
```

Then rebuild from step 2.

## Architecture Notes

All native components (DLL, injector, launcher) must be compiled as **32-bit** because IDM is a 32-bit application. The installer can be 64-bit as it runs separately.

## Release Checklist

Before creating a release:

1. ✅ Clean build all components
2. ✅ Test installer on clean Windows installation
3. ✅ Verify IDM launches correctly after patching
4. ✅ Test uninstallation process
5. ✅ Update version numbers in code
6. ✅ Update CHANGELOG.md
7. ✅ Create Git tag
8. ✅ Build release installer
9. ✅ Test release installer
10. ✅ Create GitHub release with installer attached
