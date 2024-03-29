﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TCC.Data;

namespace TCC.UI.Controls.Classes.Elements;

public class EdgeControlBase : UserControl
{
    public Color GlowColor
    {
        get => (Color)GetValue(GlowColorProperty);
        set => SetValue(GlowColorProperty, value);
    }
    public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register(nameof(GlowColor), typeof(Color), typeof(EdgeControlBase));

    public Brush FillBrush
    {
        get => (Brush)GetValue(FillBrushProperty);
        set => SetValue(FillBrushProperty, value);
    }
    public static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register(nameof(FillBrush), typeof(Brush), typeof(EdgeControlBase));

    public Counter? EdgeCounter
    {
        get => (Counter)GetValue(EdgeCounterProperty);
        set => SetValue(EdgeCounterProperty, value);
    }
    public static readonly DependencyProperty EdgeCounterProperty = DependencyProperty.Register(nameof(EdgeCounter), typeof(Counter), typeof(EdgeControlBase));

    public virtual List<FrameworkElement> EdgeElements { get; }

    public EdgeControlBase()
    {
        EdgeElements = new List<FrameworkElement>();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (EdgeCounter == null) return;
        EdgeCounter.PropertyChanged += OnEdgeChanged;
    }
    protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (EdgeCounter == null) return;
        EdgeCounter.PropertyChanged -= OnEdgeChanged;
    }

    protected virtual void OnEdgeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Counter.Val)) return;
        Dispatcher.Invoke(() =>
        {
            if (EdgeCounter == null) return;
            for (var i = 0; i < EdgeCounter.MaxValue; i++)
            {
                EdgeElements[i].Opacity = i < EdgeCounter.Val ? 1 : 0;
            }
        });
    }
}