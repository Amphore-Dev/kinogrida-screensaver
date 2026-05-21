using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Kinogrida
{
    class KGArcShape : KGBaseShape
    {
        private float _angleOffset;
        private float _arc;
        private bool  _clockwise;
        private float _rotationAmount;

        private const float RAD2DEG = 180f / MathF.PI;

        public KGArcShape(KGGrid grid, int x, int y, Color color)
            : base(grid, x, y, color)
        {
            _angleOffset    = RandomAngleOffset();
            _arc            = 2f;
            _clockwise      = true;
            _rotationAmount = 0.25f;
        }

        private static float RandomAngleOffset()
        {
            float[] angles = { 0f, 0.25f, 0.5f, 0.75f };
            return angles[Random.Shared.Next(4)] * MathF.PI * 2f;
        }

        private struct ArcGeo
        {
            public float CenterX, CenterY;
            public float EndAngle, InitAngle;
            public float StartAngle, TailAngle;  // head and tail arc positions
            public float ArcRadius, ArcCenterX, ArcCenterY;
            public float CellSize, LineWidth;
            public float RotationDirection;
            public float OffsetX, OffsetY;
        }

        private ArcGeo ComputeGeometry(KGGridConfig config, float angleOffset, float arc)
        {
            float cs  = config.CellSize;
            float ox  = config.OffsetX, oy = config.OffsetY;
            float lw  = config.LineWidth;

            float centerX = ox + X * cs + cs / 2f;
            float centerY = oy + Y * cs + cs / 2f;

            float endAngle  = angleOffset;
            float initAngle = endAngle - MathF.PI;
            float rotDir    = _clockwise ? 1f : -1f;
            float rotAngle  = 2f * MathF.PI * _rotationAmount;

            float startAngle = initAngle + rotAngle * Progress     * rotDir;
            float tailAngle  = initAngle + rotAngle * TailProgress * rotDir;

            float arcRadius  = cs * arc;
            float arcCenterX = centerX + cs * arc * MathF.Cos(endAngle);
            float arcCenterY = centerY + cs * arc * MathF.Sin(endAngle);

            return new ArcGeo
            {
                CenterX = centerX,      CenterY = centerY,
                EndAngle = endAngle,    InitAngle = initAngle,
                StartAngle = startAngle, TailAngle = tailAngle,
                ArcRadius = arcRadius,
                ArcCenterX = arcCenterX, ArcCenterY = arcCenterY,
                CellSize = cs,           LineWidth = lw,
                RotationDirection = rotDir,
                OffsetX = ox,            OffsetY = oy
            };
        }

        private ArcGeo ComputeGeometry(KGGridConfig config) => ComputeGeometry(config, _angleOffset, _arc);

        private void DrawArcElements(Graphics g, ArcGeo geo, Color color, float width)
        {
            float halfW    = width / 2f;
            float rotAngle = 2f * MathF.PI * _rotationAmount;

            // Arc stroke spans from tailAngle (tail) to startAngle (head)
            float arcStartDeg = geo.TailAngle  * RAD2DEG;
            float arcSweepDeg = (geo.StartAngle - geo.TailAngle) * RAD2DEG;

            // Head cap center
            float hx = geo.ArcCenterX + geo.ArcRadius * MathF.Cos(geo.StartAngle);
            float hy = geo.ArcCenterY + geo.ArcRadius * MathF.Sin(geo.StartAngle);

            // Tail cap center
            float tx = geo.ArcCenterX + geo.ArcRadius * MathF.Cos(geo.TailAngle);
            float ty = geo.ArcCenterY + geo.ArcRadius * MathF.Sin(geo.TailAngle);

            using var pen = new Pen(color, geo.LineWidth) { StartCap = LineCap.Round, EndCap = LineCap.Round };

            // Outer arc
            float outerR = geo.ArcRadius + halfW;
            if (outerR > 0)
                g.DrawArc(pen,
                    geo.ArcCenterX - outerR, geo.ArcCenterY - outerR, outerR * 2f, outerR * 2f,
                    arcStartDeg, arcSweepDeg);

            // Inner arc
            float innerR = MathF.Max(0.001f, geo.ArcRadius - halfW);
            g.DrawArc(pen,
                geo.ArcCenterX - innerR, geo.ArcCenterY - innerR, innerR * 2f, innerR * 2f,
                arcStartDeg, arcSweepDeg);

            if (halfW <= 0) return;

            // Head cap (semicircle at leading edge)
            float headCapStart = (_clockwise ? geo.StartAngle : geo.StartAngle - MathF.PI) * RAD2DEG;
            g.DrawArc(pen, hx - halfW, hy - halfW, halfW * 2f, halfW * 2f, headCapStart, 180f);

            // Tail cap (semicircle at trailing edge)
            float tailCapStart = (_clockwise ? geo.TailAngle - MathF.PI : geo.TailAngle) * RAD2DEG;
            g.DrawArc(pen, tx - halfW, ty - halfW, halfW * 2f, halfW * 2f, tailCapStart, 180f);
        }

        public override void Draw(Graphics g, KGGridConfig config)
        {
            var geo      = ComputeGeometry(config);
            float baseSize = config.CellSize * 0.8f;
            DrawArcElements(g, geo, Color, baseSize);
            DrawArcElements(g, geo, System.Drawing.Color.White, baseSize - geo.LineWidth * 4f);
        }

        public override void UpdatePosition(KGGrid grid)
        {
            if (!IsMoving) return;

            if (Progress < 1f)
            {
                Progress = MathF.Min(1f, Progress + Speed);
                if (Progress >= 1f) HasReachedTarget = true;
            }
            else if (TailProgress < 1f)
            {
                TailProgress = MathF.Min(1f, TailProgress + Speed);
            }
        }

        // Called by engine after all shapes update to finalize arc position
        public void FinalizeProgress(KGGridConfig config, KGGrid grid)
        {
            if (!IsMoving || TailProgress < 1f) return;

            var geo      = ComputeGeometry(config);
            float rotAngle   = 2f * MathF.PI * _rotationAmount;
            float finalAngle = geo.EndAngle - MathF.PI + rotAngle * geo.RotationDirection;

            int newX = (int)MathF.Floor((geo.ArcCenterX + geo.ArcRadius * MathF.Cos(finalAngle) - geo.OffsetX) / geo.CellSize);
            int newY = (int)MathF.Floor((geo.ArcCenterY + geo.ArcRadius * MathF.Sin(finalAngle) - geo.OffsetY) / geo.CellSize);

            OnMoveComplete(grid, newX, newY);
            Progress = TailProgress = 0f;
        }

        public override void CalculateNewTarget(KGGrid grid, KGGridConfig config)
        {
            if (IsMoving) return;

            int maxArc = Math.Min(config.NbrColumns, config.NbrRows) / 2 - 1;
            if (maxArc < 1) return;

            float newOffset = RandomAngleOffset();
            float newArc    = Random.Shared.Next(1, maxArc + 1);
            bool  newCW     = Random.Shared.Next(2) == 0;
            float[] rotAmounts = { 0.25f, 0.5f, 0.75f };
            float newRot    = rotAmounts[Random.Shared.Next(3)];

            // Save current params
            float savedOffset = _angleOffset, savedArc = _arc;
            bool  savedCW     = _clockwise;
            float savedRot    = _rotationAmount;

            // Temporarily apply candidate params to calculate endpoint
            _angleOffset    = newOffset;
            _arc            = newArc;
            _clockwise      = newCW;
            _rotationAmount = newRot;

            var geo      = ComputeGeometry(config);
            float rotAngle   = 2f * MathF.PI * newRot;
            float finalAngle = geo.EndAngle - MathF.PI + rotAngle * (newCW ? 1f : -1f);

            int endX = (int)MathF.Floor((geo.ArcCenterX + geo.ArcRadius * MathF.Cos(finalAngle) - geo.OffsetX) / geo.CellSize);
            int endY = (int)MathF.Floor((geo.ArcCenterY + geo.ArcRadius * MathF.Sin(finalAngle) - geo.OffsetY) / geo.CellSize);

            bool valid = grid.IsValid(endY, endX)
                      && grid.IsEmpty(endY, endX)
                      && (endX != (int)X || endY != (int)Y);

            if (!valid)
            {
                _angleOffset    = savedOffset;
                _arc            = savedArc;
                _clockwise      = savedCW;
                _rotationAmount = savedRot;
                return;
            }

            MoveTo(grid, config, endX, endY);
        }

        protected override List<GridPoint> GenLockPath(KGGrid grid, KGGridConfig config, int tx, int ty)
        {
            var geo    = ComputeGeometry(config);
            int steps  = Math.Max(10, (int)(_arc * _arc * (_rotationAmount * 8f)));
            float halfW   = config.CellSize * 0.5f - config.LineWidth * 0.51f;
            float rotAngle = 2f * MathF.PI * _rotationAmount;

            var path = new List<GridPoint>();
            var seen = new System.Collections.Generic.HashSet<GridPoint>();

            for (int i = 0; i <= steps; i++)
            {
                float t     = (float)i / steps;
                float angle = geo.EndAngle - MathF.PI + rotAngle * t * geo.RotationDirection;

                float cx = geo.ArcCenterX + geo.ArcRadius * MathF.Cos(angle);
                float cy = geo.ArcCenterY + geo.ArcRadius * MathF.Sin(angle);
                float innerR = MathF.Max(0.001f, geo.ArcRadius - halfW);
                float ix = geo.ArcCenterX + innerR * MathF.Cos(angle);
                float iy = geo.ArcCenterY + innerR * MathF.Sin(angle);
                float ox2 = geo.ArcCenterX + (geo.ArcRadius + halfW) * MathF.Cos(angle);
                float oy2 = geo.ArcCenterY + (geo.ArcRadius + halfW) * MathF.Sin(angle);

                foreach (var (wx, wy) in new[] { (cx, cy), (ix, iy), (ox2, oy2) })
                {
                    int gc = (int)MathF.Floor((wx - geo.OffsetX) / geo.CellSize);
                    int gr = (int)MathF.Floor((wy - geo.OffsetY) / geo.CellSize);

                    if (!grid.IsValid(gr, gc)) return null;

                    var pt = new GridPoint(gc, gr);
                    if (seen.Add(pt)) path.Add(pt);
                }
            }
            return path;
        }
    }
}
