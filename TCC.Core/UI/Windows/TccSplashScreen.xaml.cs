using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;

namespace TCC.UI.Windows;

public partial class TccSplashScreen
{
    DoubleAnimation _progressAnimation;
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
            var dpi = this.GetDpiScale();
            var top = (bounds.Height / 2f - ActualHeight / 2f - 40 )/ dpi.DpiScaleY;
            BeginAnimation(TopProperty, AnimationFactory.CreateDoubleAnimation(500, to: top, easing: true));
        }
        catch
        {
            // ignored
        }
    }

    void OnProgressChanged()
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