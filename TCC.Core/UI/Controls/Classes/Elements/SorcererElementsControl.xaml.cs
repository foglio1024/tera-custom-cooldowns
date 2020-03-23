using System.Windows;

namespace TCC.UI.Controls.Classes.Elements
{
    /// <summary>
    /// Logica di interazione per SorcererElementsControl.xaml
    /// </summary>
    public partial class SorcererElementsControl 
    {
        public SorcererElementsControl()
        {
            InitializeComponent();
        }

        public bool Fire
        {
            get => (bool)GetValue(FireProperty);
            set => SetValue(FireProperty, value);
        }
        public static readonly DependencyProperty FireProperty = DependencyProperty.Register("Fire", typeof(bool), typeof(SorcererElementsControl));

        public bool Ice
        {
            get => (bool)GetValue(IceProperty);
            set => SetValue(IceProperty, value);
        }
        public static readonly DependencyProperty IceProperty = DependencyProperty.Register("Ice", typeof(bool), typeof(SorcererElementsControl));

        public bool Arcane
        {
            get => (bool)GetValue(ArcaneProperty);
            set => SetValue(ArcaneProperty, value);
        }
        public static readonly DependencyProperty ArcaneProperty = DependencyProperty.Register("Arcane", typeof(bool), typeof(SorcererElementsControl));

        public bool FireBoosted
        {
            get => (bool)GetValue(FireBoostedProperty);
            set => SetValue(FireBoostedProperty, value);
        }
        public static readonly DependencyProperty FireBoostedProperty = DependencyProperty.Register("FireBoosted", typeof(bool), typeof(SorcererElementsControl));

        public bool IceBoosted
        {
            get => (bool)GetValue(IceBoostedProperty);
            set => SetValue(IceBoostedProperty, value);
        }
        public static readonly DependencyProperty IceBoostedProperty = DependencyProperty.Register("IceBoosted", typeof(bool), typeof(SorcererElementsControl));

        public bool ArcaneBoosted
        {
            get => (bool)GetValue(ArcaneBoostedProperty);
            set => SetValue(ArcaneBoostedProperty, value);
        }
        public static readonly DependencyProperty ArcaneBoostedProperty = DependencyProperty.Register("ArcaneBoosted", typeof(bool), typeof(SorcererElementsControl));

        public double IndicatorsWidth
        {
            get => (double)GetValue(IndicatorsWidthProperty);
            set => SetValue(IndicatorsWidthProperty, value);
        }
        public static readonly DependencyProperty IndicatorsWidthProperty = DependencyProperty.Register("IndicatorsWidth", typeof(double), typeof(SorcererElementsControl));

        public double IndicatorsHeight
        {
            get => (double)GetValue(IndicatorsHeightProperty);
            set => SetValue(IndicatorsHeightProperty, value);
        }
        public static readonly DependencyProperty IndicatorsHeightProperty = DependencyProperty.Register("IndicatorsHeight", typeof(double), typeof(SorcererElementsControl));

        public Thickness FireBoostMargin
        {
            get => (Thickness)GetValue(FireBoostMarginProperty);
            set => SetValue(FireBoostMarginProperty, value);
        }
        public static readonly DependencyProperty FireBoostMarginProperty = DependencyProperty.Register("FireBoostMargin", typeof(Thickness), typeof(SorcererElementsControl));

        public Thickness IceBoostMargin
        {
            get => (Thickness)GetValue(IceBoostMarginProperty);
            set => SetValue(IceBoostMarginProperty, value);
        }
        public static readonly DependencyProperty IceBoostMarginProperty = DependencyProperty.Register("IceBoostMargin", typeof(Thickness), typeof(SorcererElementsControl));

        public double FireIceBoostWidth
        {
            get => (double)GetValue(FireIceBoostWidthProperty);
            set => SetValue(FireIceBoostWidthProperty, value);
        }
        public static readonly DependencyProperty FireIceBoostWidthProperty = DependencyProperty.Register("FireIceBoostWidth", typeof(double), typeof(SorcererElementsControl));

        public Thickness ArcaneBoostMargin
        {
            get => (Thickness)GetValue(ArcaneBoostMarginProperty);
            set => SetValue(ArcaneBoostMarginProperty, value);
        }
        public static readonly DependencyProperty ArcaneBoostMarginProperty = DependencyProperty.Register("ArcaneBoostMargin", typeof(Thickness), typeof(SorcererElementsControl));

        public double ArcaneBoostWidth
        {
            get => (double)GetValue(ArcaneBoostWidthProperty);
            set => SetValue(ArcaneBoostWidthProperty, value);
        }
        public static readonly DependencyProperty ArcaneBoostWidthProperty = DependencyProperty.Register("ArcaneBoostWidth", typeof(double), typeof(SorcererElementsControl));


    }
}
