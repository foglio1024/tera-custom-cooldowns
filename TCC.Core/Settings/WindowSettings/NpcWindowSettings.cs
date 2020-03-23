using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class NpcWindowSettings : WindowSettingsBase
    {
        private bool _accurateHp;
        private bool _hideAdds;
        private EnrageLabelMode _enrageLabelMode;

        public event Action AccurateHpChanged;
        public event Action HideAddsChanged;

        public bool HideAdds
        {
            get => _hideAdds;
            set
            {
                if (_hideAdds == value) return;
                _hideAdds = value;
                N();
                HideAddsChanged?.Invoke();
            }
        }

        public bool AccurateHp
        {
            get => _accurateHp;
            set
            {
                if (_accurateHp == value) return;
                _accurateHp = value;
                N();
                AccurateHpChanged?.Invoke();
            }
        }

        public EnrageLabelMode EnrageLabelMode
        {
            get => _enrageLabelMode;
            set
            {
                if (_enrageLabelMode == value) return;
                _enrageLabelMode = value;
                N();
            }
        }

        public NpcWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.4, 0, ButtonsPosition.Above);

            EnrageLabelMode = EnrageLabelMode.Remaining;
            AccurateHp = true;
            HideAdds = false;

            GpkNames.Add("GageBoss");
            GpkNames.Add("TargetInfo");
        }
    }
}