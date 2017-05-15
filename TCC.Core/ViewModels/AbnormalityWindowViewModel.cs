using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCC.ViewModels
{
    public class AbnormalityWindowViewModel : BaseINPC
    {

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private double scale = SettingsManager.BuffBarWindowSettings.Scale;
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

        private FlowDirection direction = SettingsManager.BuffsDirection;
        public FlowDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                if (direction == value) return;
                direction = value;
                RaisePropertyChanged("Direction");
            }
        }

        public AbnormalityWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    BuffBarWindowManager.Instance.Dispatcher.Invoke(() =>
                    {
                        WindowManager.BuffBar.Topmost = false;
                        WindowManager.BuffBar.Topmost = true;
                    });
                }
            };
        }
    }
}
