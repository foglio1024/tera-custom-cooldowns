using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FoglioUtils;

namespace TCC.Windows
{
    public partial class TccSplashScreen
    {
        private DoubleAnimation _progressAnimation;
        public SplashScreenViewModel VM { get; }
        public TccSplashScreen()
        {
            InitializeComponent();
            VM = new SplashScreenViewModel();
            DataContext = VM;
            _progressAnimation = AnimationFactory.CreateDoubleAnimation(250, VM.ProgressPerc, easing: true);
            VM.ProgressChangedEvent += OnProgressChanged;
        }

        private void OnProgressChanged()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _progressAnimation.To = VM.ProgressPerc;
                (ProgressBar.RenderTransform as ScaleTransform)?.BeginAnimation(ScaleTransform.ScaleXProperty, _progressAnimation);
            }));
        }

        public new void CloseWindowSafe()
        {
            Dispatcher.Invoke(() =>
            {
                var an = AnimationFactory.CreateDoubleAnimation(300, 0, easing: true, completed: (s, ev) =>
                {
                    Close();
                    Dispatcher.InvokeShutdown();
                });

                BeginAnimation(OpacityProperty, an);
            });
        }


    }
}
