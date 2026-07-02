@echo off
setlocal enabledelayedexpansion

:: Compile automatically if the EXE is missing
if not exist "%~dp0ScreenDeadPixelFixer.exe" (
    echo Compiling Screen Dead Pixel Fixer using built-in Windows C# compiler...
    "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /target:winexe /out:ScreenDeadPixelFixer.exe /win32icon:icon.ico /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xaml.dll /r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\WindowsBase.dll /r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationCore.dll /r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationFramework.dll /r:System.Core.dll "%~dp0App.cs" "%~dp0MainWindow.cs" "%~dp0OverlayWindow.cs" "%~dp0SelectionWindow.cs" "%~dp0NativeMethods.cs" "%~dp0ScreenDeadPixelFixerEngine.cs"
    if %errorlevel% neq 0 (
        echo Compilation failed.
        pause
        exit /b
    )
)

echo Launching Screen Dead Pixel Fixer...
start "" "%~dp0ScreenDeadPixelFixer.exe"
