using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Nostrum;
using Nostrum.Extensions;
using TCC.Controls;
using TCC.Data;
using TCC.Settings;
using TCC.Utils;

namespace TCC.Windows.Widgets
{
    public class TccWidget : Window
    {
        private static bool _showBoundaries;
        private static event Action ShowBoundariesToggled;

        private readonly DoubleAnimation _opacityAnimation = AnimationFactory.CreateDoubleAnimation(100, to: 0);
        private readonly DoubleAnimation _hideButtonsAnimation = AnimationFactory.CreateDoubleAnimation(1000, to: 0);
        private readonly DoubleAnimation _showButtonsAnimation = AnimationFactory.CreateDoubleAnimation(150, to: 1);
        private DispatcherTimer _buttonsTimer;
        private IntPtr _handle;
        protected bool _canMove = true;
        private Point WindowCenter => new Point(Left + ActualWidth / 2, Top + ActualHeight / 2);

        protected WindowButtons ButtonsRef;
        protected UIElement MainContent;
        protected UIElement BoundaryRef;

        public WindowSettings WindowSettings { get; private set; }

        protected void Init(WindowSettings settings)
        {
            WindowSettings = settings;
            MainContent.Opacity = 0;
            if (BoundaryRef != null) BoundaryRef.Opacity = 0;
            Topmost = true;
            Left = (WindowSettings.X * WindowManager.ScreenSize.Width);
            Top = (WindowSettings.Y * WindowManager.ScreenSize.Height);
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

            WindowManager.VisibilityManager.VisibilityChanged += OnVisibilityChanged;
            WindowManager.VisibilityManager.DimChanged += OnDimChanged;
            WindowManager.VisibilityManager.ClickThruChanged += OnClickThruModeChanged;
            WindowManager.RepositionRequestedEvent += ReloadPosition;
            WindowManager.ResetToCenterEvent += ResetToCenter;
            WindowManager.DisposeEvent += CloseWindowSafe;
            WindowManager.MakeGlobalEvent += WindowSettings.MakePositionsGlobal;
            //WindowManager.ApplyScreenCorrectionEvent += _ => OnTeraScreenChanged(); //WindowSettings.ApplyScreenCorrection;

            FocusManager.TeraScreenChanged += OnTeraScreenChanged;
            FocusManager.FocusTick += OnFocusTick;

            OnClickThruModeChanged();
            OnVisibilityChanged();
            OnWindowVisibilityChanged(WindowSettings.Visible);

            FocusManager.MakeUnfocusable(_handle);

            if (ButtonsRef == null)
            {
                if (_canMove) MouseLeftButtonDown += Drag;
                return;
            }

            _buttonsTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _buttonsTimer.Tick += OnButtonsTimerTick;

            MouseEnter += (_, __) =>
            {
                if (!App.Settings.HideHandles) ButtonsRef.BeginAnimation(OpacityProperty, _showButtonsAnimation);
            };
            MouseLeave += (_, __) => _buttonsTimer.Start();
            if (_canMove) ButtonsRef.MouseLeftButtonDown += Drag;
            ShowBoundariesToggled += ShowHideBoundaries;
        }

        private void OnTeraScreenChanged(System.Drawing.Point oldPos, System.Drawing.Point newPos, System.Drawing.Size size)
        {
            var op = new Point(oldPos.X, oldPos.Y); //sigh
            var np = new Point(newPos.X, newPos.Y); //sigh
            var s = new Size(size.Width, size.Height); //sigh
            WindowSettings.ApplyScreenOffset(op, np, s);
            ReloadPosition();
        }

        private void ShowHideBoundaries()
        {
            var anim = _showBoundaries ? _showButtonsAnimation : _hideButtonsAnimation;
            Dispatcher?.InvokeAsync(() =>
            {
                BoundaryRef?.BeginAnimation(OpacityProperty, anim);
                ButtonsRef?.BeginAnimation(OpacityProperty, anim);
                OnClickThruModeChanged();
            });
        }

        protected void ReloadPosition()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                var left = WindowSettings.X * WindowManager.ScreenSize.Width;
                Left = left >= int.MaxValue ? 0 : left;

                var top = WindowSettings.Y * WindowManager.ScreenSize.Height;
                Top = top >= int.MaxValue ? 0 : top;

                CheckBounds();

                if (ButtonsRef == null) return;
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
            });
        }
        public void ResetToCenter()
        {
            Log.CW($"[{GetType().Name}] {nameof(ResetToCenter)}()");

            Dispatcher?.Invoke(() =>
            {
                Left = FocusManager.TeraScreen.Bounds.X + FocusManager.TeraScreen.Bounds.Width / 2 - ActualWidth / 2;
                Top = FocusManager.TeraScreen.Bounds.Y + FocusManager.TeraScreen.Bounds.Height / 2 - ActualHeight / 2;
                SetRelativeCoordinates();
            });
        }

        private void OnFocusTick()
        {
            if (FocusManager.PauseTopmost) return;
            if (WindowSettings.ShowAlways) RefreshTopmost();
            if (WindowManager.VisibilityManager.Visible) RefreshTopmost();
        }

        private void OnWindowVisibilityChanged(bool visible)
        {
            SetVisibility(visible);
        }

        private void OnButtonsTimerTick(object sender, EventArgs e)
        {
            _buttonsTimer.Stop();
            if (IsMouseOver || _showBoundaries) return;
            ButtonsRef.BeginAnimation(OpacityProperty, _hideButtonsAnimation);
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

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            _handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(_handle);
            FocusManager.HideFromToolBar(_handle);
            if (!WindowSettings.Enabled) Hide();
        }
        private void OnDimChanged()
        {
            if (!WindowManager.VisibilityManager.Visible) return;

            if (!WindowSettings.AutoDim)
                AnimateContentOpacity(WindowSettings.MaxOpacity);
            else
            {
                if (WindowSettings.UndimOnFlyingGuardian) AnimateContentOpacity(WindowManager.VisibilityManager.Dim ? WindowSettings.DimOpacity : WindowSettings.MaxOpacity);
                else if (FlyingGuardianDataProvider.IsInProgress) AnimateContentOpacity(WindowSettings.DimOpacity);
                else AnimateContentOpacity(WindowManager.VisibilityManager.Dim ? WindowSettings.DimOpacity : WindowSettings.MaxOpacity);
            }

            OnClickThruModeChanged();
        }
        protected virtual void OnVisibilityChanged()
        {
            if (WindowManager.VisibilityManager.Visible)
            {
                if (WindowManager.VisibilityManager.Dim && WindowSettings.AutoDim)
                {
                    AnimateContentOpacity(WindowSettings.DimOpacity);
                }
                else
                {
                    AnimateContentOpacity(WindowSettings.MaxOpacity);
                }
            }
            else
            {
                if (WindowSettings.ShowAlways) return;
                AnimateContentOpacity(0);
            }
            RefreshTopmost();
        }
        private void OnClickThruModeChanged()
        {
            if (_showBoundaries)
            {
                FocusManager.UndoClickThru(_handle);
                return;
            }

            switch (WindowSettings.ClickThruMode)
            {
                case ClickThruMode.Never:
                    FocusManager.UndoClickThru(_handle);
                    break;
                case ClickThruMode.Always:
                    FocusManager.MakeClickThru(_handle);
                    break;
                case ClickThruMode.WhenDim:
                    if (WindowManager.VisibilityManager.Dim) FocusManager.MakeClickThru(_handle);
                    else FocusManager.UndoClickThru(_handle);
                    break;
                case ClickThruMode.WhenUndim:
                    if (WindowManager.VisibilityManager.Dim) FocusManager.UndoClickThru(_handle);
                    else FocusManager.MakeClickThru(_handle);
                    break;
                case ClickThruMode.GameDriven:
                    if (Game.InGameUiOn) FocusManager.UndoClickThru(_handle);
                    else FocusManager.MakeClickThru(_handle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected virtual void OnEnabledChanged(bool enabled)
        {
            try
            {
                Dispatcher?.Invoke(() =>
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
            Dispatcher?.InvokeAsync(() =>
                {
                    _opacityAnimation.To = opacity;
                    MainContent.BeginAnimation(OpacityProperty, _opacityAnimation);
                }
            , DispatcherPriority.DataBind);
        }
        private void RefreshTopmost()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                if (FocusManager.PauseTopmost) return;
                Topmost = false; Topmost = true;
            }, DispatcherPriority.DataBind);
        }
        private void SetVisibility(bool v)
        {
            if (Dispatcher?.Thread.IsAlive == false) return;
            Dispatcher?.Invoke(() =>
            {
                Visibility = !v ? Visibility.Visible : Visibility.Collapsed; // meh ok
                Visibility = v ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        private void CheckBounds()
        {
            if (WindowSettings == null) return;
            if (WindowSettings.AllowOffScreen) return;
            if (Left + ActualWidth > SystemParameters.VirtualScreenWidth)
            {
                Left = SystemParameters.VirtualScreenWidth - ActualWidth;
            }
            if (Top + ActualHeight > SystemParameters.VirtualScreenHeight)
            {
                Top = SystemParameters.VirtualScreenHeight - ActualHeight;
            }
            CheckIndividualScreensBounds();
            SetRelativeCoordinates();
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
            var currOp = Opacity;
            if (!_showBoundaries) BoundaryRef?.BeginAnimation(OpacityProperty, _showButtonsAnimation);
            Opacity = .7;
            this.TryDragMove();
            if (!_showBoundaries) BoundaryRef?.BeginAnimation(OpacityProperty, _hideButtonsAnimation);
            Opacity = currOp;
            UpdateButtons();
            CheckBounds();
            if (WindowSettings != null && !WindowSettings.IgnoreSize) ResizeMode = ResizeMode.CanResize;
            if (WindowSettings == null) return;
            SetRelativeCoordinates();
            App.Settings.Save();
        }

        private void SetRelativeCoordinates()
        {
            WindowSettings.X = (Left /*+ FocusManager.TeraScreen.Bounds.Left*/) / WindowManager.ScreenSize.Width;
            WindowSettings.Y = (Top /*+ FocusManager.TeraScreen.Bounds.Top*/) / WindowManager.ScreenSize.Height;
        }


        public void CloseWindowSafe()
        {
            Dispatcher?.Invoke(() =>
            {
                WindowManager.VisibilityManager.VisibilityChanged -= OnVisibilityChanged;
                WindowManager.VisibilityManager.DimChanged -= OnDimChanged;
                WindowManager.VisibilityManager.ClickThruChanged -= OnClickThruModeChanged;
                WindowManager.RepositionRequestedEvent -= ReloadPosition;
                WindowManager.ResetToCenterEvent -= ResetToCenter;
                WindowManager.DisposeEvent -= CloseWindowSafe;
                WindowManager.MakeGlobalEvent -= WindowSettings.MakePositionsGlobal;
                FocusManager.FocusTick -= OnFocusTick;
                WindowSettings.EnabledChanged -= OnEnabledChanged;
                WindowSettings.ClickThruModeChanged -= OnClickThruModeChanged;
                WindowSettings.VisibilityChanged -= OnWindowVisibilityChanged;
                WindowSettings.ResetToCenter -= ResetToCenter;
                Loaded -= OnLoaded;
                SizeChanged -= OnSizeChanged;
                Close();
            });

            if (Dispatcher != App.BaseDispatcher)
            {
                Log.CW($"[{GetType().Name}] Invoking dispatcher shutdown");
                Dispatcher?.Invoke(() => Thread.Sleep(100)); //uhmmmmm ok
                Dispatcher?.BeginInvokeShutdown(DispatcherPriority.ContextIdle);
            }
        }

        public static void OnShowAllHandlesToggled()
        {
            _showBoundaries = !_showBoundaries;
            ShowBoundariesToggled?.Invoke();
        }

    }
}