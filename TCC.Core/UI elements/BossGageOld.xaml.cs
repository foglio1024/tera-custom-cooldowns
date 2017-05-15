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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TCC.Data;
using TCC.Messages;
using TCC.Parsing;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per BossGageWindow.xaml
    /// </summary>
    public partial class BossGageOld: UserControl
    {
        int AnimationTime = 150;
        public bool BossVisible { get; set; }
        float MaxHP { get; set; }// = 2000000;
        float CurrentHP { get; set; }
        ulong CurrentId;
        bool isEnraged;
        bool IsEnraged { get { return isEnraged; }
            set
            {
                isEnraged = value;
                EnragedUpdated?.Invoke(value);          
            }
        }
        double LastEnragePercent { get; set; } = 100;
        int EnrageDuration = 36000;
        int CurrentEnrageTime = 36;
        event Action<bool> EnragedUpdated;
        NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = "`", NumberDecimalDigits = 0 };
        Timer TestTimer;
        Timer NumberTimer;

        public BossGageOld()
        {
            InitializeComponent();
            PacketRouter.BossInfoChanged += Update;
            EnragedUpdated += BossGageWindow_EnragedChanged;
            TestTimer = new Timer(10000);
            TestTimer.Elapsed += (s, ev) => {
                //AnimateHP(testHP);
                if(IsEnraged)
                {
                    IsEnraged = false; ;
                }
                else
                {
                    IsEnraged = true;
                }
            };
        }

        private void BossGageWindow_EnragedChanged(bool enraged)
        {
            Dispatcher.Invoke(() => 
            {
                if (enraged)
                {
                    number.Text = CurrentEnrageTime.ToString();
                    NextEnrage.BeginAnimation(MarginProperty, SlideNextEnrageIndicatorDirectAnimation(HPrect.ActualWidth - 2));
                    HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, GetDoubleAnimation(1));
                    HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, GetColorAnimation(Colors.Red));
                    EnrageArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(EnrageDuration)));
                    EnrageGrid.BeginAnimation(WidthProperty, GetDoubleAnimation(60));
                    NumberTimer = new Timer(1000);
                    NumberTimer.Elapsed += (s, ev) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            number.Text = CurrentEnrageTime.ToString();
                            CurrentEnrageTime--;
                        });
                    };
                    NumberTimer.Enabled = true;
                }
                else
                {
                    if (NumberTimer != null)
                    {
                        NumberTimer.Stop();
                    }
                    NextEnrage.BeginAnimation(MarginProperty, SlideNextEnrageIndicatorAnimation(LastEnragePercent - 10));
                    HPrect.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, GetDoubleAnimation(0));
                    HPrect.Fill.BeginAnimation(SolidColorBrush.ColorProperty, GetColorAnimation(Colors.OrangeRed));
                    EnrageGrid.BeginAnimation(WidthProperty, GetDoubleAnimation(0));
                    CurrentEnrageTime = 36;

                }
            });
        }

        double testHP;


        void Update(S_BOSS_GAGE_INFO packet)
        {

            MaxHP = packet.MaxHP;
            CurrentHP = packet.CurrentHP;
            AnimateHP(packet.CurrentHP);
            BossVisible = true;
            if(CurrentHP == 0)
            {
                BossVisible = false;
            }
            Dispatcher.Invoke(() =>
            {
                BossName.Text = MonsterDatabase.GetName(packet.Npc, packet.Type);
            });
        }


        void AnimateHP(double val)
        {
            Dispatcher.Invoke(() =>
            {
                HPrect.BeginAnimation(WidthProperty, GetDoubleAnimation(ValueToLength(val, MaxHP)));
                Perc.Text = String.Format("{1} / {2} - {0:0.0}%", 100* val / MaxHP, val.ToString("n",nfi), MaxHP.ToString("n",nfi));
            });
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.CharacterWindowLeft = Left;
            Properties.Settings.Default.CharacterWindowTop = Top;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            testHP = MaxHP;
            HPrect.Fill = new SolidColorBrush(Colors.OrangeRed);

            //TestTimer.Enabled = true;
            //this.Top = Properties.Settings.Default.CharacterWindowTop;
            //this.Left = Properties.Settings.Default.CharacterWindowLeft;
        }

        double ValueToLength(double value, double maxValue)
        {
            if (maxValue == 0)
            {
                return 0;
            }
            else
            {
                double n = BaseRect.Width * ((double)value / (double)maxValue);
                return n;
            }

        }
        private DoubleAnimation GetDoubleAnimation(double value)
        {
            return new DoubleAnimation(value, TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() };
        }
        private ColorAnimation GetColorAnimation(Color c)
        {
            return new ColorAnimation(c, TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() };
        }
        private ThicknessAnimation SlideNextEnrageIndicatorAnimation(double percentage)
        {
            var t = percentage * 398 / 100;
            Console.WriteLine("New position: {0}", t);
            return new ThicknessAnimation(new Thickness(t, 0, 0, 0), TimeSpan.FromSeconds(.4)) { EasingFunction = new QuadraticEase() };
        }
        private ThicknessAnimation SlideNextEnrageIndicatorDirectAnimation(double thickness)
        {
            Console.WriteLine("New direct position: {0}", thickness);
            return new ThicknessAnimation(new Thickness(thickness, 0, 0, 0), TimeSpan.FromSeconds(.4)) { EasingFunction = new QuadraticEase() };

        }

        public void SetEnraged(bool isEnraged)
        {
            if (IsEnraged && !isEnraged)
            {
                LastEnragePercent = 100 * CurrentHP / MaxHP;
            }
            IsEnraged = isEnraged;
        }
    }
}
