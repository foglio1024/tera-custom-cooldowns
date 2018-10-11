using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_CHANGE_GUILD_CHIEF : ParsedMessage
    {
        public uint PlayerId { get; set; }

        public S_CHANGE_GUILD_CHIEF(TeraMessageReader reader) : base(reader)
        {
            PlayerId = reader.ReadUInt32();
        }
    }
}
