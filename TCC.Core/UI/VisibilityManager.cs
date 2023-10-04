using System;
using System.Windows.Threading;

namespace TCC.UI;

public class VisibilityManager
{
    readonly DispatcherTimer _dimTimer;
    bool _forceVisible;
    bool _forceUndim;

    public event Action? VisibilityChanged;
    public event Action? DimChanged;
    public event Action? ClickThruChanged;

    public bool Dim => !_dimTimer.IsEnabled &&
                       !Game.Encounter &&
                       !ForceUndim;
    public bool Visible => Game.Logged &&
                           !Game.LoadingScreen &&
                           FocusManager.IsForeground ||
                           ForceVisible;
    public bool ForceUndim
    {
        get => _forceUndim;
        set
        {
            if (_forceUndim == value) return;
            _forceUndim = value;
            NotifyDimChanged();
        }
    }
    public bool ForceVisible
    {
        get => _forceVisible;
        set
        {
            if (_forceVisible == value) return;
            _forceVisible = value;
            NotifyVisibilityChanged();
        }
    }

    public VisibilityManager()
    {
        Game.LoadingScreenChanged += NotifyVisibilityChanged;
        Game.LoggedChanged += NotifyVisibilityChanged;
        Game.EncounterChanged += NotifyDimChanged;
        Game.GameUiModeChanged += OnGameUiModeChanged;
        Game.SkillStarted += OnSkillStarted;

        FocusManager.ForegroundChanged += NotifyVisibilityChanged;

        _dimTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _dimTimer.Tick += (_, _) =>
        {
            _dimTimer.Stop();
            NotifyDimChanged();
        };
    }

    void OnGameUiModeChanged()
    {
        App.BaseDispatcher.Invoke(() => ClickThruChanged?.Invoke());
    }

    void OnSkillStarted()
    {
        _dimTimer.Stop();
        _dimTimer.Start();
        NotifyDimChanged();
    }

    void NotifyVisibilityChanged()
    {
        App.BaseDispatcher.InvokeAsync(() =>
            VisibilityChanged?.Invoke(), DispatcherPriority.Background);
    }

    void NotifyDimChanged()
    {
        App.BaseDispatcher.InvokeAsync(() =>
            DimChanged?.Invoke(), DispatcherPriority.Background);
    }
    public void RefreshDim()
    {
        if (App.Loading) return;
        App.BaseDispatcher.Invoke(() =>
        {
            ForceUndim = true;
            DimChanged?.Invoke();
            ForceUndim = false;
            DimChanged?.Invoke();
        });
    }
    public void RefreshVisible()
    {
        App.BaseDispatcher.InvokeAsync(() =>
        {
            _forceVisible = true;
            VisibilityChanged?.Invoke();
            _forceVisible = false;
            VisibilityChanged?.Invoke();
        });
    }
}