using System;
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
    public enum MysticAuras
    {
        None, Crit, Mana, CritRes, Swift
    }
    public class StanceTracker<T> : TSPropertyChanged where T : IComparable
    {
        private T _currentStance;
        public T CurrentStance
        {
            get => _currentStance;
            set
            {
                if (_currentStance.CompareTo(value) == 0) return;
                _currentStance = value;
                NotifyPropertyChanged(nameof(CurrentStance));
            }
        }
        public StanceTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
    public class AurasTracker : TSPropertyChanged
    {
        private bool _crit, _mp, _res, _swift;
        public bool CritAura
        {
            get => _crit; set
            {
                if (_crit == value) return;
                _crit = value;
                NotifyPropertyChanged("AuraChanged");
            }
        }
        public bool ManaAura
        {
            get => _mp; set
            {
                if (_mp == value) return;
                _mp = value;
                NotifyPropertyChanged("AuraChanged");
            }
        }
        public bool CritResAura
        {
            get => _res; set
            {
                if (_res == value) return;
                _res = value;
                NotifyPropertyChanged("AuraChanged");
            }
        }
        public bool SwiftAura
        {
            get => _swift; set
            {
                if (_swift == value) return;
                _swift = value;
                NotifyPropertyChanged("AuraChanged");
            }
        }

        public AurasTracker()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
