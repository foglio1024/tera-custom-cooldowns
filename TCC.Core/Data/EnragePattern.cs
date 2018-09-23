using System;

namespace TCC.Data
{
    public class EnragePattern
    {
        private readonly float _flatHp;

        public double Percentage { get; set; }
        public int Duration { get; set; }
        public EnragePattern()
        {
            Percentage = 10;
            Duration = 36;
        }
        public EnragePattern(double percentage, int duration)
        {
            Percentage = percentage;
            Duration = duration;
        }
        public EnragePattern(long maxHp, long flatHp, int duration)
        {
            Duration = duration;
            _flatHp = flatHp;
            Update(maxHp);
        }


        internal void Update(float maxHp)
        {
            Percentage = (_flatHp / (float)maxHp) * 100;
        }
    }
}
