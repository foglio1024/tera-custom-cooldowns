using System.Windows;

namespace TCC.UI.Controls;

/// <summary>
/// Logica di interazione per WindowButtons.xaml
/// </summary>
public partial class WindowButtons
{
    public WindowButtons()
    {
        InitializeComponent();
    }

    public string WindowName
    {
        get => (string)GetValue(WindowNameProperty);
        set => SetValue(WindowNameProperty, value);
    }
    public static readonly DependencyProperty WindowNameProperty = DependencyProperty.Register("WindowName", typeof(string), typeof(WindowButtons));

    public Visibility AutoDimButtonVisiblity
    {
        get => (Visibility)GetValue(AutoDimButtonVisiblityProperty);
        set => SetValue(AutoDimButtonVisiblityProperty, value);
    }
    public static readonly DependencyProperty AutoDimButtonVisiblityProperty = DependencyProperty.Register("AutoDimButtonVisiblity", typeof(Visibility), typeof(WindowButtons));

    public Visibility HideButtonVisibility
    {
        get => (Visibility)GetValue(HideButtonVisibilityProperty);
        set => SetValue(HideButtonVisibilityProperty, value);
    }
    public static readonly DependencyProperty HideButtonVisibilityProperty = DependencyProperty.Register("HideButtonVisibility", typeof(Visibility), typeof(WindowButtons));

    public Visibility CloseButtonVisibility
    {
        get => (Visibility)GetValue(CloseButtonVisibilityProperty);
        set => SetValue(CloseButtonVisibilityProperty, value);
    }
    public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(WindowButtons));
}