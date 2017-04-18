using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC
{
    public class Character
    {
        public string Name { get; set; }
        public Class Class { get; set; }
        public Laurel Laurel { get; set; }
        public Character(string _name, Class c, Laurel l)
        {
            Name = _name;
            Class = c;
            Laurel = l;
        }
    }
}
