using Newtonsoft.Json;
using Nostrum;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Pc;

public class GuardianInfo : ThreadSafeObservableObject
{
    public const float MaxCredits = 100000;
    public const int MaxDailies = 100;

    int _credits;
    int _cleared;
    int _claimed;

    public int Credits
    {
        get => _credits;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _credits)) return;
            InvokePropertyChanged(nameof(CreditsFactor));
        }
    }
    public int Cleared
    {
        get => _cleared;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _cleared)) return;
            InvokePropertyChanged(nameof(ClearedFactor));
        }
    }
    public int Claimed
    {
        get => _claimed;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _claimed)) return;
            InvokePropertyChanged(nameof(ClaimedFactor));
        }
    }
    [JsonIgnore] public float CreditsFactor => (float)MathUtils.FactorCalc(Credits, MaxCredits);
    [JsonIgnore] public float ClearedFactor => (float)MathUtils.FactorCalc(Cleared, MaxDailies);
    [JsonIgnore] public float ClaimedFactor => (float)MathUtils.FactorCalc(Claimed, MaxDailies);
}