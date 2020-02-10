using Nostrum;
using System.Windows;
using System.Windows.Input;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class LfgListWindow
    {
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
            ProxyInterface.Instance.Stub.RequestListings();

            base.ShowWindow();

            Dispatcher?.Invoke(() =>
            {
                var teraScreen = FocusManager.TeraScreen;
                var x = teraScreen.Bounds.X + teraScreen.Bounds.Size.Width / 2D;
                var y = teraScreen.Bounds.Y + teraScreen.Bounds.Size.Height / 2D;

                x -= this.Width / 2;
                y -= this.Height / 2;
                Left = x;
                Top = y;
            });

        }

        private void OnTbMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            FocusManager.UndoUnfocusable(Handle);
        }

        private void OnTbLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            FocusManager.MakeUnfocusable(Handle);
        }

        private void OnBgMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void LfgPopup_OnMouseLeave(object sender, MouseEventArgs e)
        {
            VM.IsPopupOpen = false;
        }
    }
}
