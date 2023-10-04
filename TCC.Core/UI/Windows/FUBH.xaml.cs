using System.Windows.Input;
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

    void FUBH_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.TryDragMove();
    }

    void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Hide();
        App.Settings.Save();
    }

    void Hyperlink_OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        Utils.Utilities.OpenUrl(e.Uri.AbsoluteUri);
    }
}

public class FUBHVM : ThreadSafeObservableObject
{
    public string CloseMessage => $"Click to {(!_dontshowagain ? "temporarily " : "")}close this window";
    bool _dontshowagain;

    public bool DontShowAgain
    {
        get => _dontshowagain;
        set
        {
            if (_dontshowagain == value) return;
            _dontshowagain = value;
            N();
            N(nameof(CloseMessage));
            App.Settings.DontShowFUBH = value;
        }
    }
}