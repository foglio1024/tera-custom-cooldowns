using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class GroupWindowViewModel : BaseINPC
    {

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private double scale = SettingsManager.GroupWindowSettings.Scale;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale == value) return;
                scale = value;
                RaisePropertyChanged("Scale");
            }
        }
        public GroupWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    GroupWindowManager.Instance.Dispatcher.Invoke(() =>
                    {
                        WindowManager.GroupWindow.Topmost = false;
                        WindowManager.GroupWindow.Topmost = true;
                    });
                }
            };
        }
    }
}
