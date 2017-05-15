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
    public partial class AbnormalityIndicator : UserControl
    {
        public AbnormalityIndicator()
        {
            InitializeComponent();
        }

        private AbnormalityDuration _context;
        private void buff_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Refresh")
            {
                if (((AbnormalityDuration)sender).Duration < 0) return;
                if (!((AbnormalityDuration)sender).Animated) return;
                var an = new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(((AbnormalityDuration)sender).Duration));
                int fps = ((AbnormalityDuration)sender).Duration > 80000 ? 1 : 30;
                DoubleAnimation.SetDesiredFrameRate(an, fps);
                arc.BeginAnimation(Arc.EndAngleProperty, an);
            }

        }

       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _context = (AbnormalityDuration)DataContext;
            _context.PropertyChanged += buff_PropertyChanged;
            this.RenderTransform = new ScaleTransform(1, 1, .5, .5);
            this.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(85)));

            if (((AbnormalityDuration)DataContext).Duration > 0 && ((AbnormalityDuration)DataContext).Animated)
            {
                var an = new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(((AbnormalityDuration)DataContext).Duration));
                int fps = ((AbnormalityDuration)DataContext).Duration > 80000 ? 1 : 30;
                DoubleAnimation.SetDesiredFrameRate(an, fps);
                arc.BeginAnimation(Arc.EndAngleProperty, an);
            }

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _context.PropertyChanged -= buff_PropertyChanged;
            _context = null;
        }
    }
}
namespace TCC.Converters
{
    public class DurationLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int seconds = (int)value/1000;
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
            return size / 1.7;
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
            return size / 1.9;
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
