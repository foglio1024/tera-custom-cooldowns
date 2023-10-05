using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Factories;

namespace TCC.UI.Controls.Dashboard;

public partial class EventControl
{
    readonly DoubleAnimation _scaleUp;
    readonly DoubleAnimation _scaleDown;

    public EventControl()
    {
        _scaleUp = AnimationFactory.CreateDoubleAnimation(800, 1.01);
        _scaleUp.EasingFunction = new ElasticEase();
        _scaleDown = AnimationFactory.CreateDoubleAnimation(150, 1, easing: true);

        InitializeComponent();
    }

    void UserControl_MouseEnter(object sender, MouseEventArgs e)
    {
        Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _scaleUp);
        Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _scaleUp);
    }

    void UserControl_MouseLeave(object sender, MouseEventArgs e)
    {
        Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _scaleDown);
        Border.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _scaleDown);
    }
}