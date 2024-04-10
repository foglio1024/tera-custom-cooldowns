using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Factories;

namespace TCC.UI.Controls.Dashboard;

public partial class DungeonInfoControl
{
    private readonly DoubleAnimation _bubbleScale;
    private readonly DoubleAnimation _fadeIn;

    public DungeonInfoControl()
    {
        _bubbleScale = AnimationFactory.CreateDoubleAnimation(1000, from: .1, to: .9);
        _bubbleScale.EasingFunction = new ElasticEase();
        _fadeIn = AnimationFactory.CreateDoubleAnimation(200, from: 0, to: 1);
        InitializeComponent();
    }

    public void AnimateIn()
    {
        Dispatcher?.Invoke(() =>
        {
            EntriesBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _bubbleScale);
            EntriesBubble.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _bubbleScale);
            EntriesBubble.Child.BeginAnimation(OpacityProperty, _fadeIn);
        });
    }
}