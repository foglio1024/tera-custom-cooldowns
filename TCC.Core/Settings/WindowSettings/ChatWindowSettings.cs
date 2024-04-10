using System;
using System.Collections.Generic;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Settings.WindowSettings;

public class ChatWindowSettings : WindowSettingsBase
{
    private bool _fadeOut = true;
    private double _backgroundOpacity = .3;
    private double _frameOpacity = 1;
    private bool _lfgOn = true;
    private int _hideTimeout;
    private bool _canCollapse = true;
    private bool _staysCollapsed;
    private bool _showImportant = true;
    private int _collapsedHeight = 64;

    public event Action? FadeoutChanged;
    public event Action? OpacityChanged;
    public event Action? TimeoutChanged;
    public event Action? CanCollapseChanged;
    public event Action? StaysCollapsedChanged;
    public event Action? CollapsedHeightChanged;


    public int HideTimeout
    {
        get => _hideTimeout;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _hideTimeout)) return;
            TimeoutChanged?.Invoke();
        }
    }
    public double BackgroundOpacity
    {
        get => _backgroundOpacity;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _backgroundOpacity)) return;
            OpacityChanged?.Invoke();
        }
    }
    public double FrameOpacity
    {
        get => _frameOpacity;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _frameOpacity)) return;
            OpacityChanged?.Invoke();
        }
    }
    public bool FadeOut
    {
        get => _fadeOut;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _fadeOut)) return;
            FadeoutChanged?.Invoke();
        }
    }
    public bool LfgOn
    {
        get => _lfgOn;
        set => RaiseAndSetIfChanged(value, ref _lfgOn);
    }
    public bool CanCollapse
    {
        get => _canCollapse;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _canCollapse)) return;
            if (!_canCollapse) StaysCollapsed = false;
            CanCollapseChanged?.Invoke();
        }
    }
    public bool StaysCollapsed
    {
        get => _staysCollapsed;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _staysCollapsed)) return;
            StaysCollapsedChanged?.Invoke();
        }
    }
    public bool ShowImportant
    {
        get => _showImportant;
        set => RaiseAndSetIfChanged(value, ref _showImportant);
    }


    public int CollapsedHeight
    {
        get => _collapsedHeight;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _collapsedHeight)) return;
            CollapsedHeightChanged?.Invoke();
        }
    }


    public List<TabInfo> Tabs { get; }

    public ChatWindowSettings()
    {
        Tabs = new List<TabInfo>();
        PerClassPosition = false;
        IgnoreSize = false;
    }
    public ChatWindowSettings(WindowSettingsBase other) : base(other)
    {
        Tabs = new List<TabInfo>();
        PerClassPosition = false;
        IgnoreSize = false;

    }
    public ChatWindowSettings(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen) : base(x, y, h, w, visible, ctm, scale, autoDim, dimOpacity, showAlways, enabled, allowOffscreen)
    {
        Tabs = new List<TabInfo>();
        PerClassPosition = false;
        IgnoreSize = false;
    }
}