using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Kinogrida
{
    class KGSquareShape : KGBaseShape
    {
        private readonly float _radiusPercent;

        public KGSquareShape(KGGrid grid, int x, int y, Color color, float radiusPercent)
            : base(grid, x, y, color)
        {
            _radiusPercent = radiusPercent;
        }

        protected override List<GridPoint> GetLineCells(GridPoint from, GridPoint to)
        {
            var cells = new List<GridPoint>();
            if (from.Row == to.Row)
            {
                for (int col = Math.Min(from.Col, to.Col); col <= Math.Max(from.Col, to.Col); col++)
                    cells.Add(new GridPoint(col, from.Row));
            }
            else if (from.Col == to.Col)
            {
                for (int row = Math.Min(from.Row, to.Row); row <= Math.Max(from.Row, to.Row); row++)
                    cells.Add(new GridPoint(from.Col, row));
            }
            return cells;
        }

        public override void CalculateNewTarget(KGGrid grid, KGGridConfig config)
        {
            int dir  = Random.Shared.Next(1, 5); // 1=up 2=right 3=down 4=left
            int newX = (int)X, newY = (int)Y;

            if (dir == 1 || dir == 3)
            {
                int maxDelta = config.NbrRows / 2 - 1;
                if (maxDelta < 1) return;
                int delta = Random.Shared.Next(1, maxDelta + 1) * (dir == 1 ? -1 : 1);
                newY = Math.Clamp((int)Y + delta, 0, config.NbrRows - 1);
            }
            else
            {
                int maxDelta = config.NbrColumns / 2 - 1;
                if (maxDelta < 1) return;
                int delta = Random.Shared.Next(1, maxDelta + 1) * (dir == 2 ? -1 : 1);
                newX = Math.Clamp((int)X + delta, 0, config.NbrColumns - 1);
            }

            if (grid.IsEmpty(newY, newX) && (newX != (int)X || newY != (int)Y))
                MoveTo(grid, config, newX, newY);
        }

        public override void UpdatePosition(KGGrid grid)
        {
            float step = Speed * MoveDistance;

            if (HasReachedTarget)
            {
                if (Math.Abs(TailX - TargetX) > step) TailX += TailX < TargetX ? step : -step;
                else TailX = TargetX;

                if (Math.Abs(TailY - TargetY) > step) TailY += TailY < TargetY ? step : -step;
                else TailY = TargetY;

                if (TailX == TargetX && TailY == TargetY)
                    OnMoveComplete(grid, (int)TargetX, (int)TargetY);
            }
            else
            {
                if (Math.Abs(X - TargetX) > step) X += X < TargetX ? step : -step;
                else X = TargetX;

                if (Math.Abs(Y - TargetY) > step) Y += Y < TargetY ? step : -step;
                else Y = TargetY;

                if (X == TargetX && Y == TargetY) HasReachedTarget = true;
            }
        }

        public override void Draw(Graphics g, KGGridConfig config)
        {
            float cs  = config.CellSize;
            float ox  = config.OffsetX, oy = config.OffsetY;
            float pad = cs * 0.1f;

            float origPX = TailX * cs + ox + pad;
            float origPY = TailY * cs + oy + pad;
            float currPX = X     * cs + ox + pad;
            float currPY = Y     * cs + oy + pad;

            float startX = Math.Min(origPX, currPX);
            float startY = Math.Min(origPY, currPY);
            float rectW  = Math.Max(origPX, currPX) - startX + cs * 0.8f;
            float rectH  = Math.Max(origPY, currPY) - startY + cs * 0.8f;

            if (rectW <= 0 || rectH <= 0) return;

            float targetR = _radiusPercent * cs;
            float radius  = Math.Min(targetR, Math.Min(rectW, rectH) / 2f);
            float radius2 = Math.Min(targetR / 2f, Math.Min(rectW - cs * 0.4f, rectH - cs * 0.4f) / 2f);
            float lw      = config.LineWidth;

            using var outerPath = RoundedRect(startX, startY, rectW, rectH, radius);
            using var pen       = new Pen(Color, lw);
            g.DrawPath(pen, outerPath);

            float ix = startX + cs * 0.2f, iy = startY + cs * 0.2f;
            float iw = rectW - cs * 0.4f,  ih = rectH - cs * 0.4f;
            if (iw > 0 && ih > 0)
            {
                using var innerPath = RoundedRect(ix, iy, iw, ih, Math.Max(0, radius2));
                using var whitePen  = new Pen(System.Drawing.Color.White, lw);
                g.DrawPath(whitePen, innerPath);
            }
        }

        private static GraphicsPath RoundedRect(float x, float y, float w, float h, float r)
        {
            var path = new GraphicsPath();
            if (r <= 0)
            {
                path.AddRectangle(new RectangleF(x, y, w, h));
                return path;
            }
            float d = r * 2;
            path.AddArc(x,         y,         d, d, 180, 90);
            path.AddArc(x + w - d, y,         d, d, 270, 90);
            path.AddArc(x + w - d, y + h - d, d, d,   0, 90);
            path.AddArc(x,         y + h - d, d, d,  90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
