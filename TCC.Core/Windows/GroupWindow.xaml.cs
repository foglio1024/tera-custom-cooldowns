using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.ViewModels;

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
