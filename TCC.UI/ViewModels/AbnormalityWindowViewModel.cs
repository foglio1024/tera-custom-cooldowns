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
        public ObservableCollection<AbnormalityDuration> Buffs
        {
            get => SessionManager.CurrentPlayer.Buffs;
        }
        public ObservableCollection<AbnormalityDuration> Debuffs
        {
            get => SessionManager.CurrentPlayer.Debuffs;
        }
        public ObservableCollection<AbnormalityDuration> InfiniteBuffs
        {
            get => SessionManager.CurrentPlayer.InfBuffs;
        }

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }

        public AbnormalityWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) => RaisePropertyChanged("IsTeraOnTop");
        }
    }
}
