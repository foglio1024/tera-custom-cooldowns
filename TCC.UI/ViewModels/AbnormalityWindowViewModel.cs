using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.ViewModels
{
    public class AbnormalityWindowViewModel : BaseINPC
    {

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
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
