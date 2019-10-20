using FoglioUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using FoglioUtils.Extensions;
using TCC.Data;
using TCC.Interop.Proxy;
using TCC.Loader;
using TCC.Parsing;
using TCC.Settings;
using TCC.Test;
using TCC.Utils;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC
{
    public partial class App
    {
        private static bool _restarted;
        private static bool _running;
        private static Mutex _mutex;

        public static bool Experimental { get; } = true;

        public static Dispatcher BaseDispatcher { get; private set; }
        /// <summary>
        /// Version in the "TCC vX.Y.Z" format.
        /// </summary>
        public static string AppVersion { get; private set; } //"TCC vX.Y.Z"
        /// <summary>
        /// 'TCC.exe' folder
        /// </summary>
        public static string BasePath { get; } = Path.GetDirectoryName(typeof(App).Assembly.Location);
        /// <summary>
        /// 'TCC/resources' folder
        /// </summary>
        public static string ResourcesPath { get; } = Path.Combine(BasePath, "resources");
        /// <summary>
        /// 'TCC/resources/data' folder
        /// </summary>
        public static string DataPath { get; } = Path.Combine(ResourcesPath, "data");
        public static bool Loading { get; private set; }
        public static bool ToolboxMode { get; private set; }
        public static Random Random { get; } = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
        public static TccSplashScreen SplashScreen { get; set; }
        public static SettingsContainer Settings { get; set; }


        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _running = true;
            Log.Config(Path.Combine(BasePath, "logs"), AppVersion); // NLog when?
            ParseStartupArgs(e.Args.ToList());
            BaseDispatcher = Dispatcher.CurrentDispatcher;
            BaseDispatcher.Thread.Name = "Main";
            TccMessageBox.CreateAsync(); 

            if (IsAlreadyRunning())
            {
                if (!ToolboxMode) TccMessageBox.Show("Another instance of TCC is already running. Shutting down.", MessageBoxType.Information);
                Current.Shutdown();
                return;
            }

            Loading = true;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"TCC v{v.Major}.{v.Minor}.{v.Build}{(Experimental ? "-e" : "")}";
            TccSplashScreen.InitOnNewThread();
#if RELEASE
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.HandleGlobalException;
#endif

            if (!ToolboxMode)
            {
                UpdateManager.TryDeleteUpdater();

                SplashScreen.VM.BottomText = "Checking for application updates...";
                await UpdateManager.CheckAppVersion();
            }

            SplashScreen.VM.Progress = 10;
            SplashScreen.VM.BottomText = "Checking for icon database updates...";
            await UpdateManager.CheckIconsVersion();

            SplashScreen.VM.Progress = 20;
            SplashScreen.VM.BottomText = "Loading settings...";

            WindowManager.VisibilityManager = new VisibilityManager();

            SettingsContainer.Load();

            SplashScreen.VM.Progress = 30;

            Process.GetCurrentProcess().PriorityClass = Settings.HighPriority
                ? ProcessPriorityClass.High
                : ProcessPriorityClass.Normal;
            if (Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            SplashScreen.VM.BottomText = "Pre-loading databases...";
            UpdateManager.CheckDatabaseHash();

            SplashScreen.VM.Progress = 50;

            await Game.InitAsync();

            SplashScreen.VM.Progress = 70;
            SplashScreen.VM.BottomText = "Initializing widgets...";


            RunningDispatchers = new ConcurrentDictionary<int, Dispatcher>();
            await WindowManager.Init();
            StartDispatcherWatcher();

            SplashScreen.VM.BottomText = "Initializing packet processor...";
            SplashScreen.VM.Progress = 80;

            PacketAnalyzer.ProcessorReady += () => BaseDispatcher.Invoke(() =>
            {
                try
                {
                    ModuleLoader.LoadModules(BasePath);
                }
                catch (FileLoadException fle)
                {
                    TccMessageBox.Show("TCC module loader",
                        $"An error occured while loading {fle.FileName}. TCC will now close. You can find more info about this error in TERA Dps discord #known-issues channel.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                catch (FileNotFoundException fnfe)
                {
                    TccMessageBox.Show("TCC module loader",
                        $"An error occured while loading {Path.GetFileName(fnfe.FileName)}. TCC will now close. You can find more info about this error in TERA Dps discord #known-issues channel.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            });
            await PacketAnalyzer.InitAsync();
            SplashScreen.VM.Progress = 100;
            SplashScreen.VM.BottomText = "Starting";

            TimeManager.Instance.SetServerTimeZone(Settings.LastLanguage);
            ChatWindowManager.Instance.AddTccMessage(AppVersion);

            UpdateManager.StartPeriodicCheck();

            if (!Experimental && Settings.ExperimentalNotification && UpdateManager.IsExperimentalNewer())
                WindowManager.ViewModels.NotificationAreaVM.Enqueue("TCC experimental",
                    "An experimental version of TCC is available. Open System settings to download it or disable this notification.",
                    NotificationType.Success,
                    10000);

            SplashScreen.CloseWindowSafe();
            Loading = false;
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
            {
                SettingsContainer.SettingsOverride = args[settingsOverrideIdx + 1];
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
            ProxyInterface.Instance.Disconnect();
            UpdateManager.StopTimer();
            Environment.Exit(0);
        }

        private static bool IsAlreadyRunning()
        {
            _mutex = new Mutex(true, "TCC", out var createdNew);
            if (createdNew || !_restarted) return !createdNew;
            _mutex.WaitOne();
            return false;
        }
        public static void ReleaseMutex()
        {
            _running = false;
            _mutex.ReleaseMutex();
        }

        public static ConcurrentDictionary<int, Dispatcher> RunningDispatchers { get; private set; }
        public static void AddDispatcher(int threadId, Dispatcher d)
        {
            App.RunningDispatchers[threadId] = d;
        }
        public static void RemoveDispatcher(int threadId)
        {
            App.RunningDispatchers.TryRemove(threadId, out _);
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
            var t = new Thread(() =>
            {
                while (_running)
                {
                    var deadlockedDispatchers = new List<Dispatcher>();
                    try
                    {
                        Parallel.ForEach(Enumerable.Append(RunningDispatchers.Values, App.BaseDispatcher), (v) =>
                        {
                            if (v.IsAlive(60000).Result) return;
                            Log.CW($"{v.Thread.Name} didn't respond in time!");
                            deadlockedDispatchers.Add(v);
                        });
                        Thread.Sleep(1000);
                    }
                    catch { }
                    if (deadlockedDispatchers.Count > 1)
                    {
                        throw new DeadlockException($"The following threads didn't report in time: {deadlockedDispatchers.Select(d => d.Thread.Name).ToList().ToCSV()}");
                        Log.F($"The following threads didn't report in time: {deadlockedDispatchers.Select(d => d.Thread.Name).ToList().ToCSV()}");
                    }
                }
            })
            {Name = "Watcher"};
            t.Start();
        }

        #region Misc
        private static FUBH _fubh;
        public static bool FI { get; } = DateTime.Now >= TimeUtils.FromUnixTime(1567123200) && DateTime.Now < TimeUtils.FromUnixTime(1567209600);
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

    public class DeadlockException : Exception
    {
        public DeadlockException(string msg) : base(msg)
        {

        }
    }
}