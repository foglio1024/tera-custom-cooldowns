using System;
using System.Collections.Generic;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.UI.Windows.Widgets;
using TeraDataLite;

namespace TCC.Settings.WindowSettings;

public class GroupWindowSettings : WindowSettingsBase
{
    public event Action? SettingsUpdated;
    public event Action? IgnoreMeChanged;
    public event Action? ThresholdChanged;
    public event Action? LayoutChanged;

    private GroupHpLabelMode _hpLabelMode;
    private bool _ignoreMe;
    private uint _hideBuffsThreshold;
    private uint _hideDebuffsThreshold;
    private uint _hideHpThreshold;
    private uint _hideMpThreshold;
    private uint _hideStThreshold;
    private uint _disableAbnormalitiesThreshold;
    private uint _disableAbnormalitiesAnimationThreshold;
    private uint _groupSizeThreshold;
    private GroupWindowLayout _layout;
    private bool _showAwakenIcon;
    private bool _showDetails;
    private bool _showLaurels;
    private bool _showOnlyAggroStacks;

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

    [Obsolete]
    public bool ShowAllAbnormalities { get; set; }
    [Obsolete]
    public Dictionary<Class, List<uint>> GroupAbnormals { get; }
    [Obsolete]
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
        
        GpkNames.Add("PartyWindow");
        GpkNames.Add("PartyWindowRaidInfo");

    }
}