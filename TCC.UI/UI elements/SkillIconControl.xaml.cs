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

namespace TCC
{
    public delegate void SkillEndedEventHandler(Skill sk, int cd);
    /// <summary>
    /// Logica di interazione per SkillIconControl.xaml
    /// </summary>
    public partial class SkillIconControl : UserControl, INotifyPropertyChanged
    {
        DispatcherTimer NumberTimer;
        DispatcherTimer MainTimer;
        DispatcherTimer CloseTimer;
        int ending = SkillManager.Ending;

        public static event SkillEndedEventHandler SkillEnded;
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }



        public string IconName
        {
            get { return (string)GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconNameProperty =
            DependencyProperty.Register("IconName", typeof(string), typeof(SkillIconControl));



        public uint Id
        {
            get { return (uint)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(uint), typeof(SkillIconControl));
        
        public int Cooldown
        {
            get { return (int)GetValue(CooldownProperty); }
            set { SetValue(CooldownProperty, value); }
        }
        public static readonly DependencyProperty CooldownProperty = DependencyProperty.Register("Cooldown", typeof(int), typeof(SkillIconControl));
        
        public string SkillName
        {
            get { return (string)GetValue(SkillNameProperty); }
            set { SetValue(SkillNameProperty, value); }
        }
        public static readonly DependencyProperty SkillNameProperty = DependencyProperty.Register("SkillName", typeof(string), typeof(SkillIconControl));

        public Skill Skill
        {
            get { return (Skill)GetValue(SkillProperty); }
            set { SetValue(SkillProperty, value); }
        }
        public static readonly DependencyProperty SkillProperty = DependencyProperty.Register("Skill", typeof(Skill), typeof(SkillIconControl));

        public void Reset(Skill sk)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (sk == Skill)
                {
                    CloseTimer.Stop();
                    RemoveSkill();
                }
            }));
        }

        private double currentCD;
        public double CurrentCD
        {
            get { return currentCD; }
            set {
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
            icon.DataContext = this;
            number.DataContext = this;
            SkillManager.Changed += ChangeCooldown;
            SkillManager.Reset += Reset;
        }

        private void ChangeCooldown(SkillCooldown s)
        {
            Dispatcher.Invoke(() =>
            {
                if (s.Skill.Name != this.Skill.Name) return;

                CurrentCD = (double)s.Cooldown / 1000;
                double newAngle = (double)s.Cooldown / (double)Cooldown;

                if (s.Cooldown > ending)
                {
                    MainTimer.Interval = TimeSpan.FromMilliseconds(s.Cooldown - ending);
                }
                else
                {
                    MainTimer.Interval = TimeSpan.FromMilliseconds(1);
                }

                //arc.BeginAnimation(Arc.EndAngleProperty, null);
                arc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9 * newAngle, 0, TimeSpan.FromMilliseconds(s.Cooldown)));
                //this.BeginAnimation(OpacityProperty, new DoubleAnimation(.5, 1, TimeSpan.FromMilliseconds(150)));
            });
        }
        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            CurrentCD = (double)Cooldown / 1000;
            ToolTip = SkillName;
            NumberTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
            MainTimer = new DispatcherTimer() {Interval = TimeSpan.FromMilliseconds(Cooldown) };
            CloseTimer = new DispatcherTimer() {Interval = TimeSpan.FromMilliseconds(ending)};

            CloseTimer.Tick += CloseTimer_Tick; ;

            NumberTimer.Tick += (s, o) =>
            {
                CurrentCD--;
            };
            
            MainTimer.Tick += (s, o) =>
            {
                //var c = new DoubleAnimation(22, 0, TimeSpan.FromMilliseconds(ending))
                //{
                //    EasingFunction = new QuadraticEase()
                //};
                var w = new DoubleAnimation(0, TimeSpan.FromMilliseconds(ending))
                {
                    EasingFunction = new QuadraticEase()
                };
                var h = new DoubleAnimation(0, TimeSpan.FromMilliseconds(ending))
                {
                    EasingFunction = new QuadraticEase()
                };
                //var t = new ThicknessAnimation(new Thickness(0), TimeSpan.FromMilliseconds(ending))
                //{
                //    EasingFunction = new QuadraticEase()
                //};

                //g.BeginAnimation(WidthProperty, c);
                //g.BeginAnimation(HeightProperty, c);
                //g.BeginAnimation(MarginProperty, t);
                MainGrid.BeginAnimation(WidthProperty, w);
                MainGrid.BeginAnimation(HeightProperty, h);
                //arc.BeginAnimation(WidthProperty, w);
                //arc.BeginAnimation(HeightProperty, h);
                //this.BeginAnimation(MarginProperty, t);
                CloseTimer.IsEnabled = true;

                MainTimer.Stop();
            };
            AnimateCooldown();
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            RemoveSkill();
        }

        void RemoveSkill()
        {
            MainTimer.Stop();
            NumberTimer.Stop();
            CloseTimer.Stop();
            SkillEnded?.Invoke(Skill, Cooldown);

        }

        void AnimateCooldown()
        {
            arc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(Cooldown)));
            NumberTimer.IsEnabled = true;
            MainTimer.IsEnabled = true;
        }

    }

    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            string iconName = value.ToString();
            iconName = iconName.Replace(".", "/");
            return "/resources/" + iconName + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
