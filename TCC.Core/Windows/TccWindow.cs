using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TCC.Windows
{
    public class TccWindow : Window
    {
        protected IntPtr _handle;
        protected bool clickThru;
        public bool ClickThru
        {
            get { return clickThru; }
            set
            {
                if (clickThru == value) return;
                clickThru = value;

                if (clickThru) FocusManager.MakeTransparent(_handle);
                else FocusManager.UndoTransparent(_handle);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClickThru"));
            }
        }

        protected WindowSettings _settings;

        protected void InitWindow(WindowSettings ws, bool canClickThru = true, bool canHide = true)
        {
            _handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(_handle);
            FocusManager.HideFromToolBar(_handle);
            Topmost = true;
            ContextMenu = new System.Windows.Controls.ContextMenu();

            if (canHide)
            {
                var HideButton = new System.Windows.Controls.MenuItem() { Header = "Hide" };
                HideButton.Click += (s, ev) =>
                {
                    SetVisibility(Visibility.Hidden);
                };
                ContextMenu.Items.Add(HideButton);
            }

            if (canClickThru)
            {
                var ClickThruButton = new System.Windows.Controls.MenuItem() { Header = "Click through" };
                ClickThruButton.Click += (s, ev) =>
                {
                    SetClickThru(true);
                };
                ContextMenu.Items.Add(ClickThruButton);
            }

            _settings = ws;

            Left = ws.X;
            Top = ws.Y;
            Visibility = ws.Visibility;
            SetClickThru(ws.ClickThru);
            ((FrameworkElement)this.Content).Opacity = 0;

            WindowManager.TccVisibilityChanged += OpacityChange;
            WindowManager.TccDimChanged += OpacityChange;

            Closed += TccWindow_Closed;
        }

        private void TccWindow_Closed(object sender, EventArgs e)
        {
            Dispatcher.InvokeShutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OpacityChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsTccVisible")
            {
                if (WindowManager.IsTccVisible)
                {
                    if (WindowManager.IsTccDim && _settings.AutoDim)
                    {
                        AnimateContentOpacity(_settings.DimOpacity);
                    }
                    else
                    {
                        AnimateContentOpacity(1);
                    }
                }
                else
                {
                    if (_settings.ShowAlways) return;
                    AnimateContentOpacity(0);
                }
            }

            if (e.PropertyName == "IsTccDim")
            {
                if (!WindowManager.IsTccVisible) return;
                if (!_settings.AutoDim) return;

                if (WindowManager.IsTccDim)
                {
                    AnimateContentOpacity(_settings.DimOpacity);
                    if (!SettingsManager.ClickThruWhenDim) return;
                    FocusManager.MakeTransparent(_handle);
                }
                else
                {
                    AnimateContentOpacity(1);
                    if (!SettingsManager.ClickThruWhenDim) return;
                    if (!clickThru) FocusManager.UndoTransparent(_handle);

                }
            }
        }
        public void SetClickThru(bool t)
        {
            ClickThru = t;
        }
        public void SetVisibility(Visibility v)
        {
            this.Dispatcher.Invoke(() =>
            {
                Visibility = v;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visibility"));
            });
        }
        public void AnimateContentOpacity(double opacity)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                ((FrameworkElement)this.Content).BeginAnimation(OpacityProperty, new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(250)));
            }, System.Windows.Threading.DispatcherPriority.DataBind);
        }
        public void RefreshTopmost()
        {
            Dispatcher.InvokeIfRequired(() => { Topmost = false; Topmost = true; }, System.Windows.Threading.DispatcherPriority.DataBind);
        }

        protected void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
                var screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
                var source = PresentationSource.FromVisual(this);
                if (source?.CompositionTarget == null) return;
                var m = source.CompositionTarget.TransformToDevice;
                var dx = m.M11;
                var dy = m.M22;
                var newLeft = Left * dx;
                var newTop = Top * dx;

                _settings.X = newLeft / dx;
                _settings.Y = newTop / dy;
                SettingsManager.SaveSettings();
            }
            catch (Exception)
            {

            }
        }

        public void CloseWindowSafe()
        {
            if (Dispatcher.CheckAccess())
                Close();
            else
                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(Close));
        }

    }
}