using System;
using System.Windows.Threading;
using FoglioUtils;

namespace TCC.Data
{

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
                N();
            }
        }
        public StanceTracker()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
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
                N();
                AuraChanged?.Invoke();
            }
        }
        public bool ManaAura
        {
            get => _mp; set
            {
                if (_mp == value) return;
                _mp = value;
                N();
                AuraChanged?.Invoke();
            }
        }
        public bool CritResAura
        {
            get => _res; set
            {
                if (_res == value) return;
                _res = value;
                N();
                AuraChanged?.Invoke();
            }
        }
        public bool SwiftAura
        {
            get => _swift; set
            {
                if (_swift == value) return;
                _swift = value;
                N();
                AuraChanged?.Invoke();
            }
        }

        public bool AllMissing => !_crit && !_mp && !_res && !_swift;

        public event Action AuraChanged;

        public AurasTracker()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
