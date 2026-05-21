using System;
using System.Globalization;
using Microsoft.Win32;

namespace Kinogrida
{
    internal static class Settings
    {
        private const string RegKey = @"Software\Kinogrida";

        // Palette change interval in seconds (default: 120 = 2 minutes)
        public static double PaletteDuration
        {
            get => GetDouble("PaletteDuration", 120.0);
            set => SetDouble("PaletteDuration", value);
        }

        // Fade duration in seconds (default: 1.2)
        public static double FadeDuration
        {
            get => GetDouble("FadeDuration", 1.2);
            set => SetDouble("FadeDuration", value);
        }

        // Shape animation speed in seconds per cycle (default: 10)
        public static double AnimSpeed
        {
            get => GetDouble("AnimSpeed", 10.0);
            set => SetDouble("AnimSpeed", value);
        }

        // Grid fill percentage (default: 20)
        public static int FillPercent
        {
            get => (int)GetDouble("FillPercent", 20.0);
            set => SetDouble("FillPercent", value);
        }

        private static double GetDouble(string name, double def)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegKey);
            if (key?.GetValue(name) is string s &&
                double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                return v;
            return def;
        }

        private static void SetDouble(string name, double value)
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegKey);
            key?.SetValue(name, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
