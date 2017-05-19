using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per BarCooldown.xaml
    /// </summary>
    public partial class BarCooldown : UserControl, INotifyPropertyChanged
    {
        public BarCooldown()
        {
            InitializeComponent();
        }
        DispatcherTimer _numberTimer;

        FixedSkillCooldown _context;
        private double currentCD;
        public double CurrentCD
        {
            get { return currentCD; }
            set
            {
                if (currentCD != value)
                {
                    currentCD = value;
                    NotifyPropertyChanged("CurrentCD");
                }
            }
        }

        private bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                if (isRunning == value) return;
                isRunning = value;
                NotifyPropertyChanged("IsRunning");
            }
        }



        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(BarCooldown));



        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (FixedSkillCooldown)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
            cdBar.RenderTransform = new ScaleTransform(0, 1, 0, .5);
            circle.RenderTransform = new TranslateTransform(0, 0);
            _numberTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            _numberTimer.Tick += (s, o) =>
            {
                CurrentCD = CurrentCD - 1 > 0 ? CurrentCD - 1 : 0;
            };


        }


        private void _context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Start")
            {
                CurrentCD = (double)_context.Cooldown / 1000;
                AnimateCooldown(_context.Cooldown);
            }
        }

        private void AnimateCooldown(int cooldown)
        {
            IsRunning = true;
            _numberTimer.IsEnabled = false;
            _numberTimer.IsEnabled = true;
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(cooldown));
            an.Completed += (s, ev) =>
            {
                IsRunning = false;
                _numberTimer.IsEnabled = false;
            };
            cdBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);

            var an2 = new DoubleAnimation(ActualWidth, 0, TimeSpan.FromMilliseconds(cooldown));
            circle.RenderTransform.BeginAnimation(TranslateTransform.XProperty, an2);
        }
    }
}
