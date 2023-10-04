using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Factories;
using TCC.Data.Npc;
using TCC.ViewModels;

namespace TCC.UI.Controls.NPCs;

public partial class BossGage
{
    bool _firstLoad = true;

    public BossViewModel? VM { get; set; }

    readonly DoubleAnimation _enrageArcAnimation;
    readonly DoubleAnimation _hpAnim;
    readonly DoubleAnimation _slideAnim;
    readonly DoubleAnimation _timerAnim;
    readonly DoubleAnimation _fadeAnim;

    public BossGage()
    {

        InitializeComponent();

        DataContextChanged += OnDataContextChanged;

        _slideAnim = AnimationFactory.CreateDoubleAnimation(250, 1, easing: true);
        _hpAnim = AnimationFactory.CreateDoubleAnimation(150, 1, easing: true);
        _timerAnim = AnimationFactory.CreateDoubleAnimation(1, 0, framerate: 20);
        _enrageArcAnimation = AnimationFactory.CreateDoubleAnimation(1, 0, 1, framerate: 30, completed: EnrageArcAnimation_Completed);
        _fadeAnim = AnimationFactory.CreateDoubleAnimation(250, 0, 1, framerate: 30);

        SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;
    }

    void OnDataContextChanged(object _, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not NPC npc) return;
        VM = new BossViewModel(npc);
    }

    void RefreshAbnormalityTemplate()
    {
        Abnormalities.ItemTemplateSelector = null;
        Abnormalities.ItemTemplateSelector = R.TemplateSelectors.BossAbnormalityTemplateSelector;
    }

    void AnimateTimer()
    {
        Dispatcher?.Invoke(() =>
        {
            if (VM?.NPC.TimerPattern == null) return;
            _timerAnim.From = VM.NPC.TimerPattern is HpTriggeredTimerPattern hptp ? hptp.StartAt : 1;
            _timerAnim.Duration = TimeSpan.FromSeconds(VM.NPC.TimerPattern.Duration);
            TimerDotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
            TimerBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
        });
    }

    void OnHpChanged()
    {
        AnimateHp();
        if (VM?.NPC == null) return;
        if (VM.NPC.Enraged) SlideEnrageIndicator(VM.CurrentPercentage);
    }

    void OnEnragedChanged()
    {
        if (VM?.NPC == null) return;
        if (VM.NPC.Enraged)
        {
            SlideEnrageIndicator(VM.CurrentPercentage);
            if (VM.NPC.EnragePattern == null) return;
            if (VM.NPC.EnragePattern.StaysEnraged) return;
            _enrageArcAnimation.Duration = TimeSpan.FromSeconds(VM.NPC.EnragePattern.Duration);
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
        }
        else
        {
            SlideEnrageIndicator(VM.NextEnragePercentage);
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = 0;

        }

    }

    void AnimateHp()
    {
        if (VM?.NPC == null) return; //weird but could happen 
        _hpAnim.To = VM.NPC.HPFactor; //still crashing here ffs
        DotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
        HpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_firstLoad) return;
        _firstLoad = false;
        if (VM == null) return;
        VM.HpFactorChanged += OnHpChanged;
        VM.EnragedChanged += OnEnragedChanged;
        VM.ReEnraged += OnReEnraged;
        VM.Disposed += OnDispose;
        if (VM.NPC.TimerPattern != null) VM.NPC.TimerPattern.Started += AnimateTimer;

        NextEnrage.RenderTransform = new TranslateTransform(HpBarGrid.Width, 0);

        SlideEnrageIndicator(VM.NextEnragePercentage);


    }

    void OnReEnraged()
    {
        if (VM?.NPC.EnragePattern == null) return;
        _enrageArcAnimation.Duration = TimeSpan.FromSeconds(VM.NPC.EnragePattern.Duration);
        EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
    }

    void OnDispose()
    {
        DataContextChanged -= OnDataContextChanged;
        SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
        if (VM == null) return;
        VM.HpFactorChanged -= OnHpChanged;
        VM.EnragedChanged -= OnEnragedChanged;
        VM.ReEnraged -= OnReEnraged;
        VM.Disposed -= OnDispose;
        if (VM.NPC.TimerPattern != null) VM.NPC.TimerPattern.Started -= AnimateTimer;
        AnimateFadeOut();
    }

    void AnimateFadeOut()
    {
        _fadeAnim.Duration = TimeSpan.FromMilliseconds(500);
        BeginAnimation(OpacityProperty, _fadeAnim);
    }

    void EnrageArcAnimation_Completed(object? sender, EventArgs e)
    {
        EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
        try
        {
            if (VM == null) return;
            ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = VM.NPC.Enraged ? 1 : 0;
        }
        catch
        {
            // ignored
        }
    }

    void SlideEnrageIndicator(double val)
    {
        Dispatcher?.InvokeAsync(() =>
        {
            _slideAnim.To = val < 0 ? 0 : HpBarGrid.ActualWidth * (val / 100);
            NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideAnim);
        });
    }

    void BossGage_OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (VM == null) return;
        VM.ShowOverrideBtn = true;
    }

    void BossGage_OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (VM == null) return;
        VM.ShowOverrideBtn = false;
    }
}