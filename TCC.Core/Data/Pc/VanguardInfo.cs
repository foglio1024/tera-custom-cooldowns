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
            if (_credits == value) return;
            _credits = value;
            N();
            InvokePropertyChanged(nameof(CreditsFactor));
        }
    }
    public int DailiesDone
    {
        get => _dailiesDone;
        set
        {
            if (_dailiesDone == value) return;
            _dailiesDone = value;
            N();
            InvokePropertyChanged(nameof(DailyCompletion));
        }
    }
    public int WeekliesDone
    {
        get => _weekliesDone;
        set
        {
            if (_weekliesDone == value) return;
            _weekliesDone = value;
            N();
            InvokePropertyChanged(nameof(WeeklyCompletion));

        }
    }
    public int WeekliesMax
    {
        get => _weekliesMax;
        set
        {
            if (_weekliesMax == value) return;
            _weekliesMax = value;
            N();
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