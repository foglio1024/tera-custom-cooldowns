using System;
using System.Collections.Generic;
using TCC.Interop.Proxy;

namespace TCC.Settings.WindowSettings;

public class LfgWindowSettings : WindowSettingsBase
{
    const int MinAutoPublicizeCooldown = 20;

    public event Action? HideTradeListingsChangedEvent;

    bool _hideTradeListings;
    int _minLevel;
    int _maxLevel;

    public int MinLevel
    {
        get => _minLevel;
        set
        {
            if (_minLevel == value) return;
            if (value < 1) value = 1;
            if (value > 70) value = 70;
            _minLevel = value;
            if (value > _maxLevel) MaxLevel = value;
        }
    }
    public int MaxLevel
    {
        get => _maxLevel;
        set
        {
            if (_maxLevel == value) return;
            if (value < 1) value = 1;
            if (value > 70) value = 70;
            _maxLevel = value;
            if (value < _minLevel) MinLevel = value;
        }
    }
    public bool HideTradeListings
    {
        get => _hideTradeListings;
        set
        {
            if (_hideTradeListings == value) return;
            _hideTradeListings = value;
            N();
            HideTradeListingsChangedEvent?.Invoke();
        }
    }

    int _autoPublicizeCooldown;

    public int AutoPublicizeCooldown
    {
        get => _autoPublicizeCooldown;
        set
        {
            if (_autoPublicizeCooldown == value) return;
            _autoPublicizeCooldown = value < MinAutoPublicizeCooldown ? MinAutoPublicizeCooldown : value;
            N();
        }
    }


    public List<string> BlacklistedWords { get; }

    public LfgWindowSettings()
    {
        BlacklistedWords = new List<string>();

        HideTradeListings = true;
        MinLevel = 60;
        MaxLevel = 70;

        AutoPublicizeCooldown = MinAutoPublicizeCooldown;

        GpkNames.Add("PartyBoard");
        GpkNames.Add("PartyBoardMemberInfo");
    }

    protected override void OnEnabledChanged(bool enabled)
    {
        StubInterface.Instance.StubClient.UpdateSetting("useLfg", enabled);
        // do nothing
    }
}