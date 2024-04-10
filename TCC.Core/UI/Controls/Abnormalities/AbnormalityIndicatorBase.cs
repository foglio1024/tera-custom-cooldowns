using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TCC.Data.Abnormalities;

namespace TCC.UI.Controls.Abnormalities;

public class AbnormalityIndicatorBase : UserControl
{
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
        if (DataContext is not AbnormalityDuration ab) return;
        _context = ab;
        _context.Refreshed += OnRefreshed;
        //if (_context.Abnormality.Hidden) Visibility = Visibility.Collapsed;
        if ((_context.Abnormality.Infinity || _context.Duration == uint.MaxValue) && DurationLabelRef != null)
        {
            DurationLabelRef.Visibility = Visibility.Hidden;
        }
        if (_context.Duration != uint.MaxValue && _context.Animated) AnimateCooldown();
        BeginAnimation(OpacityProperty, AnimationFactory.CreateDoubleAnimation(100, from: 0, to: 1));
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_context == null) return;
        _context.Refreshed -= OnRefreshed;
        _context = null;
        Loaded -= UserControl_Loaded;
        Unloaded -= UserControl_Unloaded;


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


    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size),
        typeof(double),
        typeof(RoundAbnormalityIndicator),
        new PropertyMetadata(18.0));
}