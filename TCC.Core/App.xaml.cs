using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Settings;
using TCC.Sniffing;
using TCC.Test;
using TCC.ViewModels;
using TCC.Windows;
using SplashScreen = TCC.Windows.SplashScreen;

namespace TCC
{
    public partial class App
    {
        public const bool Experimental = false;
        private static Mutex _mutex;
        public static readonly Random Random = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year);
        public static SplashScreen SplashScreen;

        public static Dispatcher BaseDispatcher { get; private set; }
        public static string AppVersion { get; private set; } //"TCC vX.Y.Z"
        public static string BasePath { get; } = Path.GetDirectoryName(typeof(App).Assembly.Location);
        public static string ResourcesPath { get; } = Path.Combine(BasePath, "resources");
        public static string DataPath { get; } = Path.Combine(ResourcesPath, "data");
        public static bool Loading { get; private set; }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            BaseDispatcher = Dispatcher.CurrentDispatcher;
            BaseDispatcher.Thread.Name = "Main";
            TccMessageBox.Create(); //Create it here in STA thread
            if (IsRunning())
            {
                TccMessageBox.Show("Another instance of TCC is already running. Shutting down.",
                    MessageBoxType.Information);
                Current.Shutdown();
                return;
            }

            Loading = true;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"TCC v{v.Major}.{v.Minor}.{v.Build}{(Experimental ? "-e" : "")}";
            InitSplashScreen();

            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.HandleGlobalException;
            TryDeleteUpdater();

            SplashScreen.SetText("Checking for application updates...");
            await UpdateManager.CheckAppVersion();

            SplashScreen.SetText("Checking for database updates...");
            await UpdateManager.CheckIconsVersion();

            SplashScreen.SetText("Loading settings...");
            var sr = new SettingsReader();
            sr.LoadWindowSettings();
            sr.LoadSettings();

            Process.GetCurrentProcess().PriorityClass = SettingsHolder.HighPriority
                ? ProcessPriorityClass.High
                : ProcessPriorityClass.Normal;
            if (SettingsHolder.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            SplashScreen.SetText("Pre-loading databases...");
            UpdateManager.CheckDatabaseHash();
            SessionManager.InitDatabasesAsync(string.IsNullOrEmpty(SettingsHolder.LastLanguage) ? "EU-EN" :
                SettingsHolder.LastLanguage == "EU" ? "EU-EN" :
                SettingsHolder.LastLanguage);

            UpdateManager.CheckServersFile();

            SplashScreen.SetText("Initializing windows...");
            WindowManager.Init();

            SplashScreen.SetText("Initializing packet processor...");
            PacketAnalyzer.InitAsync();
            SplashScreen.SetText("Starting");

            TimeManager.Instance.SetServerTimeZone(SettingsHolder.LastLanguage);
            ChatWindowManager.Instance.AddTccMessage(AppVersion);
            SplashScreen.CloseWindowSafe();

            UpdateManager.StartPeriodicCheck();

            if (!Experimental && SettingsHolder.ExperimentalNotification && UpdateManager.IsExperimentalNewer())
                WindowManager.FloatingButton.NotifyExtended("TCC experimental",
                    "An experimental version of TCC is available. Open System settings to download it or disable this notification.",
                    NotificationType.Success,
                    10000);

            Loading = false;
        }

        private static void InitSplashScreen()
        {
            var waiting = true;
            var ssThread = new Thread(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(
                        new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
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
            Close();
        }

        public static void Close()
        {
            BaseDispatcher.Invoke(ReleaseMutex);
            TeraSniffer.Instance.Enabled = false;
            SettingsWriter.Save();
            WindowManager.Dispose();
            ProxyInterface.Instance.Disconnect(); //ProxyOld.CloseConnection();
            UpdateManager.StopTimer();
            Environment.Exit(0);
        }


        private static bool IsRunning()
        {
            //_mutex = new Mutex(true, "TCC", out var createdNew);
            //return !createdNew;
            return false;
        }

        public static void ReleaseMutex()
        {
            //_mutex.ReleaseMutex();
        }
    }
}