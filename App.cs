using System;
using System.Threading;
using System.Windows;

namespace ScreenDeadPixelFixer
{
    public class App : Application
    {
        private static EventWaitHandle _eventWaitHandle;

        [STAThread]
        public static void Main(string[] args)
        {
            bool isBackground = false;
            foreach (var arg in args)
            {
                if (arg.Equals("--background", StringComparison.OrdinalIgnoreCase) ||
                    arg.Equals("/background", StringComparison.OrdinalIgnoreCase))
                {
                    isBackground = true;
                    break;
                }
            }

            bool createdNew;
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "ScreenDeadPixelFixer-SingleInstance-Event", out createdNew);

            if (!createdNew)
            {
                // Signal the running instance to show itself
                _eventWaitHandle.Set();
                return;
            }

            App app = new App();
            MainWindow mainWindow = new MainWindow(isBackground, _eventWaitHandle);
            app.MainWindow = mainWindow;

            if (!isBackground)
            {
                mainWindow.Show();
            }

            app.Run();
        }
    }
}
