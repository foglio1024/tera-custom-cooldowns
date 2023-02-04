using Nostrum;
using Newtonsoft.Json;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Pc
{
    public class GuardianInfo : ThreadSafeObservableObject
    {
        public const float MaxCredits = 100000;
        public const int MaxDaily = 40;

        private int _credits;
        private int _cleared;
        private int _claimed;

        public int Credits
        {
            get => _credits;
            set
            {
                if (_credits == value) return;
                _credits = value;
                N(nameof(Credits));
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

        [JsonIgnore] public float CreditsFactor => (float)MathUtils.FactorCalc(Credits, MaxCredits);
        [JsonIgnore] public float ClearedFactor => (float)MathUtils.FactorCalc(Cleared, MaxDaily);
        [JsonIgnore] public float ClaimedFactor => (float)MathUtils.FactorCalc(Claimed, MaxDaily);
    }
}