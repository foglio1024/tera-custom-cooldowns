using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public class SimpleUser
    {
        public uint PlayerId { get; }
        public string Name { get; }

        public SimpleUser(uint id, string name)
        {
            PlayerId = id;
            Name = name;
        }
    }
}
