using System.Windows.Input;

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
            ButtonsRef = Buttons;
            MainContentRef = content;
            InitWindow(SettingsManager.BuffWindowSettings, ignoreSize: true);
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
