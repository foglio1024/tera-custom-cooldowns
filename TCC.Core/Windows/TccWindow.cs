using System;
using System.ComponentModel;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;

namespace TCC.Windows
{
    public class TccWidget : Window
    {
        private WindowSettings _settings;
        private bool _ignoreSize;
        private DoubleAnimation _opacityAnimation;
        private DoubleAnimation _hideButtons;
        private DoubleAnimation _showButtons;
        private DispatcherTimer _buttonsTimer;

        protected WindowButtons ButtonsRef;
        protected UIElement MainContent;

        public WindowSettings WindowSettings => _settings;
        public IntPtr Handle => new WindowInteropHelper(this).Handle; 

        public TccWidget()
        {

        }

        protected void Init(WindowSettings settings, bool ignoreSize = true)
        {
            _settings = settings;
            _ignoreSize = ignoreSize;

            _opacityAnimation = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(250) };

            Topmost = true;
            Left = settings.X * SettingsManager.ScreenW;
            Top = settings.Y * SettingsManager.ScreenH;

            if (!ignoreSize)
            {
                if (settings.H != 0) Height = settings.H;
                if (settings.W != 0) Width = settings.W;
            }

            _settings.EnabledChanged += OnEnabledChanged;
            _settings.ClickThruModeChanged += OnClickThruModeChanged;
            _settings.VisibilityChanged += OnWindowVisibilityChanged;

            Loaded += OnLoaded;
            Closing += (_, args) =>
            {
                args.Cancel = true;
                Hide();
            };
            SizeChanged += OnSizeChanged;

            WindowManager.ForegroundManager.VisibilityChanged += OnVisibilityChanged;
            WindowManager.ForegroundManager.DimChanged += OnDimChanged;

            if(_settings.Enabled) Show();
            OnClickThruModeChanged();
            OnVisibilityChanged();
            OnWindowVisibilityChanged();

            if (ButtonsRef == null)
            {
                MouseLeftButtonDown += Drag;
                return;
            }
            _hideButtons = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000));
            _showButtons = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));
            _buttonsTimer = new DispatcherTimer{Interval = TimeSpan.FromSeconds(2)};
            _buttonsTimer.Tick +=OnButtonsTimerTick;

            MouseEnter += (_, __) => ButtonsRef.BeginAnimation(OpacityProperty, _showButtons);
            MouseLeave += (_, __) => _buttonsTimer.Start();
            ButtonsRef.MouseLeftButtonDown += Drag;
        }

        private void OnWindowVisibilityChanged()
        {
            SetVisibility(_settings.Visible);
        }

        private void OnButtonsTimerTick(object sender, EventArgs e)
        {
            _buttonsTimer.Stop();
            if (IsMouseOver) return;
            ButtonsRef.BeginAnimation(OpacityProperty, _hideButtons);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(!_settings.AllowOffScreen) CheckBounds();
            if (_ignoreSize) return;
            _settings.W = ActualWidth;
            _settings.H = ActualHeight;
            SettingsManager.SaveSettings();
        }

        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            FocusManager.MakeUnfocusable(Handle);
            FocusManager.HideFromToolBar(Handle);
            if (!_settings.Enabled) Hide();
        }

        private void OnDimChanged()
        {
            if (!WindowManager.ForegroundManager.Visible) return;
            if (!_settings.AutoDim)
            {
                AnimateContentOpacity(1);
            }
            else
            {
                AnimateContentOpacity(WindowManager.ForegroundManager.Dim ? _settings.DimOpacity : 1);
            }
            switch (_settings.ClickThruMode)
            {
                case ClickThruMode.WhenUndim:
                    if (WindowManager.ForegroundManager.Dim) FocusManager.UndoClickThru(Handle);
                    else FocusManager.MakeClickThru(Handle);
                    break;
                case ClickThruMode.WhenDim:
                    if (!WindowManager.ForegroundManager.Dim) FocusManager.UndoClickThru(Handle);
                    else FocusManager.MakeClickThru(Handle);
                    break;
            }
            OnClickThruModeChanged();
        }
        private void OnVisibilityChanged()
        {
            if (WindowManager.ForegroundManager.Visible)
            {
                if (WindowManager.ForegroundManager.Dim && _settings.AutoDim)
                {
                    AnimateContentOpacity(_settings.DimOpacity);
                }
                else
                {
                    AnimateContentOpacity(1);
                }
                RefreshTopmost();
            }
            else
            {
                if (_settings.ShowAlways) return;
                AnimateContentOpacity(0);
            }
        }
        private void OnClickThruModeChanged()
        {
            switch (_settings.ClickThruMode)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void OnEnabledChanged()
        {
            if (_settings.Enabled) Show();
            else Hide();
        }

        private void AnimateContentOpacity(double opacity)
        {
            if (MainContent == null) return;
            _opacityAnimation.To = opacity;
            Dispatcher.InvokeIfRequired(() => MainContent.BeginAnimation(OpacityProperty, _opacityAnimation)
            , DispatcherPriority.DataBind);
        }
        private void RefreshTopmost()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                Topmost = false; Topmost = true;
            }, DispatcherPriority.DataBind);
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
            if (_settings.AllowOffScreen) return;
            if ((Left + ActualWidth) > SettingsManager.ScreenW)
            {
                Left = SettingsManager.ScreenW - ActualWidth;
            }
            if ((Top + ActualHeight) > SettingsManager.ScreenH)
            {
                Top = SettingsManager.ScreenH - ActualHeight;
            }
            if (Left < 0) Left = 0;
            if (Top < 0) Top = 0;
        }
        protected void Drag(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!_ignoreSize) ResizeMode = ResizeMode.NoResize;
                DragMove();
                CheckBounds();
                if (!_ignoreSize) ResizeMode = ResizeMode.CanResize;
                //var unused = Screen.FromHandle(new WindowInteropHelper(this).Handle);
                //var source = PresentationSource.FromVisual(this);
                //if (source?.CompositionTarget == null) return;
                //var m = source.CompositionTarget.TransformToDevice;
                //var dx = m.M11;
                //var dy = m.M22;
                //var newLeft = Left * dx;
                //var newTop = Top * dx;
                //_settings.X = newLeft / dx;
                //_settings.Y = newTop / dy;

                _settings.X = Left / SettingsManager.ScreenW;
                _settings.Y = Top / SettingsManager.ScreenH;
                SettingsManager.SaveSettings();
            }
            catch
            {
                // ignored
            }
        }

        public void CloseWindowSafe()
        {
            Hide();
        }
    }

    //public class TccWindow : Window, INotifyPropertyChanged
    //{
    //    private WindowSettings _settings;
    //    private bool _ignoreSize;
    //    private bool _clickThru;
    //    private DispatcherTimer _t;
    //    private DoubleAnimation _showButtons;
    //    private DoubleAnimation _hideButtons;
    //    protected IntPtr Handle;
    //    protected WindowButtons ButtonsRef;
    //    protected UIElement MainContentRef;
    //    public bool ClickThru
    //    {
    //        get => _clickThru;
    //        set
    //        {
    //            _clickThru = value;

    //            if (_clickThru) FocusManager.MakeClickThru(Handle);
    //            else FocusManager.UndoClickThru(Handle);
    //            NPC();
    //        }
    //    }
    //    public WindowSettings WindowSettings => _settings;
    //    public static event Action<TccWindow> RecreateWindow;
    //    protected void InitWindow(WindowSettings ws, bool canClickThru = true, bool canHide = true, bool ignoreSize = true)
    //    {
    //        Topmost = true;
    //        _settings = ws;
    //        //_settings.SafeClosed += CloseWindowSafe;
    //        _settings.EnabledChanged += EnableWindow;
    //        _settings.PropertyChanged += _settings_PropertyChanged;
    //        Left = ws.X * SettingsManager.ScreenW;
    //        Top = ws.Y * SettingsManager.ScreenH;
    //        if (!ignoreSize)
    //        {
    //            if (ws.H != 0) Height = ws.H;
    //            if (ws.W != 0) Width = ws.W;
    //        }
    //        _ignoreSize = ignoreSize;
    //        SetVisibility(ws.Visible);
    //        //Visibility = ws.Visible ? Visibility.Visible : Visibility.Hidden;
    //        SetClickThru(ws.ClickThruMode == ClickThruMode.Always);
    //        if (_settings.AutoDim) AnimateContentOpacity(_settings.DimOpacity);
    //        if (!WindowManager.IsTccVisible) AnimateContentOpacity(0);

    //        WindowManager.TccVisibilityChanged += OpacityChange;
    //        WindowManager.TccDimChanged += OpacityChange;
    //        SizeChanged += TccWindow_SizeChanged;
    //        Closed += TccWindow_Closed;
    //        Loaded += TccWindow_Loaded;

    //        /*******************************CONTINUE FROM HERE************************************/
    //        if (ButtonsRef == null) return;

    //        _hideButtons = new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000));
    //        _showButtons = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150));

    //        _t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
    //        _t.Tick += (s, ev) =>
    //        {
    //            _t.Stop();
    //            if (IsMouseOver) return;
    //            ButtonsRef.BeginAnimation(OpacityProperty, _hideButtons);
    //        };

    //        MouseEnter += (s, ev) => ButtonsRef.BeginAnimation(OpacityProperty, _showButtons);
    //        MouseLeave += (s, ev) => _t.Start();
    //        ButtonsRef.MouseLeftButtonDown += Drag;

    //        if (ws.Enabled) Show();

    //    }

    //    private void EnableWindow()
    //    {
    //        RecreateWindow?.Invoke(this);
    //    }

    //    private void _settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == nameof(_settings.ClickThruMode))
    //        {
    //            switch (_settings.ClickThruMode)
    //            {
    //                case ClickThruMode.Never:
    //                    FocusManager.UndoClickThru(Handle);
    //                    break;
    //                case ClickThruMode.Always:
    //                    FocusManager.MakeClickThru(Handle);
    //                    break;
    //                case ClickThruMode.WhenDim:
    //                    if (WindowManager.IsTccDim) FocusManager.MakeClickThru(Handle);
    //                    else FocusManager.UndoClickThru(Handle);
    //                    break;
    //                case ClickThruMode.WhenUndim:
    //                    if (WindowManager.IsTccDim) FocusManager.UndoClickThru(Handle);
    //                    else FocusManager.MakeClickThru(Handle);
    //                    break;
    //                default:
    //                    throw new ArgumentOutOfRangeException();
    //            }
    //        }
    //        else if (e.PropertyName == nameof(_settings.Scale))
    //        {
    //            Dispatcher.Invoke(() =>
    //            {
    //                var vm = (TccWindowViewModel)DataContext;
    //                vm.GetDispatcher().Invoke(() => vm.Scale = _settings.Scale);
    //            });
    //        }
    //        else if (e.PropertyName == nameof(_settings.Visible))
    //        {
    //            SetVisibility(_settings.Visible);
    //        }
    //    }

    //    protected void TccWindow_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        Handle = new WindowInteropHelper(this).Handle;
    //        FocusManager.MakeUnfocusable(Handle);
    //        FocusManager.HideFromToolBar(Handle);
    //        if (!_settings.Enabled) Hide();
    //    }

    //    private void TccWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    //    {
    //        CheckBounds();
    //        if (_ignoreSize) return;
    //        _settings.W = ActualWidth;
    //        _settings.H = ActualHeight;
    //        SettingsManager.SaveSettings();
    //    }

    //    private void TccWindow_Closed(object sender, EventArgs e)
    //    {
    //        //Dispatcher.InvokeShutdown();
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void NPC([CallerMemberName] string p = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    //    }
    //    private void OpacityChange(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == "IsTccVisible")
    //        {
    //            if (WindowManager.IsTccVisible)
    //            {
    //                if (WindowManager.IsTccDim && _settings.AutoDim)
    //                {
    //                    AnimateContentOpacity(_settings.DimOpacity);
    //                }
    //                else
    //                {
    //                    AnimateContentOpacity(1);
    //                }
    //            }
    //            else
    //            {
    //                if (_settings.ShowAlways) return;
    //                AnimateContentOpacity(0);
    //            }
    //        }

    //        //TODO: rework dim/undim and clickthru logic
    //        if (e.PropertyName == "IsTccDim")
    //        {
    //            if (!WindowManager.IsTccVisible) return;
    //            if (!_settings.AutoDim) return;

    //            AnimateContentOpacity(WindowManager.IsTccDim ? _settings.DimOpacity : 1);
    //            if (_settings.ClickThruMode == ClickThruMode.WhenUndim)
    //            {
    //                SetClickThru(!WindowManager.IsTccDim);
    //            }
    //            else if (_settings.ClickThruMode == ClickThruMode.WhenDim)
    //            {
    //                SetClickThru(WindowManager.IsTccDim);
    //            }
    //        }
    //    }

    //    private void SetClickThru(bool t)
    //    {
    //        ClickThru = t;
    //    }
    //    public void SetVisibility(Visibility v)
    //    {
    //        if (!Dispatcher.Thread.IsAlive)
    //        {
    //            return;
    //        }
    //        Dispatcher.Invoke(() =>
    //        {
    //            Visibility = v;
    //            NPC(nameof(Visibility));
    //        });
    //    }

    //    private void SetVisibility(bool v)
    //    {
    //        if (!Dispatcher.Thread.IsAlive)
    //        {
    //            return;
    //        }
    //        Dispatcher.Invoke(() =>
    //        {
    //            Visibility = !v ? Visibility.Visible : Visibility.Collapsed; // meh ok
    //            Visibility = v ? Visibility.Visible : Visibility.Collapsed;
    //            NPC(nameof(Visibility));
    //        });
    //    }

    //    private void AnimateContentOpacity(double opacity)
    //    {
    //        if (MainContentRef == null) return;
    //        Dispatcher.InvokeIfRequired(() =>
    //        {
    //            //var grid = ((Grid)this.Content);
    //            MainContentRef.BeginAnimation(OpacityProperty, new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(250)));
    //        }, DispatcherPriority.DataBind);
    //    }
    //    public void RefreshTopmost()
    //    {
    //        Dispatcher.InvokeIfRequired(() => { Topmost = false; Topmost = true; }, DispatcherPriority.DataBind);
    //    }
    //    public void RefreshSettings(WindowSettings ws)
    //    {
    //        _settings = ws;
    //    }

    //    protected void Drag(object sender, MouseButtonEventArgs e)
    //    {
    //        try
    //        {
    //            if (!_ignoreSize) ResizeMode = ResizeMode.NoResize;
    //            DragMove();
    //            CheckBounds();
    //            if (!_ignoreSize) ResizeMode = ResizeMode.CanResize;
    //            var unused = Screen.FromHandle(new WindowInteropHelper(this).Handle);
    //            var source = PresentationSource.FromVisual(this);
    //            if (source?.CompositionTarget == null) return;
    //            var m = source.CompositionTarget.TransformToDevice;
    //            var dx = m.M11;
    //            var dy = m.M22;
    //            var newLeft = Left * dx;
    //            var newTop = Top * dx;
    //            //_settings.X = newLeft / dx;
    //            //_settings.Y = newTop / dy;

    //            _settings.X = Left / SettingsManager.ScreenW;
    //            _settings.Y = Top / SettingsManager.ScreenH;
    //            SettingsManager.SaveSettings();
    //        }
    //        catch
    //        {
    //            // ignored
    //        }
    //    }

    //    private void CheckBounds()
    //    {
    //        if ((Left + ActualWidth) > SettingsManager.ScreenW)
    //        {
    //            Left = SettingsManager.ScreenW - ActualWidth;
    //        }
    //        if ((Top + ActualHeight) > SettingsManager.ScreenH)
    //        {
    //            Top = SettingsManager.ScreenH - ActualHeight;
    //        }
    //        if (Left < 0) Left = 0;
    //        if (Top < 0) Top = 0;
    //    }

    //    public void CloseWindowSafe()
    //    {
    //        if (Dispatcher.CheckAccess())
    //            Hide();
    //        else
    //            Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(Hide));
    //    }

    //    private bool _isTempShow;

    //    private ClickThruMode _oldCt;
    //    private bool _oldDim;
    //    private bool _oldShowAlways;

    //    public void TempShow()
    //    {
    //        _isTempShow = !_isTempShow;
    //        if (!_settings.Enabled || !_settings.Visible) return;
    //        if (_isTempShow)
    //        {
    //            _oldCt = _settings.ClickThruMode;
    //            _oldDim = _settings.AutoDim;
    //            _oldShowAlways = _settings.ShowAlways;

    //            _settings.ClickThruMode = ClickThruMode.Never;
    //            _settings.AutoDim = false;
    //            _settings.ShowAlways = true;
    //        }
    //        else
    //        {
    //            _settings.ClickThruMode = _oldCt;
    //            _settings.AutoDim = _oldDim;
    //            _settings.ShowAlways = _oldShowAlways;
    //        }
    //    }
    //}
}