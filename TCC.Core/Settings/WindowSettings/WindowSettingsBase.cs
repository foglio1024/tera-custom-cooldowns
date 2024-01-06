using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Newtonsoft.Json;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.UI;
using TCC.UI.Windows.Widgets;
using TeraDataLite;

namespace TCC.Settings.WindowSettings;

public class WindowSettingsBase : ThreadSafeObservableObject
{
    protected double _w;
    protected double _h;
    protected bool _visible;
    protected ClickThruMode _clickThruMode;
    protected double _scale;
    protected bool _autoDim;
    protected double _dimOpacity;
    protected double _maxOpacity = 1;
    protected bool _showAlways;
    protected bool _enabled;
    protected bool _allowOffScreen;
    bool _forcedClickable;
    bool _forcedVisible;
    bool _hideIngameUI = true;


    public event Action? ResetToCenter;
    public event Action? ClickThruModeChanged;
    public event Action<bool>? EnabledChanged;
    public event Action<bool>? VisibilityChanged;

    [JsonIgnore] public string Name { get; } = "";
    [JsonIgnore] protected List<string> GpkNames { get; }

    [JsonIgnore]
    public bool ForcedClickable
    {
        get => _forcedClickable;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _forcedClickable)) return;
            InvokePropertyChanged(nameof(ClickThruMode));
        }
    }
    [JsonIgnore]
    public bool ForcedVisible
    {
        get => _forcedVisible;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _forcedVisible)) return;
            WindowManager.VisibilityManager.RefreshDim();
        }
    }

    [JsonIgnore]
    public double X
    {
        get => Positions.Position(!PerClassPosition ? Class.Common : CurrentClass()).X;
        set
        {
            if (value >= int.MaxValue) return;
            var cc = CurrentClass();
            if (cc == Class.None || !PerClassPosition) cc = Class.Common;
            var old = Positions.Position(cc);
            if (old.X == value) return;
            Positions.SetPosition(cc, old with { X = value });
            InvokePropertyChanged();
        }
    }
    [JsonIgnore]
    public double Y
    {
        get => Positions.Position(!PerClassPosition ? Class.Common : CurrentClass()).Y;
        set
        {
            if (value >= int.MaxValue) return;
            var cc = CurrentClass();
            if (cc == Class.None || !PerClassPosition) cc = Class.Common;
            var old = Positions.Position(cc);
            if (old.Y == value) return;
            Positions.SetPosition(cc, old with { Y = value });
            InvokePropertyChanged();
        }
    }
    [JsonIgnore] public bool IgnoreSize { get; set; } = true;
    [JsonIgnore] public bool UndimOnFlyingGuardian { get; set; } = true;
    [JsonIgnore] public bool PerClassPosition { get; set; } = true;
    [JsonIgnore] public ICommand ResetPositionCommand { get; }
    [JsonIgnore] public ICommand HideCommand { get; }
    [JsonIgnore] public ICommand PinCommand { get; }
    [JsonIgnore] public ICommand AutoDimCommand { get; }
    [JsonIgnore] public ICommand MakeGlobalCommand { get; }
    [JsonIgnore] public ICommand CloseCommand { get; }

    public virtual bool Enabled
    {
        get => _enabled;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _enabled)) return;

            if (_enabled)
            {
                Visible = true;
            }

            if (App.Loading) return;
            EnabledChanged?.Invoke(_enabled);
        }
    }

    public virtual bool HideIngameUI
    {
        get => _hideIngameUI;
        set => RaiseAndSetIfChanged(value, ref _hideIngameUI);
    }

    public bool Visible
    {
        get => _visible;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _visible)) return;
            VisibilityChanged?.Invoke(_visible);
        }
    }
    public bool ShowAlways
    {
        get => _showAlways;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _showAlways)) return;
            VisibilityChanged?.Invoke(Visible);
            WindowManager.VisibilityManager.RefreshVisible();
        }
    }
    public bool AllowOffScreen
    {
        get => _allowOffScreen;
        set => RaiseAndSetIfChanged(value, ref _allowOffScreen);
    }
    public bool AutoDim
    {
        get => _autoDim;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _autoDim)) return;
            WindowManager.VisibilityManager.RefreshDim();
        }
    }
    public double DimOpacity
    {
        get => _dimOpacity;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _dimOpacity)) return;
            WindowManager.VisibilityManager.RefreshDim();
        }
    }
    public double MaxOpacity
    {
        get => _maxOpacity;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _maxOpacity)) return;
            WindowManager.VisibilityManager.RefreshDim();
        }
    }
    public double Scale
    {
        get => _scale;
        set => RaiseAndSetIfChanged(value, ref _scale);

    }
    public double W
    {
        get => _w;
        set => RaiseAndSetIfChanged(value, ref _w);
    }
    public double H
    {
        get => _h;
        set => RaiseAndSetIfChanged(value, ref _h);
    }
    public ClickThruMode ClickThruMode
    {
        get => ForcedClickable ? ClickThruMode.Never : _clickThruMode;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _clickThruMode)) return;
            ClickThruModeChanged?.Invoke();
        }
    }
    public ButtonsPosition ButtonsPosition
    {
        get
        {
            var cc = CurrentClass();
            return Positions.Buttons(cc);
        }
        set
        {
            var cc = CurrentClass();
            if (cc == Class.None) return;
            Positions.SetButtons(cc, value);
            InvokePropertyChanged();
        }
    }

    public ClassPositions Positions { get; set; }

    public WindowSettingsBase()
    {
        Positions = new ClassPositions();
        GpkNames = [];
        EnabledChanged += OnEnabledChanged;

        ResetPositionCommand = new RelayCommand(_ => ResetToCenter?.Invoke());
        HideCommand = new RelayCommand(_ => Visible = false);
        PinCommand = new RelayCommand(_ => ShowAlways = !ShowAlways);
        AutoDimCommand = new RelayCommand(_ => AutoDim = !AutoDim);
        MakeGlobalCommand = new RelayCommand(MakePositionsGlobal);
        CloseCommand = new RelayCommand(_ => Enabled = false);
        //Game.LoadingScreenChanged += () => OnEnabledChanged(!Game.LoadingScreen && Enabled);
    }
    public WindowSettingsBase(double x, double y, double h, double w, bool visible, ClickThruMode ctm, double scale, bool autoDim, double dimOpacity, bool showAlways, bool enabled, bool allowOffscreen, ClassPositions? positions = null, string name = "", bool perClassPosition = true, ButtonsPosition buttonsPosition = ButtonsPosition.Above) : this()
    {
        Name = name;
        _w = w;
        _h = h;
        _visible = visible;
        _clickThruMode = ctm;
        _scale = scale;
        _autoDim = autoDim;
        _dimOpacity = dimOpacity;
        _showAlways = showAlways;
        _enabled = enabled;
        _allowOffScreen = allowOffscreen;
        PerClassPosition = perClassPosition;
        Positions = positions == null ?
            new ClassPositions(x, y, buttonsPosition) :
            new ClassPositions(positions);
    }

    protected virtual void OnEnabledChanged(bool enabled)
    {
        // do nothing for now
        //if (GpkNames.Count == 0) return;
        //if (!ProxyInterface.Instance.IsStubAvailable) return;
        //foreach (var gpkName in GpkNames)
        //{
        //    ProxyInterface.Instance.Stub.InvokeCommand($"tcc-toggle-gpk {gpkName} {(enabled ? 0 : 1)}");
        //}
    }

    protected WindowSettingsBase(WindowSettingsBase other) : this()
    {
        _w = other.W;
        _h = other.H;
        _visible = other.Visible;
        _clickThruMode = other._clickThruMode;
        _scale = other.Scale;
        _autoDim = other.AutoDim;
        _dimOpacity = other.DimOpacity;
        _showAlways = other.ShowAlways;
        _enabled = other.Enabled;
        _allowOffScreen = other.AllowOffScreen;
        PerClassPosition = other.PerClassPosition;
        Positions = other.Positions;
        Name = other.Name;


    }

    public virtual XElement ToXElement(string name)
    {
        var xe = new XElement("WindowSetting");
        xe.Add(new XAttribute("Name", name));
        xe.Add(new XAttribute(nameof(W), W));
        xe.Add(new XAttribute(nameof(H), H));
        xe.Add(new XAttribute(nameof(Visible), Visible));
        xe.Add(new XAttribute(nameof(ClickThruMode), _clickThruMode));
        xe.Add(new XAttribute(nameof(Scale), Scale));
        xe.Add(new XAttribute(nameof(AutoDim), AutoDim));
        xe.Add(new XAttribute(nameof(DimOpacity), DimOpacity));
        xe.Add(new XAttribute(nameof(ShowAlways), ShowAlways));
        xe.Add(new XAttribute(nameof(Enabled), Enabled));
        xe.Add(new XAttribute(nameof(AllowOffScreen), AllowOffScreen));
        xe.Add(BuildWindowPositionsXElement());
        return xe;
    }
    protected XElement BuildWindowPositionsXElement()
    {
        var ret = new XElement(nameof(Positions));

        foreach (Class cl in Enum.GetValues(typeof(Class)))
        {
            ret.Add(
                new XElement("Position", new XAttribute("class", cl),
                    new XAttribute("X", Positions.Position(cl).X),
                    new XAttribute("Y", Positions.Position(cl).Y),
                    new XAttribute("ButtonsPosition", Positions.Buttons(cl)))
            );
        }

        return ret;
    }

    public void MakePositionsGlobal()
    {
        //Log.CW($"[{GetType().Name}] {nameof(MakePositionsGlobal)}()");

        var currentPos = new Point(X, Y);
        Positions.SetAllPositions(currentPos);
        App.Settings.Save();
    }

    //protected void InvokeClickThruModeChanged() => ClickThruModeChanged?.Invoke();

    public void ApplyScreenCorrection(Size sc)
    {
        //Log.CW($"[{GetType().Name}] {nameof(ApplyScreenCorrection)}({sc})");

        Positions.ApplyCorrection(sc);
    }
    protected Class CurrentClass()
    {
        var cc = Game.Me.Class == Class.None ? Class.Common : Game.Me.Class;
        cc = PerClassPosition ? cc : Class.Common;
        return cc;
    }

    public void ApplyScreenOffset(Point oldPos, Point newPos, Size size)
    {
        Positions.ApplyOffset(oldPos, newPos, size);
    }
}