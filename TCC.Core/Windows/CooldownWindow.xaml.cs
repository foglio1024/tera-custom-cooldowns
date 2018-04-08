using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public partial class CooldownWindow : TccWindow
    {
        DispatcherTimer _t;
        public CooldownWindow()
        {
            InitializeComponent();
            _t = new DispatcherTimer();
            _t.Interval = TimeSpan.FromMilliseconds(1000);
            _t.Tick += _t_Tick;
            InitWindow(SettingsManager.CooldownWindowSettings, ignoreSize: true);

        }

        private void _t_Tick(object sender, EventArgs e)
        {
            if (buttons.IsMouseOver) return;
            _t.Stop();
            //buttons.Visibility = Visibility.Collapsed;
            buttons.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(1000)));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //SwitchMode();

            //((FrameworkElement)controlContainer.Content).DataContext = CooldownWindowViewModel.Instance;
        }

        public void SwitchMode()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                if (SettingsManager.CooldownBarMode == CooldownBarMode.Fixed)
                {
                    controlContainer.Content = new FixedSkillContainers();
                }
                else
                {
                    controlContainer.Content = new NormalSkillContainer();
                }

                ((FrameworkElement)controlContainer.Content).DataContext = CooldownWindowViewModel.Instance;

            }, DispatcherPriority.Normal);
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //buttons.Visibility = Visibility.Visible;
            _t.Stop();
            buttons.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(250)));

        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //buttons.Visibility = Visibility.Collapsed;
            _t.Start();
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/
