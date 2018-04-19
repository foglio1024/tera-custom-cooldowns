using DamageMeter.Sniffing;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing;
using TCC.Parsing.Messages;
using TCC.ViewModels;
using TCC.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App
    {
        public static bool Debug = true;
        public static TCC.Windows.SplashScreen SplashScreen;
        public static string Version;
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/error.txt",
                "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                "\r\n" + ex.TargetSite);
            try
            {
                var t = new Thread(() => UploadCrashDump(e));
                t.Start();
            }
            catch (Exception)
            {
            }
            System.Windows.MessageBox.Show("An error occured and TCC will now close. Check error.txt for more info.", "TCC",
                MessageBoxButton.OK, MessageBoxImage.Error);

            if (Proxy.IsConnected) Proxy.CloseConnection();
            if (WindowManager.TrayIcon != null)
            {
                WindowManager.TrayIcon.Dispose();
            }
            try { WindowManager.Dispose(); } catch { }
            Environment.Exit(-1);
        }
        private static void UploadCrashDump(UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var c = new WebClient();
            c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
            var js = new JObject();
            var full = ex.Message + "\r\n" +
                ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                "\r\n" + ex.TargetSite;
            js.Add("tcc_version", new JValue(Version));
            js.Add("full_exception", new JValue(full.Replace(@"C:\Users\Vincenzo\Documents\Progetti VS\", "")));
            js.Add("inner_exception", new JValue(ex.InnerException != null ? ex.InnerException.Message.ToString() : "undefined"));
            js.Add("exception", new JValue(ex.Message.ToString()));
            js.Add("game_version", new JValue(PacketProcessor.Version));
            if (PacketProcessor.Server != null)
            {
                js.Add("region", new JValue(PacketProcessor.Server.Region));
                js.Add("server_id", new JValue(PacketProcessor.Server.ServerId));
            }
            else
            {
                js.Add("region", new JValue(""));
                js.Add("server_id", new JValue(""));
            }
            c.Encoding = Encoding.UTF8;
            c.UploadString(new Uri("https://us-central1-tcc-report.cloudfunctions.net/crash"), Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
        }
        internal static void Restart()
        {
            SettingsManager.SaveSettings();
            Process.Start("TCC.exe");
            CloseApp();
        }
        private static void InitSS()
        {
            var waiting = true;
            var ssThread = new Thread(new ThreadStart(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                SplashScreen = new TCC.Windows.SplashScreen();
                SplashScreen.SetText("Initializing...");
                SplashScreen.SetVer(Version);
                SplashScreen.Show();
                waiting = false;
                Dispatcher.Run();
            }));
            ssThread.Name = "SplashScreen window thread";
            ssThread.SetApartmentState(ApartmentState.STA);
            ssThread.Start();
            while (waiting)
            {
                Thread.Sleep(1);
            }
        }

        public static void SendUsageStat()
        {
            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var js = new JObject()
                {
                    { "server", PacketProcessor.Server.ServerId},
                    { "id", InfoWindowViewModel.Instance.Characters == null ?0 : InfoWindowViewModel.Instance.Characters.Count == 0 ? 0 : InfoWindowViewModel.Instance.Characters.FirstOrDefault(x => x.Position == 1).Id },
                    { "region", PacketProcessor.Server.Region },
                };
                c.Encoding = Encoding.UTF8;
                c.UploadStringAsync(new Uri("https://us-central1-tcc-report.cloudfunctions.net/stat"), Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
            }
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            Version = $"TCC v{v.Major}.{v.Minor}.{v.Build}";
            InitSS();
            var cd = AppDomain.CurrentDomain;
            cd.UnhandledException += GlobalUnhandledExceptionHandler;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "/TCCupdater.exe");
            }
            catch (Exception) { }
            SplashScreen.SetText("Checking for application updates...");

            UpdateManager.CheckAppVersion();
            SplashScreen.SetText("Checking for database updates...");

            UpdateManager.CheckDatabaseVersion();

            SplashScreen.SetText("Loading settings...");
            SettingsManager.LoadWindowSettings();
            SettingsManager.LoadSettings();
            SplashScreen.SetText("Initializing windows...");
            WindowManager.Init();
            WindowManager.Settings = new SettingsWindow() { Name = "Settings" };
            FocusManager.FocusTimer.Start();
            SplashScreen.SetText("Initializing Twitch connector...");
            TwitchConnector.Instance.Init();
            SplashScreen.SetText("Initializing packet processor...");
            PacketProcessor.Init();

            TeraSniffer.Instance.NewConnection += (srv) =>
            {
                PacketProcessor.Server = srv;
                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
                ChatWindowViewModel.Instance.AddTccMessage($"Connected to {srv.Name}.");
            };
            TeraSniffer.Instance.EndConnection += () =>
            {
                ChatWindowViewModel.Instance.AddTccMessage("Disconnected from the server.");
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

            SplashScreen.SetText("Starting");

            TimeManager.Instance.SetServerTimeZone(SettingsManager.LastRegion);

            ChatWindowViewModel.Instance.AddTccMessage(Version);

            SplashScreen.CloseWindowSafe();
            if (!Debug) return;
            SessionManager.CurrentPlayer = new Player(1, "Foglio");
            SessionManager.CurrentPlayer.Class = Class.Priest;
            SkillsDatabase.Load("EU-EN");
            CooldownWindowViewModel.Instance.LoadSkills(Utils.ClassEnumToString(Class.Priest).ToLower() + "-skills.xml", Class.Priest);
            AbnormalityManager.CurrentDb = new AbnormalityDatabase("EU-EN");
            var s = AbnormalityManager.CurrentDb.Abnormalities[4];
            BuffBarWindowViewModel.Instance.Player.AddOrRefreshBuff(s, Int32.MaxValue, 5);
            //ss.Dispatcher.Invoke(new Action(() => ss.Close()));


            var u = new User(GroupWindowViewModel.Instance.GetDispatcher());
            u.Name = "Test_Dps";
            u.PlayerId = 1;
            u.ServerId = 0;
            u.Online = true;
            u.UserClass = Class.Warrior;


            //GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            //u = new User(GroupWindowViewModel.Instance.GetDispatcher());
            //u.Name = "Test_Healer";
            //u.PlayerId = 2;
            //u.ServerId = 0;
            //u.Online = true;
            //u.UserClass = Class.Priest;

            //GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            //u = new User(GroupWindowViewModel.Instance.GetDispatcher());
            //u.Name = "Test_Tank";
            //u.PlayerId = 3;
            //u.ServerId = 0;
            //u.Online = true;
            //u.UserClass = Class.Lancer;

            GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            WindowManager.ClassWindow.Context.CurrentClass = Class.Priest;
            var l = new List<User>();
            for (uint i = 0; i < 10; i++)
            {
                l.Add(new User(GroupWindowViewModel.Instance.GetDispatcher())
                {
                    Name = i.ToString(),
                    PlayerId = i,
                    ServerId = i,
                    EntityId = i
                });
            }

            foreach (var item in l)
            {
                GroupWindowViewModel.Instance.AddOrUpdateMember(item);
            }
            return;
            foreach (var item in l)
            {
                GroupWindowViewModel.Instance.AddOrUpdateMember(item);
            }
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsManager.SaveSettings();
            WindowManager.Dispose();
            Proxy.CloseConnection();
            Environment.Exit(0);
        }

        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            FocusManager.Running = false;
        }

        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            FocusManager.Running = true;

        }
    }

}
