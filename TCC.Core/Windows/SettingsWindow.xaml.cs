using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Closing += SettingsWindow_Closing;
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Image_MouseLeftButtonDown(null, null);
        }

        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, RoutedEventArgs routedEventArgs)
        {
            SettingsWriter.Save();
            HideWindow();
        }

        public void HideWindow()
        {
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            a.Completed += (s, ev) =>
            {
                Hide();
                if (Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            };
            BeginAnimation(OpacityProperty, a);
            WindowManager.ForegroundManager.RefreshVisible();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FocusManager.settingsWindowHandle = new WindowInteropHelper(this).Handle;

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
            ((FrameworkElement)Content).Focus();
        }
        public void ShowWindow()
        {
            if (Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;

            Opacity = 0;
            Activate();
            Show();
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));

        }

        private void SendWebhookTest(object sender, RoutedEventArgs e)
        {
            TimeManager.Instance.SendWebhookMessageOld(testMessage: true);
        }

        private void OpenSettingsFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/resources/config");
        }

/*
        private void ConnectToTwitch(object sender, RoutedEventArgs e)
        {
            //TwitchConnector.Instance.Init();
        }
*/

        private void PaypalLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://paypal.me/foglio1024");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            WindowManager.GroupAbnormalConfigWindow.ShowWindow();
        }

        private void MakePositionsGlobal(object sender, RoutedEventArgs e)
        {
            WindowManager.MakeGlobal();
        }

        private void WindowPositionsReset(object sender, RoutedEventArgs e)
        {
            WindowManager.ResetToCenter();
        }

        private void ResetCharacterWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.CharacterWindow.ResetToCenter();
        }

        private void ResetBossWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.BossWindow.ResetToCenter();
        }

        private void ResetCooldownWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.CooldownWindow.ResetToCenter();
        }

        private void ResetChatWindowsPosition(object sender, RoutedEventArgs e)
        {
            foreach (var cw in ChatWindowManager.Instance.ChatWindows)
            {
                cw.ResetToCenter();
            }
        }

        private void ResetBuffWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.BuffWindow.ResetToCenter();
        }

        private void ResetClassWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.ClassWindow.ResetToCenter();
        }

        private void ResetGroupWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.GroupWindow.ResetToCenter();
        }

        private void ResetFlightGaugePosition(object sender, RoutedEventArgs e)
        {
            WindowManager.FlightDurationWindow.ResetToCenter();
        }

        private void ResetCuWindowPosition(object sender, RoutedEventArgs e)
        {
            WindowManager.CivilUnrestWindow.ResetToCenter();
        }
    }
}
