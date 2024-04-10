using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;
using TCC.ViewModels;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace TCC.UI.Windows.Widgets;

public partial class FloatingButtonWindow
{
    private readonly DoubleAnimation _slideInAnim;
    private readonly DoubleAnimation _slideOutAnim;
    private readonly DoubleAnimation _bubbleAnim;
    private readonly DoubleAnimation _bubbleSlideIn;
    private readonly DoubleAnimation _bubbleSlideOut;
    private readonly ScaleTransform _bubbleScaleTransform;
    private readonly TranslateTransform _bubbleTranslateTransform;
    private readonly Timer _animRepeatTimer;


    private readonly DoubleAnimation _s1Anim;
    private readonly DoubleAnimation _s2Anim;
    private readonly DoubleAnimation _s3Anim;


    public FloatingButtonWindow(FloatingButtonViewModel vm)
    {
        DataContext = vm;
        _animRepeatTimer = new Timer { Interval = 2000 };
        _slideOutAnim = AnimationFactory.CreateDoubleAnimation(250, to: -288, easing: true);
        _slideInAnim = AnimationFactory.CreateDoubleAnimation(250, to: -1, easing: true);
        _bubbleSlideIn = AnimationFactory.CreateDoubleAnimation(250, to: -77, easing: true);
        _bubbleSlideOut = AnimationFactory.CreateDoubleAnimation(250, to: 0, easing: true);
        _bubbleAnim = AnimationFactory.CreateDoubleAnimation(800, 1, .75, true);
        vm.NotificationsCleared += OnNotificationsCleared;
        vm.NotificationsAdded += OnNotificationsAdded;

        var duration = TimeSpan.FromSeconds(5);
        const int start = -1;
        const int end = 1;
        const double tailSize = 0.5;
        const double headSize = 0.5;
        const int fps = 10;

        _s1Anim = new DoubleAnimation { Duration = duration, RepeatBehavior = RepeatBehavior.Forever, From = start, To = end };
        _s2Anim = new DoubleAnimation { Duration = duration, RepeatBehavior = RepeatBehavior.Forever, From = start + tailSize, To = end + tailSize };
        _s3Anim = new DoubleAnimation { Duration = duration, RepeatBehavior = RepeatBehavior.Forever, From = start + tailSize + headSize, To = end + tailSize + headSize };

        Timeline.SetDesiredFrameRate(_s1Anim, fps);
        Timeline.SetDesiredFrameRate(_s2Anim, fps);
        Timeline.SetDesiredFrameRate(_s3Anim, fps);

        InitializeComponent();

        _bubbleScaleTransform = (ScaleTransform) ((TransformGroup) NotificationBubble.RenderTransform).Children[0];
        _bubbleTranslateTransform = (TranslateTransform)((TransformGroup)NotificationBubble.RenderTransform).Children[1];

        MainContent = WindowContent;
        ButtonsRef = null;
        _canMove = false;
        Init(App.Settings.FloatingButtonSettings);

        SettingsWindowViewModel.IntegratedGpuSleepWorkaroundChanged += OnIntegratedGpuSleepWorkaroundChanged;
    }

    private void OnIntegratedGpuSleepWorkaroundChanged()
    {
        Dispatcher.InvokeAsync(() =>
        {
            if (App.Settings.IntegratedGpuSleepWorkaround) StartGradientAnimation();
            else StopGradientAnimation();
        });
    }

    private void StartGradientAnimation()
    {
        Stop1.BeginAnimation(GradientStop.OffsetProperty, _s1Anim);
        Stop2.BeginAnimation(GradientStop.OffsetProperty, _s2Anim);
        Stop3.BeginAnimation(GradientStop.OffsetProperty, _s3Anim);
    }

    private void StopGradientAnimation()
    {
        Stop1.BeginAnimation(GradientStop.OffsetProperty, null);
        Stop2.BeginAnimation(GradientStop.OffsetProperty, null);
        Stop3.BeginAnimation(GradientStop.OffsetProperty, null);
    }


    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        base.OnLoaded(sender, e);
        Left = 0;
        Top = Screen.PrimaryScreen!.Bounds.Height / 2f - ActualHeight / 2f;
        _animRepeatTimer.Tick += (_, _) => AnimateBubble();
        StartGradientAnimation();
    }
    protected override void OnVisibilityChanged()
    {
        base.OnVisibilityChanged();
        Dispatcher.InvokeAsync(() =>
        {
            var teraScreenBounds = FocusManager.TeraScreen.Bounds;
            var dpi = this.GetDpiScale();
            Left = teraScreenBounds.X /dpi.DpiScaleX ;
            Top = (teraScreenBounds.Y + teraScreenBounds.Height / 2f) / dpi.DpiScaleY;
        });
    }

    private void OnNotificationsAdded()
    {
        Dispatcher?.InvokeAsync(() =>
        {
            AnimateBubble();
            _animRepeatTimer.Start();
        });
    }

    private void OnNotificationsCleared()
    {
        Dispatcher?.InvokeAsync(() =>
        {
            _bubbleScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _bubbleScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            _animRepeatTimer.Stop();
        });
    }

    private void AnimateBubble()
    {
        _bubbleScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _bubbleAnim);
        _bubbleScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _bubbleAnim);
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideInAnim);
        _bubbleTranslateTransform.BeginAnimation(TranslateTransform.XProperty, _bubbleSlideIn);
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        Task.Delay(1000).ContinueWith(_ =>
        {
            Dispatcher.Invoke(() =>
            {
                if (IsMouseOver) return;
                RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideOutAnim);
                _bubbleTranslateTransform.BeginAnimation(TranslateTransform.XProperty, _bubbleSlideOut);
            });
        });
    }
}