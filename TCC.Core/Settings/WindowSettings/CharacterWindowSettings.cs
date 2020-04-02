using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class CharacterWindowSettings : WindowSettingsBase
    {
        public event Action SorcererShowElementsChanged;
        public event Action WarriorShowEdgeChanged;
        public event Action ShowStaminaChanged;

        private bool _sorcererShowElements;
        private bool _warriorShowEdge;
        private bool _compactMode;
        private bool _showStamina;

        public bool CompactMode
        {
            get => _compactMode;
            set
            {
                if (_compactMode == value) return;
                _compactMode = value;
                N();
            }
        }
        public bool SorcererShowElements
        {
            get => _sorcererShowElements;
            set
            {
                if (_sorcererShowElements == value) return;
                _sorcererShowElements = value;
                SorcererShowElementsChanged?.Invoke();
                N();
            }
        }
        public bool WarriorShowEdge
        {
            get => _warriorShowEdge;
            set
            {
                if (_warriorShowEdge == value) return;
                _warriorShowEdge = value;
                WarriorShowEdgeChanged?.Invoke();
                N();
            }
        }
        public bool ShowStamina
        {
            get => _showStamina;
            set
            {
                if (_showStamina == value) return;
                _showStamina = value;
                ShowStaminaChanged?.Invoke();
                N();
            }
        }

        public CharacterWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.4, 1, ButtonsPosition.Above);

            CompactMode = true;
            UndimOnFlyingGuardian = false;
            GpkNames.Add("CharacterWindow");

        }
    }
}