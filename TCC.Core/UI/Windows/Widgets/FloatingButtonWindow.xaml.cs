using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.Factories;

namespace TCC.UI.Windows.Widgets
{
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

            InitializeComponent();

            _bubbleScaleTransform = (ScaleTransform) ((TransformGroup) NotificationBubble.RenderTransform).Children[0];
            _bubbleTranslateTransform = (TranslateTransform)((TransformGroup)NotificationBubble.RenderTransform).Children[1];

            MainContent = WindowContent;
            ButtonsRef = null;
            _canMove = false;
            Init(App.Settings.FloatingButtonSettings);
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            Left = 0;
            Top = Screen.PrimaryScreen.Bounds.Height / 2 - ActualHeight / 2;
            _animRepeatTimer.Tick += (_, __) => AnimateBubble();
        }
        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            if (FocusManager.TeraScreen == null) return;
            Dispatcher.InvokeAsync(() =>
            {
                var teraScreenBounds = FocusManager.TeraScreen.Bounds;
                Left = teraScreenBounds.X;
                Top = teraScreenBounds.Y + teraScreenBounds.Height / 2;
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

        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideInAnim);
            _bubbleTranslateTransform.BeginAnimation(TranslateTransform.XProperty, _bubbleSlideIn);
        }
        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Task.Delay(1000).ContinueWith(t =>
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
}