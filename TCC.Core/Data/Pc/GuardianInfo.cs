using Nostrum;
using Newtonsoft.Json;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Pc;

public class GuardianInfo : ThreadSafeObservableObject
{
    public const float MAX_CREDITS = 100000;
    public const int MAX_DAILIES = 100;

    int _credits;
    int _cleared;
    int _claimed;

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
    public int Cleared
    {
        get => _cleared;
        set
        {
            if (_cleared == value) return;
            _cleared = value;
            N();
            N(nameof(ClearedFactor));
        }

    }
    public int Claimed
    {
        get => _claimed;
        set
        {
            if (_claimed == value) return;
            _claimed = value;
            N();
            N(nameof(ClaimedFactor));
        }
    }

    [JsonIgnore] public float CreditsFactor => (float)MathUtils.FactorCalc(Credits, MAX_CREDITS);
    [JsonIgnore] public float ClearedFactor => (float)MathUtils.FactorCalc(Cleared, MAX_DAILIES);
    [JsonIgnore] public float ClaimedFactor => (float)MathUtils.FactorCalc(Claimed, MAX_DAILIES);
}