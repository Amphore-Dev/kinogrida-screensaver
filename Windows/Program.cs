using System;
using System.Windows.Forms;

namespace Kinogrida
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string arg = args.Length > 0 ? args[0].ToLower().Trim() : "";

            if (arg == "/s")
            {
                var screens = Screen.AllScreens;
                for (int i = 1; i < screens.Length; i++)
                    new ScreensaverForm(screens[i].Bounds).Show();
                Application.Run(new ScreensaverForm(screens[0].Bounds));
            }
            else if (arg.StartsWith("/p"))
            {
                string hwndStr = args.Length > 1 ? args[1] : arg.Substring(2).Trim();
                if (long.TryParse(hwndStr, out long hwnd))
                    Application.Run(new PreviewForm(new IntPtr(hwnd)));
            }
            else if (arg == "/c" || arg.StartsWith("/c:"))
            {
                Application.Run(new SettingsForm());
            }
            else
            {
                Application.Run(new SettingsForm());
            }
        }
    }
}
