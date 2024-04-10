using System;
using TCC.Data;
using TCC.UI.Windows.Widgets;

namespace TCC.Settings.WindowSettings;

public class FlightWindowSettings : WindowSettingsBase
{
    public event Action? RotationChanged;
    public event Action? FlipChanged;

    private bool _flip;
    private double _rotation;

    public bool Flip
    {
        get => _flip;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _flip)) return;
            FlipChanged?.Invoke();
        }
    }

    public double Rotation
    {
        get => _rotation;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _rotation)) return;
            RotationChanged?.Invoke();
        }
    }

    public FlightWindowSettings()
    {
        _visible = true;
        _clickThruMode = ClickThruMode.Never;
        _scale = 1;
        _autoDim = false;
        _dimOpacity = 1;
        _showAlways = false;
        _enabled = true;
        _allowOffScreen = false;
        Positions = new ClassPositions(.5, .5, ButtonsPosition.Above);

        PerClassPosition = false;

        Rotation = 0;
        Flip = false;

        GpkNames.Add("ProgressBar");
    }

    protected override void OnEnabledChanged(bool enabled)
    {
        //TODO: add specific code
    }
}