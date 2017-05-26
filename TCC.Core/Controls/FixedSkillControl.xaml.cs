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

        FixedSkillCooldown _context;



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
        }

        void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Refresh")
            {
                if (_context.Cooldown == _context.OriginalCooldown) return;
                double newVal = (double)_context.Cooldown / (double)_context.OriginalCooldown;
                if (_context.Cooldown == 0) newVal = 0;
                if (newVal > 1) newVal = 1;

                AnimateArcAngle(newVal);
            }
            else if (e.PropertyName == "Start")
            {
                IsRunning = true;
                AnimateArcAngle();
            }
        }

        void AnimateArcAngle(double val = 1)
        {
            var an = new DoubleAnimation(val * 359.9, 0, TimeSpan.FromMilliseconds(_context.Cooldown));
            an.Completed += An_Completed;
            int fps = _context.Cooldown > 80000 ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(an, fps);
            arc.BeginAnimation(Arc.EndAngleProperty, an);
        }
        void AnimateArcThickness(double val = 1)
        {
            var an = new DoubleAnimation(val * 25, 0, TimeSpan.FromMilliseconds(_context.Cooldown));
            an.Completed += An_Completed;
            int fps = _context.Cooldown > 80000 ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(an, fps);
            arc.BeginAnimation(Arc.StrokeThicknessProperty, an);
        }
        void AnimateEllipseSize(double val = 0)
        {
            if (val == 0) val = 1;
            var an = new DoubleAnimation(val, 0, TimeSpan.FromMilliseconds(_context.Cooldown));
            an.Completed += An_Completed;
            int fps = _context.Cooldown > 80000 ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(an, fps);
            growEllipse.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
            growEllipse.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, an);
        }

        private void An_Completed(object sender, EventArgs e)
        {
            IsRunning = false;
            var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)) { EasingFunction = new QuadraticEase() };
            glow.BeginAnimation(OpacityProperty, an);
        }
    }
}
