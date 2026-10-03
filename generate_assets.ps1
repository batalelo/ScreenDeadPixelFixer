param (
    [string]$IconPath = "$PSScriptRoot\icon.ico",
    [string]$OutputDir = "$PSScriptRoot\Package\Assets"
)

Add-Type -AssemblyName System.Drawing

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$sourceBitmap = New-Object System.Drawing.Bitmap($IconPath)

function Resize-SquareImage {
    param (
        [System.Drawing.Bitmap]$source,
        [int]$size,
        [string]$destinationPath
    )
    $destBitmap = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($destBitmap)
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $graphics.Clear([System.Drawing.Color]::Transparent)

    $graphics.DrawImage($source, 0, 0, $size, $size)
    $graphics.Dispose()

    $destBitmap.Save($destinationPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $destBitmap.Dispose()
    Write-Host "Created: $destinationPath ($($size)x$($size))"
}

function Create-WideImage {
    param (
        [System.Drawing.Bitmap]$source,
        [int]$width,
        [int]$height,
        [int]$iconSize,
        [string]$destinationPath,
        [System.Drawing.Color]$bgColor
    )
    $destBitmap = New-Object System.Drawing.Bitmap($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = [System.Drawing.Graphics]::FromImage($destBitmap)
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $graphics.Clear($bgColor)

    $x = ($width - $iconSize) / 2
    $y = ($height - $iconSize) / 2

    $graphics.DrawImage($source, [int]$x, [int]$y, $iconSize, $iconSize)
    $graphics.Dispose()

    $destBitmap.Save($destinationPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $destBitmap.Dispose()
    Write-Host "Created: $destinationPath ($($width)x$($height))"
}

# 1. Standard Square Logos
Resize-SquareImage -source $sourceBitmap -size 44 -destinationPath "$OutputDir\Square44x44Logo.png"
Resize-SquareImage -source $sourceBitmap -size 50 -destinationPath "$OutputDir\StoreLogo.png"
Resize-SquareImage -source $sourceBitmap -size 71 -destinationPath "$OutputDir\Square71x71Logo.png"
Resize-SquareImage -source $sourceBitmap -size 150 -destinationPath "$OutputDir\Square150x150Logo.png"
Resize-SquareImage -source $sourceBitmap -size 310 -destinationPath "$OutputDir\Square310x310Logo.png"

# 2. Target Size variants for 44x44 (unplated for taskbar & alt-tab)
Resize-SquareImage -source $sourceBitmap -size 44 -destinationPath "$OutputDir\Square44x44Logo.targetsize-44_altform-unplated.png"
Resize-SquareImage -source $sourceBitmap -size 24 -destinationPath "$OutputDir\Square44x44Logo.targetsize-24_altform-unplated.png"
Resize-SquareImage -source $sourceBitmap -size 16 -destinationPath "$OutputDir\Square44x44Logo.targetsize-16_altform-unplated.png"
Resize-SquareImage -source $sourceBitmap -size 256 -destinationPath "$OutputDir\Square44x44Logo.targetsize-256_altform-unplated.png"

# 3. Wide and Splash
$darkBg = [System.Drawing.ColorTranslator]::FromHtml("#111116")
$transparentColor = [System.Drawing.Color]::Transparent
Create-WideImage -source $sourceBitmap -width 310 -height 150 -iconSize 100 -destinationPath "$OutputDir\Wide310x150Logo.png" -bgColor $transparentColor
Create-WideImage -source $sourceBitmap -width 620 -height 300 -iconSize 130 -destinationPath "$OutputDir\SplashScreen.png" -bgColor $darkBg

$sourceBitmap.Dispose()
Write-Host "All Microsoft Store assets generated successfully in $OutputDir"
