﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Nostrum.WPF.Factories;
using TCC.ViewModels;

namespace TCC.UI.Windows;
public partial class LootDistributionWindow
{
    readonly LootDistributionViewModel _vm;
    public LootDistributionWindow(LootDistributionViewModel vm) : base(false)
    {
        _vm = vm;
        DataContext = vm;
        Game.LootDistributionWindowShowRequest += OnShowRequest;

        WindowManager.VisibilityManager.VisibilityChanged += () =>
        {
            if (!WindowManager.VisibilityManager.Visible) return;
            RefreshTopmost();
        };
        FocusManager.FocusTick += RefreshTopmost;
        WindowManager.VisibilityManager.VisibilityChanged += OnVisibilityChanged;

        SizeChanged += OnSizeChanged;
        InitializeComponent();

    }

    void OnVisibilityChanged()
    {
        Opacity = WindowManager.VisibilityManager.Visible || _vm.Settings!.ShowAlways ? 1 : 0;
    }

    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        base.OnLoaded(sender, e);
        FocusManager.HideFromToolBar(Handle);
        FocusManager.MakeUnfocusable(Handle);
    }

    void OnShowRequest()
    {
        if (_vm.Settings?.Enabled != true) return;
        if (IsVisible) return;
        ShowWindow();
        Dispatcher.Invoke(() =>
        {
            Left = _vm.Settings.X * WindowManager.ScreenSize.Width;
            Top = _vm.Settings.Y * WindowManager.ScreenSize.Height;
            if (_vm.Settings.IgnoreSize) return;

            if (_vm.Settings.H != 0) Height = _vm.Settings.H;
            if (_vm.Settings.W != 0) Width = _vm.Settings.W;
        });
    }

    void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        Hide();
    }

    protected override void Drag(object sender, MouseButtonEventArgs e)
    {
        ResizeMode = ResizeMode.NoResize;
        base.Drag(sender, e);
        ResizeMode = ResizeMode.CanResize;
        SizeToContent = SizeToContent.Width;

        if (_vm.Settings == null) return;

        _vm.Settings.X = Left / WindowManager.ScreenSize.Width;
        _vm.Settings.Y = Top / WindowManager.ScreenSize.Height;

        App.Settings.Save();
    }

    void OnShowListButtonClick(object sender, RoutedEventArgs e)
    {
        SizeToContent = SizeToContent.Width;
    }

    void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_vm.Settings == null) return;
        if (_vm.Settings.IgnoreSize) return;
        if (_vm.Settings.W == ActualWidth && _vm.Settings.H == ActualHeight) return;
        _vm.Settings.W = ActualWidth;
        _vm.Settings.H = ActualHeight;
        if (!App.Loading) App.Settings.Save();
    }

    void OnBarLoaded(object sender, RoutedEventArgs e)
    {
        var bar = (FrameworkElement)sender;
        var scale = new ScaleTransform(1, 1);
        bar.LayoutTransform = scale;
        scale.BeginAnimation(ScaleTransform.ScaleXProperty, AnimationFactory.CreateDoubleAnimation(60 * 1000, 0, 1));
    }
}