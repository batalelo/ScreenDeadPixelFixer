param (
    [string]$OutputDir = "$PSScriptRoot\StoreScreenshots"
)

Add-Type -AssemblyName System.Drawing

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

function Create-Screenshot1 {
    # 1920x1080 Desktop View with damaged circle and bubble overlay
    $bmp = New-Object System.Drawing.Bitmap(1920, 1080, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit

    # Desktop wallpaper gradient (Modern dark slate / tech style)
    $rect = New-Object System.Drawing.Rectangle(0, 0, 1920, 1080)
    $gradBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, 
        [System.Drawing.ColorTranslator]::FromHtml("#0f172a"), 
        [System.Drawing.ColorTranslator]::FromHtml("#1e293b"), 
        [System.Drawing.Drawing2D.LinearGradientMode]::ForwardDiagonal)
    $g.FillRectangle($gradBrush, $rect)
    $gradBrush.Dispose()

    # Windows 11 Taskbar
    $taskbarBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#18181b"))
    $g.FillRectangle($taskbarBrush, 0, 1032, 1920, 48)
    $taskbarBrush.Dispose()

    # Draw simulated desktop browser/window
    $winBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#ffffff"))
    $g.FillRectangle($winBrush, 240, 140, 1440, 800)
    $winBrush.Dispose()

    # Window title bar
    $winTitleBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#e2e8f0"))
    $g.FillRectangle($winTitleBrush, 240, 140, 1440, 42)
    $winTitleBrush.Dispose()

    # Window sample text lines
    $lineBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#cbd5e1"))
    for ($y = 220; $y -lt 880; $y += 35) {
        $g.FillRectangle($lineBrush, 300, $y, 750, 14)
    }
    $lineBrush.Dispose()

    # Simulate physical black circle damage on screen (Liquid crystal leak)
    $damageCenter = New-Object System.Drawing.Point(920, 520)
    $damageRadius = 90
    $blackBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::Black)
    $g.FillEllipse($blackBrush, $damageCenter.X - $damageRadius, $damageCenter.Y - $damageRadius, $damageRadius * 2, $damageRadius * 2)
    $blackBrush.Dispose()

    # Draw Black Circle Fixer Magnifying Bubble Overlay
    $bubbleCenter = New-Object System.Drawing.Point(1140, 520)
    $bubbleRadius = 95
    $bubbleBorderBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#00ADB5"))
    $bubbleBgBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#ffffff"))
    
    $g.FillEllipse($bubbleBorderBrush, $bubbleCenter.X - $bubbleRadius - 4, $bubbleCenter.Y - $bubbleRadius - 4, ($bubbleRadius * 2) + 8, ($bubbleRadius * 2) + 8)
    $g.FillEllipse($bubbleBgBrush, $bubbleCenter.X - $bubbleRadius, $bubbleCenter.Y - $bubbleRadius, $bubbleRadius * 2, $bubbleRadius * 2)
    
    # Render magnified content inside bubble
    $magLineBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#0f172a"))
    $font = New-Object System.Drawing.Font("Segoe UI", 16, [System.Drawing.FontStyle]::Bold)
    $g.DrawString("Magnified Text", $font, $magLineBrush, $bubbleCenter.X - 70, $bubbleCenter.Y - 30)
    $g.DrawString("Behind Circle!", $font, $bubbleBorderBrush, $bubbleCenter.X - 70, $bubbleCenter.Y)
    $font.Dispose()
    $magLineBrush.Dispose()
    $bubbleBorderBrush.Dispose()
    $bubbleBgBrush.Dispose()

    # Dashboard Control Panel in corner
    $dashBg = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#111116"))
    $g.FillRectangle($dashBg, 1460, 200, 380, 210)
    $dashBg.Dispose()

    $dashTitleFont = New-Object System.Drawing.Font("Segoe UI", 12, [System.Drawing.FontStyle]::Bold)
    $cyanBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#00ADB5"))
    $g.DrawString("BLACK CIRCLE FIXER", $dashTitleFont, $cyanBrush, 1480, 212)
    
    # Button on dashboard
    $g.FillRectangle($cyanBrush, 1480, 255, 340, 42)
    $darkTextBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#111116"))
    $btnFont = New-Object System.Drawing.Font("Segoe UI", 11, [System.Drawing.FontStyle]::Bold)
    $g.DrawString("SELECT BLACK CIRCLE / DEAD ZONE", $btnFont, $darkTextBrush, 1500, 267)

    # Status tag
    $greenBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#22c55e"))
    $g.FillRectangle($greenBrush, 1480, 315, 340, 40)
    $g.DrawString("ENGINE RUNNING (30 FPS)", $btnFont, $darkTextBrush, 1530, 325)

    $dashTitleFont.Dispose()
    $cyanBrush.Dispose()
    $darkTextBrush.Dispose()
    $btnFont.Dispose()
    $greenBrush.Dispose()

    # Callout labels explaining the feature
    $labelFont = New-Object System.Drawing.Font("Segoe UI", 14, [System.Drawing.FontStyle]::Bold)
    $whiteBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $redBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#ef4444"))
    $g.DrawString("1. Physical Black Circle / Dead Zone", $labelFont, $redBrush, 750, 400)
    $cyanLabel = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#00ADB5"))
    $g.DrawString("2. Live Magnifying Bubble (Click-Through)", $labelFont, $cyanLabel, 1020, 400)
    $labelFont.Dispose()
    $whiteBrush.Dispose()
    $redBrush.Dispose()
    $cyanLabel.Dispose()

    $g.Dispose()
    $destPath = Join-Path $OutputDir "screenshot1.png"
    $bmp.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Created: $destPath"
}

function Create-Screenshot2 {
    # 1920x1080 Dashboard close-up presentation
    $bmp = New-Object System.Drawing.Bitmap(1920, 1080, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit

    # Dark background
    $rect = New-Object System.Drawing.Rectangle(0, 0, 1920, 1080)
    $gradBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, 
        [System.Drawing.ColorTranslator]::FromHtml("#0b0f19"), 
        [System.Drawing.ColorTranslator]::FromHtml("#111827"), 
        [System.Drawing.Drawing2D.LinearGradientMode]::ForwardDiagonal)
    $g.FillRectangle($gradBrush, $rect)
    $gradBrush.Dispose()

    # Big Header text
    $h1Font = New-Object System.Drawing.Font("Segoe UI", 36, [System.Drawing.FontStyle]::Bold)
    $cyanBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#00ADB5"))
    $whiteBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $g.DrawString("Black Circle Fixer", $h1Font, $cyanBrush, 150, 150)
    
    $subFont = New-Object System.Drawing.Font("Segoe UI", 18, [System.Drawing.FontStyle]::Regular)
    $subBrush = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#94a3b8"))
    $g.DrawString("Smart Workaround for Black Circle (Black Spot) on Laptop Screens", $subFont, $subBrush, 150, 220)

    # Large Center Card showing Dashboard UI
    $cardBg = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#111116"))
    $g.FillRectangle($cardBg, 660, 360, 600, 360)
    
    # Dashboard Title bar
    $dashTitleBar = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#181820"))
    $g.FillRectangle($dashTitleBar, 660, 360, 600, 55)
    $titleFont = New-Object System.Drawing.Font("Segoe UI", 14, [System.Drawing.FontStyle]::Bold)
    $g.DrawString("BLACK CIRCLE FIXER", $titleFont, $cyanBrush, 690, 375)

    # Select Button
    $g.FillRectangle($cyanBrush, 700, 450, 520, 60)
    $darkText = New-Object System.Drawing.SolidBrush([System.Drawing.ColorTranslator]::FromHtml("#111116"))
    $btnFont = New-Object System.Drawing.Font("Segoe UI", 14, [System.Drawing.FontStyle]::Bold)
    $g.DrawString("SELECT BLACK CIRCLE / DEAD ZONE", $btnFont, $darkText, 760, 467)

    # Checkbox
    $cbFont = New-Object System.Drawing.Font("Segoe UI", 13, [System.Drawing.FontStyle]::Regular)
    $g.DrawString("[v] Start automatically with Windows", $cbFont, $subBrush, 710, 535)

    # Start Engine Button
    $g.FillRectangle($cyanBrush, 700, 580, 520, 60)
    $g.DrawString("START ENGINE", $btnFont, $darkText, 880, 597)

    # Footer
    $footFont = New-Object System.Drawing.Font("Segoe UI", 11, [System.Drawing.FontStyle]::Regular)
    $g.DrawString("Developed with <3 by TakeYourSite.com", $footFont, $subBrush, 830, 665)

    $h1Font.Dispose()
    $cyanBrush.Dispose()
    $whiteBrush.Dispose()
    $subFont.Dispose()
    $subBrush.Dispose()
    $cardBg.Dispose()
    $dashTitleBar.Dispose()
    $titleFont.Dispose()
    $darkText.Dispose()
    $btnFont.Dispose()
    $cbFont.Dispose()
    $footFont.Dispose()

    $g.Dispose()
    $destPath = Join-Path $OutputDir "screenshot2.png"
    $bmp.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Created: $destPath"
}

Create-Screenshot1
Create-Screenshot2
Write-Host "All Microsoft Store screenshots created successfully in $OutputDir"
