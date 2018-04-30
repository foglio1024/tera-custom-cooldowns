using System;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Converters;
using TCC.Data;
using Arc = TCC.Controls.Arc;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per FlightDurationWindow.xaml
    /// </summary>
    public partial class FlightDurationWindow
    {
        private readonly DoubleAnimation _arcAn;
        private readonly DoubleAnimation _winShow;
        private readonly DoubleAnimation _winHide;
        private bool _firstLoad = true;
        public FlightDurationWindow()
        {
            InitializeComponent();
            InitWindow(new WindowSettings(Left, Top, Height, Width, true, ClickThruMode.Always, 1, false, 1, false, true, true));
            Hide();
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                if (WindowManager.IsTccVisible)
                {
                    RefreshTopmost();
                }
            };

            ButtonsRef = null;
            MainContentRef = Content as UIElement;
            _winHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
            _winHide.Completed += (s, ev) => Hide();
            _winShow = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            _arcAn = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase()
            };
            _arcAn.Completed += (s, ev) =>
            {
                if (Arc.EndAngle >= 87 && _arcAn.From < _arcAn.To) HideWindow();
                else
                {
                    if (!IsVisible) ShowWindow();
                }
            };
        }

        public void SetEnergy(double val)
        {
            if (!SettingsManager.ShowFlightEnergy) return;
            Dispatcher.Invoke(() =>
            {
                if (!IsVisible) ShowWindow();
                var c = new FactorToAngleConverter();
                _arcAn.From = Arc.EndAngle;
                // ReSharper disable once PossibleNullReferenceException
                _arcAn.To = (double)c.Convert(val/1000, null, 4, null);
                Arc.BeginAnimation(Arc.EndAngleProperty, _arcAn);
            });
        }

        private new void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            base.TccWindow_Loaded(sender, e);
            if(_firstLoad) Top = Top + 100;
            _firstLoad = false;
        }

        private void HideWindow()
        {
            BeginAnimation(OpacityProperty, _winHide);
        }

        private void ShowWindow()
        {
            FocusManager.MakeTransparent(Handle);
            Opacity = 0;
            Show();
            BeginAnimation(OpacityProperty, _winShow);
        }
    }
}
