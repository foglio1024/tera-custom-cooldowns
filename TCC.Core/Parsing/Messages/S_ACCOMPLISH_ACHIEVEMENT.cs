using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_ACCOMPLISH_ACHIEVEMENT : ParsedMessage
    {
        public uint AchievementId { get; private set; }
        public S_ACCOMPLISH_ACHIEVEMENT(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(16);
            AchievementId = reader.ReadUInt32();
        }
    }
}
