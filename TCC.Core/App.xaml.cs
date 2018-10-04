using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;
using TCC.Sniffing;
using TCC.TeraCommon.Game;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;
using SplashScreen = TCC.Windows.SplashScreen;

namespace TCC
{
    public partial class App
    {
        public const bool Debug = false;
        private static string _version;
        public static SplashScreen SplashScreen;
        public static Dispatcher BaseDispatcher;

        public static bool Loading { get; set; }
        //public static DebugWindow DebugWindow;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            Loading = true;
            //#if DEBUG
            //            DebugWindow = new DebugWindow();
            //            DebugWindow.Show();
            //#endif

            var v = Assembly.GetExecutingAssembly().GetName().Version;
            _version = $"TCC v{v.Major}.{v.Minor}.{v.Build}";
            InitSplashScreen();

            BaseDispatcher = Dispatcher.CurrentDispatcher;
            TccMessageBox.Create(); //Create it here in STA thread
#if DEBUG
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
#endif
            TryDeleteUpdater();

            SplashScreen.SetText("Checking for application updates...");
            UpdateManager.CheckAppVersion();

            SplashScreen.SetText("Checking for database updates...");
            UpdateManager.CheckDatabaseVersion();

            SplashScreen.SetText("Loading settings...");
            var sr = new SettingsReader();
            sr.LoadWindowSettings();
            sr.LoadSettings();

            Process.GetCurrentProcess().PriorityClass = Settings.HighPriority ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            if (Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            SplashScreen.SetText("Pre-loading databases...");
            SessionManager.InitDatabases(string.IsNullOrEmpty(Settings.LastRegion) ? "EU-EN" : Settings.LastRegion == "EU" ? "EU-EN" : Settings.LastRegion);

            SplashScreen.SetText("Initializing windows...");
            WindowManager.Init();

            //SplashScreen.SetText("Initializing Twitch connector...");
            //TwitchConnector.Instance.Init();

            SplashScreen.SetText("Initializing packet processor...");
            PacketProcessor.Init();
            TeraSniffer.Instance.NewConnection += TeraSniffer_OnNewConnection;
            TeraSniffer.Instance.EndConnection += TeraSniffer_OnEndConnection;
            TeraSniffer.Instance.Enabled = true;
            WindowManager.FloatingButton.NotifyExtended("TCC", "Ready to connect.", NotificationType.Normal);
            SplashScreen.SetText("Starting");
            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.Name = "player";
            SessionManager.CurrentPlayer.EntityId = 10;
            TimeManager.Instance.SetServerTimeZone(Settings.LastRegion);
            ChatWindowManager.Instance.AddTccMessage(_version);
            SplashScreen.CloseWindowSafe();

            UpdateManager.StartCheck();

            DebugStuff();
            Loading = false;
        }
        

        private static void DebugStuff()
        {
            //for (int i = 0; i < 2000; i++)
            //{
            //    ChatWindowManager.Instance.AddTccMessage($"Test {i}");
            //}
            /*
                        EntitiesManager.SpawnNPC(210, 1108, 11, Visibility.Visible);
                        var c = 0;
                        while (c < 1000)
                        {
                            AbnormalityManager.BeginAbnormality(2, 10, 500, 1);
                            AbnormalityManager.BeginAbnormality(2, 11, 500, 1);
                            Console.WriteLine("Added " + c);
                            Thread.Sleep(100);
                            AbnormalityManager.EndAbnormality(2, 10);
                            AbnormalityManager.EndAbnormality(2, 11);
                            Console.WriteLine("Removed " + c);
                            c++;
                        }
            */
            //AbnormalityManager.BeginAbnormality(1495, 10, 10000, 5);
            //AbnormalityManager.BeginAbnormality(2066, 10, 100000, 10);
            //AbnormalityManager.BeginAbnormality(2074, 10, 10000000, 20);
            //var r = new Random();
            //for (int i = 0; i < 30; i++)
            //{
            //    WindowManager.CivilUnrestWindow.VM.AddGuild(new CityWarGuildInfo(1, (uint)i, 0, 0, (float)r.Next(0, 100) / 100));
            //    WindowManager.CivilUnrestWindow.VM.SetGuildName((uint)i, "Guild " + i);
            //    WindowManager.CivilUnrestWindow.VM.AddDestroyedGuildTower((uint)r.Next(0, 29));

            //}

            //ClassWindowViewModel.Instance.CurrentClass = Class.Priest;
            //EntitiesManager.SpawnNPC(920, 3000, 10, Visibility.Visible);
            //Task.Delay(2000).ContinueWith(t => BossGageWindowViewModel.Instance.AddOrUpdateBoss(10,5250000000,3240000000,true, HpChangeSource.BossGage));
            //Task.Delay(4000).ContinueWith(t => BossGageWindowViewModel.Instance.AddOrUpdateBoss(10,5250000000,2240000000,true, HpChangeSource.BossGage));
            //EntitiesManager.SpawnNPC(950,3000,10,Visibility.Visible);
            //EntitiesManager.SpawnNPC(970,1000,11,Visibility.Visible);
            //EntitiesManager.SpawnNPC(970,2000,12,Visibility.Visible);
            //EntitiesManager.SpawnNPC(970,3000,13,Visibility.Visible);
            //EntitiesManager.SetNPCStatus(10, true);

            //Task.Delay(1000).ContinueWith(t => (ClassWindowViewModel.Instance.CurrentManager as WarriorBarManager).DeadlyGamble.Buff.Start(10000));
            //WindowManager.LfgListWindow.ShowWindow();
            // var l = new Listing();
            // l.LeaderId = 10;
            // l.Message = "SJG exp only";
            // l.LeaderName = "Foglio";
            // l.Players.Add(new User(WindowManager.LfgListWindow.Dispatcher){PlayerId = 10, IsLeader = true, Online = true});
            // l.Applicants.Add(new User(WindowManager.LfgListWindow.Dispatcher){PlayerId = 1, Name = "Applicant", Online = true, UserClass = Class.Priest});
            // WindowManager.LfgListWindow.VM.Listings.Add(l);

            //var l = new List<User>();
            //var r = new Random();
            //for (uint i = 0; i < 10; i++)
            //{
            //    var u = new User(GroupWindowViewModel.Instance.GetDispatcher())
            //    {
            //        Name = i.ToString(),
            //        PlayerId = i,
            //        ServerId = i,
            //        EntityId = i,
            //        Online = true,
            //        Laurel = (Laurel)(r.Next(0, 5)),
            //        HasAggro = i == 1,
            //        Alive = i != 0,
            //        UserClass = (Class)r.Next(0, 12),
            //        Awakened = i < 5,
            //    };
            //    GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            //}

            ////GroupWindowViewModel.Instance.SetRaid(true);
            //GroupWindowViewModel.Instance.SetNewLeader(1, "1");
        }

        private static void TeraSniffer_OnNewConnection(Server srv)
        {
            PacketProcessor.Server = srv;
            SkillManager.Clear();
            WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
            ChatWindowManager.Instance.AddTccMessage($"Connected to {srv.Name}.");
            WindowManager.FloatingButton.NotifyExtended("TCC", $"Connected to {srv.Name}", NotificationType.Success);
        }

        private static void TeraSniffer_OnEndConnection()
        {
            ChatWindowManager.Instance.AddTccMessage("Disconnected from the server.");
            WindowManager.FloatingButton.NotifyExtended("TCC", "Disconnected", NotificationType.Warning);

            GroupWindowViewModel.Instance.ClearAllAbnormalities();
            SessionManager.CurrentPlayer.ClearAbnormalities();
            //BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            EntitiesManager.ClearNPC();
            SkillManager.Clear();
            WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
            Proxy.CloseConnection();
        }

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
                // ignored
            }

            TccMessageBox.Show("TCC", "An error occured and TCC will now close. Check error.txt for more info.",
                MessageBoxButton.OK, MessageBoxImage.Error);

            if (Proxy.IsConnected) Proxy.CloseConnection();
            if (WindowManager.TrayIcon != null) WindowManager.TrayIcon.Dispose();
            try
            {
                WindowManager.Dispose();
            }
            catch
            {
                /* ignored*/
            }

            Environment.Exit(-1);
        }

        private static void UploadCrashDump(UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                var js = new JObject();
                var full = ex.Message + "\r\n" +
                           ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" +
                           ex.InnerException +
                           "\r\n" + ex.TargetSite;
                js.Add("tcc_version", new JValue(_version));
                js.Add("full_exception", new JValue(full));
                js.Add("inner_exception",
                    new JValue(ex.InnerException != null ? ex.InnerException.Message : "undefined"));
                js.Add("exception", new JValue(ex.Message));
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
                c.UploadString(new Uri("https://us-central1-tcc-report.cloudfunctions.net/crash"),
                    Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
            }
        }

        private static void InitSplashScreen()
        {
            var waiting = true;
            var ssThread = new Thread(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                    SplashScreen = new SplashScreen();
                    SplashScreen.SetText("Initializing...");
                    SplashScreen.SetVer(_version);
                    SplashScreen.Show();
                    waiting = false;
                    Dispatcher.Run();
                })
            { Name = "SplashScreen window thread" };
            ssThread.SetApartmentState(ApartmentState.STA);
            ssThread.Start();
            while (waiting) Thread.Sleep(1);
        }

        private static void TryDeleteUpdater()
        {
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "/TCCupdater.exe");
            }
            catch
            {
                /* ignored*/
            }
        }

        public static void Restart()
        {
            SettingsWriter.Save();
            Process.Start("TCC.exe");
            CloseApp();
        }

        public static void SendUsageStat()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Headers.Add(HttpRequestHeader.UserAgent,
                    "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                var js = new JObject
                {
                    {"server", PacketProcessor.Server.ServerId},
                    {
                        "id",
                        InfoWindowViewModel.Instance.Characters == null ? 0 :
                        InfoWindowViewModel.Instance.Characters.Count == 0 ? 0 :
                        // ReSharper disable once PossibleNullReferenceException
                        InfoWindowViewModel.Instance.Characters.FirstOrDefault(x => x.Position == 1).Id
                    },
                    {"region", PacketProcessor.Server.Region}
                };
                c.Encoding = Encoding.UTF8;
                c.UploadStringAsync(new Uri("https://us-central1-tcc-report.cloudfunctions.net/stat"),
                    Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));

                Settings.StatSent = true;
                SettingsWriter.Save();
            }
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsWriter.Save();
            WindowManager.Dispose();
            Proxy.CloseConnection();
            UpdateManager.StopTimer();

            Environment.Exit(0);
        }

    }
}