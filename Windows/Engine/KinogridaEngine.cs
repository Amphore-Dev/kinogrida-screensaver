using System;
using System.Collections.Generic;
using System.Drawing;

namespace Kinogrida
{
    class KinogridaEngine
    {
        private KGGridConfig _config;
        private KGGrid       _grid;
        private List<KGBaseShape> _shapes = new List<KGBaseShape>();
        private float _canvasW, _canvasH;

        private float _timeAccumulator;
        private float _fadeAlpha;
        private FadeState _fadeState = FadeState.Normal;

        private readonly bool _isPreview;

        private const float kCellPixelSize   = 100f;
        private const float kPreviewCellSize = 10f;
        private const float kPreviewSpeed    = 3000f;

        private float ChangeEvery   => _isPreview ? 10000f          : (float)(Settings.PaletteDuration * 1000.0);
        private float FadeDuration  => _isPreview ? 600f            : (float)(Settings.FadeDuration    * 1000.0);
        private float AnimSpeed     => _isPreview ? kPreviewSpeed   : (float)(Settings.AnimSpeed       * 1000.0);
        private float FillPercent   => _isPreview ? 0.20f           : Settings.FillPercent / 100f;
        private float CellPixelSize => _isPreview ? kPreviewCellSize : kCellPixelSize;

        private enum FadeState { Normal, FadingOut, FadingIn }

        public KinogridaEngine(float canvasW, float canvasH, bool isPreview = false)
        {
            _isPreview = isPreview;
            _config    = KGGridConfig.Default();
            _grid      = new KGGrid(1, 1);
            Rebuild(canvasW, canvasH);
        }

        public void UpdateBounds(float canvasW, float canvasH)
        {
            _timeAccumulator = 0f;
            _fadeAlpha       = 0f;
            _fadeState       = FadeState.Normal;
            Rebuild(canvasW, canvasH);
        }

        private void Rebuild(float canvasW, float canvasH)
        {
            _canvasW = canvasW; _canvasH = canvasH;

            _config.NbrColumns = Math.Max(1, (int)MathF.Floor(canvasW / CellPixelSize));
            _config.NbrRows    = Math.Max(1, (int)MathF.Floor(canvasH / CellPixelSize));
            _config.Speed      = AnimSpeed;
            _config.Colors     = KGColors.RandomPalette();
            _config.UpdateForCanvasSize(canvasW, canvasH);

            _shapes.Clear();
            _grid = new KGGrid(_config.NbrRows, _config.NbrColumns);
            FillGridRandomly();
        }

        private void Rebuild()
        {
            _config.Colors = KGColors.RandomPalette();
            _shapes.Clear();
            _grid = new KGGrid(_config.NbrRows, _config.NbrColumns);
            FillGridRandomly();
        }

        private void FillGridRandomly()
        {
            int maxCells = Math.Max(1, (int)MathF.Floor(_config.NbrColumns * _config.NbrRows * FillPercent));
            int added = 0, attempts = 0;

            while (added < maxCells && attempts < 100)
            {
                int x = Random.Shared.Next(0, _config.NbrColumns);
                int y = Random.Shared.Next(0, _config.NbrRows);

                if (_grid.IsEmpty(y, x))
                {
                    var color = _config.Colors[Random.Shared.Next(_config.Colors.Length)];
                    var shape = RandomShape(x, y, color);
                    _grid.SetShape(y, x, shape);
                    _shapes.Add(shape);
                    added++;
                    attempts = 0;
                }
                else
                {
                    attempts++;
                }
            }
        }

        private KGBaseShape RandomShape(int x, int y, Color color)
        {
            if (Random.Shared.Next(2) == 0)
                return new KGSquareShape(_grid, x, y, color, Random.Shared.Next(2) == 0 ? 1f : 0f);
            else
                return new KGArcShape(_grid, x, y, color);
        }

        public void Update(float deltaTime)
        {
            switch (_fadeState)
            {
                case FadeState.Normal:
                    _timeAccumulator += deltaTime;
                    if (_timeAccumulator >= ChangeEvery)
                    {
                        _timeAccumulator = 0f;
                        _fadeAlpha       = 0f;
                        _fadeState       = FadeState.FadingOut;
                    }
                    UpdateShapes(deltaTime);
                    break;

                case FadeState.FadingOut:
                    _fadeAlpha = MathF.Min(1f, _fadeAlpha + deltaTime / FadeDuration);
                    if (_fadeAlpha >= 1f)
                    {
                        Rebuild();
                        _fadeState = FadeState.FadingIn;
                    }
                    UpdateShapes(deltaTime);
                    break;

                case FadeState.FadingIn:
                    _fadeAlpha = MathF.Max(0f, _fadeAlpha - deltaTime / FadeDuration);
                    if (_fadeAlpha <= 0f) _fadeState = FadeState.Normal;
                    UpdateShapes(deltaTime);
                    break;
            }
        }

        private void UpdateShapes(float deltaTime)
        {
            foreach (var shape in _shapes)
                shape.Update(_grid, _config, deltaTime);

            foreach (var shape in _shapes)
                if (shape is KGArcShape arc)
                    arc.FinalizeProgress(_config, _grid);
        }

        public void Draw(Graphics g)
        {
            foreach (var shape in _shapes)
                shape.Draw(g, _config);

            if (_fadeAlpha > 0f)
            {
                int a = (int)(_fadeAlpha * 255f);
                using var brush = new SolidBrush(Color.FromArgb(a, 0, 0, 0));
                g.FillRectangle(brush, 0, 0, _canvasW, _canvasH);
            }
        }
    }
}
