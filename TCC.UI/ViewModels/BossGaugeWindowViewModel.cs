using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BossGaugeWindowViewModel : BaseINPC
    {
        public ObservableCollection<Boss> CurrentNPCs
        {
            get
            {
                if (HarrowholdMode)
                {
                    return null;
                }
                else
                {
                    return EntitiesManager.CurrentBosses;
                }
            }
        }
        public bool HarrowholdMode
        {
            get => SessionManager.HarrowholdMode;            
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private void SessionManager_HhModeChanged(bool val)
        {
            RaisePropertyChanged("CurrentNPCs");
            RaisePropertyChanged("HarrowholdMode");
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

        public BossGaugeWindowViewModel()
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
