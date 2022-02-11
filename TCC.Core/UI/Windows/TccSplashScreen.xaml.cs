using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nostrum.WPF.Factories;
using TCC.Utilities;

namespace TCC.UI.Windows
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
            Loaded += OnLoaded;
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var screen = Screen.FromRectangle(new System.Drawing.Rectangle((int)Left, (int)Top, (int)Width, (int)Height));
                var bounds = screen.Bounds;
                var (_, dpiY) = TccUtils.GetDPI(this);
                var top = (bounds.Height / 2 - ActualHeight / 2 - 40 )/ dpiY;
                BeginAnimation(TopProperty, AnimationFactory.CreateDoubleAnimation(500, to: top, easing: true));
            }
            catch { }
        }

        private void OnProgressChanged()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                _progressAnimation.To = VM.ProgressPerc;
                (ProgressBar.RenderTransform as ScaleTransform)?.BeginAnimation(ScaleTransform.ScaleXProperty, _progressAnimation);
            });
        }

        public new void CloseWindowSafe()
        {
            Dispatcher?.Invoke(() =>
            {
                var an = AnimationFactory.CreateDoubleAnimation(300, 0, easing: true, completed: (_, _) =>
                {
                    Close();
                    Dispatcher?.InvokeShutdown();
                });

                BeginAnimation(OpacityProperty, an);
            });
        }

        public static void InitOnNewThread()
        {
            var waiting = true;
            var ssThread = new Thread(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                    App.SplashScreen = new TccSplashScreen();
                    App.SplashScreen.VM.BottomText = "Initializing...";
                    App.SplashScreen.Show();
                    waiting = false;
                    Dispatcher.Run();
                })
            { Name = "SplashScreen window thread" };
            ssThread.SetApartmentState(ApartmentState.STA);
            ssThread.Start();
            while (waiting) Thread.Sleep(1);
        }


    }
}
