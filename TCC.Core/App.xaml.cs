//#define FORCE_TTB

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
using TCC.Test;
using TCC.ViewModels;
using TCC.Windows;
using TeraPacketParser.Messages;
using SplashScreen = TCC.Windows.SplashScreen;

namespace TCC
{
    public partial class App
    {
        public const bool Experimental = true;

        private static Mutex _mutex;
        public static readonly Random Random = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year);
        public static SplashScreen SplashScreen;
        public static Dispatcher BaseDispatcher { get; private set; }
        public static string AppVersion { get; private set; } //"TCC vX.Y.Z"
        public static string BasePath { get; } = Path.GetDirectoryName(typeof(App).Assembly.Location);
        public static string ResourcesPath { get; } = Path.Combine(BasePath, "resources");
        public static string DataPath { get; } = Path.Combine(ResourcesPath, "data");
        public static bool Loading { get; private set; }
        public static bool ToolboxMode { get; private set; }
        public static bool Restarted { get; private set; }

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
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.HandleGlobalException;
#endif
            if (!ToolboxMode)
            {
                UpdateManager.TryDeleteUpdater();

                SplashScreen.SetText("Checking for application updates...");
                await UpdateManager.CheckAppVersion();
            }

            SplashScreen.SetText("Checking for icon database updates...");
            await UpdateManager.CheckIconsVersion();

            SplashScreen.SetText("Loading settings...");
            WindowManager.ForegroundManager = new ForegroundManager();

            SettingsContainer.Load();

            //var sr = new JsonSettingsReader();
            //sr.LoadWindowSettings();
            //sr.LoadSettings();

            Process.GetCurrentProcess().PriorityClass = Settings.HighPriority
                ? ProcessPriorityClass.High
                : ProcessPriorityClass.Normal;
            if (Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            SplashScreen.SetText("Pre-loading databases...");
            UpdateManager.CheckDatabaseHash();
            await Session.InitAsync();
            SplashScreen.SetText("Initializing windows...");
            WindowManager.Init();

            SplashScreen.SetText("Initializing packet processor...");
            PacketAnalyzer.InitAsync();
            SplashScreen.SetText("Starting");

            TimeManager.Instance.SetServerTimeZone(Settings.LastLanguage);
            ChatWindowManager.Instance.AddTccMessage(AppVersion);
            SplashScreen.CloseWindowSafe();

            ModuleLoader.LoadModules();

            if (!ToolboxMode) UpdateManager.StartPeriodicCheck();

            if (!Experimental && Settings.ExperimentalNotification && UpdateManager.IsExperimentalNewer())
                WindowManager.FloatingButton.NotifyExtended("TCC experimental",
                    "An experimental version of TCC is available. Open System settings to download it or disable this notification.",
                    NotificationType.Success,
                    10000);

            Loading = false;
        }

        private static void ParseStartupArgs(List<string> list)
        {
#if FORCE_TTB
            ToolboxMode = true;
#else
            ToolboxMode = list.IndexOf("--toolbox") != -1;
#endif
            Restarted = list.IndexOf("--restart") != -1;
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

        public static void Restart()
        {
            Settings.Save();
            Process.Start("TCC.exe", "--restart");
            Close();
        }

        public static void Close(bool releaseMutex = true)
        {
            if (releaseMutex) BaseDispatcher.Invoke(ReleaseMutex);
            PacketAnalyzer.Sniffer.Enabled = false;
            Settings.Save();
            WindowManager.Dispose();
            ProxyInterface.Instance.Disconnect(); //ProxyOld.CloseConnection();
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