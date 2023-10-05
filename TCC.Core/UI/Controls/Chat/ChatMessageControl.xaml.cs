using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Factories;
using TCC.Data.Chat;

namespace TCC.UI.Controls.Chat;

public partial class ChatMessageControl
{
    readonly DoubleAnimation _anim;
    ChatMessage? _dc;
    public ChatMessageControl()
    {
        InitializeComponent();
        _anim = AnimationFactory.CreateDoubleAnimation(250, 1, 0, true, AnimCompleted, 30);
        Timeline.SetDesiredFrameRate(_anim, 30);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_dc == null) return;
        _dc.IsVisible = true;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if(_dc == null) return;
        _dc.IsVisible = false;
        _dc = null;
    }

    void AnimCompleted(object? sender, EventArgs e)
    {
        SetAnimated();
    }

    void SetAnimated()
    {
        if (_dc == null) return;
        _dc.Animate = false;
    }

    void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if(DataContext is not ChatMessage message) return;
        _dc = message;
        var tg = (TransformGroup) LayoutTransform;
        var sc = tg.Children[0];
        if (!_dc.Animate)
        {
            var sc2 = new ScaleTransform(1,1);
            tg.Children[0] = sc2;
            return;
        }
        sc.BeginAnimation(ScaleTransform.ScaleYProperty, _anim);
    }
}