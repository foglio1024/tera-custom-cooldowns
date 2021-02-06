using Nostrum;
using Nostrum.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.Data.Skills;
using TCC.Utils;

namespace TCC.UI.Controls.Skills
{
    public class SkillControlBase : UserControl, INotifyPropertyChanged
    {
        #region INPC
        public event PropertyChangedEventHandler? PropertyChanged;
        protected  void NPC([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        protected Cooldown? Context;
        protected Arc? MainArcRef;
        protected Arc? PreArcRef;
        protected FrameworkElement? ResetArcRef;
        private readonly DoubleAnimation _arcAnimation;
        private bool _isRunning;

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
        public string SecondsText => Context == null ? "0" : TimeUtils.FormatMilliseconds(Convert.ToInt64((Context.Seconds > uint.MaxValue ? 0 : Context.Seconds) * 1000), App.Settings.ShowDecimalsInCooldowns);

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
            if (DesignerProperties.GetIsInDesignMode(this) || !(DataContext is Cooldown)) return;
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
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            if (Context == null)
            {
                Log.CW("[SkillControlBase.OnUnloaded] Context is null!");
                return;
            }
            Context.Ended -= OnCooldownEnded;
            Context.Started -= OnCooldownStarted;
            Context.SecondsUpdated -= OnSecondsUpdated;

        }
        private void OnSecondsUpdated()
        {
            NPC(nameof(SecondsText));
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

        private void StartArcAnimation(ulong duration, Arc? arc, double val = 1)
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
}