using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Annotations;
using TCC.Data;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Controls.ClassBars
{
    /// <summary>
    /// Logica di interazione per WarriorBar.xaml
    /// </summary>
    public partial class WarriorBar : INotifyPropertyChanged
    {
        public WarriorBar()
        {
            InitializeComponent();
        }

        private WarriorBarManager _dc;
        private DoubleAnimation _an;
        private DoubleAnimation _tc;
        private DoubleAnimation _tcCd;
        private DoubleAnimation _anCd;



        public bool WarningStance
        {
            get => _warningStance;
            set
            {
                if (_warningStance == value) return;
                _warningStance = value;
                NPC();
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dc = DataContext as WarriorBarManager;
            _dc.TraverseCut.PropertyChanged += AnimateTraverseCut;
            _dc.TempestAura.PropertyChanged += AnimateTempestAura;
            _dc.TempestAura.OnToZero += CooldownTempestAura;
            _dc.TraverseCut.OnToZero += CooldownTraverseCut;
            _an = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            _tc = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            _tc.Completed += (_, __) => _tcAnimating = false;
            _tcCd = new DoubleAnimation(0, TimeSpan.FromMilliseconds(0));
            _anCd = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(0));
            _anCd.Completed += (o, args) => _cooldown = false;
            SessionManager.CurrentPlayer.PropertyChanged += CheckStanceWarning;
            _dc.Stance.PropertyChanged += CheckStanceWarning;
        }

        private void CheckStanceWarning(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Player.IsInCombat) &&
                e.PropertyName != nameof(StanceTracker<WarriorStance>.CurrentStance)) return;
            WarningStance = _dc.Stance.CurrentStance == WarriorStance.None && SessionManager.CurrentPlayer.IsInCombat;
        }

        private void CooldownTraverseCut(uint cd)
        {
            if (_tcAnimating)
            {
                Task.Delay(210).ContinueWith(t => CooldownTraverseCut(cd - 210));
                return;
            }
            //if (!_dc.TraverseCut.Maxed) return;
            Dispatcher.Invoke(() =>
            {
                _tcCd.From = _dc.TraverseCut.Factor;
                _tcCd.Duration = TimeSpan.FromMilliseconds(cd);
                TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _tcCd);
            });

        }

        private bool _cooldown;
        private void CooldownTempestAura(uint cd)
        {
            //Dispatcher.Invoke(() =>
            //{
            //    _cooldown = true;
            //    _anCd.Duration = TimeSpan.FromMilliseconds(cd);
            //    TaGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _anCd);
            //});
        }

        private void AnimateTempestAura(object sender, PropertyChangedEventArgs e)
        {

            //if (e.PropertyName == nameof(StatTracker.Factor))
            //{
            //    if (_cooldown) return;
            //    _an.To = _dc.TempestAura.Factor;
            //    TaGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _an);
            //}
        }

        private bool _tcAnimating;
        private bool _warningStance;

        private void AnimateTraverseCut(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                _tc.To = _dc.TraverseCut.Factor;
                _tcAnimating = true;
                TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _tc);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                        return Brushes.Transparent;
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
