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
            if (!RaiseAndSetIfChanged(value, ref _autorollDelaySec)) return;
            DelayChanged?.Invoke(value);
        }
    }

    public HotKey RollHotKey
    {
        get => _rollHotKey;
        set
        {
            if (_rollHotKey.Equals(value)) return;
            _rollHotKey = value;
            N();
            /// verify that it doesn't break anything
            //if (!RaiseAndSetIfChanged(value, ref _rollHotKey)) return;

            if (_passHotKey.Equals(value)) PassHotKey = new HotKey(Keys.None, ModifierKeys.None);
        }
    }

    public HotKey PassHotKey
    {
        get => _passHotKey;
        set
        {
            
            if (_passHotKey.Equals(value)) return;
            _passHotKey = value;
            N();
            /// verify that it doesn't break anything
            //if (!RaiseAndSetIfChanged(value, ref _passHotKey)) return;

            if (_rollHotKey.Equals(value)) RollHotKey = new HotKey(Keys.None, ModifierKeys.None);
        }
    }

    public HotKey ToggleHotkey
    {
        get => _toggleHotKey;
        set
        {
            if (_toggleHotKey.Equals(value)) return;
            _toggleHotKey = value;
            N();
            /// verify that it doesn't break anything
            //if (!RaiseAndSetIfChanged(value, ref _toggleHotKey)) return;

            KeyboardHook.Instance.ChangeHotkey(_toggleHotKey, value);
        }
    }

    public bool AlwaysRoll
    {
        get => _alwaysRoll;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _alwaysRoll)) return;

            if (_alwaysPass == _alwaysRoll && value)
                _alwaysPass = !_alwaysRoll;
            InvokePropertyChanged(nameof(AlwaysPass));
            AutoRollPolicyChanged?.Invoke();
        }
    }

    public bool AlwaysPass
    {
        get => _alwaysPass;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _alwaysPass)) return;

            if (_alwaysPass == _alwaysRoll && value)
                _alwaysRoll = !_alwaysPass;
            InvokePropertyChanged(nameof(AlwaysRoll));
            AutoRollPolicyChanged?.Invoke();
        }
    }

    public bool AutoShowUponRoll
    {
        get => _autoShowUponRoll;
        set => RaiseAndSetIfChanged(value, ref _autoShowUponRoll);
    }

    bool _autoRollItemsBlacklistMode;

    public bool AutoRollItemsBlacklistMode
    {
        get => _autoRollItemsBlacklistMode;
        set => RaiseAndSetIfChanged(value, ref _autoRollItemsBlacklistMode);
    }

    bool _autoPassItemsBlacklistMode;

    public bool AutoPassItemsBlacklistMode
    {
        get => _autoPassItemsBlacklistMode;
        set => RaiseAndSetIfChanged(value, ref _autoPassItemsBlacklistMode);
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