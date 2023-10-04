using Nostrum;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.UI.Controls.Skills;

public partial class RoundSkillEffectControl : INotifyPropertyChanged
{
    readonly DoubleAnimation _anim;
    SkillWithEffect? _context;

    public string DurationLabel => _context == null ? "" : TimeUtils.FormatSeconds(Convert.ToInt64(_context.Effect.Seconds / _context.Effect.Interval));
    public bool ShowEffectSeconds => _context?.Effect != null && _context.Effect.Seconds > 0;

    public RoundSkillEffectControl()
    {
        _anim = AnimationFactory.CreateDoubleAnimation(0, from: 359.99, to: 0);
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DesignerProperties.GetIsInDesignMode(this) || DataContext == null) return;
        _context = (SkillWithEffect)DataContext;
        FixedSkillControl.DataContext = _context.Cooldown;
        _context.Effect.Started += OnBuffStarted;
        _context.Effect.SecondsUpdated += OnSecondsUpdated;
        _context.Effect.Ended += OnBuffEnded;
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_context != null)
        {
            _context.Effect.Started -= OnBuffStarted;
            _context.Effect.SecondsUpdated -= OnSecondsUpdated;
            _context.Effect.Ended -= OnBuffEnded;
        }

        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }

    void OnBuffEnded(Data.CooldownMode obj)
    {
        ExternalArc.BeginAnimation(Arc.EndAngleProperty, null);
        ExternalArc.EndAngle = 0;
    }

    void OnSecondsUpdated()
    {
        NPC(nameof(DurationLabel));
        NPC(nameof(ShowEffectSeconds));
    }

    void OnBuffStarted(ulong duration, Data.CooldownMode mode)
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