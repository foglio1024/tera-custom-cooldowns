using Nostrum;
using Nostrum.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Exceptions;
using TCC.Interop.Proxy;
using TCC.Analysis;
using TCC.Settings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Update;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;

namespace TCC
{
    public partial class App
    {
        private static bool _restarted;
        private static bool _running;
        private static Mutex _mutex;

        public static bool Beta { get; } = true;

        /// <summary>
        ///     Version in the "TCC vX.Y.Z-e" format.
        /// </summary>
        public static string AppVersion { get; private set; }

        /// <summary>
        ///     'TCC.exe' folder
        /// </summary>
        public static string BasePath { get; private set; } = Path.GetDirectoryName(typeof(App).Assembly.Location);

        /// <summary>
        ///     'TCC/resources' folder
        /// </summary>
        public static string ResourcesPath { get; private set; } = Path.Combine(BasePath, "resources");

        /// <summary>
        ///     'TCC/resources/data' folder
        /// </summary>
        public static string DataPath { get; private set; } = Path.Combine(ResourcesPath, "data");

        public static bool Loading { get; private set; }
        public static bool ToolboxMode { get; private set; }
        public static Random Random { get; } = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year +
                                                          DateTime.Now.Minute + DateTime.Now.Second +
                                                          DateTime.Now.Millisecond);
        public static TccSplashScreen SplashScreen { get; set; }
        public static SettingsContainer Settings { get; set; }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _running = true;
            AppVersion = TccUtils.GetTccVersion();
            Log.Config(Path.Combine(BasePath, "logs"), AppVersion); // NLog when?
            ParseStartupArgs(e.Args.ToList());
            BaseDispatcher = Dispatcher.CurrentDispatcher;
            BaseDispatcher.Thread.Name = "Main";
            TccMessageBox.CreateAsync();
            if (IsAlreadyRunning() && !Debugger.IsAttached)
            {
                if (!ToolboxMode) TccMessageBox.Show("Another instance of TCC is already running. Shutting down.", MessageBoxType.Information);
                Current.Shutdown();
                return;
            }
            
            if (!Debugger.IsAttached) AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.HandleGlobalException;

            Loading = true;
            await Setup();
            Loading = false;
        }

        private static async Task Setup()
        {
            TccSplashScreen.InitOnNewThread();

            if (!ToolboxMode)
            {
                UpdateManager.TryDeleteUpdater();

                SplashScreen.VM.BottomText = "Checking for application updates...";
                await UpdateManager.CheckAppVersion();
            }

            // ----------------------------
            SplashScreen.VM.Progress = 10;
            SplashScreen.VM.BottomText = "Loading settings...";
            SettingsContainer.Load();
            WindowManager.InitSettingsWindow(); // need it in case language is not correct

            SplashScreen.VM.Progress = 20;
            Process.GetCurrentProcess().PriorityClass = Settings.HighPriority
                ? ProcessPriorityClass.High
                : ProcessPriorityClass.Normal;
            if (Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            // ----------------------------
            SplashScreen.VM.Progress = 30;
            SplashScreen.VM.BottomText = "Pre-loading databases...";
            UpdateManager.CheckDatabaseHash();
            SplashScreen.VM.Progress = 40;
            await Game.InitAsync();

            // ----------------------------
            SplashScreen.VM.Progress = 50;
            SplashScreen.VM.BottomText = "Initializing widgets...";
            RunningDispatchers = new ConcurrentDictionary<int, Dispatcher>();
            await WindowManager.Init();
            SplashScreen.VM.Progress = 60;
            StartDispatcherWatcher();

            // ----------------------------
            SplashScreen.VM.Progress = 70;
            SplashScreen.VM.BottomText = "Checking for icon database updates...";
            _ = Task.Run(() => new IconsUpdater().CheckForUpdates());

            // ----------------------------
            SplashScreen.VM.BottomText = "Initializing packet processor...";
            SplashScreen.VM.Progress = 80;
            await PacketAnalyzer.InitAsync();

            // ----------------------------
            SplashScreen.VM.Progress = 90;
            SplashScreen.VM.BottomText = "Starting";
            GameEventManager.Instance.SetServerTimeZone(Settings.LastLanguage);
            UpdateManager.StartPeriodicCheck();

            SplashScreen.VM.Progress = 100;
            SplashScreen.CloseWindowSafe();

            // ----------------------------
            ChatManager.Instance.AddTccMessage($"{AppVersion} ready.");

            if (!Beta && Settings.BetaNotification && UpdateManager.IsExperimentalNewer())
                Log.N("TCC beta", "A beta version of TCC is available. Open System settings to download it or to disable this notification.",
                    NotificationType.Success,
                    10000);
        }
        private static void ParseStartupArgs(IList<string> args)
        {
            // --toolbox
            ToolboxMode = args.IndexOf("--toolbox") != -1;

            // --restart
            _restarted = args.IndexOf("--restart") != -1;

            // --settings_override 'path'
            var settingsOverrideIdx = args.IndexOf("--settings_override");
            if (settingsOverrideIdx != -1) 
                SettingsContainer.SettingsOverride = args[settingsOverrideIdx + 1];

            // --root_override 'path'
            var rootOverrideIdx = args.IndexOf("--root_override");
            if (rootOverrideIdx != -1)
            {
                BasePath = args[rootOverrideIdx + 1];
                ResourcesPath = Path.Combine(BasePath, "resources");
                DataPath = Path.Combine(ResourcesPath, "data");
            }
        }
        public static void Restart()
        {
            Settings.Save();
            Process.Start("TCC.exe", $"--restart{(ToolboxMode ? " --toolbox" : "")}");
            Close();
        }
        public static void Close(bool releaseMutex = true)
        {
            _running = false;
            if (releaseMutex) BaseDispatcher.Invoke(ReleaseMutex);
            PacketAnalyzer.Sniffer.Enabled = false;
            Settings.Save();
            WindowManager.Dispose();
            StubInterface.Instance.Disconnect();
            UpdateManager.StopTimer();
            Environment.Exit(0);
        }
        private static bool IsAlreadyRunning()
        {
            _mutex = new Mutex(true, nameof(TCC), out var createdNew);
            if (createdNew || !_restarted) return !createdNew;
            _mutex.WaitOne();
            return false;
        }
        public static void ReleaseMutex()
        {
            _running = false;
            BaseDispatcher.Invoke(() => _mutex.ReleaseMutex());
        }

        #region Dispatchers
        public static Dispatcher BaseDispatcher { get; private set; }

        public static ConcurrentDictionary<int, Dispatcher> RunningDispatchers { get; private set; }

        public static void AddDispatcher(int threadId, Dispatcher d)
        {
            RunningDispatchers[threadId] = d;
        }

        public static void RemoveDispatcher(int threadId)
        {
            RunningDispatchers.TryRemove(threadId, out _);
        }

        public static void WaitDispatchersShutdown()
        {
            if (RunningDispatchers == null) return;
            var tries = 50;
            while (tries > 0)
            {
                if (RunningDispatchers.Count == 0) break;
                Log.CW($"Waiting all dispatcher to shutdown... ({RunningDispatchers.Count} left)");
                Thread.Sleep(100);
                tries--;
            }

            Log.CW("All dispatchers shut down.");
        }

        private static void StartDispatcherWatcher()
        {
            new Thread(() =>
                {
                    while (_running)
                    {
                        var deadlockedDispatchers = new List<Dispatcher>();
                        try
                        {
                            Parallel.ForEach(RunningDispatchers.Values.Append(BaseDispatcher), v =>
                            {
                                if (v.IsAlive(60000).Result) return;
                                Log.CW($"{v.Thread.Name} didn't respond in time!");
                                deadlockedDispatchers.Add(v);
                            });
                            Thread.Sleep(1000);
                        }
                        catch
                        {
                        }

                        if (deadlockedDispatchers.Count > 1)
                            throw new DeadlockException($"The following threads didn't report in time: {deadlockedDispatchers.Select(d => d.Thread.Name).ToList().ToCSV()}");
                        //Log.F($"The following threads didn't report in time: {deadlockedDispatchers.Select(d => d.Thread.Name).ToList().ToCSV()}");
                    }
                })
            { Name = "Watcher" }.Start();
        }
        #endregion

        #region Misc

        private static FUBH _fubh;

        public static bool FI { get; } = DateTime.Now >= TimeUtils.FromUnixTime(1567123200) &&
                                         DateTime.Now < TimeUtils.FromUnixTime(1567209600);

        public static void FUBH()
        {
            BaseDispatcher.InvokeAsync(() =>
            {
                if (_fubh == null) _fubh = new FUBH();
                _fubh.Show();
            });
        }

        #endregion
    }
}