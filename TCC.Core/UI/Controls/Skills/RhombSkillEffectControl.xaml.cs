using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.UI.Controls.Skills;

public partial class RhombSkillEffectControl : INotifyPropertyChanged
{
    SkillWithEffect? _context;
    readonly DoubleAnimation _anim;

    public RhombSkillEffectControl()
    {
        _anim = AnimationFactory.CreateDoubleAnimation(0, from: 328, to: 32, framerate: 20);

        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
        if (_context?.Effect == null) return;
        _context.Effect.Started -= OnBuffStarted;
        _context.Effect.SecondsUpdated -= OnSecondsUpdated;
        _context.Effect.Ended -= OnBuffEnded;

    }

    public string DurationLabel => _context == null ? "" : TimeUtils.FormatSeconds(Convert.ToInt64(_context.Effect.Seconds));
    public bool ShowEffectSeconds => _context?.Effect is { Seconds: > 0 };

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(this) || DataContext is not SkillWithEffect swe) return;
        _context = swe;
        RhombFixedSkillControl.DataContext = _context.Cooldown;
        _context.Effect.Started += OnBuffStarted;
        _context.Effect.SecondsUpdated += OnSecondsUpdated;
        _context.Effect.Ended += OnBuffEnded;
    }

    void OnBuffEnded(CooldownMode obj)
    {
        ExternalArc.BeginAnimation(Arc.EndAngleProperty, null);
        ExternalArc.EndAngle = 32;
    }

    void OnSecondsUpdated()
    {
        NPC(nameof(DurationLabel));
        NPC(nameof(ShowEffectSeconds));
    }

    void OnBuffStarted(ulong duration, CooldownMode mode)
    {
        _anim.Duration = TimeSpan.FromMilliseconds(duration);
        ExternalArc.BeginAnimation(Arc.EndAngleProperty, _anim);

    }



    public event PropertyChangedEventHandler? PropertyChanged;

    protected  void NPC([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}