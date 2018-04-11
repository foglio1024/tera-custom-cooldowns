using System.Windows;
using System.Windows.Input;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per HPbar.xaml
    /// </summary>
    public partial class CharacterWindow : TccWindow
    {
        public CharacterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow(SettingsManager.CharacterWindowSettings, ignoreSize: true);

            //rootGrid.DataContext = CharacterWindowViewModel.Instance.Player;
        }



        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

