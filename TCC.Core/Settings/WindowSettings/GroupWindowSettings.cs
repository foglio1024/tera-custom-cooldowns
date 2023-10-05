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
            if (_hpLabelMode == value) return;
            _hpLabelMode = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public bool IgnoreMe
    {
        get => _ignoreMe;
        set
        {
            if (_ignoreMe == value) return;
            _ignoreMe = value;
            N();
            IgnoreMeChanged?.Invoke();
        }
    }
    public uint HideBuffsThreshold
    {
        get => _hideBuffsThreshold;
        set
        {
            if (_hideBuffsThreshold == value) return;
            _hideBuffsThreshold = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideDebuffsThreshold
    {
        get => _hideDebuffsThreshold;
        set
        {
            if (_hideDebuffsThreshold == value) return;
            _hideDebuffsThreshold = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideHpThreshold
    {
        get => _hideHpThreshold;
        set
        {
            if (_hideHpThreshold == value) return;
            _hideHpThreshold = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideMpThreshold
    {
        get => _hideMpThreshold;
        set
        {
            if (_hideMpThreshold == value) return;
            _hideMpThreshold = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public uint HideStThreshold
    {
        get => _hideStThreshold;
        set
        {
            if (_hideStThreshold == value) return;
            _hideStThreshold = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public uint DisableAbnormalitiesThreshold
    {
        get => _disableAbnormalitiesThreshold;
        set
        {
            if (_disableAbnormalitiesThreshold == value) return;
            _disableAbnormalitiesThreshold = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public uint GroupSizeThreshold
    {
        get => _groupSizeThreshold;
        set
        {
            if (_groupSizeThreshold == value) return;
            _groupSizeThreshold = value;
            N();
            ThresholdChanged?.Invoke();
        }
    }
    public GroupWindowLayout Layout
    {
        get => _layout;
        set
        {
            if (_layout == value) return;
            _layout = value;
            LayoutChanged?.Invoke();
            N();
        }
    }
    public bool ShowAwakenIcon
    {
        get => _showAwakenIcon;
        set
        {
            if (_showAwakenIcon == value) return;
            _showAwakenIcon = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public bool ShowDetails
    {
        get => _showDetails;
        set
        {
            if (_showDetails == value) return;
            _showDetails = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public bool ShowLaurels
    {
        get => _showLaurels;
        set
        {
            if (_showLaurels == value) return;
            _showLaurels = value;
            N();
            SettingsUpdated?.Invoke();
        }
    }
    public bool ShowOnlyAggroStacks
    {
        get => _showOnlyAggroStacks;
        set
        {
            if (_showOnlyAggroStacks == value) return;
            _showOnlyAggroStacks = value;
            N();
        }
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