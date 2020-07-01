using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum;
using Nostrum.Controls;
using Nostrum.Factories;
using TCC.ViewModels;

namespace TCC.UI.Controls.Skills
{
    public partial class RhombSkillEffectControl : INotifyPropertyChanged
    {
        private SkillWithEffect? _context;
        private readonly DoubleAnimation _anim;

        public RhombSkillEffectControl()
        {
            _anim = AnimationFactory.CreateDoubleAnimation(0, from: 328, to: 32, framerate: 20);

            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            if (_context?.Effect == null) return;
            _context.Effect.Started -= OnBuffStarted;
            _context.Effect.SecondsUpdated -= OnSecondsUpdated;
            _context.Effect.Ended -= OnBuffEnded;

        }

        public string DurationLabel => _context == null ? "" : TimeUtils.FormatTime(_context.Effect.Seconds);
        public bool ShowEffectSeconds => _context?.Effect != null && _context.Effect.Seconds > 0;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) || !(DataContext is SkillWithEffect swe)) return;
            _context = swe;
            RhombFixedSkillControl.DataContext = _context.Cooldown;
            _context.Effect.Started += OnBuffStarted;
            _context.Effect.SecondsUpdated += OnSecondsUpdated;
            _context.Effect.Ended += OnBuffEnded;
        }

        private void OnBuffEnded(Data.CooldownMode obj)
        {
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, null);
            ExternalArc.EndAngle = 32;
        }

        private void OnSecondsUpdated()
        {
            NPC(nameof(DurationLabel));
            NPC(nameof(ShowEffectSeconds));
        }

        private void OnBuffStarted(ulong duration, Data.CooldownMode mode)
        {
            _anim.Duration = TimeSpan.FromMilliseconds(duration);
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, _anim);

        }



        public event PropertyChangedEventHandler PropertyChanged = null!;

        protected  void NPC([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
