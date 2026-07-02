# Screen Dead Pixel Fixer

Screen Dead Pixel Fixer is an ultra-lightweight, low-overhead WPF utility that helps users with dead or broken screen pixels. It allows defining a rectangular "Dead Zone". When the mouse cursor enters this zone, a circular magnifying "Bubble" overlay pops up next to the cursor, displaying a real-time, click-through screen capture of the content hidden under the dead zone.

## Key Features

- **Ultra-lightweight File Size**: The compiled executable is only **~27 KB**!
- **Zero-dependency Portability**: Targets `.NET Framework 4.8` (built-in natively on Windows 10 & 11). Requires absolutely no runtimes or software installations.
- **Interactive Region Selection**: Click the `SELECT DEAD ZONE ON SCREEN` button to dim the screen and select the exact dead pixels area using your mouse.
- **Dynamic Cursor Rendering**: The overlay captures and draws the actual Windows mouse cursor in real-time, matching its style and correct hotspot position.
- **Ultra-Minimal UI Dashboard**: Clean, modern, borderless layout with custom close (`X`) and minimize (`_`) controls.
- **Pre-activated Windows Auto-Start**: Pre-registered to launch automatically with Windows.
- **Developed by TakeYourSite.com**: Clicking the footer link opens the website in your default browser.

---

## Running the Application Automatically

To build and run the application instantly:

1. Double-click the **[run_screendeadpixelfixer.bat](file:///d:/windows_bad_screen_app/run_screendeadpixelfixer.bat)** file.
2. The script will automatically compile the code using the built-in Windows C# compiler (`csc.exe`) if the executable is missing, and launch the application.

---

## Open Source Verification & Trust / دليل الأمان والشفافية البرمجية

To prove to your users that this application is 100% safe, open-source, and free of spyware or malware, you can follow these industry-standard trust verification methods:

### 1. Cloud-Based CI/CD Builds (GitHub Actions)
Do not distribute binaries compiled on your local machine. Set up a GitHub Action to automatically compile the binary on GitHub's secure servers upon code changes. This guarantees that the executable downloaded from the "Releases" section matches the public repository source code exactly.

### 2. Digital Fingerprinting (SHA-256 Checksum)
Publish the SHA-256 cryptographic hash of the compiled `ScreenDeadPixelFixer.exe`. Users can verify the download integrity by running this command in PowerShell:
```powershell
Get-FileHash ScreenDeadPixelFixer.exe -Algorithm SHA256
```
Compare the output string to your published hash to confirm the file has not been altered.

### 3. Native Decompilation Audits (dnSpy / ILSpy)
Since .NET binaries compile to Intermediate Language (IL), users can drag-and-drop `ScreenDeadPixelFixer.exe` into decompilers like **dnSpy** or **ILSpy**. This will decompile the binary back into C# source code, allowing any user to verify that the executable running on their machine is identical to the open-source code.

### 4. Reproducible Builds (Local Compilation)
Because the codebase is written in clean, dependency-free C#, users do not need to download a pre-built binary. They can download the raw `.cs` files and compile the executable themselves in 1 second using the built-in Windows C# compiler by running:
```bash
run_screendeadpixelfixer.bat
```

### 5. VirusTotal Scan Reports
Upload the compiled `ScreenDeadPixelFixer.exe` to [VirusTotal](https://www.virustotal.com/) (which scans it with 70+ antiviruses) and share the public permalink of the clean scan results with your users.

---

## File Structure

- `ScreenDeadPixelFixer.exe`: The pre-compiled ultra-lightweight binary (27 KB) with the black circle icon.
- `run_screendeadpixelfixer.bat`: Script to compile (if needed) and run the application.
- `App.cs`: Entry point for WPF in pure C#.
- `MainWindow.cs`: Control Dashboard UI implemented entirely in C#.
- `OverlayWindow.cs`: Click-through circular overlay implemented entirely in C#.
- `SelectionWindow.cs`: Transparent full-screen region selector in pure C#.
- `NativeMethods.cs`: Win32 integration P/Invokes.
- `ScreenPatchEngine.cs`: Core background polling and capture loop.

---

## Version Control (Git) / حفظ التعديلات باستخدام Git

To save your changes to Git after making any modifications to the files, follow these steps:

1. **Check Modified Files**:
   ```powershell
   git status
   ```
2. **Stage Your Changes**:
   To stage all modified files for committing:
   ```powershell
   git add .
   ```
3. **Commit the Changes**:
   Save the staged changes to the repository history with a descriptive commit message:
   ```powershell
   git commit -m "your description of changes"
   ```

---

لحفظ تعديلاتك على Git بعد إجراء أي تعديل على الملفات، اتبع الخطوات التالية:

1. **التحقق من الملفات المعدلة**:
   ```powershell
   git status
   ```
2. **تجهيز الملفات للحفظ (Staging)**:
   لتجهيز جميع الملفات المعدلة للحفظ:
   ```powershell
   git add .
   ```
3. **حفظ التغييرات (Commit)**:
   حفظ التعديلات المجهزة في سجل Git مع كتابة وصف مختصر للتعديل:
   ```powershell
   git commit -m "وصف التعديل الخاص بك"
   ```

