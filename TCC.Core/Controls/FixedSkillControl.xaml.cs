using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    ///     Logica di interazione per FixedSkillControl.xaml
    /// </summary>
    public partial class FixedSkillControl : INotifyPropertyChanged
    {
        private FixedSkillCooldown _context;
        private readonly DispatcherTimer _warnTimer;
        private readonly DoubleAnimation _expandWarn;
        private readonly DoubleAnimation _expandWarnInner;
        private readonly DoubleAnimation _arcAnimation;
        private readonly DoubleAnimation _resetAnimation;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                if (!_isRunning) _warnTimer.Stop();
                NPC();
            }
        }

        public string SecondsText => _context == null ? "" : Utils.TimeFormatter(Convert.ToUInt32(_context.Seconds > UInt32.MaxValue ? 0 : _context.Seconds));

        public FixedSkillControl()
        {
            InitializeComponent();
            _warnTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _warnTimer.Tick += WarnTimer_Tick;
            _arcAnimation = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(1));
            _resetAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            _expandWarn =
                new DoubleAnimation(0, 1.5, TimeSpan.FromMilliseconds(300)) { EasingFunction = new QuadraticEase() };
            _expandWarnInner =
                new DoubleAnimation(35, 0, TimeSpan.FromMilliseconds(400)) { EasingFunction = new QuadraticEase() };
            Timeline.SetDesiredFrameRate(_expandWarn, 30);
            Timeline.SetDesiredFrameRate(_expandWarnInner, 30);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) || DataContext == null) return;
            _context = (FixedSkillCooldown)DataContext;
            //_context.PropertyChanged += _context_PropertyChanged;

            _context.Ended += OnCooldownEnded;
            _context.Started += OnCooldownStarted;
            _context.FlashingForced += OnFlashingStarted;
            _context.SecondsUpdated += OnSecondsUpdated;
            _context.Reset += OnReset;
        }

        private void OnReset()
        {
            //ResetArc.Opacity = 1;
            ResetArc.BeginAnimation(Arc.StrokeThicknessProperty, new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(500)){EasingFunction = new QuadraticEase()});
            //ResetArc.BeginAnimation(OpacityProperty, _resetAnimation);
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
                    var newVal = _context.Cooldown / (double) _context.OriginalCooldown;
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

        //private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    //Dispatcher.InvokeIfRequired(() =>
        //    //{
        //    //    switch (e.PropertyName)
        //    //    {
        //    //        //case "Refresh" when _context.Cooldown == _context.OriginalCooldown: return;
        //    //        //case "Refresh":
        //    //        //    var newVal = _context.Cooldown / (double)_context.OriginalCooldown;
        //    //        //    if (newVal > 1) newVal = 1;
        //    //        //    if (_context.Cooldown == 0)
        //    //        //    {
        //    //        //        IsRunning = false;
        //    //        //        AnimateAvailableSkill();
        //    //        //        return;
        //    //        //    }

        //    //        //    AnimateArcAngle(newVal);
        //    //        //    break;
        //    //        //case "Start":
        //    //        //    IsRunning = true;
        //    //        //    AnimateArcAngle();
        //    //        //    break;
        //    //        //case "IsAvailable" when _context.IsAvailable:
        //    //        //    IsRunning = false;
        //    //        //    AnimateAvailableSkill();
        //    //        //    break;
        //    //        //case "IsAvailable":
        //    //        //    _warnTimer.Stop();
        //    //        //    break;
        //    //        //case nameof(_context.Seconds):
        //    //        //    NPC(nameof(SecondsText));
        //    //        //    break;
        //    //        //case "StartPre":
        //    //        //    IsRunning = true;
        //    //        //    AnimatePreArcAngle();
        //    //        //    break;
        //    //        //case "StopPre":
        //    //        //    IsRunning = false;
        //    //        //    PreArc.BeginAnimation(Arc.EndAngleProperty, null); //stop any arc animations
        //    //        //    PreArc.EndAngle = 0.01;
        //    //        //    break;
        //    //    }
        //    //}, DispatcherPriority.DataBind);
        //}

        private void NPC([CallerMemberName] string p = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        private void WarnTimer_Tick(object sender, EventArgs e)
        {
            WarnAvailableSkill();
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

            WarnAvailableSkill();
            StartWarning();
        }

        private void StopPreAnimation()
        {
            PreArc.BeginAnimation(Arc.EndAngleProperty, null); //stop any arc animations
            PreArc.EndAngle = 0.01;
        }
        private void StartWarning()
        {
            _warnTimer.Start();
        }
        private void StopWarning()
        {
            _warnTimer.Stop();
        }

        private void WarnAvailableSkill()
        {
            if (!SessionManager.Encounter) return;
            if (!_context.FlashOnAvailable) return;
            WarnArc.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _expandWarn);
            WarnArc.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _expandWarn);
            WarnArc.BeginAnimation(Shape.StrokeThicknessProperty, _expandWarnInner);
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