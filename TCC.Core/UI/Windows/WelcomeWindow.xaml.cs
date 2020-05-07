using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Nostrum.Extensions;

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
                Process.Start("https://github.com/Foglio1024/Tera-custom-cooldowns/wiki");
            }
            catch
            {

            }
        }
        private void OnTeraDpsDiscordButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://discord.gg/anUXQTp");
            }
            catch 
            {

            }
        }
        private void OnToolboxDiscordButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://discord.gg/dUNDDtw");
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
