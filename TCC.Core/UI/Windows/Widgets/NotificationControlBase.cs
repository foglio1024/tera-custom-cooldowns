using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Nostrum.WPF.Factories;

namespace TCC.UI.Windows.Widgets;

public class NotificationControlBase : UserControl
{
    protected readonly DoubleAnimation _slideInAnimation;
    protected readonly DoubleAnimation _slideOutAnimation;
    protected readonly DoubleAnimation _fadeInAnimation;
    protected readonly DoubleAnimation _fadeOutAnimation;
    protected readonly DoubleAnimation _shrinkAnimation;
    protected Effect? _rootEffect;
    protected FrameworkElement? _root;
    protected NotificationInfoBase? _dc;

    protected NotificationControlBase()
    {
        Loaded += OnLoaded;
        _slideInAnimation = AnimationFactory.CreateDoubleAnimation(250, 0, 20, true, OnSlideInCompleted);
        _fadeInAnimation = AnimationFactory.CreateDoubleAnimation(250, 1, 0, framerate: 30);

        _slideOutAnimation = AnimationFactory.CreateDoubleAnimation(250, -20, 0, completed: OnFadeFinished, easing: true);
        _fadeOutAnimation = AnimationFactory.CreateDoubleAnimation(250, 0, framerate: 30);
        _shrinkAnimation = AnimationFactory.CreateDoubleAnimation(250, 0, 1, true, OnShrinkFinished);

    }

    protected void Init(FrameworkElement root)
    {
        _root = root;
        _root.Opacity = 0;

        // keep Root shadow reference, remove it from view, apply after animation
        _rootEffect = _root.Effect;
        _root.Effect = null;

    }
    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        _dc = (NotificationInfoBase)DataContext;
        if (_dc == null || _root == null) return;

        _root.BeginAnimation(OpacityProperty, _fadeInAnimation);
        _root.RenderTransform.BeginAnimation(TranslateTransform.YProperty, _slideInAnimation);
    }
    protected virtual void OnSlideInCompleted(object? sender, EventArgs e)
    {
        if (_root == null) return;
        _root.Effect = _rootEffect;
    }

    private void OnShrinkFinished(object? sender, EventArgs e)
    {
        if (_dc == null) return;
        WindowManager.ViewModels.NotificationAreaVM.DeleteNotification(_dc);
    }

    private void OnFadeFinished(object? sender, EventArgs e)
    {
        Dispatcher?.InvokeAsync(() =>
        {
            if (_root == null) return;
            var h = _root.ActualHeight;
            _root.Height = h;
            _root.Style = null;
            _root.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkAnimation);
        }, DispatcherPriority.Background);
    }


}