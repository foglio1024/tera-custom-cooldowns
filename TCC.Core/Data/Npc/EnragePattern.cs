namespace TCC.Data.Npc
{
    public class EnragePattern
    {
        private readonly float _flatHp;

        public double Percentage { get; set; }
        public int Duration { get; set; }
        public bool StaysEnraged { get; set; }
        public EnragePattern()
        {
            Percentage = 10;
            Duration = 36;
            _flatHp = -1;
        }
        public EnragePattern(double percentage, int duration)
        {
            Percentage = percentage;
            Duration = duration;
            _flatHp = -1;
        }
        public EnragePattern(long maxHp, long flatHp, int duration)
        {
            Duration = duration;
            _flatHp = flatHp;
            Update(maxHp);
        }


        internal void Update(double maxHp)
        {
            if (_flatHp == -1) return;
            Percentage = _flatHp / maxHp * 100;
        }
    }
}
