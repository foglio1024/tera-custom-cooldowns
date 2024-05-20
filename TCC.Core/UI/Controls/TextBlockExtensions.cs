using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace TCC.UI.Controls;

public static class TextBlockExtensions
{
    public static IEnumerable<Inline> GetBindableInlines(TextBlock obj)
    {
        return (IEnumerable<Inline>)obj.GetValue(BindableInlinesProperty);
    }

    public static void SetBindableInlines(TextBlock obj, IEnumerable<Inline> value)
    {
        obj.SetValue(BindableInlinesProperty, value);
    }

    public static readonly DependencyProperty BindableInlinesProperty =
        DependencyProperty.RegisterAttached("BindableInlines", typeof(IEnumerable<Inline>), typeof(TextBlockExtensions), new PropertyMetadata(null, OnBindableInlinesChanged));

    private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock target)
        {
            return;
        }

        if (e.NewValue == null)
        {
            target.Inlines.Clear();
            return;
        }

        target.Inlines.Clear();
        target.Inlines.AddRange((System.Collections.IEnumerable)e.NewValue);
    }
}

public static class ToggleButtonExtensions
{
    public static CornerRadius GetCornerRadius(ToggleButton obj)
    {
        return (CornerRadius)obj.GetValue(CornerRadiusProperty);
    }

    public static void SetCornerRadius(ToggleButton obj, CornerRadius value)
    {
        obj.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached(nameof(CornerRadius), typeof(CornerRadius), typeof(ToggleButtonExtensions), new PropertyMetadata(new CornerRadius(0)));




    public static Brush GetCheckedBrush(ToggleButton obj)
    {
        return (Brush)obj.GetValue(CheckedBrushProperty);
    }

    public static void SetCheckedBrush(ToggleButton obj, Brush value)
    {
        obj.SetValue(CheckedBrushProperty, value);
    }

    public static readonly DependencyProperty CheckedBrushProperty =
        DependencyProperty.RegisterAttached("CheckedBrush", typeof(Brush), typeof(ToggleButtonExtensions), new PropertyMetadata(R.Brushes.DefensiveStanceBrush));




    public static Brush GetHighlightBrush(ToggleButton obj)
    {
        return (Brush)obj.GetValue(HighlightBrushProperty);
    }

    public static void SetHighlightBrush(ToggleButton obj, Brush value)
    {
        obj.SetValue(HighlightBrushProperty, value);
    }

    // Using a DependencyProperty as the backing store for HighlightBrush.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HighlightBrushProperty =
        DependencyProperty.RegisterAttached("HighlightBrush", typeof(Brush), typeof(ToggleButtonExtensions), new PropertyMetadata(Brushes.Transparent));


}