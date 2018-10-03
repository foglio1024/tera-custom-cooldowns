using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    ///     Logica di interazione per FixedSkillControl.xaml
    /// </summary>
    public partial class RhombFixedSkillControl : INotifyPropertyChanged
    {
        private FixedSkillCooldown _context;
        private readonly DoubleAnimation _arcAnimation;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                if (!_isRunning) Warning = false;
                NPC();
            }
        }

        private bool _warning;
        public bool Warning
        {
            get => _warning;
            set
            {
                if (_warning == value) return;
                _warning = value;
                NPC();
            }
        }

        public string SecondsText => _context == null ? "" :
            Utils.TimeFormatter(Convert.ToUInt32(_context.Seconds > UInt32.MaxValue ?
                0 :
                _context.Seconds));

        public RhombFixedSkillControl()
        {
            InitializeComponent();
            _arcAnimation = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(1));
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) || DataContext == null) return;
            _context = (FixedSkillCooldown)DataContext;

            _context.Ended += OnCooldownEnded;
            _context.Started += OnCooldownStarted;
            _context.FlashingForced += OnFlashingStarted;
            _context.SecondsUpdated += OnSecondsUpdated;
            _context.Reset += OnReset;
            _context.FlashingStopForced += OnFlashingForceStopped;

        }


        private void OnFlashingForceStopped()
        {
            StopWarning();
        }

        private void OnReset()
        {
            ResetArc.Opacity = 1;
            ResetArc.BeginAnimation(Shape.StrokeThicknessProperty, new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() });
        }

        private void OnSecondsUpdated()
        {
            NPC(nameof(SecondsText));
        }

        private void OnFlashingStarted()
        {
            if (_context.IsAvailable) AnimateAvailableSkill();
            else StopWarning();
        }

        private void OnCooldownStarted(CooldownMode mode)
        {
            IsRunning = true;
            StopWarning();
            switch (mode)
            {
                case CooldownMode.Normal:
                    var newVal = _context.Cooldown / (double)_context.OriginalCooldown;
                    newVal = newVal > 1 ? 1 : newVal;
                    AnimateArcAngle(newVal);
                    break;
                case CooldownMode.Pre:
                    AnimatePreArcAngle();
                    break;
            }
        }

        private void OnCooldownEnded(CooldownMode mode)
        {
            IsRunning = false;
            switch (mode)
            {
                case CooldownMode.Normal:
                    AnimateAvailableSkill();
                    break;
                case CooldownMode.Pre:
                    StopPreAnimation();
                    break;
            }
        }

        private void NPC([CallerMemberName] string p = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        private void AnimateArcAngle(double val = 1)
        {
            _arcAnimation.Duration = TimeSpan.FromMilliseconds(_context.Cooldown);
            _arcAnimation.From = 359.9 * val;
            var fps = _context.Cooldown > 80000 ? 1 : 30;
            Timeline.SetDesiredFrameRate(_arcAnimation, fps);
            Arc.BeginAnimation(Arc.EndAngleProperty, _arcAnimation);
        }
        private void AnimatePreArcAngle(double val = 1)
        {
            _arcAnimation.Duration = TimeSpan.FromMilliseconds(_context.Cooldown);
            _arcAnimation.From = 359.9 * val;
            var fps = _context.Cooldown > 80000 ? 1 : 30;
            Timeline.SetDesiredFrameRate(_arcAnimation, fps);
            PreArc.BeginAnimation(Arc.EndAngleProperty, _arcAnimation);
        }
        private void AnimateAvailableSkill()
        {
            Arc.BeginAnimation(Arc.EndAngleProperty, null); //stop any arc animations
            if (!_context.FlashOnAvailable)
            {
                var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                Glow.BeginAnimation(OpacityProperty, an);
            }
            if (_context.FlashOnAvailable && (SessionManager.Combat || SessionManager.Encounter)) StartWarning();

        }

        private void StopPreAnimation()
        {
            PreArc.BeginAnimation(Arc.EndAngleProperty, null); //stop any arc animations
            PreArc.EndAngle = 0.01;
        }
        private void StartWarning()
        {
            Warning = true;
        }
        private void StopWarning()
        {
            Warning = false;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CooldownWindowViewModel.Instance.DeleteFixedSkill(_context);
        }
    }
}