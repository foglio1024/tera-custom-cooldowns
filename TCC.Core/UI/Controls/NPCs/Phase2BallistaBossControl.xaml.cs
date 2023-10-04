using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TCC.UI.Controls.NPCs;

/// <summary>
/// Interaction logic for Phase2BallistaBossControl.xaml
/// </summary>
public partial class Phase2BallistaBossControl
{
    public Phase2BallistaBossControl()
    {
        InitializeComponent();
    }

    void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        EnrageEll.RenderTransform.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(240, 0, TimeSpan.FromSeconds(240)));
    }
}