using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Controls;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per HPbar.xaml
    /// </summary>
    public partial class CharacterWindow
    {
        private DoubleAnimation _hp;
        private DoubleAnimation _mp;
        private DoubleAnimation _st;
        private TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(50);
        private QuadraticEase DefaultEasing = new QuadraticEase();
        public CharacterWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = content;
            Init(SettingsManager.CharacterWindowSettings, ignoreSize: true);
            _hp = new DoubleAnimation()
            {
                Duration = DefaultDuration,
                EasingFunction = DefaultEasing
            };
            _mp = new DoubleAnimation()
            {
                Duration = DefaultDuration,
                EasingFunction = DefaultEasing
            };
            _st = new DoubleAnimation()
            {
                Duration = DefaultDuration,
                EasingFunction = DefaultEasing
            };
            (DataContext as CharacterWindowViewModel).Player.PropertyChanged += Player_PropertyChanged;
        }

        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Player.HpFactor))
            {
                _hp.From = (HpGovernor.LayoutTransform as ScaleTransform).ScaleX;
                _hp.To = (sender as Player).HpFactor;
                HpGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hp);
            }
            else if (e.PropertyName == nameof(Player.MpFactor))
            {
                _mp.From = (MpGovernor.LayoutTransform as ScaleTransform).ScaleX;
                _mp.To = (sender as Player).MpFactor;
                MpGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _mp);

            }
            else if (e.PropertyName == nameof(Player.StFactor))
            {
                _st.From = (StGovernor.LayoutTransform as ScaleTransform).ScaleX;
                _st.To = (sender as Player).StFactor;
                StGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _st);
                ReArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(((sender as Player).StFactor)*(360-80) + 40, _st.Duration));
            }
            else if (e.PropertyName == nameof(Player.ShieldFactor))
            {
                var _sh = new DoubleAnimation {Duration = DefaultDuration, EasingFunction = DefaultEasing};
                _sh.From = (ShGovernor.LayoutTransform as ScaleTransform).ScaleX;
                _sh.To = (sender as Player).ShieldFactor;
                ShGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _sh);
            }
        }
    }
}

