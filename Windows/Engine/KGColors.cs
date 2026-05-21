using System;
using System.Drawing;

namespace Kinogrida
{
    static class KGColors
    {
        private static Color FromHex(string hex)
        {
            string s = hex.TrimStart('#');
            int rgb = Convert.ToInt32(s, 16);
            return Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
        }

        private static Color[][] AllPalettes() => new[]
        {
            // Océan
            new[] { FromHex("#001f3f"), FromHex("#2E86AB"), FromHex("#A23B72"), FromHex("#F18F01"),
                    FromHex("#C73E1D"), FromHex("#7FDBFF"), FromHex("#85C1E9"), FromHex("#48C9B0"),
                    FromHex("#52BE80"), FromHex("#F8C471") },
            // Coucher de soleil
            new[] { FromHex("#FF6B6B"), FromHex("#FF8E53"), FromHex("#FF6B9D"), FromHex("#FFD93D"),
                    FromHex("#6BCF7F"), FromHex("#4ECDC4"), FromHex("#45B7D1"), FromHex("#96CEB4"),
                    FromHex("#FFEAA7"), FromHex("#DDA0DD") },
            // Sombre
            new[] { FromHex("#2C3E50"), FromHex("#34495E"), FromHex("#E74C3C"), FromHex("#E67E22"),
                    FromHex("#F39C12"), FromHex("#27AE60"), FromHex("#16A085"), FromHex("#3498DB"),
                    FromHex("#9B59B6"), FromHex("#95A5A6") },
            // Cyberpunk
            new[] { FromHex("#0F3460"), FromHex("#533483"), FromHex("#E94560"), FromHex("#0F4C75"),
                    FromHex("#3282B8"), FromHex("#BBE1FA"), FromHex("#FF6B6B"), FromHex("#4ECDC4"),
                    FromHex("#45B7D1"), FromHex("#96CEB4") },
            // Pastèque
            new[] { FromHex("#007A3D"), FromHex("#FFFFFF"), FromHex("#FF0000") },
        };

        public static Color[] RandomPalette()
        {
            var all = AllPalettes();
            return all[Random.Shared.Next(all.Length)];
        }
    }
}
