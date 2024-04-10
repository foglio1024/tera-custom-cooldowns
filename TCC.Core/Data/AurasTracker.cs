using System;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data;

public class AurasTracker : ThreadSafeObservableObject
{
    public event Action? AuraChanged;

    private bool _crit, _mp, _res, _swift;
    public bool CritAura
    {
        get => _crit; set
        {
            if (!RaiseAndSetIfChanged(value, ref _crit)) return;

            InvokePropertyChanged(nameof(OffenseAura));
            AuraChanged?.Invoke();
        }
    }
    public bool ManaAura
    {
        get => _mp; set
        {
            if (!RaiseAndSetIfChanged(value, ref _mp)) return;

            InvokePropertyChanged(nameof(SupportAura));
            AuraChanged?.Invoke();
        }
    }
    public bool CritResAura
    {
        get => _res; set
        {
            if (!RaiseAndSetIfChanged(value, ref _res)) return;

            InvokePropertyChanged(nameof(SupportAura));
            AuraChanged?.Invoke();
        }
    }
    public bool SwiftAura
    {
        get => _swift; set
        {
            if (!RaiseAndSetIfChanged(value, ref _swift)) return;

            InvokePropertyChanged(nameof(OffenseAura));
            AuraChanged?.Invoke();
        }
    }
    public bool AllMissing => !_crit && !_mp && !_res && !_swift;
    public bool OffenseAura => _crit || _swift;
    public bool SupportAura => _mp || _res;
}