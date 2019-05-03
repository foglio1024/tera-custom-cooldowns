//#define FIRESTORE

using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Settings;
using TCC.Sniffing;
using TCC.Utilities.Extensions;
using TCC.ViewModels;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;
using SplashScreen = TCC.Windows.SplashScreen;

namespace TCC
{
    public partial class App
    {
        private static Mutex _mutex;
        public static string AppVersion { get; private set; } //"TCC vX.Y.Z"
        public const bool Experimental = false;
        public static SplashScreen SplashScreen;
        public static Dispatcher BaseDispatcher;
        public static string BasePath { get; } = Path.GetDirectoryName(typeof(App).Assembly.Location);
        public static string ResourcesPath { get; } = Path.Combine(BasePath, "resources");
        public static string DataPath { get; } = Path.Combine(ResourcesPath, "data");
        public static bool Loading { get; private set; }
        public static readonly Random Random = new Random(DateTime.Now.DayOfYear + DateTime.Now.Year);

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            BaseDispatcher = Dispatcher.CurrentDispatcher;
            BaseDispatcher.Thread.Name = "Main";
            TccMessageBox.Create(); //Create it here in STA thread
            _mutex = new Mutex(true, "TCC", out var createdNew);
            if (!createdNew)
            {
                TccMessageBox.Show("Another instance of TCC is already running. Shutting down.",
                    MessageBoxType.Information);
                Application.Current.Shutdown();
                return;
            }
            Loading = true;
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            AppVersion = $"TCC v{v.Major}.{v.Minor}.{v.Build}{(Experimental ? "-e" : "")}";
            InitSplashScreen();

            AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
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
            SessionManager.InitDatabases(string.IsNullOrEmpty(SettingsHolder.LastLanguage) ? "EU-EN" :
                SettingsHolder.LastLanguage == "EU" ? "EU-EN" : SettingsHolder.LastLanguage);
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

            if (!Experimental && SettingsHolder.ExperimentalNotification && UpdateManager.IsExperimentalNewer())
                WindowManager.FloatingButton.NotifyExtended("TCC experimental",
                    "An experimental version of TCC is available. Open System settings to download it or disable this notification.",
                    NotificationType.Success,
                    10000);

            Loading = false;
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
            Log.F(sb.ToString(), "crash.log");
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
            TccMessageBox.Show("TCC",
                "An error occured and TCC will now close. Report this issue to the developer attaching crash.log from TCC folder.",
                MessageBoxButton.OK, MessageBoxImage.Error);

            //if (ProxyOld.IsConnected) ProxyOld.CloseConnection();
            ProxyInterface.Instance.Disconnect();
            if (WindowManager.TrayIcon != null) WindowManager.TrayIcon.Dispose();
            try
            {
                WindowManager.Dispose();
            }
            catch
            {
                /* ignored*/
            }

            try
            {
                Firebase.RegisterWebhook(SettingsHolder.WebhookUrlGuildBam, false);
                Firebase.RegisterWebhook(SettingsHolder.WebhookUrlFieldBoss, false);
            }
            catch
            {
            }
            _mutex.ReleaseMutex();
            Environment.Exit(-1);
        }

        private static void UploadCrashDump(UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;

                var full = $"{ex.Message}\r\n{ex.StackTrace}\r\n{ex.Source}\r\n{ex}\r\n{ex.Data}\r\n{ex.InnerException}\r\n{ex.TargetSite}";
                var js = new JObject
                {
                    { "tcc_version" , new JValue(AppVersion) },
                    { "tcc_hash", Utils.GenerateFileHash(typeof(App).Assembly.Location) },
                    { "exception", new JValue(ex.Message)},
                    { "full_exception", new JValue(full)},
                    { "inner_exception",new JValue(ex.InnerException != null ? ex.InnerException.Message : "undefined") },
                    { "game_version", new JValue(PacketAnalyzer.Factory.ReleaseVersion)},
                    { "region", new JValue(SessionManager.Server != null ? SessionManager.Server.Region : "")},
                    { "server_id", new JValue(SessionManager.Server != null ? SessionManager.Server.ServerId.ToString() : "")},
                    { "settings_summary", new JObject
                        {
                            { "windows", new JObject
                                {
                                { "cooldown", SettingsHolder.CooldownWindowSettings.Enabled },
                                { "buffs", SettingsHolder.BuffWindowSettings.Enabled },
                                { "character", SettingsHolder.CharacterWindowSettings.Enabled },
                                { "class", SettingsHolder.ClassWindowSettings.Enabled },
                                { "chat", SettingsHolder.ChatEnabled},
                                { "group", SettingsHolder.GroupWindowSettings.Enabled }
                                }
                            },
                            {
                                "generic", new JObject
                                {
                                    { "proxy_enabled", SettingsHolder.EnableProxy },
                                }
                            }
                        }
                    }
                };

                c.UploadString(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/crash_report"),
                               Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
            }
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
            try { File.Delete(Path.Combine(BasePath, "TCCupdater.exe")); } catch {/* ignored*/}
        }

        public static void Restart()
        {
            SettingsWriter.Save();
            Process.Start("TCC.exe");
            CloseApp();
        }

        public static void SendUsageStat()
        {
            if (SettingsHolder.StatSentVersion == AppVersion &&
                SettingsHolder.StatSentTime.Month == DateTime.UtcNow.Month &&
                SettingsHolder.StatSentTime.Day == DateTime.UtcNow.Day) return;

            try
            {
                using (var c = Utils.GetDefaultWebClient())
                {
                    c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    c.Encoding = Encoding.UTF8;

                    var accountNameHash = SHA256.Create().ComputeHash(SessionManager.CurrentAccountName.ToByteArray()).ToStringEx();
                    var js = new JObject
                    {
                        {"region", SessionManager.Server.Region},
                        {"server", SessionManager.Server.ServerId},
                        {"account", accountNameHash},
                        {"tcc_version", AppVersion},
                        {"updated", SettingsHolder.StatSentTime.Month == DateTime.Now.Month &&
                                    SettingsHolder.StatSentTime.Day == DateTime.Now.Day &&
                                    SettingsHolder.StatSentVersion != AppVersion
                        },
                        { "settings_summary", new JObject
                            {
                                { "windows", new JObject
                                    {
                                        { "cooldown", SettingsHolder.CooldownWindowSettings.Enabled },
                                        { "buffs", SettingsHolder.BuffWindowSettings.Enabled },
                                        { "character", SettingsHolder.CharacterWindowSettings.Enabled },
                                        { "class", SettingsHolder.ClassWindowSettings.Enabled },
                                        { "chat", SettingsHolder.ChatEnabled},
                                        { "group", SettingsHolder.GroupWindowSettings.Enabled }
                                    }
                                },
                                {
                                    "generic", new JObject
                                    {
                                        { "proxy_enabled", SettingsHolder.EnableProxy },
                                    }
                                }
                            }
                        }

                    };

                    c.UploadStringAsync(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/usage_stat"),
                                        Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));

                    SettingsHolder.StatSentTime = DateTime.UtcNow;
                    SettingsHolder.StatSentVersion = AppVersion;
                }
            }
            catch
            {
                //TODO: write error?
            }
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsWriter.Save();
            WindowManager.Dispose();
            ProxyInterface.Instance.Disconnect(); //ProxyOld.CloseConnection();
            UpdateManager.StopTimer();
            _mutex.ReleaseMutex();
            Environment.Exit(0);
        }

    }
}