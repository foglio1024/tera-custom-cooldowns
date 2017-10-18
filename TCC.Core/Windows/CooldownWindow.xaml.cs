using System.Windows;
using System.Windows.Threading;
using TCC.Controls;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public partial class CooldownWindow : TccWindow
    {
        public CooldownWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow(SettingsManager.CooldownWindowSettings, ignoreSize: true);

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
