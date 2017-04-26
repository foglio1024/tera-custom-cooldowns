using System.Timers;
using TCC.Data;

namespace TCC
{
    public class AbnormalityDuration
    {
        public ulong Target { get; set; }
        public Abnormality Abnormality { get; set; }
        public int Duration { get; set; }
        public int Stacks { get; set; }
        public Timer timer;
        public int DurationLeft { get; set; }
        public AbnormalityDuration(Abnormality b, int d, int s, ulong t)
        {
            Abnormality = b;
            Duration = d;
            Stacks = s;
            Target = t;

            DurationLeft = d;
            timer = new Timer(1000);
            timer.Elapsed += (se, ev) => DurationLeft = DurationLeft - 1000;
            if (!Abnormality.Infinity)
            {
                timer.Start();
            }
        }
        ~AbnormalityDuration()
        {
            timer.Stop();
        }
    }

}
