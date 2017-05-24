using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TCC.Data
{
    public enum ArcherStance
    {
        None, SniperEye
    }
    public enum WarriorStance
    {
        None, Assault, Defensive
    }
    public class StanceTracker<T> : TSPropertyChanged where T : IComparable
    {
        T _currentStance;
        public T CurrentStance
        {
            get => _currentStance;
            set
            {
                if (_currentStance.CompareTo(value) == 0) return;
                _currentStance = value;
                NotifyPropertyChanged("CurrentStance");
            }
        }
        public StanceTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
