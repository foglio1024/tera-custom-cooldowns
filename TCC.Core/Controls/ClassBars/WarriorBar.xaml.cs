using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using TCC.Annotations;
using TCC.Data;
using TCC.ViewModels;
using Brushes = System.Windows.Media.Brushes;

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
        private DoubleAnimation _tc;
        private DoubleAnimation _tcCd;


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
            _dc.TraverseCut.OnToZero += CooldownTraverseCut;
            _dc.EdgeCounter.PropertyChanged += EdgeCounter_PropertyChanged;

            new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            _tc = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            _tc.Completed += (_, __) => _tcAnimating = false;
            _tcCd = new DoubleAnimation(0, TimeSpan.FromMilliseconds(0));
            new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(0));
            //_anCd.Completed += (o, args) => _cooldown = false;
            SessionManager.CurrentPlayer.PropertyChanged += CheckStanceWarning;
            _dc.Stance.PropertyChanged += CheckStanceWarning;
        }

        private void EdgeCounter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_dc.EdgeCounter.Val == 8 || _dc.EdgeCounter.Val == 10)
            {
                EdgeCounterBorder.BorderBrush = Brushes.White;
                EdgeCounterBorder.Background = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
                (EdgeCounterBorder.Effect as DropShadowEffect).Opacity = 1;
                (EdgeCounterBorder.Effect as DropShadowEffect).Color = Colors.White;
            }
            else
            {
                EdgeCounterBorder.Background = App.Current.FindResource("KrBgColor") as SolidColorBrush;
                EdgeCounterBorder.BorderBrush = App.Current.FindResource("KrBorderColor") as SolidColorBrush;
                (EdgeCounterBorder.Effect as DropShadowEffect).Opacity = 0;
            }
        }

        private void TranslateMovingTo(int edge)
        {
            var d = TimeSpan.FromMilliseconds(250);
            var e = new QuadraticEase();
            var rt = Moving.RenderTransform as TranslateTransform;
            var yan = new DoubleAnimation(-edge * 9, d) { EasingFunction = e };
            var xan = new DoubleAnimation(edge < 4 ? edge * 9 : (10 - edge - 2) * 9 - 4, d) { EasingFunction = e };
            rt.BeginAnimation(TranslateTransform.XProperty, xan);
            rt.BeginAnimation(TranslateTransform.YProperty, yan);
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
                _tcCd.From = _dc.TraverseCut.Factor * 359.9;
                _tcCd.Duration = TimeSpan.FromMilliseconds(cd);
                //TODO: TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _tcCd);
                TcArc.BeginAnimation(Arc.EndAngleProperty, _tcCd);
            });

        }

        private bool _tcAnimating;
        private bool _warningStance;

        private void AnimateTraverseCut(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                _tc.To = _dc.TraverseCut.Factor * 359.9;
                _tcAnimating = true;
                //TODO: TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _tc);
                TcArc.BeginAnimation(Arc.EndAngleProperty, _tc);

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
