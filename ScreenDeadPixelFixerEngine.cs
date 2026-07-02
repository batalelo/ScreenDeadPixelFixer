using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ScreenDeadPixelFixer
{
    public class ScreenDeadPixelFixerEngine
    {
        private readonly DispatcherTimer _timer;
        private readonly OverlayWindow _overlayWindow;
        private readonly MainWindow _mainWindow;

        // Configurations
        public int DeadZoneX { get; set; }
        public int DeadZoneY { get; set; }
        public int DeadZoneWidth { get; set; }
        public int DeadZoneHeight { get; set; }

        public bool IsRunning 
        { 
            get { return _timer.IsEnabled; } 
        }

        public ScreenDeadPixelFixerEngine(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _overlayWindow = new OverlayWindow();

            _timer = new DispatcherTimer(DispatcherPriority.Render);
            _timer.Interval = TimeSpan.FromMilliseconds(33); // ~30 FPS
            _timer.Tick += EngineTick;
        }

        public void Start()
        {
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                _overlayWindow.Hide();
            }
        }

        private void EngineTick(object sender, EventArgs e)
        {
            NativeMethods.POINT cursorPt;
            if (!NativeMethods.GetCursorPos(out cursorPt))
            {
                return;
            }

            // Check if cursor is in physical screen coordinates Dead Zone
            bool inDeadZone = cursorPt.X >= DeadZoneX &&
                             cursorPt.X <= (DeadZoneX + DeadZoneWidth) &&
                             cursorPt.Y >= DeadZoneY &&
                             cursorPt.Y <= (DeadZoneY + DeadZoneHeight);

            if (inDeadZone)
            {
                // Get DPI scale of the screen/main window to position the overlay correctly
                double scaleX = 1.0;
                double scaleY = 1.0;
                var source = PresentationSource.FromVisual(_mainWindow);
                if (source != null && source.CompositionTarget != null)
                {
                    scaleX = source.CompositionTarget.TransformToDevice.M11;
                    scaleY = source.CompositionTarget.TransformToDevice.M22;
                }

                // Compute physical overlay window size
                int physicalOverlayWidth = (int)(_overlayWindow.Width * scaleX);
                int physicalOverlayHeight = (int)(_overlayWindow.Height * scaleY);

                // Physical bounds of the Dead Zone
                int dzLeft = DeadZoneX;
                int dzRight = DeadZoneX + DeadZoneWidth;
                int dzTop = DeadZoneY;

                // Adjacent positioning: Place the bubble exactly clinging to the side of the Dead Zone (X axis)
                // and perfectly aligned on the same horizontal level (centered vertically on the Y axis of the Dead Zone).
                int targetPhysicalX;
                int safetyMargin = 2; // 2px safety offset to avoid float precision capture overlap

                // Get primary screen size (physical)
                int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
                int physicalScreenWidth = (int)(screenWidth * scaleX);

                if (dzRight + safetyMargin + physicalOverlayWidth <= physicalScreenWidth)
                {
                    // Placed stuck to the right edge of the dead zone
                    targetPhysicalX = dzRight + safetyMargin;
                }
                else if (dzLeft - safetyMargin - physicalOverlayWidth >= 0)
                {
                    // Placed stuck to the left edge of the dead zone
                    targetPhysicalX = dzLeft - safetyMargin - physicalOverlayWidth;
                }
                else
                {
                    // Fallback
                    if (physicalScreenWidth - dzRight > dzLeft)
                        targetPhysicalX = dzRight + 2;
                    else
                        targetPhysicalX = dzLeft - physicalOverlayWidth - 2;
                }

                // Align vertically on the exact same horizontal level (centered relative to the Dead Zone height)
                int targetPhysicalY = dzTop + (DeadZoneHeight / 2) - (physicalOverlayHeight / 2);

                // Convert physical target coordinates back to logical WPF pixels
                double logicalTargetX = targetPhysicalX / scaleX;
                double logicalTargetY = targetPhysicalY / scaleY;

                _overlayWindow.Left = logicalTargetX;
                _overlayWindow.Top = logicalTargetY;

                // Ensure overlay is shown
                if (!_overlayWindow.IsVisible)
                {
                    _overlayWindow.Show();
                }

                // Capture the hidden content under the Dead Zone
                UpdateOverlayImage();
            }
            else
            {
                if (_overlayWindow.IsVisible)
                {
                    _overlayWindow.Hide();
                }
            }
        }

        private void UpdateOverlayImage()
        {
            if (DeadZoneWidth <= 0 || DeadZoneHeight <= 0) return;

            try
            {
                using (var bitmap = new Bitmap(DeadZoneWidth, DeadZoneHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        // 1. Copy the screen pixels
                        graphics.CopyFromScreen(DeadZoneX, DeadZoneY, 0, 0, new System.Drawing.Size(DeadZoneWidth, DeadZoneHeight), CopyPixelOperation.SourceCopy);
                        
                        // 2. Overlay the current cursor icon
                        DrawCursorOnGraphics(graphics, DeadZoneX, DeadZoneY);
                    }

                    // Convert Bitmap to BitmapSource without leaking handles
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    try
                    {
                        var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        
                        // Freeze to allow cross-thread UI bindings
                        bitmapSource.Freeze();

                        _overlayWindow.SetOverlayImage(bitmapSource);
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hBitmap);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to capture screen: " + ex.Message);
            }
        }

        private void DrawCursorOnGraphics(Graphics g, int captureLeft, int captureTop)
        {
            var cursorInfo = new NativeMethods.CURSORINFO();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);
            if (NativeMethods.GetCursorInfo(out cursorInfo) && cursorInfo.flags == NativeMethods.CURSOR_SHOWING)
            {
                NativeMethods.ICONINFO iconInfo;
                if (NativeMethods.GetIconInfo(cursorInfo.hCursor, out iconInfo))
                {
                    // Compute relative position on captured frame
                    int cursorX = cursorInfo.ptScreenPos.X - captureLeft - iconInfo.xHotspot;
                    int cursorY = cursorInfo.ptScreenPos.Y - captureTop - iconInfo.yHotspot;

                    IntPtr hdc = g.GetHdc();
                    try
                    {
                        NativeMethods.DrawIcon(hdc, cursorX, cursorY, cursorInfo.hCursor);
                    }
                    finally
                    {
                        g.ReleaseHdc(hdc);
                    }

                    // Clean up GDI bitmap resources generated by GetIconInfo to avoid memory leaks
                    if (iconInfo.hbmMask != IntPtr.Zero) NativeMethods.DeleteObject(iconInfo.hbmMask);
                    if (iconInfo.hbmColor != IntPtr.Zero) NativeMethods.DeleteObject(iconInfo.hbmColor);
                }
            }
        }
    }
}
