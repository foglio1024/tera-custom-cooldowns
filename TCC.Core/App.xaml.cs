using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Parsing;
using TCC.Settings;
using TCC.Sniffing;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;
using SplashScreen = TCC.Windows.SplashScreen;

namespace TCC
{
    public partial class App
    {
        public const bool Debug = false;
        public static string AppVersion { get; private set; } //"TCC vX.Y.Z"
        public static bool Experimental = true;
        public static SplashScreen SplashScreen;
        public static Dispatcher BaseDispatcher;
        public static string BasePath { get; } = Path.GetDirectoryName(typeof(App).Assembly.Location);
        public static string ResourcesPath { get; } = Path.Combine(BasePath, "resources");
        public static string DataPath { get; } = Path.Combine(ResourcesPath, "data");
        public static bool Loading { get; private set; }
        public static Random Random = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year);

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            Loading = true;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"TCC v{v.Major}.{v.Minor}.{v.Build}{(Experimental ? "-e" : "")}";
            InitSplashScreen();

            BaseDispatcher = Dispatcher.CurrentDispatcher;
            BaseDispatcher.Thread.Name = "Main";
            TccMessageBox.Create(); //Create it here in STA thread
            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            TryDeleteUpdater();

            SplashScreen.SetText("Checking for application updates...");
            UpdateManager.CheckAppVersion();

            SplashScreen.SetText("Checking for database updates...");
            await UpdateManager.CheckIconsVersion();

            SplashScreen.SetText("Loading settings...");
            var sr = new SettingsReader();
            sr.LoadWindowSettings();
            sr.LoadSettings();

            Process.GetCurrentProcess().PriorityClass = SettingsHolder.HighPriority ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
            if (SettingsHolder.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            SplashScreen.SetText("Pre-loading databases...");
            UpdateManager.CheckDatabaseHash();
            SessionManager.InitDatabases(string.IsNullOrEmpty(SettingsHolder.LastLanguage) ? "EU-EN" : SettingsHolder.LastLanguage == "EU" ? "EU-EN" : SettingsHolder.LastLanguage);
            UpdateManager.CheckServersFile();

            SplashScreen.SetText("Initializing windows...");
            WindowManager.Init();

            SplashScreen.SetText("Initializing packet processor...");
            PacketAnalyzer.Init();
            WindowManager.FloatingButton.NotifyExtended("TCC", "Ready to connect.", NotificationType.Normal);
            SplashScreen.SetText("Starting");

            TimeManager.Instance.SetServerTimeZone(SettingsHolder.LastLanguage);
            ChatWindowManager.Instance.AddTccMessage(AppVersion);
            SplashScreen.CloseWindowSafe();

            UpdateManager.StartPeriodicCheck();

            if (!Experimental && SettingsHolder.ExperimentalNotification)
                WindowManager.FloatingButton.NotifyExtended("TCC experimental",
                    "An experimental version of TCC is available. Open System settings to download it or disable this notification.", 
                    NotificationType.Success, 
                    10000);

            if (Debug)
            {
#pragma warning disable CS0162 
                DebugStuff();
#pragma warning restore CS0162
            }
            Loading = false;

        }
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
        public class ThreadInfo
        {
            private double _totalTime;
            private double _diffTime;
            public string Name;
            public int Id;
            public double TotalTime
            {
                get => _totalTime;
                set
                {
                    var old = _totalTime;
                    _totalTime = value;
                    _diffTime = value - old;
                }
            }
            public double DiffTime
            {
                get => _diffTime;
            }
            public ThreadPriority Priority;
        }
        private static void DebugStuff()
        {
            var _t = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            var dispatchers = new List<Dispatcher>
            {
                 App.BaseDispatcher                                ,
                 WindowManager.BossWindow.Dispatcher               ,
                 WindowManager.BuffWindow.Dispatcher               ,
                 WindowManager.CharacterWindow.Dispatcher          ,
                 WindowManager.GroupWindow.Dispatcher              ,
                 WindowManager.CooldownWindow.Dispatcher           ,
                 WindowManager.ClassWindow.Dispatcher              ,
            };
            var threadIdToName = new ConcurrentDictionary<int, string>();
            foreach (var disp in dispatchers)
            {
                disp.Invoke(() =>
                {
                    var myId = GetCurrentThreadId();
                    threadIdToName[myId] = disp.Thread.ManagedThreadId == 1 ? "Main" : disp.Thread.Name;
                });
            }
            threadIdToName[PacketAnalyzer.AnalysisThreadId] = PacketAnalyzer.AnalysisThread.Name;

            var stats = new Dictionary<int, ThreadInfo> { };
            _t.Tick += (_, __) =>
            {
                var p = Process.GetCurrentProcess();
                foreach (ProcessThread th in p.Threads)
                {
                    if (threadIdToName.ContainsKey(th.Id))
                    {
                        if (!stats.ContainsKey(th.Id)) stats.Add(th.Id, new ThreadInfo
                        {
                            Name = threadIdToName[th.Id],
                            Id = th.Id,
                            TotalTime = th.TotalProcessorTime.TotalMilliseconds,
                            Priority = threadIdToName[th.Id] == "Anal" ? PacketAnalyzer.AnalysisThread.Priority : dispatchers.FirstOrDefault(d => d.Thread.Name == threadIdToName[th.Id]).Thread.Priority
                        });
                        else stats[th.Id].TotalTime = th.TotalProcessorTime.TotalMilliseconds;
                    }
                }
                foreach (var item in stats)
                {
                    Console.WriteLine($"{threadIdToName[item.Key]} ({(int)item.Value.Priority}):\t\t{item.Value.TotalTime:0}\t\t{item.Value.DiffTime/1000:P}\t");
                }


                Console.WriteLine("----------------------------------");
            };
            _t.Start();



            //new DebugWindow().Show();
            //var i = 0;
            //while (i < 50)
            //{
            //    ChatWindowManager.Instance.AddTccMessage($"Test {i++}");
            //}

            //var broken =
            //    "00000f00790a0100000000000000000000000000000000000000790a810a00000000810a890a00000000890a910a00000000910a990a00000000990aa10a00000000a10aa90a00000000a90ab10a00000000b10ab90a00000000b90ac10a00000000c10ac90a00000000c90ad10a00000000d10ad90a00000000d90ae10a00000000e10ae90a00000000e90a000000000000f10aba0c0200920b900b414501004a4f7b1000000000e92f0000000000006b0000000000000001000000000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffff4c5a0100000000000000000000000000920b260c0f00ae0b0000000000000000000000000000000000000000ae0bb60b00000000b60bbe0b00000000be0bc60b00000000c60bce0b00000000ce0bd60b00000000d60bde0b00000000de0be60b00000000e60bee0b00000000ee0bf60b00000000f60bfe0b00000000fe0b060c00000000060c0e0c000000000e0c160c00000000160c1e0c000000001e0c000000000000260c00000f00420c0100000000000000000000000000000000000000420c4a0c000000004a0c520c00000000520c5a0c000000005a0c620c00000000620c6a0c000000006a0c720c00000000720c7a0c000000007a0c820c00000000820c8a0c000000008a0c920c00000000920c9a0c000000009a0ca20c00000000a20caa0c00000000aa0cb20c00000000b20c000000000000ba0c830e02005b0d590d424501004b4f7b1000000000e92f0000000000006c0000000000000001000000000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffff4c5a01000000000000000000000000005b0def0d0f00770d0000000000000000000000000000000000000000770d7f0d000000007f0d870d00000000870d8f0d000000008f0d970d00000000970d9f0d000000009f0da70d00000000a70daf0d00000000af0db70d00000000b70dbf0d00000000bf0dc70d00000000c70dcf0d00000000cf0dd70d00000000d70ddf0d00000000df0de70d00000000e70d000000000000ef0d00000f000b0e01000000000000000000000000000000000000000b0e130e00000000130e1b0e000000001b0e230e00000000230e2b0e000000002b0e330e00000000330e3b0e000000003b0e430e00000000430e4b0e000000004b0e530e00000000530e5b0e000000005b0e630e00000000630e6b0e000000006b0e730e00000000730e7b0e000000007b0e000000000000830e4c100200240f220f5b4501004c4f7b1000000000e92f0000000000006d0000000000000001000000000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffff4c5a0100000000000000000000000000240fb80f0f00400f0000000000000000000000000000000000000000400f480f00000000480f500f00000000500f580f00000000580f600f00000000600f680f00000000680f700f00000000700f780f00000000780f800f00000000800f880f00000000880f900f00000000900f980f00000000980fa00f00000000a00fa80f00000000a80fb00f00000000b00f000000000000b80f00000f00d40f0100000000000000000000000000000000000000d40fdc0f00000000dc0fe40f00000000e40fec0f00000000ec0ff40f00000000f40ffc0f00000000fc0f04100000000004100c10000000000c1014100000000014101c10000000001c1024100000000024102c10000000002c1034100000000034103c10000000003c1044100000000044100000000000004c1015120200ed10eb10104501004d4f7b1000000000e92f0000000000006a0000000000000001000000000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffff4c5a0100000000000000000000000000ed1081110f0009110000000000000000000000000000000000000000091111110000000011111911000000001911211100000000211129110000000029113111000000003111391100000000391141110000000041114911000000004911511100000000511159110000000059116111000000006111691100000000691171110000000071117911000000007911000000000000811100000f009d1101000000000000000000000000000000000000009d11a51100000000a511ad1100000000ad11b51100000000b511bd1100000000bd11c51100000000c511cd1100000000cd11d51100000000d511dd1100000000dd11e51100000000e511ed1100000000ed11f51100000000f511fd1100000000fd1105120000000005120d12000000000d120000000000001512de130200b612b412920500004e4f7b1000000000e92f0000000000005e0000000000000004000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffff00000000000000000000000000000000b6124a130f00d2120000000000000000000000000000000000000000d212da1200000000da12e21200000000e212ea1200000000ea12f21200000000f212fa1200000000fa1202130000000002130a13000000000a1312130000000012131a13000000001a1322130000000022132a13000000002a1332130000000032133a13000000003a1342130000000042130000000000004a1300000f006613010000000000000000000000000000000000000066136e13000000006e1376130000000076137e13000000007e1386130000000086138e13000000008e1396130000000096139e13000000009e13a61300000000a613ae1300000000ae13b61300000000b613be1300000000be13c61300000000c613ce1300000000ce13d61300000000d613000000000000de13a71502007f147d149cd902005d6a7f1000000000e92f0000000000003e0000000000000009000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffffffff000000000000000000000000000000007f1413150f009b1400000000000000000000000000000000000000009b14a31400000000a314ab1400000000ab14b31400000000b314bb140000000000";
            //var msg = new Message(DateTime.Now, MessageDirection.ServerToClient,
            //    new ArraySegment<byte>(StringUtils.StringToByteArray(broken)));
            //var fac = new MessageFactory();
            //var del = MessageFactory.Contructor<Func<TeraMessageReader, S_INVEN>>();
            //var reader = new TeraMessageReader(msg, null, fac, null);
            //del.DynamicInvoke(reader);



            //new DebugWindow().Show();
            //SessionManager.Logged = true;
            //SessionManager.LoadingScreen = false;
            //SessionManager.CurrentPlayer.Class = Class.Warrior;
            //WindowManager.ClassWindow.VM.CurrentClass = SessionManager.CurrentPlayer.Class;

            //SessionManager.Combat = true;
            //SessionManager.Encounter = true;
            //(WindowManager.ClassWindow.VM.CurrentManager as WarriorBarManager).DeadlyGamble.Cooldown.Start(2000);
            //System.Timers.Timer _t = new System.Timers.Timer { Interval = 1000 };
            //var r = new Random();
            //_t.Elapsed += (_, __) =>
            //{
            //    ChatWindowManager.Instance.AddTccMessage("Long Random message which I'm using to test multi thread performance in TCC. Now only cooldown window runs on its own UI thread and should not show stutter when adding this kind of messages. " + r.Next(200));
            //    //SessionManager.SetPlayerSt(10, SessionManager.CurrentPlayer.CurrentST + 100 > SessionManager.CurrentPlayer.MaxST ?
            //    //    0 : SessionManager.CurrentPlayer.CurrentST + 100);
            //};
            //WindowManager.ClassWindow.VM.CurrentClass = SessionManager.CurrentPlayer.Class;
            //CooldownWindowViewModel.Instance.LoadSkills(Utils.ClassEnumToString(SessionManager.CurrentPlayer.Class).ToLower() + "-skills.xml", SessionManager.CurrentPlayer.Class);
            ////SessionManager.SetSorcererElements(true, true, true);
            //SessionManager.SetPlayerMaxSt(10, 1000);
            //SessionManager.SetPlayerSt(10, 1000);

            //_t.Start();
            //var i = 0;
            //while (i < 20000)
            //{
            //    ChatWindowManager.Instance.AddTccMessage($"Test {i++}");
            //}
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            WindowManager.GroupWindow.VM.AddOrUpdateMember(new User(WindowManager.GroupWindow.VM.GetDispatcher())
            {
                Alive = true,
                Awakened = true,
                CurrentHp = 0,
                MaxHp = 1000,
                EntityId = 1,
                ServerId = 1,
                PlayerId = 1,
                UserClass = Class.Archer,
                Online = true,
                HasAggro = true
            });
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

            //var t = new System.Timers.Timer() { Interval = 1000 };
            //EntityManager.SpawnNPC(920, 3000, 11, true, false);
            //var val = 1000;
            //t.Elapsed += (_, __) =>
            //{
            //    val -= 10;
            //    if (val <= 900) val = 1000;
            //    EntityManager.UpdateNPC(11, val, 1000);
            //};
            //t.Start();

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

            //WindowManager.ClassWindow.VM.CurrentClass = Class.Priest;
            //EntityManager.SpawnNPC(920, 3000, 10, Visibility.Visible);
            //Task.Delay(2000).ContinueWith(t => WindowManager.BossWindow.VM.AddOrUpdateBoss(10,5250000000,3240000000,true, HpChangeSource.BossGage));
            //Task.Delay(4000).ContinueWith(t => WindowManager.BossWindow.VM.AddOrUpdateBoss(10,5250000000,2240000000,true, HpChangeSource.BossGage));
            //EntityManager.SpawnNPC(950,3000,10,Visibility.Visible);
            //EntityManager.SpawnNPC(970,1000,11,Visibility.Visible);
            //EntityManager.SpawnNPC(970,2000,12,Visibility.Visible);
            //EntityManager.SpawnNPC(970,3000,13,Visibility.Visible);
            //EntityManager.SetNPCStatus(10, true);

            //Task.Delay(1000).ContinueWith(t => (WindowManager.ClassWindow.VM.CurrentManager as WarriorBarManager).DeadlyGamble.Buff.Start(10000));
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
            //    var u = new User(WindowManager.GroupWindow.VM.GetDispatcher())
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
            //    WindowManager.GroupWindow.VM.AddOrUpdateMember(u);
            //}

            ////WindowManager.GroupWindow.VM.SetRaid(true);
            //WindowManager.GroupWindow.VM.SetNewLeader(1, "1");
        }

        private static void _t_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var sb = new StringBuilder("\r\n\r\n");
            sb.AppendLine($"##### {AppVersion} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} #####");
            sb.Append($"Version: {PacketAnalyzer.Factory.Version}");
            if (SessionManager.Server != null)
            {
                sb.Append($" - Region: {SessionManager.Server.Region}");
                sb.Append($" - Server:{SessionManager.Server.ServerId}");
            }
            sb.AppendLine();
            sb.AppendLine($"{ex.Message}");
            sb.AppendLine($"{ex.StackTrace}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"Data: {ex.Data}");
            if (ex.InnerException != null) sb.AppendLine($"InnerException: \n{ex.InnerException}");
            sb.AppendLine($"TargetSite: {ex.TargetSite}");
            File.AppendAllText(Path.GetDirectoryName(typeof(App).Assembly.Location) + "/crash.log", sb.ToString());
#if !DEBUG

            try
            {
                var t = new Thread(() => UploadCrashDump(e));
                t.Start();
            }
            catch (Exception)
            {
                // ignored
            }
#endif
            TccMessageBox.Show("TCC", "An error occured and TCC will now close. Report this issue to the developer attaching crash.log from TCC folder.",
                MessageBoxButton.OK, MessageBoxImage.Error);

            if (Proxy.Proxy.IsConnected) Proxy.Proxy.CloseConnection();
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
                js.Add("tcc_version", new JValue(AppVersion));
                js.Add("full_exception", new JValue(full));
                js.Add("inner_exception",
                    new JValue(ex.InnerException != null ? ex.InnerException.Message : "undefined"));
                js.Add("exception", new JValue(ex.Message));
                js.Add("game_version", new JValue(PacketAnalyzer.Factory.Version));
                if (SessionManager.Server != null)
                {
                    js.Add("region", new JValue(SessionManager.Server.Region));
                    js.Add("server_id", new JValue(SessionManager.Server.ServerId));
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
                    SplashScreen.SetVer(AppVersion);
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
                File.Delete(Path.Combine(BasePath, "TCCupdater.exe"));
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
                    {"server", SessionManager.Server.ServerId},
                    {
                        "id",
                        WindowManager.Dashboard.VM.Characters == null ? 0 :
                        WindowManager.Dashboard.VM.Characters.Count == 0 ? 0 :
                        WindowManager.Dashboard.VM.Characters.FirstOrDefault(x => x.Position == 1)?.Id
                    },
                    {"region", SessionManager.Server.Region}
                };
                c.Encoding = Encoding.UTF8;
                c.UploadStringAsync(new Uri("https://us-central1-tcc-report.cloudfunctions.net/stat"),
                    Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));

                SettingsHolder.StatSent = true;
                SettingsWriter.Save();
            }
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsWriter.Save();
            WindowManager.Dispose();
            Proxy.Proxy.CloseConnection();
            UpdateManager.StopTimer();

            Environment.Exit(0);
        }

    }
}