using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
