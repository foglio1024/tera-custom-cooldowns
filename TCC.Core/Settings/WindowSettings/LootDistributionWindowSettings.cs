using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TCC.Data;
using TCC.UI;

namespace TCC.Settings.WindowSettings;

public class LootDistributionWindowSettings : WindowSettingsBase
{
    public event Action<int>? DelayChanged;
    public event Action? AutoRollPolicyChanged;

    bool _alwaysRoll;
    bool _alwaysPass;
    HotKey _toggleHotKey = new(Keys.L, ModifierKeys.Control);
    HotKey _rollHotKey = new(Keys.PageUp, ModifierKeys.None);
    HotKey _passHotKey = new(Keys.Next, ModifierKeys.None);
    int _autorollDelaySec = 10;
    bool _autoShowUponRoll;

    public int AutorollDelaySec
    {
        get => _autorollDelaySec;
        set
        {
            if (_autorollDelaySec == value) return;
            _autorollDelaySec = value;
            N();
            DelayChanged?.Invoke(value);
        }
    }

    public HotKey RollHotKey
    {
        get => _rollHotKey;
        set
        {
            if (_rollHotKey.Equals(value)) return;
            if (_passHotKey.Equals(value)) PassHotKey = new HotKey(Keys.None, ModifierKeys.None);

            _rollHotKey = value;
            N();
        }
    }

    public HotKey PassHotKey
    {
        get => _passHotKey;
        set
        {
            if (_passHotKey.Equals(value)) return;
            if (_rollHotKey.Equals(value)) RollHotKey = new HotKey(Keys.None, ModifierKeys.None);

            _passHotKey = value;
            N();
        }
    }

    public HotKey ToggleHotkey
    {
        get => _toggleHotKey;
        set
        {
            if (_toggleHotKey.Equals(value)) return;
            KeyboardHook.Instance.ChangeHotkey(_toggleHotKey, value);
            _toggleHotKey = value;
            N();
        }
    }

    public bool AlwaysRoll
    {
        get => _alwaysRoll;
        set
        {
            if (_alwaysRoll == value) return;
            _alwaysRoll = value;
            if (_alwaysPass == _alwaysRoll && value)
                _alwaysPass = !_alwaysRoll;
            N();
            N(nameof(AlwaysPass));
            AutoRollPolicyChanged?.Invoke();
        }
    }

    public bool AlwaysPass
    {
        get => _alwaysPass;
        set
        {
            if (_alwaysPass == value) return;
            _alwaysPass = value;
            if (_alwaysPass == _alwaysRoll && value)
                _alwaysRoll = !_alwaysPass;
            N();
            N(nameof(AlwaysRoll));
            AutoRollPolicyChanged?.Invoke();
        }
    }

    public bool AutoShowUponRoll
    {
        get => _autoShowUponRoll;
        set
        {
            if (_autoShowUponRoll == value) return;
            _autoShowUponRoll = value;
            N();
        }
    }

    bool _autoRollItemsBlacklistMode;

    public bool AutoRollItemsBlacklistMode
    {
        get => _autoRollItemsBlacklistMode;
        set
        {
            if (_autoRollItemsBlacklistMode == value) return;
            _autoRollItemsBlacklistMode = value;
            N();
        }
    }

    bool _autoPassItemsBlacklistMode;

    public bool AutoPassItemsBlacklistMode
    {
        get => _autoPassItemsBlacklistMode;
        set
        {
            if (_autoPassItemsBlacklistMode == value) return;
            _autoPassItemsBlacklistMode = value;
            N();
        }
    }

    public List<uint> AutoRollItems { get; set; } = new();
    public List<uint> AutoPassItems { get; set; } = new();

    public LootDistributionWindowSettings()
    {
        _visible = false;
        _enabled = true;
        _autoDim = true;
        _showAlways = true;
        _autoShowUponRoll = true;
        IgnoreSize = false;
        PerClassPosition = false;
    }
}