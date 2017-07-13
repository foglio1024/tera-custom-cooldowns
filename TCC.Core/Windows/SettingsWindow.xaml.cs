using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingsManager.SaveSettings();
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            a.Completed += (s, ev) => Hide();
            this.BeginAnimation(OpacityProperty, a);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.settingsWindowHandle = new WindowInteropHelper(this).Handle;

        }

        private void GitHubLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/Tera-custom-cooldowns/releases");
        }

        private void DiscordLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://discord.gg/anUXQTp");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((FrameworkElement)this.Content).Focus();
        }
    }
}
