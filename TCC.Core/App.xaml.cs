using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
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
    /// <summary>
    ///     Logica di interazione per App.xaml
    /// </summary>
    public partial class App
    {
        private static string _version;
        public const bool Debug = false;
        public static SplashScreen SplashScreen;
        public static Dispatcher BaseDispatcher;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            _version = $"TCC v{v.Major}.{v.Minor}.{v.Build}";
            //new Task(() => { if (new Firebase.Firebase().CheckService()) Console.WriteLine("Firebase ok");}).Start(); 
            InitSplashScreen();

            BaseDispatcher = Dispatcher.CurrentDispatcher;
            TccMessageBox.Create(); //Create it here in STA thread

            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            TryDeleteUpdater();

            SplashScreen.SetText("Checking for application updates...");
            UpdateManager.CheckAppVersion();

            SplashScreen.SetText("Checking for database updates...");
            UpdateManager.CheckDatabaseVersion();

            SplashScreen.SetText("Loading settings...");
            SettingsManager.LoadWindowSettings();
            SettingsManager.LoadSettings();

            SplashScreen.SetText("Pre-loading databases...");
            SessionManager.InitDatabases(string.IsNullOrEmpty(SettingsManager.LastRegion) ? "EU-EN" : SettingsManager.LastRegion == "EU" ? "EU-EN" : SettingsManager.LastRegion);

            SplashScreen.SetText("Initializing windows...");
            WindowManager.Init();

            SplashScreen.SetText("Initializing Twitch connector...");
            //TwitchConnector.Instance.Init();

            SplashScreen.SetText("Initializing packet processor...");
            PacketProcessor.Init();
            TeraSniffer.Instance.NewConnection += TeraSniffer_OnNewConnection;
            TeraSniffer.Instance.EndConnection += TeraSniffer_OnEndConnection;
            TeraSniffer.Instance.Enabled = true;

            SplashScreen.SetText("Starting");
            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.Name = "player";
            SessionManager.CurrentPlayer.PlayerId = 10;
            TimeManager.Instance.SetServerTimeZone(SettingsManager.LastRegion);
            ChatWindowManager.Instance.AddTccMessage(_version);
            SplashScreen.CloseWindowSafe();

            UpdateManager.StartCheck();
            ClassWindowViewModel.Instance.CurrentClass = Class.Warrior;
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
            //for (uint i = 0; i < 3; i++)
            //{
            //    var u = new User(GroupWindowViewModel.Instance.GetDispatcher())
            //    {
            //        Name = i.ToString(),
            //        PlayerId = i,
            //        ServerId = i,
            //        EntityId = i,
            //        Online = true,
            //        Laurel = (Laurel)(r.Next(0,5)),
            //        HasAggro = i == 1,
            //        Alive = i != 0,
            //        UserClass = (Class)r.Next(0, 12)
            //    };
            //    GroupWindowViewModel.Instance.AddOrUpdateMember(u);
            //}

            //GroupWindowViewModel.Instance.SetRaid(true);
            //GroupWindowViewModel.Instance.SetNewLeader(10, "player");
        }

        private static void TeraSniffer_OnNewConnection(Server srv)
        {
            PacketProcessor.Server = srv;
            SkillManager.Clear();
            WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
            ChatWindowManager.Instance.AddTccMessage($"Connected to {srv.Name}.");
            WindowManager.FloatingButton.NotifyExtended($"TCC", $"Connected to {srv.Name}", NotificationType.Success);
        }

        private static void TeraSniffer_OnEndConnection()
        {
            ChatWindowManager.Instance.AddTccMessage("Disconnected from the server.");
            WindowManager.FloatingButton.NotifyExtended($"TCC", "Disconnected", NotificationType.Warning);

            GroupWindowViewModel.Instance.ClearAllAbnormalities();
            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
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
            SettingsManager.SaveSettings();
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

                SettingsManager.StatSent = true;
                SettingsManager.SaveSettings();
            }
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsManager.SaveSettings();
            WindowManager.Dispose();
            Proxy.CloseConnection();
            UpdateManager.StopTimer();

            Environment.Exit(0);
        }

        // ------------------------------ Handlers for controls defined in App.xaml ------------------------------ //

        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = false;
        }

        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = true;
        }
    }
}