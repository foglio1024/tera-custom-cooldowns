using FoglioUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Interop.Proxy;
using TCC.Loader;
using TCC.Parsing;
using TCC.Settings;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC
{
    public partial class App
    {
        public const bool Experimental = true;

        private static Mutex _mutex;
        public static readonly Random Random = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
        public static TccSplashScreen SplashScreen;
        public static Dispatcher BaseDispatcher { get; private set; }
        public static string AppVersion { get; private set; } //"TCC vX.Y.Z"
        public static string BasePath { get; } = Path.GetDirectoryName(typeof(App).Assembly.Location);
        public static string ResourcesPath { get; } = Path.Combine(BasePath, "resources");
        public static string DataPath { get; } = Path.Combine(ResourcesPath, "data");
        public static bool Loading { get; private set; }
        public static bool ToolboxMode { get; private set; }
        public static bool Restarted { get; private set; }

        public static bool FI = DateTime.Now >= TimeUtils.FromUnixTime(1567123200) && DateTime.Now < TimeUtils.FromUnixTime(1567209600);

        public static SettingsContainer Settings;

        private static FUBH fubh;
        public static void FUBH()
        {
            BaseDispatcher.BeginInvoke(new Action(() =>
            {
                if (fubh == null) fubh = new FUBH();
                fubh.Show();
            }));
        }
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            ParseStartupArgs(e.Args.ToList());
            BaseDispatcher = Dispatcher.CurrentDispatcher;
            BaseDispatcher.Thread.Name = "Main";
            TccMessageBox.Create(); //Create it here in STA thread

            if (IsRunning())
            {
                if (!ToolboxMode) TccMessageBox.Show("Another instance of TCC is already running. Shutting down.",
                     MessageBoxType.Information);
                Current.Shutdown();
                return;
            }

            Loading = true;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"TCC v{v.Major}.{v.Minor}.{v.Build}{(Experimental ? "-e" : "")}";
            InitSplashScreen();

            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.HandleGlobalException;

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

            WindowManager.ForegroundManager = new ForegroundManager();

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

            await WindowManager.Init();

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

        private static void ParseStartupArgs(List<string> list)
        {
            ToolboxMode = list.IndexOf("--toolbox") != -1;
            Restarted = list.IndexOf("--restart") != -1;
            var settingsOverrideIdx = list.IndexOf("--settings_override");
            if (settingsOverrideIdx != -1)
            {
                SettingsContainer.SettingsOverride = list[settingsOverrideIdx + 1];
            }

        }


        private static void InitSplashScreen()
        {
            var waiting = true;
            var ssThread = new Thread(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                    SplashScreen = new TccSplashScreen();
                    SplashScreen.VM.BottomText = "Initializing...";
                    SplashScreen.Show();
                    waiting = false;
                    Dispatcher.Run();
                })
            { Name = "SplashScreen window thread" };
            ssThread.SetApartmentState(ApartmentState.STA);
            ssThread.Start();
            while (waiting) Thread.Sleep(1);
        }

        public static void Restart()
        {
            Settings.Save();
            Process.Start("TCC.exe", $"--restart{(ToolboxMode ? " --toolbox" : "")}");
            Close();
        }

        public static void Close(bool releaseMutex = true)
        {
            if (releaseMutex) BaseDispatcher.Invoke(ReleaseMutex);
            PacketAnalyzer.Sniffer.Enabled = false;
            Settings.Save();
            WindowManager.Dispose();
            ProxyInterface.Instance.Disconnect();
            UpdateManager.StopTimer();
            Environment.Exit(0);
        }


        private static bool IsRunning()
        {
            _mutex = new Mutex(true, "TCC", out var createdNew);
            if (createdNew || !Restarted) return !createdNew;
            _mutex.WaitOne();
            return false;
        }

        public static void ReleaseMutex()
        {
            _mutex.ReleaseMutex();
        }
    }

}