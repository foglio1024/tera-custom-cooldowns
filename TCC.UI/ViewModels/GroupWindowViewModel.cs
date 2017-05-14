using System;
using System.Collections.Generic;
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
