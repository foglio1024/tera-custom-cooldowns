using System;
using System.Collections.Generic;
using TCC.Data;
using TCC.UI.Windows.Widgets;
using TeraDataLite;

namespace TCC.Settings.WindowSettings;

public class GroupWindowSettings : WindowSettingsBase
{
    public event Action? SettingsUpdated;
    public event Action? IgnoreMeChanged;
    public event Action? ThresholdChanged;
    public event Action? LayoutChanged;

    GroupHpLabelMode _hpLabelMode;
    bool _ignoreMe;
    uint _hideBuffsThreshold;
    uint _hideDebuffsThreshold;
    uint _hideHpThreshold;
    uint _hideMpThreshold;
    uint _hideStThreshold;
    uint _disableAbnormalitiesThreshold;
    uint _disableAbnormalitiesAnimationThreshold;
    uint _groupSizeThreshold;
    GroupWindowLayout _layout;
    bool _showAwakenIcon;
    bool _showDetails;
    bool _showLaurels;
    bool _showOnlyAggroStacks;

    public GroupHpLabelMode HpLabelMode
    {
        get => _hpLabelMode;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hpLabelMode)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public bool IgnoreMe
    {
        get => _ignoreMe;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _ignoreMe)) return;
            IgnoreMeChanged?.Invoke();
        }
    }
    public uint HideBuffsThreshold
    {
        get => _hideBuffsThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hideBuffsThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideDebuffsThreshold
    {
        get => _hideDebuffsThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hideDebuffsThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideHpThreshold
    {
        get => _hideHpThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hideHpThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideMpThreshold
    {
        get => _hideMpThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hideMpThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideStThreshold
    {
        get => _hideStThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hideStThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint DisableAbnormalitiesThreshold
    {
        get => _disableAbnormalitiesThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _disableAbnormalitiesThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint DisableAbnormalitiesAnimationThreshold
    {
        get => _disableAbnormalitiesAnimationThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _disableAbnormalitiesAnimationThreshold)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public uint GroupSizeThreshold
    {
        get => _groupSizeThreshold;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _groupSizeThreshold)) return;
            ThresholdChanged?.Invoke();
        }
    }
    public GroupWindowLayout Layout
    {
        get => _layout;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _layout)) return;
            LayoutChanged?.Invoke();
        }
    }
    public bool ShowAwakenIcon
    {
        get => _showAwakenIcon;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showAwakenIcon)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public bool ShowDetails
    {
        get => _showDetails;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showDetails)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public bool ShowLaurels
    {
        get => _showLaurels;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showLaurels)) return;
            SettingsUpdated?.Invoke();
        }
    }
    public bool ShowOnlyAggroStacks
    {
        get => _showOnlyAggroStacks;
        set => RaiseAndSetIfChanged(value, ref _showOnlyAggroStacks);
    }
    public bool ShowAllAbnormalities { get; set; }

    public Dictionary<Class, List<uint>> GroupAbnormals { get; }
    public List<uint> Hidden { get; set; }

    public GroupWindowSettings()
    {
        _visible = true;
        _clickThruMode = ClickThruMode.Never;
        _scale = 1;
        _autoDim = true;
        _dimOpacity = .5;
        _showAlways = false;
        _enabled = true;
        _allowOffScreen = false;
        Positions = new ClassPositions(0, 0, ButtonsPosition.Above);
        Hidden = new List<uint>();

        UndimOnFlyingGuardian = false;
        IgnoreSize = false;

        GroupSizeThreshold = 7;
        HideBuffsThreshold = 7;
        HideDebuffsThreshold = 7;
        DisableAbnormalitiesThreshold = 7;
        HideHpThreshold = 7;
        HideMpThreshold = 7;
        ShowOnlyAggroStacks = true;
        ShowDetails = true;
        ShowAwakenIcon = true;
        HpLabelMode = GroupHpLabelMode.Percentage;
        Layout = GroupWindowLayout.RoleSeparated;

        GroupAbnormals = new Dictionary<Class, List<uint>>
        {
            {       0, new List<uint>()},
            {(Class)1, new List<uint>()},
            {(Class)2, new List<uint>()},
            {(Class)3, new List<uint>()},
            {(Class)4, new List<uint>()},
            {(Class)5, new List<uint>()},
            {(Class)6, new List<uint>()},
            {(Class)7, new List<uint>()},
            {(Class)8, new List<uint>()},
            {(Class)9, new List<uint>()},
            {(Class)10, new List<uint>()},
            {(Class)11, new List<uint>()},
            {(Class)12, new List<uint>()},
            {(Class)255, new List<uint>()}
        };

        GpkNames.Add("PartyWindow");
        GpkNames.Add("PartyWindowRaidInfo");

    }
}