using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;
using TCC.Interop;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class SettingsWindow
    {
        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        public SettingsWindow()
        {
            DataContext = new SettingsWindowViewModel();
            InitializeComponent();
            TitleBarGrid.MouseLeftButtonDown += (_, __) => DragMove();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            HideWindow();
            App.Settings.Save();
        }

        private void OpenPlayerBuffSettings(object sender, RoutedEventArgs e)
        {
            new MyAbnormalConfigWindow().ShowWindow(); //by HQ
        }

        private void OpenGroupBuffSettings(object sender, RoutedEventArgs e)
        {
            new GroupAbnormalConfigWindow().ShowWindow();
        }
        private void ResetChatWindowsPosition(object sender, RoutedEventArgs e)
        {
            foreach (var cw in ChatWindowManager.Instance.ChatWindows)
            {
                cw.ResetToCenter();
            }
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

        private void OnBigPathLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement t)) return;
            t.Opacity = 0;
            t.RenderTransform = new TranslateTransform(-20, 0);
            var ease = new QuadraticEase();
            var slideAnim = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(750)) { EasingFunction = ease };
            var fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(750)) { EasingFunction = ease };
            t.BeginAnimation(OpacityProperty, fadeAnim);
            t.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideAnim);
        }

        private void ClearChatMessages(object sender, RoutedEventArgs e)
        {
            foreach (var chatMessage in ChatWindowManager.Instance.ChatMessages)
            {
                chatMessage.Dispose();
            }

            ChatWindowManager.Instance.ChatMessages.Clear();
        }

        private async void ForceExperimentalBuildDownlaod(object sender, RoutedEventArgs e)
        {
            if (TccMessageBox.Show("Warning: experimental build could be unstable. Proceed?", MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.Yes)
            {
                await Task.Factory.StartNew(UpdateManager.ForceUpdateExperimental);
            }
        }

        private void RegisterGuildBamWebhook(object sender, RoutedEventArgs e)
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, true);
        }

        private void RegisterFieldBossWebhook(object sender, RoutedEventArgs e)
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, true);
        }

        // memeing
        private int _testNotifIdx;
        List<string> _lyrics = new List<string>
        {
            "This was a triumph",
            "I'm making a note here:",
            "Huge success",
            "It's hard to overstate",
            "My satisfaction",
            "Aperture Science",
            "We do what we must",
            "Because we can",
            "For the good of all of us",
            "Except the ones who are dead", //9

            "But there's no sense crying",
            "Over every mistake",
            "You just keep on trying",
            "Till you run out of cake",
            "And the Science gets done",
            "And you make a neat gun",
            "For the people who are",
            "Still alive", //17

            "I'm not even angry",
            "I'm being so sincere right now",
            "Even though you broke my heart",
            "And killed me",
            "And tore me to pieces",
            "And threw every piece into a fire",
            "As they burned it hurt because",
            "I was so happy for you!", //25

            "Now these points of data",
            "Make a beautiful line",
            "And we're out of beta",
            "We're releasing on time",
            "So I'm glad. I got burned",
            "Think of all the things we learned",
            "For the people who are",
            "Still alive", //33

            "Go ahead and leave me",
            "I think I prefer to stay inside",
            "Maybe you'll find someone else",
            "To help you",
            "Maybe Black Mesa...",
            "That was A joke, ha ha, fat chance",
            "Anyway this cake is great",
            "It's so delicious and moist", //41

            "Look at me still talking when there's science to do",
            "When I look out there",
            "It makes me glad I'm not you",
            "I've experiments to be run",
            "There is research to be done",
            "On the people who are",
            "Still alive", //48

            "And believe me I am still alive",
            "I'm doing science and I'm still alive",
            "I feel fantastic and I'm still alive",
            "And while you're dying I'll be still alive",
            "And when you're dead I will be still alive",
            "Still alive",
            "Still alive" //55
        };
        private void TestNotification(object sender, RoutedEventArgs e)
        {
            var msg = _lyrics[_testNotifIdx];

            var type = NotificationType.Normal;
            if (_testNotifIdx == 2) type = NotificationType.Success;
            else if (_testNotifIdx > 9 && _testNotifIdx <= 17) type = NotificationType.Warning;
            else if (_testNotifIdx > 33 && _testNotifIdx <= 41) type = NotificationType.Error;
            else if (_testNotifIdx > 48 ) type = NotificationType.Success;

            WindowManager.ViewModels.NotificationArea.Enqueue("GLaDOS", msg, type, 2000);
            _testNotifIdx++;
            if (_testNotifIdx >= _lyrics.Count) _testNotifIdx = 0;
        }

        private void OnTabBackgroundMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            ((FrameworkElement)sender).Focus();
        }
    }
}
