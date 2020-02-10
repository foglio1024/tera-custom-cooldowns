using System;
using System.Windows.Input;
using Nostrum;
using TCC.Settings;
using TCC.Settings.WindowSettings;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public class FloatingButtonViewModel : TccWindowViewModel
    {
        public event Action NotificationsAdded;
        public event Action NotificationsCleared;
        private bool _pendingNotifications;
        private int _pendingNotificationsAmount;

        public bool PendingNotifications
        {
            get => _pendingNotifications;
            set
            {
                if (_pendingNotifications == value) return;
                _pendingNotifications = value;
                N();
            }
        }
        public int PendingNotificationsAmount
        {
            get => _pendingNotificationsAmount;
            set
            {
                if (_pendingNotificationsAmount == value) return;
                _pendingNotificationsAmount = value;
                N();
            }
        }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenLfgCommand { get; }
        public ICommand OpenDashboardCommand { get; }

        public FloatingButtonViewModel(FloatingButtonWindowSettings settings) : base(settings)
        {
            OpenSettingsCommand = new RelayCommand(_ => WindowManager.SettingsWindow.ShowWindow());
            OpenLfgCommand = new RelayCommand(_ => WindowManager.LfgListWindow.ShowWindow());
            OpenDashboardCommand = new RelayCommand(_ =>
            {
                WindowManager.DashboardWindow.ShowWindow();
                PendingNotifications = false;
                PendingNotificationsAmount = 0;
                NotificationsCleared?.Invoke();
            });
        }

        public void WarnCloseEvents(int closeEventsCount)
        {
            PendingNotificationsAmount = closeEventsCount;
            PendingNotifications = true;
            NotificationsAdded?.Invoke();
        }
    }
}