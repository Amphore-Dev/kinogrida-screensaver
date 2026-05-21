using System;
using System.Drawing;

namespace Kinogrida
{
    class KGGridConfig
    {
        public int NbrColumns = 10;
        public int NbrRows    = 10;
        public float CellSize  = 1f;
        public float GridMargin = 50f;
        public float OffsetX   = 0f;
        public float OffsetY   = 0f;
        public float Width     = 0f;
        public float Height    = 0f;
        public float LineWidth = 1f;
        public float Speed     = 10000f;
        public Color[] Colors  = Array.Empty<Color>();

        public static KGGridConfig Default()
        {
            var c = new KGGridConfig();
            c.Colors = KGColors.RandomPalette();
            return c;
        }

        public void UpdateForCanvasSize(float canvasW, float canvasH)
        {
            float availW = canvasW - 2 * GridMargin;
            float availH = canvasH - 2 * GridMargin;
            CellSize = MathF.Floor(MathF.Min(availW / NbrColumns, availH / NbrRows));
            Width    = NbrColumns * CellSize;
            Height   = NbrRows    * CellSize;
            OffsetX  = (canvasW - Width)  / 2f;
            OffsetY  = (canvasH - Height) / 2f;
            LineWidth = MathF.Max(1f, MathF.Floor(CellSize * 0.1f));
        }
    }
}
