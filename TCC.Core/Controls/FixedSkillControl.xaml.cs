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
    /// Logica di interazione per FixedSkillControl.xaml
    /// </summary>
    public partial class FixedSkillControl : UserControl, INotifyPropertyChanged
    {
        public FixedSkillControl()
        {
            InitializeComponent();
        }

        //DispatcherTimer _numberTimer;
        FixedSkillCooldown _context;

        //private double currentCD;
        //public double CurrentCD
        //{
        //    get { return currentCD; }
        //    set
        //    {
        //        if (currentCD != value)
        //        {
        //            currentCD = value;
        //            NotifyPropertyChanged("CurrentCD");
        //        }
        //    }
        //}

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

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (FixedSkillCooldown)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
            //_numberTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            //_numberTimer.Tick += (s, o) => {
            //    CurrentCD = CurrentCD - 1 > 0 ? CurrentCD - 1 : 0;
            //};
        }

        void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Refresh")
            {
                if (_context.Cooldown == _context.OriginalCooldown) return;
                //_numberTimer.Stop();
                //CurrentCD = (double)_context.Cooldown / 1000;
                double newAngle = (double)_context.Cooldown / (double)_context.OriginalCooldown;
                if (_context.Cooldown == 0) newAngle = 0;
                if (newAngle > 1) newAngle = 1;

                AnimateCooldown(newAngle);
            }
            else if (e.PropertyName == "Start")
            {
                IsRunning = true;
                //CurrentCD = (double)_context.Cooldown / 1000;
                //_numberTimer.IsEnabled = false;
                AnimateCooldown();

            }
        }
        void AnimateCooldown(double angle = 1)
        {
            var an = new DoubleAnimation(angle * 359.9, 0, TimeSpan.FromMilliseconds(_context.Cooldown));
            an.Completed += An_Completed;
            int fps = _context.Cooldown > 80000 ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(an, fps);
            arc.BeginAnimation(Arc.EndAngleProperty, an);
            //_numberTimer.IsEnabled = true;

        }

        private void An_Completed(object sender, EventArgs e)
        {
            IsRunning = false;
            //_numberTimer.IsEnabled = false;
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            glow.BeginAnimation(OpacityProperty, an);
        }
    }
}
