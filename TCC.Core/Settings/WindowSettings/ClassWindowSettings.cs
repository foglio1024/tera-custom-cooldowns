using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class ClassWindowSettings : WindowSettingsBase
    {
        public event Action WarriorShowEdgeChanged = null!;
        public event Action WarriorEdgeModeChanged = null!;
        public event Action WarriorShowTraverseCutChanged = null!;
        public event Action ValkyrieShowRagnarokChanged = null!;
        public event Action ValkyrieShowGodsfallChanged = null!;
        public event Action SorcererShowElementsChanged = null!;
        public event Action FlashAvailableSkillsChanged = null!;

        private bool _warriorShowEdge;
        private bool _warriorShowTraverseCut;
        private bool _sorcererShowElements;
        private bool _valkyrieShowRagnarok;
        private bool _valkyrieShowGodsfall;
        private bool _flashAvailableSkills;

        private WarriorEdgeMode _warriorEdgeMode;

        public bool WarriorShowEdge
        {
            get => _warriorShowEdge;
            set
            {
                if (_warriorShowEdge == value) return;
                _warriorShowEdge = value;
                N();
                WarriorShowEdgeChanged?.Invoke();
            }
        }
        public bool ValkyrieShowRagnarok
        {
            get => _valkyrieShowRagnarok;
            set
            {
                if (_valkyrieShowRagnarok == value) return;
                _valkyrieShowRagnarok = value;
                N();
                ValkyrieShowRagnarokChanged?.Invoke();
            }
        }
        public bool ValkyrieShowGodsfall
        {
            get => _valkyrieShowGodsfall;
            set
            {
                if (_valkyrieShowGodsfall == value) return;
                _valkyrieShowGodsfall = value;
                N();
                ValkyrieShowGodsfallChanged?.Invoke();
            }
        }
        public bool SorcererShowElements
        {
            get => _sorcererShowElements;
            set
            {
                if (_sorcererShowElements == value) return;
                _sorcererShowElements = value;
                N();
                SorcererShowElementsChanged?.Invoke();
            }
        }
        public bool WarriorShowTraverseCut
        {
            get => _warriorShowTraverseCut;
            set
            {
                if (_warriorShowTraverseCut == value) return;
                _warriorShowTraverseCut = value;
                N();
                WarriorShowTraverseCutChanged?.Invoke();

            }
        }
        public bool FlashAvailableSkills
        {
            get => _flashAvailableSkills;
            set
            {
                if (_flashAvailableSkills == value) return;
                _flashAvailableSkills = value;
                N();
                FlashAvailableSkillsChanged?.Invoke();
            }
        }
        public WarriorEdgeMode WarriorEdgeMode
        {
            get => _warriorEdgeMode;
            set
            {
                if (_warriorEdgeMode == value) return;
                _warriorEdgeMode = value;
                N();
                WarriorEdgeModeChanged?.Invoke();
            }
        }

        public ClassWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(.25, .6, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;

            FlashAvailableSkills = true;

            WarriorShowTraverseCut = true;
            WarriorShowEdge = true;
            WarriorEdgeMode = WarriorEdgeMode.Rhomb;
            SorcererShowElements = true;

        }
    }
}