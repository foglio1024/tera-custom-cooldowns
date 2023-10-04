using System;
using System.Windows.Media.Animation;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.ViewModels.ClassManagers;

namespace TCC.UI.Controls.Classes;

// TODO: refactor dis
public partial class LancerLayout
{
    LancerLayoutVM? _dc;
    readonly DoubleAnimation _lineHeldDurationAn;
    readonly DoubleAnimation _lineHeldStacksAn;

    public LancerLayout()
    {
        _lineHeldDurationAn = AnimationFactory.CreateDoubleAnimation(150, 42, 318, framerate: 20);
        _lineHeldStacksAn = AnimationFactory.CreateDoubleAnimation(150, 0, framerate: 30);

        InitializeComponent();

        Loaded += OnLoaded;
    }

    void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        _dc = (LancerLayoutVM)DataContext;

        if (_dc == null) return;
        _dc.LH.BaseStacksChanged += OnStacksChanged;
        _dc.LH.BaseBuffStarted += OnLineHeldRefreshed;
        _dc.LH.BaseBuffRefreshed += OnLineHeldRefreshed;
        _dc.LH.BuffEnded += OnLineHeldEnded;
    }

    void OnLineHeldEnded()
    {
        Dispatcher?.Invoke(() =>
        {
            _lineHeldStacksAn.To = 42;
            SecReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldStacksAn);
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldStacksAn);
        });

    }

    void OnStacksChanged(int stacks)
    {
        Dispatcher?.Invoke(() =>
        {
            _lineHeldStacksAn.To = stacks / 5D * 280 + 42;
            SecReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldStacksAn);
        });
    }

    void OnLineHeldRefreshed(long duration)
    {
        Dispatcher?.Invoke(() =>
        {
            _lineHeldDurationAn.Duration = TimeSpan.FromMilliseconds(duration);
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _lineHeldDurationAn);
        });
    }
}