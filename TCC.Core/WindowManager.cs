using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FoglioUtils.Extensions;
using Microsoft.Win32;
using TCC.Utilities;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;
using TCC.Windows;
using TCC.Windows.Widgets;

namespace TCC
{
    public static class WindowManager
    {
        public static event Action RepositionRequestedEvent;
        public static event Action ResetToCenterEvent;
        public static event Action MakeGlobalEvent;
        public static event Action<Size> ApplyScreenCorrectionEvent;
        public static event Action DisposeEvent;

        public static ConcurrentDictionary<int, Dispatcher> RunningDispatchers { get; private set; }
        public static TccTrayIcon TrayIcon { get; private set; }
        public static ForegroundManager ForegroundManager { get; set; }

        public static class ViewModels
        {
            private static CivilUnrestViewModel _civilUnrestVm;
            private static DashboardViewModel _dashboardVm;
            private static LfgListViewModel _lfgVm;
            private static FlightGaugeViewModel _flightGaugeVm;

            public static CooldownWindowViewModel CooldownsVM { get; set; }
            public static CharacterWindowViewModel CharacterVM { get; set; }
            public static NpcWindowViewModel NpcVM { get; set; }
            public static BuffBarWindowViewModel AbnormalVM { get; set; }
            public static ClassWindowViewModel ClassVM { get; set; }
            public static NotificationAreaViewModel NotificationAreaVM { get; set; }

            public static CivilUnrestViewModel CivilUnrestVM =>
                _civilUnrestVm ?? (_civilUnrestVm = new CivilUnrestViewModel(App.Settings.CivilUnrestWindowSettings));

            public static DashboardViewModel DashboardVM =>
                _dashboardVm ?? (_dashboardVm = new DashboardViewModel(null));

            public static LfgListViewModel LfgVM =>
                _lfgVm ?? (_lfgVm = new LfgListViewModel(App.Settings.LfgWindowSettings));

            public static FlightGaugeViewModel FlightGaugeVM =>
                _flightGaugeVm ?? (_flightGaugeVm = new FlightGaugeViewModel(App.Settings.FlightGaugeWindowSettings));

            public static GroupWindowViewModel GroupVM { get; set; }
        }

        public static Size ScreenSize;
        private static bool _running;

        public static CooldownWindow CooldownWindow { get; private set; }
        public static CharacterWindow CharacterWindow { get; private set; }
        public static BossWindow BossWindow { get; private set; }
        public static BuffWindow BuffWindow { get; private set; }
        public static GroupWindow GroupWindow { get; private set; }
        public static ClassWindow ClassWindow { get; private set; }
        public static SettingsWindow SettingsWindow { get; private set; }
        public static CivilUnrestWindow CivilUnrestWindow { get; private set; }
        public static FloatingButtonWindow FloatingButton { get; private set; }
        public static NotificationAreaWindow NotificationArea { get; private set; }
        public static FlightDurationWindow FlightDurationWindow { get; private set; }
        public static SkillConfigWindow SkillConfigWindow { get; set; }

        public static Dashboard DashboardWindow { get; private set; }

        public static LfgListWindow LfgListWindow { get; private set; }


        public static async Task Init()
        {
            _running = true;
            ScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            FocusManager.Init();

            await LoadWindows();

            TrayIcon = new TccTrayIcon();


            if (App.Settings.UseHotkeys) KeyboardHook.Instance.Enable();

            KeyboardHook.Instance.RegisterCallback(App.Settings.ToggleBoundariesHotkey, TccWidget.OnShowAllHandlesToggled);
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }
        private static Size GetScreenCorrection()
        {
            var wFac = App.Settings.LastScreenSize.Width / ScreenSize.Width;
            var hFac = App.Settings.LastScreenSize.Height / ScreenSize.Height;
            return new Size(wFac, hFac);
        }

        private static void UpdateScreenCorrection()
        {
            if (ScreenSize.IsEqual(App.Settings.LastScreenSize)) return;
            ApplyScreenCorrection(GetScreenCorrection());
            App.Settings.LastScreenSize = ScreenSize;
            if (!App.Loading) App.Settings.Save();
        }

        private static void ApplyScreenCorrection(Size sc)
        {
            ApplyScreenCorrectionEvent?.Invoke(sc);
        }


        private static void SystemEventsOnDisplaySettingsChanged(object sender, EventArgs e)
        {
            ScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            UpdateScreenCorrection();
            ReloadPositions();
        }

        public static void Dispose()
        {
            _running = false;
            DisposeEvent?.Invoke();
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;

            WaitDispatchersShutdown();

            App.BaseDispatcher.Invoke(() =>
            {
                TrayIcon?.Dispose();
                CloseOtherWindows();
            });

        }

        private static void CloseOtherWindows()
        {
            Application.Current.Windows.ToList()
            .Where(w => !(w is TccWidget)).ToList()
            .ForEach(w => w.TryClose());
        }

        private static async Task LoadWindows()
        {
            RunningDispatchers = new ConcurrentDictionary<int, Dispatcher>();

            // TODO: TccModules should define and create their own windows
            var b1 = new TccWidgetBuilder<CharacterWindow, CharacterWindowViewModel>(App.Settings.CharacterWindowSettings);
            CharacterWindow = await b1.GetWindow();
            ViewModels.CharacterVM = await b1.GetViewModel();

            var b2 = new TccWidgetBuilder<CooldownWindow, CooldownWindowViewModel>(App.Settings.CooldownWindowSettings);
            CooldownWindow = await b2.GetWindow();
            ViewModels.CooldownsVM = await b2.GetViewModel();

            var b3 = new TccWidgetBuilder<ClassWindow, ClassWindowViewModel>(App.Settings.ClassWindowSettings);
            ClassWindow = await b3.GetWindow();
            ViewModels.ClassVM = await b3.GetViewModel();

            var b4 = new TccWidgetBuilder<GroupWindow, GroupWindowViewModel>(App.Settings.GroupWindowSettings);
            GroupWindow = await b4.GetWindow();
            ViewModels.GroupVM = await b4.GetViewModel();

            var b5 = new TccWidgetBuilder<BossWindow, NpcWindowViewModel>(App.Settings.NpcWindowSettings);
            BossWindow = await b5.GetWindow();
            ViewModels.NpcVM = await b5.GetViewModel();

            var b6 = new TccWidgetBuilder<BuffWindow, BuffBarWindowViewModel>(App.Settings.BuffWindowSettings);
            BuffWindow = await b6.GetWindow();
            ViewModels.AbnormalVM = await b6.GetViewModel();

            var b7 = new TccWidgetBuilder<NotificationAreaWindow, NotificationAreaViewModel>(App.Settings.NotificationAreaSettings);
            NotificationArea = await b7.GetWindow();
            ViewModels.NotificationAreaVM = await b7.GetViewModel();


            FlightDurationWindow = new FlightDurationWindow(ViewModels.FlightGaugeVM);
            if (FlightDurationWindow.WindowSettings.Enabled) FlightDurationWindow.Show();

            CivilUnrestWindow = new CivilUnrestWindow(ViewModels.CivilUnrestVM);
            if (CivilUnrestWindow.WindowSettings.Enabled) CivilUnrestWindow.Show();

            FloatingButton = new FloatingButtonWindow();
            if (FloatingButton.WindowSettings.Enabled) FloatingButton.Show();

            DashboardWindow = new Dashboard(ViewModels.DashboardVM);
            LfgListWindow = new LfgListWindow(ViewModels.LfgVM);

            ChatWindowManager.Start();

            SettingsWindow = new SettingsWindow();

            StartDispatcherWatcher();
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
                        Parallel.ForEach(RunningDispatchers.Values.Append(App.BaseDispatcher), (v) =>
                        {
                            if (v.IsAlive(10000).Result) return;
                            Log.CW($"{v.Thread.Name} didn't respond in time!");
                            deadlockedDispatchers.Add(v);
                        });
                        Thread.Sleep(100);
                    }
                    catch { }
                    if (deadlockedDispatchers.Count > 1)
                    {
                        throw new DeadlockException($"The following threads didn't report in time: {deadlockedDispatchers.Select(d => d.Thread.Name).ToList().ToCSV()}");
                    }
                }
            })
            {Name = "Watcher"};
            t.Start();
        }

        public static void AddDispatcher(int threadId, Dispatcher d)
        {
            RunningDispatchers[threadId] = d;
        }

        public static void RemoveDispatcher(int threadId)
        {
            RunningDispatchers.TryRemove(threadId, out _);
        }

        private static void WaitDispatchersShutdown()
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

        public static void ReloadPositions()
        {
            RepositionRequestedEvent?.Invoke();
        }

        public static void MakeGlobal()
        {
            MakeGlobalEvent?.Invoke();
            App.Settings.Save();
        }

        public static void ResetToCenter()
        {
            ResetToCenterEvent?.Invoke();
        }
    }
}