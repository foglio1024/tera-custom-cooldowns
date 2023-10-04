using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;

namespace TCC.UI.Windows;

public class TccWindow : Window
{
    static readonly List<TccWindow> _createdWindows = new();

    public event Action? Hidden;
    public event Action? Showed;

    readonly bool _canClose;
    //readonly DoubleAnimation _showAnim;
    readonly DoubleAnimation _hideAnim;

    public IntPtr Handle { get; private set; }


    protected TccWindow(bool canClose)
    {
        _createdWindows.Add(this);
        _canClose = canClose;
        Closing += OnClosing;
        Loaded += OnLoaded;
        //_showAnim = AnimationFactory.CreateDoubleAnimation(150, 1);
        _hideAnim = AnimationFactory.CreateDoubleAnimation(150, 0, completed: (_, _) =>
        {
            Hide();
            if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            Hidden?.Invoke();
        });
    }

    public virtual void HideWindow()
    {
        Dispatcher?.InvokeAsync(() =>
        {
            BeginAnimation(OpacityProperty, _hideAnim);
        });
    }
    public virtual void ShowWindow()
    {
        if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
        Dispatcher?.InvokeAsync(() =>
        {
            BeginAnimation(OpacityProperty, null);
            //((FrameworkElement)Content).BeginAnimation(OpacityProperty, null);
            //((FrameworkElement)Content).Opacity = 0;
            //_showAnim.From = 0;
            Show();
            Showed?.Invoke();
            RefreshTopmost();
            //((FrameworkElement)Content).BeginAnimation(OpacityProperty, _showAnim);
        });
    }

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
        Handle = new WindowInteropHelper(this).Handle;
    }
    protected virtual void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_canClose)
        {
            _createdWindows.Remove(this);
            return;
        }
        e.Cancel = true;
        HideWindow();
    }

    protected virtual void Drag(object sender, MouseButtonEventArgs e)
    {
        ((UIElement)Content).Opacity = .7;
        this.TryDragMove();
        ((UIElement)Content).Opacity = 1;
    }

    protected void RefreshTopmost()
    {
        if (FocusManager.PauseTopmost) return;

        Dispatcher?.InvokeAsync(() =>
        {
            Topmost = false;
            Topmost = true;
        });
    }

    public static bool Exists(Type type)
    {
        return _createdWindows.Any(w => w.GetType() == type);
    }
    public static bool Exists(IntPtr handle)
    {
        return _createdWindows.Any(w => w.Handle == handle && w.Handle != IntPtr.Zero);
    }

}