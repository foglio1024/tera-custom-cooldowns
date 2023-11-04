using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings;

public class ClassWindowSettings : WindowSettingsBase
{
    public event Action? WarriorShowEdgeChanged;
    public event Action? WarriorEdgeModeChanged;
    public event Action? WarriorShowTraverseCutChanged;
    public event Action? WarriorShowInfuriateChanged;
    public event Action? ValkyrieShowRagnarokChanged;
    public event Action? ValkyrieShowGodsfallChanged;
    public event Action? SorcererShowElementsChanged;
    public event Action? FlashAvailableSkillsChanged;

    bool _warriorShowEdge;
    bool _warriorShowInfuriate;
    bool _warriorShowTraverseCut;
    bool _sorcererShowElements;
    bool _valkyrieShowRagnarok;
    bool _valkyrieShowGodsfall;
    bool _flashAvailableSkills;

    WarriorEdgeMode _warriorEdgeMode;

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
    public bool WarriorShowInfuriate
    {
        get => _warriorShowInfuriate;
        set
        {
            if (_warriorShowInfuriate == value) return;
            _warriorShowInfuriate = value;
            N();
            WarriorShowInfuriateChanged?.Invoke();
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