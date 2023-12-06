using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.UI.Controls.Classes.Elements;

public partial class TraverseCutControl
{
    readonly DoubleAnimation _toZeroAnimation;
    readonly DoubleAnimation _anim;

    bool _isAnimating;

    readonly DispatcherTimer _delay;
    uint _lastDuration;

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public static readonly DependencyProperty IconNameProperty =
        DependencyProperty.Register(nameof(IconName), typeof(string), typeof(TraverseCutControl), new PropertyMetadata(""));

    public StatTracker? Tracker
    {
        get => (StatTracker?)GetValue(TrackerProperty);
        set => SetValue(TrackerProperty, value);
    }

    public static readonly DependencyProperty TrackerProperty =
        DependencyProperty.Register(nameof(Tracker), typeof(StatTracker), typeof(TraverseCutControl), new PropertyMetadata(null, HandleTrackerChanged));

    static void HandleTrackerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TraverseCutControl tc) return;
        tc.OnTrackerChanged((StatTracker?)e.NewValue, (StatTracker?)e.OldValue);
    }

    void OnTrackerChanged(StatTracker? newValue, StatTracker? oldValue)
    {
        if (oldValue != null)
        {
            oldValue.ToZero -= OnToZero;
            oldValue.FactorChanged -= OnFactorChanged;
        }

        if (newValue != null)
        {
            newValue.ToZero += OnToZero;
            newValue.FactorChanged += OnFactorChanged;
        }
    }

    public TraverseCutControl()
    {
        _anim = AnimationFactory.CreateDoubleAnimation(100, 0, completed: (_, _) => _isAnimating = false, framerate: 20);
        _toZeroAnimation = AnimationFactory.CreateDoubleAnimation(0, 0, framerate: 20);

        _delay = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(_anim.Duration.TimeSpan.Milliseconds + 10)
        };
        _delay.Tick += (_, _) =>
        {
            _delay.Stop();
            OnToZero(_lastDuration);
        };

        InitializeComponent();
    }


    void OnToZero(uint duration)
    {
        if (_isAnimating)
        {
            _lastDuration = duration;
            _delay.Start();
            return;
        }

        Dispatcher?.Invoke(() =>
        {
            _toZeroAnimation.Duration = TimeSpan.FromMilliseconds(duration);
            if (Tracker != null)
            {
                _toZeroAnimation.From = Tracker.Factor * 359.9;
            }
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, _toZeroAnimation);
        });
    }

    void OnFactorChanged(double newFactor)
    {
        if (Tracker != null)
        {
            _anim.To = newFactor * 359.9;
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, _anim);
        }
        _isAnimating = true;
    }
}