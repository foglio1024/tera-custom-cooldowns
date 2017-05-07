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
        private bool topMost;
        public bool TopMost
        {
            get => topMost;
            set
            {
                if (topMost != value)
                {
                    topMost = value;
                    RaisePropertyChanged("TopMost");
                }
            }
        }

        public BossGageWindowViewModel()
        {
            SessionManager.HhModeChanged += SessionManager_HhModeChanged;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    TopMost = false;
                    TopMost = true;
                }
            };

        }
    }
}
