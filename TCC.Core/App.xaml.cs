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
using TCC.Settings;
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

        // ReSharper disable once InconsistentNaming
        public const string ThankYou_mEME =
            "Due to the recent events regarding EME's DMCA takedowns of proxy related repositories, TCC will stop to be supported for NA, meaning that all data required to make it work after patch won't be released for this region. Sorry for this and thanks for all your support.";
        public static bool Loading { get; private set; }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            Loading = true;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            _version = $"TCC v{v.Major}.{v.Minor}.{v.Build}";
            InitSplashScreen();

            BaseDispatcher = Dispatcher.CurrentDispatcher;
            TccMessageBox.Create(); //Create it here in STA thread
#if !DEBUG
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

            Process.GetCurrentProcess().PriorityClass = Settings.Settings.HighPriority ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            if (Settings.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            SplashScreen.SetText("Pre-loading databases...");
            SessionManager.InitDatabases(string.IsNullOrEmpty(Settings.Settings.LastRegion) ? "EU-EN" : Settings.Settings.LastRegion == "EU" ? "EU-EN" : Settings.Settings.LastRegion);

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
            TimeManager.Instance.SetServerTimeZone(Settings.Settings.LastRegion);
            ChatWindowManager.Instance.AddTccMessage(_version);
            SplashScreen.CloseWindowSafe();

            UpdateManager.StartCheck();

            if (Settings.Settings.LastRegion == "NA" || Settings.Settings.LastRegion == "")
                WindowManager.FloatingButton.NotifyExtended("So long, and thanks for all the fish", ThankYou_mEME, NotificationType.Error, 15000);
            if (Debug) DebugStuff();
            Loading = false;
        }

        private static System.Timers.Timer _t;
        private static void DebugStuff()
        {
            new DebugWindow().Show();
            SessionManager.Logged = true;
            SessionManager.LoadingScreen = false;
            _t = new System.Timers.Timer { Interval = 1000 };
            _t.Elapsed += (_, __) =>
            {
                SessionManager.SetPlayerSt(10, SessionManager.CurrentPlayer.CurrentST + 100 > SessionManager.CurrentPlayer.MaxST ?
                    0 : SessionManager.CurrentPlayer.CurrentST + 100);
            };
            SessionManager.CurrentPlayer.Class = Class.Warrior;
            ClassWindowViewModel.Instance.CurrentClass = SessionManager.CurrentPlayer.Class;
            CooldownWindowViewModel.Instance.LoadSkills(Utils.ClassEnumToString(SessionManager.CurrentPlayer.Class).ToLower() + "-skills.xml", SessionManager.CurrentPlayer.Class);
            //SessionManager.SetSorcererElements(true, true, true);
            SessionManager.SetPlayerMaxSt(10, 1000);
            SessionManager.SetPlayerSt(10, 1000);

            //_t.Start();
            //var i = 0;
            //while (i < 20000)
            //{
            //    ChatWindowManager.Instance.AddTccMessage($"Test {i++}");
            //}
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GroupWindowViewModel.Instance.AddOrUpdateMember(new User(BaseDispatcher)
            //{
            //    Alive = true,
            //    Awakened = true,
            //    CurrentHp = 1000,
            //    MaxHp = 1000,
            //    EntityId = 1,
            //    ServerId = 1,
            //    PlayerId = 1,
            //    UserClass = Class.Archer,
            //    Online = true
            //});
            //bool up = true;
            //bool inv = false;
            //ulong i = 0;
            //while (true)
            //{
            //    if (i > 1000)
            //    {
            //        up = false;
            //        if(!inv) Thread.Sleep(10000);
            //        inv = true;
            //    }

            //    if (up)
            //    {

            //        Console.WriteLine($"[{i}] Spawning NPCs");
            //        EntityManager.SpawnNPC(15, 1, i, Visibility.Visible);
            //        i++;
            //    }
            //    else
            //    {
            //        Console.WriteLine($"[{i}] Despawning NPCs");
            //        EntityManager.DespawnNPC(i, DespawnType.OutOfView);
            //        i--;
            //    }
            //    Thread.Sleep(2);
            //    if (i == 0) {break;}
            //}
            //EntityManager.SpawnNPC(920, 3000, 11, Visibility.Visible);
            //EntityManager.UpdateNPC(12, 1000, 1000);
            //AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(1, 1, 1495, 200000, 1);
            //AbnormalityManager.BeginAbnormality(1495, 10, 200000, 1);
            //AbnormalityManager.BeginAbnormality(1495, 11, 200000, 1);
            //AbnormalityManager.BeginAbnormality(1495, 12, 200000, 1);

            //for (int i = 0; i < 2000; i++)
            //{
            //    ChatWindowManager.Instance.AddTccMessage($"Test {i}");
            //}
            /*
                        EntityManager.SpawnNPC(210, 1108, 11, Visibility.Visible);
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
            //EntityManager.SpawnNPC(920, 3000, 10, Visibility.Visible);
            //Task.Delay(2000).ContinueWith(t => BossGageWindowViewModel.Instance.AddOrUpdateBoss(10,5250000000,3240000000,true, HpChangeSource.BossGage));
            //Task.Delay(4000).ContinueWith(t => BossGageWindowViewModel.Instance.AddOrUpdateBoss(10,5250000000,2240000000,true, HpChangeSource.BossGage));
            //EntityManager.SpawnNPC(950,3000,10,Visibility.Visible);
            //EntityManager.SpawnNPC(970,1000,11,Visibility.Visible);
            //EntityManager.SpawnNPC(970,2000,12,Visibility.Visible);
            //EntityManager.SpawnNPC(970,3000,13,Visibility.Visible);
            //EntityManager.SetNPCStatus(10, true);

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
            //for (uint i = 0; i <= 10; i++)
            //{
            //    var u = new User(GroupWindowViewModel.Instance.GetDispatcher())
            //    {
            //        Name = i.ToString(),
            //        PlayerId = i,
            //        ServerId = i,
            //        EntityId = i,
            //        Online = true,
            //        Laurel = (Laurel)(r.Next(0, 6)),
            //        HasAggro = i == 1,
            //        Alive = true, //i != 0,
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
            EntityManager.ClearNPC();
            SkillManager.Clear();
            WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
            Proxy.CloseConnection();
            SessionManager.Logged = false;
            SessionManager.LoadingScreen = true;
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var sb = new StringBuilder("\r\n\r\n");
            sb.AppendLine($"##### {_version} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} #####");
            sb.AppendLine($"Version: {PacketProcessor.Version}");
            if (PacketProcessor.Server != null)
            {
                sb.Append($" - Region: {PacketProcessor.Server.Region}");
                sb.Append($" - Server:{PacketProcessor.Server.ServerId}");
            }

            sb.AppendLine($"{ex.Message}");
            sb.AppendLine($"{ex.StackTrace}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"Data: {ex.Data}");
            if(ex.InnerException != null) sb.AppendLine($"InnerException: \n{ex.InnerException}");
            sb.AppendLine($"TargetSite: {ex.TargetSite}");
            File.AppendAllText(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/error.txt", sb.ToString());
            try
            {
                var t = new Thread(() => UploadCrashDump(e));
                t.Start();
            }
            catch (Exception)
            {
                // ignored
            }

            TccMessageBox.Show("TCC", "An error occured and TCC will now close. Report this issue to the developer attaching error.txt from TCC folder.",
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
                File.Delete(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/TCCupdater.exe");
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

                Settings.Settings.StatSent = true;
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