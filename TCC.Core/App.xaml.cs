using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.WPF.Extensions;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Loader;
using TCC.Notice;
using TCC.Settings;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Update;
using TCC.Utilities;
using TCC.Utils;
using TCC.Utils.Exceptions;
using TeraPacketParser.Analysis;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC;

public partial class App
{
    public static event Action? ReadyEvent;

    static bool _restarted;
    static bool _running;
    static Mutex? _mutex;

    public static bool Beta => GlobalFlags.IsBeta;

    /// <summary>
    ///     Version in the "TCC vX.Y.Z-b" format.
    /// </summary>
    public static string AppVersion { get; private set; } = null!;

    /// <summary>
    ///     'TCC.exe' folder
    /// </summary>
    public static string BasePath { get; private set; } = Path.GetDirectoryName(typeof(App).Assembly.Location)!;

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
    static bool FirstStart { get; set; }

    public static Random Random { get; } = new(DateTime.Now.DayOfYear + DateTime.Now.Year +
                                               DateTime.Now.Minute + DateTime.Now.Second +
                                               DateTime.Now.Millisecond);

    public static TccSplashScreen SplashScreen { get; set; } = null!;
    public static SettingsContainer Settings { get; private set; } = null!;

    async void OnStartup(object sender, StartupEventArgs e)
    {
        _running = true;
        AppVersion = TccUtils.GetTccVersion();
        Log.Config(Path.Combine(BasePath, "logs"), AppVersion); // NLog when?
        ParseStartupArgs(e.Args.ToList());

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        FirstStart = !File.Exists(Path.Combine(BasePath, SettingsGlobals.SettingsFileName));

        BaseDispatcher = Dispatcher.CurrentDispatcher;
        BaseDispatcher.Thread.Name = "Main";
        Utils.Utilities.CloseRequested += () => Close();

        TccMessageBox.CreateAsync();
        if (IsAlreadyRunning() && !Debugger.IsAttached)
        {
            if (!ToolboxMode)
            {
                TccMessageBox.Show(SR.AnotherInstanceRunning, MessageBoxType.Information);
            }
            Current.Shutdown();
            return;
        }

        if (!Debugger.IsAttached)
        {
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler.OnGlobalException;
        }

        Loading = true;
        await Setup();
        Loading = false;

        if (FirstStart)
        {
            new WelcomeWindow().Show();
        }


        //TCC.Debug.Tester.Enable();
        ////TCC.Debug.Tester.AddAbnormality(101301);
        ////TCC.Debug.Tester.AddAbnormality(690093);
        ////TCC.Debug.Tester.AddAbnormality(4030);
        ////TCC.Debug.Tester.AddAbnormality(4830);
        ////TCC.Debug.Tester.AddAbnormality(902);
        ////TCC.Debug.Tester.AddAbnormality(922);
        //TCC.Debug.Tester.AddFakeGroupMember(1, TeraDataLite.Class.Sorcerer, TeraDataLite.Laurel.Gold, false);
        //TCC.Debug.Tester.AddAbnormalityToGroupMember(memberId: 1, abnormalId: 4030);
        //TCC.Debug.Tester.AddAbnormalityToGroupMember(memberId: 1, abnormalId: 700000);
        //TCC.Debug.Tester.AddAbnormalityToGroupMember(memberId: 1, abnormalId: 101301);
        //Tester.StartItemCooldown(444);
        //TCC.Debug.Tester.ShowDebugWindow();
    }

    static async Task Setup()
    {
        TccUtils.SetAlignment();

        NoticeChecker.Init();

        TccSplashScreen.InitOnNewThread();

        if (!ToolboxMode)
        {
            UpdateManager.TryDeleteUpdater();

            SplashScreen.VM.BottomText = "Checking for application updates...";
            await UpdateManager.CheckAppVersion();
        }

        // ----------------------------
        SplashScreen.VM.Progress = 10;
        SplashScreen.VM.BottomText = Random.NextDouble() <= 0.4 ? "Complaining because I'm too lazy to keep updating this..." : "Loading settings...";
        Settings = SettingsContainer.Load();
        WindowManager.InitSettingsWindow(); // need it in case language is not correct
        SplashScreen.VM.Progress = 20;
        Process.GetCurrentProcess().PriorityClass = Settings.HighPriority
            ? ProcessPriorityClass.High
            : ProcessPriorityClass.Normal;
        if (Settings.ForceSoftwareRendering)
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }

        // ----------------------------
        SplashScreen.VM.Progress = 30;
        SplashScreen.VM.BottomText = Random.NextDouble() <= 0.4 ? "WTB <Patch 28> /w me" : "Pre-loading databases...";
        UpdateManager.CheckDatabaseHash();
        SplashScreen.VM.Progress = 40;
        await Game.InitAsync();

        // ----------------------------
        SplashScreen.VM.Progress = 50;
        SplashScreen.VM.BottomText = Random.NextDouble() <= 0.4 ? "WTS <Mark of Bloodshed> 40k /w me" : "Initializing widgets...";
        await WindowManager.Init();
        SplashScreen.VM.Progress = 60;
        StartDispatcherWatcher();

        // ----------------------------
        SplashScreen.VM.Progress = 70;
        SplashScreen.VM.BottomText = Random.NextDouble() <= 0.4 ? "Ok but where is my flying EX-TRM????" : "Checking for icon database updates...";
        _ = Task.Run(() => new IconsUpdater().CheckForUpdates());

        // ----------------------------
        SplashScreen.VM.Progress = 80;
        SplashScreen.VM.BottomText = /*Random.NextDouble() <= 0.4 ? "Just move to Genshin Impact :kekw:" :*/ "Initializing packet processor...";
        PacketAnalyzer.ProcessorReady += LoadModules;
        PacketAnalyzer.InitServerDatabase(DataPath, Path.Combine(ResourcesPath, "config/server-overrides.txt"), string.IsNullOrEmpty(Settings.LastLanguage)
            ? "EU-EN"
            : Settings.LastLanguage);
        await PacketAnalyzer.InitAsync(Settings.CaptureMode, ToolboxMode);
        _ = StubInterface.Instance.InitAsync(Settings.LfgWindowSettings.Enabled,
            Settings.EnablePlayerMenu,
            Settings.EnableProxy,
            Settings.ShowIngameChat,
            Settings.ChatEnabled);

        // ----------------------------
        SplashScreen.VM.Progress = 90;
        SplashScreen.VM.BottomText = Random.NextDouble() <= 0.4 ? "Imagine unironically playing TERA :^)" : "Starting";
        GameEventManager.Instance.SetServerTimeZone(Settings.LastLanguage);
        UpdateManager.StartPeriodicCheck();

        SplashScreen.VM.Progress = 100;
        SplashScreen.CloseWindowSafe();

        // ----------------------------
        Log.Chat($"{AppVersion} ready.");
        ReadyEvent?.Invoke();

        if (!Beta && Settings.BetaNotification && UpdateManager.IsBetaNewer())
        {
            Log.N("TCC beta available", SR.BetaAvailable, NotificationType.Success, 10000);
        }
    }

    static void ParseStartupArgs(IList<string> args)
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
        if (rootOverrideIdx == -1) return;

        BasePath = args[rootOverrideIdx + 1];
        ResourcesPath = Path.Combine(BasePath, "resources");
        DataPath = Path.Combine(ResourcesPath, "data");
    }

    public static void Restart()
    {
        Settings.Save();
        Process.Start("TCC.exe", $"--restart{(ToolboxMode ? " --toolbox" : "")}");
        Close();
    }

    public static void Close(bool releaseMutex = true, int code = 0)
    {
        _running = false;
        PacketAnalyzer.Sniffer.Enabled = false;
        Settings.Save();
        WindowManager.Dispose();
        StubInterface.Instance.Disconnect();
        Firebase.Dispose();
        UpdateManager.StopTimer();
        if (releaseMutex)
        {
            BaseDispatcher.Invoke(ReleaseMutex);
        }
        Environment.Exit(code);
    }

    static bool IsAlreadyRunning()
    {
        _mutex = new Mutex(true, nameof(TCC), out var createdNew);
        if (createdNew || !_restarted) return !createdNew;
        _mutex.WaitOne();
        return false;
    }

    public static void ReleaseMutex()
    {
        _running = false;
        BaseDispatcher.Invoke(() =>
        {
            try
            {
                _mutex?.ReleaseMutex();
            }
            catch (Exception e)
            {
                Log.F($"Failed to release mutex: {e}");
            }
        });
    }

    static void LoadModules()
    {
        BaseDispatcher.Invoke(() =>
        {
            try
            {
                ModuleLoader.LoadModules(BasePath);
            }
            catch (FileLoadException fle)
            {
                TccMessageBox.Show("TCC module loader", SR.ErrorWhileLoadingModule(fle.FileName), MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            catch (FileNotFoundException fnfe)
            {
                TccMessageBox.Show("TCC module loader", SR.ErrorWhileLoadingModule(Path.GetFileName(fnfe.FileName)), MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        });
    }

    #region Dispatchers

    public static Dispatcher BaseDispatcher { get; private set; } = null!;

    public static ConcurrentDictionary<int, Dispatcher> RunningDispatchers { get; } = new();

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
        var tries = 50;
        while (tries > 0)
        {
            if (RunningDispatchers.Count == 0) break;
            //Log.CW($"Waiting for all dispatcher to shutdown... ({RunningDispatchers.Count} left)");
            Thread.Sleep(100);
            tries--;
        }

        //Log.CW("All dispatchers shut down.");
    }

    static void StartDispatcherWatcher()
    {
        const int limit = 60000;

        Dispatcher[] dispatchers = [.. RunningDispatchers.Values, BaseDispatcher];
        new Thread(() =>
            {
                while (_running)
                {
                    var deadlockedDispatchers = new List<Dispatcher>();
                    try
                    {
                        Parallel.ForEach(dispatchers, async dispatcher =>
                        {
                            //Log.CW($"{dispatcher.Thread.Name} checking...");
                            if (await dispatcher.IsAlive(limit))
                            {
                                //Log.CW($"{dispatcher.Thread.Name} is alive!");
                                return;
                            }
                            Log.F($"{dispatcher.Thread.Name} didn't respond in time!");
                            deadlockedDispatchers.Add(dispatcher);
                        });
                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        Log.F($"Error while checking for threads activity: {e}");
                    }

                    if (deadlockedDispatchers.Count <= 1) continue;
                    var threadNames = deadlockedDispatchers.Select(d => d.Thread.Name).ToList();
                    Log.F($"The following threads didn't report in time: {threadNames.ToCSV()}");
                    throw new DeadlockException($"The following threads didn't report in time: {threadNames.ToCSV()}", threadNames);
                }
            })
        { Name = "Watcher" }.Start();
    }

    #endregion Dispatchers

    #region Misc

    static FUBH? _fubh;

    public static bool FI { get; } = DateTime.Now >= TimeUtils.FromUnixTime(1567123200) &&
                                     DateTime.Now < TimeUtils.FromUnixTime(1567209600);

    public static void FUBH()
    {
        BaseDispatcher.InvokeAsync(() =>
        {
            _fubh ??= new FUBH();
            _fubh.Show();
        });
    }

    #endregion Misc
}