using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenDeadPixelFixer
{
    public class SelectionWindow : Window
    {
        private Canvas _selectionCanvas;
        private Rectangle _selectionRect;
        private System.Windows.Point _startPoint;
        private bool _isDragging = false;

        private Int32Rect _selectedRect;
        public Int32Rect SelectedRect 
        { 
            get { return _selectedRect; } 
            private set { _selectedRect = value; }
        }

        private bool _isSelectionConfirmed = false;
        public bool IsSelectionConfirmed 
        { 
            get { return _isSelectionConfirmed; } 
            private set { _isSelectionConfirmed = value; }
        }

        public SelectionWindow()
        {
            Title = "Select Dead Zone";
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Color.FromArgb(96, 0, 0, 0));
            Topmost = true;
            WindowState = WindowState.Maximized;
            Cursor = Cursors.Cross;
            WindowStartupLocation = WindowStartupLocation.Manual;

            _selectionCanvas = new Canvas { Background = Brushes.Transparent, Focusable = true };
            _selectionCanvas.MouseDown += SelectionCanvas_MouseDown;
            _selectionCanvas.MouseMove += SelectionCanvas_MouseMove;
            _selectionCanvas.MouseUp += SelectionCanvas_MouseUp;
            _selectionCanvas.KeyDown += SelectionCanvas_KeyDown;

            _selectionRect = new Rectangle
            {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5")),
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 4 }),
                Fill = new SolidColorBrush(Color.FromArgb(48, 0, 173, 181)),
                Visibility = Visibility.Collapsed
            };

            TextBlock textBlock = new TextBlock
            {
                Text = "Click and drag to select the Dead Zone. Press ESC to cancel.",
                Foreground = Brushes.White,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetTop(textBlock, 20);
            Canvas.SetLeft(textBlock, 20);

            _selectionCanvas.Children.Add(_selectionRect);
            _selectionCanvas.Children.Add(textBlock);

            Content = _selectionCanvas;

            Loaded += (s, e) => {
                _selectionCanvas.Focus();
            };
        }

        private void SelectionCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _startPoint = e.GetPosition(_selectionCanvas);
                _isDragging = true;

                Canvas.SetLeft(_selectionRect, _startPoint.X);
                Canvas.SetTop(_selectionRect, _startPoint.Y);
                _selectionRect.Width = 0;
                _selectionRect.Height = 0;
                _selectionRect.Visibility = Visibility.Visible;

                _selectionCanvas.CaptureMouse();
            }
        }

        private void SelectionCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = e.GetPosition(_selectionCanvas);

                double x = Math.Min(_startPoint.X, currentPoint.X);
                double y = Math.Min(_startPoint.Y, currentPoint.Y);
                double width = Math.Abs(_startPoint.X - currentPoint.X);
                double height = Math.Abs(_startPoint.Y - currentPoint.Y);

                Canvas.SetLeft(_selectionRect, x);
                Canvas.SetTop(_selectionRect, y);
                _selectionRect.Width = width;
                _selectionRect.Height = height;
            }
        }

        private void SelectionCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && e.ChangedButton == MouseButton.Left)
            {
                _isDragging = false;
                _selectionCanvas.ReleaseMouseCapture();

                var currentPoint = e.GetPosition(_selectionCanvas);
                double logicalX = Math.Min(_startPoint.X, currentPoint.X);
                double logicalY = Math.Min(_startPoint.Y, currentPoint.Y);
                double logicalWidth = Math.Abs(_startPoint.X - currentPoint.X);
                double logicalHeight = Math.Abs(_startPoint.Y - currentPoint.Y);

                double scaleX = 1.0;
                double scaleY = 1.0;
                var source = PresentationSource.FromVisual(this);
                if (source != null && source.CompositionTarget != null)
                {
                    scaleX = source.CompositionTarget.TransformToDevice.M11;
                    scaleY = source.CompositionTarget.TransformToDevice.M22;
                }

                int physicalX = (int)(logicalX * scaleX);
                int physicalY = (int)(logicalY * scaleY);
                int physicalWidth = (int)(logicalWidth * scaleX);
                int physicalHeight = (int)(logicalHeight * scaleY);

                if (physicalWidth > 5 && physicalHeight > 5)
                {
                    SelectedRect = new Int32Rect(physicalX, physicalY, physicalWidth, physicalHeight);
                    IsSelectionConfirmed = true;
                }

                Close();
            }
        }

        private void SelectionCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                IsSelectionConfirmed = false;
                Close();
            }
        }
    }
}
