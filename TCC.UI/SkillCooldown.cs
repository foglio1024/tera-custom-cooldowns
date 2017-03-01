using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace TCC.UI
{
    public class SkillCooldown
    {
        public uint Id { get; set; }
        public uint Cooldown { get; set; }
        public string Name { get; set; }
        public Timer Timer { get; set; }

        public SkillCooldown(uint id, uint cd)
        {
            Id = id;
            Cooldown = cd;
            if(cd != 0)
            {
                Timer = new Timer(Cooldown);
            }
        }     
    }
}
