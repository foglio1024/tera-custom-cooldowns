using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TCC.Data.Abnormalities;

namespace TCC.Controls.Abnormalities
{
    public class AbnormalityIndicatorBase : UserControl
    {
        private readonly DoubleAnimation _an;
        private AbnormalityDuration _context;
        protected FrameworkElement DurationLabelRef;
        protected FrameworkElement MainArcRef;

        protected AbnormalityIndicatorBase()
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
            if (_context.Abnormality.Infinity || _context.Duration == uint.MaxValue) DurationLabelRef.Visibility = Visibility.Hidden;
            if (_context.Duration != uint.MaxValue && _context.Animated) AnimateCooldown();
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(100)));

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
            var fps = _context.DurationLeft > 20000 ? 1 : 5;
            Timeline.SetDesiredFrameRate(_an, fps);
            MainArcRef.BeginAnimation(Arc.EndAngleProperty, _an);

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