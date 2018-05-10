using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ClassBars
{
    /// <summary>
    /// Logica di interazione per WarriorBar.xaml
    /// </summary>
    public partial class WarriorBar
    {
        public WarriorBar()
        {
            InitializeComponent();
        }

        private WarriorBarManager _dc;
        private DoubleAnimation _an;
        private DoubleAnimation _anCd;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = DataContext as WarriorBarManager;
            _dc.TraverseCut.PropertyChanged += AnimateTraverseCut;
            _dc.TempestAura.PropertyChanged += AnimateTempestAura;
            _dc.TempestAura.OnToZero += CooldownTempestAura;
            _an = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            _anCd = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(0));
            _anCd.Completed += (o, args) => _cooldown = false;
        }

        private bool _cooldown;
        private void CooldownTempestAura(uint cd)
        {
            Dispatcher.Invoke(() =>
            {
                _cooldown = true;
                _anCd.Duration = TimeSpan.FromMilliseconds(cd);
                TaGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _anCd);
            });
        }

        private void AnimateTempestAura(object sender, PropertyChangedEventArgs e)
        {
            
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                if (_cooldown) return;
                _an.To = _dc.TempestAura.Factor;
                TaGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _an);
            }
        }

        private void AnimateTraverseCut(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                _an.To = _dc.TraverseCut.Factor;
                TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _an);
            }
        }

    }
    public class WarriorStanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var s = (WarriorStance)value;
                switch (s)
                {
                    default:
                        return Application.Current.FindResource("DefaultBackgroundColor");
                    case WarriorStance.Assault:
                        return Application.Current.FindResource("AssaultStanceColor");
                    case WarriorStance.Defensive:
                        return Application.Current.FindResource("DefensiveStanceColor");
                }
            }
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ArcherStanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var s = (ArcherStance)value;
                switch (s)
                {
                    default:
                        return Brushes.DimGray;
                    case ArcherStance.SniperEye:
                        return Application.Current.FindResource("SniperEyeColor");
                }
            }
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
