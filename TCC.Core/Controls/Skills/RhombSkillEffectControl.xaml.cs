using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Properties;
using TCC.ViewModels;

namespace TCC.Controls.Skills
{
    public partial class RhombSkillEffectControl : INotifyPropertyChanged
    {
        public RhombSkillEffectControl()
        {
            InitializeComponent();
        }

        private DurationCooldownIndicator _context;
        private DoubleAnimation _anim;
        public string DurationLabel => _context == null ? "" : Utils.TimeFormatter(_context.Buff.Seconds);
        public bool ShowEffectSeconds => _context?.Buff != null && _context.Buff.Seconds > 0;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(50000)));
            if (DesignerProperties.GetIsInDesignMode(this) || !( DataContext is DurationCooldownIndicator)) return;
            _context = (DurationCooldownIndicator)DataContext;
            RhombFixedSkillControl.DataContext = _context.Cooldown;
            _context.Buff.Started += OnBuffStarted;
            _context.Buff.SecondsUpdated += OnSecondsUpdated;
            _context.Buff.Ended += OnBuffEnded;
            _anim = new DoubleAnimation(328, 32, TimeSpan.FromMilliseconds(_context.Buff.Duration));
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

        private void OnBuffStarted(Data.CooldownMode obj)
        {
            _anim.Duration = TimeSpan.FromMilliseconds(_context.Buff.Duration);
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, _anim);

        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
