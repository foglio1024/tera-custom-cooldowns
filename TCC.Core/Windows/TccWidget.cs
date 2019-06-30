using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Controls;
using FoglioUtils.Extensions;
using TCC.Controls;
using TCC.Data;
using TCC.Settings;

namespace TCC.Windows
{
    public class TccWidget : Window
    {
        private readonly DoubleAnimation _opacityAnimation = new DoubleAnimation { Duration = TimeSpan.FromMilliseconds(100) };
        private readonly DoubleAnimation _hideButtons = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000));
        private readonly DoubleAnimation _showButtons = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));
        private DispatcherTimer _buttonsTimer;

        protected WindowButtons ButtonsRef;
        protected UIElement MainContent;

        public WindowSettings WindowSettings { get; private set; }

        public IntPtr Handle { get; private set; }
        public bool CanMove { get; set; } = true;

        protected void Init(WindowSettings settings)
        {
            WindowSettings = settings;
            MainContent.Opacity = 0;
            Topmost = true;
            Left = WindowSettings.X * WindowManager.ScreenSize.Width;
            Top = WindowSettings.Y * WindowManager.ScreenSize.Height;
            if (!WindowSettings.IgnoreSize)
            {
                if (WindowSettings.H != 0) Height = WindowSettings.H;
                if (WindowSettings.W != 0) Width = WindowSettings.W;
            }
            CheckBounds();

            WindowSettings.EnabledChanged += OnEnabledChanged;
            WindowSettings.ClickThruModeChanged += OnClickThruModeChanged;
            WindowSettings.VisibilityChanged += OnWindowVisibilityChanged;
            WindowSettings.ResetToCenter += ResetToCenter;

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;

            WindowManager.ForegroundManager.VisibilityChanged += OnVisibilityChanged;
            WindowManager.ForegroundManager.DimChanged += OnDimChanged;
            WindowManager.ForegroundManager.ClickThruChanged += OnClickThruModeChanged;
            FocusManager.FocusTick += OnFocusTick;

            OnClickThruModeChanged();
            OnVisibilityChanged();
            OnWindowVisibilityChanged();

            FocusManager.MakeUnfocusable(Handle);

            if (ButtonsRef == null)
            {
                if (CanMove) MouseLeftButtonDown += Drag;
                return;
            }

            _buttonsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _buttonsTimer.Tick += OnButtonsTimerTick;

            MouseEnter += (_, __) =>
            {
                if (!App.Settings.HideHandles) ButtonsRef.BeginAnimation(OpacityProperty, _showButtons);
            };
            MouseLeave += (_, __) => _buttonsTimer.Start();
            if (CanMove) ButtonsRef.MouseLeftButtonDown += Drag;
        }

        public void ReloadPosition()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Left = WindowSettings.X * WindowManager.ScreenSize.Width;
                Top = WindowSettings.Y * WindowManager.ScreenSize.Height;

                CheckBounds();

                if (ButtonsRef != null)
                {
                    switch (WindowSettings.ButtonsPosition)
                    {
                        case ButtonsPosition.Above:
                            Grid.SetRow(ButtonsRef, 0);
                            break;
                        case ButtonsPosition.Below:
                            Grid.SetRow(ButtonsRef, 2);
                            break;
                    }
                    UpdateButtons();
                }
            }));
        }
        public void ResetToCenter()
        {
            Dispatcher.Invoke(() =>
            {
                Left = Screen.PrimaryScreen.Bounds.X + Screen.PrimaryScreen.Bounds.Width / 2 - ActualWidth / 2;
                Top = Screen.PrimaryScreen.Bounds.Y + Screen.PrimaryScreen.Bounds.Height / 2 - ActualHeight / 2;
                WindowSettings.X = Left / WindowManager.ScreenSize.Width;
                WindowSettings.Y = Top / WindowManager.ScreenSize.Height;
            });
        }

        private void OnFocusTick()
        {
            if (WindowManager.ForegroundManager.Visible) RefreshTopmost();
        }

        private void OnWindowVisibilityChanged()
        {
            SetVisibility(WindowSettings.Visible);
        }

        private void OnButtonsTimerTick(object sender, EventArgs e)
        {
            _buttonsTimer.Stop();
            if (IsMouseOver) return;
            ButtonsRef.BeginAnimation(OpacityProperty, _hideButtons);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!WindowSettings.AllowOffScreen) CheckBounds();
            if (WindowSettings.IgnoreSize) return;
            if (WindowSettings.W != ActualWidth ||
                WindowSettings.H != ActualHeight)
            {
                WindowSettings.W = ActualWidth;
                WindowSettings.H = ActualHeight;
                if (!App.Loading) App.Settings.Save();
            }
        }

        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            Handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(Handle);
            FocusManager.HideFromToolBar(Handle);
            if (!WindowSettings.Enabled) Hide();
        }
        private void OnDimChanged()
        {
            if (!WindowManager.ForegroundManager.Visible) return;

            if (!WindowSettings.AutoDim)
                AnimateContentOpacity(1);
            else
            {
                if (WindowSettings.UndimOnFlyingGuardian) AnimateContentOpacity(WindowManager.ForegroundManager.Dim ? WindowSettings.DimOpacity : 1);
                else if (FlyingGuardianDataProvider.IsInProgress) AnimateContentOpacity(WindowSettings.DimOpacity);
                else AnimateContentOpacity(WindowManager.ForegroundManager.Dim ? WindowSettings.DimOpacity : 1);
            }

            OnClickThruModeChanged();
        }
        private void OnVisibilityChanged()
        {
            if (WindowManager.ForegroundManager.Visible)
            {
                if (WindowManager.ForegroundManager.Dim && WindowSettings.AutoDim)
                {
                    AnimateContentOpacity(WindowSettings.DimOpacity);
                }
                else
                {
                    AnimateContentOpacity(1);
                }
                RefreshTopmost();
            }
            else
            {
                if (WindowSettings.ShowAlways) return;
                AnimateContentOpacity(0);
            }
        }
        private void OnClickThruModeChanged()
        {
            switch (WindowSettings.ClickThruMode)
            {
                case ClickThruMode.Never:
                    FocusManager.UndoClickThru(Handle);
                    break;
                case ClickThruMode.Always:
                    FocusManager.MakeClickThru(Handle);
                    break;
                case ClickThruMode.WhenDim:
                    if (WindowManager.ForegroundManager.Dim) FocusManager.MakeClickThru(Handle);
                    else FocusManager.UndoClickThru(Handle);
                    break;
                case ClickThruMode.WhenUndim:
                    if (WindowManager.ForegroundManager.Dim) FocusManager.UndoClickThru(Handle);
                    else FocusManager.MakeClickThru(Handle);
                    break;
                case ClickThruMode.GameDriven:
                    if (Session.InGameUiOn) FocusManager.UndoClickThru(Handle);
                    else FocusManager.MakeClickThru(Handle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected void OnEnabledChanged()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (WindowSettings.Enabled) Show();
                    else Hide();
                });
            }
            catch { }
        }
        private void AnimateContentOpacity(double opacity)
        {
            if (MainContent == null) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _opacityAnimation.To = opacity;
                MainContent.BeginAnimation(OpacityProperty, _opacityAnimation);
            })
            , DispatcherPriority.DataBind);
        }
        private void RefreshTopmost()
        {
            if (FocusManager.PauseTopmost) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Topmost = false; Topmost = true;
            }), DispatcherPriority.DataBind);
        }
        private void SetVisibility(bool v)
        {
            if (!Dispatcher.Thread.IsAlive) return;
            Dispatcher.Invoke(() =>
            {
                Visibility = !v ? Visibility.Visible : Visibility.Collapsed; // meh ok
                Visibility = v ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void CheckBounds()
        {
            if (WindowSettings == null) return;
            if (WindowSettings.AllowOffScreen) return;
            if (Left + ActualWidth > WindowManager.ScreenSize.Width)
            {
                Left = WindowManager.ScreenSize.Width - ActualWidth;
            }
            if (Top + ActualHeight > WindowManager.ScreenSize.Height)
            {
                Top = WindowManager.ScreenSize.Height - ActualHeight;
            }
            CheckIndividualScreensBounds();

            WindowSettings.X = Left / WindowManager.ScreenSize.Width;
            WindowSettings.Y = Top / WindowManager.ScreenSize.Height;
        }

        private void CheckIndividualScreensBounds()
        {
            if (IsWindowFullyVisible()) return;
            var nearestScreen = FindNearestScreen();

            if (Top + ActualHeight > nearestScreen.Bounds.Y + nearestScreen.Bounds.Height) Top = nearestScreen.Bounds.Y + nearestScreen.Bounds.Height - ActualHeight;
            else if (Top < nearestScreen.Bounds.Y) Top = nearestScreen.Bounds.Y;
            if (Left + ActualWidth > nearestScreen.Bounds.X + nearestScreen.Bounds.Width) Left = nearestScreen.Bounds.X + nearestScreen.Bounds.Width - ActualWidth;
            else if (Left < nearestScreen.Bounds.X) Left = nearestScreen.Bounds.X;
        }

        private Screen FindNearestScreen()
        {
            if (ScreenFromWindowCenter() != null) return ScreenFromWindowCenter();
            var distances = new List<Vector>();
            foreach (var screen in Screen.AllScreens)
            {
                var screenCenter = new Point(screen.Bounds.X + screen.Bounds.Size.Width / 2,
                                             screen.Bounds.Y + screen.Bounds.Size.Height / 2);

                var dist = screenCenter - WindowCenter;
                distances.Add(dist);
            }

            var min = new Vector(double.MaxValue, double.MaxValue);
            foreach (var distance in distances)
            {
                if (distance.Length < min.Length) min = distance;
            }
            var index = distances.IndexOf(min);
            return Screen.AllScreens[index != -1 ? index : 0];
        }

        private Point WindowCenter => new Point(Left + ActualWidth / 2, Top + ActualHeight / 2);

        private bool IsWindowFullyVisible()
        {
            var tl = false; var tr = false; var bl = false; var br = false;
            foreach (var screen in Screen.AllScreens)
            {
                if (IsTopLeftCornerInScreen(screen)) tl = true;
                if (IsTopRightCornerInScreen(screen)) tr = true;
                if (IsBottomLeftCornerInScreen(screen)) bl = true;
                if (IsBottomRightCornerInScreen(screen)) br = true;
            }

            return tl && tr && bl && br;

        }
        private bool IsTopLeftCornerInScreen(Screen screen)
        {
            return screen.Bounds.Contains(Convert.ToInt32(Left), Convert.ToInt32(Top));
        }
        private bool IsBottomRightCornerInScreen(Screen screen)
        {
            return screen.Bounds.Contains(Convert.ToInt32(Left + ActualWidth), Convert.ToInt32(Top + ActualHeight));
        }
        private bool IsTopRightCornerInScreen(Screen screen)
        {
            return screen.Bounds.Contains(Convert.ToInt32(Left + ActualWidth), Convert.ToInt32(Top));
        }
        private bool IsBottomLeftCornerInScreen(Screen screen)
        {
            return screen.Bounds.Contains(Convert.ToInt32(Left), Convert.ToInt32(Top + ActualHeight));
        }

        private Screen ScreenFromWindowCenter()
        {
            return Screen.AllScreens.FirstOrDefault(x =>
                x.Bounds.Contains(Convert.ToInt32(WindowCenter.X), Convert.ToInt32(WindowCenter.Y)));
        }
        private void UpdateButtons()
        {
            if (ButtonsRef == null) return;

            var screenMiddle = WindowManager.ScreenSize.Height / 2;
            var middle = Top + Height / 2;
            var deadzone = WindowManager.ScreenSize.Height / 15;
            var distance = Math.Abs(screenMiddle - middle);

            if (!(distance > deadzone)) return;
            if (middle >= screenMiddle)
            {
                WindowSettings.ButtonsPosition = ButtonsPosition.Above;
                Grid.SetRow(ButtonsRef, 0);
            }
            else
            {
                WindowSettings.ButtonsPosition = ButtonsPosition.Below;
                Grid.SetRow(ButtonsRef, 2);
            }
        }
        protected void Drag(object sender, MouseButtonEventArgs e)
        {
            if (WindowSettings != null && !WindowSettings.IgnoreSize) ResizeMode = ResizeMode.NoResize;
            this.TryDragMove();
            UpdateButtons();
            CheckBounds();
            if (WindowSettings != null && !WindowSettings.IgnoreSize) ResizeMode = ResizeMode.CanResize;
            if (WindowSettings == null) return;
            WindowSettings.X = Left / WindowManager.ScreenSize.Width;
            WindowSettings.Y = Top / WindowManager.ScreenSize.Height;
            App.Settings.Save();
        }
        public void CloseWindowSafe()
        {
            Dispatcher.Invoke(() => Close());
            Dispatcher.InvokeShutdown();
            //Hide();
        }
    }
}