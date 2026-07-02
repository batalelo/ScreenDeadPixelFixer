# Screen Dead Pixel Fixer (Stuck & Broken Screen Pixel Workaround)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Platform: Windows](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)]()
[![Developed by: TakeYourSite](https://img.shields.io/badge/Developed%20by-TakeYourSite.com-orange.svg)](https://takeyoursite.com)

> **Idea & Development:** Developed and designed by [TakeYourSite.com](https://takeyoursite.com)

> [!TIP]
> **🚀 Quick Start (Fastest Solution):**
> You can download the pre-compiled, ready-to-run executable directly from the **[Latest Release (v1.0.0)](https://github.com/batalelo/ScreenDeadPixelFixer/releases/tag/v1.0.0)** and start using it instantly.
> 
> *Otherwise, if you want to build it yourself, please follow the compilation instructions detailed below.*

**Screen Dead Pixel Fixer** is an ultra-lightweight, zero-dependency Windows desktop utility designed to bypass the issue of dead, stuck, or broken screen pixels. Unlike traditional pixel repair tools that flash bright colors (which rarely work for physically dead pixels), this utility provides a smart, real-time visual workaround.

It allows you to define a custom rectangular **"Dead Zone"** on your monitor. Whenever your mouse cursor enters this zone, a circular magnifying **"Bubble Overlay"** pops up dynamically next to the cursor, rendering a real-time, click-through magnification of the content blocked by the dead pixels.

![Screen Dead Pixel Fixer Workaround Demo](demo.jpg)

---

## 🌟 Key Features & Capabilities

* **Smart Visual Workaround**: Instantly see text, buttons, and UI elements hidden behind dead or black screen regions.
* **Ultra-Lightweight Executable**: The compiled binary size is only **~27 KB** with an extremely low CPU and RAM overhead.
* **Zero Runtime Dependencies**: Targets `.NET Framework 4.8` (pre-installed natively on Windows 10 & 11). Run it instantly without installing any runtimes, libraries, or setups.
* **Interactive Screen Selection**: Click a single button to dim the screen and drag your mouse to select the exact region of the dead pixels.
* **Dynamic Cursor Rendering**: The magnifying bubble captures and draws the real Windows mouse cursor in real-time, matching its style (pointer, hand, text select) and hotspot location.
* **Fully Click-Through Overlay**: The magnifying bubble is completely transparent to clicks; you can click, drag, and interact with the windows behind the bubble normally.
* **Windows Auto-Start**: Easily register the application to launch automatically on Windows startup.

---

## 🛠️ File Structure & Architecture

The codebase is written in pure C# (WPF) without XAML files to keep the build process incredibly simple, modular, and transparent.

* **[App.cs](file:///d:/ScreenDeadPixelFixer/App.cs)**: The application entry point that initializes the WPF lifecycle and handles single-instance execution.
* **[MainWindow.cs](file:///d:/ScreenDeadPixelFixer/MainWindow.cs)**: The main dashboard UI. Designed with a clean, borderless, dark-themed control panel.
* **[SelectionWindow.cs](file:///d:/ScreenDeadPixelFixer/SelectionWindow.cs)**: An interactive, full-screen canvas that lets users visually drag-select their dead pixel zone.
* **[OverlayWindow.cs](file:///d:/ScreenDeadPixelFixer/OverlayWindow.cs)**: The click-through circular magnifying window that displays the captured screen content.
* **[ScreenDeadPixelFixerEngine.cs](file:///d:/ScreenDeadPixelFixer/ScreenDeadPixelFixerEngine.cs)**: The core engine that polls the mouse position, captures the screen under the dead zone, and triggers updates.
* **[NativeMethods.cs](file:///d:/ScreenDeadPixelFixer/NativeMethods.cs)**: Native Win32 API bindings (P/Invokes) used to achieve click-through functionality, mouse tracking, and desktop capture.

---

## 💻 How to Compile and Run Locally

You do not need an IDE like Visual Studio to compile this project. You can build it in 1 second using the built-in Windows C# compiler:

### Option 1: Automatic Batch Script
Simply double-click the **[run_screendeadpixelfixer.bat](file:///d:/ScreenDeadPixelFixer/run_screendeadpixelfixer.bat)** file. It automatically finds the Windows C# compiler (`csc.exe`), compiles the executable, and runs it.

### Option 2: Manual Command Line Compilation
Open Command Prompt (CMD) or PowerShell in the project directory and run:
```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:ScreenDeadPixelFixer.exe /win32icon:AppIcon.ico App.cs MainWindow.cs SelectionWindow.cs OverlayWindow.cs ScreenDeadPixelFixerEngine.cs NativeMethods.cs
```

---

## 🚀 Automated Builds via GitHub Actions (CI/CD)

To guarantee software integrity and build transparency for your users, you can compile the executable directly on GitHub's secure servers using **GitHub Actions**. This ensures that the downloaded binary exactly matches the open-source repository code.

### Step-by-Step Setup:

1. Create a directory structure in your repository: `.github/workflows/`
2. Create a new file named `build.yml` inside that directory.
3. Paste the following configuration into the file:

```yaml
name: Build and Release Executable

on:
  push:
    branches: [ main ]
  workflow_dispatch:

jobs:
  build:
    runs-on: windows-latest

    steps:
    - name: Checkout Repository
      uses: actions/checkout@v4

    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v2

    - name: Compile Application
      run: |
        C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:ScreenDeadPixelFixer.exe /win32icon:AppIcon.ico App.cs MainWindow.cs SelectionWindow.cs OverlayWindow.cs ScreenDeadPixelFixerEngine.cs NativeMethods.cs

    - name: Upload Build Artifact
      uses: actions/upload-artifact@v4
      with:
        name: ScreenDeadPixelFixer
        path: ScreenDeadPixelFixer.exe
```

Whenever you push a change to the `main` branch, GitHub will automatically compile the binary and upload it as a downloadable workflow artifact.

---

## 🛡️ Security, Transparency & Trust Verification

Since this application performs low-level actions like desktop screen capture and startup registration, users might be cautious. We provide multiple ways to verify security:

1. **Digital Fingerprinting (SHA-256 Checksum)**:
   Verify the downloaded executable's integrity by running this command in Windows PowerShell:
   ```powershell
   Get-FileHash ScreenDeadPixelFixer.exe -Algorithm SHA256
   ```
2. **Native Decompilation**:
   Because this is a standard .NET executable, users can open `ScreenDeadPixelFixer.exe` using decompilers like **dnSpy** or **ILSpy** to read the exact C# code running on their machines.
3. **Local Compilation**:
   Anyone can download the raw `.cs` files and compile them locally in seconds using `csc.exe`, guaranteeing no malicious code is introduced in pre-built releases.

*Project idea, design, and code developed by [TakeYourSite.com](https://takeyoursite.com).*
