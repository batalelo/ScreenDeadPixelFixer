using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenDeadPixelFixer
{
    public class OverlayWindow : Window
    {
        private Image _capturedImage;

        public OverlayWindow()
        {
            Title = "Screen Dead Pixel Fixer Overlay";
            Height = 200;
            Width = 200;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            Topmost = true;
            ShowInTaskbar = false;
            ShowActivated = false;
            WindowStartupLocation = WindowStartupLocation.Manual;

            Grid grid = new Grid();

            Border border = new Border
            {
                CornerRadius = new CornerRadius(100),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5")),
                BorderThickness = new Thickness(4),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222831")),
                ClipToBounds = true
            };

            Grid innerGrid = new Grid();
            _capturedImage = new Image
            {
                Stretch = Stretch.UniformToFill
            };
            innerGrid.Children.Add(_capturedImage);

            Border innerShadow = new Border
            {
                CornerRadius = new CornerRadius(100),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#393E46")),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(-1)
            };
            innerGrid.Children.Add(innerShadow);

            border.Child = innerGrid;
            grid.Children.Add(border);
            Content = grid;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            IntPtr extendedStyle = NativeMethods.GetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLongPtr(
                hwnd, 
                NativeMethods.GWL_EXSTYLE, 
                new IntPtr(extendedStyle.ToInt64() | NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_NOACTIVATE)
            );
        }

        public void SetOverlayImage(BitmapSource source)
        {
            _capturedImage.Source = source;
        }
    }
}
