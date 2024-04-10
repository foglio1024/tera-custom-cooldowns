using Nostrum.WPF.Extensions;
using System.Windows;
using System.Windows.Input;

namespace TCC.UI.Controls.Chat;

public partial class TranslationIndicator 
{
    public TranslationIndicator()
    {
        InitializeComponent();
    }

    private void OnTranslationIndicatorMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is not FrameworkElement uiel) return;
        var popup = uiel.Parent.FindVisualChild<TccPopup>();
        if (popup != null) { popup.IsOpen = true; }
    }

    private void OnTranslationIndicatorMouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is not FrameworkElement uiel) return;
        var popup = uiel.Parent.FindVisualChild<TccPopup>();
        if (popup != null) { popup.IsOpen = false; }
    }
}