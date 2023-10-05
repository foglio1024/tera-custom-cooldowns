using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.UI.Controls.Dashboard;

/// <summary>
/// Logica di interazione per RectangleBarGauge.xaml
/// </summary>
public partial class RectangleBarGauge
{
    public RectangleBarGauge()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        var a = new DoubleAnimation(0, Factor, TimeSpan.FromMilliseconds(App.Random.Next(350, 650))) { EasingFunction = new QuadraticEase() };
        GaugeValue.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, a);
    }

    public new double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }
    public new static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(nameof(Width), typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(double.NaN));

    public new double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }
    public new  static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register(nameof(Height), typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(double.NaN));

    public Brush Color
    {
        get => (Brush)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(RectangleBarGauge));

    public double Factor
    {
        get => (double)GetValue(FactorProperty);
        set => SetValue(FactorProperty, value);
    }
    public static readonly DependencyProperty FactorProperty =
        DependencyProperty.Register(nameof(Factor), typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(0d, OnFactorChanged));

    static void OnFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        var a = new DoubleAnimation((double)e.NewValue, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() };
        (d as RectangleBarGauge)?.GaugeValue.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, a);
    }

    public double Skew
    {
        get => (double)GetValue(SkewProperty);
        set => SetValue(SkewProperty, value);
    }
    public static readonly DependencyProperty SkewProperty =
        DependencyProperty.Register(nameof(Skew), typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(0d));



}