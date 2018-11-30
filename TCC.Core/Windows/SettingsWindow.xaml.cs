using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly DoubleAnimation _closeWindowAnim;
        private readonly DoubleAnimation _showWindowAnim;

        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        public SettingsWindow()
        {
            InitializeComponent();

            _closeWindowAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            _closeWindowAnim.Completed += (s, ev) =>
            {
                Hide();
                if (Settings.Settings.ForceSoftwareRendering)
                    RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            };
            _showWindowAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));

            TitleBarGrid.MouseLeftButtonDown += (_, __) => DragMove();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsWriter.Save();
            HideWindow();
        }

        public void ShowWindow()
        {
            if (Settings.Settings.ForceSoftwareRendering)
                RenderOptions.ProcessRenderMode = RenderMode.Default;

            Opacity = 0;
            Activate();
            Show();
            BeginAnimation(OpacityProperty, _showWindowAnim);

        }
        public void HideWindow()
        {
            BeginAnimation(OpacityProperty, _closeWindowAnim);
        }

        private void OpenPlayerBuffSettings(object sender, RoutedEventArgs e)
        {
            //Add My Abnormals Setting by HQ ============================================================
            WindowManager.MyAbnormalConfigWindow.ShowWindow();
            //===========================================================================================
        }

        private void OpenGroupBuffSettings(object sender, RoutedEventArgs e)
        {
            WindowManager.GroupAbnormalConfigWindow.ShowWindow();
        }
        private void ResetChatWindowsPosition(object sender, RoutedEventArgs e)
        {
            foreach (var cw in ChatWindowManager.Instance.ChatWindows)
            {
                cw.ResetToCenter();
            }
        }

        private void SendWebhookTest(object sender, RoutedEventArgs e)
        {
            TimeManager.Instance.SendWebhookMessageOld(testMessage: true);
        }

        private void MakePositionsGlobal(object sender, RoutedEventArgs e)
        {
            WindowManager.MakeGlobal();
        }

        private void WindowPositionsReset(object sender, RoutedEventArgs e)
        {
            WindowManager.ResetToCenter();
        }

        private void OpenResourcesFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine(App.BasePath, "resources/config"));
        }

        private void GoToPaypal(object sender, RoutedEventArgs e)
        {
            Process.Start("https://paypal.me/foglio1024");
        }

        private void GoToBamTimes(object sender, RoutedEventArgs e)
        {
            Process.Start("https://tcc-web-99a64.firebaseapp.com/");
        }

        private void GoToRestyle(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/tera-restyle/wiki");
        }

        private void GoToChat2(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/S1UI_chat2/blob/master/p75/S1UI_Chat2.gpk");
        }

        private void GoToTccStub(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/tcc-stub");
        }

        private void GoToCaaliModsDiscord(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/dUNDDtw");
        }

        private void GoToTeraDpsDiscord(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/anUXQTp");
        }

        private void GoToIssues(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/Tera-custom-cooldowns/issues");
        }

        private void GoToReleases(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/Tera-custom-cooldowns/releases");
        }

        private void GoToWiki(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Foglio1024/Tera-custom-cooldowns/wiki");
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            var t = sender as FrameworkElement;
            t.Opacity = 0;
            t.RenderTransform = new TranslateTransform(-20, 0);
            var ease = new QuadraticEase();
            var slideAnim = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(750)){EasingFunction = ease};
            var fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(750)){EasingFunction = ease};
            t.BeginAnimation(OpacityProperty, fadeAnim);
            t.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideAnim);

        }
    }
}
