using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class CooldownWindowViewModel : BaseINPC
    {

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

        public CooldownWindowViewModel()
        {
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
