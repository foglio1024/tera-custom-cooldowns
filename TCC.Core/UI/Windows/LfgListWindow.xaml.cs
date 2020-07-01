using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Nostrum;
using Nostrum.WinAPI;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.UI.Windows
{
    public partial class LfgListWindow
    {
        private bool _keepPopupOpen;

        private LfgListViewModel VM { get; }

        public ICommand HideWindowCommand { get; }

        public LfgListWindow(LfgListViewModel vm) : base(false)
        {
            InitializeComponent();
            DataContext = vm;
            VM = vm;
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
        }

        public override void ShowWindow()
        {
            StubInterface.Instance.StubClient.RequestListings();

            base.ShowWindow();

            Dispatcher?.Invoke(() =>
            {
                var teraScreen = FocusManager.TeraScreen;
                var x = teraScreen.Bounds.X + teraScreen.Bounds.Size.Width / 2D;
                var y = teraScreen.Bounds.Y + teraScreen.Bounds.Size.Height / 2D;

                x -= Width / 2;
                y -= Height / 2;
                Left = x;
                Top = y;
            });

        }

        private void OnTbMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _keepPopupOpen = true;
            FocusManager.UndoUnfocusable(Handle);
            var src = (HwndSource) PresentationSource.FromVisual(ActionsPopup.Child);
            if (src != null)
            {
                User32.SetForegroundWindow(src.Handle);
                FocusManager.UndoUnfocusable(src.Handle);
            }

            ((FrameworkElement) sender).Focus();
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
            VM.IsPopupOpen = false;
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
}
