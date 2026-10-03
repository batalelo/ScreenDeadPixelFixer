# Black Circle Fixer

### Smart Workaround for Black Circle (Black Spot) on Laptop Screens

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Platform: Windows](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)]()
[![Developed by: TakeYourSite](https://img.shields.io/badge/Developed%20by-TakeYourSite.com-orange.svg)](https://takeyoursite.com)

> **Idea & Development:** Developed and designed by [TakeYourSite.com](https://takeyoursite.com)

> [!TIP]
> **🚀 Quick Start (Fastest Solution):**
> You can download the pre-compiled, ready-to-run executable directly from the **[Latest Release (v1.0.0)](https://github.com/batalelo/ScreenDeadPixelFixer/releases/tag/v1.0.0)** and start using it instantly.
> 
> *Otherwise, if you want to build it yourself, please follow the compilation instructions detailed below.*

---

## 🎯 What is Black Circle Fixer?

**Black Circle Fixer** is an ultra-lightweight, zero-dependency Windows desktop utility designed as a **smart visual workaround** for laptops and monitors suffering from a **black circle, black spot, LCD bleed, or physical screen bruise**.

### The Problem:
When a laptop screen suffers physical impact or pressure (such as closing the lid on a pen, earbud, or cable), the inner LCD glass substrate cracks, causing liquid crystals to leak into a permanent **black circle** or **black spot**. 
* **Traditional "Dead Pixel Fixer" tools fail completely:** Flashing RGB colors (like JScreenFix) only works for tiny microscopic stuck pixels—it cannot repair physically cracked LCD panels.
* **Screen replacement is expensive:** Replacing an entire laptop display panel often costs between **$150 and $300+**.

### The Solution:
**Black Circle Fixer** does not attempt impossible physical glass repairs; instead, it provides a **smart, real-time visual workaround**:
1. You drag and select the rectangular **"Black Circle / Dead Zone"** covering your damaged screen area.
2. Whenever your mouse cursor enters this zone, a circular magnifying **"Bubble Overlay"** pops up dynamically right next to the damaged area.
3. The bubble displays a real-time, **click-through magnification** of whatever is hidden behind the black circle (buttons, dialog boxes, text, taskbar icons).
4. You can see your live mouse cursor inside the bubble and click, scroll, drag, and interact normally!

![Black Circle Fixer Workaround Demo](demo.jpg)

---

## 🌟 Key Features & Capabilities

* **Smart Visual Workaround**: Instantly see text, buttons, and UI elements hidden behind black circles or damaged screen regions without replacing the screen.
* **Ultra-Lightweight Executable**: The compiled binary size is only **~29 KB** with virtually zero CPU and RAM overhead.
* **Zero Runtime Dependencies**: Built with native C# targeting `.NET Framework 4.8` (pre-installed natively on Windows 10 & 11). Run it instantly without installing any runtimes or setups.
* **Interactive Screen Selection**: Click a single button to dim the screen and drag your mouse to select the exact boundary of your black circle or dead zone.
* **Live Click-Through Overlay**: The magnifying bubble is completely transparent to Windows clicks (`WS_EX_TRANSPARENT`), allowing you to click, drag, and interact with the windows behind the bubble normally.
* **Dynamic Cursor Rendering**: Accurately tracks and draws the real Windows mouse cursor in real-time inside the magnifying bubble, matching its current style (pointer, hand, text select) and hotspot location.
* **Windows Auto-Start**: Easily register the application to launch automatically with Windows on startup.
* **System Tray Minimization**: Runs silently in the background tray with instant activation.

---

## 🛠️ File Structure & Architecture

The codebase is written in pure C# (WPF) without XAML files to keep the build process incredibly simple, modular, and transparent.

* **[App.cs](file:///d:/ScreenDeadPixelFixer/App.cs)**: The application entry point that initializes the WPF lifecycle and handles single-instance execution.
* **[MainWindow.cs](file:///d:/ScreenDeadPixelFixer/MainWindow.cs)**: The main dashboard UI. Designed with a clean, borderless, dark-themed control panel.
* **[SelectionWindow.cs](file:///d:/ScreenDeadPixelFixer/SelectionWindow.cs)**: An interactive, full-screen canvas that lets users visually drag-select their black circle / dead zone.
* **[OverlayWindow.cs](file:///d:/ScreenDeadPixelFixer/OverlayWindow.cs)**: The click-through circular magnifying window that displays the captured screen content.
* **[BlackCircleFixerEngine.cs](file:///d:/ScreenDeadPixelFixer/BlackCircleFixerEngine.cs)**: The core engine that polls the mouse position, captures the screen under the dead zone, and triggers real-time updates.
* **[NativeMethods.cs](file:///d:/ScreenDeadPixelFixer/NativeMethods.cs)**: Native Win32 API bindings (P/Invokes) used to achieve click-through functionality, mouse tracking, and desktop capture.

---

## 💻 How to Compile and Run Locally

You do not need an IDE like Visual Studio to compile this project. You can build it in 1 second using the built-in Windows C# compiler:

### Option 1: Automatic Batch Script
Simply double-click the **[run_blackcirclefixer.bat](file:///d:/ScreenDeadPixelFixer/run_blackcirclefixer.bat)** file. It automatically finds the Windows C# compiler (`csc.exe`), compiles the executable, and runs it.

### Option 2: Manual Command Line Compilation
Open Command Prompt (CMD) or PowerShell in the project directory and run:
```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:BlackCircleFixer.exe /win32icon:icon.ico /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xaml.dll /r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\WindowsBase.dll /r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationCore.dll /r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationFramework.dll /r:System.Core.dll App.cs MainWindow.cs OverlayWindow.cs SelectionWindow.cs NativeMethods.cs BlackCircleFixerEngine.cs
```

---

## 🛒 Microsoft Store Distribution (MSIX)

This application is fully structured and configured for seamless distribution through the **Microsoft Store (Partner Center)** using modern **MSIX Desktop Bridge**.

* **Ultra-Lightweight**: Packaged size is only **~80 KB** (no bundled runtimes needed).
* **Automatic Cloud Builds**: GitHub Actions compiles and packages the `.msix` container automatically on every push.
* **Store Guide**: For complete step-by-step submission instructions, see **[STORE_GUIDE.md](file:///d:/ScreenDeadPixelFixer/STORE_GUIDE.md)**.

---

## 🚀 Automated Builds via GitHub Actions (CI/CD)

The repository includes a ready-to-run GitHub Actions workflow in **`.github/workflows/package-msix.yml`**. On every commit, it automatically:
1. Compiles `BlackCircleFixer.exe`.
2. Generates all required Store logo assets via `generate_assets.ps1`.
3. Packages the app into `BlackCircleFixer.msix` using Microsoft's `MakeAppx.exe`.
4. Uploads both the **Standalone EXE** and **Microsoft Store MSIX** as downloadable artifacts.

---

## 🛡️ Security, Transparency & Trust Verification

Since this application performs low-level actions like desktop screen capture and startup registration, users might be cautious. We provide multiple ways to verify security:

1. **Digital Fingerprinting (SHA-256 Checksum)**:
   Verify the downloaded executable's integrity by running this command in Windows PowerShell:
   ```powershell
   Get-FileHash BlackCircleFixer.exe -Algorithm SHA256
   ```
2. **Native Decompilation**:
   Because this is a standard .NET executable, users can open `BlackCircleFixer.exe` using decompilers like **dnSpy** or **ILSpy** to read the exact C# code running on their machines.
3. **Local Compilation**:
   Anyone can download the raw `.cs` files and compile them locally in seconds using `csc.exe`, guaranteeing no malicious code is introduced in pre-built releases.

---

## ❓ Frequently Asked Questions (FAQ)

### Can software fix a physical black circle or ink spot on an LCD screen?
No software can physically repair broken glass or reverse leaked liquid crystals. However, **BlackCircleFixer** solves the daily usability problem by projecting the blocked content onto a floating, magnifying bubble whenever your cursor is in the dead zone, allowing you to read text, click buttons, and use your laptop without spending hundreds on a new screen.

### Will this slow down my laptop?
No. The application is written in lightweight native C# and consumes less than 1% CPU and ~40 MB of RAM only when actively magnifying. When your cursor is outside the dead zone, it sits idle.

---

*Project idea, design, and code developed by [TakeYourSite.com](https://takeyoursite.com).*
