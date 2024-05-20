using System;
using System.Collections.Generic;
using System.Windows;
using TCC.Data;
using TCC.UI.Windows.Widgets;
using TeraDataLite;

namespace TCC.Settings.WindowSettings;

public class BuffWindowSettings : WindowSettingsBase
{
    public event Action? DirectionChanged;
    public event Action? OverlapChanged;

    private FlowDirection _direction;
    private double _overlap;
    public bool ShowAll { get; set; } // by HQ

    public FlowDirection Direction
    {
        get => _direction;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _direction)) return;
            DirectionChanged?.Invoke();
        }
    }
    public double Overlap
    {
        get => _overlap;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _overlap)) return;
            OverlapChanged?.Invoke();
        }
    }

    [Obsolete]
    public Dictionary<Class, List<uint>> MyAbnormals { get; } // by HQ
    [Obsolete]
    public List<uint> Specials { get; }
    [Obsolete]
    public List<uint> Hidden { get; private set; }


    public BuffWindowSettings()
    {
        _visible = true;
        _clickThruMode = ClickThruMode.Never;
        _scale = 1;
        _autoDim = true;
        _dimOpacity = .5;
        _showAlways = false;
        _enabled = true;
        _allowOffScreen = false;
        Positions = new ClassPositions(1, .7, ButtonsPosition.Above);

        UndimOnFlyingGuardian = false;

        Direction = FlowDirection.RightToLeft;
        ShowAll = true;
        Specials = new List<uint>();
        Hidden = new List<uint>();
        MyAbnormals = new Dictionary<Class, List<uint>>();


        GpkNames.Add("Abnormality");

    }

}