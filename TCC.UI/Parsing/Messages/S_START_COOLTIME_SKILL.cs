using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Messages
{
    public class S_START_COOLTIME_SKILL : ParsedMessage
    {
        /// <summary>
        /// Skill's ID
        /// </summary>
        public uint SkillId { get; set; }

        /// <summary>
        /// Skill's cooldown in milliseconds
        /// </summary>
        public uint Cooldown { get; set; }

        public S_START_COOLTIME_SKILL(TeraMessageReader reader) : base(reader)
        {
            SkillId = reader.ReadUInt32() - 0x04000000;
            Cooldown = reader.ReadUInt32();
        }
    }
}
