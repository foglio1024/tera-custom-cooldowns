using System;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.ViewModels.ClassManagers;

namespace TCC.UI.Controls.Classes;

public partial class ArcherLayout
{
    ArcherLayoutViewModel? _context;
    readonly DoubleAnimation _an;
    readonly DoubleAnimation _an2;

    public ArcherLayout()
    {
        _an = AnimationFactory.CreateDoubleAnimation(150, 42, 318, framerate: 20);
        _an2 = AnimationFactory.CreateDoubleAnimation(150, 0, framerate: 30);
        InitializeComponent();
    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        _context = (ArcherLayoutViewModel)DataContext;
        _context.Focus.EmpoweredBuffStarted += OnFocusXStarted;
        _context.Focus.BaseStacksChanged += OnStacksChanged;
    }


    void OnStacksChanged(int stacks)
    {
        Dispatcher?.InvokeAsync(() =>
        {
            _an2.To = stacks / 10D * 280 + 42;
            _an2.Duration = TimeSpan.FromMilliseconds(150);
            SecReArc.BeginAnimation(Arc.EndAngleProperty, _an2);
        });

    }

    void OnFocusXStarted(long duration)
    {
        Dispatcher?.Invoke(() =>
        {
            _an.Duration = TimeSpan.FromMilliseconds(duration);
            MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
        });
    }
}