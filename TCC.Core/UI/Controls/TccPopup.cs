using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TCC.UI.Controls;

public class TccPopup : Popup
{
    public double MouseLeaveTolerance
    {
        get => (double)GetValue(MouseLeaveToleranceProperty);
        set => SetValue(MouseLeaveToleranceProperty, value);
    }

    public static readonly DependencyProperty MouseLeaveToleranceProperty = DependencyProperty.Register(nameof(MouseLeaveTolerance),
        typeof(double),
        typeof(TccPopup),
        new PropertyMetadata(0D));

    public TccPopup()
    {
        WindowManager.VisibilityManager.VisibilityChanged += OnVisiblityChanged;
        FocusManager.ForegroundChanged += OnForegroundChanged;
    }


    protected override void OnOpened(EventArgs e)
    {
        FocusManager.PauseTopmost = true;
        base.OnOpened(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        FocusManager.PauseTopmost = false;
    }

    void OnForegroundChanged()
    {
        if (FocusManager.IsForeground) return;
        Dispatcher?.InvokeAsync(() => IsOpen = false);
    }

    void OnVisiblityChanged()
    {
        if (WindowManager.VisibilityManager.Visible) return;
        Dispatcher?.InvokeAsync(() => IsOpen = false);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        if (Child == null) return;
        var content = (FrameworkElement) Child;
        var pos = e.MouseDevice.GetPosition(content);
        if (pos.X > MouseLeaveTolerance && pos.X < content.ActualWidth - MouseLeaveTolerance
                                        && pos.Y > MouseLeaveTolerance && pos.Y < content.ActualHeight - MouseLeaveTolerance) return;
        IsOpen = false;
    }
}