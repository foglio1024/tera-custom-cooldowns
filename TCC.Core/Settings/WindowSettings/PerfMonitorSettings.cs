using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class PerfMonitorSettings : WindowSettingsBase
    {
        public override bool Enabled { get => true; set { } }

        public PerfMonitorSettings()
        {
            _visible = false;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = false;
            _dimOpacity = 1;
            _showAlways = true;
            _enabled = true;
            _allowOffScreen = false;
            PerClassPosition = false;
            Positions = new ClassPositions(1, .45, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;

        }
    }
}