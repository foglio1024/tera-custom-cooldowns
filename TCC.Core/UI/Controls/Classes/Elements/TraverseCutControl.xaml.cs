using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nostrum.WPF.Factories;
using TCC.Data;

namespace TCC.UI.Controls.Classes.Elements;

public partial class TraverseCutControl
{
    readonly DoubleAnimation _toZeroAnimation;
    readonly DoubleAnimation _anim;
    StatTracker? _dc;
    bool _isAnimating;
    DispatcherTimer _delay;
    uint _lastDuration;
    public string IconName { get; } = "icon_skills.dualrapidpiercing_tex";

    public TraverseCutControl()
    {
        _anim = AnimationFactory.CreateDoubleAnimation(100, 0, completed: (_, _) => _isAnimating = false, framerate: 20);
        _toZeroAnimation = AnimationFactory.CreateDoubleAnimation(0, 0,  framerate: 20);

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
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_dc == null) return;
        _dc.ToZero -= OnToZero;
        _dc.PropertyChanged -= OnPropertyChanged;
    }

    void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _dc = (StatTracker)DataContext;

        if (_dc == null) return;
        _dc.ToZero += OnToZero;
        _dc.PropertyChanged += OnPropertyChanged;
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
            if (_dc != null)
            {
                _toZeroAnimation.From = _dc.Factor * 359.9;
            }
            ExternalArc.BeginAnimation(Nostrum.WPF.Controls.Arc.EndAngleProperty, _toZeroAnimation);
        });
    }

    void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(StatTracker.Factor)) return;
        if (_dc != null)
        {
            _anim.To = _dc.Factor * 359.9;
            ExternalArc.BeginAnimation(Nostrum.WPF.Controls.Arc.EndAngleProperty, _anim);
        }
        _isAnimating = true;
    }
}