using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.ViewModels.ClassManagers;

namespace TCC.UI.Controls.Classes;

public partial class NinjaLayout
{
    private NinjaLayoutViewModel? _dc;
    private readonly DoubleAnimation _an;

    public NinjaLayout()
    {
        _an = AnimationFactory.CreateDoubleAnimation(150, 0 * 359.99);
        InitializeComponent();
    }

    private void NinjaLayout_OnLoaded(object sender, RoutedEventArgs e)
    {
        _dc = (NinjaLayoutViewModel)DataContext;
        _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;
    }

    private void ST_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_dc == null) return;
        if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
        _an.To = _dc.StaminaTracker.Factor * (359.99 - 80) + 40;
        MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
    }
}