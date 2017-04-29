using DamageMeter.Sniffing;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace TCC
{
    public partial class CooldownWindow : Window
    {
        public CooldownWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            Topmost = true;
            Opacity = 0;
            ContextMenu = new ContextMenu();
            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) =>
            {
                this.Visibility = Visibility.Collapsed;
            };
            ContextMenu.Items.Add(HideButton);
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/