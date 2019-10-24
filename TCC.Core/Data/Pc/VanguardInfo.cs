using FoglioUtils;
using Newtonsoft.Json;

namespace TCC.Data.Pc
{
    public class VanguardInfo : TSPropertyChanged
    {
        public const float MaxCredits = 20000;
        public const int MaxDaily = 16;
        public const int MaxWeekly = 16;

        private int _credits;
        private int _dailiesDone;
        private int _weekliesDone;

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
        public int DailiesDone
        {
            get => _dailiesDone;
            set
            {
                if (_dailiesDone == value) return;
                _dailiesDone = value;
                N(nameof(DailiesDone));
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
                N(nameof(WeekliesDone));
                N(nameof(WeeklyCompletion));

            }
        }

        [JsonIgnore] public float CreditsFactor => (float)MathUtils.FactorCalc(Credits, MaxCredits);
        [JsonIgnore] public float DailyCompletion => (float)MathUtils.FactorCalc(DailiesDone, MaxDaily);
        [JsonIgnore] public float WeeklyCompletion => (float)MathUtils.FactorCalc(WeekliesDone, MaxWeekly);

    }
}