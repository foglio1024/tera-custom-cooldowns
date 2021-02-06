using System;
using System.Windows.Threading;
using Nostrum;

namespace TCC.Data
{

    public class StanceTracker<T> : TSPropertyChanged where T : struct, IComparable
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
        public event Action? AuraChanged;

        private bool _crit, _mp, _res, _swift;
        public bool CritAura
        {
            get => _crit; set
            {
                if (_crit == value) return;
                _crit = value;
                N();
                N(nameof(OffenseAura));
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
                N(nameof(SupportAura));
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
                N(nameof(SupportAura));
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
                N(nameof(OffenseAura));
                AuraChanged?.Invoke();
            }
        }

        public bool AllMissing => !_crit && !_mp && !_res && !_swift;
        public bool OffenseAura => _crit || _swift;
        public bool SupportAura => _mp || _res;

        public AurasTracker()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
