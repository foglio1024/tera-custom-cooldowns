using Nostrum.Extensions;
using System.Windows;
using System.Windows.Input;
using TCC.Utilities;

namespace TCC.UI.Windows
{
    public partial class WelcomeWindow
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWikiButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TccUtils.OpenUrl("https://github.com/Foglio1024/Tera-custom-cooldowns/wiki");
            }
            catch
            {

            }
        }
        private void OnTeraDpsDiscordButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TccUtils.OpenUrl("https://discord.gg/anUXQTp");
            }
            catch 
            {

            }
        }
        private void OnToolboxDiscordButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TccUtils.OpenUrl("https://discord.gg/dUNDDtw");
            }
            catch 
            {

            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.TryDragMove();
        }
    }
}
