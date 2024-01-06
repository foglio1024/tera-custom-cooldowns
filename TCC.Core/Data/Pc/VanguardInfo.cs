using Newtonsoft.Json;
using Nostrum;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Pc;

public class VanguardInfo : ThreadSafeObservableObject
{
    public const float MAX_CREDITS = 100000;
    public const int MAX_DAILIES = 8;

    int _credits;
    int _dailiesDone;
    int _weekliesDone;
    int _weekliesMax;

    public int Credits
    {
        get => _credits;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _credits)) return;
            InvokePropertyChanged(nameof(CreditsFactor));
        }
    }
    public int DailiesDone
    {
        get => _dailiesDone;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _dailiesDone)) return;
            InvokePropertyChanged(nameof(DailyCompletion));
        }
    }
    public int WeekliesDone
    {
        get => _weekliesDone;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _weekliesDone)) return;
            InvokePropertyChanged(nameof(WeeklyCompletion));

        }
    }
    public int WeekliesMax
    {
        get => _weekliesMax;
        set
        {
            if (!RaiseAndSetIfChanged(value, ref _weekliesMax)) return;
            InvokePropertyChanged(nameof(WeeklyCompletion));
        }
    }
    [JsonIgnore] 
    public float CreditsFactor => (float)MathUtils.FactorCalc(Credits, MAX_CREDITS);
    [JsonIgnore] 
    public float DailyCompletion => (float)MathUtils.FactorCalc(DailiesDone, MAX_DAILIES);
    [JsonIgnore] 
    public float WeeklyCompletion => (float)MathUtils.FactorCalc(WeekliesDone, WeekliesMax);
}