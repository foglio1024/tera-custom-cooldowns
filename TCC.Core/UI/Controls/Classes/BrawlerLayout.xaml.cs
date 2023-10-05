using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.ViewModels.ClassManagers;

namespace TCC.UI.Controls.Classes;

public partial class BrawlerLayout
{
    BrawlerLayoutViewModel? _dc;
    readonly DoubleAnimation _an;

    public BrawlerLayout()
    {
        _an = AnimationFactory.CreateDoubleAnimation(150, to: 400);
        InitializeComponent();
    }

    void BrawlerLayout_OnLoaded(object sender, RoutedEventArgs e)
    {
        _dc = (BrawlerLayoutViewModel)DataContext;
        _dc.StaminaTracker.PropertyChanged += ST_PropertyChanged;
    }

    void ST_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(_dc.StaminaTracker.Factor)) return;
        if (_dc == null) return;
        _an.To = _dc.StaminaTracker.Factor * (359.99 - 2 * MainReArc.StartAngle) + MainReArc.StartAngle;
        MainReArc.BeginAnimation(Arc.EndAngleProperty,_an);
    }
}