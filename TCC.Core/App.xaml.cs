using DamageMeter.Sniffing;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing;
using TCC.Parsing.Messages;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App
    {
        public static bool Debug = false;
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            File.WriteAllText(Environment.CurrentDirectory + "/error.txt",
                "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                "\r\n" + ex.TargetSite);
            MessageBox.Show("An error occured and TCC will now close. Check error.txt for more info.", "TCC",
                MessageBoxButton.OK, MessageBoxImage.Error);
            if (WindowManager.TrayIcon != null)
            {
                WindowManager.TrayIcon.Dispose();
            }
        }

        internal static void Restart()
        {
            SettingsManager.SaveSettings();
            Process.Start("TCC.exe");
            CloseApp();
        }

        private void OnStartup(object sender, StartupEventArgs ev)
        {
            var cd = AppDomain.CurrentDomain;
            cd.UnhandledException += GlobalUnhandledExceptionHandler;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            if (File.Exists(Environment.CurrentDirectory + "/TCCupdater.exe"))
            {
                File.Delete(Environment.CurrentDirectory + "/TCCupdater.exe");
            }
            UpdateManager.CheckAppVersion();
            UpdateManager.CheckDatabaseVersion();

            SkillsDatabase.Load();
            ItemSkillsDatabase.SetBroochesIcons();
            SettingsManager.LoadWindowSettings();
            SettingsManager.LoadSettings();
            WindowManager.Init();
            WindowManager.Settings = new SettingsWindow() { Name = "Settings" };
            FocusManager.FocusTimer.Start();
            TwitchConnector.Instance.Init();
            PacketProcessor.Init();

            TeraSniffer.Instance.NewConnection += (srv) =>
            {
                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
                ChatWindowViewModel.Instance.AddChatMessage(
                    new ChatMessage(ChatChannel.TCC, "System", "<FONT>Connected to server.</FONT>"));
            };
            TeraSniffer.Instance.EndConnection += () =>
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System",
                    "<FONT>Disconnected from server.</FONT>"));
                GroupWindowViewModel.Instance.ClearAllAbnormalities();
                BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
                EntitiesManager.ClearNPC();

                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
                Proxy.CloseConnection();
            };

            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.Name = "player";

            TeraSniffer.Instance.Enabled = true;
            TimeManager.Instance.SetServerTimeZone(SettingsManager.LastRegion);

            var v = Assembly.GetExecutingAssembly().GetName().Version;
            var ver = $"TCC v{v.Major}.{v.Minor}.{v.Build}";
            ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System", $"<FONT>{ver}</FONT>"));

            return;
            SessionManager.CurrentPlayer = new Player(1, "Foglio");
            CooldownWindowViewModel.Instance.LoadSkills(Utils.ClassEnumToString(Class.Warrior).ToLower() + "-skills.xml", Class.Warrior);

            for (uint i = 0; i < 1; i++)
            {
                var u = new User(GroupWindowViewModel.Instance.GetDispatcher());
                u.Name = "Test_Dps" + i;
                u.PlayerId = i;
                u.Online = true;
                u.UserClass = Class.Warrior;
                

                GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            }
            for (uint i = 0; i < 2; i++)
            {
                var u = new User(GroupWindowViewModel.Instance.GetDispatcher());
                u.Name = "Test_Healer" + i;
                u.PlayerId = i +10;
                u.Online = true;
                u.UserClass = Class.Elementalist;
                if (i == 1) u.Alive = false;
                if (i == 0) u.Name = "Foglio";

                GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            }
            for (uint i = 0; i < 4; i++)
            {
                var u = new User(GroupWindowViewModel.Instance.GetDispatcher());
                u.Name = "Test_Tank" + i;
                u.PlayerId = i + 100;
                u.Online = true;
                u.UserClass = Class.Lancer;
                GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            }
            foreach (var user in GroupWindowViewModel.Instance.All)
            {
                user.AddOrRefreshBuff(new Abnormality(4611, true, true, false, AbnormalityType.Buff),60*100*60,1);
                user.AddOrRefreshBuff(new Abnormality(46126, true, true, false, AbnormalityType.Buff),60*100*60,1);
                user.AddOrRefreshDebuff(new Abnormality(89308100, true, false, false, AbnormalityType.DOT),60*100*60,5);
                user.AddOrRefreshDebuff(new Abnormality(89308101, true, false, false, AbnormalityType.DOT),60*100*60,5);

            }
            //GroupWindowViewModel.Instance.StartRoll();
            //GroupWindowViewModel.Instance.SetReadyStatus(new ReadyPartyMember());
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsManager.SaveSettings();
            WindowManager.Dispose();
            Proxy.CloseConnection();
            Environment.Exit(0);
        }

    }
}
