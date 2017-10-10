using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

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

        private void Image_MouseLeftButtonDown(object sender, RoutedEventArgs routedEventArgs)
        {
            SettingsManager.SaveSettings();
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            a.Completed += (s, ev) => Hide();
            this.BeginAnimation(OpacityProperty, a);
            WindowManager.IsTccVisible = true;

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
        public void ShowWindow()
        {
            Opacity = 0;
            Show();
            BeginAnimation(Window.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));

        }

        private void SendWebhookTest(object sender, RoutedEventArgs e)
        {
            TimeManager.Instance.SendWebhookMessage();
        }

        private void OpenSettingsFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory + "/resources/config");
        }

        private void ConnectToTwitch(object sender, RoutedEventArgs e)
        {
            TwitchConnector.Instance.Init();
        }
    }
}
