using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace ScreenDeadPixelFixer
{
    public class MainWindow : Window
    {
        private readonly ScreenDeadPixelFixerEngine _engine;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isShuttingDown = false;
        
        // Internal variables for coordinate tracking
        private int _zoneX = 100;
        private int _zoneY = 100;
        private int _zoneWidth = 150;
        private int _zoneHeight = 150;
        
        private bool _isStartingInBackground = false;
        private bool _isInitializing = true;
        private System.Threading.EventWaitHandle _singleInstanceEvent;

        private Button BtnToggle;
        private CheckBox ChkStartup;

        public MainWindow(bool isStartingInBackground, System.Threading.EventWaitHandle singleInstanceEvent)
        {
            _isStartingInBackground = isStartingInBackground;
            _singleInstanceEvent = singleInstanceEvent;
            // Set Window Styles to borderless with custom rounded corners
            Title = "Screen Dead Pixel Fixer";
            Height = 210;
            Width = 380;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
            FontFamily = new FontFamily("Segoe UI Semibold, Segoe UI, sans-serif");

            // Extract window icon dynamically from the compiled executable's Win32 icon
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using (System.Drawing.Icon sysIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath))
                {
                    if (sysIcon != null)
                    {
                        this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            sysIcon.Handle,
                            System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    }
                }
            }
            catch { }

            // Outer border for premium rounded corner look
            Border outerBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E24")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5")),
                BorderThickness = new Thickness(1.5),
                CornerRadius = new CornerRadius(8)
            };

            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Title Bar Border
            Border titleBar = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111116")),
                Height = 40,
                Padding = new Thickness(15, 0, 15, 0),
                CornerRadius = new CornerRadius(7, 7, 0, 0)
            };
            Grid titleGrid = new Grid();
            
            TextBlock titleText = new TextBlock
            {
                Text = "SCREEN DEAD PIXEL FIXER",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5")),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            titleGrid.Children.Add(titleText);
            
            // Minimize & Close controls
            StackPanel winControls = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            Button btnMin = new Button
            {
                Content = "_",
                FontWeight = FontWeights.Bold,
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                BorderThickness = new Thickness(0),
                Width = 25,
                Height = 25,
                Cursor = Cursors.Hand
            };
            btnMin.Click += (s, e) => this.WindowState = WindowState.Minimized;
            
            Button btnClose = new Button
            {
                Content = "X",
                FontWeight = FontWeights.Bold,
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                BorderThickness = new Thickness(0),
                Width = 25,
                Height = 25,
                Cursor = Cursors.Hand,
                Margin = new Thickness(5, 0, 0, 0)
            };
            btnClose.Click += (s, e) => {
                HandleCloseRequest();
            };

            winControls.Children.Add(btnMin);
            winControls.Children.Add(btnClose);
            titleGrid.Children.Add(winControls);

            titleBar.Child = titleGrid;
            
            // Window drag behavior
            titleBar.MouseLeftButtonDown += (s, e) => {
                if (e.LeftButton == MouseButtonState.Pressed) DragMove();
            };

            mainGrid.Children.Add(titleBar);
            Grid.SetRow(titleBar, 0);

            // Content Panel
            StackPanel contentPanel = new StackPanel
            {
                Margin = new Thickness(20, 15, 20, 15)
            };

            // 1. Select Dead Zone Button
            Button btnSelect = new Button
            {
                Content = "SELECT DEAD ZONE ON SCREEN",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111116")),
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10),
                Cursor = Cursors.Hand
            };
            btnSelect.Click += SelectDeadZoneInteractively_Click;
            contentPanel.Children.Add(btnSelect);

            // 2. Startup Checkbox (Checked by default)
            ChkStartup = new CheckBox
            {
                Content = "Start automatically with Windows",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC")),
                Margin = new Thickness(5, 12, 0, 12),
                FontSize = 12,
                IsChecked = true,
                Cursor = Cursors.Hand
            };
            ChkStartup.Checked += ChkStartup_Changed;
            ChkStartup.Unchecked += ChkStartup_Changed;
            contentPanel.Children.Add(ChkStartup);

            // 3. Engine Toggle Button
            BtnToggle = new Button
            {
                Content = "START ENGINE",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5")),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#111116")),
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Padding = new Thickness(10),
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };
            BtnToggle.Click += ToggleEngine_Click;
            contentPanel.Children.Add(BtnToggle);

            // 4. Footer link (Developed with ❤ by TakeYourSite.com)
            TextBlock footerBlock = new TextBlock
            {
                Text = "Developed with ❤ by TakeYourSite.com",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888")),
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0),
                Cursor = Cursors.Hand
            };
            
            // Hover styles for hyperlink look
            footerBlock.MouseEnter += (s, ev) => {
                footerBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5"));
                footerBlock.TextDecorations = TextDecorations.Underline;
            };
            footerBlock.MouseLeave += (s, ev) => {
                footerBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                footerBlock.TextDecorations = null;
            };
            
            // Click to open URL
            footerBlock.MouseLeftButtonDown += (s, ev) => {
                try
                {
                    System.Diagnostics.Process.Start("https://takeyoursite.com");
                }
                catch
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://takeyoursite.com") { UseShellExecute = true });
                    }
                    catch { }
                }
            };
            contentPanel.Children.Add(footerBlock);

            mainGrid.Children.Add(contentPanel);
            Grid.SetRow(contentPanel, 1);

            outerBorder.Child = mainGrid;
            Content = outerBorder;

            // Instantiate Engine
            _engine = new ScreenDeadPixelFixerEngine(this);

            // Setup Tray Icon System
            SetupTrayIcon();

            // Enforce default Registry auto-start on first run
            SetStartup(true);

            // Load saved settings
            LoadSettings();

            // Start background activation listener
            if (_singleInstanceEvent != null)
            {
                var waitThread = new System.Threading.Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            if (_singleInstanceEvent.WaitOne())
                            {
                                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ShowAndActivate();
                                }));
                            }
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            break;
                        }
                        catch
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                });
                waitThread.IsBackground = true;
                waitThread.Start();
            }

            _isInitializing = false;
        }

        private void SetupTrayIcon()
        {
            try
            {
                _notifyIcon = new System.Windows.Forms.NotifyIcon();
                _notifyIcon.Text = "Screen Dead Pixel Fixer";
                
                System.Drawing.Icon appIcon = null;
                try
                {
                    string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    appIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                }
                catch { }

                if (appIcon == null)
                {
                    try
                    {
                        string iconPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                        if (System.IO.File.Exists(iconPath))
                        {
                            appIcon = new System.Drawing.Icon(iconPath);
                        }
                    }
                    catch { }
                }

                if (appIcon == null)
                {
                    appIcon = System.Drawing.SystemIcons.Application;
                }

                _notifyIcon.Icon = appIcon;
                _notifyIcon.Visible = true;
                
                // Double-click tray icon to restore the window
                _notifyIcon.DoubleClick += (s, e) => {
                    ShowAndActivate();
                };

                // Add simple Context Menu
                var contextMenu = new System.Windows.Forms.ContextMenu();
                contextMenu.MenuItems.Add("Show Dashboard", (s, e) => {
                    ShowAndActivate();
                });
                contextMenu.MenuItems.Add("Exit Completely", (s, e) => {
                    ShutdownApplication();
                });
                _notifyIcon.ContextMenu = contextMenu;
            }
            catch { }
        }

        private void ShowAndActivate()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void HandleCloseRequest()
        {
            MessageBoxResult result = MessageBox.Show(
                "Do you want to keep the engine running in the background?\n\n- Click 'Yes' to run in the background (System Tray).\n- Click 'No' to close the application completely.", 
                "Screen Dead Pixel Fixer", 
                MessageBoxButton.YesNoCancel, 
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                this.Hide();
                if (_notifyIcon != null)
                {
                    _notifyIcon.ShowBalloonTip(
                        3000, 
                        "Screen Dead Pixel Fixer", 
                        "The application is still running in the background. Double-click the tray icon to open.", 
                        System.Windows.Forms.ToolTipIcon.Info
                    );
                }
            }
            else if (result == MessageBoxResult.No)
            {
                ShutdownApplication();
            }
        }

        private void ShutdownApplication()
        {
            _isShuttingDown = true;
            _engine.Stop();
            SaveSettings();
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            if (_singleInstanceEvent != null)
            {
                try { _singleInstanceEvent.Close(); } catch { }
            }
            Application.Current.Shutdown();
        }

        private void ToggleEngine_Click(object sender, RoutedEventArgs e)
        {
            ToggleEngine(!_engine.IsRunning);
            SaveSettings();
        }

        private void ToggleEngine(bool start)
        {
            if (!start)
            {
                _engine.Stop();
                BtnToggle.Content = "START ENGINE";
                BtnToggle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ADB5"));
            }
            else
            {
                if (ApplySettings())
                {
                    _engine.Start();
                    BtnToggle.Content = "STOP ENGINE";
                    BtnToggle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C60"));
                    
                    // Hide window automatically when engine starts (only if not initial load)
                    if (!_isInitializing)
                    {
                        this.Hide();
                        if (_notifyIcon != null)
                        {
                            _notifyIcon.ShowBalloonTip(
                                3000, 
                                "Screen Dead Pixel Fixer", 
                                "The engine is now running in the background. Double-click the tray icon to open the dashboard.", 
                                System.Windows.Forms.ToolTipIcon.Info
                            );
                        }
                    }
                }
            }
        }

        private bool ApplySettings()
        {
            _engine.DeadZoneX = _zoneX;
            _engine.DeadZoneY = _zoneY;
            _engine.DeadZoneWidth = _zoneWidth;
            _engine.DeadZoneHeight = _zoneHeight;
            return true;
        }

        private void SelectDeadZoneInteractively_Click(object sender, RoutedEventArgs e)
        {
            var prevWindowState = this.WindowState;
            this.WindowState = WindowState.Minimized;

            var delayTimer = new System.Windows.Threading.DispatcherTimer();
            delayTimer.Interval = TimeSpan.FromMilliseconds(200);
            delayTimer.Tick += (s, ev) =>
            {
                delayTimer.Stop();

                var selectWin = new SelectionWindow();
                selectWin.ShowDialog();

                this.WindowState = prevWindowState;

                if (selectWin.IsSelectionConfirmed)
                {
                    _zoneX = selectWin.SelectedRect.X;
                    _zoneY = selectWin.SelectedRect.Y;
                    _zoneWidth = selectWin.SelectedRect.Width;
                    _zoneHeight = selectWin.SelectedRect.Height;

                    if (_engine.IsRunning)
                    {
                        ApplySettings();
                    }
                    SaveSettings();
                }
            };
            delayTimer.Start();
        }

        private void ChkStartup_Changed(object sender, RoutedEventArgs e)
        {
            bool isChecked = ChkStartup.IsChecked == true;
            SetStartup(isChecked);
            SaveSettings();
        }

        // --- Config file Save/Load ---

        private string GetConfigPath()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(appDir, "ScreenDeadPixelFixerSettings.txt");
        }

        private void SaveSettings()
        {
            try
            {
                string path = GetConfigPath();
                string[] lines = new string[]
                {
                    _zoneX.ToString(),
                    _zoneY.ToString(),
                    _zoneWidth.ToString(),
                    _zoneHeight.ToString(),
                    _engine.IsRunning.ToString(),
                    (ChkStartup.IsChecked == true).ToString()
                };
                File.WriteAllLines(path, lines);
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                string path = GetConfigPath();
                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);
                    if (lines.Length >= 6)
                    {
                        _zoneX = int.Parse(lines[0]);
                        _zoneY = int.Parse(lines[1]);
                        _zoneWidth = int.Parse(lines[2]);
                        _zoneHeight = int.Parse(lines[3]);
                        
                        bool autoRunEngine = false;
                        bool startupWin = true;

                        if (lines.Length >= 8)
                        {
                            bool.TryParse(lines[6], out autoRunEngine);
                            bool.TryParse(lines[7], out startupWin);
                        }
                        else
                        {
                            bool.TryParse(lines[4], out autoRunEngine);
                            bool.TryParse(lines[5], out startupWin);
                        }
                        
                        ChkStartup.IsChecked = startupWin;

                        ApplySettings();

                        if (autoRunEngine)
                        {
                            ToggleEngine(true);
                        }
                    }
                }
            }
            catch { }
        }

        private void SetStartup(bool enable)
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
                {
                    if (key != null)
                    {
                        if (enable)
                        {
                            string appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                            key.SetValue("ScreenDeadPixelFixer", "\"" + appPath + "\" --background");
                        }
                        else
                        {
                            key.DeleteValue("ScreenDeadPixelFixer", false);
                        }
                    }
                }
            }
            catch { }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isShuttingDown)
            {
                e.Cancel = true;
                HandleCloseRequest();
            }
            else
            {
                base.OnClosing(e);
            }
        }
    }
}
