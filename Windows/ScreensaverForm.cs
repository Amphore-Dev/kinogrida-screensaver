using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Kinogrida
{
    public class ScreensaverForm : Form
    {
        private KinogridaEngine _engine;
        private DateTime _prevTime;

        private readonly Timer _timer;
        private Point _firstMousePos = Point.Empty;
        protected readonly bool IsPreviewMode;

        public ScreensaverForm(Rectangle bounds, bool isPreview = false)
        {
            IsPreviewMode = isPreview;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            ShowInTaskbar = false;
            TopMost = !isPreview;

            if (bounds != Rectangle.Empty)
                Bounds = bounds;

            if (!isPreview)
            {
                MouseMove  += OnMouseMove;
                MouseClick += (s, e) => ExitScreensaver();
                KeyDown    += (s, e) => ExitScreensaver();
            }

            _timer = new Timer { Interval = 33 }; // ~30 FPS
            _timer.Tick += OnTick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPreviewMode) Cursor.Hide();
            _engine   = new KinogridaEngine(ClientSize.Width, ClientSize.Height, IsPreviewMode);
            _prevTime = DateTime.UtcNow;
            _timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;
            float dt = (float)Math.Min((now - _prevTime).TotalMilliseconds, 100.0);
            _prevTime = now;
            _engine.Update(dt);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Black);
            _engine?.Draw(g);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_firstMousePos == Point.Empty) { _firstMousePos = e.Location; return; }
            if (Math.Abs(e.X - _firstMousePos.X) > 5 || Math.Abs(e.Y - _firstMousePos.Y) > 5)
                ExitScreensaver();
        }

        protected void ExitScreensaver()
        {
            _timer.Stop();
            Cursor.Show();
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
