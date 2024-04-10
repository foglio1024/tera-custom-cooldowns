using System.Windows;
using TCC.Data;

namespace TCC.UI.Controls.Classes.Elements;

/// <summary>
/// Logica di interazione per SorcererElementsControl.xaml
/// </summary>
public partial class SorcererElementsControl
{
    public SorcererElementsControl()
    {
        InitializeComponent();
    }

    public FusionElements Elements
    {
        get => (FusionElements)GetValue(ElementsProperty);
        set => SetValue(ElementsProperty, value);
    }

    public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(nameof(Elements), typeof(FusionElements), typeof(SorcererElementsControl), new PropertyMetadata(FusionElements.None, HandleElementsChanged));

    public FusionElements Boosts
    {
        get => (FusionElements)GetValue(BoostsProperty);
        set => SetValue(BoostsProperty, value);
    }

    public static readonly DependencyProperty BoostsProperty = DependencyProperty.Register(nameof(Boosts), typeof(FusionElements), typeof(SorcererElementsControl), new PropertyMetadata(FusionElements.None, HandleBoostsChanged));

    public bool Fire
    {
        get => (bool)GetValue(FireProperty);
        private set => SetValue(FireProperty, value);
    }

    private static readonly DependencyPropertyKey FirePropertyKey = DependencyProperty.RegisterReadOnly(nameof(Fire), typeof(bool), typeof(SorcererElementsControl), new PropertyMetadata(false));
    public static readonly DependencyProperty FireProperty = FirePropertyKey.DependencyProperty;

    public bool Ice
    {
        get => (bool)GetValue(IceProperty);
        private set => SetValue(IceProperty, value);
    }

    private static readonly DependencyPropertyKey IcePropertyKey = DependencyProperty.RegisterReadOnly(nameof(Ice), typeof(bool), typeof(SorcererElementsControl), new PropertyMetadata(false));
    public static readonly DependencyProperty IceProperty = IcePropertyKey.DependencyProperty;

    public bool Arcane
    {
        get => (bool)GetValue(ArcaneProperty);
        private set => SetValue(ArcaneProperty, value);
    }

    private static readonly DependencyPropertyKey ArcanePropertyKey = DependencyProperty.RegisterReadOnly(nameof(Arcane), typeof(bool), typeof(SorcererElementsControl), new PropertyMetadata(false));
    public static readonly DependencyProperty ArcaneProperty = ArcanePropertyKey.DependencyProperty;

    public bool FireBoosted
    {
        get => (bool)GetValue(FireBoostedProperty);
        private set => SetValue(FireBoostedProperty, value);
    }

    private static readonly DependencyPropertyKey FireBoostedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(FireBoosted), typeof(bool), typeof(SorcererElementsControl), new PropertyMetadata(false));
    public static readonly DependencyProperty FireBoostedProperty = FireBoostedPropertyKey.DependencyProperty;

    public bool IceBoosted
    {
        get => (bool)GetValue(IceBoostedProperty);
        private set => SetValue(IceBoostedProperty, value);
    }

    private static readonly DependencyPropertyKey IceBoostedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IceBoosted), typeof(bool), typeof(SorcererElementsControl), new PropertyMetadata(false));
    public static readonly DependencyProperty IceBoostedProperty = IceBoostedPropertyKey.DependencyProperty;

    public bool ArcaneBoosted
    {
        get => (bool)GetValue(ArcaneBoostedProperty);
        private set => SetValue(ArcaneBoostedProperty, value);
    }

    private static readonly DependencyPropertyKey ArcaneBoostedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ArcaneBoosted), typeof(bool), typeof(SorcererElementsControl), new PropertyMetadata(false));
    public static readonly DependencyProperty ArcaneBoostedProperty = ArcaneBoostedPropertyKey.DependencyProperty;

    public double IndicatorsWidth
    {
        get => (double)GetValue(IndicatorsWidthProperty);
        set => SetValue(IndicatorsWidthProperty, value);
    }

    public static readonly DependencyProperty IndicatorsWidthProperty = DependencyProperty.Register(nameof(IndicatorsWidth), typeof(double), typeof(SorcererElementsControl));

    public double IndicatorsHeight
    {
        get => (double)GetValue(IndicatorsHeightProperty);
        set => SetValue(IndicatorsHeightProperty, value);
    }

    public static readonly DependencyProperty IndicatorsHeightProperty = DependencyProperty.Register(nameof(IndicatorsHeight), typeof(double), typeof(SorcererElementsControl));

    public Thickness FireBoostMargin
    {
        get => (Thickness)GetValue(FireBoostMarginProperty);
        set => SetValue(FireBoostMarginProperty, value);
    }

    public static readonly DependencyProperty FireBoostMarginProperty = DependencyProperty.Register(nameof(FireBoostMargin), typeof(Thickness), typeof(SorcererElementsControl));

    public Thickness IceBoostMargin
    {
        get => (Thickness)GetValue(IceBoostMarginProperty);
        set => SetValue(IceBoostMarginProperty, value);
    }

    public static readonly DependencyProperty IceBoostMarginProperty = DependencyProperty.Register(nameof(IceBoostMargin), typeof(Thickness), typeof(SorcererElementsControl));

    public double FireIceBoostWidth
    {
        get => (double)GetValue(FireIceBoostWidthProperty);
        set => SetValue(FireIceBoostWidthProperty, value);
    }

    public static readonly DependencyProperty FireIceBoostWidthProperty = DependencyProperty.Register(nameof(FireIceBoostWidth), typeof(double), typeof(SorcererElementsControl));

    public Thickness ArcaneBoostMargin
    {
        get => (Thickness)GetValue(ArcaneBoostMarginProperty);
        set => SetValue(ArcaneBoostMarginProperty, value);
    }

    public static readonly DependencyProperty ArcaneBoostMarginProperty = DependencyProperty.Register(nameof(ArcaneBoostMargin), typeof(Thickness), typeof(SorcererElementsControl));

    public double ArcaneBoostWidth
    {
        get => (double)GetValue(ArcaneBoostWidthProperty);
        set => SetValue(ArcaneBoostWidthProperty, value);
    }

    public static readonly DependencyProperty ArcaneBoostWidthProperty = DependencyProperty.Register(nameof(ArcaneBoostWidth), typeof(double), typeof(SorcererElementsControl));

    private static void HandleElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not SorcererElementsControl sec) return;

        sec.OnElementsChanged((FusionElements)e.NewValue);
    }

    private static void HandleBoostsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not SorcererElementsControl sec) return;

        sec.OnBoostsChanged((FusionElements)e.NewValue);
    }

    private void OnElementsChanged(FusionElements elements)
    {
        Fire = (elements & FusionElements.Flame) == FusionElements.Flame;
        Ice = (elements & FusionElements.Frost) == FusionElements.Frost;
        Arcane = (elements & FusionElements.Arcane) == FusionElements.Arcane;
    }

    private void OnBoostsChanged(FusionElements elements)
    {
        FireBoosted = (elements & FusionElements.Flame) == FusionElements.Flame;
        IceBoosted = (elements & FusionElements.Frost) == FusionElements.Frost;
        ArcaneBoosted = (elements & FusionElements.Arcane) == FusionElements.Arcane;
    }
}