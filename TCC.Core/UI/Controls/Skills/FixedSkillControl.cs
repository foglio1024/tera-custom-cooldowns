using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Nostrum.WPF.Factories;
using TCC.Data;

namespace TCC.UI.Controls.Skills;

public class FixedSkillControl : SkillControlBase
{
    protected FrameworkElement? GlowRef;
    protected FrameworkElement? DeleteButtonRef;
    readonly DoubleAnimation _resetAnimation;
    readonly DoubleAnimation _glowAnimation;
    bool _warning;
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
    public FixedSkillControl()
    {
        _resetAnimation = AnimationFactory.CreateDoubleAnimation(500, to: 0, @from: 30, true, framerate: 60); // new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new QuadraticEase() };
        _glowAnimation = AnimationFactory.CreateDoubleAnimation(200, to: 0, @from: 1, framerate: 30); // new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
    }

    protected override void OnCooldownStarted(ulong duration, CooldownMode mode)
    {
        base.OnCooldownStarted(duration, mode);
        Warning = false;
    }

    protected override void OnCooldownEnded(CooldownMode mode)
    {
        base.OnCooldownEnded(mode);
        AnimateAvailableSkill();
        //GlowRef?.BeginAnimation(OpacityProperty, _glowAnimation);
    }
    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        base.OnLoaded(sender, e);
        if (DeleteButtonRef is Button delBtn)
        {
            delBtn.Click += DeleteButtonClicked;
        }
        if (Context == null) return;

        Context.FlashingForced += OnForceFlashing;
        Context.FlashingStopForced += OnForceStopFlashing;
        Context.Reset += OnReset;

        if (!Context.IsAvailable) OnCooldownStarted(Context.Duration, Context.Mode);

    }
    protected override void OnUnloaded(object sender, RoutedEventArgs e)
    {
        base.OnUnloaded(sender, e);
        if (DeleteButtonRef is Button delBtn)
        {
            delBtn.Click -= DeleteButtonClicked;
        }

        if (Context == null) return;
        Context.FlashingForced -= OnForceFlashing;
        Context.FlashingStopForced -= OnForceStopFlashing;
        Context.Reset -= OnReset;
    }
    protected void OnForceFlashing()
    {
        if (Context == null) return;
        if (!Context.IsAvailable) Warning = false;
        else AnimateAvailableSkill();
    }
    protected void OnForceStopFlashing()
    {
        Warning = false;
    }

    void OnReset()
    {
        ResetArcRef?.BeginAnimation(Shape.StrokeThicknessProperty, _resetAnimation);
    }

    void AnimateAvailableSkill()
    {

        StopArcAnimation(MainArcRef); //stop any arc animations
        StopArcAnimation(PreArcRef); //stop any arc animations
        GlowRef?.BeginAnimation(OpacityProperty, _glowAnimation);
        if (Context == null) return;
        if (Context.FlashOnAvailable && (Game.Combat || Game.Encounter)) Warning = true;

    }

    protected void ActivatorMouseEnter(object sender, MouseEventArgs e)
    {
        if (DeleteButtonRef == null) return;
        DeleteButtonRef.Visibility = Visibility.Visible;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (DeleteButtonRef == null) return;
        DeleteButtonRef.Visibility = Visibility.Collapsed;
    }

    protected void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        if (Context == null) return;
        WindowManager.ViewModels.CooldownsVM.DeleteFixedSkill(Context);
    }
}