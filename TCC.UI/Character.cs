using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.UI
{
    class Character
    {
        public string Name { get; set; }
        public Class Class { get; set; }
        public Character(string _name, Class c)
        {
            Name = _name;
            Class = c;
        }
    }
}
