using Microsoft.Win32;
using Nostrum.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TCC.UI.Controls;
using TCC.UI.Windows;
using TCC.UI.Windows.Widgets;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;
using Application = System.Windows.Application;
using Size = System.Drawing.Size;

namespace TCC.UI
{
    public static class WindowManager
    {
        public static event Action? RepositionRequestedEvent;
        public static event Action? ResetToCenterEvent;
        public static event Action? MakeGlobalEvent;
        public static event Action? DisposeEvent;

        public static TccTrayIcon TrayIcon { get; } = new();
        public static VisibilityManager VisibilityManager { get; } = new();
        public static Size ScreenSize => FocusManager.TeraScreen.Bounds.Size;


        public static CooldownWindow CooldownWindow { get; private set; } = null!;
        public static CharacterWindow CharacterWindow { get; private set; } = null!;
        public static BossWindow BossWindow { get; private set; } = null!;
        public static BuffWindow BuffWindow { get; private set; } = null!;
        public static GroupWindow GroupWindow { get; private set; } = null!;
        public static ClassWindow ClassWindow { get; private set; } = null!;
        public static SettingsWindow SettingsWindow { get; private set; } = null!;
        public static CivilUnrestWindow CivilUnrestWindow { get; private set; } = null!;
        public static FloatingButtonWindow FloatingButton { get; private set; } = null!;
        public static NotificationAreaWindow NotificationArea { get; private set; } = null!;
        public static FlightDurationWindow FlightDurationWindow { get; private set; } = null!;
        //public static SkillConfigWindow? SkillConfigWindow { get; set; }
        public static PerfMonitorWindow PerfMonitor { get; private set; } = null!;

        public static Dashboard DashboardWindow { get; private set; } = null!;

        public static LfgListWindow LfgListWindow { get; private set; } = null!;


        public static async Task Init()
        {
            FocusManager.Init();

            await LoadWindows();


            if (App.Settings.UseHotkeys) KeyboardHook.Instance.Enable();

            KeyboardHook.Instance.RegisterCallback(App.Settings.ToggleBoundariesHotkey, TccWidget.OnShowAllHandlesToggled);
            KeyboardHook.Instance.RegisterCallback(App.Settings.ToggleHideAllHotkey, TccWidget.OnHideAllToggled);

            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }

        private static void SystemEventsOnDisplaySettingsChanged(object? sender, EventArgs e)
        {
            ReloadPositions();
        }

        public static void Dispose()
        {
            DisposeEvent?.Invoke();
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;

            App.WaitDispatchersShutdown();

            App.BaseDispatcher.Invoke(() =>
            {
                TrayIcon.Dispose();
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

            var b6 = new TccWidgetBuilder<BuffWindow, AbnormalityWindowViewModel>(App.Settings.BuffWindowSettings);
            BuffWindow = await b6.GetWindow();
            ViewModels.AbnormalVM = await b6.GetViewModel();

            var b7 = new TccWidgetBuilder<NotificationAreaWindow, NotificationAreaViewModel>(App.Settings.NotificationAreaSettings);
            NotificationArea = await b7.GetWindow();
            ViewModels.NotificationAreaVM = await b7.GetViewModel();

            var b8 = new TccWidgetBuilder<FloatingButtonWindow, FloatingButtonViewModel>(App.Settings.FloatingButtonSettings);
            FloatingButton = await b8.GetWindow();
            ViewModels.FloatingButtonVM = await b8.GetViewModel();

            var b9 = new TccWidgetBuilder<Dashboard, DashboardViewModel>(null);
            DashboardWindow = await b9.GetWindow();
            ViewModels.DashboardVM = await b9.GetViewModel();

            var b10 = new TccWidgetBuilder<PerfMonitorWindow, PerfMonitorViewModel>(App.Settings.PerfMonitorSettings);
            PerfMonitor = await b10.GetWindow();
            ViewModels.PerfMonitorVM = await b10.GetViewModel();

            FlightDurationWindow = new FlightDurationWindow(ViewModels.FlightGaugeVM);
            if (FlightDurationWindow.WindowSettings?.Enabled == true) FlightDurationWindow.Show();

            CivilUnrestWindow = new CivilUnrestWindow(ViewModels.CivilUnrestVM);
            if (CivilUnrestWindow.WindowSettings?.Enabled == true) CivilUnrestWindow.Show();

            LfgListWindow = new LfgListWindow(ViewModels.LfgVM);

            ViewModels.PlayerMenuVM = new PlayerMenuViewModel();

            ChatManager.Start();
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

        public static void InitSettingsWindow()
        {
            //if (App.Settings.ShowConsole) Kernel32.AllocConsole();
            SettingsWindow = new SettingsWindow();
        }

        public static class ViewModels
        {
            private static CivilUnrestViewModel? _civilUnrestVm;
            private static LfgListViewModel? _lfgVm;
            private static FlightGaugeViewModel? _flightGaugeVm;

            public static CooldownWindowViewModel CooldownsVM { get; set; } = null!;
            public static CharacterWindowViewModel CharacterVM { get; set; } = null!;
            public static NpcWindowViewModel NpcVM { get; set; } = null!;
            public static AbnormalityWindowViewModel AbnormalVM { get; set; } = null!;
            public static ClassWindowViewModel ClassVM { get; set; } = null!;
            public static NotificationAreaViewModel NotificationAreaVM { get; set; } = null!;
            public static PlayerMenuViewModel PlayerMenuVM { get; set; } = null!;
            public static DashboardViewModel DashboardVM { get; set; } = null!;
            public static FloatingButtonViewModel FloatingButtonVM { get; set; } = null!;
            public static GroupWindowViewModel GroupVM { get; set; } = null!;
            public static PerfMonitorViewModel PerfMonitorVM { get; set; } = null!;

            public static CivilUnrestViewModel CivilUnrestVM => _civilUnrestVm ??= new CivilUnrestViewModel(App.Settings.CivilUnrestWindowSettings);
            public static LfgListViewModel LfgVM => _lfgVm ??= new LfgListViewModel(App.Settings.LfgWindowSettings);
            public static FlightGaugeViewModel FlightGaugeVM => _flightGaugeVm ??= new FlightGaugeViewModel(App.Settings.FlightGaugeWindowSettings);
        }

    }
}