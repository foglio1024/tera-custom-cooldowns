using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    public partial class AbnormalityIndicator : UserControl, INotifyPropertyChanged
    {
        public AbnormalityIndicator()
        {
            InitializeComponent();

            PacketRouter.BuffUpdated += PacketRouter_BuffUpdated;

            //abnormalityName.DataContext = this;
            rootGrid.DataContext = this;
            abnormalityIcon.DataContext = this;
            bgEll.DataContext = this;
            number.DataContext = this;
            fill.DataContext = this;
            durationLabel.DataContext = this;
            stacksLabel.DataContext = this;
            arc.DataContext = this;


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
                        //number.Text = "";
                        SecondsTimer.Stop();
                    }
                    //else if (CurrentTime > 60 * 60 * 24)
                    //{
                    //    number.Text = String.Format("{0:0}d", CurrentTime / 3600 * 24);
                    //}
                    //else if (CurrentTime > 3600)
                    //{
                    //    number.Text = String.Format("{0:0}h", CurrentTime / 3600);
                    //}
                    //else if (CurrentTime > 60)
                    //{
                    //    number.Text = String.Format("{0:0}m", CurrentTime / 60);
                    //}
                    //else
                    //{
                    //    number.Text = String.Format("{0:0}", CurrentTime);
                    //}
                });
            });

        }

        //void SetStacksNumber()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        if (Stacks > 1)
        //        {
        //            s.Visibility = Visibility.Visible;
        //            stacksnumber.Text = Stacks.ToString();
        //        }
        //        else
        //        {
        //            s.Visibility = Visibility.Hidden;
        //        }
        //    });
        //}
        private void PacketRouter_BuffUpdated(ulong target, Data.Abnormality ab, int duration, int stacks)
        {
            //SecondsTimer.Stop();

            Dispatcher.Invoke(() =>
            {
                if (target == TargetId)
                {
                    if (ab.Id == AbnormalityId)
                    {
                        Duration = duration;
                        Stacks = stacks;
                        CurrentTime = duration / 1000;
                        //number.Text = (duration / 1000).ToString();
                        //SetStacksNumber();
                        //InitTimer();
                        if (SecondsTimer != null)
                        {
                            SecondsTimer.Stop();
                            SecondsTimer.Enabled = true;
                        }
                        if (duration < 0)
                        {
                            //durationLabel.Visibility = Visibility.Hidden;
                            //number.Text = "-";
                            return;
                        }
                        arc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(duration)));
                    }
                }

            });
        }

        public ulong TargetId
        {
            get { return (ulong)GetValue(TargetIdProperty); }
            set { SetValue(TargetIdProperty, value); }
        }
        public static readonly DependencyProperty TargetIdProperty = DependencyProperty.Register("TargetId", typeof(ulong), typeof(AbnormalityIndicator));

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

        public string AbnormalityTooltip
        {
            get { return (string)GetValue(AbnormalityTooltipProperty); }
            set { SetValue(AbnormalityTooltipProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityTooltipProperty = DependencyProperty.Register("AbnormalityTooltip", typeof(string), typeof(AbnormalityIndicator));

        public AbnormalityType Type
        {
            get { return (AbnormalityType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(AbnormalityType), typeof(AbnormalityIndicator));

        public string IconName
        {
            get { return (string)GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }
        public static readonly DependencyProperty IconNameProperty = DependencyProperty.Register("IconName", typeof(string), typeof(AbnormalityIndicator));

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
        
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(AbnormalityIndicator));




        System.Timers.Timer SecondsTimer;

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        int currentTime;
        public int CurrentTime
        {
            get { return currentTime; }
            set
            {
                if (value != currentTime)
                {
                    currentTime = value;
                    NotifyPropertyChanged("CurrentTime");
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.RenderTransform = new ScaleTransform(0, 0, .5, .5);
                this.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                this.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
                abnormalityIcon.Width = Size * .9;
                abnormalityIcon.Height = Size * .9;
                bgEll.Width = Size;
                bgEll.Height = Size;
                arc.Width = Size * .9;
                arc.Height = Size * .9;

                if (Duration > 0)
                {
                    var an = new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(Duration));
                    arc.BeginAnimation(Arc.EndAngleProperty, an);
                    CurrentTime = Duration / 1000;
                    //number.Text = (Duration / 1000).ToString();

                    InitTimer();
                    SecondsTimer.Stop();
                    SecondsTimer.Enabled = true;
                }
                else
                {
                    //g.Visibility = Visibility.Hidden;
                    //number.Text = "-";
                }
            });
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            PacketRouter.BuffUpdated -= PacketRouter_BuffUpdated;
        }
    }
}
namespace TCC.Converters
{
    public class DurationLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int seconds = (int)value;
            int minutes = seconds / 60;
            int hours = minutes / 60;
            int days = hours / 24;

            if(minutes < 3)
            {
                return seconds.ToString();
            }
            else if(hours < 3)
            {
                return minutes + "m";
            }
            else if(days < 1)
            {
                return hours + "h";
            }
            else
            {
                return days + "d";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class AbnormalityStrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (AbnormalityType)value;
            switch (val)
            {
                case AbnormalityType.Stun:
                    return new SolidColorBrush(Colors.Red);
                case AbnormalityType.DamageOverTime:
                    return new SolidColorBrush(Color.FromRgb(0x98, 0x42, 0xf4));
                case AbnormalityType.WeakeningEffect:
                    return new SolidColorBrush(Color.FromRgb(0x8f, 0xf4, 0x42));
                case AbnormalityType.Buff:
                    return new SolidColorBrush(Color.FromRgb(0x3f, 0x9f, 0xff));
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StacksToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int stacks = (int)value;
            if(stacks > 1)
            {
                return Visibility.Visible; 
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DurationToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int duration = (int)value;
            if(duration < 0)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SizeToStackLabelSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (double)value;
            return size / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SizeToDurationLabelSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (double)value;
            return size / 1.8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SizeToDurationLabelMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (double)value;
            return new Thickness(0, 0, 0, -size * 1.25);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
