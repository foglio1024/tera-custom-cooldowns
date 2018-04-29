using System.Windows;

namespace TCC.Converters
{
}
namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow : TccWindow
    {
        public GroupWindow()
        {
            InitializeComponent();
            _b = buttons;
            _c = content;
            InitWindow(SettingsManager.GroupWindowSettings, ignoreSize: false);
        }

        private void LootSettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            Proxy.LootSettings();
        }

    }
}
