using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class CooldownWindowSettings : WindowSettingsBase
    {
        public event Action ShowItemsChanged;
        public event Action ModeChanged;

        private bool _showItems;
        private CooldownBarMode _mode;

        public bool ShowItems
        {
            get => _showItems;
            set
            {
                if (_showItems == value) return;
                _showItems = value;
                N();
                ShowItemsChanged?.Invoke();
            }
        }

        public CooldownBarMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                N();
                ModeChanged?.Invoke();
            }
        }

        public CooldownWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.4, .7, ButtonsPosition.Above);

            Mode = CooldownBarMode.Fixed;
            ShowItems = true;
            UndimOnFlyingGuardian = false;

        }
    }
}