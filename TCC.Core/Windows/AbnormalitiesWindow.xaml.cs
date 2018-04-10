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
            _b = buttons;
            InitWindow(SettingsManager.BuffWindowSettings, ignoreSize: true);
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
