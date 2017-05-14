using System;

namespace TCC.ViewModels
{
    public class BossGageWindowViewModel : BaseINPC
    {
        public bool HarrowholdMode
        {
            get => SessionManager.HarrowholdMode;
        }
        private void SessionManager_HhModeChanged(bool val)
        {
            RaisePropertyChanged("CurrentNPCs");
            RaisePropertyChanged("HarrowholdMode");
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }

        public BossGageWindowViewModel()
        {
            SessionManager.HhModeChanged += SessionManager_HhModeChanged;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    BossGageWindowManager.Instance.Dispatcher.Invoke(() =>
                    {
                        WindowManager.BossGauge.Topmost = false;
                        WindowManager.BossGauge.Topmost = true;
                    });
                }
            };

        }
    }
}
