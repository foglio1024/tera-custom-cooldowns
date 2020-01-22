using Nostrum;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Windows.Widgets
{
    public partial class FloatingButtonWindow
    {
        private readonly DoubleAnimation _slideInAnim;
        private readonly DoubleAnimation _slideOutAnim;
        private readonly DoubleAnimation _bubbleAnim;
        private Timer _animRepeatTimer;

        public FloatingButtonWindow(FloatingButtonViewModel vm)
        {
            DataContext = vm;

            _slideOutAnim = AnimationFactory.CreateDoubleAnimation(150, -32, -1, true);
            _slideInAnim = AnimationFactory.CreateDoubleAnimation(150, -1, -32, true);
            _bubbleAnim = AnimationFactory.CreateDoubleAnimation(800, 1, .75, true);

            vm.NotificationsCleared += OnNotificationsCleared;
            vm.NotificationsAdded += OnNotificationsAdded;

            InitializeComponent();

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
            _animRepeatTimer = new Timer { Interval = 2000 };
            _animRepeatTimer.Tick += (_, __) => AnimateBubble();
        }
        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            if (FocusManager.TeraScreen == null) return;
            var teraScreenBounds = FocusManager.TeraScreen.Bounds;
            Left = teraScreenBounds.X;
            Top = teraScreenBounds.Y + teraScreenBounds.Height / 2;
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
                NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                _animRepeatTimer.Stop();
            });
        }
        private void AnimateBubble()
        {
            NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _bubbleAnim);
            NotificationBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _bubbleAnim);
        }

        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideInAnim);
        }
        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RootGrid.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideOutAnim);
        }
    }
}