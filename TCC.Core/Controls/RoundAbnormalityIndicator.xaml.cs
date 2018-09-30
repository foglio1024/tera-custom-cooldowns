using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;

namespace TCC.Controls
{
    public partial class RoundAbnormalityIndicator : AbnormalityIndicatorBase
    {
        public RoundAbnormalityIndicator()
        {
            InitializeComponent();
            _durationLabel = DurationLabel;
            _mainArc = Arc;
        }
    }
    public class AbnormalityIndicatorBase : UserControl
    {
        protected readonly DoubleAnimation _an;
        protected AbnormalityDuration _context;
        protected FrameworkElement _durationLabel;
        protected FrameworkElement _mainArc;

        public AbnormalityIndicatorBase()
        {
            _an = new DoubleAnimation(0, 359.9, TimeSpan.Zero);
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (!(DataContext is AbnormalityDuration ab)) return;
            _context = ab;
            _context.Refreshed += OnRefreshed;
            if (_context.Abnormality.Infinity || _context.Duration == uint.MaxValue) _durationLabel.Visibility = Visibility.Hidden;
            if (_context.Duration != uint.MaxValue && _context.Animated) AnimateCooldown();
            this.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(100)));

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_context == null) return;
            _context.Refreshed -= OnRefreshed;
            _context = null;
        }
        private void OnRefreshed()
        {
            if (_context == null) return;
            if (_context.Duration == uint.MaxValue) return;
            if (!_context.Animated) return;
            Dispatcher.Invoke(AnimateCooldown);
        }
        private void AnimateCooldown()
        {
            if (_context == null) return;
            _an.Duration = TimeSpan.FromMilliseconds(_context.DurationLeft);
            var fps = _context.DurationLeft > 20000 ? 1 : 60;
            Timeline.SetDesiredFrameRate(_an, fps);
            _mainArc.BeginAnimation(Arc.EndAngleProperty, _an);

        }
        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size",
                                                                                             typeof(double),
                                                                                             typeof(RoundAbnormalityIndicator),
                                                                                             new PropertyMetadata(18.0));
    }
}

namespace TCC.Converters
{

    public class DurationLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var seconds = (uint)value / 1000;
            var minutes = seconds / 60;
            var hours = minutes / 60;
            var days = hours / 24;

            if (minutes < 3) return seconds.ToString();
            if (hours < 3) return minutes + "m";
            if (days < 1) return hours + "h";
            return days + "d";

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
            var stacks = (int)value;
            if (stacks > 1)
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
            var duration = (uint)value;
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
            var size = (double)value;
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
            var size = (double)value;
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
            var size = (double)value;
            return new Thickness(0, 0, 0, -size * 1.25);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
