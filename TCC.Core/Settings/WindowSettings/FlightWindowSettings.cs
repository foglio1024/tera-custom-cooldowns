using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class FlightWindowSettings : WindowSettingsBase
    {
        public event Action RotationChanged = null!;
        public event Action FlipChanged = null!;

        private bool _flip;
        private double _rotation;

        public bool Flip
        {
            get => _flip;
            set
            {
                if (_flip == value) return;
                _flip = value;
                N();
                FlipChanged?.Invoke();
            }
        }

        public double Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value) return;
                _rotation = value;
                N();
                RotationChanged?.Invoke();
            }
        }

        public FlightWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = false;
            _dimOpacity = 1;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.5, .5, ButtonsPosition.Above);

            PerClassPosition = false;

            Rotation = 0;
            Flip = false;

            GpkNames.Add("ProgressBar");
        }

        protected override void OnEnabledChanged(bool enabled)
        {
            //TODO: add specific code
        }
    }
}