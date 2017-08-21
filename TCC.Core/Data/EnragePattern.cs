namespace TCC.Data
{
    public class EnragePattern
    {
        public double Percentage { get; set; }
        public int Duration { get; set; }
        public EnragePattern(double p, int d)
        {
            Percentage = p;
            Duration = d;
        }
    }
}
