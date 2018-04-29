using System;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Converters;
using Arc = TCC.Controls.Arc;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per FlightDurationWindow.xaml
    /// </summary>
    public partial class FlightDurationWindow : TccWindow
    {
        private DoubleAnimation _arcAn, _winShow, _winHide;
        private bool _firstLoad = true;
        public FlightDurationWindow()
        {
            InitializeComponent();
            InitWindow(new WindowSettings(this.Left, this.Top, this.Height, this.Width, true, ClickThruMode.Always, 1, false, 1, false, true, true));
            this.Hide();
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                if (WindowManager.IsTccVisible)
                {
                    this.RefreshTopmost();
                }
            };

            _b = null;
            _c = this.Content as UIElement;
            _winHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
            _winHide.Completed += (s, ev) => this.Hide();
            _winShow = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            _arcAn = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase()
            };
            _arcAn.Completed += (s, ev) =>
            {
                if (arc.EndAngle >= 87 && _arcAn.From < _arcAn.To) this.HideWindow();
                else
                {
                    if (!this.IsVisible) this.ShowWindow();
                }
            };
        }

        public void SetEnergy(double val)
        {
            if (!SettingsManager.ShowFlightEnergy) return;
            Dispatcher.Invoke(() =>
            {
                if (!this.IsVisible) this.ShowWindow();
                var c = new FactorToAngleConverter();
                _arcAn.From = arc.EndAngle;
                _arcAn.To = (double)c.Convert(val/1000, null, 4, null);
                arc.BeginAnimation(Arc.EndAngleProperty, _arcAn);
            });
        }

        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            base.TccWindow_Loaded(sender, e);
            if(_firstLoad) this.Top = this.Top + 100;
            _firstLoad = false;
        }

        private void HideWindow()
        {
            this.BeginAnimation(OpacityProperty, _winHide);
        }

        private void ShowWindow()
        {
            FocusManager.MakeTransparent(this._handle);
            this.Opacity = 0;
            this.Show();
            this.BeginAnimation(OpacityProperty, _winShow);
        }
    }
}
