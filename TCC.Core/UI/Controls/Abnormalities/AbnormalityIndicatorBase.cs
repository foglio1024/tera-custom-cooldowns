using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Nostrum.Controls;
using Nostrum.Factories;
using TCC.Data.Abnormalities;
using TCC.UI.Windows.Widgets;

namespace TCC.UI.Controls.Abnormalities
{
    public class AbnormalityIndicatorBase : UserControl
    {
        private static event Action<object, bool> VisibilityChanged = null!;
        private readonly DoubleAnimation _an;
        private AbnormalityDuration? _context;
        protected FrameworkElement? DurationLabelRef;
        protected FrameworkElement? MainArcRef;

        protected AbnormalityIndicatorBase()
        {
            _an = AnimationFactory.CreateDoubleAnimation(0, from: 0, to: 359.9);
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (!(DataContext is AbnormalityDuration ab)) return;
            _context = ab;
            _context.Refreshed += OnRefreshed;
            OnVisibilityChanged(Window.GetWindow(this), false);
            //if (_context.Abnormality.Hidden) Visibility = Visibility.Collapsed;
            if ((_context.Abnormality.Infinity || _context.Duration == uint.MaxValue) && DurationLabelRef != null)
            {
                DurationLabelRef.Visibility = Visibility.Hidden;
            }
            if (_context.Duration != uint.MaxValue && _context.Animated) AnimateCooldown();
            BeginAnimation(OpacityProperty, AnimationFactory.CreateDoubleAnimation(100, from: 0, to: 1));
            VisibilityChanged += OnVisibilityChanged;

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_context == null) return;
            _context.Refreshed -= OnRefreshed;
            _context = null;
            VisibilityChanged -= OnVisibilityChanged;
            Loaded -= UserControl_Loaded;
            Unloaded -= UserControl_Unloaded;


        }
        private void OnVisibilityChanged(object sender, bool mouseEnter)
        {
            Dispatcher.InvokeAsync(() =>
            {
                var myWindow = ReferenceEquals(sender, Window.GetWindow(this));

                var hidden = _context != null && sender switch
                {
                    BuffWindow _ => App.Settings.BuffWindowSettings.Hidden.Contains(_context.Abnormality.Id) && myWindow,
                    GroupWindow _ => App.Settings.GroupWindowSettings.Hidden.Contains(_context.Abnormality.Id) && myWindow,
                    _ => false
                };
                if(!myWindow) return;

                if (mouseEnter)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    if (hidden)
                        Visibility = Visibility.Collapsed;
                }
            });
        }

        private void OnRefreshed()
        {
            if (_context == null) return;
            if (_context.Duration == uint.MaxValue) return;
            if (!_context.Animated) return;
            Dispatcher?.Invoke(AnimateCooldown);
        }
        private void AnimateCooldown()
        {
            if (_context == null) return;
            _an.Duration = TimeSpan.FromMilliseconds(_context.DurationLeft);
            var fps = _context.DurationLeft > 20000 ? 1 : 10;
            Timeline.SetDesiredFrameRate(_an, fps);
            MainArcRef?.BeginAnimation(Arc.EndAngleProperty, _an);

        }

        public static void InvokeVisibilityChanged(object sender, bool val)
        {
            VisibilityChanged?.Invoke(sender, val);
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