using System;
using TCC.Data;
using TCC.Windows.Widgets;

namespace TCC.Settings.WindowSettings
{
    public class ClassWindowSettings : WindowSettingsBase
    {
        public event Action WarriorShowEdgeChanged;
        public event Action WarriorEdgeModeChanged;
        public event Action WarriorShowTraverseCutChanged;
        public event Action SorcererShowElementsChanged;

        private bool _warriorShowEdge;
        private bool _sorcererShowElements;
        private bool _warriorShowTraverseCut;
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

            WarriorShowTraverseCut = true;
            WarriorShowEdge = true;
            WarriorEdgeMode = WarriorEdgeMode.Rhomb;
            SorcererShowElements = true;

        }
    }
}