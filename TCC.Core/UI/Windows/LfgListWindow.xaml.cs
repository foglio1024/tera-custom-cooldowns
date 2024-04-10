using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Nostrum.WinAPI;
using Nostrum.WPF;
using Nostrum.WPF.Extensions;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.UI.Windows;

public partial class LfgListWindow
{
    private bool _keepPopupOpen;

    private readonly LfgListViewModel _vm;

    public ICommand HideWindowCommand { get; }

    public LfgListWindow(LfgListViewModel vm) : base(false)
    {
        InitializeComponent();
        DataContext = vm;
        _vm = vm;
        WindowManager.VisibilityManager.VisibilityChanged += () =>
        {
            if (!WindowManager.VisibilityManager.Visible) return;
            RefreshTopmost();
        };
        FocusManager.FocusTick += RefreshTopmost;

        HideWindowCommand = new RelayCommand(_ => HideWindow());
    }


    protected override void OnLoaded(object sender, RoutedEventArgs e)
    {
        base.OnLoaded(sender, e);
        FocusManager.HideFromToolBar(Handle);
        FocusManager.MakeUnfocusable(Handle);

        var teraScreen = FocusManager.TeraScreen;
        var dpi = this.GetDpiScale();
        var x = teraScreen.Bounds.X + teraScreen.Bounds.Size.Width / (2D * dpi.DpiScaleX);
        var y = teraScreen.Bounds.Y + teraScreen.Bounds.Size.Height / (2D * dpi.DpiScaleY);

        x -= Width / 2;
        y -= Height / 2;
        Left = x;
        Top = y;
    }

    public override void ShowWindow()
    {
        StubInterface.Instance.StubClient.RequestListings(App.Settings.LfgWindowSettings.MinLevel, App.Settings.LfgWindowSettings.MaxLevel);

        base.ShowWindow();
    }

    private void OnTbMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _keepPopupOpen = true;
        FocusManager.UndoUnfocusable(Handle);
        var src = (HwndSource?)PresentationSource.FromVisual(ActionsPopup.Child);
        if (src != null)
        {
            User32.SetForegroundWindow(src.Handle);
            FocusManager.UndoUnfocusable(src.Handle);
        }

        ((FrameworkElement)sender).Focus();
        Keyboard.Focus((FrameworkElement)sender);
    }

    private void OnTbLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        FocusManager.MakeUnfocusable(Handle);
        _keepPopupOpen = false;
    }

    private void OnBgMouseLeftButtonDown(object sender, MouseButtonEventArgs? e)
    {
        Keyboard.ClearFocus();
        _keepPopupOpen = false;
    }

    private void LfgPopup_OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (_keepPopupOpen) return;
        _vm.IsPopupOpen = false;
    }

    private void OnTbKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            OnBgMouseLeftButtonDown(sender, null);
        }
    }

    private void OnTbMouseLeave(object sender, MouseEventArgs e)
    {
        _keepPopupOpen = false;
    }
}