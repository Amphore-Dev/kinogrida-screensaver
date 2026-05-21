using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kinogrida
{
    public class SettingsForm : Form
    {
        private readonly TrackBar _paletteTrack;
        private readonly TrackBar _fadeTrack;
        private readonly TrackBar _speedTrack;
        private readonly TrackBar _fillTrack;
        private readonly Label _paletteLabel;
        private readonly Label _fadeLabel;
        private readonly Label _speedLabel;
        private readonly Label _fillLabel;

        public SettingsForm()
        {
            Text = "Kinogrida — Settings";
            ClientSize = new Size(400, 360);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            _paletteLabel = MakeLabel("", 20, 15);
            _paletteTrack = MakeTrack(20, 35, 10, 300, (int)Settings.PaletteDuration);
            _paletteTrack.ValueChanged += (s, e) => UpdateLabels();

            _fadeLabel = MakeLabel("", 20, 95);
            _fadeTrack = MakeTrack(20, 115, 1, 30, (int)Math.Round(Settings.FadeDuration * 10));
            _fadeTrack.ValueChanged += (s, e) => UpdateLabels();

            _speedLabel = MakeLabel("", 20, 175);
            _speedTrack = MakeTrack(20, 195, 1, 30, (int)Settings.AnimSpeed);
            _speedTrack.ValueChanged += (s, e) => UpdateLabels();

            _fillLabel = MakeLabel("", 20, 255);
            _fillTrack = MakeTrack(20, 275, 5, 50, Settings.FillPercent);
            _fillTrack.ValueChanged += (s, e) => UpdateLabels();

            var ok     = new Button { Text = "OK",     DialogResult = DialogResult.OK,     Size = new Size(80, 28), Location = new Point(220, 320) };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Size = new Size(80, 28), Location = new Point(310, 320) };

            ok.Click += (s, e) =>
            {
                Settings.PaletteDuration = _paletteTrack.Value;
                Settings.FadeDuration    = _fadeTrack.Value / 10.0;
                Settings.AnimSpeed       = _speedTrack.Value;
                Settings.FillPercent     = _fillTrack.Value;
                Close();
            };
            cancel.Click += (s, e) => Close();

            AcceptButton = ok;
            CancelButton = cancel;

            Controls.AddRange(new Control[]
            {
                _paletteLabel, _paletteTrack,
                _fadeLabel,    _fadeTrack,
                _speedLabel,   _speedTrack,
                _fillLabel,    _fillTrack,
                ok, cancel
            });
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            _paletteLabel.Text = $"Palette duration: {_paletteTrack.Value} s";
            _fadeLabel.Text    = $"Fade duration: {_fadeTrack.Value / 10.0:0.0} s";
            _speedLabel.Text   = $"Animation speed: {_speedTrack.Value} s per cycle";
            _fillLabel.Text    = $"Grid fill: {_fillTrack.Value} %";
        }

        private static Label MakeLabel(string text, int x, int y) => new Label
        {
            Text = text, Location = new Point(x, y), AutoSize = true
        };

        private static TrackBar MakeTrack(int x, int y, int min, int max, int value) => new TrackBar
        {
            Location = new Point(x, y), Size = new Size(360, 45),
            Minimum = min, Maximum = max,
            Value = Math.Max(min, Math.Min(max, value)),
            TickFrequency = (max - min) / 10
        };
    }
}
