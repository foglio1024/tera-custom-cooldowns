using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public class Location
    {
        public uint World, Guard, Section;
        public Point Position;
        public Location(uint w, uint g, uint s, double x, double y)
        {
            World = w;
            Guard = g;
            Section = s;
            Position = new Point(x, y);
        }
        
    }
}
