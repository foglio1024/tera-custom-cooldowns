using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TCC.Windows
{
    /// <inheritdoc cref="TccWindow" />
    /// <summary>
    /// Logica di interazione per SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        public void SetText(string t)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => txt.Text = t));
        }
        public void SetVer(string t)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ver.Text = t));
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
        private bool waiting = true;
        private bool updateAnswer = false;
        public bool AskUpdate(string updateText)
        {
            Dispatcher.Invoke(() =>
            {
                Topmost = true;
                Topmost = false;
            });
            SetText(updateText);
            Dispatcher.Invoke(() => ShowHideButton(true));
            while (waiting)
            {
                Thread.Sleep(1);
            }
            waiting = true;
            return updateAnswer;
        }
        private void ShowHideButton(bool show)
        {
            var scale = show ? 1 : 0;
            Dispatcher.Invoke(() => buttonsGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                new DoubleAnimation(scale, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() }));
        }

        private void NoClick(object sender, RoutedEventArgs e)
        {
            ShowHideButton(false);
            updateAnswer = false;
            waiting = false;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            ShowHideButton(false);

            updateAnswer = true;
            waiting = false;
        }

        internal void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                bar.Value = e.ProgressPercentage;
                if (bar.Value == 100) bar.Value = 0;
            });
        }
    }
}