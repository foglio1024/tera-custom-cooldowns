using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FoglioUtils;
using TCC.Controls;

namespace TCC.Windows.Widgets
{
    public partial class DefaultNotificationControl
    {
        private readonly DoubleAnimation _arcAnimation;
        private readonly DoubleAnimation _slideInAnimation;
        private readonly DoubleAnimation _slideOutAnimation;
        private readonly DoubleAnimation _fadeInAnimation;
        private readonly DoubleAnimation _fadeOutAnimation;
        private readonly DoubleAnimation _shrinkAnimation;
        private NotificationInfo _dc;
        public DefaultNotificationControl()
        {
            Loaded += OnLoaded;
            _arcAnimation = AnimationFactory.CreateDoubleAnimation(4000, 0, 359.9, completed: OnTimeExpired);

            _slideInAnimation = AnimationFactory.CreateDoubleAnimation(150, 0, -100, easing: true, completed: OnSlideInCompleted);
            _fadeInAnimation = AnimationFactory.CreateDoubleAnimation(150, 1,0);

            _slideOutAnimation = AnimationFactory.CreateDoubleAnimation(150, -100, 0, easing: true);
            _fadeOutAnimation = AnimationFactory.CreateDoubleAnimation(150, 0, completed: OnFadeFinished);
            _shrinkAnimation = AnimationFactory.CreateDoubleAnimation(150, 0, 1, easing: true, completed: OnShrinkFinished);

            Opacity = 0;
            InitializeComponent();
        }

        private void OnSlideInCompleted(object sender, EventArgs e)
        {
            _arcAnimation.Duration = TimeSpan.FromMilliseconds(_dc.Duration);
            TimeArc.BeginAnimation(Arc.EndAngleProperty, _arcAnimation);
        }

        private void OnShrinkFinished(object sender, EventArgs e)
        {
            WindowManager.ViewModels.NotificationArea.DeleteNotification(_dc);
        }

        private void OnFadeFinished(object sender, EventArgs e)
        {
            LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkAnimation);
        }

        private void OnTimeExpired(object sender, EventArgs e)
        {
            BeginAnimation(OpacityProperty, _fadeOutAnimation);
            RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideOutAnimation);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (NotificationInfo)DataContext;
            BeginAnimation(OpacityProperty, _fadeInAnimation);
            RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideInAnimation);
        }
    }
}
