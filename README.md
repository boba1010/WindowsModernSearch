# WindowsModernSearch

A lightweight, native **WinUI 3** replacement for the redesigned Windows 11 Search & Run experience—built to run silently in the system tray, intercept `Win + S` and `Win + Q` globally, and launch commands, apps, URIs, and paths with zero web bloat.

---

## Features

- **Native Windows 11 Fluent UI**: Built with WinUI 3 (`Mica` / `DesktopAcrylic` backdrop, borderless `OverlappedPresenter`, DWM rounded corners, and taskbar-docked positioning).
- **Global Shortcut Interception (`Win + S` / `Win + Q`)**: Lightweight `WH_KEYBOARD_LL` hook with `VK_F24` modifier suppression so Windows Search never flashes or steals focus.
- **Zero-Footprint Tray Residency**: Starts hidden in the system tray via `H.NotifyIcon.WinUI`, wakes up on the first keypress, and automatically hides when losing focus or pressing `Esc`.
- **Multi-Monitor & Taskbar Aware**: Dynamically docks above the active monitor's taskbar work area using `DisplayArea`.
- **Clean Input Experience**: Custom-styled borderless search box with native Run/Open action pill.

---

## Tech Stack

- **.NET 8 / .NET 9** + **WinUI 3 (Windows App SDK)**
- **Win32 Interop (`user32.dll` / `kernel32.dll`)**: Low-level keyboard hook (`WH_KEYBOARD_LL`), foreground focus management, and `AppWindow` positioning
- **[`H.NotifyIcon.WinUI`](https://github.com/HavenDV/H.NotifyIcon)**: System tray integration

---

## Getting Started

### Prerequisites

- Windows 11 (Build `22621` or newer)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with the **Windows application development** workload
- [.NET SDK](https://dotnet.microsoft.com/download) & **Windows App SDK**

### Build & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/WindowsModernSearch.git
   cd WindowsModernSearch
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build and launch**
   ```bash
   dotnet build -c Release
   ```
   Run the compiled `WindowsModernSearch.exe` (or press `F5` in Visual Studio). The app will start minimized to the system tray.

---

## Usage

| Action | Behavior |
| :--- | :--- |
| **`Win + S`** or **`Win + Q`** | Opens `WindowsModernSearch` docked above the taskbar |
| **`Enter`** or **`Open`** | Executes the entered command, path, URL, or `ms-settings:` / `shell:` URI |
| **`Esc`** or **Click Outside** | Hides the flyout back to the system tray |
| **Tray Icon -> Right-Click** | Open the search flyout or exit the background process |

---

## Roadmap

- [ ] `RunMRU` registry history sync (`HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU`)
- [ ] Live autocomplete for `PATH` executables and Start Menu `.lnk` shortcuts
- [ ] Native high-DPI icon extraction (`SHGetFileInfo`)
- [ ] `Ctrl + Shift + Enter` elevation ("Run as Administrator")
- [ ] Optional Taskbar Search pill UI Automation (`IUIAutomation`) click interception

---

## License

MIT License — see [LICENSE](LICENSE.txt) for details.