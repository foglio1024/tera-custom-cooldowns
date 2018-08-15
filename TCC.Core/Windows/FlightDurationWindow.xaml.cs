using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Annotations;
using TCC.Converters;
using TCC.Data;
using TCC.Parsing.Messages;
using Arc = TCC.Controls.Arc;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per FlightDurationWindow.xaml
    /// </summary>
    public partial class FlightDurationWindow : INotifyPropertyChanged
    {
        private readonly DoubleAnimation _arcAn;
        private readonly DoubleAnimation _winShow;
        private readonly DoubleAnimation _winHide;
        private bool _firstLoad = true;

        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => SettingsManager.FlightGaugeRotation;
        public bool FlipFlightGauge => SettingsManager.FlipFlightGauge;


        public FlightDurationWindow()
        {
            InitializeComponent();

            ButtonsRef = null;
            MainContent = Content as UIElement;

            //SettingsManager.FlightGaugeWindowSettings.ShowAlways = true;
            //SettingsManager.FloatingButtonSettings.AutoDim = false;

            FlyingGuardianDataProvider.StackTypeChanged += (t) => NPC(nameof(Type));
            FlyingGuardianDataProvider.StacksChanged += SetStacks;
            FlyingGuardianDataProvider.IsInProgressChanged += OnFlyingGuardianInProgressChanged; 

            Init(SettingsManager.FlightGaugeWindowSettings);
            Opacity = 0;

            _winHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
            _winShow = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            _arcAn = new DoubleAnimation()
            {
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase()
            };
            _arcAn.Completed += (s, ev) =>
            {
                if (Arc.EndAngle >= 87 && 
                _arcAn.From < _arcAn.To && 
                !FlyingGuardianDataProvider.IsInProgress) HideWindow();
                else
                {
                    if (Opacity == 0) ShowWindow();
                }
            };
        }

        private void OnFlyingGuardianInProgressChanged(bool obj)
        {
            Dispatcher.Invoke(() =>
            {
                StacksContainer.Visibility = obj ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        public void SetEnergy(double val)
        {
            if (!SettingsManager.ShowFlightEnergy) return;
            Dispatcher.Invoke(() =>
            {
                if (Opacity == 0) ShowWindow();
                var c = new FactorToAngleConverter();
                _arcAn.From = Arc.EndAngle;
                _arcAn.To = (double)c.Convert(val / 1000, null, 4, null);
                Arc.BeginAnimation(Arc.EndAngleProperty, _arcAn);
            });
        }


        public void SetStacks(int stacks)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 9; i >= 0; i--)
                {
                    (StacksContainer.Children[i] as FrameworkElement).Opacity = i + 1 <= stacks ? 1 : 0.2;
                }
            });
        }

        private void HideWindow()
        {
            BeginAnimation(OpacityProperty, _winHide);
        }

        private void ShowWindow()
        {
            Opacity = 0;
            BeginAnimation(OpacityProperty, _winShow);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ExNPC(string prop)
        {
            NPC(prop);
        }
    }
}
