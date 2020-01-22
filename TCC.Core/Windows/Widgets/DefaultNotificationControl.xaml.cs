using System;
using System.Windows.Media;
using System.Windows.Threading;
using Nostrum;

namespace TCC.Windows.Widgets
{
    public partial class DefaultNotificationControl : NotificationControlBase
    {
        private readonly DispatcherTimer _duration;
        public DefaultNotificationControl()
        {
            _duration = new DispatcherTimer();
            InitializeComponent();
            Init(Root);
        }

        protected override void OnSlideInCompleted(object sender, EventArgs e)
        {
            base.OnSlideInCompleted(sender, e);
            if (_dc == null) return;
            _duration.Interval = TimeSpan.FromMilliseconds(_dc.Duration);
            _duration.Tick += OnTimeExpired;
            _duration.Start();
            Root.Effect = _rootEffect;
            TimeRect.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty,
                AnimationFactory.CreateDoubleAnimation((int)_dc.Duration, 0, 1));

        }

        private void OnTimeExpired(object sender, EventArgs e)
        {
            _duration.Stop();
            _duration.Tick -= OnTimeExpired;
            Dispatcher?.InvokeAsync(() =>
            {
                Root.Effect = null;
                Root.BeginAnimation(OpacityProperty, _fadeOutAnimation);
                Root.RenderTransform.BeginAnimation(TranslateTransform.YProperty, _slideOutAnimation);
            }, DispatcherPriority.Background);
        }
    }
}