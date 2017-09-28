using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per AbnormalityIndicator.xaml
    /// </summary>
    public partial class AbnormalityIndicator
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
                if (_context.Duration == uint.MaxValue) return;
                if (!_context.Animated) return;
                AnimateCooldown();
            }

        }
        void AnimateCooldown()
        {
            var an = new DoubleAnimation(0, 359.9, TimeSpan.FromMilliseconds(_context.DurationLeft));
            int fps = _context.DurationLeft > 20000 ? 1 : 10;
            DoubleAnimation.SetDesiredFrameRate(an, fps);
            arc.BeginAnimation(Arc.EndAngleProperty, an);

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(DesignerProperties.GetIsInDesignMode(this))return;
            _context = (AbnormalityDuration)DataContext;
            _context.PropertyChanged += buff_PropertyChanged;
            RenderTransform = new ScaleTransform(1, 1, .5, .5);
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(85)));
            if (_context.Abnormality.Infinity || _context.Duration == uint.MaxValue) durationLabel.Visibility = Visibility.Hidden;

            if (_context.Duration != uint.MaxValue && _context.Animated)
            {
                AnimateCooldown();
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
            uint seconds = (uint)value/1000;
            uint minutes = seconds / 60;
            uint hours = minutes / 60;
            uint days = hours / 24;

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
            uint duration = (uint)value;
            if (duration == uint.MaxValue)
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
