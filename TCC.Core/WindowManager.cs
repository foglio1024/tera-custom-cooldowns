using FoglioUtils.Extensions;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using TCC.Controls;
using TCC.Controls.Chat;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;
using TCC.Windows;
using TCC.Windows.Widgets;
using Application = System.Windows.Application;

namespace TCC
{
    public static class WindowManager
    {
        public static event Action RepositionRequestedEvent;
        public static event Action ResetToCenterEvent;
        public static event Action MakeGlobalEvent;
        public static event Action DisposeEvent;

        public static TccTrayIcon TrayIcon { get; private set; }
        public static VisibilityManager VisibilityManager { get; set; }
        public static System.Drawing.Size ScreenSize => FocusManager.TeraScreen.Bounds.Size;

        public static class ViewModels
        {
            private static CivilUnrestViewModel _civilUnrestVm;
            private static DashboardViewModel _dashboardVm;
            private static LfgListViewModel _lfgVm;
            private static FlightGaugeViewModel _flightGaugeVm;
            private static FloatingButtonViewModel _floatingButtonVm;

            public static CooldownWindowViewModel CooldownsVM { get; set; }
            public static CharacterWindowViewModel CharacterVM { get; set; }
            public static NpcWindowViewModel NpcVM { get; set; }
            public static AbnormalityWindowViewModel AbnormalVM { get; set; }
            public static ClassWindowViewModel ClassVM { get; set; }
            public static NotificationAreaViewModel NotificationAreaVM { get; set; }
            public static PlayerMenuViewModel PlayerMenuVM { get; set; }

            public static CivilUnrestViewModel CivilUnrestVM => _civilUnrestVm ?? (_civilUnrestVm = new CivilUnrestViewModel(App.Settings.CivilUnrestWindowSettings));

            public static DashboardViewModel DashboardVM => _dashboardVm ?? (_dashboardVm = new DashboardViewModel(null));

            public static LfgListViewModel LfgVM => _lfgVm ?? (_lfgVm = new LfgListViewModel(App.Settings.LfgWindowSettings));

            public static FlightGaugeViewModel FlightGaugeVM => _flightGaugeVm ?? (_flightGaugeVm = new FlightGaugeViewModel(App.Settings.FlightGaugeWindowSettings));
            public static FloatingButtonViewModel FloatingButtonVM => _floatingButtonVm ?? (_floatingButtonVm = new FloatingButtonViewModel(App.Settings.FloatingButtonSettings));

            public static GroupWindowViewModel GroupVM { get; set; }
        }


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
            VisibilityManager = new VisibilityManager();

            FocusManager.Init();

            await LoadWindows();

            TrayIcon = new TccTrayIcon();


            if (App.Settings.UseHotkeys) KeyboardHook.Instance.Enable();

            KeyboardHook.Instance.RegisterCallback(App.Settings.ToggleBoundariesHotkey, TccWidget.OnShowAllHandlesToggled);
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
        }




        private static void SystemEventsOnDisplaySettingsChanged(object sender, EventArgs e)
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


            FlightDurationWindow = new FlightDurationWindow(ViewModels.FlightGaugeVM);
            if (FlightDurationWindow.WindowSettings.Enabled) FlightDurationWindow.Show();

            CivilUnrestWindow = new CivilUnrestWindow(ViewModels.CivilUnrestVM);
            if (CivilUnrestWindow.WindowSettings.Enabled) CivilUnrestWindow.Show();

            FloatingButton = new FloatingButtonWindow(ViewModels.FloatingButtonVM);
            if (FloatingButton.WindowSettings.Enabled) FloatingButton.Show();

            DashboardWindow = new Dashboard(ViewModels.DashboardVM);
            LfgListWindow = new LfgListWindow(ViewModels.LfgVM);

            ViewModels.PlayerMenuVM = new PlayerMenuViewModel();

            ChatWindowManager.Start();

            SettingsWindow = new SettingsWindow();
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