using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();
            try
            {
                var r = new Random();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"resources/images/splash/{r.Next(1, 15)}.jpg");
                Img.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            catch { }
        }

        public void SetText(string t)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Txt.Text = t));
        }
        public void SetVer(string t)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Ver.Text = t;
                if (App.Experimental) Ver.Foreground = R.Brushes.HpBrushLight;
            }));
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
        public new void CloseWindowSafe()
        {
            DoubleAnimation an;
            Dispatcher.Invoke(() =>
            {
                an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { EasingFunction = new QuadraticEase() };

                an.Completed += (s, ev) =>
                 {
                     Close();
                     Dispatcher.InvokeShutdown();
                 };
                BeginAnimation(OpacityProperty, an);
            });
        }
        private bool _waiting = true;
        private bool _updateAnswer;
        public bool AskUpdate(string updateText)
        {
            Dispatcher.Invoke(() =>
            {
                Topmost = true;
                Topmost = false;
            });
            SetText(updateText);
            Dispatcher.Invoke(() => ShowHideButton(true));
            while (_waiting)
            {
                Thread.Sleep(1);
            }
            _waiting = true;
            return _updateAnswer;
        }
        private void ShowHideButton(bool show)
        {
            var scale = show ? 1 : 0;
            Dispatcher.Invoke(() => ButtonsGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                new DoubleAnimation(scale, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() }));
        }

        private void NoClick(object sender, RoutedEventArgs e)
        {
            ShowHideButton(false);
            _updateAnswer = false;
            _waiting = false;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            ShowHideButton(false);

            _updateAnswer = true;
            _waiting = false;
        }

        internal void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Bar.Value = e.ProgressPercentage;
                if (Bar.Value == 100) Bar.Value = 0;
            });
        }
    }
}