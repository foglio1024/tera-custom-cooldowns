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

        public CooldownWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.CooldownWindow.Dispatcher.Invoke(() =>
                    {
                       WindowManager.CooldownWindow.Topmost = false;
                       WindowManager.CooldownWindow.Topmost = true;
                    });
                }
            };
        }
    }
}
