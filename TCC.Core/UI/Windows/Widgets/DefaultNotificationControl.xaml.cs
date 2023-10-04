using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nostrum.WPF.Factories;

namespace TCC.UI.Windows.Widgets;

public partial class DefaultNotificationControl
{
    readonly DispatcherTimer _duration;
    readonly DoubleAnimation _anim;

    public DefaultNotificationControl()
    {
        _duration = new DispatcherTimer();
        _anim = AnimationFactory.CreateDoubleAnimation(0, 0, 1);
        InitializeComponent();
        Init(Root);
    }

    protected override void OnSlideInCompleted(object? sender, EventArgs e)
    {
        base.OnSlideInCompleted(sender, e);
        if (_dc == null) return;
        _dc.Disposed += OnDisposed;
        _duration.Interval = TimeSpan.FromMilliseconds(_dc.Duration);
        _duration.Tick += OnTimeExpired;
        _duration.Start();
        Root.Effect = _rootEffect;
        _anim.Duration = TimeSpan.FromMilliseconds(_dc.Duration);
        TimeRect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _anim);

    }

    void OnDisposed()
    {
        OnTimeExpired(null, null);
    }

    void OnTimeExpired(object? sender, EventArgs? e)
    {
        _duration.Stop();
        _duration.Tick -= OnTimeExpired;
        AnimateDismiss();
    }

    void AnimateDismiss()
    {
        Dispatcher?.InvokeAsync(() =>
        {
            Root.Effect = null;
            Root.BeginAnimation(OpacityProperty, _fadeOutAnimation);
            Root.RenderTransform.BeginAnimation(TranslateTransform.YProperty, _slideOutAnimation);
        }, DispatcherPriority.Background);

    }
}