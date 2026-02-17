<div align="center">

# 🚀 IDM Patcher

**Automatic trial reset and license bypass for Internet Download Manager**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)

[Download Latest Release](https://github.com/Davidonchik/idm-patcher/releases) • [Report Bug](https://github.com/Davidonchik/idm-patcher/issues) • [Buy Me a Coffee ☕](https://buymeacoffee.com/davidonchik)

**English** | [Русский](README.ru.md)

</div>

---

## ✨ Features

- 🔄 **Auto Trial Reset** - Resets trial period on every IDM launch
- 🚫 **Message Blocking** - Blocks all registration and trial prompts
- 🔓 **License Bypass** - Download without restrictions or fake serial warnings
- 🎯 **Transparent** - Works through original IDM shortcuts
- 🌍 **Multilingual** - Installer supports 6 languages (EN/RU/DE/ES/FR/ZH)
- 🎨 **Seamless** - Uses original IDM icon, completely invisible to user

## 📥 Installation

1. **Download** the latest `IDMPatcherInstaller.exe` from [Releases](https://github.com/yourusername/idm-patcher/releases)
2. **Run as Administrator**
3. **Click Install**
4. **Launch IDM** normally - patch applies automatically!

> The installer auto-detects IDM location or lets you browse manually.

## 🛠️ How It Works

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│   Launcher  │ ───> │   Injector   │ ───> │  Patch DLL  │
│ (IDMan.exe) │      │ (DLL inject) │      │ (IAT hooks) │
└─────────────┘      └──────────────┘      └─────────────┘
                                                    │
                                                    ▼
                                            ┌───────────────┐
                                            │ • Reset trial │
                                            │ • Block msgs  │
                                            │ • Bypass lic  │
                                            └───────────────┘
```

**Technical Details:**
- **DLL Injection** via `CreateRemoteThread` + `LoadLibrary`
- **IAT Hooking** for `MessageBoxA/W`, `RegQueryValueExA`, `ExitProcess`
- **Registry Cleanup** removes trial tracking keys on every launch
- **Launcher Replacement** makes patching automatic and transparent

## 🔧 Building from Source

### Prerequisites
- Visual Studio 2022+ (C++ and C# workloads)
- .NET 8.0 SDK
- Windows SDK

### Build Steps

```cmd
# 1. Compile native components (DLL, injector, launcher)
compile_final.bat

# 2. Copy to installer directory
copy *.dll Installer\
copy *.exe Installer\

# 3. Build installer
dotnet publish Installer\IDMPatcherInstaller.csproj -c Release -r win-x64 --self-contained -o Release
```

Output: `Release\IDMPatcherInstaller.exe`

See [BUILD.md](BUILD.md) for detailed instructions.

## 🗑️ Uninstallation

1. Navigate to IDM installation directory
2. Delete `IDMan.exe`, `idm_injector.exe`, `idm_patch.dll`
3. Rename `IDMan_original.exe` → `IDMan.exe`
4. Delete `C:\ProgramData\IDM_Patcher`

## ❓ FAQ

<details>
<summary><b>Does this work with the latest IDM version?</b></summary>
Yes! The patcher uses generic hooking techniques that work across IDM versions.
</details>

<details>
<summary><b>Is this safe to use?</b></summary>
The patcher only modifies IDM's behavior locally. No network activity, no data collection. Open source for transparency.
</details>

<details>
<summary><b>Why does antivirus flag this?</b></summary>
DLL injection and IAT hooking are legitimate techniques but often flagged as suspicious. The code is open source - review it yourself!
</details>

<details>
<summary><b>Can I use this commercially?</b></summary>
This is for educational purposes. Please support IDM developers by purchasing a license for commercial use.
</details>

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| "Access denied" during install | Close IDM completely (tray icon → Exit), run installer as Admin |
| IDM doesn't start | Check if `IDMan_original.exe` exists in IDM directory |
| Messages still appear | Restart IDM, check `%TEMP%\idm_patch.log` for errors |

## 📜 License

MIT License - see [LICENSE](LICENSE) file.

## ⚠️ Disclaimer

This project is for **educational purposes only**. Users are responsible for complying with Internet Download Manager's license terms and applicable laws. Please support the developers by purchasing a legitimate license.

---

<div align="center">

**Enjoying this project?**

[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-Support-orange?style=for-the-badge&logo=buy-me-a-coffee)](https://buymeacoffee.com/davidonchik)

Made with ❤️

</div>
