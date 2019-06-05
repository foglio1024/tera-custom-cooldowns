using FoglioUtils;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.ViewModels;
using Arc = TCC.Controls.Arc;

namespace TCC.Windows.Widgets
{
    public partial class FlightDurationWindow 
    {
        private readonly DoubleAnimation _arcAn;
        private readonly DoubleAnimation _winShow;
        private readonly DoubleAnimation _winHide;

        public FlightDurationWindow()
        {
            InitializeComponent();

            VM = new FlightGaugeViewModel();
            DataContext = VM;

            ButtonsRef = null;
            MainContent = Content as UIElement;

            //FlyingGuardianDataProvider.StackTypeChanged += (t) => NPC(nameof(Type));
            //FlyingGuardianDataProvider.IsInProgressChanged += OnFlyingGuardianInProgressChanged;
            FlyingGuardianDataProvider.StacksChanged += SetStacks;
            SessionManager.CombatChanged += OnCombatChanged;

            Init(App.Settings.FlightGaugeWindowSettings, perClassPosition: false);
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

        public FlightGaugeViewModel VM { get; set; }

        public void SetEnergy(double val)
        {
            if (!App.Settings.ShowFlightEnergy) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Opacity == 0) ShowWindow();
                _arcAn.From = Arc.EndAngle;
                _arcAn.To = MathUtils.FactorToAngle(val / 1000, 4);
                Arc.BeginAnimation(Arc.EndAngleProperty, _arcAn);
            }));
        }

        private void OnCombatChanged()
        {
            if (SessionManager.Combat) HideWindow();
        }

        private void SetStacks()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                for (var i = 9; i >= 0; i--)
                {
                    ((FrameworkElement)StacksContainer.Children[i]).Opacity = i + 1 <= FlyingGuardianDataProvider.Stacks ? 1 : 0.2;
                }
            }));
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
    }
}
