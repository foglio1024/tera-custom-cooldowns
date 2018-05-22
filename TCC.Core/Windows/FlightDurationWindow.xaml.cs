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

            ButtonsRef = null;
            MainContent = Content as UIElement;

            Init(SettingsManager.FlightGaugeWindowSettings);

            Opacity = 0;

            _winHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
            //_winHide.Completed += (s, ev) => Hide();
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
                    if (Opacity == 0) ShowWindow();
                }
            };
        }

        public void SetEnergy(double val)
        {
            if (!SettingsManager.ShowFlightEnergy) return;
            Dispatcher.Invoke(() =>
            {
                if (Opacity == 0) ShowWindow();
                var c = new FactorToAngleConverter();
                _arcAn.From = Arc.EndAngle;
                _arcAn.To = (double)c.Convert(val/1000, null, 4, null);
                Arc.BeginAnimation(Arc.EndAngleProperty, _arcAn);
            });
        }


        private void HideWindow()
        {
            BeginAnimation(OpacityProperty, _winHide);
        }

        private void ShowWindow()
        {
            //FocusManager.MakeClickThru(Handle);
            Opacity = 0;
            //Show();
            BeginAnimation(OpacityProperty, _winShow);
        }
    }
}
