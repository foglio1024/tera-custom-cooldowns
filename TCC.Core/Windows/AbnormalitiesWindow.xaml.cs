using System.Windows;
using System.Windows.Input;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per AbnormalitiesWindow.xaml
    /// </summary>
    public partial class BuffWindow : TccWindow
    {
        public BuffWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow(SettingsManager.BuffWindowSettings, ignoreSize: true);
            //(this.DataContext as BuffBarWindowViewModel).Player = new Data.Player();
            //buffs.ItemsSource = BuffBarWindowViewModel.Instance.Player.Buffs;
            //debuffs.ItemsSource = BuffBarWindowViewModel.Instance.Player.Debuffs;
            //infBuffs.ItemsSource = BuffBarWindowViewModel.Instance.Player.InfBuffs;
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
