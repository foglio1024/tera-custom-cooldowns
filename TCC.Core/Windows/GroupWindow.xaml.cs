using System.Windows;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow
    {
        public GroupWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContentRef = content;
            InitWindow(SettingsManager.GroupWindowSettings, ignoreSize: false);
        }

        private void LootSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            Proxy.LootSettings();
        }

    }
}