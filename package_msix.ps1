# PowerShell Script to Package BlackCircleFixer into an MSIX for the Microsoft Store

$projectRoot = $PSScriptRoot
$packageDir = Join-Path $projectRoot "Package"
$outputMsix = Join-Path $projectRoot "BlackCircleFixer.msix"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  Building Microsoft Store MSIX Package: BlackCircleFixer " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Compile Executable if needed
$exePath = Join-Path $projectRoot "BlackCircleFixer.exe"
Write-Host "`n[1/4] Compiling executable..." -ForegroundColor Yellow
$cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$sourceFiles = @(
    (Join-Path $projectRoot "App.cs"),
    (Join-Path $projectRoot "MainWindow.cs"),
    (Join-Path $projectRoot "OverlayWindow.cs"),
    (Join-Path $projectRoot "SelectionWindow.cs"),
    (Join-Path $projectRoot "NativeMethods.cs"),
    (Join-Path $projectRoot "BlackCircleFixerEngine.cs")
)

$compileArgs = @(
    "/target:winexe",
    "/out:$exePath",
    "/win32icon:$projectRoot\icon.ico",
    "/r:System.dll",
    "/r:System.Drawing.dll",
    "/r:System.Windows.Forms.dll",
    "/r:System.Xaml.dll",
    "/r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\WindowsBase.dll",
    "/r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationCore.dll",
    "/r:C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationFramework.dll",
    "/r:System.Core.dll"
) + $sourceFiles

& $cscPath $compileArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "Compilation failed."
    exit $LASTEXITCODE
}
Write-Host "Executable compiled successfully." -ForegroundColor Green

# 2. Ensure Assets exist
Write-Host "`n[2/4] Verifying Store assets..." -ForegroundColor Yellow
$assetsDir = Join-Path $packageDir "Assets"
if (-not (Test-Path (Join-Path $assetsDir "StoreLogo.png"))) {
    & "$projectRoot\generate_assets.ps1"
} else {
    Write-Host "Store assets verified." -ForegroundColor Green
}

# 3. Copy executable into Package folder
Write-Host "`n[3/4] Staging files in Package directory..." -ForegroundColor Yellow
Copy-Item -Path $exePath -Destination (Join-Path $packageDir "BlackCircleFixer.exe") -Force
Write-Host "Staging complete." -ForegroundColor Green

# 4. Locate MakeAppx.exe and build MSIX
Write-Host "`n[4/4] Packing MSIX container..." -ForegroundColor Yellow
$makeAppxPath = $null

$possiblePaths = @(
    "C:\Program Files (x86)\Windows Kits\10\bin\*\x64\makeappx.exe",
    "C:\Program Files\Windows Kits\10\bin\*\x64\makeappx.exe"
)

foreach ($pattern in $possiblePaths) {
    $matches = Resolve-Path $pattern -ErrorAction SilentlyContinue
    if ($matches) {
        $makeAppxPath = $matches[-1].Path
        break
    }
}

if (-not $makeAppxPath) {
    $cmd = Get-Command makeappx.exe -ErrorAction SilentlyContinue
    if ($cmd) {
        $makeAppxPath = $cmd.Source
    }
}

if ($makeAppxPath) {
    Write-Host "Found MakeAppx at: $makeAppxPath" -ForegroundColor Cyan
    & $makeAppxPath pack /d "$packageDir" /p "$outputMsix" /o
    if ($LASTEXITCODE -eq 0) {
        $size = (Get-Item $outputMsix).Length / 1KB
        Write-Host "`nSUCCESS: Package generated at: $outputMsix ($([math]::Round($size, 2)) KB)" -ForegroundColor Green
    } else {
        Write-Error "MakeAppx failed to package."
        exit $LASTEXITCODE
    }
} else {
    Write-Host "`nNote: Windows SDK (MakeAppx.exe) was not found on this local machine." -ForegroundColor Yellow
    Write-Host "However, our automated GitHub Actions workflow will automatically build and package" -ForegroundColor Cyan
    Write-Host "the complete 'BlackCircleFixer.msix' in the cloud on every push!" -ForegroundColor Cyan
}
