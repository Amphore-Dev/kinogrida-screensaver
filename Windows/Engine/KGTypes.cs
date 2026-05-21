using System;

namespace Kinogrida
{
    struct GridPoint : IEquatable<GridPoint>
    {
        public int Col, Row;
        public GridPoint(int col, int row) { Col = col; Row = row; }
        public bool Equals(GridPoint other) => Col == other.Col && Row == other.Row;
        public override bool Equals(object obj) => obj is GridPoint g && Equals(g);
        public override int GetHashCode() => HashCode.Combine(Col, Row);
    }

    enum CellState { Empty, Locked, Shape }

    class KGCell
    {
        public CellState State = CellState.Empty;
        public KGBaseShape Shape = null;
    }

    class KGGrid
    {
        private readonly KGCell[,] _cells;
        public int Rows { get; }
        public int Cols { get; }

        public KGGrid(int rows, int cols)
        {
            Rows = rows; Cols = cols;
            _cells = new KGCell[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    _cells[r, c] = new KGCell();
        }

        public CellState GetState(int row, int col) => _cells[row, col].State;
        public KGBaseShape GetShape(int row, int col) => _cells[row, col].Shape;

        public void SetEmpty(int row, int col)
        {
            _cells[row, col].State = CellState.Empty;
            _cells[row, col].Shape = null;
        }

        public void SetLocked(int row, int col)
        {
            _cells[row, col].State = CellState.Locked;
            _cells[row, col].Shape = null;
        }

        public void SetShape(int row, int col, KGBaseShape shape)
        {
            _cells[row, col].State = CellState.Shape;
            _cells[row, col].Shape = shape;
        }

        public bool IsEmpty(int row, int col) => _cells[row, col].State == CellState.Empty;
        public bool IsValid(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;
    }
}
