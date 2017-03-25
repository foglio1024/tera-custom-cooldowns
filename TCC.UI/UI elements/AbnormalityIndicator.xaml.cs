using System;
using System.Collections.Generic;
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
using TCC.Parsing;

namespace TCC.UI_elements
{
    /// <summary>
    /// Logica di interazione per AbnormalityIndicator.xaml
    /// </summary>
    public partial class AbnormalityIndicator : UserControl
    {
        public AbnormalityIndicator()
        {
            InitializeComponent();

            PacketRouter.BossBuffUpdated += PacketRouter_BossBuffUpdated;

            //abnormalityId.DataContext = this;
            abnormalityName.DataContext = this;
            abnormalityIcon.DataContext = this;
        }

        void InitTimer()
        {
            SecondsTimer = new System.Timers.Timer(1000);
            SecondsTimer.Elapsed += ((s, ev) =>
            {
                Dispatcher.Invoke(() =>
                {
                    CurrentTime--;
                    if (CurrentTime < 0)
                    {
                        number.Text = "";
                        SecondsTimer.Stop();
                    }
                    else if (CurrentTime > 60 * 60 * 24)
                    {
                        number.Text = String.Format("{0:0}d", CurrentTime / 3600 * 24);
                    }
                    else if (CurrentTime > 3600)
                    {
                        number.Text = String.Format("{0:0}h", CurrentTime / 3600);
                    }
                    else if (CurrentTime > 60)
                    {
                        number.Text = String.Format("{0:0}m", CurrentTime / 60);
                    }
                    else
                    {
                        number.Text = String.Format("{0:0}", CurrentTime);
                    }
                });
            });

        }

        void SetStacksNumber()
        {
            Dispatcher.Invoke(() =>
            {
                if (Stacks > 1)
                {
                    s.Visibility = Visibility.Visible;
                    stacksnumber.Text = Stacks.ToString();
                }
                else
                {
                    s.Visibility = Visibility.Hidden;
                }
            });
        }
        private void PacketRouter_BossBuffUpdated(Boss b, Data.Abnormality ab, int duration, int stacks)
        {
            //SecondsTimer.Stop();

            Dispatcher.Invoke(() =>
            {
                if (b.EntityId == BossId)
                {
                    if(ab.Id == AbnormalityId)
                    {
                        Duration = duration;
                        Stacks = stacks;
                        CurrentTime = duration / 1000;
                        number.Text = (duration/1000).ToString();
                        SetStacksNumber();
                        //InitTimer();
                        if (SecondsTimer != null)
                        {
                            SecondsTimer.Stop();
                            SecondsTimer.Enabled = true;
                        }

                        arc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(duration)));
                    }
                }

            });
        }



        public ulong BossId
        {
            get { return (ulong)GetValue(BossIdProperty); }
            set { SetValue(BossIdProperty, value); }
        }
        public static readonly DependencyProperty BossIdProperty = DependencyProperty.Register("BossId", typeof(ulong), typeof(AbnormalityIndicator));

        public uint AbnormalityId
        {
            get { return (uint)GetValue(AbnormalityIdProperty); }
            set { SetValue(AbnormalityIdProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityIdProperty = DependencyProperty.Register("AbnormalityId", typeof(uint), typeof(AbnormalityIndicator));

        public string AbnormalityName
        {
            get { return (string)GetValue(AbnormalityNameProperty); }
            set { SetValue(AbnormalityNameProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityNameProperty = DependencyProperty.Register("AbnormalityName", typeof(string), typeof(AbnormalityIndicator));



        public string IconName
        {
            get { return (string)GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconNameProperty =
            DependencyProperty.Register("IconName", typeof(string), typeof(AbnormalityIndicator));



        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(int), typeof(AbnormalityIndicator));

        public int Stacks
        {
            get { return (int)GetValue(StacksProperty); }
            set { SetValue(StacksProperty, value); }
        }
        public static readonly DependencyProperty StacksProperty = DependencyProperty.Register("Stacks", typeof(int), typeof(AbnormalityIndicator));

        System.Timers.Timer SecondsTimer;
        int CurrentTime;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.RenderTransform = new ScaleTransform(0, 0, .5, .5);
                this.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                this.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });

                SetStacksNumber();

                if (Duration < 2000000000)
                {
                    arc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(Duration)));
                    CurrentTime = Duration / 1000;
                    number.Text = (Duration / 1000).ToString();

                    InitTimer();
                    SecondsTimer.Stop();
                    SecondsTimer.Enabled = true;
                }
                else
                {
                    number.Text = "-";
                }
            });
        }
    }
}
