using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
