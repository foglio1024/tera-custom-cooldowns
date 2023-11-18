using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Nostrum;
using Nostrum.WPF.Controls;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.UI.Controls.Skills;

public class SkillControlBase : UserControl, INotifyPropertyChanged
{
    #region INPC
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void NPC([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
    protected Cooldown? Context;
    protected Arc? MainArcRef;
    protected Arc? PreArcRef; // todo: remove (#315)
    protected FrameworkElement? ResetArcRef;
    readonly DoubleAnimation _arcAnimation;
    bool _isRunning;

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;
            _isRunning = value;
            NPC();
        }
    }

    string _secondsText = "0";
    public string SecondsText
    {
        get => _secondsText;
        set
        {
            if (_secondsText == value) return;
            _secondsText = value;
            NPC();
        }
    }


    protected SkillControlBase()
    {
        _arcAnimation = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(1));
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnToolTipOpening(ToolTipEventArgs e)
    {
        base.OnToolTipOpening(e);
        FocusManager.PauseTopmost = true;
    }

    protected override void OnToolTipClosing(ToolTipEventArgs e)
    {
        base.OnToolTipClosing(e);
        FocusManager.PauseTopmost = false;
    }

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(this) || DataContext is not Cooldown) return;
        Context = (Cooldown)DataContext;
        OnSecondsUpdated();
        if (!Context.IsAvailable)
        {
            OnCooldownStarted(Context.Duration, Context.Mode);
        }
        Context.Ended += OnCooldownEnded;
        Context.Started += OnCooldownStarted;
        Context.SecondsUpdated += OnSecondsUpdated;
    }
    protected virtual void OnUnloaded(object sender, RoutedEventArgs e) //TODO: maybe use DataContextChanged
    {
        Unloaded -= OnUnloaded;
        if (Context == null)
        {
            //Log.CW("[SkillControlBase.OnUnloaded] Context is null!");
            return;
        }
        Context.Ended -= OnCooldownEnded;
        Context.Started -= OnCooldownStarted;
        Context.SecondsUpdated -= OnSecondsUpdated;

    }

    void OnSecondsUpdated()
    {
        if (Context == null)
        {
            SecondsText = "";
            return;
        }

        var showDecimals = App.Settings.CooldownsDecimalMode switch
        {
            CooldownDecimalMode.Never => false,
            CooldownDecimalMode.LessThanOne when Context.Seconds < 1 => true,
            CooldownDecimalMode.LessThanTen when Context.Seconds < 10 => true,
            _ => false
        };

        SecondsText = TimeUtils.FormatMilliseconds(
            Convert.ToInt64((Context?.Seconds > uint.MaxValue ? 0 : Context?.Seconds) * 1000),
            showDecimals
            );
    }
    protected virtual void OnCooldownStarted(ulong duration, CooldownMode mode)
    {
        IsRunning = true;
        switch (mode)
        {
            case CooldownMode.Normal:
                StopArcAnimation(PreArcRef);
                if (Context != null)
                {
                    var newVal = duration / (double)Context.OriginalDuration;
                    newVal = newVal > 1 ? 1 : newVal;
                    //if (Context.Duration == 0) newVal = 0; //TODO: check this
                    StartArcAnimation(duration, MainArcRef, newVal);
                }
                break;
            case CooldownMode.Pre:
                StartArcAnimation(duration, PreArcRef);
                break;
        }
    }
    protected virtual void OnCooldownEnded(CooldownMode mode)
    {
        IsRunning = false;
        switch (mode)
        {
            case CooldownMode.Normal:
                StopArcAnimation(MainArcRef);
                break;
            case CooldownMode.Pre:
                StopArcAnimation(PreArcRef);
                break;
        }
    }

    void StartArcAnimation(ulong duration, Arc? arc, double val = 1)
    {
        Dispatcher?.Invoke(() =>
        {
            if (arc == null) return;
            _arcAnimation.Duration = TimeSpan.FromMilliseconds(duration);
            _arcAnimation.From = 359.9 * (double.IsNaN(val) ? 0 : val);
            var fps = duration > 30000 ? 1 : 20;
            Timeline.SetDesiredFrameRate(_arcAnimation, fps);
            arc.BeginAnimation(Arc.EndAngleProperty, _arcAnimation);
        });
    }
    protected void StopArcAnimation(Arc? arc)
    {
        Dispatcher?.Invoke(() =>
        {
            if (arc == null) return;
            arc.BeginAnimation(Arc.EndAngleProperty, null); //stop any arc animations
            arc.EndAngle = 0.01;
        });
    }
}