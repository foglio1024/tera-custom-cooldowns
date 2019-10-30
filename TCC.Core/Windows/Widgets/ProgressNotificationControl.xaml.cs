using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TCC.Windows.Widgets
{
    public partial class ProgressNotificationControl
    {
        public ProgressNotificationControl()
        {
            InitializeComponent();
            Init(Root);
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            ((ProgressNotificationInfo)_dc).Disposed += OnNotificationDisposed;
        }

        private void OnNotificationDisposed()
        {

            Dispatcher?.InvokeAsync(() =>
            {
                ((ProgressNotificationInfo)_dc).Disposed -= OnNotificationDisposed;
                Root.Effect = null;
                Root.BeginAnimation(OpacityProperty, _fadeOutAnimation);
                Root.RenderTransform.BeginAnimation(TranslateTransform.YProperty, _slideOutAnimation);
            }, DispatcherPriority.Background);

        }
    }
}
