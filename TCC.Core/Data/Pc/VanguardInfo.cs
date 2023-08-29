using Nostrum;
using Newtonsoft.Json;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Pc;

public class VanguardInfo : ThreadSafeObservableObject
{
    public const float MAX_CREDITS = 100000;
    public const int MAX_DAILIES = 8;

    private int _credits;
    private int _dailiesDone;
    private int _weekliesDone;
    private int _weekliesMax;

    public int Credits
    {
        get => _credits;
        set
        {
            if (_credits == value) return;
            _credits = value;
            N();
            N(nameof(CreditsFactor));
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
            N(nameof(DailyCompletion));
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
            N(nameof(WeeklyCompletion));

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
            N(nameof(WeeklyCompletion));
        }
    }

    [JsonIgnore] public float CreditsFactor => (float)MathUtils.FactorCalc(Credits, MAX_CREDITS);
    [JsonIgnore] public float DailyCompletion => (float)MathUtils.FactorCalc(DailiesDone, MAX_DAILIES);
    [JsonIgnore] public float WeeklyCompletion => (float)MathUtils.FactorCalc(WeekliesDone, WeekliesMax);

}