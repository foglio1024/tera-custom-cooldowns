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
        FixedSkillCooldown _context;
        DispatcherTimer warnTimer;
        DoubleAnimation ExpandWarn;
        DoubleAnimation ExpandWarnInner;
        DoubleAnimation ArcAnimation;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isRunning = false;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                if (isRunning == value) return;
                isRunning = value;
                if (!isRunning) warnTimer.Stop();
                NotifyPropertyChanged("IsRunning");
            }
        }


        public FixedSkillControl()
        {
            InitializeComponent();
            warnTimer = new DispatcherTimer();
            warnTimer.Interval = TimeSpan.FromMilliseconds(1000);
            warnTimer.Tick += WarnTimer_Tick;


            ArcAnimation = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(1));
            ExpandWarn = new DoubleAnimation(0, 1.5, TimeSpan.FromMilliseconds(300)) { EasingFunction = new QuadraticEase() };
            ExpandWarnInner = new DoubleAnimation(35, 0, TimeSpan.FromMilliseconds(400)) { EasingFunction = new QuadraticEase() };
        }

        void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) || DataContext == null) return;
            _context = (FixedSkillCooldown)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
        }
        void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                if (e.PropertyName == "Refresh")
                {
                    if (_context.Cooldown == _context.OriginalCooldown) return;
                    double newVal = (double)_context.Cooldown / (double)_context.OriginalCooldown;
                    if (newVal > 1) newVal = 1;
                    if (_context.Cooldown == 0)
                    {
                        IsRunning = false;
                        AnimateAvailableSkill();
                        return;
                    }

                    AnimateArcAngle(newVal);
                }
                else if (e.PropertyName == "Start")
                {
                    IsRunning = true;
                    AnimateArcAngle();
                }
                else if (e.PropertyName == "IsAvailable")
                {
                    if (_context.IsAvailable)
                    {
                        IsRunning = false;
                        AnimateAvailableSkill();
                    }
                    else
                    {
                        warnTimer.Stop();
                    }
                }
            },
            DispatcherPriority.DataBind);

        }

        public void NotifyPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
        private void WarnTimer_Tick(object sender, EventArgs e)
        {
            WarnAvailableSkill();
        }



        void AnimateArcAngle(double val = 1)
        {
            ArcAnimation.Duration = TimeSpan.FromMilliseconds(_context.Cooldown);
            ArcAnimation.From = 359.9 * val;
            int fps = _context.Cooldown > 80000 ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(ArcAnimation, fps);
            arc.BeginAnimation(Arc.EndAngleProperty, ArcAnimation);
        }

        private void AnimateAvailableSkill()
        {
            arc.BeginAnimation(Arc.EndAngleProperty, null); //stop any arc animations
            if (!_context.FlashOnAvailable)
            {
                var an = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                glow.BeginAnimation(OpacityProperty, an);
            }
            WarnAvailableSkill();
            StartWarning();
        }
        private void StartWarning()
        {
            warnTimer.Start();
        }
        private void WarnAvailableSkill()
        {
            if (!SessionManager.Encounter) return;
            if (!_context.FlashOnAvailable) return;
            warnArc.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, ExpandWarn);
            warnArc.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, ExpandWarn);
            warnArc.BeginAnimation(Arc.StrokeThicknessProperty, ExpandWarnInner);
        }

    }
}
