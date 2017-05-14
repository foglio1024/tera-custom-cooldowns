using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using TCC.ViewModels;

namespace TCC
{
    public delegate void SkillEndedEventHandler(Skill sk, int cd);
    public partial class SkillIconControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private DispatcherTimer NumberTimer;
        //private DispatcherTimer MainTimer;
        private DispatcherTimer CloseTimer;
        private int ending = SkillManager.Ending;

        private SkillCooldown _context;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

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

        public SkillIconControl()
        {
            InitializeComponent();
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            _context = (SkillCooldown)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;

            LayoutTransform = new ScaleTransform(1, 1, .5, .5);

<<<<<<< HEAD
                var an = new DoubleAnimation(359.9 * newAngle, 0, TimeSpan.FromMilliseconds(s.Cooldown));
                var fps = (s.Cooldown > 2 * 60 * 1000) ? 1 : 30;
                DoubleAnimation.SetDesiredFrameRate(an, fps);
                arc.BeginAnimation(Arc.EndAngleProperty, an);
            });
        }
        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            CurrentCD = (double)Cooldown / 1000;
=======
            CurrentCD = (double)_context.Cooldown / 1000;
>>>>>>> refs/remotes/origin/multithread-test

            NumberTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            CloseTimer = new  DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(ending) };

            CloseTimer.Tick += CloseTimer_Tick;
            NumberTimer.Tick += (s, o) =>
            {
                CurrentCD--;
            };
            AnimateCooldown();
        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Refresh")
            {
                if (_context.Cooldown == _context.OriginalCooldown) return;
                NumberTimer.Stop();
                NumberTimer.IsEnabled = true;
                CurrentCD = (double)_context.Cooldown / 1000;
                double newAngle = (double)_context.Cooldown / (double)_context.OriginalCooldown;
                if (_context.Cooldown == 0) newAngle = 0;
                if (newAngle > 1) newAngle = 1;

                //if (_context.Cooldown > ending)
                //{
                //    MainTimer.Interval = TimeSpan.FromMilliseconds(_context.Cooldown - ending);
                //}
                //else
                //{
                //    MainTimer.Interval = TimeSpan.FromMilliseconds(1);
                //}
                AnimateCooldown(newAngle);
            }
            else if(e.PropertyName == "Ending")
            {
                var w = new DoubleAnimation(0, TimeSpan.FromMilliseconds(ending))
                {
                    EasingFunction = new QuadraticEase()
                };
                var h = new DoubleAnimation(0, TimeSpan.FromMilliseconds(ending))
                {
                    EasingFunction = new QuadraticEase()
                };
                CooldownBarWindowManager.Instance.Dispatcher.Invoke(() =>
                {
                    this.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, w);
                    this.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, h);
                });
                CloseTimer.IsEnabled = true;
            }
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            Dispose();
            CooldownBarWindowManager.Instance.RemoveSkill(_context.Skill);
        }

        void AnimateCooldown(double angle = 1)
        {
<<<<<<< HEAD
            var an = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(Cooldown));
            var fps = (Cooldown > 2 * 60 * 1000) ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(an,fps);
=======
            var an = new DoubleAnimation(angle*359.9, 0, TimeSpan.FromMilliseconds(_context.Cooldown));
            int fps = _context.Cooldown > 80000 ? 1 : 30;
            DoubleAnimation.SetDesiredFrameRate(an, fps);
>>>>>>> refs/remotes/origin/multithread-test
            arc.BeginAnimation(Arc.EndAngleProperty, an);
            NumberTimer.IsEnabled = true;
            //MainTimer.IsEnabled = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        public void Dispose()
        {
            NumberTimer.Stop();
            //MainTimer.Stop();
            CloseTimer.Stop();

            NumberTimer = null;
            //MainTimer = null;
            CloseTimer = null;
        }
    }
}
namespace TCC.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iconName = "unknown";
            if (value != null)
            {
                if (value.ToString() != "")
                {
                    iconName = value.ToString();
                    iconName = iconName.Replace(".", "/");
                }
            }
            return Environment.CurrentDirectory + "/resources/images/" + iconName + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
