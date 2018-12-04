using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using TCC.Annotations;
using TCC.Data;
using TCC.Data.Pc;
using TCC.ViewModels;

namespace TCC.Controls.Classes
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
        //private DoubleAnimation _tc;
        //private DoubleAnimation _tcCd;


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
            if (_dc != null)
            {
               //_dc.TraverseCut.PropertyChanged += AnimateTraverseCut;
               //_dc.TraverseCut.ToZero += CooldownTraverseCut;
                //_dc.EdgeCounter.PropertyChanged += EdgeCounter_PropertyChanged;
                //_tc = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200)) {EasingFunction = new QuadraticEase()};
                //_tc.Completed += (_, __) => _tcAnimating = false;
                //_tcCd = new DoubleAnimation(0, TimeSpan.FromMilliseconds(0));
                SessionManager.CurrentPlayer.PropertyChanged += CheckStanceWarning;
                _dc.Stance.PropertyChanged += CheckStanceWarning;
            }
        }

        private void EdgeCounter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (_dc.EdgeCounter.Val == 8 || _dc.EdgeCounter.Val == 10)
            //{
            //    EdgeCounterBorder.BorderBrush = Brushes.White;
            //    EdgeCounterBorder.Background = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            //    ((DropShadowEffect) EdgeCounterBorder.Effect).Opacity = 1;
            //    ((DropShadowEffect) EdgeCounterBorder.Effect).Color = Colors.White;
            //}
            //else
            //{
            //    EdgeCounterBorder.Background = Application.Current.FindResource("RevampBackgroundBrush") as SolidColorBrush;
            //    EdgeCounterBorder.BorderBrush = Application.Current.FindResource("RevampBorderBrush") as SolidColorBrush;
            //    ((DropShadowEffect) EdgeCounterBorder.Effect).Opacity = 0;
            //}
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
                //_tcCd.From = _dc.TraverseCut.Factor * 359.9;
                //_tcCd.Duration = TimeSpan.FromMilliseconds(cd);
                //TODO: TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _tcCd);
                //TcArc.BeginAnimation(Arc.EndAngleProperty, _tcCd);
            });

        }

        private bool _tcAnimating;
        private bool _warningStance;

        private void AnimateTraverseCut(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                //_tc.To = _dc.TraverseCut.Factor * 359.9;
                _tcAnimating = true;
                //TODO: TcGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _tc);
                //TcArc.BeginAnimation(Arc.EndAngleProperty, _tc);

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
