using FoglioUtils;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls.Skills
{
    public class FixedSkillControlBase : SkillControlBase
    {
        protected FrameworkElement GlowRef;
        private readonly DoubleAnimation _resetAnimation;
        private readonly DoubleAnimation _glowAnimation;
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
        public FixedSkillControlBase()
        {
            _resetAnimation = AnimationFactory.CreateDoubleAnimation(500, to: 0, from: 30, true, framerate: 20); // new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new QuadraticEase() };
            _glowAnimation = AnimationFactory.CreateDoubleAnimation(200, to: 0, from: 1, framerate: 20); // new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
        }

        protected override void OnCooldownStarted(CooldownMode mode)
        {
            base.OnCooldownStarted(mode);
            Warning = false;
        }

        protected override void OnCooldownEnded(CooldownMode mode)
        {
            base.OnCooldownEnded(mode);
            AnimateAvailableSkill();
            GlowRef?.BeginAnimation(OpacityProperty, _glowAnimation);
        }
        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            if (Context == null) return;

            Context.FlashingForced += OnForceFlashing;
            Context.FlashingStopForced += OnForceStopFlashing;
            Context.Reset += OnReset;

            if (!Context.IsAvailable) OnCooldownStarted(Context.Mode);

        }
        protected override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);
            if (Context == null) return;
            Context.FlashingForced -= OnForceFlashing;
            Context.FlashingStopForced -= OnForceStopFlashing;
            Context.Reset -= OnReset;
        }
        protected void OnForceFlashing()
        {
            if (!Context.IsAvailable) Warning = false;
            else AnimateAvailableSkill();
        }
        protected void OnForceStopFlashing()
        {
            Warning = false;
        }
        private void OnReset()
        {
            ResetArcRef.BeginAnimation(Shape.StrokeThicknessProperty, _resetAnimation);
        }

        private void AnimateAvailableSkill()
        {
            StopArcAnimation(MainArcRef); //stop any arc animations
            StopArcAnimation(PreArcRef); //stop any arc animations
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            GlowRef.BeginAnimation(OpacityProperty, an);
            if (Context.FlashOnAvailable && (Game.Combat || Game.Encounter)) Warning = true;

        }
    }
}