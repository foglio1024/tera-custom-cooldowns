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

    private bool _warriorShowEdge;
    private bool _warriorShowInfuriate;
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
            if (!RaiseAndSetIfChanged(value, ref _warriorShowEdge)) return;
            WarriorShowEdgeChanged?.Invoke();
        }
    }
    public bool WarriorShowInfuriate
    {
        get => _warriorShowInfuriate;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _warriorShowInfuriate)) return;
            WarriorShowInfuriateChanged?.Invoke();
        }
    }
    public bool ValkyrieShowRagnarok
    {
        get => _valkyrieShowRagnarok;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _valkyrieShowRagnarok)) return;
            ValkyrieShowRagnarokChanged?.Invoke();
        }
    }
    public bool ValkyrieShowGodsfall
    {
        get => _valkyrieShowGodsfall;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _valkyrieShowGodsfall)) return;
            ValkyrieShowGodsfallChanged?.Invoke();
        }
    }
    public bool SorcererShowElements
    {
        get => _sorcererShowElements;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _sorcererShowElements)) return;
            SorcererShowElementsChanged?.Invoke();
        }
    }
    public bool WarriorShowTraverseCut
    {
        get => _warriorShowTraverseCut;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _warriorShowTraverseCut)) return;
            WarriorShowTraverseCutChanged?.Invoke();

        }
    }
    public bool FlashAvailableSkills
    {
        get => _flashAvailableSkills;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _flashAvailableSkills)) return;
            FlashAvailableSkillsChanged?.Invoke();
        }
    }
    public WarriorEdgeMode WarriorEdgeMode
    {
        get => _warriorEdgeMode;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _warriorEdgeMode)) return;
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