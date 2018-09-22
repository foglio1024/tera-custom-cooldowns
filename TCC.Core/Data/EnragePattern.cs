namespace TCC.Data
{
    public class EnragePattern
    {
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
            Percentage = (flatHp / (float)maxHp) * 100;
        }
    }
}
