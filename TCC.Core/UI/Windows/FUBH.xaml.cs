using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Navigation;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.ThreadSafe;

namespace TCC.UI.Windows;

public partial class FUBH
{
    public FUBH()
    {
        InitializeComponent();
        DataContext = new FUBHVM();
        Closing += FUBH_Closing;
    }

    private void FUBH_Closing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.TryDragMove();
    }

    private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Hide();
        App.Settings.Save();
    }

    private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Utils.Utilities.OpenUrl(e.Uri.AbsoluteUri);
    }
}

public class FUBHVM : ThreadSafeObservableObject
{
    public string CloseMessage => $"Click to {(!_dontshowagain ? "temporarily " : "")}close this window";
    private bool _dontshowagain;

    public bool DontShowAgain
    {
        get => _dontshowagain;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _dontshowagain)) return;
            InvokePropertyChanged(nameof(CloseMessage));
            App.Settings.DontShowFUBH = value;
        }
    }
}