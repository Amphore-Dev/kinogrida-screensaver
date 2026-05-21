using System;
using System.Collections.Generic;
using System.Drawing;

namespace Kinogrida
{
    abstract class KGBaseShape
    {
        public float X, Y;
        public float Speed;
        public float MoveDistance;
        public float Progress;
        public float TailProgress;

        public long LastMoveTime;
        public long MoveDebounce;

        public bool IsMoving;
        public bool HasReachedTarget;
        public float TailX, TailY;
        public float OriginalX, OriginalY;
        public float TargetX, TargetY;

        public Color Color;

        protected List<GridPoint> LockedCells = new List<GridPoint>();

        protected KGBaseShape(KGGrid grid, int x, int y, Color color)
        {
            X = TailX = OriginalX = TargetX = x;
            Y = TailY = OriginalY = TargetY = y;
            Color = color;
            LastMoveTime = Environment.TickCount64;
            MoveDebounce = (long)(Random.Shared.NextSingle() * 10000f);
        }

        public virtual void Update(KGGrid grid, KGGridConfig config, float deltaTime)
        {
            Speed = (2f * deltaTime) / config.Speed;
            if (IsMoving)
                UpdatePosition(grid);
            else
                CalculateNewTarget(grid, config);
        }

        public virtual void CalculateNewTarget(KGGrid grid, KGGridConfig config) { }
        public virtual void UpdatePosition(KGGrid grid) { }
        public abstract void Draw(Graphics g, KGGridConfig config);

        protected void MoveTo(KGGrid grid, KGGridConfig config, int tx, int ty)
        {
            long now = Environment.TickCount64;
            if (IsMoving || (now - LastMoveTime) < MoveDebounce) return;

            var locked = GenLockPath(grid, config, tx, ty);
            if (locked == null) return;
            if (!LockCells(grid, locked, apply: false)) return;
            LockCells(grid, locked, apply: true);

            TargetX   = tx; TargetY   = ty;
            TailX     = X;  TailY     = Y;
            OriginalX = X;  OriginalY = Y;
            MoveDistance = MathF.Abs(tx - X) + MathF.Abs(ty - Y);
            IsMoving     = true;
            LastMoveTime = now;
        }

        protected virtual List<GridPoint> GenLockPath(KGGrid grid, KGGridConfig config, int tx, int ty)
        {
            return GetLineCells(new GridPoint((int)MathF.Round(X), (int)MathF.Round(Y)),
                                new GridPoint(tx, ty));
        }

        protected bool LockCells(KGGrid grid, List<GridPoint> cells, bool apply)
        {
            foreach (var pt in cells)
            {
                var state = grid.GetState(pt.Row, pt.Col);
                if (state == CellState.Empty)
                {
                    if (apply)
                    {
                        grid.SetLocked(pt.Row, pt.Col);
                        LockedCells.Add(pt);
                    }
                }
                else
                {
                    // Allow only the shape's own starting cell
                    if (pt.Row != (int)Y || pt.Col != (int)X) return false;
                }
            }
            return true;
        }

        protected void UnlockCells(KGGrid grid)
        {
            foreach (var pt in LockedCells)
                if (grid.IsValid(pt.Row, pt.Col))
                    grid.SetEmpty(pt.Row, pt.Col);
            LockedCells.Clear();
        }

        protected void OnMoveComplete(KGGrid grid, int newX, int newY)
        {
            int savedX = (int)OriginalX, savedY = (int)OriginalY;

            X = OriginalX = newX;
            Y = OriginalY = newY;
            IsMoving = false;
            LastMoveTime = Environment.TickCount64;
            MoveDebounce = (long)(Random.Shared.NextSingle() * 10000f);

            UnlockCells(grid);
            grid.SetEmpty(savedY, savedX);
            grid.SetShape(newY, newX, this);
            HasReachedTarget = false;
        }

        protected virtual List<GridPoint> GetLineCells(GridPoint from, GridPoint to)
        {
            return new List<GridPoint>();
        }
    }
}
