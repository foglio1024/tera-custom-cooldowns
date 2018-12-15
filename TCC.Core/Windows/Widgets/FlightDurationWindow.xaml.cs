using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.Annotations;
using TCC.Data;
using Arc = TCC.Controls.Arc;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per FlightDurationWindow.xaml
    /// </summary>
    public partial class FlightDurationWindow : INotifyPropertyChanged
    {
        private readonly DoubleAnimation _arcAn;
        private readonly DoubleAnimation _winShow;
        private readonly DoubleAnimation _winHide;

        public FlightStackType Type => FlyingGuardianDataProvider.StackType;
        public double FlightGaugeRotation => Settings.SettingsHolder.FlightGaugeRotation;
        public bool FlipFlightGauge => Settings.SettingsHolder.FlipFlightGauge;


        public FlightDurationWindow()
        {
            InitializeComponent();

            ButtonsRef = null;
            MainContent = Content as UIElement;

            //Settings.FlightGaugeWindowSettings.ShowAlways = true;
            //Settings.FloatingButtonSettings.AutoDim = false;

            FlyingGuardianDataProvider.StackTypeChanged += (t) => NPC(nameof(Type));
            FlyingGuardianDataProvider.StacksChanged += SetStacks;
            FlyingGuardianDataProvider.IsInProgressChanged += OnFlyingGuardianInProgressChanged;
            SessionManager.CombatChanged += OnCombatChanged;

            Init(Settings.SettingsHolder.FlightGaugeWindowSettings, perClassPosition: false);
            Opacity = 0;

            _winHide = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));
            _winShow = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));
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

        private void OnCombatChanged()
        {
            if (SessionManager.Combat) HideWindow();
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
            if (!Settings.SettingsHolder.ShowFlightEnergy) return;
            Dispatcher.Invoke(() =>
            {
                if (Opacity == 0) ShowWindow();
                _arcAn.From = Arc.EndAngle;
                _arcAn.To = Utils.FactorToAngle(val / 1000, 4);
                Arc.BeginAnimation(Arc.EndAngleProperty, _arcAn);
            });
        }


        private void SetStacks(int stacks)
        {
            Dispatcher.Invoke(() =>
            {
                for (var i = 9; i >= 0; i--)
                {
                    ((FrameworkElement) StacksContainer.Children[i]).Opacity = i + 1 <= stacks ? 1 : 0.2;
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
