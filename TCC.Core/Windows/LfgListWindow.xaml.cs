using Nostrum;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class LfgListWindow
    {
        private readonly ColorAnimation _colAn = AnimationFactory.CreateColorAnimation(200);
        private readonly DoubleAnimation _expandAn = AnimationFactory.CreateDoubleAnimation(150, 1, easing: true);
        private readonly DoubleAnimation _shrinkAn = AnimationFactory.CreateDoubleAnimation(150, 0, easing: true);
        private readonly DoubleAnimation _publicizeCdAn = AnimationFactory.CreateDoubleAnimation(4000, 0, 1);
        private readonly ThicknessAnimation _margin1An = AnimationFactory.CreateThicknessAnimation(150, new Thickness(4, 0, 4, 0), easing: true);
        private readonly ThicknessAnimation _margin2An = AnimationFactory.CreateThicknessAnimation(150, new Thickness(4), easing: true);
        private LfgListViewModel VM { get; }

        public ICommand HideWindowCommand { get; }

        public LfgListWindow(LfgListViewModel vm) : base(false)
        {
            InitializeComponent();
            DataContext = vm;
            VM = vm;
            VM.Publicized += OnPublicized;
            VM.MyLfgStateChanged += OnMyLfgStateChanged;
            VM.CreatingStateChanged += OnCreatingStateChanged;
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

        private void OnCreatingStateChanged()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                _colAn.To = VM.Creating
                    ? string.IsNullOrEmpty(VM.NewMessage)
                        ? R.Colors.HpColor
                        : R.Colors.GreenColor
                    : R.Colors.ChatMegaphoneColorDark;
                var newBg = new SolidColorBrush(((SolidColorBrush) CreateMessageBtn.Background).Color);
                CreateMessageBtn.Background = newBg;
                CreateMessageBtn.Background.BeginAnimation(SolidColorBrush.ColorProperty, _colAn);
                if (VM.Creating)
                {
                    NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AnimationFactory.CreateDoubleAnimation(150, 1, easing: true));
                    FocusManager.UndoUnfocusable(Handle);
                    Activate();
                    NewMessageTextBox.Focus();
                }
                else
                {
                    NewMessageGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AnimationFactory.CreateDoubleAnimation(150, 0, easing: true));
                    FocusManager.MakeUnfocusable(Handle);

                }
            });
        }

        private void OnMyLfgStateChanged()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                LfgMgmtBtn.BeginAnimation(OpacityProperty, VM.AmIinLfg ? _expandAn : _shrinkAn);
                CreateMessageBtn.BeginAnimation(OpacityProperty, VM.AmIinLfg ? _shrinkAn : _expandAn);
            });
        }


        private void OnPublicized(int cd)
        {
            _publicizeCdAn.Duration = TimeSpan.FromMilliseconds(cd * 1000);
            PublicizeBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _publicizeCdAn);
        }


        public override void ShowWindow()
        {
            ProxyInterface.Instance.Stub.RequestListings();

            var teraScreen = FocusManager.TeraScreen;
            var x = teraScreen.Bounds.X + teraScreen.Bounds.Size.Width / 2D;
            var y = teraScreen.Bounds.Y + teraScreen.Bounds.Size.Height / 2D;

            x -= this.ActualWidth / 2;
            y -= this.ActualHeight/ 2;

            Left = x;
            Top = y;

            base.ShowWindow();
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
    }
}
